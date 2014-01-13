using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Deaths;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class GoToHospitalScenario : SimScenario, IAlarmScenario
    {
        SimDescription.DeathType mDeathType;
        int mChanceOfDeath = -1;
        string mStoryName = null;

        SimDescription mKiller;

        GameObject mReason;

        public GoToHospitalScenario(SimDescription sim, SimDescription killer, GameObject reason, string storyName, SimDescription.DeathType deathType)
            : this(sim, killer, storyName, deathType, -1)
        {
            mReason = reason;
        }
        public GoToHospitalScenario(SimDescription sim, SimDescription killer, GameObject reason, string storyName, SimDescription.DeathType deathType, int chanceOfDeath)
            : this(sim, killer, storyName, deathType, chanceOfDeath)
        {
            mReason = reason;
        }
        public GoToHospitalScenario(SimDescription sim, SimDescription killer, string storyName, SimDescription.DeathType deathType)
            : this(sim, killer, storyName, deathType, -1)
        { }
        public GoToHospitalScenario(SimDescription sim, SimDescription killer, string storyName, SimDescription.DeathType deathType, int chanceOfDeath)
            : base(sim)
        {
            mKiller = killer;
            mDeathType = deathType;
            mChanceOfDeath = chanceOfDeath;
            mStoryName = storyName;
        }
        protected GoToHospitalScenario(GoToHospitalScenario scenario)
            : base (scenario)
        {
            mKiller = scenario.mKiller;
            mDeathType = scenario.mDeathType;
            mChanceOfDeath = scenario.mChanceOfDeath;
            mStoryName = scenario.mStoryName;
            mReason = scenario.mReason;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "GoToHospital";
            }
            else
            {
                return mStoryName;
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

        protected override int Rescheduling
        {
            get { return 30; }
        }

        protected override int MaximumReschedules
        {
            get { return 12; }
        }

        protected override int PushChance
        {
            get 
            {
                if (mChanceOfDeath > 0)
                {
                    return 100;
                }
                else
                {
                    return AddScoring("GoToHospital", GetValue<ChanceGoToHospitalOption, int>(Sim), ScoringLookup.OptionType.Bounded, Sim);
                }
            }
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarm(this, 1);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (sim.DeathStyle != SimDescription.DeathType.None)
            {
                IncStat("Ghost");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            try
            {
                if ((Sim.CreatedSim != null) && (Sim.CreatedSim.InteractionQueue != null))
                {
                    Sim.CreatedSim.InteractionQueue.CancelAllInteractions();
                }
            }
            catch (Exception e)
            {
                Common.DebugException(Sim, e);

                IncStat("Exception");
                return false;
            }

            int defChanceOfDeath = GetValue<PushDeathChanceOption, int>(Sim);

            if ((mChanceOfDeath < 0) || ((mChanceOfDeath > 0) && (mChanceOfDeath < defChanceOfDeath)))
            {
                mChanceOfDeath = defChanceOfDeath;
            }

            AddScoring("Chance of Death", mChanceOfDeath);

            if (RandomUtil.RandomChance(mChanceOfDeath))
            {
                if (mKiller != null)
                {
                    Deaths.AddAlarm(new KillScenario(Sim, mKiller, mDeathType, GetTitlePrefix(PrefixType.Story)));
                    return false;
                }
            }
            else
            {
                IncStat("Chance Fail");
            }

            if (!Situations.PushToRabbitHole(this, Sim, RabbitHoleType.Hospital, false, false))
            {
                IncStat("Push Fail");
                return false;
            }

            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Situations;
            }

            if ((parameters == null) && (mReason != null))
            {
                parameters = new object[] { Sim, mReason.CatalogName };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new GoToHospitalScenario(this);
        }
    }
}
