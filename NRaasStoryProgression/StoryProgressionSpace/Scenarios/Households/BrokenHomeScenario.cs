using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
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
    public class BrokenHomeScenario : DualSimScenario
    {
        bool mOpposing;

        public BrokenHomeScenario()
        { }
        protected BrokenHomeScenario(BrokenHomeScenario scenario)
            : base (scenario)
        {
            mOpposing = scenario.mOpposing;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (mOpposing)
            {
                return "OpposingBrokenHome";
            }
            else
            {
                return "BrokenHome";
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
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return HouseholdsEx.Humans(sim.Household);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Households.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (sim.Household == null)
            {
                return false;
            }
            else if (SimTypes.IsSpecial(sim))
            {
                IncStat("Special");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (SimTypes.HeadOfFamily(sim.Household) == sim)
            {
                IncStat("Head");
                return false;
            }
            else if (!Households.AllowSoloMove(sim))
            {
                IncStat("Too Young");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription target)
        {
            if (target.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (Sim.Partner == target)
            {
                IncStat("Partner");
                return false;
            }


            bool check = true;
            if (GetValue<SplitOpposingClanOption,bool>())
            {
                if (!Households.Allow(this, Sim, target, Managers.Manager.AllowCheck.None))
                {
                    check = false;
                }
            }

            if (check)
            {
                Relationship relation = ManagerSim.GetRelationship(Sim, target);
                if (relation == null)
                {
                    IncStat("Bad Relation");
                    return false;
                }

                if (AddScoring("MoveAwayFrom", target, Sim) < 20)
                {
                    IncStat("Score Fail");
                    return false;
                }
            }

            return base.TargetAllow(target);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (GetValue<SplitOpposingClanOption, bool>())
            {
                mOpposing = !Households.Allow(this, Sim, Target, Managers.Manager.AllowCheck.None);
            }

            Add(frame, new BrokenHomeMoveOutScenario(Sim, Target), ScenarioResult.Start);
            return false;
        }

        public override Scenario Clone()
        {
            return new BrokenHomeScenario(this);
        }

        protected class BrokenHomeMoveOutScenario : MoveOutScenario
        {
            public BrokenHomeMoveOutScenario(SimDescription go, SimDescription stay)
                : base(go, stay)
            { }
            protected BrokenHomeMoveOutScenario(BrokenHomeMoveOutScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;

                return "BrokenHomeMoveOut";
            }

            protected override HouseholdBreakdown.ChildrenMove ChildMove
            {
                get { return HouseholdBreakdown.ChildrenMove.Scoring; }
            }

            protected override MoveInLotScenario GetMoveInLotScenario(List<SimDescription> going)
            {
                return new StandardMoveInLotScenario(going, 0);
            }

            protected override ScoredMoveInScenario GetMoveInScenario(List<SimDescription> going)
            {
                return new InspectedScoredMoveInScenario(Sim, going);
            }

            public override Scenario Clone()
            {
                return new BrokenHomeMoveOutScenario(this);
            }
        }

        public class Option : BooleanScenarioOptionItem<ManagerHousehold, BrokenHomeScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "BrokenHomeMove";
            }
        }

        public class SplitOpposingClanOption : BooleanManagerOptionItem<ManagerHousehold>
        {
            public SplitOpposingClanOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SplitOpposingClan";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option,bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
