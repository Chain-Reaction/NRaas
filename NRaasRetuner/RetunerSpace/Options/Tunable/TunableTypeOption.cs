using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.RetunerSpace.Helpers.FieldInfos;
using NRaas.RetunerSpace.Options.Tunable.Fields;
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

namespace NRaas.RetunerSpace.Options.Tunable
{
    public class TunableTypeOption : InteractionOptionList<ITunableFieldOption, GameObject>
    {
        static Dictionary<Type, ITunableConvertOption> sLookup = new Dictionary<Type, ITunableConvertOption>();

        static TunableNestedOption sNestedOption = new TunableNestedOption();
        static TunableArrayOption sArrayOption = new TunableArrayOption();
        static TunableSingleEnumOption sEnumOption = new TunableSingleEnumOption();

        Type mType;

        List<FieldInfo> mFields;

        static TunableTypeOption()
        {
            foreach (ITunableConvertOption option in Common.DerivativeSearch.Find<ITunableConvertOption>())
            {
                Type fieldType = option.FieldType;
                if (fieldType == null) continue;

                sLookup.Add(fieldType, option);
            }
        }
        public TunableTypeOption(Type type, List<FieldInfo> fields)
            : base(type.ToString())
        {
            mName = mName.Substring(mName.LastIndexOf('.') + 1);

            mType = type;
            mFields = fields;
        }

        public void Export(Common.StringBuilder result)
        {
            foreach (ITunableFieldOption option in GetOptions())
            {
                option.Export(result);
            }
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public Type FieldType
        {
            get { return null; }
        }

        public ITunableConvertOption Clone(TunableFieldInfo field)
        {
            return null;
        }

        public object Convert(string value, bool fireError)
        {
            return null;
        }

        public static ITunableConvertOption GetFieldOption(Type fieldType)
        {
            ITunableConvertOption staticOption;
            if (sLookup.TryGetValue(fieldType, out staticOption))
            {
                return staticOption;
            }
            else if (fieldType.IsEnum)
            {
                return sEnumOption;
            }
            else if (fieldType.GetFields().Length > 0)
            {
                return sNestedOption;
            }
            else if (fieldType.IsArray)
            {
                return sArrayOption;
            }
            else
            {
                return null;
            }
        }

        public override List<ITunableFieldOption> GetOptions()
        {
            List<ITunableFieldOption> results = new List<ITunableFieldOption>();

            foreach (FieldInfo field in mFields)
            {
                ITunableConvertOption staticOption = GetFieldOption(field.FieldType);
                if (staticOption != null)
                {
                    results.Add(staticOption.Clone(new TunableFieldInfo (field, null)));
                }
                else
                {
                    results.Add(new TunableUnhandledOption(new TunableFieldInfo(field, null)));
                }
            }

            return results;
        }
    }
}
