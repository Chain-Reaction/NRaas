using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Interactions;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

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
