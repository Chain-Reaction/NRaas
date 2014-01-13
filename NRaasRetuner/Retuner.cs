using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.RetunerSpace;
using NRaas.RetunerSpace.Booters;
using NRaas.RetunerSpace.Helpers.Stores;
using NRaas.RetunerSpace.Options;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using System;
using System.Collections.Generic;

namespace NRaas
{
    public class Retuner : Common, Common.IWorldLoadFinished, Common.IWorldQuit
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        static SeasonSettings sActiveSettings = null;

        static SeasonSettings sDefaultSettings = new SeasonSettings(new CurrentKey());

        static List<AlarmTask> sAlarms = new List<AlarmTask>();

        static Retuner()
        {
            Bootstrap();

            BooterHelper.Add(new TuningBooter());
            BooterHelper.Add(new InteractionBooter());
            BooterHelper.Add(new SocialBooter());
        }

        public static PersistedSettings Settings
        {
            get
            {
                if (sSettings == null)
                {
                    sSettings = new PersistedSettings();
                }

                return sSettings;
            }
        }

        public static SeasonSettings SeasonSettings
        {
            get
            {
                if (sActiveSettings == null)
                {
                    sActiveSettings = Settings.GetSettings(new GeneralKey());
                }

                return sActiveSettings;
            }
        }

        public static void ResetSettings()
        {
            ApplyDefaultSettings();

            sSettings = null;

            FunFactor.ApplyFunFactor();

            ApplySettings();

            StartAlarms();
        }

        public void OnWorldLoadFinished()
        {
            if (GameUtils.IsInstalled(ProductVersion.EP8))
            {
                Settings.Add(new SeasonKey(Season.Spring), true);
                Settings.Add(new SeasonKey(Season.Summer), true);
                Settings.Add(new SeasonKey(Season.Fall), true);
                Settings.Add(new SeasonKey(Season.Winter), true);
            }

            ApplySettings();

            new Common.DelayedEventListener(EventTypeId.kSeasonTransition, OnSeasonChanged);

            StartAlarms();

            kDebugging = Settings.Debugging;
        }

        public static void StartAlarms()
        {
            foreach (AlarmTask task in sAlarms)
            {
                task.Dispose();
            }

            sAlarms.Clear();

            Dictionary<float,bool> times = new Dictionary<float,bool>();

            foreach (SettingsKey key in Settings.Keys)
            {
                if (key.IsDefault) continue;

                float hour = key.mHours.x;
                if (!times.ContainsKey(hour))
                {
                    sAlarms.Add(new AlarmTask(hour + 0.01f, DaysOfTheWeek.All, ApplySettings));
                    times.Add(hour, true);
                }

                hour = key.mHours.y;
                if (!times.ContainsKey(hour))
                {
                    sAlarms.Add(new AlarmTask(hour + 0.01f, DaysOfTheWeek.All, ApplySettings));
                    times.Add(hour, true);
                }
            }
        }

        public static void ApplySettings()
        {
            ApplyDefaultSettings();

            ITUNAutonomy.ApplyChanges();

            FunFactor.ApplyFunFactor();

            Settings.Apply();
        }

        public void OnWorldQuit()
        {
            ApplyDefaultSettings();
        }

        protected static void OnSeasonChanged(Event e)
        {
            SeasonTransitionEvent seasonEvent = e as SeasonTransitionEvent;
            if (seasonEvent == null) return;

            ApplySettings();
        }

        public static string TuningName(string objectName, string interactionName)
        {
            return objectName + "|" + interactionName;
        }

        public static List<CASAGSAvailabilityFlags> AgeSpeciesToList(CASAGSAvailabilityFlags flags)
        {
            List<CASAGSAvailabilityFlags> results = new List<CASAGSAvailabilityFlags>();

            foreach (CASAGSAvailabilityFlags ageSpecies in Enum.GetValues(typeof(CASAGSAvailabilityFlags)))
            {
                if (!Allow(ageSpecies)) continue;

                if ((flags & ageSpecies) == ageSpecies)
                {
                    results.Add(ageSpecies);
                }
            }

            return results;
        }

        public static bool Allow(CASAGSAvailabilityFlags value)
        {
            switch (value)
            {
                case CASAGSAvailabilityFlags.All:
                case CASAGSAvailabilityFlags.AllAnimalsMask:
                case CASAGSAvailabilityFlags.AllCatsMask:
                case CASAGSAvailabilityFlags.AllDogsMask:
                case CASAGSAvailabilityFlags.AllDomesticAnimalsMask:
                case CASAGSAvailabilityFlags.AllHorsesMask:
                case CASAGSAvailabilityFlags.AllLittleDogsMask:
                case CASAGSAvailabilityFlags.AllWildAnimalsMask:
                case CASAGSAvailabilityFlags.Female:
                case CASAGSAvailabilityFlags.GenderMask:
                case CASAGSAvailabilityFlags.HandednessMask:
                case CASAGSAvailabilityFlags.HumanAgeMask:
                case CASAGSAvailabilityFlags.LeftHanded:
                case CASAGSAvailabilityFlags.Male:
                case CASAGSAvailabilityFlags.None:
                case CASAGSAvailabilityFlags.RightHanded:
                case CASAGSAvailabilityFlags.SimLeadingHorse:
                case CASAGSAvailabilityFlags.SimWalkingDog:
                case CASAGSAvailabilityFlags.SimWalkingLittleDog:
                    return false;
            }

            return true;
        }

        public static bool StoreDefault(SettingsKey key, InteractionTuning tuning)
        {
            if (!key.IsActive) return false;

            sDefaultSettings.StoreDefaultSettings(tuning);
            return true;
        }
        public static bool StoreDefault(SettingsKey key, ActionData data)
        {
            if (!key.IsActive) return false;

            sDefaultSettings.StoreDefaultSettings(data);
            return true;
        }
        public static bool StoreDefault(SettingsKey key, TunableStore store)
        {
            if (!key.IsActive) return false;

            if (store == null) return false;

            sDefaultSettings.AddTunable(store, false);
            return true;
        }

        public static void ApplyDefaultSettings()
        {
            sDefaultSettings.Apply();
            sDefaultSettings = new SeasonSettings(new GeneralKey());
        }

        public class ActiveSettingsToggle : IDisposable
        {
            public ActiveSettingsToggle(SettingsKey season)
            {
                sActiveSettings = Retuner.Settings.GetSettings(season);
            }

            public void Dispose()
            {
                sActiveSettings = null;
            }
        }

        public class SettingPersistence : Persistence
        {
            public override string PersistencePrefix
            {
                get { return ""; }
            }

            public override void Import(Persistence.Lookup settings)
            {
                sSettings = settings.GetChild<PersistedSettings>("Settings");

                SeasonSettings generalSettings = Settings.GetSettings(new GeneralKey());

                // Backwards compatibility with Version 1
                foreach (PersistedSettings.Settings setting in settings.GetList<PersistedSettings.Settings>("Settings"))
                {
                    generalSettings.ApplyLegacySetting(setting);
                }

                Common.FunctionTask.Perform(ApplySettings);
            }

            public override void Export(Persistence.Lookup settings)
            {
                settings.AddChild("Settings", sSettings);
            }
        }
    }
}
