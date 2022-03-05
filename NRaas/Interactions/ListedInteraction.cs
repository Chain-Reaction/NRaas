using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;

namespace NRaas.CommonSpace.Interactions
{
    public abstract class ListedInteraction<T, TTarget> : CommonInteraction<T, TTarget>
        where T : class, IInteractionOptionItem<IActor,TTarget,GameHitParameters<TTarget>>
        where TTarget : class, IGameObject
    {
        protected override OptionResult Perform(IActor actor, TTarget target, GameObjectHit hit)
        {
            return new InteractionOptionList<T, TTarget>.AllList(GetInteractionName(actor, target, hit), SingleSelection).Perform(new GameHitParameters< TTarget>(actor, target, hit));
        }
    }
}
