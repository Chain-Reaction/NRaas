using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public abstract class AssignSchoolScenario : SimScenario
    {
        public AssignSchoolScenario()
        { }
        protected AssignSchoolScenario(SimDescription sim)
            : base (sim)
        { }
        protected AssignSchoolScenario(AssignSchoolScenario scenario)
            : base (scenario)
        { }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected int SchoolFee
        {
            get { return GetValue<SchoolFeesScenario.Option,int>(); }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Careers.SchoolChildren;
        }

        protected override bool Allow()
        {
            if (!GetValue<AutoOption, bool>()) return false;

            if (Common.IsOnTrueVacation()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.Household == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (SimTypes.IsService(sim))
            {
                IncStat("Service");
                return false;
            }
            else if (SimTypes.IsDead(sim))
            {
                IncStat("Dead");
                return false;
            }
            else if ((!sim.Child) && (!sim.Teen))
            {
                IncStat("Not Child");
                return false;
            }
            else if (sim.CareerManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (GetValue<ManualSchoolOption, bool>(sim))
            {
                IncStat("Manual");
                return false;
            }
            else if (!Careers.AllowFindJob(this, sim))
            {
                IncStat("Find Job Denied");
                return false;
            }
            else if (!Careers.Allow(this, sim, Managers.Manager.AllowCheck.None))
            {
                IncStat("Careers Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected CareerLocation FindClosestCareerLocation(SimDescription s, OccupationNames careerName)
        {
            if (s.Household == null)
            {
                IncStat("No Household");
                return null;
            }

            Career staticCareer = CareerManager.GetStaticCareer(careerName);
            if (staticCareer == null)
            {
                IncStat("Career Fail");
                return null;
            }

            Lot lotHome = s.Household.LotHome;
            if (lotHome == null)
            {
                if (staticCareer.Locations.Count > 0x0)
                {
                    return RandomUtil.GetRandomObjectFromList<CareerLocation>(staticCareer.Locations);
                }
                else
                {
                    IncStat("No Lot Home");
                    return null;
                }
            }

            CareerLocation location = null;
            float bestDistance = float.MaxValue;
            float distanceToObject = 0f;
            foreach (CareerLocation location2 in staticCareer.Locations)
            {
                if (!GetLotOptions(location2.Owner.LotCurrent).AllowCastes(this, s)) continue;

                distanceToObject = lotHome.GetDistanceToObject(location2.Owner.RabbitHoleProxy);
                if (distanceToObject < bestDistance)
                {
                    location = location2;
                    bestDistance = distanceToObject;
                }
            }

            return location;
        }

        protected abstract List<CareerLocation> GetPotentials();

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<CareerLocation> schools = GetPotentials();

            if (schools.Count == 0)
            {
                IncStat("No Choice");
                return false;
            }
            else
            {
                if ((Sim.CareerManager.School != null) && 
                    (schools.Contains(Sim.CareerManager.School.CareerLoc)))
                {
                    IncStat("Already Has");
                    return false;
                }

                CareerLocation location = RandomUtil.GetRandomObjectFromList(schools);

                if ((Sim.CareerManager.School != null) &&
                    (location == Sim.CareerManager.School.CareerLoc))
                {
                    IncStat("Same");
                    return false;
                }
                else
                {
                    Occupation job = Sim.Occupation;
                    Occupation retiredJob = Sim.CareerManager.mRetiredCareer;
                    
                    Sim.CareerManager.mJob = null;
                    Sim.CareerManager.mRetiredCareer = null;

                    try
                    {
                        if (Sim.AcquireOccupation(new AcquireOccupationParameters(location, false, false)))
                        {
                            School school = Sim.CareerManager.School;
                            if (school != null)
                            {
                                school.mWhenCurLevelStarted = SimClock.Subtract(school.mWhenCurLevelStarted, TimeUnit.Days, 1);
                            }

                            return true;
                        }
                        else
                        {
                            IncStat("Core Failure");
                            return false;
                        }
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(Sim, e);

                        IncStat("Exception Failure");
                        return false;
                    }
                    finally
                    {
                        Sim.CareerManager.mJob = job;
                        Sim.CareerManager.mRetiredCareer = retiredJob;
                    }
                }
            }
        }

        protected static void DropNonMatchingStudents(ManagerCareer manager, Common.IStatGenerator stats, Lot lot)
        {
            LotOptions lotOptions = manager.GetLotOptions(lot);

            foreach (RabbitHole hole in lot.GetObjects<RabbitHole>())
            {
                if (hole.CareerLocations == null) continue;

                foreach (CareerLocation location in hole.CareerLocations.Values)
                {
                    if (location == null) continue;

                    if (location.Career is School)
                    {
                        if (location.Workers == null) continue;

                        foreach (SimDescription sim in new List<SimDescription>(location.Workers))
                        {
                            try
                            {
                                if (!lotOptions.AllowCastes(stats, sim))
                                {
                                    if (sim.CareerManager != null)
                                    {
                                        if (sim.CareerManager.School != null)
                                        {
                                            sim.CareerManager.School.LeaveJobNow(Career.LeaveJobReason.kJobBecameInvalid);
                                        }

                                        sim.CareerManager.School = null;
                                    }

                                    manager.Scenarios.Post(new ScheduledAssignSchoolScenario(sim));
                                }
                            }
                            catch (Exception e)
                            {
                                Common.Exception(sim, e);
                            }
                        }
                    }
                }
            }
        }

        public class AutoOption : BooleanManagerOptionItem<ManagerCareer>, ManagerCareer.ISchoolOption
        {
            public AutoOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AutoAssignSchool";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
