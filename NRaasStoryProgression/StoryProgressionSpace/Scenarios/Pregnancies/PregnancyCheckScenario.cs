using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Sims;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class PregnancyCheckScenario : SimUpdateScenario, IAlarmScenario
    {
        public PregnancyCheckScenario()
        { }
        protected PregnancyCheckScenario(PregnancyCheckScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "PregnancyCheck";
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
            return alarms.AddAlarmImmediate(this, 1f, TimeUnit.Days);
        }

        protected bool AllowProgression(SimDescription sim)
        {
            if (!SimTypes.IsSelectable(sim))
            {
                if (GetValue<InStasisOption, bool>(sim)) return false;
            }

            return true;
        }

        protected override void PrivatePerform(SimDescription sim, SimData data, ScenarioFrame frame)
        {
            PregnancySimData other = data.Get<PregnancySimData>();

            if (sim.IsPregnant)
            {
                if ((other.mLastCheckPregnancy != 0) && (other.mLastCheckPregnancy < SimClock.ElapsedCalendarDays()))
                {
                    if (!AllowProgression(sim))
                    {
                        // Delay pregnancy indefinitely
                        sim.Pregnancy.mHourOfPregnancy = other.mPregnancyHour;

                        IncStat("Pregnancy Suspended");
                    }
                    else
                    {
                        if (other.mPregnancyHour == sim.Pregnancy.mHourOfPregnancy)
                        {
                            IncStat(sim.FirstName + " " + sim.LastName + ": Stuck Pregnancy", Common.DebugLevel.High);

                            if (!SimTypes.IsSelectable(sim))
                            {
                                Sims.Reset(sim);

                                if ((sim.CreatedSim != null) && (sim.LotHome != null))
                                {
                                    SimDescription dad = ManagerSim.Find(sim.Pregnancy.DadDescriptionId);
                                    if (dad != null)
                                    {
                                        Sims.Instantiate(dad, sim.LotHome, false);
                                    }

                                    if (sim.Pregnancy.PreggersAlarm != AlarmHandle.kInvalidHandle)
                                    {
                                        AlarmManager.Global.RemoveAlarm(sim.Pregnancy.PreggersAlarm);
                                        sim.Pregnancy.PreggersAlarm = AlarmHandle.kInvalidHandle;
                                    }
                                    sim.Pregnancy.Continue(sim.CreatedSim, true);

                                    IncStat("Pregnancy Reset");
                                }
                            }
                        }
                    }
                }

                other.mPregnancyHour = sim.Pregnancy.mHourOfPregnancy;
                other.mLastCheckPregnancy = SimClock.ElapsedCalendarDays();
            }
            else
            {
                other.mPregnancyHour = 0;
                other.mLastCheckPregnancy = 0;
            }
        }

        public override Scenario Clone()
        {
            return new PregnancyCheckScenario(this);
        }

        protected class PregnancySimData : ElementalSimData
        {
            public int mPregnancyHour = 0;
            public int mLastCheckPregnancy = 0;

            public PregnancySimData()
            { }

            public override string ToString()
            {
                Common.StringBuilder text = new Common.StringBuilder();
                
                text.AddXML("Hour", mPregnancyHour);
                text.AddXML("LastCheck", mLastCheckPregnancy);

                return text.ToString();
            }
        }

        public class Option : BooleanAlarmOptionItem<ManagerPregnancy, PregnancyCheckScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "PregnancyCheck";
            }
        }
    }
}
