using NRaas.CommonSpace.Helpers;
using NRaas.TempestSpace.Booters;
using NRaas.TempestSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TempestSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("Percent chance hail kills harvestables on outdoor plants")]
        protected static int kHailKillsHarvestables = 0;

        [Tunable, TunableComment("Percent chance hail kills plants outright")]
        protected static int kHailKillsPlants = 0;

        [Tunable, TunableComment("Whether to suppress insect spawners during winter")]
        protected static bool kSuppressInsectInWinter = true;

        [Tunable, TunableComment("Whether to suppress inactive holiday parties")]
        protected static bool kAllowHolidayParties = true;

		[Tunable, TunableComment("Whether to remove fallen leaves on lots in Winter")]
		protected static bool kRemoveLeavesInWinter = false;

        [Tunable, TunableComment("Percent chance occults will give occult items during trick or treating")]
        protected static int kChanceOccultItemTrickOrTreat = 50;

        [Tunable, TunableComment("Whether to enable auto lighting of fireplaces when the temp teaches kAutoLightFireplacesTemperature")]
        protected static bool kAutoLightFireplaces = true;
        [Tunable, TunableComment("At which temperature to auto light said fireplaces")]
        protected static float kAutoLightFireplacesTemperature = 40.0f;

        public int mHailKillsHarvestables = kHailKillsHarvestables;

        public int mHailKillsPlants = kHailKillsPlants;

        public bool mSuppressInsectInWinter = kSuppressInsectInWinter;

        public bool mAllowHolidayParties = kAllowHolidayParties;

		public bool mRemoveLeavesInWinter = kRemoveLeavesInWinter;

        public int mChanceOccultItemTrickOrTreat = kChanceOccultItemTrickOrTreat;

        public bool mAutoLightFireplaces = kAutoLightFireplaces;
        public float mAutoLightFireplacesTemperature = kAutoLightFireplacesTemperature;


        Dictionary<Season, HolidaySettings> mHolidaySettings = new Dictionary<Season, HolidaySettings>();

        Dictionary<Season, WeatherSettings> mWeatherSettings = new Dictionary<Season, WeatherSettings>();

        public bool mDebugging = false;

        public PersistedSettings()
        {
            foreach (Season season in Enum.GetValues(typeof(Season)))
            {
                GetHolidays(season).Add(season);
            }
        }

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

        public IEnumerable<WeatherSettings> Weather
        {
            get { return mWeatherSettings.Values; }
        }

        public HolidaySettings GetHolidays(Season season)
        {
            HolidaySettings settings;
            if (!mHolidaySettings.TryGetValue(season, out settings))
            {
                settings = new HolidaySettings(season);
                mHolidaySettings.Add(season, settings);
            }

            return settings;
        }

        public WeatherSettings GetWeather(Season season)
        {
            return GetWeather(season, true);
        }
        public WeatherSettings GetWeather(Season season, bool createDefault)
        {
            WeatherSettings settings;
            if (!mWeatherSettings.TryGetValue(season, out settings))
            {
                settings = new WeatherSettings(season, createDefault);
                mWeatherSettings.Add(season, settings);
            }

            return settings;
        }

        public static uint GetActualDay(int day, Season season)
        {
            if (day == 0) return 0;

            uint length = SeasonsManager.GetSeasonLength(season);

            if (day > 0)
            {
                if (day > length) return 0;
            }
            else
            {
                day = ((int)length + day);

                if (day < 0) return 0;
            }

            return (uint)day;
        }

        public void Apply(bool force, bool updateQueue)
        {
            if (SeasonsManager.Enabled)
            {
                if (!GetWeather(SeasonsManager.CurrentSeason).Apply(force)) return;

                if ((force) || (updateQueue))
                {
                    SeasonsManager.sInstance.mWeatherManager.ResetWeatherQueue();
                }
            }
        }

        public void Import(Persistence.Lookup settings)
        {
            mHolidaySettings.Clear();
            foreach (HolidaySettings setting in settings.GetList<HolidaySettings>("Holidays"))
            {
                mHolidaySettings[setting.Season] = setting;
            }

            mWeatherSettings.Clear();
            foreach (WeatherSettings setting in settings.GetList<WeatherSettings>("Weather"))
            {
                mWeatherSettings[setting.Season] = setting;
            }

            Tempest.ReapplySettings();
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("Holidays", mHolidaySettings.Values);
            settings.Add("Weather", mWeatherSettings.Values);
        }

        public string ToXMLString()
        {
            Common.StringBuilder temperature = new Common.StringBuilder();

            Dictionary<Weather, Common.StringBuilder> weather = new Dictionary<Sims3.SimIFace.Enums.Weather, Common.StringBuilder>();

            foreach(Weather w in Enum.GetValues(typeof(Weather)))
            {
                switch (w)
                {
                    case Sims3.SimIFace.Enums.Weather.BewitchingRain:
                    case Sims3.SimIFace.Enums.Weather.None:
                    case Sims3.SimIFace.Enums.Weather.RevivingSprinkle:
                        continue;
                }

                Common.StringBuilder builder = new Common.StringBuilder();

                builder += Common.NewLine + "  <" + w + "Tuning>";
                builder += Common.NewLine + "    <!-- This is the default set, do not alter or remove -->";
                builder += Common.NewLine + "    <Season></Season>";
                builder += Common.NewLine + "    <Name></Name>";
                builder += Common.NewLine + "    <Weight>0</Weight>";
                builder += Common.NewLine + "    <MinLength>0</MinLength>";
                builder += Common.NewLine + "    <MaxLength>24</MaxLength>";
                builder += Common.NewLine + "    <MinTemp>-1000</MinTemp>";
                builder += Common.NewLine + "    <MaxTemp>1000</MaxTemp>";

                switch (w)
                {
                    case Sims3.SimIFace.Enums.Weather.Rain:
                    case Sims3.SimIFace.Enums.Weather.Snow:
                        builder += Common.NewLine + "    <LightWeight>1</LightWeight>";
                        builder += Common.NewLine + "    <ModerateWeight>1</ModerateWeight>";
                        builder += Common.NewLine + "    <HeavyWeight>1</HeavyWeight>";
                        builder += Common.NewLine + "    <MinTransitionTime>0.25</MinTransitionTime>";
                        builder += Common.NewLine + "    <MaxTransitionTime>1</MaxTransitionTime>";
                        builder += Common.NewLine + "    <NumIntensityChangeWeights>1,1,1,1,1,1</NumIntensityChangeWeights>";
                        builder += Common.NewLine + "    <MinIntensityDuration>1</MinIntensityDuration>";
                        break;
                }

                builder += Common.NewLine + "  </" + w + "Tuning>";

                weather.Add(w, builder);
            }

            temperature += Common.NewLine + "  <Temperature>";
            temperature += Common.NewLine + "    <!-- This is the default set, do not alter or remove -->";
            temperature += Common.NewLine + "    <Season></Season>";
            temperature += Common.NewLine + "    <Name></Name>";
            temperature += Common.NewLine + "    <StartDay>1</StartDay>";
            temperature += Common.NewLine + "    <EndDay>-1</EndDay>";
            temperature += Common.NewLine + "    <Enabled>True</Enabled>";
            temperature += Common.NewLine + "    <MorningMin>0</MorningMin>";
            temperature += Common.NewLine + "    <MorningMax>0</MorningMax>";
            temperature += Common.NewLine + "    <NoonMin>0</NoonMin>";
            temperature += Common.NewLine + "    <NoonMax>0</NoonMax>";
            temperature += Common.NewLine + "    <EveningMin>0</EveningMin>";
            temperature += Common.NewLine + "    <EveningMax>0</EveningMax>";
            temperature += Common.NewLine + "    <NightMin>0</NightMin>";
            temperature += Common.NewLine + "    <NightMax>0</NightMax>";
            temperature += Common.NewLine + "  </Temperature>";

            foreach (KeyValuePair<Season, WeatherSettings> setting in mWeatherSettings)
            {
                foreach (WeatherProfile profile in setting.Value.Profiles)
                {
                    temperature += profile.ToXMLTemperatureString(setting.Key);

                    foreach (KeyValuePair<Weather, Common.StringBuilder> w in weather)
                    {
                        w.Value.Append(profile.ToXMLWeatherString(setting.Key, w.Key));
                    }
                }
            }

            Common.StringBuilder result = new Common.StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            result += Common.NewLine + "<Seasons>";

            result += temperature;

            foreach (Common.StringBuilder w in weather.Values)
            {
                result += w;
            }

            result += Common.NewLine + "</Seasons>";

            return result.ToString();
        }
    }
}
