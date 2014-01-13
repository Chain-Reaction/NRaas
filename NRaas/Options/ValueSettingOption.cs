using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Options
{
    public abstract class ValueSettingOption<TValue> : InteractionOptionItem<IMiniSimDescription, IMiniSimDescription, MiniSimDescriptionParameters>
    {
        protected TValue mValue;

        public ValueSettingOption()
        { }
        public ValueSettingOption(TValue value, string name, int count)
            : base(name, count)
        {
            mValue = value;
        }
        public ValueSettingOption(TValue value, string name, int count, string icon, ProductVersion version)
            : base(name, count, icon, version)
        {
            mValue = value;
        }
        public ValueSettingOption(TValue value, string name, int count, ResourceKey icon)
            : base(name, count, icon)
        {
            mValue = value;
        }
        public ValueSettingOption(TValue value, string name, int count, ThumbnailKey thumbnail)
            : base(name, count, thumbnail)
        {
            mValue = value;
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public virtual TValue Value
        {
            get
            {
                return mValue;
            }
        }

        public override string DisplayValue
        {
            get 
            {
                string key = DisplayKey;
                if (!string.IsNullOrEmpty(key))
                {
                    return Common.Localize(key + ":" + Value.ToString());
                }
                else
                {
                    return null;
                }
            }
        }

        protected override OptionResult Run(MiniSimDescriptionParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
