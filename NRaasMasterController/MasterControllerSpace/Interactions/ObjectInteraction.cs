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
using Sims3.Gameplay.Interfaces;
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
    public class ObjectInteraction : PrimaryInteraction<IObjectOption>
    {
        public static InteractionDefinition Singleton = new CommonDefinition<ObjectInteraction>(true, true);

        protected override string GetInteractionName(IActor actor, GameObject target, GameObjectHit hit)
        {
            return base.GetInteractionName(actor, target, hit) + ": " + target.CatalogName;
        }

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<GameObject>(Singleton);
        }
    }
}
