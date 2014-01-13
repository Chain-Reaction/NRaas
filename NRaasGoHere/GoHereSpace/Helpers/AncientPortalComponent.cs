using NRaas.CommonSpace.Helpers;
using NRaas.GoHereSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Helpers
{
    public class AncientPortalComponent : PortalComponent
    {
        static Dictionary<Sim, AncientPortal> sTargetPortals = new Dictionary<Sim, AncientPortal>();

        AncientPortal mPortal;
        Sim mCurrentlyRoutingSim;
        StateMachineClient mSMC;

        protected AncientPortalComponent()
        {
            mAddPortalsCallback = AddAncientPortal;
        }
        public AncientPortalComponent(GameObject owner) 
            : base(owner)
        {
            mPortal = owner as AncientPortal;
            mAddPortalsCallback = AddAncientPortal;
        }

        public static void AddTargetPortal(Sim sim, AncientPortal portal)
        {
            sTargetPortals[sim] = portal;
        }

        public override bool LockRoutingLane(Sim s, Vector3 preferredPos, bool checkForSims, Route.RouteOption routeOptions, out bool preferredLaneSelected)
        {
            preferredLaneSelected = true;
            return true;
            //return base.LockRoutingLane(s, preferredPos, checkForSims, routeOptions, out preferredLaneSelected);
        }

        public void AddAncientPortal()
        {
            try
            {
                if (mPortal.GetRoutingSlots().Length == 0) return;

                Vector3 startPos = Owner.GetPositionOfSlot(mPortal.GetRoutingSlots()[0]);
                Vector3 endPos = mPortal.PositionOnFloor;

                CASAGSAvailabilityFlags ageFlags = CASAGSAvailabilityFlags.HumanYoungAdult | CASAGSAvailabilityFlags.HumanTeen | CASAGSAvailabilityFlags.HumanAdult | CASAGSAvailabilityFlags.HumanElder | CASAGSAvailabilityFlags.HumanChild | CASAGSAvailabilityFlags.Male | CASAGSAvailabilityFlags.Female | CASAGSAvailabilityFlags.LeftHanded | CASAGSAvailabilityFlags.RightHanded;

                //ageFlags |= CASAGSAvailabilityFlags.CatAdult | CASAGSAvailabilityFlags.CatElder | CASAGSAvailabilityFlags.DogAdult | CASAGSAvailabilityFlags.DogElder | CASAGSAvailabilityFlags.LittleDogAdult | CASAGSAvailabilityFlags.LittleDogElder;

                Route.AddPortal(Owner.ObjectId, startPos, endPos, 1f, PortalType.PortalTypeAnimateThrough, RoutePortalFlags.OneWayPortal, ageFlags);
                Route.AddPortal(Owner.ObjectId, endPos, startPos, 1f, PortalType.PortalTypeAnimateThrough, RoutePortalFlags.OneWayPortal, ageFlags);
            }
            catch (Exception e)
            {
                Common.Exception("AddAncientPortal", e);
            }
        }

        public override uint GetNumLanes()
        {
            return 0x1;
        }

        public override PortalObjectType GetPortalObjectType()
        {
            return PortalObjectType.AnimateThroughPortalObject;
        }

        public override bool GetLaneInfo(Sim s, out LaneInfo lane)
        {
            lane = new PortalComponent.LaneInfo(0x0, new Slot[] { mPortal.GetRoutingSlots()[0], mPortal.GetRoutingSlots()[0] }, PortalComponent.LaneInfoFlags.None);
            return true;
        }

        public override Slot[] GetSlotsForLaneIndex(uint laneIndex)
        {
            try
            {
                return mPortal.GetRoutingSlots();
            }
            catch (Exception e)
            {
                Common.Exception("GetSlotsForLaneIndex", e);
                return new Slot[0];
            }
        }

        public override List<Slot> GetStartSlotsForApproachingSim(Sim s, Vector3 preferredPos, ref bool preferredLaneSelected)
        {
            try
            {
                List<Slot> list = new List<Slot>(0);
                list.Add(mPortal.GetRoutingSlots()[0]);

                s.SimRoutingComponent.OnRouteActionsFinished -= OnPortalApproachCancelledAndFinished;
                s.SimRoutingComponent.OnRouteActionsFinished += OnPortalApproachCancelledAndFinished;
                return list;
            }
            catch (Exception e)
            {
                Common.Exception(s, e);
                return new List<Slot>();
            }
        }

        private void OnPortalApproachCancelledAndFinished(GameObject routingObject, bool routeSuccess)
        {
            Sim s = routingObject as Sim;

            try
            {
                if (s != null)
                {
                    if (!routeSuccess)
                    {
                        s.Wander(1f, 2f, false, RouteDistancePreference.PreferNearestToRouteOrigin, false);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(s, e);
            }
        }
        
        private void HideSim(StateMachineClient sender, IEvent evt)
        {
            try
            {
                mCurrentlyRoutingSim.SetHiddenFlags(HiddenFlags.Model);
            }
            catch (Exception e)
            {
                Common.Exception(mCurrentlyRoutingSim, e);
            }
        }

        private void ShowSim(StateMachineClient sender, IEvent evt)
        {
            try
            {
                if (mCurrentlyRoutingSim != null)
                {
                    mCurrentlyRoutingSim.SetHiddenFlags(HiddenFlags.Nothing);
                }
            }
            catch (Exception e)
            {
                Common.Exception(mCurrentlyRoutingSim, e);
            }
        }

        public override bool OnPortalStart(Sim sim)
        {
            try
            {
                if ((mCurrentlyRoutingSim != null) && (mCurrentlyRoutingSim.HasBeenDestroyed))
                {
                    if (mSMC != null)
                    {
                        mSMC.Dispose();
                        mSMC = null;
                    }
                }

                mCurrentlyRoutingSim = sim;

                if (mSMC != null)
                {
                    sim.PlayRouteFailure(mPortal);
                    return false;
                }

                AncientPortal targetPortal = null;
                if (!sTargetPortals.TryGetValue(sim, out targetPortal))
                {
                    sim.PlayRouteFailure(mPortal);
                    return false;
                }

                if (targetPortal == mPortal)
                {
                    return false;
                }

                mPortal.AddToUseList(sim);

                sim.SimRoutingComponent.OnRouteActionsFinished -= OnPortalApproachCancelledAndFinished;
                sim.SetExitReasonsInterruptForMultiPortalRoute();
                sim.SimRoutingComponent.StartIgnoringObstacles();

                targetPortal.AddToUseList(sim);

                targetPortal.EnableFootprint(AncientPortal.CatchABeam.FootprintPlacementHash);
                targetPortal.PushSimsFromFootprint(AncientPortal.CatchABeam.FootprintPlacementHash, sim, null, false);

                Vector3 slotFoward = mPortal.GetForwardOfSlot(mPortal.GetRoutingSlots()[0]);

                sim.SetForward(slotFoward);

                mSMC = StateMachineClient.Acquire(sim, "AncientPortal", AnimationPriority.kAPDefault);
                mSMC.SetActor("x", sim);
                mSMC.SetActor("portal", mPortal);
                mSMC.EnterState("x", "Enter");
                mSMC.EnterState("portal", "Enter");

                mSMC.AddOneShotScriptEventHandler(0x65, HideSim);
                mSMC.AddOneShotScriptEventHandler(0x66, ShowSim);

                mSMC.RequestState("x", "InsidePortal");

                mPortal.RemoveFromUseList(sim);

                return true;
            }
            catch (ResetException)
            {
                if (mCurrentlyRoutingSim != null)
                {
                    mCurrentlyRoutingSim.SetHiddenFlags(HiddenFlags.Nothing);
                }

                throw;
            }
            catch (Exception e)
            {
                if (mCurrentlyRoutingSim != null)
                {
                    mCurrentlyRoutingSim.SetHiddenFlags(HiddenFlags.Nothing);
                }

                Common.Exception(sim, Owner, e);
                return false;
            }
        }

        public override bool OnPortalStop(Sim sim)
        {
            try
            {
                AncientPortal targetPortal = null;
                if (!sTargetPortals.TryGetValue(sim, out targetPortal))
                {
                    sim.PlayRouteFailure(mPortal);
                    return false;
                }

                mSMC.SetActor("portal", targetPortal);

                Slot slotName = targetPortal.GetRoutingSlots()[0x0];
                Vector3 positionOfSlot = targetPortal.GetPositionOfSlot(slotName);
                Vector3 forwardOfSlot = targetPortal.GetForwardOfSlot(slotName);
                sim.SetPosition(positionOfSlot);
                sim.SetForward(forwardOfSlot);

                targetPortal.DisableFootprint(AncientPortal.CatchABeam.FootprintPlacementHash);

                mSMC.RequestState("x", "Exit");

                if (SimTypes.IsSelectable(sim))
                {
                    for (int i = 0x0; i < AncientPortal.CatchABeam.kPotentialTravelBuffs.Length; i++)
                    {
                        if (RandomUtil.RandomChance(AncientPortal.CatchABeam.kChanceForEachBuff))
                        {
                            sim.BuffManager.AddElement(AncientPortal.CatchABeam.kPotentialTravelBuffs[i], Origin.FromAncientPortal);
                        }
                    }
                }

                targetPortal.RemoveFromUseList(sim);

                if (targetPortal.LotCurrent.IsResidentialLot && !sim.IsGreetedOnLot(targetPortal.LotCurrent))
                {
                    sim.GreetSimOnLot(targetPortal.LotCurrent);
                }

                if (mSMC != null)
                {
                    mSMC.Dispose();
                    mSMC = null;
                }

                mCurrentlyRoutingSim = null;

                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(sim, Owner, e);
                return false;
            }
            finally
            {
                if (mCurrentlyRoutingSim != null)
                {
                    mCurrentlyRoutingSim.SetHiddenFlags(HiddenFlags.Nothing);
                }
            }
        }

        public override void OnReset()
        {
            try
            {
                if (mSMC != null)
                {
                    mSMC.Dispose();
                    mSMC = null;
                }

                if (mPortal != null)
                {
                    if (mCurrentlyRoutingSim != null)
                    {
                        mPortal.RemoveFromUseList(mCurrentlyRoutingSim);
                    }
                }

                if ((mCurrentlyRoutingSim != null) && (!mCurrentlyRoutingSim.HasBeenDestroyed))
                {
                    mCurrentlyRoutingSim.SetHiddenFlags(HiddenFlags.Nothing);

                    AncientPortal targetPortal = null;
                    if (sTargetPortals.TryGetValue(mCurrentlyRoutingSim, out targetPortal))
                    {
                        targetPortal.RemoveFromUseList(mCurrentlyRoutingSim);

                        targetPortal.DisableFootprint(AncientPortal.CatchABeam.FootprintPlacementHash);
                    }
                }

                mCurrentlyRoutingSim = null;

                base.OnReset();
            }
            catch (Exception e)
            {
                Common.Exception(Owner, e);
            }
        }

        public override bool AddToUseListDuringTraversal
        {
            get { return true; }
        }

        public override bool RemoveFromUseListAfterTraversal
        {
            get { return true; }
        }

        public override PortalComponent.PortalAddInitTime ShouldAddPortalAtInit
        {
            get 
            {
                return PortalComponent.PortalAddInitTime.OnStartup; 
            }
        }

        public override bool CanUseUmbrella
        {
            get { return false; }
        }
    }
}
