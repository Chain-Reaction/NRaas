using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Interactions;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TrafficSpace.Interactions
{
    public class PrimaryInteraction : ListedInteraction<IPrimaryOption<GameObject>,GameObject>
    {
        static InteractionDefinition Singleton = new CommonDefinition<PrimaryInteraction>();

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddRoot(Singleton);
        }
    }
}
