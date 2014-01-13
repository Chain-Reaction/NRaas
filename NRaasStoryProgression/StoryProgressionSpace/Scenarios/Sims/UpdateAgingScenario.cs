using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class UpdateAgingScenario : SimUpdateScenario, IAlarmScenario
    {
        public UpdateAgingScenario()
        { }
        protected UpdateAgingScenario(UpdateAgingScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "UpdateAging";
        }

        protected override ListenHandleType HandleImmediately
        {
            get { return ListenHandleType.Task; }
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarmDay(this, 23.5f);
        }

        protected override bool ContinuousUpdate
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override void PrivatePerform(SimDescription sim, SimData data, ScenarioFrame frame)
        {
            if (SimTypes.InServicePool(sim, ServiceType.GrimReaper))
            {
                IncStat("Service");
                return;
            }
            else if ((sim.IsDeer) || (sim.IsRaccoon))
            {
                IncStat("Deer or Raccoon");
                return;
            }

            bool bAging = Sims.AllowAging(this, sim);
            if ((!bAging) && (sim.AgingEnabled != bAging))
            {
                AgingManager.Singleton.CancelAgingAlarmsForSim(sim.AgingState);
            }

            sim.AgingEnabled = bAging;
        }

        public override Scenario Clone()
        {
            return new UpdateAgingScenario(this);
        }

        public class Option : BooleanAlarmOptionItem<ManagerSim, UpdateAgingScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "UpdateAgingAlarm";
            }
        }

        public class ScheduledOption : BooleanScenarioOptionItem<ManagerSim, UpdateAgingScenario>, IDebuggingOption
        {
            public ScheduledOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "UpdateAgingScehduled";
            }
        }
    }
}
