using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class Focus : HouseholdFromList, IHouseholdOption
    {
        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (parameters.mTarget is Lot) return false;

            return base.Allow(parameters);
        }

        public override string GetTitlePrefix()
        {
            return "FocusHouse";
        }

        protected override Lot GetLot(SimDescription sim)
        {
            return sim.LotHome;
        }

        protected override bool Allow(Lot lot, Household me)
        {
            if (!base.Allow(lot, me)) return false;

            if (lot is WorldLot) return false;

            return (lot != null);
        }

        public static OptionResult Perform(Lot lot)
        {
            if (lot == null) return OptionResult.Failure;

            if (CameraController.IsMapViewModeEnabled())
            {
                Sims3.Gameplay.Core.Camera.ToggleMapView();
            }

            Camera.FocusOnLot(lot.LotId, 0f);

            return OptionResult.SuccessClose;
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            return Perform(lot);
        }
    }
}
