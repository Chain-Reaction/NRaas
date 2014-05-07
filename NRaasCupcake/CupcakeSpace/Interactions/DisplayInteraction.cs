using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.CupcakeSpace.Options.Displays;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.SimIFace;
using Sims3.Store.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CupcakeSpace.Interactions
{
    public class DisplayInteraction : ListedInteraction<ICaseOption, GameObject>
    {
        public static InteractionDefinition Singleton = new CommonDefinition<DisplayInteraction>(true, true);

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {            
            interactions.Add<CraftersConsignmentDisplay>(Singleton);
        }        
    }
}
