using NRaas.CommonSpace.Dialogs;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class AcademicCareerEx
    {
        public static void OnTermCompleted(AcademicCareer ths)
        {
            float gradeAsPercentage = ths.GetGradeAsPercentage();
            OnTermCompleted(ths, gradeAsPercentage);
        }
        public static void OnTermCompleted(AcademicCareer ths, float gradeAsPercentage)
        {
            if (ths.OwnerDescription == null)
            {
                throw new NullReferenceException("OwnerDescription");
            }
            else if (ths.OwnerDescription.CareerManager == null)
            {
                throw new NullReferenceException("CareerManager");
            }
            else if (ths.OwnerDescription.CareerManager.DegreeManager == null)
            {
                throw new NullReferenceException("DegreeManager");
            }
            else if (ths.DegreeInformation == null)
            {
                throw new NullReferenceException("DegreeInformation");
            }

            if (ths.OwnerDescription.CareerManager.DegreeManager.CommitAcademicProgressToDegree(ths.OwnerDescription, ths.DegreeInformation.AcademicDegreeName, (int)ths.mNumberOfCoursesPerDay, gradeAsPercentage, (uint)AcademicCareer.sTermLengthEx))
            {
                CollegeGraduationEx.GraduateSim(ths.OwnerDescription, ths.mDegree);
            }

            ActiveTopic.RemoveTopicFromSim(ths.OwnerSim, "During University");
            ths.LeaveJob(false, Career.LeaveJobReason.kNone);
        }

        public static bool EnrollInAcademicCareer(Sim actor, TravelUtilEx.Type filter, out List<SimDescription> others, out int tuitionCost)
        {
            List<SimDescription> choices = new List<SimDescription>();
            foreach (KeyValuePair<SimDescription, string> key in CommonSpace.Helpers.TravelUtilEx.GetTravelChoices(actor, filter, true))
            {
                if (!string.IsNullOrEmpty(key.Value)) continue;

                choices.Add(key.Key);
            }

            return EnrollInAcademicCareer(actor, choices, out others, out tuitionCost);
        }
        public static bool EnrollInAcademicCareer(Sim actor, IEnumerable<SimDescription> choices, out List<SimDescription> others, out int tuitionCost)
        {
            others = null;
            tuitionCost = 0;
            if (!IntroTutorial.IsRunning || IntroTutorial.AreYouExitingTutorial())
            {
                if ((actor.DegreeManager != null) && actor.DegreeManager.ShouldDisplaySUATMessage(actor.SimDescription))
                {
                    SimpleMessageDialog.Show(Localization.LocalizeString("Gameplay/Objects/Electronics/Phone:HaveYouTakenSUAT", new object[0]), Localization.LocalizeString("Gameplay/Objects/Electronics/Phone:SUATBenefits", new object[0]), ModalDialog.PauseMode.PauseSimulator);
                }

                if (!UIUtils.IsOkayToStartModalDialog())
                {
                    return false;
                }

                int termLen = 0;
                int totalCost = 0;
                List<IEnrollmentData> list = EnrollmentDialogEx.Show(actor.SimDescription, choices, false, out termLen, out totalCost);
                if ((list != null) && (list.Count > 0))
                {
                    TravelUtil.PlayerMadeTravelRequest = true;
                    foreach (IEnrollmentData data in list)
                    {
                        SimDescription enrollingSimDesc = data.EnrollingSimDesc as SimDescription;
                        if (((enrollingSimDesc != null) && (enrollingSimDesc.CareerManager != null)) && (enrollingSimDesc.CareerManager.DegreeManager != null))
                        {
                            if (others == null)
                            {
                                others = new List<SimDescription>();
                            }
                            others.Add(enrollingSimDesc);
                            enrollingSimDesc.CareerManager.DegreeManager.SetEnrollmentData(data);
                            EventTracker.SendEvent(new AcademicEvent(EventTypeId.kEnrolledInUniversity, enrollingSimDesc.CreatedSim, (AcademicDegreeNames)data.AcademicDegreeName));
                        }
                    }
                    tuitionCost = -totalCost;

                    AcademicCareer.GlobalTermLength = (AcademicCareer.TermLength)termLen;
                    return true;
                }
            }
            return false;
        }

        public static void Enroll(List<SimDescription> sims)
        {
            bool promptShown = false;

            foreach (SimDescription sim in sims)
            {
                if (sim.Occupation != null)
                {
                    sim.Occupation.LeaveJob(false, Career.LeaveJobReason.kJobBecameInvalid);
                }

                AcademicCareer.EnrollSimInAcademicCareer(sim, sim.CareerManager.DegreeManager.EnrollmentAcademicDegreeName, sim.CareerManager.DegreeManager.EnrollmentCouseLoad);
                if (!promptShown)
                {
                    string becameAStudentTnsText = sim.OccupationAsAcademicCareer.GetBecameAStudentTnsText();
                    if (!string.IsNullOrEmpty(becameAStudentTnsText))
                    {
                        sim.Occupation.ShowOccupationTNS(becameAStudentTnsText, false);
                    }
                    promptShown = true;
                }

                CustomAcademicDegrees.AdjustCustomAcademics(sim);
            }
        }
    }
}