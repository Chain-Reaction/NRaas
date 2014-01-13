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
    public class WeatherData : IPersistence
    {
        Weather mType;

        public Pair<float,float> mTemp;

        public Pair<float, float> mLength;

        public int mWeight;

        public WeatherData()
        { }
        public WeatherData(Weather type)
        {
            mType = type;
        }
        public WeatherData(WeatherData data)
        {
            mType = data.mType;
            mTemp = data.mTemp;
            mLength = data.mLength;
            mWeight = data.mWeight;
        }

        public Weather Type
        {
            get { return mType; }
        }

        public string Name
        {
            get { return Common.Localize("Weather:MenuName", false, new object[] { Common.Localize("Weather:" + mType) }); }
        }

        public string PersistencePrefix
        {
            get { return null; }
        }

        public virtual void Parse(XmlDbRow row)
        {
            mWeight = row.GetInt("Weight", 0);
            mLength.First = row.GetFloat("MinLength", 0);
            mLength.Second = row.GetFloat("MaxLength", 0);
            mTemp.First = row.GetFloat("MinTemp", 0);
            mTemp.Second = row.GetFloat("MaxTemp", 0);
        }

        public virtual void ReadDefaults(Season season)
        {
            SeasonsManager.WeatherManager.WeatherProfile profile = SeasonsManager.WeatherManager.sProfiles[mType];

            mLength.First = profile.mMinLength[season];
            mLength.Second = profile.mMaxLength[season];

            mTemp.First = profile.mMinTemp[season];
            mTemp.Second = profile.mMaxTemp[season];

            mWeight = (int)profile.mWeight[season];
        }

        public virtual void Apply(Season season)
        {
            SeasonsManager.WeatherManager.WeatherProfile profile = SeasonsManager.WeatherManager.sProfiles[mType];

            profile.mMinLength[season] = mLength.First;
            profile.mMaxLength[season] = mLength.Second;

            profile.mMinTemp[season] = mTemp.First;
            profile.mMaxTemp[season] = mTemp.Second;

            profile.mWeight[season] = mWeight;
        }

        protected void Import(ref Pair<float, float> pair, Persistence.Lookup settings, string key)
        {
            pair.First = settings.GetFloat(key + "First", 0);
            pair.Second = settings.GetFloat(key + "Second", 0);
        }

        public virtual void Import(Persistence.Lookup settings)
        {
            mType = settings.GetEnum<Weather>("Type", Weather.None);

            Import(ref mTemp, settings, "Temp");
            Import(ref mLength, settings, "Length");

            mWeight = settings.GetInt("Weight", 0);
        }

        protected void Export(ref Pair<float, float> pair, Persistence.Lookup settings, string key)
        {
            settings.Add(key + "First", pair.First);
            settings.Add(key + "Second", pair.Second);
        }

        public virtual void Export(Persistence.Lookup settings)
        {
            settings.Add("Type", mType.ToString());

            Export(ref mTemp, settings, "Temp");
            Export(ref mLength, settings, "Length");

            settings.Add("Weight", mWeight);
        }

        protected virtual string ToXMLString()
        {
            Common.StringBuilder result = new Common.StringBuilder();

            result += Common.NewLine + "    <Weight>" + mWeight + "</Weight>";
            result += Common.NewLine + "    <MinLength>" + mLength.First + "</MinLength>";
            result += Common.NewLine + "    <MaxLength>" + mLength.Second + "</MaxLength>";
            result += Common.NewLine + "    <MinTemp>" + mTemp.First + "</MinTemp>";
            result += Common.NewLine + "    <MaxTemp>" + mTemp.Second + "</MaxTemp>";

            return result.ToString();
        }

        public string ToXMLString(Season season, Weather weather, string name)
        {
            Common.StringBuilder result = new Common.StringBuilder();

            result += Common.NewLine + "  <" + weather + "Tuning>";
            result += Common.NewLine + "    <Season>" + season + "</Season>";
            result += Common.NewLine + "    <Name>" + name + "</Name>";
            result += ToXMLString();
            result += Common.NewLine + "  </" + weather + "Tuning>";

            return result.ToString();
        }

        public virtual WeatherData Clone()
        {
            return new WeatherData(this);
        }
    }
}
