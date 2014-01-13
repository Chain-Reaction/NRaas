using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Deaths
{
    public class KillScenario : SimScenario, IHasSkill, IAlarmScenario
    {
        SimDescription mKiller;

        SimDescription.DeathType mDeathType;

        string mStoryName;

        static Common.MethodStore sAssassinationAddKill = new Common.MethodStore("NRaasCareer", "Assassination", "SetPotential", new Type[] { typeof(SimDescription), typeof(SimDescription), typeof(bool) });

        public KillScenario(SimDescription sim, SimDescription killer, SimDescription.DeathType deathType)
            : base(sim)
        {
            mKiller = killer;
            mDeathType = deathType;
            mStoryName = deathType.ToString();
        }
        public KillScenario(SimDescription sim, SimDescription killer, SimDescription.DeathType deathType, string storyName)
            : base (sim)
        {
            mKiller = killer;
            mDeathType = deathType;
            mStoryName = storyName;
        }
        protected KillScenario(KillScenario scenario)
            : base (scenario)
        {
            mKiller = scenario.mKiller;
            mDeathType = scenario.mDeathType;
            mStoryName = scenario.mStoryName;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Kill";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public SkillNames[] CheckSkills
        {
            // Required by the Assassination skill check
            get { return new SkillNames[0]; }
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Deaths.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.Allow(sim);
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarm(this, 1);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        public void AddAssassinationKill(SimDescription a, SimDescription b, bool erased)
        {
            if (sAssassinationAddKill.Valid)
            {
                bool allow = false;

                if (Skills.Allow(this, a))
                {
                    allow = true;
                }

                if (!allow) return;

                sAssassinationAddKill.Invoke<object>(new object[] { a, b, erased });
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (mDeathType == SimDescription.DeathType.None)
            {
                mDeathType = ManagerDeath.GetRandomDeathType();
            }

            GettingOldScenario.AddDeathScenarios(this, frame);

            if (mKiller != null)
            {
                AddAssassinationKill(mKiller, Sim, false);
            }

            Manager.AddAlarm(new PostScenario(Sim, mDeathType, mStoryName));
            return true;
        }

        public override Scenario Clone()
        {
            return new KillScenario(this);
        }

        public class PostScenario : SimScenario, IAlarmScenario
        {
            SimDescription.DeathType mDeathType;

            string mStoryName;

            public PostScenario(SimDescription sim, SimDescription.DeathType deathType, string storyName)
                : base (sim)
            {
                mDeathType = deathType;
                mStoryName = storyName;
            }
            protected PostScenario(PostScenario scenario)
                : base(scenario)
            {
                mDeathType = scenario.mDeathType;
                mStoryName = scenario.mStoryName;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "KillPost";
                }
                else
                {
                    return mStoryName;
                }
            }

            public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
            {
                return alarms.AddAlarm(this, 0.1f);
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

            protected override bool Allow(SimDescription sim)
            {
                if (sim.CreatedSim == null)
                {
                    IncStat("Hibernating");
                    return false;
                }
                else if (sim.CreatedSim.InteractionQueue == null)
                {
                    IncStat("No Queue");
                }
                else if (!Deaths.Allow(this, sim))
                {
                    IncStat("User Denied");
                    return false;
                }
                else if (Sim.CreatedSim.LotCurrent.IsWorldLot)
                {
                    IncStat("In Transit");
                    return false;
                }
                else if (sim.CreatedSim.InteractionQueue.HasInteractionOfType(Urnstone.KillSim.Singleton))
                {
                    IncStat("Dying");
                    return false;
                }
                else if (!Deaths.IsDying(sim))
                {
                    IncStat("Saved");
                    return false;
                }

                return base.Allow(sim);
            }

            protected override void AllowFailCleanup()
            {
                GetData<ManagerDeath.DyingSimData>(Sim).Testing = false;
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                GetData<ManagerDeath.DyingSimData>(Sim).Testing = false;
                return true;
            }

            protected override bool Push()
            {
                if (Sim.CreatedSim.InteractionQueue != null)
                {
                    Sim.CreatedSim.InteractionQueue.CancelAllInteractions();
                }

                InteractionInstance entry = Urnstone.KillSim.Singleton.CreateInstance(Sim.CreatedSim, Sim.CreatedSim, new InteractionPriority(InteractionPriorityLevel.MaxDeath, 0f), false, false);
                (entry as Urnstone.KillSim).simDeathType = mDeathType;
                return Sim.CreatedSim.InteractionQueue.Add(entry);
            }

            protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
            {
                if (manager == null)
                {
                    manager = Deaths;
                }

                GetData<ManagerDeath.DyingSimData>(Sim).Notified = true;

                return base.PrintStory(manager, name, parameters, extended, logging);
            }

            public override Scenario Clone()
            {
                return new PostScenario(this);
            }
        }
    }
}
