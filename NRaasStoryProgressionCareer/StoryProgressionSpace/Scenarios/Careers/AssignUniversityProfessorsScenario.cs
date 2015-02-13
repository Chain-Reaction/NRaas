using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Objects.Miscellaneous;
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
    public class AssignUniversityProfessorsScenario : ScheduledSoloScenario, IAlarmScenario
    {
        public AssignUniversityProfessorsScenario()
        { }
        protected AssignUniversityProfessorsScenario(AssignUniversityProfessorsScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "AssignUniversityProfessors";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarmDay(this, 6);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            if (Household.ActiveHousehold == null)
            {
                IncStat("No Active");
                return false;
            }
            else if (Household.ActiveHousehold.LotHome == null)
            {
                IncStat("No Active Lot");
                return false;
            }
            else if (GameUtils.IsUniversityWorld())
            {
                IncStat("University World");
                return false;
            }
            else if (!Careers.AllowHomeworldUniversity(null))
            {
                IncStat("Homeworld University Fail");
                return false;
            }

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Common.StringBuilder msg = new Common.StringBuilder();
            List<AcademicDegreeNames> degreesThatNeedProfessors = new List<AcademicDegreeNames>();

            foreach (AcademicDegreeNames degree in Enum.GetValues(typeof(AcademicDegreeNames)))
            {
                if (degree == AcademicDegreeNames.Undefined || degree == AcademicDegreeNames.MaxDegreeNames) continue;

                if (AcademicCareer.sProfessors != null)
                {
                    if (AcademicCareer.sProfessors.ContainsKey(degree) && SimDescription.Find(AcademicCareer.sProfessors[degree]) != null) continue;
                }
                
                msg += "DegreesThatNeedProfessors: " + degree + Common.NewLine;
                degreesThatNeedProfessors.Add(degree);
            }           

            if(degreesThatNeedProfessors.Count == 0)
            {
                Common.DebugNotify("No professors needed");
                return false;
            }
            
            List<SimDescription> possibleProfessors = new List<SimDescription>();
            List<AcademicDegreeNames> degreesWithEnrollment = new List<AcademicDegreeNames>();

            // Sims who have completed X degree 

            foreach(SimDescription sim in Sims.Humans)
            {
                msg += Common.NewLine + "Sim: " + sim.FullName;
                if (sim.CareerManager == null)
                {
                    msg += Common.NewLine + "CareerManager null";
                    continue;
                }

                if (sim.ChildOrBelow)
                {
                    msg += Common.NewLine + "ChildOrBelow";
                    continue;
                }

                if (SimTypes.IsSpecial(sim))
                {
                    msg += Common.NewLine + "IsSpecial";
                    continue;
                }

                if (sim.Occupation != null)
                {
                    msg += Common.NewLine + "Employed";
                    if (sim.CareerManager.Occupation is StoryProgressionSpace.Careers.Retired && !GetValue<AllowRetiredElder, bool>())
                    {
                        msg += Common.NewLine + "Retire skip";
                        continue;
                    }

                    if (sim.OccupationAsAcademicCareer == null && !GetValue<AllowQuitOccupation, bool>())
                    {
                        msg += Common.NewLine + "AllowQuitOccupation skip";
                        continue;
                    }

                    AcademicCareer career = sim.OccupationAsAcademicCareer;
                    if (career != null && career.Level == 2)
                    {
                        msg += Common.NewLine + "AlreadyProfessor";
                        continue;
                    }

                    if (career != null && career.DegreeInformation != null && degreesThatNeedProfessors.Contains(career.DegreeInformation.AcademicDegreeName))
                    {
                        if (!degreesWithEnrollment.Contains(career.DegreeInformation.AcademicDegreeName))
                        {
                            // note this isn't added to unless it needs a professor
                            msg += Common.NewLine + "DegreesWithEnrollment: " + career.DegreeInformation.AcademicDegreeName;
                            degreesWithEnrollment.Add(career.DegreeInformation.AcademicDegreeName);
                        }
                    }
                }
                else
                {
                    msg += Common.NewLine + "Unemployed";
                }

                if (sim.Teen || !GetValue<AllowBeProfessorOption, bool>(sim))
                {
                    msg += Common.NewLine + "Teen or !AllowBeProfessor";
                    continue;
                }

                if (!Careers.AllowFindJob(this, sim))
                {
                    msg += Common.NewLine + "!AllowFindJob";
                    continue;
                }

                if (!Careers.TestCareer(this, sim, OccupationNames.AcademicCareer))
                {
                    msg += Common.NewLine + "!TestCareer";
                    continue;
                }

                if (sim.Household != null && sim.Household != Household.ActiveHousehold)
                {
                    msg += Common.NewLine + "PossibleProfessor: " + sim.FullName;
                    possibleProfessors.Add(sim);                    
                }
            }

            RandomUtil.RandomizeListOfObjects<SimDescription>(possibleProfessors);

            if (degreesWithEnrollment.Count == 0)
            {
                msg += Common.NewLine + "degreesWithEnrollment = 0";
                Common.DebugWriteLog(msg);
                return false;
            }

            if (possibleProfessors.Count == 0)
            {
                msg += Common.NewLine + "possibleProfessors = 0";
                Common.DebugWriteLog(msg);
                return false;
            }

            List<SimDescription> preferredProfessors = new List<SimDescription>();            
            foreach (SimDescription pSim in new List<SimDescription>(possibleProfessors))
            {
                msg += Common.NewLine + "Promote loop: " + pSim.FullName;
                foreach (AcademicDegreeNames name in new List<AcademicDegreeNames>(degreesWithEnrollment))
                {
                    // checked because it can get removed
                    if (degreesWithEnrollment.Contains(name))
                    {
                        msg += Common.NewLine + "Degree: " + name;
                        if (pSim.OccupationAsAcademicCareer != null)
                        {
                            msg += Common.NewLine + "University Student";
                            // actively enrolled but has completed the degree we need?
                            if (pSim.CareerManager.DegreeManager != null && pSim.CareerManager.DegreeManager.HasCompletedDegree(name))
                            {
                                msg += Common.NewLine + "HCD: " + name;

                                if (pSim.OccupationAsAcademicCareer.DegreeInformation != null)
                                {
                                    // quit university for professor, use existing degree
                                    if (pSim.OccupationAsAcademicCareer.DegreeInformation.AcademicDegreeName != name)
                                    {
                                        pSim.Occupation.LeaveJobNow(Career.LeaveJobReason.kQuit);

                                        if (pSim.CreatedSim != null)
                                        {
                                            pSim.CreatedSim.InteractionQueue.CancelAllInteractionsByType(WorkInRabbitHole.Singleton);
                                        }

                                        while (pSim.Occupation != null)
                                        {
                                            Common.Sleep(5);
                                        }
                                    }

                                    AcademicCareer.EnrollSimInAcademicCareer(pSim, name, AcademicCareer.ChooseRandomCoursesPerDay());
                                    degreesWithEnrollment.Remove(name);
                                    pSim.Occupation.PromoteSim();
                                    possibleProfessors.Remove(pSim);
                                    msg += Common.NewLine + "Re-enrolling";                                    
                                    break;
                                }                                
                            }
                        }
                        else
                        {
                            // not active university Sim, have they completed this degree?
                            if (pSim.CareerManager.DegreeManager != null && pSim.CareerManager.DegreeManager.HasCompletedDegree(name))
                            {
                                // they have...
                                if (pSim.Occupation != null)
                                {
                                    msg += Common.NewLine + "Quit job";
                                    pSim.Occupation.LeaveJobNow(Career.LeaveJobReason.kQuit);

                                    if (pSim.CreatedSim != null)
                                    {
                                        pSim.CreatedSim.InteractionQueue.CancelAllInteractionsByType(WorkInRabbitHole.Singleton);
                                    }

                                    while (pSim.Occupation != null)
                                    {
                                        Common.Sleep(5);
                                    }
                                }

                                bool yes = AcademicCareer.EnrollSimInAcademicCareer(pSim, name, AcademicCareer.ChooseRandomCoursesPerDay());
                                Common.Notify("Success? " + yes);
                                degreesWithEnrollment.Remove(name);
                                pSim.OccupationAsAcademicCareer.PromoteSim();
                                possibleProfessors.Remove(pSim);
                                msg += Common.NewLine + "Enrolling";                                
                                break;
                            }
                        }
                    }                   
                }

                // if we got this far they aren't in uni and haven't completed any degrees...
                if (pSim.Elder && (pSim.Occupation == null || pSim.Occupation is StoryProgressionSpace.Careers.Retired))
                {
                    preferredProfessors.Add(pSim);
                }
            }

            if (degreesWithEnrollment.Count == 0)
            {                
                Common.DebugWriteLog(msg);
                return false;
            }

            if (possibleProfessors.Count == 0)
            {                
                Common.DebugWriteLog(msg);
                return false;
            }

            int loop = 1;
            int count = degreesWithEnrollment.Count;
            msg += Common.NewLine + "Count: " + count; 
            while (loop <= count)
            {
                msg += Common.NewLine + "Loop: " + loop;
                if (degreesWithEnrollment.Count == 0 || possibleProfessors.Count == 0) break;

                SimDescription newP;
                if (preferredProfessors.Count > 0)
                {
                    newP = RandomUtil.GetRandomObjectFromList<SimDescription>(preferredProfessors);
                    preferredProfessors.Remove(newP);
                }
                else
                {
                    newP = RandomUtil.GetRandomObjectFromList<SimDescription>(possibleProfessors);
                }

                AcademicDegreeNames enroll = RandomUtil.GetRandomObjectFromList<AcademicDegreeNames>(degreesWithEnrollment);

                msg += Common.NewLine + "Professor: " + newP.FullName;
                msg += Common.NewLine + "Degree: " + enroll;

                if (newP.Occupation != null)
                {
                    msg += Common.NewLine + "Quit Job";
                    newP.Occupation.LeaveJobNow(Career.LeaveJobReason.kQuit);

                    if (newP.CreatedSim != null)
                    {
                        newP.CreatedSim.InteractionQueue.CancelAllInteractionsByType(WorkInRabbitHole.Singleton);
                    }

                    while (newP.Occupation != null)
                    {
                        Common.Sleep(5);
                    }
                }

                bool su = AcademicCareer.EnrollSimInAcademicCareer(newP, enroll, AcademicCareer.ChooseRandomCoursesPerDay());
                degreesWithEnrollment.Remove(enroll);

                newP.OccupationAsAcademicCareer.PromoteSim();
                loop++;
                possibleProfessors.Remove(newP);
                msg += Common.NewLine + "Success ? " + su;                
            }

            Common.DebugWriteLog(msg);

            return false;
        }

        public override Scenario Clone()
        {
            return new AssignUniversityProfessorsScenario(this);
        }

        public class Option : BooleanAlarmOptionItem<ManagerCareer, AssignUniversityProfessorsScenario>, ManagerCareer.IHomeworldUniversityOption
        {
            static AlarmHandle sMyHandle = AlarmHandle.kInvalidHandle;

            public Option()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "AssignUniversityProfessors";
            }                     
        }

        public class AllowQuitOccupation : BooleanManagerOptionItem<ManagerCareer>, ManagerCareer.IHomeworldUniversityOption
        {
            public AllowQuitOccupation()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ProfessorAllowQuitOccupation";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }

        public class AllowRetiredElder : BooleanManagerOptionItem<ManagerCareer>, ManagerCareer.IHomeworldUniversityOption
        {
            public AllowRetiredElder()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ProfessorAllowRetiredElder";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
