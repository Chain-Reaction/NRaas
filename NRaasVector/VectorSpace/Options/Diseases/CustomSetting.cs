using NRaas.CommonSpace.Helpers;
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
    public class CustomSetting : BooleanSettingOption<GameObject>, ISettingOption
    {
        string mSetting;

        public CustomSetting()
        { }
        public CustomSetting(string setting)
        {
            mSetting = setting;
        }

        protected override bool Value
        {
            get
            {
                return Vector.Settings.IsSet(mSetting);
            }
            set
            {
                Vector.Settings.SetCustom(mSetting, value);
            }
        }

        public override string GetTitlePrefix()
        {
            return "Setting";
        }

        public override string Name
        {
            get
            {
                return Common.Localize("Setting:" + mSetting);
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override void Import(Persistence.Lookup settings)
        {
            Vector.Settings.ClearSettings();

            foreach (string setting in settings.GetStringList("Settings"))
            {
                Vector.Settings.SetCustom(setting, true);
            }
        }

        public override void Export(Persistence.Lookup settings)
        {
            List<string> value = new List<string>();
            foreach (VectorBooter.Data vector in VectorBooter.Vectors)
            {
                foreach (string setting in vector.CustomSettings)
                {
                    if (Vector.Settings.IsSet(setting))
                    {
                        value.Add(setting);
                    }
                }
            }

            settings.Add("Settings", value);
        }

        public override string PersistencePrefix
        {
            get { return "Settings"; }
        }
    }
}
