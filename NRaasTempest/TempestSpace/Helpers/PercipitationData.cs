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
    public class PercipitationData : WeatherData
    {
        public Pair<float, float> mTransitionTime;

        public float mMinIntensityDuration;

        public List<int> mIntensityWeights = new List<int>();

        public List<int> mIntensityChangeWeights = new List<int>();

        public PercipitationData()
        { }
        public PercipitationData(Weather type)
            : base(type)
        { }
        public PercipitationData(PercipitationData data)
            : base(data)
        {
            mTransitionTime = data.mTransitionTime;
            mMinIntensityDuration = data.mMinIntensityDuration;
            mIntensityWeights.AddRange(data.mIntensityWeights);
            mIntensityChangeWeights.AddRange(data.mIntensityChangeWeights);
        }

        public override void Parse(XmlDbRow row)
        {
            base.Parse(row);

            mMinIntensityDuration = row.GetFloat("MinIntensityDuration", 0);
            mTransitionTime.First = row.GetFloat("MinTransitionTime", 0);
            mTransitionTime.Second = row.GetFloat("MaxTransitionTime", 0);
            mIntensityWeights = new List<int>();
            mIntensityWeights.Add(row.GetInt("LightWeight", 0));
            mIntensityWeights.Add(row.GetInt("ModerateWeight", 0));
            mIntensityWeights.Add(row.GetInt("HeavyWeight", 0));
            mIntensityChangeWeights = new List<int>();

            foreach (string strValue in row.GetStringList("NumIntensityChangeWeights", ','))
            {
                float value;
                if (float.TryParse(strValue, out value))
                {
                    mIntensityChangeWeights.Add((int)value);
                }
                else
                {
                    mIntensityChangeWeights.Add(0);
                }
            }
        }

        public override void ReadDefaults(Season season)
        {
            base.ReadDefaults(season);

            SeasonsManager.WeatherManager.PrecipitationEvent.PrecipitationProfile profile = SeasonsManager.WeatherManager.sProfiles[Type] as SeasonsManager.WeatherManager.PrecipitationEvent.PrecipitationProfile;

            mTransitionTime.First = profile.mMinTransitionTime[season];
            mTransitionTime.Second = profile.mMaxTransitionTime[season];

            mMinIntensityDuration = profile.mMinIntensityDuration[season];

            mIntensityWeights = new List<int>(3);

            for (int i = 0; i < 3; i++)
            {
                mIntensityWeights.Add((int)profile.mIntensityWeights[i][season]);
            }

            mIntensityChangeWeights = new List<int>(profile.mIntensityChangeWeights.Length);

            for (int i = 0; i < profile.mIntensityChangeWeights.Length; i++)
            {
                mIntensityChangeWeights.Add((int)profile.mIntensityChangeWeights[i][season]);
            }
        }

        public override void Apply(Season season)
        {
            base.Apply(season);

            SeasonsManager.WeatherManager.PrecipitationEvent.PrecipitationProfile profile = SeasonsManager.WeatherManager.sProfiles[Type] as SeasonsManager.WeatherManager.PrecipitationEvent.PrecipitationProfile;

            profile.mMinTransitionTime[season] = mTransitionTime.First;
            profile.mMaxTransitionTime[season] = mTransitionTime.Second;

            profile.mMinIntensityDuration[season] = mMinIntensityDuration;

            for (int i = 0; i < 3; i++)
            {
                profile.mIntensityWeights[i][season] = mIntensityWeights[i];
            }

            if (profile.mIntensityChangeWeights.Length < mIntensityChangeWeights.Count)
            {
                List<SeasonsManager.SeasonFloat> weights = new List<SeasonsManager.SeasonFloat>(profile.mIntensityChangeWeights);

                for (int i = weights.Count; i < mIntensityChangeWeights.Count; i++)
                {
                    weights.Add(new SeasonsManager.SeasonFloat());
                }

                profile.mIntensityChangeWeights = weights.ToArray();
            }

            for (int i = 0; i < mIntensityChangeWeights.Count; i++)
            {
                profile.mIntensityChangeWeights[i][season] = mIntensityChangeWeights[i];
            }

            // Zero out any remaining weights
            for (int i = mIntensityChangeWeights.Count; i < profile.mIntensityChangeWeights.Length; i++)
            {
                profile.mIntensityChangeWeights[i][season] = 0;
            }
        }

        public override void Import(Persistence.Lookup settings)
        {
            base.Import(settings);

            Import(ref mTransitionTime, settings, "TransitionTime");

            mMinIntensityDuration = settings.GetFloat("MinIntensityDuration", 0);

            mIntensityWeights = new List<int>();

            mIntensityWeights.Add(settings.GetInt("LightWeight", 0));
            mIntensityWeights.Add(settings.GetInt("MediumWeight", 0));
            mIntensityWeights.Add(settings.GetInt("HeavyWeight", 0));

            mIntensityChangeWeights = settings.GetList<int>("ChangeWeights", Convert);
        }

        protected static int Convert(string value)
        {
            int result;
            if (!int.TryParse(value, out result)) return 0;
            return result;
        }

        public override void Export(Persistence.Lookup settings)
        {
            base.Export(settings);

            Export(ref mTransitionTime, settings, "TransitionTime");

            settings.Add("MinIntensityDuration", mMinIntensityDuration);

            settings.Add("LightWeight", mIntensityWeights[0]);
            settings.Add("MediumWeight", mIntensityWeights[1]);
            settings.Add("HeavyWeight", mIntensityWeights[2]);

            settings.Add("ChangeWeights", mIntensityChangeWeights, mIntensityChangeWeights.Count);
        }

        protected override string ToXMLString()
        {
            Common.StringBuilder result = new Common.StringBuilder(base.ToXMLString());

            result += Common.NewLine + "    <LightWeight>" + mIntensityWeights[0] + "</LightWeight>";
            result += Common.NewLine + "    <ModerateWeight>" + mIntensityWeights[1] + "</ModerateWeight>";
            result += Common.NewLine + "    <HeavyWeight>" + mIntensityWeights[2] + "</HeavyWeight>";
            result += Common.NewLine + "    <MinTransitionTime>" + mTransitionTime.First + "</MinTransitionTime>";
            result += Common.NewLine + "    <MaxTransitionTime>" + mTransitionTime.Second + "</MaxTransitionTime>";
            result += Common.NewLine + "    <NumIntensityChangeWeights>";

            for (int i = 0; i < mIntensityChangeWeights.Count; i++)
            {
                if (i != 0)
                {
                    result += ",";
                }

                result += mIntensityChangeWeights[i].ToString();
            }

            result += "</NumIntensityChangeWeights>";
            result += Common.NewLine + "    <MinIntensityDuration>" + mMinIntensityDuration + "</MinIntensityDuration>";

            return result.ToString();
        }

        public override WeatherData Clone()
        {
            return new PercipitationData(this);
        }
    }
}
