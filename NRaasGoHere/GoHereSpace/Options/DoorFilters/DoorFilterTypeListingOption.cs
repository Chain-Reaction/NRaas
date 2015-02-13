using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.GoHereSpace.Helpers;
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

namespace NRaas.GoHereSpace.Options.DoorFilters
{
    public class DoorFilterTypeListingOption : InteractionOptionList<IFilterTypeOption, GameObject>, IFilterRootOption
    {
        GameObject mTarget;

        public override string GetTitlePrefix()
        {
            return "FilterType";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mTarget = parameters.mTarget;
            return base.Allow(parameters);
        }

        public override List<IFilterTypeOption> GetOptions()
        {
            List<IFilterTypeOption> results = new List<IFilterTypeOption>();

            foreach (DoorPortalComponentEx.DoorSettings.SettingType value in Enum.GetValues(typeof(DoorPortalComponentEx.DoorSettings.SettingType)))
            {
                if(value == DoorPortalComponentEx.DoorSettings.SettingType.Unset) continue;

                results.Add(new FilterTypeOption(value, mTarget));
            }

            return results;
        }
    }
}