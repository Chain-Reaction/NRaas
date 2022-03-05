using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;

namespace NRaas.Gameplay.Rewards
{
    public class CurseDeathReward : RewardInfoEx
    {
        public CurseDeathReward()
        { }

        public override void Grant(Sim actor, object target)
        {
            if (actor.SimDescription.IsDead) return;

            actor.Kill(SimDescription.DeathType.MummyCurse);
        }
    }
}
