using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Object;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class ComputerInteraction : PrimaryInteraction<IPrimaryOption<GameObject>>
    {
        public static InteractionDefinition Singleton = new CommonDefinition<ComputerInteraction>();

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddRoot(Singleton);
        }
    }
}
