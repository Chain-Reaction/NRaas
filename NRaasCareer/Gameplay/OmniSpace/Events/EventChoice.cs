using NRaas.CareerSpace.Booters;
using NRaas.Gameplay.Rewards;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;

namespace NRaas.Gameplay.OmniSpace.Events
{
    public class EventChoice : Career.EventChoice
    {
        // Methods
        public EventChoice()
        { }
        public EventChoice(XmlDbRow row, Dictionary<string, Dictionary<int, CareerLevel>> careerLevels, string careerName)
            : base(row, careerLevels, careerName)
        {
            mYesRewardsList = OpportunityBooter.ParseRewards(row, sYesRewardColumns, kNumRewards);

            mYesLoseRewardsList = OpportunityBooter.ParseRewards(row, sYesLoseRewardColumns, kNumRewards);

            mNoRewardsList = OpportunityBooter.ParseRewards(row, sNoRewardColumns, kNumRewards);
        }

        public override void GiveRewards(bool bDialogResult, Sim sim, string icon, Origin origin)
        {
            if (bDialogResult)
            {
                if (RewardsManager.CheckForWin(mChanceToWin, mModifiersList, sim))
                {
                    base.ShowEventTNS(Common.LocalizeEAString(sim.IsFemale, mYesResultText, new object[] { sim }), sim.ObjectId, icon);
                    RewardsManager.GiveRewards(mYesRewardsList, sim, origin);

                    RewardInfoEx.GiveRewards(sim, null, mYesRewardsList);
                }
                else
                {
                    base.ShowEventTNS(Common.LocalizeEAString(sim.IsFemale, mYesLoseResultText, new object[] { sim }), sim.ObjectId, icon);
                    RewardsManager.GiveRewards(mYesLoseRewardsList, sim, origin);

                    RewardInfoEx.GiveRewards(sim, null, mYesLoseRewardsList);
                }
            }
            else
            {
                base.ShowEventTNS(Common.LocalizeEAString(sim.IsFemale, mNoResultText, new object[] { sim }), sim.ObjectId, icon);
                RewardsManager.GiveRewards(mNoRewardsList, sim, origin);

                RewardInfoEx.GiveRewards(sim, null, mNoRewardsList);
            }
        }
    }
}
