using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Replacers;
using NRaas.TempestSpace;
using NRaas.TempestSpace.Booters;
using NRaas.TempestSpace.Helpers;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using System;
using System.Collections.Generic;

namespace NRaas
{
    public class Tempest : Common, Common.IPreLoad, Common.IWorldLoadFinished, Common.IDelayedWorldLoadFinished, Common.IWorldQuit
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        static Dictionary<Season, WeatherProfile> sDefaultProfiles = null;

        static AlarmTask sApplySettings = null;

        static Tempest()
        {
            Bootstrap();
        }

        public static PersistedSettings Settings
        {
            get
            {
                if (sSettings == null)
                {
                    sSettings = new PersistedSettings();

                    List<BySeasonBooter> booters = new List<BySeasonBooter>();

                    booters.Add(new TemperatureBooter());

                    foreach(Weather weather in Enum.GetValues(typeof(Weather)))
                    {
                        switch (weather)
                        {
                            case Weather.BewitchingRain:
                            case Weather.None:
                            case Weather.RevivingSprinkle:
                                continue;
                        }

                        booters.Add(new WeatherBooter(weather));
                    }

                    foreach (BySeasonBooter booter in booters)
                    {
                        booter.Perform(true);
                    }
                }

                return sSettings;
            }
        }

        public void OnPreLoad()
        {
            BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("Trick Or Treat", "OnTrickOrTreatAccept"));
        }

        public void OnWorldLoadFinished()
        {
            kDebugging = Settings.Debugging;            

            if (sDefaultProfiles == null)
            {
                sDefaultProfiles = new Dictionary<Season, WeatherProfile>();

                foreach (Season season in Enum.GetValues(typeof(Season)))
                {
                    WeatherProfile profile = new WeatherProfile(Common.Localize("DefaultProfile:Name"));
                    profile.ReadDefaults(season);

                    sDefaultProfiles.Add(season, profile);
                }
            }

            new Common.AlarmTask(0.05f, DaysOfTheWeek.All, OnNewDay);
        }

        public void OnDelayedWorldLoadFinished()
        {
            Settings.Apply(false, false);
        }

        public void OnWorldQuit()
        {
            ApplyDefaultProfiles();
        }

        protected static void ApplyDefaultProfiles()
        {
            foreach (KeyValuePair<Season, WeatherProfile> profile in sDefaultProfiles)
            {
                profile.Value.Apply(profile.Key);
            }
        }

        public static void ResetSettings()
        {
            sSettings = null;

            ApplyDefaultProfiles();

            ReapplySettings();
        }

        public static void ReapplySettings()
        {
            if (sApplySettings != null) return;

            sApplySettings = new Common.AlarmTask(1, Sims3.Gameplay.Utilities.TimeUnit.Minutes, OnApplySettings);
        }

        protected static void OnNewDay()
        {
            Settings.Apply(false, true);
        }

        protected static void OnApplySettings()
        {
            Settings.Apply(true, true);

            sApplySettings = null;
        }

        public static int GetCurrentSeasonDay()
        {
            if (!SeasonsManager.Enabled) return 0;

            DateAndTime currentTime = SimClock.Subtract(SimClock.CurrentTime(), TimeUnit.Hours, SimClock.CurrentTime().Hour);

            // Number of remaining days
            int num = ((int) SimClock.ElapsedTime(TimeUnit.Days, currentTime, SeasonsManager.ExpectedEndTime));

            return ((int)SeasonsManager.GetSeasonLength(SeasonsManager.CurrentSeason) - num);
        }

        public class SettingPersistence : Persistence
        {
            public override string PersistencePrefix
            {
                get { return "Settings"; }
            }

            public override void Import(Persistence.Lookup settings)
            {
                Settings.Import(settings);
            }

            public override void Export(Persistence.Lookup settings)
            {
                Settings.Export(settings);
            }
        }
    }
}
