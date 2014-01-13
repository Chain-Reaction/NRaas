using NRaas.CommonSpace.Interactions;
using NRaas.DebugEnablerSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class ObjectInfo : DebugEnablerInteraction<GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            list.Add(new InteractionObjectPair(Singleton, obj));
        }

        protected static Common.StringBuilder ComponentToString(ObjectComponent component)
        {
            Common.StringBuilder msg = new Common.StringBuilder();

            if (component is Door.DoorPortalComponent)
            {
                Door.DoorPortalComponent doorPortalComponent = component as Door.DoorPortalComponent;

                msg += Common.NewLine + " OwnerDoor: " + (doorPortalComponent.OwnerDoor != null);
                msg += Common.NewLine + " mAddPortalsCallback: " + (doorPortalComponent.mAddPortalsCallback != null);

                if (doorPortalComponent.mLockedLaneIndices != null)
                {
                    foreach (KeyValuePair<Sim, Door.DoorPortalComponent.LaneInfo> pair in doorPortalComponent.mLockedLaneIndices)
                    {
                        msg += Common.NewLine + " Key: " + pair.Key.FullName;

                        msg += Common.NewLine + " mLaneIndex: " + pair.Value.mLaneIndex;
                        msg += Common.NewLine + " mLaneFlags: " + pair.Value.mLaneFlags;

                        if (pair.Value.mLaneSlots != null)
                        {
                            msg += Common.NewLine + " mLaneSlots: " + pair.Value.mLaneSlots.Length;
                            foreach (Slot slot in pair.Value.mLaneSlots)
                            {
                                msg += Common.NewLine + "  " + slot;
                            }
                        }
                        else
                        {
                            msg += Common.NewLine + " mLaneSlots: <null>";
                        }
                    }
                }
                else
                {
                    msg += Common.NewLine + " mLockedLaneIndices: <null>";
                }
            }

            return msg;
        }

        public override bool Run()
        {
            Common.StringBuilder msg = new Common.StringBuilder(Common.LocalizeEAString("SimInteractions/DebugInteraction:WhoAmI"));

            msg += Common.NewLine + Target.GetType();

            try
            {
                msg += Common.NewLine + Common.NewLine;

                Common.StringBuilder noticeText = new Common.StringBuilder();
                Common.StringBuilder logText = new Common.StringBuilder();
                Common.LoadLogger.Convert(Target, noticeText, logText);

                msg += logText;
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, msg, exception);
            }

            try
            {
                if (Target.mObjComponents != null)
                {
                    msg += Common.NewLine + "mObjComponents";

                    foreach (ObjectComponent component in Target.mObjComponents)
                    {
                        msg += Common.NewLine + component.GetType();

                        msg += ComponentToString(component);
                    }
                }
                else
                {
                    msg += Common.NewLine + "mObjComponents: <null>";
                }

                if (Target.ActorsUsingMe != null)
                {
                    foreach (Sim sim in Target.ActorsUsingMe)
                    {
                        msg += Common.NewLine + "ActorsUsingMe: " + ((sim != null) ? sim.FullName : "null");
                    }
                }
                else
                {
                    msg += Common.NewLine + "ActorsUsingMe: <null>";
                }

                if (Target.RoutingReferenceList != null)
                {
                    foreach (Sim sim in Target.RoutingReferenceList)
                    {
                        msg += Common.NewLine + "RoutingReferenceList: " + ((sim != null) ? sim.FullName : "null");
                    }
                }
                else
                {
                    msg += Common.NewLine + "RoutingReferenceList: <null>";
                }

                if (Target.ReferenceList != null)
                {
                    foreach (Sim sim in Target.ReferenceList)
                    {
                        msg += Common.NewLine + "ReferenceList: " + ((sim != null) ? sim.FullName : "null");
                    }
                }
                else
                {
                    msg += Common.NewLine + "ReferenceList: <null>";
                }

                if (Target is ImaginaryDoll)
                {
                    ImaginaryDoll target = Target as ImaginaryDoll;

                    msg += Common.NewLine + "Gender Set: " + target.mGenderSet;
                    msg += Common.NewLine + "Is Female: " + target.mIsFemale;
                    msg += Common.NewLine + "Live State Id: " + target.mLiveStateSimDescId;
                    msg += Common.NewLine + "Days Since Reminder: " + target.mDaysSinceRemindingPlayerToLetDollTurnLive;
                    if (target.mOwner != null)
                    {
                        msg += Common.NewLine + "Owner: " + target.mOwner.FullName;
                    }
                    msg += Common.NewLine + "Relationship: " + target.mRelationship;
                    msg += Common.NewLine + "OwnershipState: " + target.mOwnershipState;
                    msg += Common.NewLine + "Start Neglect: " + target.mStartTimeOfNeglect;
                }
                else if (Target is Fish)
                {
                    Fish target = Target as Fish;

                    msg += Common.NewLine + "Fish Type: " + target.mFishType;
                    msg += Common.NewLine + "Fish Data: " + target.mData;
                }
                else if (Target is MountedFish)
                {
                    MountedFish target = Target as MountedFish;

                    msg += Common.NewLine + "Fish: " + target.mFish;
                    if (target.mFish != null)
                    {
                        msg += Common.NewLine + "Type: " + target.mFish.GetType();
                        msg += Common.NewLine + "Type: " + target.mFish.mFishType;
                        msg += Common.NewLine + "Data: " + target.mFish.Data;
                    }
                }
                else if (Target is Door)
                {
                    Door door = Target as Door;

                    msg += Common.NewLine + "mEventCounter: " + door.mEventCounter;
                    msg += Common.NewLine + "mIsNPCDoor: " + door.mIsNPCDoor;

                    uint numDoors = (uint)door.GetNumDoors();
                    for (uint i = 0; i < numDoors; i++)
                    {
                        msg += Common.NewLine + "Index: " + i;
                        msg += Common.NewLine + "State: " + door.GetDoorState(i);
                    }

                    if (door.mDoorOpenStates != null)
                    {
                        foreach (Door.DoorOpenState state in door.mDoorOpenStates)
                        {
                            msg += Common.NewLine + "mAlarm: " + state.mAlarm;
                            msg += Common.NewLine + "mFirstSideOpened: " + state.mFirstSideOpened;
                            msg += Common.NewLine + "mUseCount: " + state.mUseCount;
                        }
                    }
                    else
                    {
                        msg += Common.NewLine + "mDoorOpenStates: null";
                    }

                    if (door.mEventListDictionary != null)
                    {
                        foreach (KeyValuePair<DoorEvent.tEventType, List<DoorEvent>> pair in door.mEventListDictionary)
                        {
                            msg += Common.NewLine + "tEventType: " + pair.Key;

                            foreach (DoorEvent ev in pair.Value)
                            {
                                msg += Common.NewLine + " mDoorIndex: " + ev.mDoorIndex;
                                msg += Common.NewLine + " mDoorObjId: " + ev.mDoorObjId;
                                msg += Common.NewLine + " mDoorSide: " + ev.mDoorSide;
                                msg += Common.NewLine + " mLocalIndex: " + ev.mLocalIndex;
                                msg += Common.NewLine + " mSenderObjId: " + ev.mSenderObjId;
                                msg += Common.NewLine + " mType: " + ev.mType;
                            }
                        }
                    }
                    else
                    {
                        msg += Common.NewLine + "mEventListDictionary: null";
                    }
                }
                else if (Target is Sim)
                {

                }

                Common.Notify(msg);
                Common.WriteLog(msg);
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, msg, exception);
            }
            return true;
        }

        [DoesntRequireTuning]
        private class Definition : DebugEnablerDefinition<ObjectInfo>
        {
            public override string[] GetPath(bool isFemale)
            {
                return Cheats.ObjectPath;
            }

            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.LocalizeEAString("SimInteractions/DebugInteraction:WhoAmI");
            }
        }
    }
}