using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Helpers
{
    public class BroadcasterCleanup : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            try
            {
                foreach (Lot lot in LotManager.AllLots)
                {
                    if (lot == null) continue;

                    if (lot.mSavedData == null) continue;

                    List<ReactionBroadcaster> all = new List<ReactionBroadcaster>(lot.mSavedData.mReactions);

                    foreach (ReactionBroadcaster broadcaster in all)
                    {
                        if (broadcaster == null) continue;

                        if (broadcaster.mOnEnterCallback == SocialComponentEx.ReactToJealousEventHigh)
                        {
                            lot.RemoveReaction(broadcaster);
                        }
                        else if (broadcaster.mOnEnterCallback == SocialComponentEx.ReactToJealousEventMedium)
                        {
                            lot.RemoveReaction(broadcaster);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("BroadcasterCleanup:OnWorldLoadFinished", e);
            }
        }
    }
}
