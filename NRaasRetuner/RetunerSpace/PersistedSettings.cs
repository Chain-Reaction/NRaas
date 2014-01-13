using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.RetunerSpace.Booters;
using NRaas.RetunerSpace.Helpers.Stores;
using NRaas.RetunerSpace.Options.Tunable;
using NRaas.RetunerSpace.Options.Tunable.Fields;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RetunerSpace
{
    [Persistable]
    public class PersistedSettings : IPersistence
    {
        public enum Autonomy : int
        {
            NoChange = 0,
            Autonomous,
            NonAutonomous,
        }

        [Tunable, TunableComment("Whether to display the menu on each object")]
        protected static bool kShowObjectMenu = false;

        [Tunable, TunableComment("A percent factor by which to adjust all fun advertised interactions in the game")]
        public static int kFunFactor = 100;

        [Tunable, TunableComment("Whether to mass change all ITUN to be autonomous or not (NoChange, Autonomous, NonAutonomous)")]
        public static Autonomy kInteractionAutonomy = Autonomy.NoChange;

        Dictionary<Season, List<SeasonSettings>> mSettings = null;
        
        static SeasonSettings sEmptySettings = null;

        public bool mShowObjectMenu = kShowObjectMenu;

        public int mFunFactor = kFunFactor;

        public Autonomy mInteractionAutonomy = kInteractionAutonomy;

        bool mDebugging = false;

        public PersistedSettings()
        {}

        public bool Debugging
        {
            get
            {
                return mDebugging;
            }
            set
            {
                mDebugging = value;

                Common.kDebugging = value;
            }
        }

        public string PersistencePrefix
        {
            get { return null; }
        }

        public IEnumerable<SettingsKey> Keys
        {
            get
            {
                List<SettingsKey> results = new List<SettingsKey>();

                foreach (List<SeasonSettings> settings in SeasonSettings.Values)
                {
                    foreach (SeasonSettings setting in settings)
                    {
                        results.Add(setting.Key);
                    }
                }

                return results;
            }
        }

        public string ToXMLString()
        {
            List<SeasonSettings> allSettings = new List<SeasonSettings>();

            foreach (List<SeasonSettings> list in SeasonSettings.Values)
            {
                allSettings.AddRange(list);
            }

            return RetunerSpace.SeasonSettings.ToXMLString(allSettings);
        }

        public void Import(Persistence.Lookup settings)
        {
            SeasonSettings.Clear();
            foreach (SeasonSettings setting in settings.GetList<SeasonSettings>("Settings"))
            {
                List<SeasonSettings> list;
                if (!SeasonSettings.TryGetValue(setting.Key.Season, out list))
                {
                    list = new List<SeasonSettings>();
                    SeasonSettings.Add(setting.Key.Season, list);
                }

                list.Add(setting);
            }

            Apply();
        }

        public void Export(Persistence.Lookup settings)
        {
            List<SeasonSettings> allSettings = new List<SeasonSettings>();

            foreach (List<SeasonSettings> list in SeasonSettings.Values)
            {
                allSettings.AddRange(list);
            }

            settings.Add("Settings", allSettings);
        }

        public bool Exists(SettingsKey key)
        {
            List<SeasonSettings> list;
            if (!SeasonSettings.TryGetValue(key.Season, out list)) return false;

            foreach (SeasonSettings settings in list)
            {
                if (settings.Key.IsEqual(key)) return true;
            }

            return false;
        }

        public SeasonSettings Add(SettingsKey key, bool checkExist)
        {
            if (checkExist)
            {
                if (Exists(key)) return null;
            }

            List<SeasonSettings> list;
            if (!SeasonSettings.TryGetValue(key.Season, out list))
            {
                list = new List<SeasonSettings>();

                SeasonSettings.Add(key.Season, list);
            }

            SeasonSettings settings = new SeasonSettings(key);

            list.Add(settings);

            return settings;
        }

        public void Remove(SettingsKey key)
        {
            List<SeasonSettings> list;
            if (!SeasonSettings.TryGetValue(key.Season, out list)) return;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Key.IsEqual(key))
                {
                    list.RemoveAt(i);
                }
            }
        }

        protected Dictionary<Season, List<SeasonSettings>> SeasonSettings
        {
            get
            {
                if (mSettings == null)
                {
                    mSettings = new Dictionary<Season, List<SeasonSettings>>();

                    Add(new GeneralKey(), false);

                    if (GameUtils.IsInstalled(ProductVersion.EP8))
                    {
                        Add(new SeasonKey(Season.Spring), false);
                        Add(new SeasonKey(Season.Summer), false);
                        Add(new SeasonKey(Season.Fall), false);
                        Add(new SeasonKey(Season.Winter), false);
                    }

                    foreach (SettingsKey key in InteractionBooter.Keys)
                    {
                        Add(key, true);
                    }
                    foreach (SettingsKey key in SocialBooter.Keys)
                    {
                        Add(key, true);
                    }
                    foreach (SettingsKey key in TuningBooter.Keys)
                    {
                        Add(key, true);
                    }

                    foreach (List<SeasonSettings> settings in mSettings.Values)
                    {
                        foreach (SeasonSettings setting in settings)
                        {
                            setting.SetToDefault();
                        }
                    }
                }

                return mSettings;
            }
        }

        public SeasonSettings GetSettings(SettingsKey key)
        {
            SeasonSettings result;

            if (key is CurrentKey)
            {
                if (sEmptySettings == null)
                {
                    sEmptySettings = new SeasonSettings(key);
                }

                result = sEmptySettings;
            }
            else
            {
                List<SeasonSettings> list;
                if (!SeasonSettings.TryGetValue(key.Season, out list)) return null;

                result = null;
                foreach (SeasonSettings setting in list)
                {
                    if (setting.IsEqual(key))
                    {
                        result = setting;
                        break;
                    }
                }

                if (result == null) return null;
            }

            result.SetKey(key);

            return result;
        }

        public void Apply()
        {
            Apply(SettingsKey.sAllSeasons);

            if (SeasonsManager.Enabled)
            {
                Apply(SeasonsManager.CurrentSeason);
            }
        }
        protected void Apply(Season season)
        {
            List<SeasonSettings> list;
            if (!SeasonSettings.TryGetValue(season, out list)) return;

            foreach (SeasonSettings setting in list)
            {
                if (!setting.Key.IsActive) continue;

                setting.Apply();
            }
        }

        // Legacy Compatibility with Version 1
        public class Settings : SeasonSettings.ITUNSettings
        {
            public Settings()
            { }
        }
    }
}
