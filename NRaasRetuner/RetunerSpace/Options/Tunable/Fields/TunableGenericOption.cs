using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.RetunerSpace.Helpers.FieldInfos;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RetunerSpace.Options.Tunable.Fields
{
    public abstract class TunableGenericOption<T> : GenericSettingOption<T, GameObject>, ITunableFieldOption, ITunableConvertOption
    {
        protected TunableFieldInfo mField;

        protected TunableGenericOption()
        { }
        public TunableGenericOption(TunableFieldInfo field)
        {
            mField = field;
        }

        public override string Name
        {
            get { return mField.Name; }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public virtual Type FieldType
        {
            get { return typeof(T); }
        }

        public void Export(Common.StringBuilder result)
        {
            result += Common.NewLine + mField.ToXMLString();
        }

        protected abstract OptionResult Convert(string value, out T result);

        public object Convert(string value, bool fireError)
        {
            T result;
            if (Convert(value, out result) == OptionResult.Failure)
            {
                if (fireError)
                {
                    BooterLogger.AddError("Value Conversion failed: " + value);
                }
                return null;
            }

            return result;
        }

        public abstract void SetField(FieldInfo field, XmlDbRow row);

        protected override T Value
        {
            get
            {
                if (mField == null) return default(T);

                return (T)mField.GetValue(false);
            }
            set
            {
                mField.SetValue(Retuner.SeasonSettings.Key, value);
            }
        }

        public override void SetImportValue(string value)
        { }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            string text = StringInputDialog.Show(Name, GetPrompt(), Value.ToString());
            if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

            T value;
            OptionResult result = Convert(text, out value);
            if (result == OptionResult.Failure) return OptionResult.Failure;

            Value = value;

            Common.Notify(ToString());
            return OptionResult.SuccessRetain;
        }

        public abstract ITunableConvertOption Clone(TunableFieldInfo field);
    }
}
