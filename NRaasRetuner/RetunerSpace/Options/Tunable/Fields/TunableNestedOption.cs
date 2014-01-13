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
    public class TunableNestedOption : InteractionOptionList<ITunableFieldOption, GameObject>, ITunableFieldOption, ITunableConvertOption
    {
        TunableFieldInfo mField;

        public TunableNestedOption()
        { }
        public TunableNestedOption(TunableFieldInfo field)
            : base(field.Name)
        {
            mField = field;
        }

        public Type FieldType
        {
            get { return null; }
        }

        public ITunableConvertOption Clone(TunableFieldInfo field)
        {
            return new TunableNestedOption(field);
        }

        public object Convert(string value, bool fireError)
        {
            return null;
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override string DisplayValue
        {
            get
            {
                return mField.Field.FieldType.Name;
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public void Export(Common.StringBuilder result)
        {
            List<ITunableFieldOption> options = GetOptions();
            options.Sort(SortByName);

            foreach (ITunableFieldOption option in options)
            {
                option.Export(result);
            }
        }

        public static int SortByName(ITunableFieldOption l, ITunableFieldOption r)
        {
            return l.Name.CompareTo(r.Name);
        }

        public override List<ITunableFieldOption> GetOptions()
        {
            List<ITunableFieldOption> results = new List<ITunableFieldOption>();

            if (mField.Depth < 10)
            {
                foreach (FieldInfo field in mField.Field.FieldType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    ITunableConvertOption option = TunableTypeOption.GetFieldOption(field.FieldType);
                    if (option == null) continue;

                    results.Add(option.Clone(new TunableFieldInfo(field, mField)));
                }
            }

            return results;
        }
    }
}
