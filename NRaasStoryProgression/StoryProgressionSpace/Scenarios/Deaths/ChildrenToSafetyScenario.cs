using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Deaths
{
    public class ChildrenToSafetyScenario : HouseholdScenario
    {
        public ChildrenToSafetyScenario(Household house)
            : base(house)
        { }
        protected ChildrenToSafetyScenario(ChildrenToSafetyScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ChildrenToSafety";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            if (SimTypes.IsService(house))
            {
                IncStat("Service");
                return false;
            }

            bool bAdult = false, bChild = false;

            foreach (SimDescription sim in house.AllSimDescriptions)
            {
                if (Deaths.IsDying(sim)) continue;

                if (Households.AllowGuardian(sim))
                {
                    bAdult = true;
                }
                else
                {
                    bChild = true;
                }
            }

            if ((bAdult) || (!bChild))
            {
                IncStat("Unnecessary");
                return false;
            }

            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new ChildSaveSplitFamilyScenario(House, true), ScenarioResult.Start);
            Add(frame, new ChildSaveSplitFamilyScenario(House, false), ScenarioResult.Start);
            Add(frame, new PostScenario(House), ScenarioResult.Start);
            Add(frame, new AbandonedChildScenario(House), ScenarioResult.Start);
            return false;
        }

        public override Scenario Clone()
        {
            return new ChildrenToSafetyScenario(this);
        }

        protected class PostScenario : DualSimScenario
        {
            Household House;

            public PostScenario(Household house)
            {
                House = house;
            }
            protected PostScenario(PostScenario scenario)
                : base (scenario)
            {
                House = scenario.House;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;

                return "ChildrenToSafetyPost";
            }

            protected override bool CheckBusy
            {
                get { return false; }
            }

            protected override bool Progressed
            {
                get { return true; }
            }

            protected override int ContinueChance
            {
                get { return 100; }
            }

            protected override ICollection<SimDescription> GetSims()
            {
                return HouseholdsEx.All(House);
            }

            protected override ICollection<SimDescription> GetTargets(SimDescription sim)
            {
                return Sims.All;
            }

            protected override bool AllowSpecies(SimDescription sim)
            {
                return true;
            }

            protected override bool AllowSpecies(SimDescription sim, SimDescription target)
            {
                return target.IsHuman;
            }

            protected override bool Allow(SimDescription sim)
            {
                if (Households.AllowGuardian(sim))
                {
                    IncStat("Too Old");
                    return false;
                }
                /*
                else if (!Households.Allow(this, sim))
                {
                    IncStat("User Denied");
                    return false;
                }*/
                else if (Deaths.IsDying(sim))
                {
                    IncStat("Dying");
                    return false;
                }

                return base.Allow(sim);
            }

            protected override bool TargetAllow(SimDescription sim)
            {
                if (Sim.Household == Target.Household)
                {
                    IncStat("Same");
                    return false;
                }
                else if (sim.Household == null)
                {
                    IncStat("No Home");
                    return false;
                }
                else if (SimTypes.IsSpecial(sim))
                {
                    IncStat("Special");
                    return false;
                }
                else if (HouseholdsEx.IsFull(sim.Household, sim.IsPet, GetValue<MaximumSizeOption,int>(sim.Household)))
                {
                    IncStat("Full");
                    return false;
                }

                return base.TargetAllow(sim);
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                Add(frame, new ChildSaveForceMoveInScenario(Sim, Target), ScenarioResult.Start);
                return false;
            }

            public override Scenario Clone()
            {
                return new PostScenario(this);
            }
        }

        protected class ChildSaveForceMoveInScenario : MoveInScenario
        {
            public ChildSaveForceMoveInScenario(SimDescription going, SimDescription moveInWith)
                : base(going, moveInWith)
            { }
            protected ChildSaveForceMoveInScenario(ChildSaveForceMoveInScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "ChildSaveForceMoveIn";
                }
                else
                {
                    return "ChildSave";
                }
            }

            protected override ManagerLot.FindLotFlags Inspect
            {
                get { return ManagerLot.FindLotFlags.None; }
            }

            protected override bool TestAllow
            {
                get { return false; }
            }

            public override Scenario Clone()
            {
                return new ChildSaveForceMoveInScenario(this);
            }
        }

        public class Option : BooleanManagerOptionItem<ManagerDeath>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ChildrenToSafety";
            }
        }
    }
}
