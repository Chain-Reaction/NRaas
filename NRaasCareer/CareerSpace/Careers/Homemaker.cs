using NRaas.CareerSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Careers
{
    public class Homemaker : SkillBasedCareer
    {
        public enum StipendGrade
        {
            A = 4,
            B = 3,
            C = 2,
            D = 1,
            F = 0,
        }

        public enum StipendValue
        {
            Undefined,
            TaughtChildSkill,
            ChildLearnedSkill,
            ParentLearnedSkill,
            ImprovedHygiene,
            MantainedHome,
            ChildCleanedUp,
            ParentCleanedUp,
            AgedUpWell,
            Homework,
            LearnedRecipe,
            PlayedWith,
            Tutored,
            BadMoodlet,
            AfterschoolActivity,
            MotiveFailure,
            BeingBad,
            UnsafeConditions,
            NegativeEnvironment,
            GoodMoodlet,
            RepairedObject,
            DirtyObject,
            PositiveSocial,
            NegativeSocial,
            CookedMeal,
            ReadBook,
            BoughtObject,
        }

        [Persistable(false)]
        AlarmHandle mStipendAlarm;

        Dictionary<DaysOfTheWeek, StipendCalculation> mStipend = new Dictionary<DaysOfTheWeek, StipendCalculation>();

        int mDiscount;

        public Homemaker()
        { }
        public Homemaker(OccupationNames guid)
            : base(guid)
        { }

        public bool HasDiscount
        {
            get { return (Level >= NRaas.Careers.Settings.mHomemakerLevelDiscount); }
        }

        public bool ImmuneToStirCrazy
        {
            get { return (Level >= NRaas.Careers.Settings.mHomemakerLevelStirCrazy); }
        }

        public override bool CanRetire()
        {
            return false;
        }

        public void AddDiscount(int value)
        {
            mDiscount += value;
        }

        public override int TotalCareerMoneyEarned()
        {
            int currentStipend = 0;

            int bonus = 0;
            int totalStipend = 0;
            foreach (KeyValuePair<DaysOfTheWeek,StipendCalculation> value in mStipend)
            {
                int dayStiped = value.Value.Stipend;

                if (value.Key == SimClock.CurrentDayOfWeek)
                {
                    currentStipend = dayStiped;

                    if (dayStiped > 0)
                    {
                        bonus = (int)base.PayPerHourOrStipend;
                    }
                }

                totalStipend += dayStiped;
            }

            if (totalStipend < 0)
            {
                totalStipend = 0;
                currentStipend = 0;
            }
            else if (currentStipend < 0)
            {
                currentStipend = 0;
            }

            return currentStipend + bonus + mDiscount;
        }

        public override float PayPerHourOrStipend
        {
            get { return 0; }
        }

        public void DumpLog()
        {
            Common.StringBuilder debugging = new Common.StringBuilder("Homemaker Stipend " + OwnerDescription.FullName);

            foreach (KeyValuePair<DaysOfTheWeek, StipendCalculation> day in mStipend)
            {
                debugging += Common.NewLine + "Day: " + day.Key;

                StipendGrade dayGrade = day.Value.CalculateGrade(this, ref debugging);

                debugging += Common.NewLine + " Grade: " + dayGrade;
            }

            Common.DebugWriteLog(debugging);
        }

        public override bool OnLoadFixup(bool isQuitCareer)
        {
            if (!base.OnLoadFixup(isQuitCareer)) return false;

            RemoveStipendAlarm();
            SetupStipendAlarm();

            return true;
        }

        public override void OnLevelUp()
        {
            base.OnLevelUp();

            if (Level >= NRaas.Careers.Settings.mHomemakerLevelLifetimeRewards)
            {
                OwnerDescription.TraitManager.AddElement(TraitNames.BornToCook);
                OwnerDescription.TraitManager.AddElement(TraitNames.SpeedyCleaner);
                OwnerDescription.TraitManager.AddElement(TraitNames.SuperNanny);
                OwnerDescription.TraitManager.AddElement(TraitNames.MakesNoMesses);
            }
        }

        public override void LeaveJobNow(Career.LeaveJobReason reason)
        {
            base.LeaveJobNow(reason);

            RemoveStipendAlarm();
        }

        public override void Cleanup()
        {
            base.Cleanup();

            RemoveStipendAlarm();
        }

        public override void SetAttributesForNewJob(CareerLocation location, ulong lotId, bool bRequestFromCharacterImport)
        {
            base.SetAttributesForNewJob(location, lotId, bRequestFromCharacterImport);

            RemoveStipendAlarm();
            SetupStipendAlarm();
        }

        private void RemoveStipendAlarm()
        {
            AlarmManager.Global.RemoveAlarm(mStipendAlarm);
            this.mStipendAlarm = AlarmHandle.kInvalidHandle;
        }

        private void SetupStipendAlarm()
        {
            if (OwnerDescription.Occupation == this)
            {
                AlarmManager.Global.RemoveAlarm(mStipendAlarm);
                mStipendAlarm = AlarmManager.Global.AddAlarmDay(23.9f, DaysOfTheWeek.All, StipendAlarmCallback, "Daily stipend.", AlarmType.NeverPersisted, OwnerDescription);
            }
        }

        public string GetNotice()
        {
            string notice = Common.Localize("HomemakerReport:Title", OwnerDescription.IsFemale, new object[] { OwnerDescription });

            int dayCount = 0;

            foreach (DaysOfTheWeek day in Enum.GetValues(typeof(DaysOfTheWeek)))
            {
                StipendCalculation calc;
                if (!mStipend.TryGetValue(day, out calc)) continue;

                int positive;
                int negative;
                Common.StringBuilder debugging = new Common.StringBuilder();
                int stiped = calc.CalculateStipend(out positive, out negative, ref debugging);

                if (stiped > 0)
                {
                    stiped += (int)base.PayPerHourOrStipend;
                }
                if (stiped < 0)
                {
                    stiped = 0;
                }

                notice += Common.Localize("HomemakerReport:Day", OwnerDescription.IsFemale, new object[] { Common.Localize("HomemakerReport:" + day), negative, positive, stiped });
                dayCount++;
            }

            notice += Common.Localize("HomemakerReport:Footer", OwnerDescription.IsFemale, new object[] { dayCount });

            return notice;
        }

        private void StipendAlarmCallback()
        {
            int bonus = 0;

            int payPerHourOrStipend = (int)TotalCareerMoneyEarned();

            if (mStipend.Count == 7)
            {
                Common.StringBuilder debugging = new Common.StringBuilder("Homemaker Stipend " + OwnerDescription.FullName);

                int positive = 0, negative = 0;

                foreach (KeyValuePair<DaysOfTheWeek, StipendCalculation> day in mStipend)
                {
                    debugging += Common.NewLine + "Day: " + day.Key;

                    StipendGrade dayGrade = day.Value.CalculateGrade(this, ref debugging);

                    debugging += Common.NewLine + " Grade: " + dayGrade;

                    switch (dayGrade)
                    {
                        case StipendGrade.A:
                        case StipendGrade.B:
                            positive++;
                            break;
                        case StipendGrade.C:
                            // C is a day with no change up or down
                            break;
                        case StipendGrade.D:
                        case StipendGrade.F:
                            negative++;
                            break;
                    }
                }

                StipendGrade grade = StipendGrade.F;

                if (negative >= 5)
                {
                    grade = StipendGrade.F;

                    if ((Level > 1) || (SimTypes.IsSelectable(OwnerDescription)))
                    {
                        DemoteSim();
                    }

                    mXp = 0;
                }
                else if (positive >= 5)
                {
                    grade = StipendGrade.A;

                    // Get the base value defining the bonus
                    bonus = (int)base.PayPerHourOrStipend;
                }
                else
                {
                    if (positive > negative)
                    {
                        grade = StipendGrade.B;
                    }
                    else
                    {
                        grade = StipendGrade.C;
                    }
                }

                Common.DebugWriteLog(debugging);

                if (SimTypes.IsSelectable(OwnerDescription))
                {
                    Common.Notify(OwnerDescription, Common.Localize("Homemaker:Grade" + grade, OwnerDescription.IsFemale, new object[] { OwnerDescription, payPerHourOrStipend, bonus }));
                }

                mStipend.Clear();
            }
            else if (payPerHourOrStipend > 0)
            {
                if (SimTypes.IsSelectable(OwnerDescription))
                {
                    Common.Notify(OwnerDescription, Common.Localize("Homemaker:GrantNotice", OwnerDescription.IsFemale, new object[] { OwnerDescription, payPerHourOrStipend }));
                }
            }

            UpdateXp(payPerHourOrStipend + bonus);

            PayOwnerSim(payPerHourOrStipend + bonus, GotPaidEvent.PayType.kCareerNormalPay);

            mDiscount = 0;
        }

        public static void AddMarks(List<Homemaker> careers, StipendValue value, int mark)
        {
            if (careers == null) return;

            foreach (Homemaker career in careers)
            {
                if (career == null) continue;

                career.AddMark(value, mark);
            }
        }

        public void AddMark(StipendValue value, int mark)
        {
            StipendCalculation day;
            if (!mStipend.TryGetValue(SimClock.CurrentDayOfWeek, out day))
            {
                day = new StipendCalculation();
                mStipend.Add(SimClock.CurrentDayOfWeek, day);
            }

            day.AddMark(value, mark);

            OwnerDescription.CareerManager.UpdateCareerUI();
        }

        [Persistable]
        public class StipendCalculation
        {
            Dictionary<StipendValue, int> mValues = new Dictionary<StipendValue, int>();

            public StipendCalculation()
            { }

            public int CalculateStipend(out int positive, out int negative, ref Common.StringBuilder debugging)
            {
                positive = 0;
                negative = 0;

                foreach (KeyValuePair<StipendValue, int> value in mValues)
                {
                    int marks = value.Value;

                    HomemakerBooter.Data control = HomemakerBooter.GetStipend(value.Key);

                    if ((control.mMaximum > 0) && (marks > control.mMaximum))
                    {
                        marks = control.mMaximum;
                    }

                    marks *= control.mFactor;

                    if (control.mPositive)
                    {
                        positive += marks;

                        if (Common.kDebugging)
                        {
                            debugging += Common.NewLine + " " + value.Key + " P=" + marks + " F=" + control.mFactor;
                        }
                    }
                    else
                    {
                        negative += marks;

                        if (Common.kDebugging)
                        {
                            debugging += Common.NewLine + " " + value.Key + " N=" + marks + " F=" + control.mFactor;
                        }
                    }
                }

                int totalMarks = positive - negative;

                if (Common.kDebugging)
                {
                    debugging += Common.NewLine + " T=" + totalMarks + " P=" + positive + " N=" + negative;
                }

                /*
                if (totalMarks < -50)
                {
                    totalMarks = -50;
                }
                */
                return (totalMarks * NRaas.Careers.Settings.mHomemakerPayPerMark);
            }

            public int Stipend
            {
                get
                {
                    Common.StringBuilder debugging = new Common.StringBuilder();
                    int positive;
                    int negative;
                    return CalculateStipend(out positive, out negative, ref debugging);
                }
            }

            public StipendGrade CalculateGrade(Homemaker career, ref Common.StringBuilder debugging)
            {
                int positive;
                int negative;
                CalculateStipend(out positive, out negative, ref debugging);

                if (positive == 0)
                {
                    if (negative > 0)
                    {
                        return StipendGrade.F;
                    }
                    else
                    {
                        return StipendGrade.C;
                    }
                }
                else
                {
                    if (negative == 0)
                    {
                        return StipendGrade.A;
                    }
                    else if (positive > negative)
                    {
                        return StipendGrade.B;
                    }
                    else
                    {
                        return StipendGrade.D;
                    }
                }
            }

            public void Reset()
            {
                mValues.Clear ();
            }

            public void AddMark(StipendValue value, int mark)
            {
                int oldMark;
                if (!mValues.TryGetValue(value, out oldMark))
                {
                    oldMark = 0;
                }

                oldMark += mark;

                mValues[value] = oldMark;
            }
        }
    }
}
