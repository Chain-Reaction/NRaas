using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;

namespace NRaas.CareerSpace.Interactions
{
    public class PrimaryInteraction : ListedInteraction<IPrimaryOption<GameObject>, GameObject>
    {
        public static InteractionDefinition Singleton = new CommonDefinition<PrimaryInteraction>();

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddRoot(Singleton);
        }

        protected override bool Test(IActor actor, GameObject target, GameObjectHit hit, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return Common.IsRootMenuObject(target);
        }
    }
}
