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

                HandleDescendants(false);
            }
        }

        public override string GetTitlePrefix()
        {
            return "DisableDescendants";
        }        

        public void OnWorldLoadFinished()
        {
            HandleDescendants(true);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {            
            return base.Run(parameters);
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

        private void HandleDescendants(bool fromWorldLoadFinished)
        {
            bool disabled = Traveler.Settings.mDisableDescendants;

            if (!disabled && !fromWorldLoadFinished && !GameUtils.IsFutureWorld())
            {
                // originally had this GameUtils.IsFutureWorld and a call to RegenerateDescendants here but it
                // creates problems when all the progninators don't travel to the future. It's a solvable issue
                // but not right now
                FutureDescendantServiceEx.AddListeners();
            }
            else if (disabled && !fromWorldLoadFinished)
            {
                FutureDescendantServiceEx.WipeDescendants();
                FutureDescendantServiceEx.ClearListeners();
            }
            else if (disabled && fromWorldLoadFinished)
            {
                FutureDescendantServiceEx.ClearListeners();
            }
        }
    }
}