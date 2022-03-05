using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;

namespace NRaas.Gameplay.Rewards
{
    public class ThirstDeathReward : RewardInfoEx
    {
        public ThirstDeathReward()
        { }

        public override void Grant(Sim actor, object target)
        {
            if (actor.SimDescription.IsDead) return;

            actor.Kill(SimDescription.DeathType.Thirst);
        }
    }
}
