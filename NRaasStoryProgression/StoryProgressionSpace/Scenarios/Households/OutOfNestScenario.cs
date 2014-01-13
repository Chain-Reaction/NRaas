using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
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
    public class OutOfNestScenario : HouseholdScenario
    {
        bool mPassedInspection = false;

        public OutOfNestScenario()
        { }
        protected OutOfNestScenario(OutOfNestScenario scenario)
            : base (scenario)
        {
            mPassedInspection = scenario.mPassedInspection;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "OutOfNest";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (GetValue<RoomToLeaveOption, int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            if (!Households.Allow(this, house, 0))
            {
                IncStat("User Denied");
                return false;
            }

            mPassedInspection = Lots.PassesHomeInspection(this, House.LotHome, HouseholdsEx.Humans(House), ManagerLot.FindLotFlags.Inspect);

            if ((mPassedInspection) && ((HouseholdsEx.NumHumansIncludingPregnancy(House) + GetValue<RoomToLeaveOption, int>()) < GetValue<MaximumSizeOption, int>(House)))
            {
                IncStat("Score Fail");
                return false;
            }

            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            ScoringList<SimDescription> scoring = new ScoringList<SimDescription>();

            SimDescription head = SimTypes.HeadOfFamily(House);

            foreach (SimDescription sim in HouseholdsEx.All(House))
            {
                if (!Households.AllowSoloMove(sim)) continue;

                if (head != null)
                {
                    if (head == sim) continue;

                    if (head.Partner == sim) continue;
                }

                scoring.Add(sim, AddScoring("FindOwnHome", sim, sim.Partner));
            }

            List<SimDescription> best = scoring.GetBestByMinScore(1);
            if ((best == null) || (best.Count == 0))
            {
                IncStat("No Choices");
                return false;
            }
            else
            {
                foreach (SimDescription sim in best)
                {
                    HouseholdBreakdown breakdown = new HouseholdBreakdown(Manager, this, UnlocalizedName, sim, HouseholdBreakdown.ChildrenMove.Scoring, false);

                    Add(frame, new StandardMoveInLotScenario(breakdown, 0), ScenarioResult.Failure);
                    Add(frame, new PostScenario(sim, mPassedInspection), ScenarioResult.Success);
                }
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new OutOfNestScenario(this);
        }

        public class PostScenario : SimScenario
        {
            bool mPassedInspection;

            public PostScenario(SimDescription sim, bool passedInspection)
                : base(sim)
            {
                mPassedInspection = passedInspection;
            }
            protected PostScenario(PostScenario scenario)
                : base(scenario)
            {
                mPassedInspection = scenario.mPassedInspection;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "OutOfNestPost";
                }
                else
                {
                    if (mPassedInspection)
                    {
                        return "OvercrowdingMove";
                    }
                    else
                    {
                        return "InspectionMove";
                    }
                }
            }

            protected override bool CheckBusy
            {
                get { return false; }
            }

            protected override bool Progressed
            {
                get { return true; }
            }

            protected override ICollection<SimDescription> GetSims()
            {
                return null;
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                return true;
            }

            public override Scenario Clone()
            {
                return new PostScenario(this);
            }
        }

        public class RoomToLeaveOption : IntegerScenarioOptionItem<ManagerHousehold, OutOfNestScenario>
        {
            public RoomToLeaveOption()
                : base(1)
            { }

            public override string GetTitlePrefix()
            {
                return "RoomToLeave";
            }
        }
    }
}
