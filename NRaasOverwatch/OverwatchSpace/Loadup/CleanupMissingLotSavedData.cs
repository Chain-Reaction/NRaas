using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupMissingLotSavedData : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupMissingLotSavedData");

            foreach (Lot lot in LotManager.AllLots)
            {
                if (lot.mSavedData == null)
                {
                    Overwatch.Log(lot.Name + " Added SavedData");

                    lot.mSavedData = new Lot.SavedData();
                }

                if (lot.mSavedData.mReactions == null)
                {
                    Overwatch.Log(lot.Name + " Added Reactions");

                    lot.mSavedData.mReactions = new List<ReactionBroadcaster>();
                }

                if (lot.mSavedData.mBroadcastersWithSims == null)
                {
                    Overwatch.Log(lot.Name + " Added Broadcasters with sims");

                    lot.mSavedData.mBroadcastersWithSims = new List<ReactionBroadcaster>();
                }

                if (lot.mSavedData.mAlarmManager == null)
                {
                    Overwatch.Log(lot.Name + " Added AlarmManager");

                    lot.mSavedData.mAlarmManager = new AlarmManager(lot.LotId);
                }

                if (lot.mSavedData.mPuddleManager == null)
                {
                    Overwatch.Log(lot.Name + " Added PuddleManager");

                    lot.mSavedData.mPuddleManager = new PuddleManager();
                }

                if (lot.mSavedData.mFireManager == null)
                {
                    Overwatch.Log(lot.Name + " Added FireManager");

                    lot.mSavedData.mFireManager = new FireManager();
                }

                if (lot.mSavedData.mLaundryManager == null)
                {
                    Overwatch.Log(lot.Name + " Added LaundryManager");

                    lot.mSavedData.mLaundryManager = new LaundryManager();
                }

                if (lot.ResortManager != null)
                {
                    if (lot.ResortManager.mOwnerLot == null || LotManager.GetLot(lot.ResortManager.mOwnerLot.LotId) == null)
                    {
                        Overwatch.Log(lot.Name + " Fixed ResortManager");
                        lot.ResortManager.mOwnerLot = lot;
                    }
                }
            }
        }
    }
}
