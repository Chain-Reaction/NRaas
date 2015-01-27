using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TempestSpace.Helpers
{
    [Persistable]
    public class HolidaySettings : IPersistence
    {
        Season mSeason;

        public bool mNoSchool;

        List<Holiday> mDays = new List<Holiday>();

        public HolidaySettings()
        { }
        public HolidaySettings(Season season)
        {
            mSeason = season;
        }

        public Season Season
        {
            get { return mSeason; }
        }

        public string PersistencePrefix
        {
            get { return null; }
        }

        public void Import(Persistence.Lookup settings)
        {
            mSeason = settings.GetEnum<Season>("Season", Season.Summer);
            mNoSchool = settings.GetBool("NoSchool", false);
            mDays = settings.GetList<Holiday>("Days");
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("Season", mSeason.ToString());
            settings.Add("NoSchool", mNoSchool);
            settings.Add("Days", mDays);
        }

        public IEnumerable<Holiday> Days
        {
            get { return mDays; }
        }

        public void Add(Season season)
        {
            mDays.Add(new Holiday(-2, season));
        }

        public void Remove(Holiday day)
        {
            mDays.Remove(day);
        }

        [Persistable]
        public class Holiday : IPersistence
        {
            int mDay;

            public Season mSeason;

            public Holiday()
            { }
            public Holiday(int day, Season season)
            {
                mDay = day;
                mSeason = season;
            }

            public string PersistencePrefix
            {
                get { return null; }
            }

            public void Import(Persistence.Lookup settings)
            {
                mSeason = settings.GetEnum<Season>("Season", Season.Summer);
                mDay = settings.GetInt("Day", 0);
            }

            public void Export(Persistence.Lookup settings)
            {
                settings.Add("Season", mSeason.ToString());
                settings.Add("Day", mDay);
            }

            public int RelativeDay
            {
                get
                {
                    return mDay;
                }
                set
                {
                    mDay = value;
                }
            }

            public uint GetActualDay(Season season)
            {
                return PersistedSettings.GetActualDay(mDay, season); //- 2;
            }

            public string DisplayValue(Season season)
            {
                return EAText.GetNumberString(GetActualDay(season) /*+ 2*/) + ": " + Common.LocalizeEAString("Ui/Tooltips/CareerPanel:" + mSeason + "Holiday");
            }
        }
    }
}
