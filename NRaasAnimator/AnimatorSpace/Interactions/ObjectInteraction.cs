using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Interactions;
using NRaas.AnimatorSpace.Options;
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

namespace NRaas.AnimatorSpace.Interactions
{
    public class ObjectInteraction : CommonInteraction<IObjectOption, GameObject>
    {
        public static InteractionDefinition Singleton = new CommonDefinition<ObjectInteraction>(true, true);

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<GameObject>(Singleton);
        }

        protected override OptionResult Perform(IActor actor, GameObject target, GameObjectHit hit)
        {
            throw new NotImplementedException();
        }
    }
}
