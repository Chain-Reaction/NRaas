using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
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
    public class ChildSaveSplitFamilyScenario : SplitFamilyScenario
    {
        bool mOnlyFamilyMoveIn = false;

        public ChildSaveSplitFamilyScenario(Household house, bool onlyFamilyMoveIn)
            : base(house)
        { 
            mOnlyFamilyMoveIn = onlyFamilyMoveIn;
        }
        protected ChildSaveSplitFamilyScenario(ChildSaveSplitFamilyScenario scenario)
            : base(scenario)
        { 
            mOnlyFamilyMoveIn = scenario.mOnlyFamilyMoveIn;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ChildSaveSplitFamily";
        }

        protected override bool OnlyChildren
        {
            get { return true; }
        }

        protected override SplitMoveInScenario GetMoveInScenario(SplitFamilyScenario.SplitHome newHome)
        {
            return new ChildSaveSplitMoveInScenario(Sim, newHome);
        }

        protected override SplitMoveOutScenario GetMoveOutScenario(SplitFamilyScenario.SplitHome newHome)
        {
            return new ChildSaveSplitMoveOutScenario(Sim, newHome, mOnlyFamilyMoveIn);
        }

        public override Scenario Clone()
        {
            return new ChildSaveSplitFamilyScenario(this);
        }

        protected class ChildSaveSplitMoveInScenario : SplitMoveInScenario
        {
            public ChildSaveSplitMoveInScenario(SimDescription sim, SplitFamilyScenario.SplitHome newHome)
                : base(sim, newHome)
            { }
            public ChildSaveSplitMoveInScenario(ChildSaveSplitMoveInScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "ChildSaveSplitMoveIn";
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
                return new ChildSaveSplitMoveInScenario(this);
            }
        }

        protected class ChildSaveSplitMoveOutScenario : SplitMoveOutScenario
        {
            bool mOnlyFamilyMoveIn = false;

            public ChildSaveSplitMoveOutScenario(SimDescription sim, SplitFamilyScenario.SplitHome newHome, bool onlyFamilyMoveIn)
                : base(sim, newHome)
            {
                mOnlyFamilyMoveIn = onlyFamilyMoveIn;
            }
            public ChildSaveSplitMoveOutScenario(ChildSaveSplitMoveOutScenario scenario)
                : base(scenario)
            {
                mOnlyFamilyMoveIn = scenario.mOnlyFamilyMoveIn;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;
                
                return "ChildSaveSplitMoveOut";
            }

            protected override HouseholdBreakdown.ChildrenMove ChildMove
            {
                get { return HouseholdBreakdown.ChildrenMove.Go; }
            }

            protected override MoveInLotScenario GetMoveInLotScenario(List<SimDescription> going)
            {
                return null;
            }

            protected override ScoredMoveInScenario GetMoveInScenario(List<SimDescription> going)
            {
                return new ChildSaveMoveInScenario(Sim, going, mOnlyFamilyMoveIn);
            }

            public override Scenario Clone()
            {
                return new ChildSaveSplitMoveOutScenario(this);
            }
        }

        public class ChildSaveMoveInScenario : ScoredMoveInScenario
        {
            bool mOnlyFamilyMoveIn = false;

            public ChildSaveMoveInScenario(SimDescription sim, List<SimDescription> going, bool onlyFamilyMoveIn)
                : base(sim, going)
            {
                mOnlyFamilyMoveIn = onlyFamilyMoveIn;
            }
            protected ChildSaveMoveInScenario(ChildSaveMoveInScenario scenario)
                : base(scenario)
            {
                mOnlyFamilyMoveIn = scenario.mOnlyFamilyMoveIn;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "ChildSaveMoveIn";
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

            protected override bool OnlyFamilyMoveIn
            {
                get { return mOnlyFamilyMoveIn; }
            }

            public override Scenario Clone()
            {
                return new ChildSaveMoveInScenario(this);
            }
        }
    }
}
