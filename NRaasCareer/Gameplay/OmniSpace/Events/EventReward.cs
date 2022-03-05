using NRaas.CareerSpace.Booters;
using NRaas.Gameplay.Rewards;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;

namespace NRaas.Gameplay.OmniSpace.Events
{
    public class EventReward : Career.EventReward
    {
        // Methods
        public EventReward()
        { }
        public EventReward(XmlDbRow row, Dictionary<string, Dictionary<int, CareerLevel>> careerLevels, string careerName)
            : base(row, careerLevels, careerName)
        {
            mYesRewardsList = OpportunityBooter.ParseRewards(row, sYesRewardColumns, kNumRewards);
        }

        public override void GiveRewards(bool bDialogResult, Sim sim, string icon, Sims3.Gameplay.ActorSystems.Origin origin)
        {
            base.GiveRewards(bDialogResult, sim, icon, origin);

            RewardInfoEx.GiveRewards(sim, null, mYesRewardsList);
        }
    }
}
