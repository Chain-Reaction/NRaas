using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims
{
    public class Focus : SimFromList, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "Focus";
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim != null) return true;

            return (Select.FindPlaceholderForSim(me) != null);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            Sim.Placeholder placeholder = Select.FindPlaceholderForSim(me);
            if (placeholder != null)
            {
                if (me.LotHome != null)
                {
                    Camera.FocusOnLot(me.LotHome.LotId, 0f);
                }
            }
            else
            {
                if (me.CreatedSim == null) return false;

                Camera.FocusOnSim(me.CreatedSim, 6f, -45f, 1f, true, CameraController.IsMapViewModeEnabled());
            }

            return true;
        }
    }
}
