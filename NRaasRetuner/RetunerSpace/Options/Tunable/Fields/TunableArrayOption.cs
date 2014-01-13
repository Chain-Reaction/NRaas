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
    public class TunableArrayOption : InteractionOptionList<ITunableFieldOption, GameObject>, ITunableFieldOption, ITunableConvertOption
    {
        TunableFieldInfo mField;

        public TunableArrayOption()
        { }
        public TunableArrayOption(TunableFieldInfo field)
            : base(field.Name)
        {
            mField = field;
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override string DisplayValue
        {
            get
            {
                return Common.Localize("Type:List");
            }
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
            return new TunableArrayOption(field);
        }

        public object Convert(string value, bool fireError)
        {
            return null;
        }

        public void Export(Common.StringBuilder result)
        {
            foreach (ITunableFieldOption option in GetOptions())
            {
                option.Export(result);
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return true;
        }

        public override List<ITunableFieldOption> GetOptions()
        {
            Array array = (Array)mField.GetValue(true);

            List<ITunableFieldOption> results = new List<ITunableFieldOption>();

            for (int i = 0; i < array.Length; i++)
            {
                object obj = array.GetValue (i);
                if (obj == null) continue;

                ITunableConvertOption arrayOption = TunableTypeOption.GetFieldOption(obj.GetType());
                if (arrayOption == null)
                {
                    results.Add(new TunableUnhandledOption(new ArrayFieldInfo(mField, i)));
                }
                else
                {
                    results.Add(arrayOption.Clone(new ArrayFieldInfo(mField, i)));
                }
            }

            return results;
        }
    }
}
