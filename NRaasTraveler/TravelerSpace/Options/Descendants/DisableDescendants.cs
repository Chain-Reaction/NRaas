using NRaas.CommonSpace.Options;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
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

namespace NRaas.TravelerSpace.Options.Descendants
{
    public class DisableDescendants : BooleanSettingOption<GameObject>, Common.IWorldLoadFinished, IDescendantOption
    {
        protected override bool Value
        {
            get
            {
                return Traveler.Settings.mDisableDescendants;
            }
            set
            {
                Traveler.Settings.mDisableDescendants = value;

                Traveler.Settings.HandleDescendants(false);
            }
        }

        public override string GetTitlePrefix()
        {
            return "DisableDescendants";
        }        

        public void OnWorldLoadFinished()
        {
            Traveler.Settings.HandleDescendants(true);
        }        

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            // wasn't here originally
            if (GameUtils.IsFutureWorld()) return false;

            return base.Allow(parameters);
        }        
    }
}