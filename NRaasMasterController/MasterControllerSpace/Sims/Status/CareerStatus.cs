using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Sims.Intermediate.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Status
{
    public class CareerStatus : SimFromList, IStatusOption
    {
        public override string GetTitlePrefix()
        {
            return "CareerStatus";
        }

        protected override bool TestValid
        {
            get { return false; }
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override OptionResult RunResult
        {
            get { return OptionResult.SuccessRetain; }
        }

        protected static string DaysToString(DaysOfTheWeek days)
        {
            string result = null;

            foreach (DaysOfTheWeek day in Enum.GetValues(typeof(DaysOfTheWeek)))
            {
                if ((day == DaysOfTheWeek.All) || (day == DaysOfTheWeek.None)) continue;

                if ((days & day) == day)
                {
                    result += Common.Localize("DayAbbreviation:" + day);
                }
            }

            return result;
        }

        protected static string GetJob(SimDescription me)
        {
            bool bSeparator = true;

            string msg = null;

            try
            {
                if (me.AssignedRole != null)
                {
                    msg += Common.Localize("Status:Role", me.IsFemale, new object[] { Roles.GetLocalizedName(me.AssignedRole) });

                    DateAndTime start = DateAndTime.Invalid;
                    DateAndTime end = DateAndTime.Invalid;
                    if (Roles.GetRoleHours(me, ref start, ref end))
                    {
                        msg += Common.Localize("Status:CareerHours", me.IsFemale, new object[] { start.ToString(), end.ToString() });
                    }
                }
                
                if (SimTypes.InServicePool(me))
                {
                    msg += Common.Localize("Status:TypeService", me.IsFemale, new object[] { Common.LocalizeEAString("Ui/Caption/Services/Service:" + me.CreatedByService.ServiceType.ToString()) });
                }

                NpcDriversManager.NpcDrivers driverType;
                if (SimTypes.InCarPool(me, out driverType))
                {
                    msg += Common.Localize("Status:TypeService", me.IsFemale, new object[] { Common.Localize("CarpoolType:" + driverType.ToString()) });
                }

                PetPoolType pool = SimTypes.GetPetPool(me);
                if (pool != PetPoolType.None)
                {
                    msg += Common.NewLine + Common.Localize("Status:PetPool", me.IsFemale, new object[] { Common.Localize("PetPool:" + pool) });
                }

                if (me.Occupation != null)
                {
                    msg += Common.Localize("Status:Career", me.IsFemale, new object[] { me.Occupation.CareerName });
                    bSeparator = false;

                    msg += Common.Localize("Status:CareerLevel", me.IsFemale, new object[] { me.Occupation.CurLevelJobTitle, me.Occupation.CareerLevel });

                    int payPerHourBase = 0;
                    int payPerHourExtra = 0;

                    Sims3.Gameplay.Careers.Career career = me.Occupation as Sims3.Gameplay.Careers.Career;
                    if (career != null)
                    {
                        if (career.CurLevel != null)
                        {
                            payPerHourBase = (int)career.CurLevel.PayPerHourBase;
                        }
                        payPerHourExtra = (int)(career.mPayPerHourExtra + career.mBonusPayFromDegreePerHour);
                    }
                    else
                    {
                        payPerHourBase = (int)me.Occupation.PayPerHourOrStipend;
                    }

                    msg += Common.Localize("Status:CareerExtra", me.IsFemale, new object[] { payPerHourBase, payPerHourExtra, me.Occupation.AverageTimeToReachWork });

                    string days = DaysToString(me.Occupation.DaysOfWeekToWork);

                    msg += Common.Localize("Status:CareerDays", me.IsFemale, new object[] { days });

                    msg += Common.Localize("Status:CareerHours", me.IsFemale, new object[] { me.Occupation.StartTimeText, me.Occupation.FinishTimeText });

                    msg += Common.Localize("Status:CareerDaysOff", me.IsFemale, new object[] { me.Occupation.PaidDaysOff, me.Occupation.mUnpaidDaysOff });

                    float performance = StatusJobPerformance.GetPerformance(me);
                    if (performance != 0f)
                    {
                        msg += Common.Localize("Status:Performance", me.IsFemale, new object[] { (int)performance });
                    }

                    if (me.Occupation.Boss != null)
                    {
                        msg += Common.Localize("Status:Boss", me.IsFemale, new object[] { me.Occupation.Boss });
                    }

                    if (me.Occupation.Coworkers != null)
                    {
                        msg += Common.Localize("Status:Coworkers", me.IsFemale, new object[] { me.Occupation.Coworkers.Count });
                        foreach (SimDescription sim in me.Occupation.Coworkers)
                        {
                            if (sim == null) continue;

                            msg += Common.NewLine + sim.FullName;
                        }
                    }
                }
                else if (me.TeenOrAbove)
                {
                    msg += Common.Localize("Status:Career", me.IsFemale, new object[] { Common.Localize("Status:Notemployed") });
                    bSeparator = false;
                }

                if (me.CareerManager != null)
                {
                    if (me.CareerManager.RetiredCareer != null)
                    {
                        if (bSeparator)
                        {
                            msg += Common.NewLine;
                        }

                        msg += Common.Localize("Status:Retired", me.IsFemale, new object[] { me.CareerManager.RetiredCareer.CurLevelJobTitle });
                        msg += Common.Localize("Status:Pension", me.IsFemale, new object[] { me.CareerManager.RetiredCareer.PensionAmount() });
                    }
                }
            }
            catch (Exception e)
            {
                msg += "EXCEPTION";

                Common.Exception(me, null, msg, e);
            }

            return msg;
        }

        protected static string GetSchool(SimDescription me)
        {
            string msg = null;

            if ((me.CareerManager != null) && (me.CareerManager.School != null))
            {
                School school = me.CareerManager.School;

                msg += Common.Localize("Status:School", me.IsFemale, new object[] { school.Name });

                if (school.CurLevel != null)
                {
                    msg += Common.Localize("Status:CareerLevel", me.IsFemale, new object[] { me.CareerManager.School.CurLevel.GetLocalizedName(me), school.CareerLevel });

                    msg += Common.Localize("Status:CareerExtra", me.IsFemale, new object[] { school.CurLevel.PayPerHourBase, school.mPayPerHourExtra, school.AverageTimeToReachWork });
                }

                if (school.Performance != 0f)
                {
                    msg += Common.Localize("Status:Performance", me.IsFemale, new object[] { (int)school.Performance });
                }

                if (school.Coworkers != null)
                {
                    msg += Common.Localize("Status:Coworkers", me.IsFemale, new object[] { school.Coworkers.Count });
                    foreach (SimDescription sim in school.Coworkers)
                    {
                        msg += Common.NewLine + sim.FullName;
                    }
                }

                if ((school.AfterschoolActivities != null) && (school.AfterschoolActivities.Count > 0))
                {
                    msg += Common.Localize("Status:AfterSchoolActivities", me.IsFemale, new object[] { school.AfterschoolActivities.Count });

                    foreach (AfterschoolActivity activity in school.AfterschoolActivities)
                    {
                        msg += Common.Localize("Status:AfterSchoolActivityElement", me.IsFemale, new object[] { AfterschoolActivity.LocalizeString(me.IsFemale, activity.CurrentActivityType.ToString(), new object[0]), DaysToString(activity.DaysForActivity) });
                    }
                }
            }
            else if ((me.Child) || (me.Teen))
            {
                bool found = false;
                if (me.BoardingSchool != null)
                {
                    BoardingSchool.BoardingSchoolData data;
                    if (BoardingSchool.BoardingSchoolData.sBoardingSchoolDataList.TryGetValue(me.BoardingSchool.CurrentSchoolType, out data))
                    {
                        msg += Common.Localize("Status:School", me.IsFemale, new object[] { Common.LocalizeEAString(data.SchoolNameKey) });
                        found = true;
                    }
                }

                if (!found)
                {
                    msg += Common.Localize("Status:School", me.IsFemale, new object[] { Common.Localize("Status:NoSchool") });
                }
            }
            else if (me.YoungAdultOrAbove)
            {
                msg += Common.Localize("Status:AlmaMater", me.IsFemale, new object[] { me.AlmaMaterName });
            }

            return msg;
        }

        public string GetDetails(IMiniSimDescription miniSim)
        {
            SimDescription me = miniSim as SimDescription;
            if (me == null) return null;

            string msg = PersonalStatus.GetHeader(me);

            msg += GetJob(me);

            msg += Common.NewLine + GetSchool(me);

            if ((me.CareerManager != null) && (me.CareerManager.QuitCareers != null) && (me.CareerManager.QuitCareers.Count > 0))
            {
                msg += Common.Localize("Status:QuitCareers", me.IsFemale, new object[] { me.CareerManager.QuitCareers.Count });
                foreach (Occupation career in me.CareerManager.QuitCareers.Values)
                {
                    msg += Common.Localize("Status:QuitCareer", me.IsFemale, new object[] { career.CareerName, career.CareerLevel });
                }
            }
            return msg;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            SimpleMessageDialog.Show(Name, GetDetails(me));
            return true;
        }
    }
}
