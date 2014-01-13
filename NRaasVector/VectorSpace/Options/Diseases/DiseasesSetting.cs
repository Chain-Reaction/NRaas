using NRaas.CommonSpace.Options;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.VectorSpace.Options.Diseases
{
    public class DiseasesSetting : InteractionOptionList<ISettingOption, GameObject>, IDiseasesOption
    {
        string mGuid;

        public DiseasesSetting(string guid)
        {
            mGuid = guid;
        }

        public override string GetTitlePrefix()
        {
            return "Diseases";
        }

        public override string Name
        {
            get
            {
                return VectorBooter.GetVector(mGuid).GetLocalizedName(false);
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<ISettingOption> GetOptions()
        {
            List<ISettingOption> results = new List<ISettingOption>();

            results.Add(new EnabledSetting(mGuid));
            results.Add(new WatchesSetting(mGuid));
            results.Add(new MotiveAdjustmentRatioSetting(mGuid));

            VectorBooter.Data vector = VectorBooter.GetVector(mGuid);
            if (vector != null)
            {
                foreach (string setting in vector.CustomSettings)
                {
                    results.Add(new CustomSetting(setting));
                }
            }

            return results;
        }
    }
}
