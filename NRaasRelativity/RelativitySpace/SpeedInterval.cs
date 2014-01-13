using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelativitySpace
{
    [Persistable]
    public class SpeedInterval : IPersistence
    {
        static List<DaysOfTheWeek> sAllDays = ParserFunctions.ParseDayList("MTWRFSU");

        public WorldName mWorld;

        public List<DaysOfTheWeek> mDays;

        public int mStartHour;

        public int mEndHour;

        public int mSpeed;

        public SpeedInterval()
        {
            mWorld = GameUtils.GetCurrentWorld();
            mDays = new List<DaysOfTheWeek>(sAllDays);
            mStartHour = 0;
            mEndHour = 24;
            mSpeed = Relativity.sOneMinute;
        }
        public SpeedInterval(WorldName world, List<DaysOfTheWeek> days, int start, int end, int speed)
        {
            mWorld = world;
            mDays = days;
            mStartHour = start;
            mEndHour = end;
            mSpeed = speed;
        }

        public string GetDays()
        {
            if (mDays.Count == 1)
            {
                return Common.Localize("Day:" + mDays[0]);
            }
            else if (mDays.Count == 7)
            {
                return Common.Localize("Day:All");
            }
            else
            {
                string text = null;
                foreach (DaysOfTheWeek day in mDays)
                {
                    text += Common.Localize("DayAbbreviation:" + day);
                }

                return text;
            }
        }

        public override string ToString()
        {
            return GetDays() + Common.Localize("Interval:Hours", false, new object[] { mStartHour, mEndHour });
        }

        public static int OnSort(SpeedInterval left, SpeedInterval right)
        {
            int result = left.mWorld.CompareTo(right.mWorld);
            if (result != 0) return result;

            // These are intentionally inversed (larger numbers to the top)
            result = right.mDays.Count.CompareTo(left.mDays.Count);
            if (result != 0) return result;

            DaysOfTheWeek leftDay = DaysOfTheWeek.None;
            foreach (DaysOfTheWeek day in left.mDays)
            {
                if (leftDay < day)
                {
                    leftDay = day;
                }
            }

            DaysOfTheWeek rightDay = DaysOfTheWeek.None;
            foreach (DaysOfTheWeek day in right.mDays)
            {
                if (rightDay < day)
                {
                    rightDay = day;
                }
            }

            result = leftDay.CompareTo(rightDay);
            if (result != 0) return result;

            result = left.mStartHour.CompareTo(right.mStartHour);
            if (result != 0) return result;

            return 0;
        }

        public void Import(Persistence.Lookup settings)
        {
            mWorld = settings.GetEnum<WorldName>("World", GameUtils.GetCurrentWorld());
            mDays = ParserFunctions.ParseDayList(settings.GetString("Days"));
            mStartHour = settings.GetInt("StartHour", 0);
            mEndHour = settings.GetInt("EndHour", 24);
            mSpeed = settings.GetInt("Speed", Relativity.sOneMinute);
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("World", mWorld.ToString());

            string days = "";
            foreach (DaysOfTheWeek day in mDays)
            {
                switch(day)
                {
                    case DaysOfTheWeek.Monday:
                        days += "M";
                        break;
                    case DaysOfTheWeek.Tuesday:
                        days += "T";
                        break;
                    case DaysOfTheWeek.Wednesday:
                        days += "W";
                        break;
                    case DaysOfTheWeek.Thursday:
                        days += "R";
                        break;
                    case DaysOfTheWeek.Friday:
                        days += "F";
                        break;
                    case DaysOfTheWeek.Saturday:
                        days += "S";
                        break;
                    case DaysOfTheWeek.Sunday:
                        days += "U";
                        break;
                }
            }

            settings.Add("Days", days);

            settings.Add("StartHour", mStartHour);
            settings.Add("EndHour", mEndHour);
            settings.Add("Speed", mSpeed);
        }

        public string PersistencePrefix
        {
            get { return null; }
        }
    }
}
