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
    public class TunableStringOption : TunableGenericOption<string>
    {
        public TunableStringOption()
        { }
        public TunableStringOption(TunableFieldInfo field)
            : base(field)
        { }

        public override void SetField(FieldInfo field, XmlDbRow row)
        {
            field.SetValue(null, row.GetString("Value"));
        }

        public override string GetTitlePrefix()
        {
            return "TunableString";
        }

        public override string DisplayValue
        {
            get
            {
                return Common.Localize("Type:Text");
            }
        }

        protected override OptionResult Convert(string value, out string result)
        {
            result = value;
            return OptionResult.SuccessRetain;
        }

        public override ITunableConvertOption Clone(TunableFieldInfo field)
        {
            return new TunableStringOption(field);
        }
    }
}
