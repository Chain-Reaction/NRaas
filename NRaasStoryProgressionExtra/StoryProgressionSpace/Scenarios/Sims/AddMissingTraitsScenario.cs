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
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class AddMissingTraitsScenario : AgeUpBaseScenario
    {
        public AddMissingTraitsScenario()
        { }
        protected AddMissingTraitsScenario(AddMissingTraitsScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "AddMissingTraits";
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (!GetValue<OptionV2,bool>()) return false;

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

            return base.Allow(sim);
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return sim.IsHuman;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            bool changed = false;

            foreach (Trait trait in new List<Trait>(Sim.TraitManager.List))
            {
                bool remove = false;

                if ((Sim.IsEP11Bot) && (!trait.IsHidden) && (!ActionData.IsBotSpecificTrait(trait.Guid)))
                {
                    remove = true;
                }

                if (HasValue<DisallowTraitOption, TraitNames>(Sim, trait.Guid)) 
                {
                    remove = true;
                }

                if (remove)
                {
                    Sim.TraitManager.RemoveElement(trait.Guid);

                    IncStat("Disallowed Trait Removed");

                    changed = true;
                }
            }

            if (changed)
            {
                GetData(Sim).InvalidateCache();
            }

            if (Sim.IsEP11Bot)
            {
                IncStat("Plumbot");
                return true;
            }

            int required = Sim.TraitManager.NumTraitsForAge() - Sim.TraitManager.CountVisibleTraits();

            AddStat("Required", required);

            if (required <= 0)
            {
                if (GetValue<RemoveTraitsOption, bool>())
                {
                    List<Trait> traits = new List<Trait>(Sim.TraitManager.List.Count);

                    for (int i = traits.Count - 1; i >= 0; i--)
                    {
                        if (Sim.TraitManager.CountVisibleTraits() <= Sim.TraitManager.NumTraitsForAge()) break;

                        if ((traits[i].IsHidden) || (traits[i].IsReward)) continue;

                        Sim.TraitManager.RemoveElement(traits[i].Guid);
                        changed = true;

                        IncStat("Removed Excess");
                    }

                    if (changed)
                    {
                        GetData(Sim).InvalidateCache();
                    }
                    return true;
                }
                else
                {
                    IncStat("Remove Disabled");
                    return false;
                }
            }

            AddStat("Add", required);

            SimDescription mom = null, dad = null;
            Relationships.GetParents(Sim, out mom, out dad);

            if (mom != null)
            {
                if ((!SimTypes.IsDead(mom)) &&
                    (mom.TraitManager != null) &&
                    (!mom.TraitManager.TraitsMaxed()))
                {
                    IncStat("Hold For Parent");
                    return false;
                }
            }

            if (dad != null)
            {
                if ((!SimTypes.IsDead(dad)) &&
                    (dad.TraitManager != null) &&
                    (!dad.TraitManager.TraitsMaxed()))
                {
                    IncStat("Hold For Parent");
                    return false;
                }
            }

            try
            {
                Sims3.Gameplay.CAS.Genetics.AssignTraits(Sim, dad, mom, false, 0, new System.Random());
                IncStat("Success");

                GetData(Sim).InvalidateCache();
                return true;
            }
            catch (Exception e)
            {
                Common.DebugException(Sim, e);
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new AddMissingTraitsScenario(this);
        }

        public class OptionV2 : BooleanScenarioOptionItem<ManagerSim, AddMissingTraitsScenario>, ManagerSim.ITraitsLTWOption
        {
            public OptionV2()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AddMissingTraits";
            }

            public override bool Install(ManagerSim main, bool initial)
            {
                if (initial)
                {
                    // Fix for EA error
                    Trait trait;
                    if (TraitManager.sDictionary.TryGetValue((ulong)TraitNames.ImaginaryFriendHiddenTrait, out trait))
                    {
                        trait.mNonPersistableData.mCanBeLearnedRandomly = false;
                    }
                }

                return base.Install(main, initial);
            }
        }

        public class AgeUpOption : BooleanEventOptionItem<ManagerSim, AddMissingTraitsScenario>, ManagerSim.ITraitsLTWOption, IDebuggingOption
        {
            public AgeUpOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AddMissingTraitsAgeUp";
            }
        }

        public class RemoveTraitsOption : BooleanManagerOptionItem<ManagerSim>, ManagerSim.ITraitsLTWOption, IDebuggingOption
        {
            public RemoveTraitsOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "RemoveTraits";
            }
        }
    }
}
