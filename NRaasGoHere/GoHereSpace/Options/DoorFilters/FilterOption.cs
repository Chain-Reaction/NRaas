using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.GoHereSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Options.DoorFilters
{
    public class FilterOption : FilterBaseOption<GameObject>, IFilterOption
    {
        public GameObject mTarget;

        public FilterOption(string filter, GameObject target)
        {
            mName = filter;
            mTarget = target;
        }

        protected override DoorPortalComponentEx.DoorSettings Value
        {
            get
            {
                return GoHere.Settings.GetDoorSettings(mTarget.ObjectId);
            }
            set
            {
                GoHere.Settings.AddOrUpdateDoorSettings(mTarget.ObjectId, (!Value.IsFilterActive(mName) ? Value.AddFilter(mName) : Value.RemoveFilter(mName)), true);                
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override string DisplayValue
        {
            get
            {
                return (Value.IsFilterActive(mName) ? Common.Localize("Boolean:True") : Common.Localize("Boolean:False"));
            }
        }

        public override string Name
        {
            get
            {
                return FilterHelper.StripNamespace(mName);
            }
        }
    }
}