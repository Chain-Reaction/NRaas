using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;

namespace NRaas.Gameplay.Rewards
{
    public class MeteorDeathReward : RewardInfoEx
    {
        public MeteorDeathReward()
        { }

        public override void Grant(Sim actor, object target)
        {
            if (actor.SimDescription.IsDead) return;

            actor.Kill(SimDescription.DeathType.Meteor);
        }
    }
}
