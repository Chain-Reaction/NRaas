using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class MaximumElderAgeScenario : SimScenario, IAlarmScenario
    {
        public MaximumElderAgeScenario()
        { }
        protected MaximumElderAgeScenario(MaximumElderAgeScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "MaximumElderAge";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarmDay(this, 1f);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.AgingState == null)
            {
                IncStat("No AgingState");
                return false;
            }
            else if (sim.Age != CASAgeGenderFlags.Elder)
            {
                IncStat("Not Elder");
                return false;
            }
            else if (Deaths.IsDying(sim))
            {
                IncStat("Already Dying");
                return false;
            }
            else if (GetValue<MaximumAgeOption, int>(sim) < 0)
            {
                IncStat("Disabled");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            float age = Sim.YearsSinceLastAgeTransition - AgingManager.GetAgingStageLength(Sim.Species, Sim.Age);
            if (age >= 0)
            {
                bool kill = false;

                age = AgingManager.Singleton.AgingYearsToSimDays(age);
                if (age >= GetValue<MaximumAgeOption, int>(Sim))
                {
                    kill = true;

                    IncStat("Beyond Maximum");
                }
                else
                {
                    int cumulChance = GetValue<CumulativeAgeChanceOption,int>(Sim);
                    if (cumulChance > 0)
                    {
                        float chance = AgingManager.Singleton.GetChancePerDayElderWillDie(Sim.Species, false, Sim.IsFrankenstein);

                        chance += cumulChance * age;

                        if (RandomUtil.RandomChance(chance))
                        {
                            kill = true;

                            AddStat("Cumul Success", chance);
                        }
                    }
                    else
                    {
                        AddStat("Too Young", age);
                    }
                }

                if (kill)
                {
                    EventTracker.SendEvent(new MiniSimDescriptionEvent(EventTypeId.kSimGettingOld, Sim));
                    Sim.AgingState.AgeTransitionWithoutCakeAlarm = AlarmManager.Global.AddAlarm(RandomUtil.GetFloat(12f, 24f), TimeUnit.Hours, Sim.AgingState.AgeTransitionWithoutCakeCallback, "The Cake is a Lie and Then You Die", AlarmType.AlwaysPersisted, Sim);
                    AlarmManager.Global.AlarmWillYield(Sim.AgingState.AgeTransitionWithoutCakeAlarm);

                    IncStat("Aging Initiated");
                }
            }
            else
            {
                AddStat("Still Under Age", age);
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new MaximumElderAgeScenario(this);
        }

        public class Option : BooleanAlarmOptionItem<ManagerSim, MaximumElderAgeScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "MaximumElderAge";
            }
        }
    }
}
