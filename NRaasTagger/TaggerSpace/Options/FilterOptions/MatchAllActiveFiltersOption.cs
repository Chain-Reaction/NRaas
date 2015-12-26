using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.TaggerSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options.FilterOptions
{
    public class MatchAllActiveFiltersOption : BooleanSettingOption<GameObject>, IFilterRootOption
    {
        protected override bool Value
        {
            get
            {
                return Tagger.Settings.mMatchAllActiveFilters;
            }
            set
            {
                Tagger.Settings.mMatchAllActiveFilters = value;

                Tagger.InitTags(false);
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return FilterHelper.GetFilters().Count > 0;
        }

        public override string GetTitlePrefix()
        {
            return "MatchAllActiveFilters";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}