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
    public abstract class TunableMultiEnumOption<T> : TunableGenericOption<T>
        where T : struct
    {
        public TunableMultiEnumOption()
        { }
        public TunableMultiEnumOption(TunableFieldInfo field)
            : base(field)
        { }

        public override string DisplayValue
        {
            get { return Value.ToString(); }
        }

        public override void SetField(FieldInfo field, XmlDbRow row)
        {
            field.SetValue(null, row.GetEnum<T>("Value", default(T)));
        }

        protected override OptionResult Convert(string value, out T result)
        {
            try
            {
                if (ParserFunctions.TryParseEnum<T>(value, out result, default(T)))
                {
                    return OptionResult.SuccessClose;
                }
            }
            catch
            { }

            result = default(T);
            return OptionResult.Failure;
        }
        protected abstract void PrivatePerform(List<T> values);

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<Item> choices = new List<Item>();
            foreach(T value in Enum.GetValues(FieldType))
            {
                choices.Add(new Item (value));
            }

            CommonSelection<Item>.Results selection = new CommonSelection<Item>(Name, choices).SelectMultiple();
            if (selection == null) return OptionResult.Failure;

            List<T> results = new List<T>();
            foreach(Item item in selection)
            {
                results.Add(item.Value);
            }

            PrivatePerform(results);
            return OptionResult.SuccessRetain;
        }

        public class Item : ValueSettingOption<T>
        {
            public Item(T value)
                : base(value, value.ToString(), 0)
            { }
        }
    }
}
