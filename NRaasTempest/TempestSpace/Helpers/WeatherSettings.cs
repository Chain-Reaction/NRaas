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
    public class WeatherSettings : IPersistence
    {
        static WeatherProfile sCurrentProfile = null;

        Season mSeason;

        List<WeatherProfile> mProfiles = new List<WeatherProfile>();

        public WeatherSettings()
        { }
        public WeatherSettings(Season season, bool createDefault)
        {
            mSeason = season;

            if (createDefault)
            {
                WeatherProfile profile = AddProfile(Common.Localize("DefaultProfile:Name"));

                profile.ReadDefaults(mSeason);
            }
        }

        public Season Season
        {
            get { return mSeason; }
        }

        public int NumProfiles
        {
            get { return mProfiles.Count; }
        }

        public IEnumerable<WeatherProfile> Profiles
        {
            get { return mProfiles; }
        }

        public bool Apply(bool force)
        {
            WeatherProfile profile = GetProfileForToday();
            if ((!force) && (profile == sCurrentProfile)) return false;

            profile.Apply(mSeason);
            sCurrentProfile = profile;
            return true;
        }

        protected WeatherProfile GetProfileForToday()
        {
            int currentDay = Tempest.GetCurrentSeasonDay();

            foreach (WeatherProfile profile in mProfiles)
            {
                profile.CalculateLength(mSeason);
            }

            mProfiles.Sort(WeatherProfile.ReverseSortByLength);

            foreach (WeatherProfile profile in mProfiles)
            {
                if (!profile.mEnabled) continue;

                if (currentDay < profile.GetActualStart(mSeason)) continue;

                if (currentDay > profile.GetActualEnd(mSeason)) continue;

                return profile;
            }

            return mProfiles[0];
        }

        public WeatherProfile AddProfile(WeatherProfile source)
        {
            WeatherProfile profile = new WeatherProfile(source);

            mProfiles.Add(profile);

            return profile;
        }
        public WeatherProfile AddProfile(string name)
        {
            foreach (WeatherProfile oldProfile in mProfiles)
            {
                if (name == oldProfile.Name) return oldProfile;
            }

            WeatherProfile newProfile = new WeatherProfile(name);

            mProfiles.Add(newProfile);

            return newProfile;
        }

        public void RemoveProfile(WeatherProfile profile)
        {
            if (mProfiles.Count == 1) return;

            mProfiles.Remove(profile);
        }

        public string PersistencePrefix
        {
            get { return null; }
        }

        public void Import(Persistence.Lookup settings)
        {
            mSeason = settings.GetEnum<Season>("Season", Season.Summer);

            mProfiles = settings.GetList<WeatherProfile>("Profiles");
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("Season", mSeason.ToString());
            settings.Add("Profiles", mProfiles);
        }
    }
}
