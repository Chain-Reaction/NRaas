using NRaas.CommonSpace.Options;
using NRaas.GoHereSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Options.DoorFilters
{
    public class FilterMatchAllOption : BooleanSettingOption<GameObject>, IDoorOption
    {
        GameObject mTarget;

        protected override bool Value
        {
            get
            {
                if (mTarget != null)
                    return GoHere.Settings.GetDoorSettings(mTarget.ObjectId).mMatchAllFilters;

                return false;
            }
            set
            {
                if (mTarget != null)
                {
                    DoorPortalComponentEx.DoorSettings settings = GoHere.Settings.GetDoorSettings(mTarget.ObjectId);
                    settings.mMatchAllFilters = value;
                    GoHere.Settings.AddOrUpdateDoorSettings(mTarget.ObjectId, settings, true);
                }
            }
        }

        public override void Export(CommonSpace.Helpers.Persistence.Lookup settings)
        {
            return;
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mTarget = parameters.mTarget;
            return base.Allow(parameters);
        }

        public override string GetTitlePrefix()
        {
            return "MatchFiltersAllOption";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
