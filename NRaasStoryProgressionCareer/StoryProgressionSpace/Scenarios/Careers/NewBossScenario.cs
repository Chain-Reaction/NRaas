using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
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
    public class NewBossScenario : SimScenario
    {
        bool mIncreasePressure = true;

        public NewBossScenario()
            : base ()
        { }
        public NewBossScenario(bool increasePressure)
        {
            mIncreasePressure = increasePressure;
        }
        public NewBossScenario(SimDescription sim)
            : base(sim)
        {
            mIncreasePressure = false;
        }
        protected NewBossScenario(NewBossScenario scenario)
            : base (scenario)
        {
            mIncreasePressure = scenario.mIncreasePressure;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "NewBoss";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected bool IncreasePressure
        {
            get { return mIncreasePressure; }
        }

        protected int ImmigrationPressure
        {
            get { return GetValue<ImmigrantRequirementScenario.PressureOption,int>(); }
        }

        protected bool MeetImmediately
        {
            get { return GetValue<NewCoworkerScenario.MeetImmediatelyOption, bool>(); }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            if (Common.IsOnTrueVacation()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Careers.Employed;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!(sim.Occupation is Career))
            {
                IncStat("Not Career");
                return false;
            }
            else if (!Careers.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (!ManagerCareer.ValidCareer(sim.Occupation))
            {
                IncStat("No Career");
                return false;
            }

            return base.Allow(sim);
        }

        protected bool IsValidBoss(Career job, SimDescription boss)
        {
            return Careers.IsValidBoss(this, job, boss);
        }

        protected bool ChooseBoss(Career job, bool increasePressure)
        {
            if (IsValidBoss(job, job.Boss)) return false;

            if (IsValidBoss(job, job.FormerBoss))
            {
                job.SetBoss(job.FormerBoss);
                return false;
            }
            else if (job.CareerLoc != null)
            {
                Dictionary<SimDescription, bool> candidateBosses = new Dictionary<SimDescription, bool>();

                foreach (SimDescription boss in job.CareerLoc.Workers)
                {
                    if (IsValidBoss(job, boss))
                    {
                        if (!candidateBosses.ContainsKey(boss))
                        {
                            candidateBosses.Add(boss, true);
                        }
                    }
                }

                if (candidateBosses.Count == 0)
                {
                    foreach (SimDescription owner in Money.GetDeedOwner(job.CareerLoc.Owner))
                    {
                        if (IsValidBoss(job, owner))
                        {
                            if (!candidateBosses.ContainsKey(owner))
                            {
                                candidateBosses.Add(owner, true);
                            }
                        }
                    }
                }

                if (candidateBosses.Count == 0)
                {
                    foreach (CareerLocation location in job.Locations)
                    {
                        if (location != job.CareerLoc)
                        {
                            location.CheckWorkers();
                            foreach (SimDescription boss in location.Workers)
                            {
                                if (IsValidBoss(job, boss))
                                {
                                    if (!candidateBosses.ContainsKey(boss))
                                    {
                                        candidateBosses.Add(boss, true);
                                    }
                                }
                            }
                        }
                    }
                }

                if (candidateBosses.Count > 0)
                {
                    List<SimDescription> randomList = new List<SimDescription>(candidateBosses.Keys);
                    SetBoss(job, RandomUtil.GetRandomObjectFromList(randomList));

                    if (job.Coworkers != null)
                    {
                        job.Coworkers.Remove(job.Boss);
                    }

                    Careers.VerifyTone(job.OwnerDescription);

                    return true;
                }
                else
                {
                    if ((increasePressure) && (NeedsBoss(job, true)))
                    {
                        IncStat("Pressure: Boss Needed");

                        Lots.IncreaseImmigrationPressure(Careers, ImmigrationPressure);
                    }
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        protected static void SetBoss(Career job, SimDescription boss)
        {
            if (job.Boss != boss)
            {
                if ((job.Boss != null) && job.Boss.IsValidDescription)
                {
                    if (job.Boss.CareerManager != null)
                    {
                        job.Boss.CareerManager.ClearBossOfCareer(job);
                    }
                    job.FormerBoss = job.Boss;
                }

                job.Boss = boss;

                if (job.Boss != null)
                {
                    try
                    {
                        Relationship relation = Relationship.Get(job.OwnerDescription, job.Boss, true);
                        if (relation != null)
                        {
                            relation.MakeAcquaintances();
                        }
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(job.OwnerDescription, job.Boss, e);
                    }

                    if (job.Boss.Occupation != null)
                    {
                        job.Boss.CareerManager.SetBossOfCareer(job);
                    }

                    if (job.FormerBoss == job.Boss)
                    {
                        job.FormerBoss = null;
                    }
                }
            }
        }

        public static bool NeedsBoss(Career job, bool checkMetric)
        {
            if (job == null) return false;

            if (job.CurLevel == null) return false;

            if (!job.CurLevel.HasBoss) return false;

            if (job.CurLevel.NextLevels == null) return false;

            if (job.CurLevel.NextLevels.Count == 0) return false;

            if (checkMetric)
            {
                bool bRelBoss = false;
                foreach (PerfMetric metric in job.CurLevel.Metrics)
                {
                    if (metric is MetricRelBoss)
                    {
                        bRelBoss = true;
                    }
                }
                if (!bRelBoss) return false;
            }

            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Career job = Sim.Occupation as Career;

            Corrections.RemoveInvalidCoworkers(job);
            job.CareerLoc.CheckWorkers();

            if ((job.Boss != null) && (!IsValidBoss(job, job.Boss)))
            {
                if ((job.Boss == Sim) || (!SimTypes.IsSelectable(Sim)))
                {
                    job.SetBoss(null);
                    IncStat("Dropped: Invalid");
                }
            }

            if ((NeedsBoss(job, false)) && (ChooseBoss(job, IncreasePressure)))
            {
                return true;
            }

            Careers.VerifyTone(Sim);
            return false;
        }

        protected override bool Push()
        {
            if ((Sim.Occupation != null) && (Sim.Occupation.Boss != null))
            {
                return Situations.PushMeetUp(this, Sim, Sim.Occupation.Boss, ManagerSituation.MeetUpType.Either, ManagerFriendship.FriendlyFirstAction);
            }
            else
            {
                return false;
            }
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Careers;
            }

            if (parameters == null)
            {
                if ((Sim.Occupation != null) && (Sim.Occupation.Boss != null))
                {
                    parameters = new object[] { Sim, Sim.Occupation.Boss };
                }
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new NewBossScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerCareer, NewBossScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ManageBosses";
            }
        }
    }
}
