using NRaas.CareerSpace.Situations;
using Sims3.Gameplay.Actors;

namespace NRaas.Gameplay.Rewards
{
    public class ArrestReward : RewardInfoEx
    {
        public ArrestReward()
        { }

        public override void Grant(Sim actor, object target)
        {
            actor.InteractionQueue.CancelAllInteractions();

            SimArrestSituationEx.Create(actor);
        }
    }
}
