using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class MoodCheckScenario : SimUpdateScenario, IAlarmScenario
    {
        public MoodCheckScenario()
        { }
        protected MoodCheckScenario(MoodCheckScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "MoodCheck";
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

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarmImmediate(this, 1f, TimeUnit.Hours);
        }

        protected override void PrivatePerform(SimDescription sim, SimData data, ScenarioFrame frame)
        {
            MoodSimData other = data.Get<MoodSimData>();

            Sim createdSim = sim.CreatedSim;

            if ((createdSim != null) && (createdSim.InWorld) && (createdSim.Proxy != null) && (!SimTypes.IsSelectable(sim)))
            {
                if (createdSim.BuffManager != null)
                {
                    createdSim.BuffManager.RemoveElement(BuffNames.TooMuchSun);

                    if ((createdSim.BuffManager.HasElement(BuffNames.Starving)) ||
                        (createdSim.BuffManager.HasElement(BuffNames.MadlyThirsty)))
                    {
                        IncStat("Hunger Reset");

                        IncStat(sim.FullName + ": Stuck Sim", Common.DebugLevel.High);

                        Sims.Reset(sim);

                        other.mMoodPushedHome = 0;
                    }
                }

                if ((createdSim.MoodManager != null) && (createdSim.MoodManager.MoodFlavor == MoodFlavor.Miserable))
                {
                    if (other.mMoodPushedHome == 0)
                    {
                        if ((createdSim.InteractionQueue == null) || (sim.LotHome == null))
                        {
                            IncStat("Mood Reset");

                            IncStat(sim.FullName + ": Stuck Townie", Common.DebugLevel.High);

                            Sims.Reset(sim);

                            other.mMoodPushedHome = 0;
                        }
                        else
                        {
                            IncStat("Mood Push");

                            createdSim.InteractionQueue.CancelAllInteractions();
                            createdSim.InteractionQueue.Add(GoHome.Singleton.CreateInstance(sim.LotHome, createdSim, new Sims3.Gameplay.Interactions.InteractionPriority(Sims3.Gameplay.Interactions.InteractionPriorityLevel.UserDirected), false, true));

                            other.mMoodPushedHome = SimClock.ElapsedCalendarDays();
                        }
                    }
                    else if ((other.mMoodPushedHome + 1) < SimClock.ElapsedCalendarDays())
                    {
                        IncStat("Mood Reset");

                        IncStat(sim.FullName + ": Stuck Sim", Common.DebugLevel.High);

                        Sims.Reset(sim);

                        other.mMoodPushedHome = 0;
                    }
                }
                else
                {
                    other.mMoodPushedHome = 0;
                }
            }
        }

        public override Scenario Clone()
        {
            return new MoodCheckScenario(this);
        }

        protected class MoodSimData : ElementalSimData
        {
            public int mMoodPushedHome = 0;

            public MoodSimData()
            { }

            public override string ToString()
            {
                Common.StringBuilder text = new Common.StringBuilder(base.ToString());

                text.AddXML("Pushed", mMoodPushedHome);

                return text.ToString();
            }
        }

        public class Option : BooleanAlarmOptionItem<ManagerSim, MoodCheckScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "MoodCheck";
            }
        }
    }
}
