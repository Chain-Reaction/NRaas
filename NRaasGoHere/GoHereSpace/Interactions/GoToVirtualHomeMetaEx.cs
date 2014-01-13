using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class GoToVirtualHomeMetaEx : Common.IPreLoad
    {
        public void OnPreLoad()
        {
            Tunings.Inject<Lot, Sim.GoToVirtualHomeMeta.Definition, Definition>(false);

            Sim.GoToVirtualHomeMeta.Singleton = new Definition();
        }

        public class Definition : Sim.GoToVirtualHomeMeta.Definition
        {
            public Definition()
            { }

            public override bool Test(Sim actor, Lot target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                // Stops the dead from leaving the cemetery
                if ((actor.SimDescription.IsDead) && (!actor.SimDescription.IsPlayableGhost)) return false;

                return base.Test(actor, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
