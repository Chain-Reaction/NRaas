using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CareerSpace.Helpers
{
    public class AcademicHelper : Common.IPreLoad, Common.IWorldLoadFinished, Common.IWorldQuit
    {
        [PersistableStatic]
        static Dictionary<ulong, AcademicController> sControllers = new Dictionary<ulong, AcademicController>();

        public void OnPreLoad()
        {
            if (GameUtils.IsInstalled(ProductVersion.EP9))
            {
                foreach (InteractionTuning tuning in InteractionTuning.sAllTunings.Values)
                {
                    switch (tuning.Availability.WorldRestrictionType)
                    {
                        case WorldRestrictionType.Allow:
                            if (tuning.Availability.WorldRestrictionWorldTypes.Contains(WorldType.University))
                            {
                                tuning.Availability.WorldRestrictionType = WorldRestrictionType.None;
                                tuning.Availability.Teens = true;

                                BooterLogger.AddTrace("Tuning Altered: Allowed (A) " + tuning.FullInteractionName + " : " + tuning.FullObjectName);
                            }

                            break;
                    }
                }

                InteractionTuning tuning2 = Tunings.GetTuning<AcademicTextBook, AcademicTextBook.StudyFromBookChooser.Definition>();
                if (tuning2 != null)
                {
                    tuning2.Availability.Teens = true;
                }

                tuning2 = Tunings.GetTuning<AcademicTextBook, AcademicTextBook.StudyFromBook.Definition>();
                if (tuning2 != null)
                {
                    tuning2.Availability.Teens = true;
                }

                tuning2 = Tunings.GetTuning<AcademicTextBook, AcademicTextBook.GetBookToStudyFrom.Definition>();
                if (tuning2 != null)
                {
                    tuning2.Availability.Teens = true;
                }

                tuning2 = Tunings.GetTuning<AcademicTextBook, AcademicTextBook.GetBookToMakeCheatSheet.Definition>();
                if (tuning2 != null)
                {
                    tuning2.Availability.Teens = true;
                }

                tuning2 = Tunings.GetTuning<AcademicTextBook, AcademicTextBook.MakeCheatSheet.Definition>();
                if (tuning2 != null)
                {
                    tuning2.Availability.Teens = true;
                }

                // Some Extra Ones that are locked in University

                tuning2 = Tunings.GetTuning<Homework, Homework.DoHomework.Definition>();
                if (tuning2 != null)
                {
                    tuning2.Availability.WorldRestrictionType = WorldRestrictionType.None;
                }

                tuning2 = Tunings.GetTuning<Homework, Homework.DoHomeworkWith.Definition>();
                if (tuning2 != null)
                {
                    tuning2.Availability.WorldRestrictionType = WorldRestrictionType.None;
                }

                tuning2 = Tunings.GetTuning<Homework, Homework.HelpWithHomework.Definition>();
                if (tuning2 != null)
                {
                    tuning2.Availability.WorldRestrictionType = WorldRestrictionType.None;
                }

                tuning2 = Tunings.GetTuning<Homework, Homework.GetHelpWithHomework.Definition>();
                if (tuning2 != null)
                {
                    tuning2.Availability.WorldRestrictionType = WorldRestrictionType.None;
                }

                tuning2 = Tunings.GetTuning<Homework, Homework.GetHomeworkForDoWith.Definition>();
                if (tuning2 != null)
                {
                    tuning2.Availability.WorldRestrictionType = WorldRestrictionType.None;
                }

                tuning2 = Tunings.GetTuning<Homework, Homework.PickUpHomework.Definition>();
                if (tuning2 != null)
                {
                    tuning2.Availability.WorldRestrictionType = WorldRestrictionType.None;
                }

                tuning2 = Tunings.GetTuning<Homework, Homework.PickUpHomework.Definition>();
                if (tuning2 != null)
                {
                    tuning2.Availability.WorldRestrictionType = WorldRestrictionType.None;
                }

                tuning2 = Tunings.GetTuning<Homework, Homework.CopyHomework.Definition>();
                if (tuning2 != null)
                {
                    tuning2.Availability.WorldRestrictionType = WorldRestrictionType.None;
                }

                tuning2 = Tunings.GetTuning<Homework, Homework.CopyHomeworkReaction.Definition>();
                if (tuning2 != null)
                {
                    tuning2.Availability.WorldRestrictionType = WorldRestrictionType.None;
                }
            }
        }

        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kEventCareerHired, OnHired);

            if (AcademicCareer.GlobalTermLength == AcademicCareer.TermLength.kInvalid)
            {
                AcademicCareer.GlobalTermLength = AcademicCareer.TermLength.kOneWeek;
            }

            foreach (SimDescription sim in SimListing.GetResidents(false).Values)
            {
                if (sim.OccupationAsAcademicCareer == null) continue;

                if (sControllers.ContainsKey(sim.SimDescriptionId)) continue;

                AddAcademic(sim, AcademicCareer.GlobalTermLength);
            }

            new AlarmTask();

            if (!GameUtils.IsUniversityWorld())
            {
                AnnexEx.OnWorldLoadFinished();

                foreach (Stadium stadium in Sims3.Gameplay.Queries.GetObjects<Stadium>())
                {
                    stadium.RabbitHoleProxy.MetaAds.Add(new InteractionObjectPair(Stadium.PlayCollegiateSport.Singleton, stadium.RabbitHoleProxy));
                    stadium.AddInteraction(Stadium.PlayCollegiateSport.Singleton);
                }
            }
        }

        public void OnWorldQuit()
        {
            sControllers.Clear();
        }

        public static bool HasFirstDayActiveStudents()
        {
            if (Household.ActiveHousehold == null) return false;

            foreach (SimDescription sim in Households.All(Household.ActiveHousehold))
            {
                AcademicController controller;
                if (!sControllers.TryGetValue(sim.SimDescriptionId, out controller)) continue;

                if (controller.IsFirstDay) return true;
            }

            return false;
        }

        // From Career
        public static int CalculateBoostedStartingLevel(Occupation career)
        {
            int result = -1;
            int careerLevelBonusFromDegree = -1;
            if (career.OwnerDescription.CareerManager.DegreeManager != null)
            {
                AcademicDegree degreeForOccupation = career.OwnerDescription.CareerManager.DegreeManager.GetDegreeForOccupation(career.Guid);
                if ((degreeForOccupation != null) && degreeForOccupation.IsDegreeCompleted)
                {
                    switch (degreeForOccupation.CurrentGpaAsLetterGrade)
                    {
                        case AcademicsUtility.AcademicGrade.D:
                            careerLevelBonusFromDegree = Career.kCareerBonusesForDegreeWithGradeOfD.kCareerLevelBonusFromDegree;
                            break;

                        case AcademicsUtility.AcademicGrade.C:
                            careerLevelBonusFromDegree = Career.kCareerBonusesForDegreeWithGradeOfC.kCareerLevelBonusFromDegree;
                            break;

                        case AcademicsUtility.AcademicGrade.B:
                            careerLevelBonusFromDegree = Career.kCareerBonusesForDegreeWithGradeOfB.kCareerLevelBonusFromDegree;
                            break;

                        case AcademicsUtility.AcademicGrade.A:
                            careerLevelBonusFromDegree = Career.kCareerBonusesForDegreeWithGradeOfA.kCareerLevelBonusFromDegree;
                            break;
                    }
                }
            }

            result = Math.Max(careerLevelBonusFromDegree, result);
            SocialNetworkingSkill element = career.OwnerDescription.SkillManager.GetElement(SkillNames.SocialNetworking) as SocialNetworkingSkill;
            if (element != null)
            {
                careerLevelBonusFromDegree = element.GetCurrentCareerBoost();
            }

            return Math.Max(careerLevelBonusFromDegree, result);
        }

        protected static void OnHired(Event e)
        {
            CareerEvent cEvent = e as CareerEvent;
            if (cEvent == null) return;

            if (cEvent.Career is AcademicCareer)
            {
                if (!GameUtils.IsUniversityWorld())
                {
                    if (AcademicCareer.GlobalTermLength == AcademicCareer.TermLength.kInvalid)
                    {
                        AcademicCareer.GlobalTermLength = AcademicCareer.TermLength.kOneWeek;
                    }

                    AddAcademic(cEvent.Career.OwnerDescription, AcademicCareer.GlobalTermLength);
                }
            }
            else //if (cEvent.Career is XpBasedCareer)
            {
                if (cEvent.Career.CareerLevel == 1)
                {
                    SimDescription sim = cEvent.Career.OwnerDescription;
                    if (sim == null) return;

                    AcademicDegreeManager degreeManager = sim.CareerManager.DegreeManager;
                    if (degreeManager == null) return;

                    int level = CalculateBoostedStartingLevel(cEvent.Career);

                    for (int i = 0; i < level; i++)
                    {
                        cEvent.Career.PromoteSim();
                    }
                }
            }
        }

        public static void AddAcademic(SimDescription sim, AcademicCareer.TermLength length)
        {
            sControllers[sim.SimDescriptionId] = new AcademicController(length);

            Common.DebugNotify("Academic Added", sim);
        }

        [Persistable]
        public class AcademicController
        {
            int mCurrentDay = 0;

            AcademicCareer.TermLength mLength = AcademicCareer.TermLength.kOneWeek;

            public AcademicController()
            { }
            public AcademicController(AcademicCareer.TermLength length)
            {
                mLength = length;
            }

            public bool IsFirstDay
            {
                get { return (mCurrentDay == 1); }
            }

            public bool IncrementOneDay(SimDescription sim, ref Household.ReportCardHelper reportCardHelper)
            {
                try
                {
                    mCurrentDay++;

                    // Set for possible use in sub-routines
                    AcademicCareer.GlobalTermLength = mLength;

                    int length = (int)mLength * NRaas.Careers.Settings.mHomeworldUniversityTermLength;

                    Common.DebugNotify("Increment One Day\n" + mCurrentDay + "\n" + length, sim);

                    AcademicCareer academicCareer = sim.OccupationAsAcademicCareer;
                    if (academicCareer == null) return false;

                    if (mCurrentDay >= length)
                    {
                        if (SimTypes.IsSelectable(sim))
                        {
                            if (reportCardHelper == null)
                            {
                                reportCardHelper = new Household.ReportCardHelper();
                            }

                            reportCardHelper.AddGPA(sim.SimDescriptionId, academicCareer.GetGradeAsLetter());
                        }

                        AcademicCareerEx.OnTermCompleted(academicCareer);
                        return false;
                    }
                    else if (mCurrentDay == (length - 0x1))
                    {
                        if (SimTypes.IsSelectable(sim))
                        {
                            float num2 = SimClock.HoursUntil(Household.kWhenOneDayLeftTNSAppears);

                            AlarmManager.Global.RemoveAlarm(sim.Household.mLastDayAlarm);
                            sim.Household.mLastDayAlarm = AlarmManager.Global.AddAlarm(num2, TimeUnit.Hours, sim.Household.OneDayLeft, "One Day left TNS", AlarmType.AlwaysPersisted, sim);
                        }
                    }
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);
                }

                return true;
            }
        }

        public class AlarmTask : Common.AlarmTask
        {
            public AlarmTask()
                : base(0, ~DaysOfTheWeek.None)
            { }

            protected override void OnPerform()
            {
                if (GameUtils.IsUniversityWorld()) return;

                Corrections.CleanupAcademics(null);

                Dictionary<ulong, SimDescription> sims = SimListing.GetResidents(false);

                List<ulong> remove = new List<ulong>();

                Household.ReportCardHelper reportCardHelper = null;

                foreach (KeyValuePair<ulong,AcademicController> controller in sControllers)
                {
                    bool success = false;

                    SimDescription sim;
                    if (sims.TryGetValue(controller.Key, out sim))
                    {
                        if (controller.Value.IncrementOneDay(sim, ref reportCardHelper))
                        {
                            success = true;
                        }
                    }

                    if (!success)
                    {
                        remove.Add(controller.Key);
                    }
                }

                if (reportCardHelper != null)
                {
                    float time = SimClock.HoursUntil(Household.kWhenReportCardArrives);
                    AlarmManager.Global.AddAlarm(time, TimeUnit.Hours, reportCardHelper.ReportCardArrives, "Report Card Arrives", AlarmType.AlwaysPersisted, Sim.ActiveActor);
                }

                foreach (ulong value in remove)
                {
                    sControllers.Remove(value);
                }
            }
        }
    }
}
