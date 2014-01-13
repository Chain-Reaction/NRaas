using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class NewImmigrantScenario : ImmigrantScenario
    {
        bool mReport = true;

        public NewImmigrantScenario(List<SimDescription> immigrants, ManagerLot.ImmigrationRequirement requirement)
            : base(immigrants, requirement)
        { }
        protected NewImmigrantScenario(NewImmigrantScenario scenario)
            : base (scenario)
        {
            mReport = scenario.mReport;
            scenario.mReport = false;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "NewImmigrant";
        }

        protected override bool ShouldReport
        {
            get { return mReport; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new ImmigrantCareerScenario(Sim, mRequirement), ScenarioResult.Start);

            if ((mRequirement.mFertile) && (Sim.TeenOrAbove) && (RandomUtil.RandomChance(25)))
            {
                Trait trait = TraitManager.GetTraitFromDictionary(TraitNames.FamilyOriented);
                if (trait != null)
                {
                    Sim.RemoveTrait(TraitManager.GetTraitFromDictionary(TraitNames.DislikesChildren));

                    List<Trait> traits = new List<Trait>(Sim.TraitManager.List);
                    if ((traits.Count > 0) && (traits.Count >= Sim.TraitManager.NumTraitsForAge()))
                    {
                        Sim.RemoveTrait(RandomUtil.GetRandomObjectFromList(traits));
                    }

                    Sim.AddTrait(trait);
                }
            }

            if ((mRequirement.mSingle) && (Sim.TeenOrAbove) && (RandomUtil.RandomChance(25)))
            {
                Trait trait = TraitManager.GetTraitFromDictionary(TraitNames.Flirty);
                if (trait != null)
                {
                    Sim.RemoveTrait(TraitManager.GetTraitFromDictionary(TraitNames.Unflirty));

                    List<Trait> traits = new List<Trait>(Sim.TraitManager.List);
                    if ((traits.Count > 0) && (traits.Count >= Sim.TraitManager.NumTraitsForAge()))
                    {
                        Sim.RemoveTrait(RandomUtil.GetRandomObjectFromList(traits));
                    }

                    Sim.AddTrait(trait);
                }
            }

            foreach (SimDescription other in mImmigrants)
            {
                if (Sim == other) continue;

                Relationship relation = Relationship.Get(Sim, other, true);
                if (relation == null) continue;

                relation.LTR.SetLiking(RandomUtil.GetFloat(25, 100));
            }

            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Lots;
            }

            if (parameters == null)
            {
                if (Sim.Household == null) return null;

                return Stories.PrintStory(manager, name, Sim.Household, extended, logging);
            }
            else
            {
                return base.PrintStory(manager, name, parameters, extended, logging);
            }
        }

        public override Scenario Clone()
        {
            return new NewImmigrantScenario(this);
        }
    }
}
