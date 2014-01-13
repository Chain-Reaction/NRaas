using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Services;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Helpers
{
    public class HotTubBaseEx
    {
        public static bool SitDown(HotTubBase ths, Sim s, Slot slot, HotTubSeat seat, Sim.SwitchOutfitHelper switchOutfitHelper, bool isSkinnyDipping, Sim invitedBy, bool isAutonomous)
        {
            bool flag = isSkinnyDipping;
            bool paramValue = true;
            if ((s.GetCurrentOutfitCategoryFromOutfitInGameObject() == OutfitCategories.Swimwear) && !isSkinnyDipping)
            {
                paramValue = false;
            }

            if ((s.OccultManager.DisallowClothesChange() || (s.GetCurrentOutfitCategoryFromOutfitInGameObject() == OutfitCategories.Singed)) || s.BuffManager.DisallowClothesChange())
            {
                paramValue = false;
            }

            if ((s.Service != null) && (s.Service.ServiceType == ServiceType.GrimReaper))
            {
                paramValue = false;
            }

            if (s.GetCurrentOutfitCategoryFromOutfitInGameObject() == OutfitCategories.Naked)
            {
                flag = false;
            }
            else
            {
                s.RefreshCurrentOutfit(false);
            }

            if (!paramValue)
            {
                flag = false;
            }

            ths.PartComponent.SetSimAtPart(s, slot);
            switchOutfitHelper.Wait(true);
            StateMachineClient smc = StateMachineClient.Acquire(s, "HotTub", AnimationPriority.kAPDefault);
            if (smc == null)
            {
                s.AddExitReason(ExitReason.NullValueFound);
                ths.PartComponent.SetSimAtPart(null, slot);
                switchOutfitHelper.Dispose();
                return false;
            }

            smc.SetActor("x", s);
            smc.SetActor("hotTub", ths);
            smc.SetParameter("IkSuffix", seat.IKSuffix);
            smc.SetParameter("isMirrored", seat.IsMirrored);
            smc.SetParameter("doClothesSpin", paramValue);
            smc.SetParameter("playLookaround", isSkinnyDipping && ths.IsFirstSkinnyDipper());
            smc.AddOneShotScriptEventHandler(0x67, new SacsEventHandler(ths.TubEntered));
            smc.AddOneShotScriptEventHandler(0xc9, new SacsEventHandler(seat.SlotSimIntoHottub));
            smc.AddPersistentScriptEventHandler(0x69, new SacsEventHandler(seat.UnparentAndUpdateDrinkStatus));
            if (flag)
            {
                smc.AddOneShotScriptEventHandler(0x67, new SacsEventHandler(seat.CreateClothingPile));
            }

            smc.EnterState("x", "Enter");
            switchOutfitHelper.AddScriptEventHandler(smc);
            Glass actor = null;
            seat.DrinkRef = null;
            Glass.CarryingGlassPosture posture = s.Posture as Glass.CarryingGlassPosture;
            if (posture != null)
            {
                actor = posture.ObjectBeingCarried as Glass;
                CarrySystem.ExitCarry(s);
                actor.FadeOut(true);
                actor.UnParent();
                s.PopPosture();
                actor.ParentToSlot(ths, seat.DrinkSlot);
                actor.FadeIn();
                smc.SetActor("drink", actor);
                seat.DrinkRef = actor;
            }

            s.InteractionQueue.CancelAllInteractionsByType(Glass.Drink.Singleton);
            StereoCheap cheap = s.Inventory.Find<StereoCheap>();
            if (((cheap != null) && !ths.IsSlotOccupied(ths.BoomboxSlot)) && s.Inventory.TryToRemove(cheap))
            {
                cheap.SetOpacity(0f, 0f);
                if (cheap.ParentToSlot(ths, ths.BoomboxSlot))
                {
                    cheap.FadeIn(false);
                }
                else
                {
                    cheap.FadeIn(false, 0f);
                    s.Inventory.TryToAdd(cheap);
                }
            }

            smc.SetParameter("hasDrink", actor != null);
            smc.RequestState("x", "Sitting");
            HotTubPosture posture2 = new HotTubPosture(s, ths, smc, seat);
            posture2.InvitedBy = invitedBy;
            posture2.AutonomouslyChosen = isAutonomous;
            s.Posture = posture2;
            if (!s.Posture.Satisfies(CommodityKind.InHotTub, ths))
            {
                s.AddExitReason(ExitReason.FailedToStart);
                ths.PartComponent.SetSimAtPart(null, slot);
                seat.DrinkRef = null;
                return false;
            }

            EventTracker.SendEvent(new Event(EventTypeId.kGoHotTubbing, s, ths));
            if (isSkinnyDipping)
            {
                StartSkinnyDipBroadcastersAndSendWishEvents(ths, s);
            }
            return true;
        }

        public static void StartSkinnyDipBroadcastersAndSendWishEvents(HotTubBase ths, Sim skinnyDipper)
        {
            foreach (ulong num in ths.GetSkinnyDippers())
            {
                SimDescription description = SimDescription.Find(num);
                if (description != null)
                {
                    Sim createdSim = description.CreatedSim;
                    EventTracker.SendEvent(EventTypeId.kGoSkinnyDipping, createdSim);
                }
            }

            EventTracker.SendEvent(EventTypeId.kGoSkinnyDipping, skinnyDipper);
            EventTracker.SendEvent(new Event(EventTypeId.kGoSkinnyDipping, skinnyDipper, ths));

            if (ths.mSkinnyDipBroadcaster != null)
            {
                try
                {
                    ths.mSkinnyDipBroadcaster.ExecuteOnEnterCallbackOnSimsInRadius(skinnyDipper);
                }
                catch (Exception e)
                {
                    ths.mSkinnyDipBroadcaster.Dispose();
                    ths.mSkinnyDipBroadcaster = null;

                    Common.DebugException(ths, skinnyDipper, e);
                }
            }

            if (ths.mSkinnyDipBroadcaster == null)
            {
                ths.mSkinnyDipBroadcaster = new ReactionBroadcaster(ths, Pool.kSkinnyDipReactionParams, new EnterSkinnyDippingHotTubAreaProxy(ths).Perform);
            }

            // Custom
            if ((!Woohooer.Settings.mAllowTeenSkinnyDip) && (Woohooer.Settings.mEnforceSkinnyDipPrivacy))
            {
                if ((ths.mChildEnteredBroadcaster == null) && ths.IsOutside)
                {
                    ths.mChildEnteredBroadcaster = new ReactionBroadcaster(ths, Pool.kChildEnterReactionParams, new ChildEnterSkinnyDippingPoolAreaProxy(ths).Perform);
                }

                ths.CheckIfChildOnLot();

                if (!ths.mHasAddedLotCheckForChild)
                {
                    ths.mHasAddedLotCheckForChild = true;
                    Sim.sOnLotChangedDelegates -= ths.OnChildLotChanged;
                    Sim.sOnLotChangedDelegates += ths.OnChildLotChanged;
                }
            }
            else
            {
                if ((ths.mChildEnteredBroadcaster == null) && ths.IsOutside)
                {
                    ths.mChildEnteredBroadcaster = new ReactionBroadcaster(ths, Pool.kChildEnterReactionParams, OnStub);
                }

                ths.mHasAddedLotCheckForChild = true;

                Sim.sOnLotChangedDelegates -= ths.OnChildLotChanged;
            }
        }

        public static void OnStub(Sim sim, ReactionBroadcaster broadcaster)
        { }

        public class ChildEnterSkinnyDippingPoolAreaProxy
        {
            HotTubBase mHotTub;

            public ChildEnterSkinnyDippingPoolAreaProxy(HotTubBase hotTub)
            {
                mHotTub = hotTub;
            }

            public void Perform(Sim sim, ReactionBroadcaster broadcaster)
            {
                PoolEx.ReactIfSimIsTeenOrBelow(sim, mHotTub.GetSkinnyDippers());
            }
        }

        public class EnterSkinnyDippingHotTubAreaProxy
        {
            HotTubBase mHotTub;

            public EnterSkinnyDippingHotTubAreaProxy(HotTubBase hotTub)
            {
                mHotTub = hotTub;
            }

            public void Perform(Sim sim, ReactionBroadcaster broadcaster)
            {
                PoolEx.ReactToSkinnyDippers(sim, mHotTub, HotTubBase.GetIn.SkinnyDipSingleton, mHotTub.GetSkinnyDippers());
            }
        }
    }
}
