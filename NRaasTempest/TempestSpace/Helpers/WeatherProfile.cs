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
    public class WeatherProfile : IPersistence
    {
        string mName;

        int mStart;
        int mEnd;

        public bool mEnabled;

        [Persistable(false)]
        uint mLength;

        public Pair<float, float> mMorningTemp;
        public Pair<float, float> mNoonTemp;
        public Pair<float, float> mEveningTemp;
        public Pair<float, float> mNightTemp;

        List<WeatherData> mData = new List<WeatherData>();

        public WeatherProfile()
        { }
        public WeatherProfile(string name)
        {
            mName = name;
            mStart = 1;
            mEnd = -1;
            mEnabled = true;

            mData.Add(new WeatherData(Weather.Sunny));
            mData.Add(new WeatherData(Weather.Fog));
            mData.Add(new PercipitationData(Weather.Rain));
            mData.Add(new PercipitationData(Weather.Snow));
            mData.Add(new WeatherData(Weather.Hail));
            //mData.Add(new WeatherData(Weather.BewitchingRain));
            //mData.Add(new WeatherData(Weather.RevivingSprinkle));
        }
        public WeatherProfile(WeatherProfile profile)
        {
            mName = profile.mName;
            mStart = profile.mStart;
            mEnd = profile.mEnd;
            mEnabled = profile.mEnabled;
            mMorningTemp = profile.mMorningTemp;
            mNoonTemp = profile.mNoonTemp;
            mEveningTemp = profile.mEveningTemp;
            mNightTemp = profile.mNightTemp;

            foreach (WeatherData data in profile.mData)
            {
                mData.Add(data.Clone());
            }
        }

        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        public string PersistencePrefix
        {
            get { return null; }
        }

        public IEnumerable<WeatherData> Data
        {
            get { return mData; }
        }

        public int RelativeStart
        {
            get
            {
                return mStart;
            }
            set
            {
                mStart = value;
            }
        }

        public uint GetActualStart(Season season)
        {
            return PersistedSettings.GetActualDay(mStart, season);
        }

        public int RelativeEnd
        {
            get
            {
                return mEnd;
            }
            set
            {
                mEnd = value;
            }
        }

        public uint GetActualEnd(Season season)
        {
            return PersistedSettings.GetActualDay(mEnd, season);
        }

        public void CalculateLength(Season season)
        {
            mLength = (GetActualEnd(season) - GetActualStart(season)) + 1;
        }

        public static int ReverseSortByLength(WeatherProfile l, WeatherProfile r)
        {
            // Intentionally reversed
            return r.mLength.CompareTo(l.mLength);
        }

        public void ParseWeather(Weather weather, XmlDbRow row)
        {
            foreach (WeatherData data in mData)
            {
                if (data.Type == weather)
                {
                    data.Parse(row);
                    return;
                }
            }
        }

        public void ReadDefaults(Season season)
        {
            ParseTemperatures(season);

            foreach (WeatherData data in mData)
            {
                data.ReadDefaults(season);
            }
        }

        public void Apply(Season season)
        {
            Common.DebugNotify("Profile Applied: " + mName);

            ApplyTemperature(season);

            foreach (WeatherData data in mData)
            {
                data.Apply(season);
            }
        }

        public void ApplyTemperature(Season season)
        {
            float[] min = new float[] { mMorningTemp.First, mNoonTemp.First, mEveningTemp.First, mNightTemp.First };
            float[] max = new float[] { mMorningTemp.Second, mNoonTemp.Second, mEveningTemp.Second, mNightTemp.Second };

            SeasonsManager.TemperatureManager.sMinCurve[season] = SeasonsManager.TemperatureManager.CreateTemperatureCurve(SeasonsManager.TemperatureManager.kTimes, min);
            SeasonsManager.TemperatureManager.sMaxCurve[season] = SeasonsManager.TemperatureManager.CreateTemperatureCurve(SeasonsManager.TemperatureManager.kTimes, max);
        }

        public void ParseTemperature(XmlDbRow row)
        {
            mStart = row.GetInt("StartDay", 1);
            mEnd = row.GetInt("EndDay", -1);

            if (row.Exists("Enabled"))
            {
                mEnabled = row.GetBool("Enabled");
            }

            for (int i = 0x0; i < SeasonsManager.TemperatureManager.kTimeNames.Length; i++)
            {
                string column = SeasonsManager.TemperatureManager.kTimeNames[i] + "Min";
                float min = row.GetFloat(column, -1f);

                column = SeasonsManager.TemperatureManager.kTimeNames[i] + "Max";
                float max = row.GetFloat(column, -1f);

                switch (i)
                {
                    case 0:
                        mMorningTemp = new Pair<float, float>(min, max);
                        break;
                    case 1:
                        mNoonTemp = new Pair<float, float>(min, max);
                        break;
                    case 2:
                        mEveningTemp = new Pair<float, float>(min, max);
                        break;
                    case 3:
                        mNightTemp = new Pair<float, float>(min, max);
                        break;
                }
            }
        }

        protected void ParseTemperatures(Season currentSeason)
        {
            XmlDbData xmlData = XmlDbData.ReadData("Seasons");
            if ((xmlData == null) || (xmlData.Tables == null)) return;

            XmlDbTable xmlDbTable = SeasonsManager.GetXmlDbTable(xmlData, "Temperature");
            if (xmlDbTable != null)
            {
                foreach (XmlDbRow row in xmlDbTable.Rows)
                {
                    Season season;
                    if (!row.TryGetEnum<Season>("Season", out season, Season.Summer)) continue;

                    if (season != currentSeason) continue;

                    ParseTemperature(row);
                }
            }
        }

        protected void Import(ref Pair<float, float> pair, Persistence.Lookup settings, string key)
        {
            pair.First = settings.GetFloat(key + "First", 0);
            pair.Second = settings.GetFloat(key + "Second", 0);
        }

        public void Import(Persistence.Lookup settings)
        {
            mName = settings.GetString("Name");

            mStart = settings.GetInt("Start", 1);
            mEnd = settings.GetInt("End", -1);

            mEnabled = settings.GetBool("Enabled", false);

            Import(ref mMorningTemp, settings, "MorningTemp");
            Import(ref mNoonTemp, settings, "NoonTemp");
            Import(ref mEveningTemp, settings, "EveningTemp");
            Import(ref mNightTemp, settings, "NightTemp");

            mData = settings.GetList<WeatherData>("Types");
        }

        protected void Export(ref Pair<float, float> pair, Persistence.Lookup settings, string key)
        {
            settings.Add(key + "First", pair.First);
            settings.Add(key + "Second", pair.Second);
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("Name", mName);

            settings.Add("Start", mStart);
            settings.Add("End", mEnd);

            settings.Add("Enabled", mEnabled);

            Export(ref mMorningTemp, settings, "MorningTemp");
            Export(ref mNoonTemp, settings, "NoonTemp");
            Export(ref mEveningTemp, settings, "EveningTemp");
            Export(ref mNightTemp, settings, "NightTemp");

            settings.Add("Types", mData);
        }

        public string ToXMLTemperatureString(Season season)
        {
            Common.StringBuilder result = new Common.StringBuilder();

            result += Common.NewLine + "  <Temperature>";
            result += Common.NewLine + "    <Season>" + season + "</Season>";
            result += Common.NewLine + "    <Name>" + mName + "</Name>";
            result += Common.NewLine + "    <StartDay>" + mStart + "</StartDay>";
            result += Common.NewLine + "    <EndDay>" + mEnd + "</EndDay>";
            result += Common.NewLine + "    <Enabled>" + mEnabled + "</Enabled>";
            result += Common.NewLine + "    <MorningMin>" + mMorningTemp.First + "</MorningMin>";
            result += Common.NewLine + "    <MorningMax>" + mMorningTemp.Second + "</MorningMax>";
            result += Common.NewLine + "    <NoonMin>" + mNoonTemp.First + "</NoonMin>";
            result += Common.NewLine + "    <NoonMax>" + mNoonTemp.Second + "</NoonMax>";
            result += Common.NewLine + "    <EveningMin>" + mEveningTemp.First + "</EveningMin>";
            result += Common.NewLine + "    <EveningMax>" + mEveningTemp.Second + "</EveningMax>";
            result += Common.NewLine + "    <NightMin>" + mNightTemp.First + "</NightMin>";
            result += Common.NewLine + "    <NightMax>" + mNightTemp.Second + "</NightMax>";
            result += Common.NewLine + "  </Temperature>";

            return result.ToString();
        }

        public string ToXMLWeatherString(Season season, Weather weather)
        {
            foreach (WeatherData data in mData)
            {
                if (data.Type == weather)
                {
                    return data.ToXMLString(season, weather, mName);
                }
            }
            return null;
        }
    }
}
