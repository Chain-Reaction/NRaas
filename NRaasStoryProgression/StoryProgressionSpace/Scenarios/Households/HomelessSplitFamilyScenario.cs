using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public class HomelessSplitFamilyScenario : SplitFamilyScenario
    {
        public HomelessSplitFamilyScenario(Household house)
            : base(house)
        { }
        protected HomelessSplitFamilyScenario(HomelessSplitFamilyScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;
            
            return "HomelessSplitFamily";
        }

        protected override bool OnlyChildren
        {
            get { return false; }
        }

        protected override SplitMoveInScenario GetMoveInScenario(SplitFamilyScenario.SplitHome newHome)
        {
            return new HomelessSplitMoveInScenario(Sim, newHome);
        }

        protected override SplitMoveOutScenario GetMoveOutScenario(SplitFamilyScenario.SplitHome newHome)
        {
            return new HomelessSplitMoveOutScenario(Sim, newHome);
        }

        public override Scenario Clone()
        {
            return new HomelessSplitFamilyScenario(this);
        }

        protected class HomelessSplitMoveInScenario : SplitMoveInScenario
        {
            public HomelessSplitMoveInScenario(SimDescription sim, SplitFamilyScenario.SplitHome newHome)
                : base(sim, newHome)
            {}
            public HomelessSplitMoveInScenario(HomelessSplitMoveInScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "HomelessSplitMoveIn";
                }
                else
                {
                    return "MoveIn";
                }
            }

            protected override ManagerLot.FindLotFlags Inspect
            {
                get 
                {
                    if (GetValue<ManagerHousehold.HomelessInspectionOption, bool>())
                    {
                        return ManagerLot.FindLotFlags.Inspect;
                    }
                    else
                    {
                        return ManagerLot.FindLotFlags.None;
                    }
                }
            }

            public override Scenario Clone()
            {
                return new HomelessSplitMoveInScenario(this);
            }
        }

        protected class HomelessSplitMoveOutScenario : SplitMoveOutScenario
        {
            public HomelessSplitMoveOutScenario(SimDescription sim, SplitFamilyScenario.SplitHome newHome)
                : base(sim, newHome)
            { }
            public HomelessSplitMoveOutScenario(HomelessSplitMoveOutScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;

                return "HomelessSplitMoveOut";
            }

            protected override HouseholdBreakdown.ChildrenMove ChildMove
            {
                get { return HouseholdBreakdown.ChildrenMove.Go; }
            }

            protected override MoveInLotScenario GetMoveInLotScenario(List<SimDescription> going)
            {
                if (Manager.GetValue<NewcomerGoneScenario.AllowHomelessMoveInOptionV2, bool>())
                {
                    return new HomelessMoveInLotScenario(going);
                }
                else
                {
                    return null;
                }
            }

            protected override ScoredMoveInScenario GetMoveInScenario(List<SimDescription> going)
            {
                return new HomelessMoveInScenario(Sim, going);
            }

            public override Scenario Clone()
            {
                return new HomelessSplitMoveOutScenario(this);
            }
        }
    }
}
