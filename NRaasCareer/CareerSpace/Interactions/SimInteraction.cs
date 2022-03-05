using NRaas.CareerSpace.Options.Sims;
using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;

namespace NRaas.CareerSpace.Interactions
{
    public class SimInteraction : CommonInteraction<ISimOption, Sim>
    {
        public static InteractionDefinition Singleton = new CommonDefinition<SimInteraction>(true, true);

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        protected override OptionResult Perform(IActor actor, Sim target, GameObjectHit hit)
        {
            throw new NotImplementedException();
        }
    }
}
