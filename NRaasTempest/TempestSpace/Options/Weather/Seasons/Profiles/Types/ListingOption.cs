using NRaas.CommonSpace.Options;
using NRaas.TempestSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Options.Weather.Seasons.Profiles.Types
{
    public class ListingOption : InteractionOptionList<ITypeOption,GameObject>, IProfileOption
    {
        WeatherData mData;

        public ListingOption(WeatherData data)
            : base(data.Name)
        {
            mData = data;
        }

        public override string DisplayValue
        {
            get
            {
                return EAText.GetNumberString(mData.mWeight);
            }
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!SeasonsManager.IsWeatherEnabled(mData.Type)) return false;

            return base.Allow(parameters);
        }

        public override List<ITypeOption> GetOptions()
        {
            List<ITypeOption> results = new List<ITypeOption>();

            results.Add(new WeightOption(mData));
            results.Add(new TemperatureOption(mData));
            results.Add(new LengthOption(mData));

            PercipitationData data = mData as PercipitationData;
            if (data != null)
            {
                results.Add(new TransitionTime(data));
                results.Add(new MinIntensityDuration(data));
                results.Add(new IntensityWeightLight(data));
                results.Add(new IntensityWeightMedium(data));
                results.Add(new IntensityWeightHeavy(data));
                results.Add(new IntensityChangeWeight(data));
            }

            return results;
        }
    }
}
