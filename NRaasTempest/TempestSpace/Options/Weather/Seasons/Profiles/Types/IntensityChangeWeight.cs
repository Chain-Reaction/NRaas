using NRaas.CommonSpace.Options;
using NRaas.TempestSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Options.Weather.Seasons.Profiles.Types
{
    public class IntensityChangeWeight : StringSettingOption<GameObject>, ITypeOption
    {
        PercipitationData mData;

        public IntensityChangeWeight(PercipitationData data)
        {
            mData = data;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override string GetTitlePrefix()
        {
            return "IntensityChangeWeight";
        }

        public override string DisplayValue
        {
            get
            {
                if (mData.mIntensityChangeWeights.Count > 3)
                {
                    return "...";
                }
                else
                {
                    Common.StringBuilder result = new Common.StringBuilder(); ;

                    for (int i = 0; i < mData.mIntensityChangeWeights.Count; i++)
                    {
                        if (i != 0)
                        {
                            result += ",";
                        }

                        result += EAText.GetNumberString(mData.mIntensityChangeWeights[i]);
                    }

                    return result.ToString();
                }
            }
        }

        protected override string Value
        {
            get
            {
                Common.StringBuilder result = new Common.StringBuilder();;

                for (int i=0; i<mData.mIntensityChangeWeights.Count; i++)
                {
                    if (i!= 0)
                    {
                        result += ",";
                    }

                    result += mData.mIntensityChangeWeights[i];
                }

                return result.ToString();
            }
            set
            {
                string[] values = value.Split(',');

                mData.mIntensityChangeWeights = new List<int>(values.Length);

                foreach (string val in values)
                {
                    int result;
                    if (!int.TryParse(val, out result)) continue;

                    if (result <= 0) continue;

                    mData.mIntensityChangeWeights.Add(result);
                }

                Tempest.ReapplySettings();
            }
        }
    }
}
