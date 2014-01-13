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

namespace NRaas.HybridSpace.Interactions
{
    public class PrimaryInteraction : ListedInteraction<IPrimaryOption<GameObject>,GameObject>
    {
        static InteractionDefinition Singleton = new CommonDefinition<PrimaryInteraction>();

        protected override bool Test(IActor actor, GameObject target, GameObjectHit hit, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (!Common.kDebugging) return false;

            return base.Test(actor, target, hit, ref greyedOutTooltipCallback);
        }

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddRoot(Singleton);
        }
    }
}
