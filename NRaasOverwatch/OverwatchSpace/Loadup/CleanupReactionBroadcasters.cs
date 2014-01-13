using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupReactionBroadcasters : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupReactionBroadcasters");

            foreach (Lot lot in LotManager.Lots)
            {
                Lot.SavedData savedData = lot.mSavedData;

                if (savedData == null) continue;

                foreach (List<ReactionBroadcaster> broadcasters in new List<ReactionBroadcaster>[] { savedData.mBroadcastersWithSims, savedData.mReactions })
                {
                    if (broadcasters == null) continue;

                    Dictionary<ObjectGuid, ReactionBroadcaster.BroadcastCallback> enterLookup = new Dictionary<ObjectGuid, ReactionBroadcaster.BroadcastCallback>();
                    Dictionary<ObjectGuid, ReactionBroadcaster.BroadcastCallback> exitLookup = new Dictionary<ObjectGuid, ReactionBroadcaster.BroadcastCallback>();

                    int index = 0;
                    while (index < broadcasters.Count)
                    {
                        ReactionBroadcaster broadcaster = broadcasters[index];

                        string reason = null;
                        if ((broadcaster == null) || (broadcaster.mBroadcastingObject == null))
                        {
                            reason = " Invalid";
                        }
                        else if ((broadcaster.mOnEnterInteraction != null) && (broadcaster.mOnEnterInteraction.InteractionDefinition == null))
                        {
                            reason = " Bad Interaction";
                        }
                        else if (broadcaster.mBroadcastingObject.HasBeenDestroyed)
                        {
                            reason = " Destroyed Object";
                        }
                        else if (broadcaster.mBroadcastingObject.LotCurrent != lot)
                        {
                            reason = " Wrong Lot";
                        }
                        else
                        {
                            if (broadcaster.mOnEnterCallback != null)
                            {
                                ReactionBroadcaster.BroadcastCallback callback;
                                if (enterLookup.TryGetValue(broadcaster.mBroadcastingObject.ObjectId, out callback))
                                {
                                    if (callback == broadcaster.mOnEnterCallback)
                                    {
                                        reason = " Duplicate Enter Broadcaster: " + broadcaster.mBroadcastingObject;
                                    }
                                }
                                else
                                {
                                    enterLookup.Add(broadcaster.mBroadcastingObject.ObjectId, broadcaster.mOnEnterCallback);
                                }
                            }

                            if (broadcaster.mOnExitCallback != null)
                            {
                                ReactionBroadcaster.BroadcastCallback callback;
                                if (enterLookup.TryGetValue(broadcaster.mBroadcastingObject.ObjectId, out callback))
                                {
                                    if (callback == broadcaster.mOnExitCallback)
                                    {
                                        reason = " Duplicate Exit Broadcaster: " + broadcaster.mBroadcastingObject;
                                    }
                                }
                                else
                                {
                                    enterLookup.Add(broadcaster.mBroadcastingObject.ObjectId, broadcaster.mOnExitCallback);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(reason))
                        {
                            Overwatch.Log(reason);

                            broadcasters.RemoveAt(index);
                        }
                        else
                        {
                            index++;
                        }
                    }
                }
            }
        }
    }
}
