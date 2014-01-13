using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public abstract class CareerAlarmScenario : CareerScenario, IAlarmScenario
    {
        public CareerAlarmScenario(SimDescription sim)
            : base (sim)
        {}
        protected CareerAlarmScenario(CareerAlarmScenario scenario)
            : base (scenario)
        { }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected abstract AlarmSimData AlarmData
        { get; }

        protected abstract float GetTime();

        protected bool AtWork(SimDescription sim)
        {
            if (sim == null) return false;

            if (sim.CreatedSim == null) return false;

            if (sim.CreatedSim.InteractionQueue == null) return false;

            return (sim.CreatedSim.InteractionQueue.GetCurrentInteraction() is ICountsAsWorking);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (AtWork(sim))
            {
                IncStat("At Work");
                return false;
            }
            else if (Job.IsDayOff)
            {
                IncStat("Day Off");
                return false;
            }

            return base.Allow(sim);
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            AlarmData.Alarm = alarms.AddAlarm(this, GetTime());

            return AlarmData.Alarm;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            AlarmData.Reset();
            return false;
        }

        protected abstract class AlarmSimData : ElementalSimData, ICareerAlarmSimData, ICommuteSimData
        {
            protected AlarmManagerReference mAlarm = null;

            public AlarmSimData()
            { }

            public bool Valid
            {
                get 
                {
                    if (mAlarm == null) return false;

                    return mAlarm.Valid; 
                }
            }

            public AlarmManagerReference Alarm
            {
                get { return mAlarm; }
                set
                {
                    if (mAlarm != null)
                    {
                        mAlarm.Dispose();
                    }
                    mAlarm = value;
                }
            }

            public override string ToString()
            {
                return Common.StringBuilder.XML("Valid", Valid);
            }

            public void Reset()
            {
                if (mAlarm != null)
                {
                    mAlarm.Dispose();
                    mAlarm = null;
                }
            }
        }
    }
}
