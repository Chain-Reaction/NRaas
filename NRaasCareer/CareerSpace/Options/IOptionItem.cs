using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;

namespace NRaas.CareerSpace.Options
{
    public interface IOptionItem<T> : IInteractionOptionItem<IActor,T,GameHitParameters<T>>
        where T : GameObject
    { }
}
