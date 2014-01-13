using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Romances
{
    public class EngagementScenario : MarriageBaseScenario
    {
        bool mTestScoring;

        public EngagementScenario(SimDescription sim, SimDescription target, bool testScoring)
            : base(sim, target)
        {
            mTestScoring = testScoring;
        }
        protected EngagementScenario(EngagementScenario scenario)
            : base (scenario)
        {
            mTestScoring = scenario.mTestScoring;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Engagement";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Romances.Partnered;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            if (sim.Partner == null) return null;

            List<SimDescription> list = new List<SimDescription>();
            list.Add(sim.Partner);
            return list;
        }

        protected override bool Score()
        {
            if (!mTestScoring) return true;

            return base.Score();
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (sim.IsEngaged)
            {
                IncStat("Already Engaged");
                return false;
            }

            if (mTestScoring)
            {
                Relationship relation = ManagerSim.GetRelationship(Sim, Target);
                if (relation == null) return false;

                if (relation.LTR.Liking < GetValue<CooldownMarriageScenario.LikingGateOption, int>())
                {
                    AddStat("No Like", relation.LTR.Liking);
                    return false;
                }
                else if (AddScoring("Engagement Cooldown", GetElapsedTime<DayOfLastPartnerOption>(sim) - GetValue<MinTimeFromPartnerToEngagementOption, int>()) < 0)
                {
                    AddStat("Too Early", GetElapsedTime<DayOfLastPartnerOption>(sim));
                    return false;
                }
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            Relationship relation = ManagerSim.GetRelationship(sim, Sim);
            if (relation == null)
            {
                IncStat("No Relation");
                return false;
            }

            return base.TargetAllow(sim);
        }

        public static event UpdateDelegate OnGatheringScenario;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!Romances.BumpToEngagement(this, Sim, Target))
            {
                IncStat("Bump Failure");
                return false;
            }
            else
            {
                SetElapsedTime<DayOfLastEngagementOption>(Sim);
                SetElapsedTime<DayOfLastEngagementOption>(Target);

                if (OnGatheringScenario != null)
                {
                    OnGatheringScenario(this, frame);
                }
                return true;
            }
        }

        public override Scenario Clone()
        {
            return new EngagementScenario(this);
        }

        public class MinTimeFromPartnerToEngagementOption : Manager.CooldownOptionItem<ManagerRomance>
        {
            public MinTimeFromPartnerToEngagementOption()
                : base(2)
            { }

            public override string GetTitlePrefix()
            {
                return "CooldownPartnerToEngagement";
            }
        }
    }
}
