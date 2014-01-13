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
    public class TunableBooleanOption : TunableGenericOption<bool>
    {
        public TunableBooleanOption()
        { }
        public TunableBooleanOption(TunableFieldInfo field)
            : base(field)
        { }

        public override string GetTitlePrefix()
        {
            return "TunableBoolean";
        }

        public override string DisplayValue
        {
            get
            {
                return Common.Localize("Boolean:" + Value);
            }
        }

        public override void SetField(FieldInfo field, XmlDbRow row)
        {
            field.SetValue(null, row.GetBool("Value"));
        }

        protected override OptionResult Convert(string value, out bool result)
        {
            try
            {
                result = ParserFunctions.ParseBool(value);
                return OptionResult.SuccessClose;
            }
            catch
            {
                result = false;
                return OptionResult.Failure;
            }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Value = !Value;

            Common.Notify(ToString());
            return OptionResult.SuccessRetain;
        }

        public override ITunableConvertOption Clone(TunableFieldInfo field)
        {
            return new TunableBooleanOption(field);
        }
    }
}
