using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
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
    public class PartneringScenario : DualSimScenario
    {
        bool mReport = true;

        bool mForce;

        public PartneringScenario(SimDescription sim, SimDescription target, bool report, bool force)
            : base (sim, target)
        {
            mReport = report;
            mForce = force;
        }
        protected PartneringScenario()
        { }
        protected PartneringScenario(PartneringScenario scenario)
            : base (scenario)
        {
            mReport = scenario.mReport;
            mForce = scenario.mForce;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Report=" + mReport;
            text += Common.NewLine + "Force=" + mForce;

            return text;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Partnered";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected virtual bool AllowAffair
        {
            get { return false; }
        }

        protected override bool ShouldReport
        {
            get { return mReport; }
        }

        protected override bool CheckBusy
        {
            get { return !mForce; }
        }

        protected virtual bool HandleAdultSeperately
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Flirts.FlirtPool;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Flirts.FindExistingFor(this, sim, true);
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Romances.Allow(this, sim))
            {
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            Relationship relation = ManagerSim.GetRelationship(Sim, Target);
            if (relation == null) return false;

            if ((!mForce) && (relation.LTR.Liking <= Sims3.Gameplay.Actors.Sim.kRomanceUseLikingGate))
            {
                IncStat("Under Gate");
                return false;
            }
            else if ((!AllowAffair) && (ManagerRomance.IsAffair(Sim, Target)))
            {
                IncStat("Affair");
                return false;
            }

            if (Sim.Partner == Target)
            {
                IncStat("Already Partner");
                return false;
            }
            else if (!Romances.Allow(this, Sim, Target))
            {
                return false;
            }

            if (!mForce)
            {
                if (relation.LTR.Liking < GetValue<PartnerLikingGateOption, int>())
                {
                    AddStat("No Like", relation.LTR.Liking);
                    return false;
                }
                else if (!relation.AreRomantic())
                {
                    IncStat("Not Romantic");
                    return false;
                }
                else if (GetElapsedTime<DayOfLastPartnerOption>(Sim) < GetValue<MinTimeFromBreakupToPartnerOption, int>())
                {
                    AddStat("Too Early", GetElapsedTime<DayOfLastPartnerOption>(Sim));
                    return false;
                }
                else if (GetElapsedTime<DayOfLastPartnerOption>(Target) < GetValue<MinTimeFromBreakupToPartnerOption, int>())
                {
                    AddStat("Too Early", GetElapsedTime<DayOfLastPartnerOption>(Target));
                    return false;
                }
                else if ((AddScoring("SettleDown", Target, Sim) <= 0) &&
                         (AddScoring("SettleDown", Sim, Target) <= 0))
                {
                    IncStat("Score Fail");
                    return false;
                }
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return Romances.BumpToHigherState(this, Sim, Target);
        }

        protected override bool Push()
        {
            return Situations.PushMeetUp(this, Sim, Target, ManagerSituation.MeetUpType.Residential, ManagerFlirt.FirstAction);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Romances;
            }

            if (HandleAdultSeperately)
            {
                if ((!Sim.Teen) && (!Target.Teen))
                {
                    name += "Adult";
                }
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new PartneringScenario(this);
        }

        public class PartnerLikingGateOption : IntegerManagerOptionItem<ManagerRomance>
        {
            public PartnerLikingGateOption()
                : base(40)
            { }

            public override string GetTitlePrefix()
            {
                return "PartnershipLikingGate";
            }
        }

        public class MinTimeFromBreakupToPartnerOption : Manager.CooldownOptionItem<ManagerRomance>
        {
            public MinTimeFromBreakupToPartnerOption()
                : base(2)
            { }

            public override string GetTitlePrefix()
            {
                return "CooldownBreakupToPartner";
            }
        }
    }
}
