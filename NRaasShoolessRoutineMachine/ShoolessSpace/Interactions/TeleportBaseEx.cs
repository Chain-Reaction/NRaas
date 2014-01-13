using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.ShoolessSpace.Interactions
{
    public class TeleportBaseEx
    {
        public delegate bool DoTeleport(IGameObject gameObject);

        public static bool Run(RoutineMachine.TeleportBase ths)
        {
            IGameObject obj2;
            if (!ths.Actor.CaregiverCheck())
            {
                return false;
            }
            else if ((ths.Actor.CarryingChildPosture != null) || (ths.Actor.CarryingPetPosture != null))
            {
                return false;
            }

            bool flag = ths.PreTeleport(out obj2, out ths.mTargetLot);
            if (!flag)
            {
                return false;
            }

            ths.OutfitHelper.Start();
            if (ths.Target.Repairable.Broken || ths.Target.Charred)
            {
                if (ths.Actor.RouteToSlotAndCheckInUse(ths.Target, Slot.RoutingSlot_0))
                {
                    ths.StandardEntry();
                    ths.EnterStateMachine("routinemachine_store", "EnterGetReady", "x", "routinemachine");
                    ths.mCurrentStateMachine.SetParameter("tired", SimClock.IsTimeBetweenTimes(RoutineMachine.TeleportBase.kSleepyTimeStart, RoutineMachine.TeleportBase.kSleepyTimeEnd));
                    ths.AnimateSim("GetInMachine");
                    ths.Target.StartPortalTeleportVisualEffects(ths.Actor);
                    ths.AnimateSim("DropThroughPortal");
                    ths.BeginCommodityUpdates();
                    ths.AnimateSim("InsideDreamscape");
                    ths.Target.Repairable.UpdateBreakage(0x64, ths.Actor);
                    if (ths.Actor.SimDescription.IsFrankenstein)
                    {
                        ths.Actor.BuffManager.AddElement(BuffNames.Energized, Origin.FromElectricity);
                    }
                    else if (ths.Actor.BuffManager.HasElement(BuffNames.SingedElectricity) || ths.Actor.TraitManager.HasElement(TraitNames.Unlucky))
                    {
                        ths.Actor.Kill(SimDescription.DeathType.Electrocution, ths.Target);
                    }
                    else
                    {
                        BuffSinged.SingeViaInteraction(ths, Origin.None);
                        ths.Actor.BuffManager.AddElement(BuffNames.SingedElectricity, Origin.None);
                    }

                    FireManager.SimShockedBy(ths.Actor, ths.Target);
                    ths.AnimateSim("ReturnFromDreamscape");
                    ths.AnimateSim("ExitDreamscape");
                    ths.EndCommodityUpdates(false);
                    ths.StandardExit();
                }
                flag = false;
            }

            if (flag && ths.Actor.RouteToSlotAndCheckInUse(ths.Target, Slot.RoutingSlot_0))
            {
                OutfitCategories categories2;
                bool flag2 = false;

                // Custom
                if (Shooless.Settings.GetPrivacy(ths.Target))
                {
                    ths.mSituation = new RoutineMachine.ShowerPrivacySituation(ths);
                    if (!ths.mSituation.Start())
                    {
                        return false;
                    }
                }

                if (!ths.Actor.RouteToSlotAndCheckInUse(ths.Target, Slot.RoutingSlot_0))
                {
                    if (ths.mSituation != null)
                    {
                        ths.mSituation.Exit();
                        ths.mSituation = null;
                    }
                    return false;
                }

                bool paramValue = false;
                ths.CancellableByPlayer = false;
                ths.StandardEntry();
                ths.EnterStateMachine("routinemachine_store", "EnterGetReady", "x", "routinemachine");
                ths.mCurrentStateMachine.SetParameter("tired", SimClock.IsTimeBetweenTimes(RoutineMachine.TeleportBase.kSleepyTimeStart, RoutineMachine.TeleportBase.kSleepyTimeEnd));

                if (ths.mSituation != null)
                {
                    ths.mSituation.StateMachine = ths.mCurrentStateMachine;
                }

                ths.AnimateSim("GetInMachine");

                if ((ths.Actor.Motives.HasMotive(CommodityKind.Hunger) && (ths.Actor.Motives.GetValue(CommodityKind.Hunger) < ths.Actor.Motives.GetMax(CommodityKind.Hunger))) && (ths.Actor.Motives.GetValue(CommodityKind.Hunger) < RoutineMachine.TeleportBase.kThresholdHunger))
                {
                    if (ths.Target.Upgradable.CurrentUpgrade == Upgrade.SoupUpSpeakers)
                    {
                        ths.Target.StartSpeakerEffects();
                    }
                    if (!paramValue)
                    {
                        paramValue = true;
                        ths.AnimateSim("InAirIdle");
                    }
                    ths.AnimateSim("SatisfyHunger");
                    ths.Actor.Motives.LerpToFill(ths, CommodityKind.Hunger, RoutineMachine.TeleportBase.kChangeHungerTime);
                    ths.Actor.ClearExitReasons();
                    ths.DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new InteractionInstance.InsideLoopFunction(ths.SatisfyHungerDelegate), ths.mCurrentStateMachine);
                    ths.Actor.ClearExitReasons();
                    ths.AnimateSim("InAirIdle");
                }

                if ((ths.Actor.Motives.HasMotive(CommodityKind.Bladder) && (ths.Actor.Motives.GetValue(CommodityKind.Bladder) < ths.Actor.Motives.GetMax(CommodityKind.Bladder))) && (ths.Actor.Motives.GetValue(CommodityKind.Bladder) < RoutineMachine.TeleportBase.kThresholdBladder))
                {
                    if (ths.Target.Upgradable.CurrentUpgrade == Upgrade.SoupUpSpeakers)
                    {
                        ths.Target.StartSpeakerEffects();
                    }
                    if (!paramValue)
                    {
                        paramValue = true;
                        ths.AnimateSim("InAirIdle");
                    }
                    ths.Actor.EnableCensor(Sim.CensorType.LowerBody);
                    ths.AnimateSim("SatisfyBladder");
                    ths.Actor.Motives.LerpToFill(ths, CommodityKind.Bladder, RoutineMachine.TeleportBase.kChangeBladderTime);
                    ths.Actor.ClearExitReasons();
                    ths.DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new InteractionInstance.InsideLoopFunction(ths.SatisfyBladderDelegate), ths.mCurrentStateMachine);
                    ths.Actor.ClearExitReasons();
                    ths.AnimateSim("InAirIdle");
                }

                if ((ths.Actor.Motives.HasMotive(CommodityKind.Hygiene) && (ths.Actor.Motives.GetValue(CommodityKind.Hygiene) < ths.Actor.Motives.GetMax(CommodityKind.Hygiene))) && (ths.Actor.Motives.GetValue(CommodityKind.Hygiene) < RoutineMachine.TeleportBase.kThresholdHygiene))
                {
                    if (ths.Target.Upgradable.CurrentUpgrade == Upgrade.SoupUpSpeakers)
                    {
                        ths.Target.StartSpeakerEffects();
                    }

                    if (!paramValue)
                    {
                        paramValue = true;
                        ths.AnimateSim("InAirIdle");
                    }
                    ths.AddSynchronousOneShotScriptEventHandler(0x3e9, ths.ShowerStartEvent);
                    ths.AddSynchronousOneShotScriptEventHandler(0x3ea, ths.ShowerEndEvent);
                    OutfitCategories naked = OutfitCategories.Naked;
                    if (ths.Actor.TraitManager.HasElement(TraitNames.NeverNude))
                    {
                        naked = OutfitCategories.Swimwear;
                    }
                    else
                    {
                        ths.Actor.EnableCensor(Sim.CensorType.FullBody);
                    }

                    using (Sim.SwitchOutfitHelper helper = new Sim.SwitchOutfitHelper(ths.Actor, Sim.ClothesChangeReason.GoingToBathe, naked, false))
                    {
                        helper.Start();
                        helper.Wait(false);
                        helper.ChangeOutfit();
                    }

                    flag2 = true;
                    ths.AnimateSim("SatisfyHygenie");
                    ths.Actor.Motives.LerpToFill(ths, CommodityKind.Hygiene, RoutineMachine.TeleportBase.kChangeHygieneTime);
                    ths.Actor.ClearExitReasons();
                    ths.DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new InteractionInstance.InsideLoopFunction(ths.SatisfyHygieneDelegate), ths.mCurrentStateMachine);
                    ths.Actor.ClearExitReasons();
                    ths.AnimateSim("InAirIdle");
                    BuffManager buffManager = ths.Actor.BuffManager;
                    buffManager.RemoveElement(BuffNames.Singed);
                    buffManager.RemoveElement(BuffNames.SingedElectricity);
                    buffManager.RemoveElement(BuffNames.GarlicBreath);
                    if (ths.Actor.SimDescription.IsMummy)
                    {
                        buffManager.AddElement(BuffNames.Soaked, Origin.FromShower);
                    }
                    if (ths.Actor.HasTrait(TraitNames.Hydrophobic))
                    {
                        ths.Actor.PlayReaction(ReactionTypes.Cry, ths.Target, ReactionSpeed.AfterInteraction);
                    }
                }

                if (ths.Actor.GetOutfitForClothingChange(ths.NewClothesStyle, out categories2) || flag2)
                {
                    ths.mCurrentStateMachine.SetParameter("inAirIdle", paramValue);
                    ths.AnimateSim("PrepareToChangeClothes");
                    using (Sim.SwitchOutfitHelper helper2 = new Sim.SwitchOutfitHelper(ths.Actor, Sim.ClothesChangeReason.Force, categories2))
                    {
                        helper2.Start();
                        helper2.Wait(false);
                        helper2.ChangeOutfit();
                    }
                }

                ths.Actor.AutoEnableCensor();
                ths.AddSynchronousOneShotScriptEventHandler(0x64, ths.TeleportOutEvent);
                ths.AnimateSim("DropThroughPortal");
                ths.Target.StopSpeakerEffects();
                if (ths.mSituation != null)
                {
                    ths.mSituation.Exit();
                    ths.mSituation = null;
                }

                if (ths.Actor.Motives.GetValue(CommodityKind.Energy) < RoutineMachine.TeleportBase.kMinEnergyToTeleport)
                {
                    ths.Target.StartDreamscapeEffects();
                    ths.Target.Repairable.UpdateBreakage(0x64, ths.Actor);
                    ths.RunDreamscape();
                    ths.Target.StopDreamscapeEffects();
                }

                flag = ths.DoTeleport(obj2);
                ths.AnimateSim("ExitGetReady");
                ths.EnterStateMachine("routinemachine_store", "EnterBeTeleported", "x", "routinemachine");
                ths.AddSynchronousOneShotScriptEventHandler(0x64, ths.TeleportBackEvent);
                ths.AnimateSim("BeTeleported");
                ths.AnimateSim("ExitBeTeleported");
                ths.StandardExit();
            }

            ths.Target.StopPortalReturnVisualEffects();
            if (ths.mSituation != null)
            {
                ths.mSituation.Exit();
            }
            return flag;
        }




    }
}


