using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class AddForcedTraitsScenario : SimScenario
    {
        public AddForcedTraitsScenario()
        { }
        protected AddForcedTraitsScenario(AddForcedTraitsScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "AddForcedTraits";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.TraitManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (!Sims.Allow(this, sim, Managers.Manager.AllowCheck.None))
            {
                IncStat("User Denied");
                return false;
            }
            else if (sim.AgingState == null)
            {
                IncStat("No AgingState");
                return false;
            }
            else if (!HasAnyValue<ForceTraitOption,TraitNames>(sim))
            {
                IncStat("Unnecessary");
                return false;
            }
            else if (sim.IsEP11Bot)
            {
                IncStat("Plumbot");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            foreach (Trait trait in new List<Trait>(Sim.TraitManager.List))
            {
                if (trait.IsReward) continue;

                if (HasValue<ForceTraitOption, TraitNames>(Sim, trait.Guid))
                {
                    IncStat("Forced Trait Found");
                    return false;
                }
            }

            IncStat("Necessary");

            TraitNames removedTrait = TraitNames.Unknown;

            int available = Sim.TraitManager.NumTraitsForAge() - Sim.TraitManager.CountVisibleTraits();

            if (available == 0)
            {
                foreach (Trait trait in new List<Trait>(Sim.TraitManager.List))
                {
                    if (HasValue<DisallowTraitOption, TraitNames>(Sim, trait.Guid))
                    {
                        Sim.TraitManager.RemoveElement(trait.Guid);

                        if ((trait.IsHidden) || (trait.IsReward)) continue;

                        removedTrait = trait.Guid;

                        IncStat("Disallowed Trait Removed");
                        available = 1;
                        break;
                    }
                }
            }

            if (available <= 0)
            {
                List<Trait> traits = new List<Trait>(Sim.TraitManager.List);
                RandomUtil.RandomizeListOfObjects(traits);

                foreach(Trait trait in traits)
                {
                    if ((trait.IsHidden) || (trait.IsReward)) continue;

                    Sim.TraitManager.RemoveElement(trait.Guid);

                    removedTrait = trait.Guid;

                    IncStat("Removed One");
                    break;
                }
            }

            List<TraitNames> choices = new List<TraitNames>(GetValue<ForceTraitOption, List<TraitNames>>(Sim));
            RandomUtil.RandomizeListOfObjects(choices);

            bool readdTrait = true;
            bool result = false;

            foreach(TraitNames traitName in choices)
            {
                Trait trait = TraitManager.GetTraitFromDictionary(traitName);
                if (trait == null) continue;

                if (!trait.TraitValidForAgeSpecies(Sim.GetCASAGSAvailabilityFlags())) continue;

                bool success = false;

                if (trait.IsHidden)
                {
                    success = Sim.TraitManager.AddElement(traitName, true);
                }
                else
                {
                    success = Sim.AddTrait(trait);
                }

                if (success)
                {
                    IncStat("Success");

                    result = true;
                    GetData(Sim).InvalidateCache();

                    if ((!trait.IsHidden) && (!trait.IsReward))
                    {
                        readdTrait = false;
                    }
                    break;
                }
            }

            if ((readdTrait) && (removedTrait != TraitNames.Unknown))
            {
                Sim.AddTrait(TraitManager.GetTraitFromDictionary(removedTrait));

                IncStat("Trait Readded");
            }

            return result;
        }

        public override Scenario Clone()
        {
            return new AddForcedTraitsScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSim, AddForcedTraitsScenario>, ManagerSim.ITraitsLTWOption, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AddForcedTraits";
            }
        }
    }
}
