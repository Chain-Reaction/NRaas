using Sims3.Gameplay.Actors;
using System.Collections;

namespace NRaas.Gameplay.Rewards
{
    public abstract class RewardInfoEx
    {
        public RewardInfoEx()
        { }

        public abstract void Grant(Sim actor, object target);

        public static void GiveRewards(Sim actor, object target, ArrayList rewards)
        {
            foreach (object obj in rewards)
            {
                RewardInfoEx reward = obj as RewardInfoEx;
                if (reward == null) continue;

                reward.Grant(actor, target);
            }
        }
    }
}
