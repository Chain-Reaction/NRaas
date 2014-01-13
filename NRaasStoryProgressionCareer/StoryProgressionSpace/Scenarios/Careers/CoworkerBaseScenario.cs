using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
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
    public abstract class CoworkerBaseScenario : SimScenario
    {
        protected List<SimDescription> mNewCoworkers = new List<SimDescription>();

        public CoworkerBaseScenario()
        { }
        public CoworkerBaseScenario(SimDescription sim)
            : base(sim)
        { }
        protected CoworkerBaseScenario(CoworkerBaseScenario scenario)
            : base (scenario)
        {
            mNewCoworkers = new List<SimDescription>(scenario.mNewCoworkers);
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

        protected override int ContinueReportChance
        {
            get { return 20; }
        }

        protected abstract Career Career
        {
            get;
        }

        protected override bool Allow()
        {
            if (Common.IsOnTrueVacation()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Careers.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (sim.CareerManager == null)
            {
                IncStat("No Manager");
                return false;
            }

            return base.Allow(sim);
        }

        protected virtual bool IsValid(Career job, SimDescription sim, bool checkExisting)
        {
            if (sim == null)
            {
                IncStat("Valid:Null");
                return false;
            }
            else if (job.OwnerDescription == sim)
            {
                IncStat("Valid:Me");
                return false;
            }
            else if ((!SimTypes.IsSelectable(sim)) && (!Friends.Allow(this, job.OwnerDescription, Managers.Manager.AllowCheck.None)))
            {
                IncStat("Valid:Me User Denied");
                return false;
            }
            else if (!sim.IsValidDescription)
            {
                IncStat("Valid:Invalid");
                return false;
            }
            else if (SimTypes.IsDead(sim))
            {
                IncStat("Valid:Dead");
                return false;
            }
            else if (sim.CareerManager == null)
            {
                IncStat("Valid:No Manager");
                return false;
            }
            else if ((checkExisting) && (job.Coworkers != null) && (job.Coworkers.Contains(sim)))
            {
                IncStat("Valid:Already");
                return false;
            }
            else if ((!SimTypes.IsSelectable(job.OwnerDescription)) && (!Friends.Allow(this, sim, Managers.Manager.AllowCheck.None)))
            {
                IncStat("Valid:Them User Denied");
                return false;
            }

            return true;
        }

        protected bool AddCoworker(Career job)
        {
            SimDescription newCoworker = null;

            if (!job.GetType().ToString().StartsWith("Sims3."))
            {
                List<SimDescription> previous = new List<SimDescription>(job.Coworkers);

                if (job.AddCoworker())
                {
                    foreach (SimDescription sim in job.Coworkers)
                    {
                        if (previous.Contains(sim)) continue;

                        newCoworker = sim;
                        break;
                    }
                }
            }
            else
            {
                List<SimDescription> candidateCoworkers = new List<SimDescription>();
                foreach (SimDescription sim in job.CareerLoc.Workers)
                {
                    if (IsValid(job, sim, true))
                    {
                        candidateCoworkers.Add(sim);
                    }
                }

                if (candidateCoworkers.Count == 0)
                {
                    foreach (CareerLocation location in job.Locations)
                    {
                        if (location == job.CareerLoc) continue;

                        location.CheckWorkers();
                        foreach (SimDescription coworker in location.Workers)
                        {
                            if (IsValid(job, coworker, true))
                            {
                                candidateCoworkers.Add(coworker);
                            }
                        }
                    }
                }

                if (candidateCoworkers.Count > 0)
                {
                    newCoworker = RandomUtil.GetRandomObjectFromList(candidateCoworkers);
                    job.AddCoworker(newCoworker);
                }
            }

            if (newCoworker != null)
            {
                if (GetValue<MeetImmediatelyOption, bool>())
                {
                    Relationship.Get(job.OwnerDescription, newCoworker, true).MakeAcquaintances();
                }
                else
                {
                    Relationship.Get(job.OwnerDescription, newCoworker, true);
                }

                if (!SimTypes.IsSelectable(newCoworker))
                {
                    mNewCoworkers.Add(newCoworker);
                }

                IncStat("Added");
                return true;
            }
            else
            {
                IncStat("No choices");
                return false;
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Career job = Career;
            if (job == null) return false;

            Corrections.RemoveInvalidCoworkers(job);

            if (job.CareerLoc == null) return false;

            job.CareerLoc.CheckWorkers();

            List<SimDescription> coworkers = new List<SimDescription>();
            if (job.Coworkers != null)
            {
                coworkers.AddRange(job.Coworkers);
            }

            foreach (SimDescription coworker in coworkers)
            {
                if (!IsValid(job, coworker, false))
                {
                    job.RemoveCoworker(coworker);

                    IncStat("Dropped: Bad");
                }
                else if (GetValue<MeetImmediatelyOption, bool>())
                {
                    Relationship relation = ManagerSim.GetRelationship(job.OwnerDescription, coworker);
                    if (relation != null)
                    {
                        relation.MakeAcquaintances();
                    }
                }
            }

            coworkers.Clear();
            if (job.Coworkers != null)
            {
                coworkers.AddRange(job.Coworkers);
            }

            foreach (SimDescription coworker in coworkers)
            {
                if ((job.Coworkers == null) || (job.Coworkers.Count <= job.SharedData.MaxCoworkers))
                {
                    break;
                }

                job.RemoveCoworker(coworker);
                IncStat("Dropped: Too Many");
            }

            int iRequired = (job.SharedData.MaxCoworkers - job.Coworkers.Count);

            AddScoring("Required", iRequired);

            for (int i = iRequired; i > 0; i--)
            {
                AddCoworker(job);
            }

            return (mNewCoworkers.Count > 0);
        }

        protected override bool Push()
        {
            if (mNewCoworkers.Count > 0)
            {
                SimDescription coworker = RandomUtil.GetRandomObjectFromList(mNewCoworkers);

                mNewCoworkers.Clear();
                mNewCoworkers.Add(coworker);

                return Situations.PushMeetUp(this, Sim, coworker, ManagerSituation.MeetUpType.Commercial, ManagerFriendship.FriendlyFirstAction);
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
                if (mNewCoworkers.Count == 0) return null;

                parameters = new object[] { Sim, mNewCoworkers[0] };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public class MeetImmediatelyOption : BooleanManagerOptionItem<ManagerCareer>
        {
            public MeetImmediatelyOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "MeetCoworkersImmediately";
            }

            public override bool ShouldDisplay()
            {
                if ((!Manager.GetValue<NewBossScenario.Option, bool>()) && (!Manager.GetValue<NewCoworkerScenario.Option, bool>()) && (!Manager.GetValue<NewClassmateScenario.Option, bool>())) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
