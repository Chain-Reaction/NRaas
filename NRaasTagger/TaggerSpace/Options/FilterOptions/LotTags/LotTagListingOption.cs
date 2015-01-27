using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.TaggerSpace.Options.ColorOptions;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options.FilterOptions.LotTags
{
    public class LotTagListingOption : InteractionOptionList<IFilterOption, GameObject>, IFilterRootOption
    {
        public override string GetTitlePrefix()
        {
            return "LotTagRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return base.Allow(parameters);
        }

        public override List<IFilterOption> GetOptions()
        {
            List<IFilterOption> results = new List<IFilterOption>();            

            Dictionary<string, bool> filters = FilterHelper.GetFilters();

            foreach (KeyValuePair<string, bool> value in filters)
            {
                results.Add(new FilterOption(value.Key, FilterOption.FilterType.LotTag));
            }

            return results;
        }
    }
}
