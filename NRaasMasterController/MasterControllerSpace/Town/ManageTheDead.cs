using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Town
{
    public class ManageTheDead : MausoleumBase
    {
        public override string GetTitlePrefix()
        {
            return "ManageTheDead";
        }

        protected override OptionResult Run(IActor actor, IMausoleum mausoleum)
        {
            if (CameraController.IsMapViewModeEnabled())
            {
                Sims3.Gameplay.Core.Camera.ToggleMapView();
            }

            HudModel.OpenObjectInventoryForOwner(mausoleum);
            return OptionResult.SuccessClose;
        }
    }
}
