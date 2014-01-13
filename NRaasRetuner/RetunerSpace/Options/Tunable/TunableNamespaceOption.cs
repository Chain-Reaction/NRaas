using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
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
    public class TunableNamespaceOption : InteractionOptionList<TunableTypeOption, GameObject>
    {
        Dictionary<Type, List<FieldInfo>> mTypes;

        public TunableNamespaceOption(string name, Dictionary<Type, List<FieldInfo>> types)
            : base(name)
        {
            mTypes = types;
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override List<TunableTypeOption> GetOptions()
        {
            List<TunableTypeOption> results = new List<TunableTypeOption>();

            foreach (KeyValuePair<Type, List<FieldInfo>> pair in mTypes)
            {
                results.Add(new TunableTypeOption(pair.Key, pair.Value));
            }

            return results;
        }

        public void Export(Common.StringBuilder results)
        {
            List<TunableTypeOption> options = GetOptions();
            options.Sort(SortByName);

            foreach (TunableTypeOption option in options)
            {
                option.Export(results);
            }
        }
    }
}
