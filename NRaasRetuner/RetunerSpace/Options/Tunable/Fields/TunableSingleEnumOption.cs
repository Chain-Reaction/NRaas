using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
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
    public class TunableSingleEnumOption : TunableGenericOption<object>
    {
        public TunableSingleEnumOption()
        { }
        public TunableSingleEnumOption(TunableFieldInfo field)
            : base(field)
        { }

        public override string GetTitlePrefix()
        {
            return "TunableEnum";
        }

        public override Type FieldType
        {
            get
            {
                if (mField == null) return null;

                return mField.ElementalType;
            }
        }

        public override string DisplayValue
        {
            get { return Value.ToString(); }
        }

        public override void SetField(FieldInfo field, XmlDbRow row)
        {
            object result;
            if (Convert(row.GetString("Value"), out result) == OptionResult.Failure)
            {
                result = null;
            }

            field.SetValue(null, result);
        }

        protected override OptionResult Convert(string value, out object result)
        {
            if (ParserFunctions.TryParseEnumType(FieldType, value, out result, true))
            {
                return OptionResult.SuccessClose;
            }
            else
            {
                result = null;
                return OptionResult.Failure;
            }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            try
            {
                List<Item> choices = new List<Item>();
                foreach (object value in Enum.GetValues(FieldType))
                {
                    choices.Add(new Item(value));
                }

                Item selection = new CommonSelection<Item>(Name, choices).SelectSingle();
                if (selection == null) return OptionResult.Failure;

                Value = selection.Value;
                return OptionResult.SuccessRetain;
            }
            catch (Exception e)
            {
                Common.Exception(FieldType.ToString(), e);
                return OptionResult.Failure;
            }
        }

        public override ITunableConvertOption Clone(TunableFieldInfo field)
        {
            return new TunableSingleEnumOption(field);
        }

        public class Item : ValueSettingOption<object>
        {
            public Item(object value)
                : base(value, value.ToString(), 0)
            { }
        }
    }
}
