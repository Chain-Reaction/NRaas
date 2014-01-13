using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.DresserSpace.Options.Sims;
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

namespace NRaas.DresserSpace.Interactions
{
    public class SimInteraction : ListedInteraction<ISimOption, Sim>
    {
        public static InteractionDefinition Singleton = new CommonDefinition<SimInteraction>();

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        protected override bool Test(IActor actor, Sim target, GameObjectHit hit, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return Dresser.Settings.mSimMenuVisibility;
        }
    }
}
