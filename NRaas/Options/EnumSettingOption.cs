using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Options
{
    public abstract class EnumSettingOption<TType, TTarget> : GenericSettingOption<TType, TTarget>
        where TType : struct
        where TTarget : class, IGameObject
    {
        protected virtual string GetValuePrefix()
        {
            return GetTitlePrefix();
        }

        public virtual string GetLocalizedValue(TType value)
        {
            return Common.Localize(GetValuePrefix() + ":" + value);
        }

        public override string DisplayValue
        {
            get { return GetLocalizedValue(Value); }
        }

        protected virtual bool Allow(TType value)
        {
            return true;
        }

        protected virtual List<EnumOption> GetOptions()
        {
            List<EnumOption> results = new List<EnumOption>();

            foreach (TType value in Enum.GetValues(typeof(TType)))
            {
                if (!Allow(value)) continue;

                results.Add(new EnumOption(this, value));
            }

            return results;
        }

        public abstract TType Default
        {
            get;
        }

        public override void SetImportValue(string value)
        {
            TType newValue;
            ParserFunctions.TryParseEnum<TType>(value, out newValue, Default);
            Value = newValue;
        }

        protected override OptionResult Run(GameHitParameters<TTarget> parameters)
        {
            EnumOption selection = new CommonSelection<EnumOption>(Name, GetOptions()).SelectSingle();
            if (selection == null) return OptionResult.Failure;

            Value = selection.Value;

            Common.Notify(ToString());
            return OptionResult.SuccessRetain;
        }

        public class EnumOption : ValueSettingOption<TType>
        {
            protected EnumOption()
            {}
            public EnumOption(EnumSettingOption<TType, TTarget> parent, TType value)
                : base(value, parent.GetLocalizedValue(value), -1)
            { }

            public override string DisplayValue
            {
                get { return null; }
            }
        }
    }
}
