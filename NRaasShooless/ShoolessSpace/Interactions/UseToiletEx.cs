using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.ShoolessSpace.Interactions
{
    public class UseToiletEx : Toilet.UseToilet, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Toilet, Toilet.UseToilet.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Toilet, Toilet.UseToilet.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                BuffInstance element = Actor.BuffManager.GetElement(BuffNames.ReallyHasToPee);
                if ((element != null) && (element.mTimeoutCount <= Toilet.kTimeoutRemainingForBladderEmergency))
                {
                    RequestWalkStyle(Sim.WalkStyle.Run);
                }
                if (!Target.Line.WaitForTurn(this, SimQueue.WaitBehavior.DefaultAllowSubstitution, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), Toilet.kTimeToWaitInLine))
                {
                    return false;
                }

                if (CanPerformShooPet)
                {
                    if (!Actor.SimRoutingComponent.PreRouteCheckForLine(Target))
                    {
                        return false;
                    }
                    Sims3.SimIFace.Route r = Actor.CreateRoute();
                    r.PlanToSlot(Target, Slot.RoutingSlot_0);
                    r.RegisterCallback(new RouteCallback(SlotInUse), RouteCallbackType.TriggerOnTrue, RouteCallbackConditions.OnEventType(RouteEvent.tEventType.EventDestinationObstructed));
                    if (!Actor.DoRoute(r))
                    {
                        return false;
                    }
                    if (Target.InUse)
                    {
                        Actor.AddExitReason(ExitReason.ObjectInUse);
                        return false;
                    }
                }
                else if (!Actor.RouteToSlotAndCheckInUse(Target, Slot.RoutingSlot_0))
                {
                    return false;
                }
                ClearRequestedWalkStyles();

                if ((!(Target is ToiletStall)) && (Shooless.Settings.GetPrivacy(Target)))
                {
                    mSituation = Toilet.ToiletSituation.Create(Actor, Actor.LotCurrent);
                }

                if (mSituation != null)
                {
                    if (!mSituation.Start())
                    {
                        return false;
                    }
                    if (!Actor.RouteToSlotAndCheckInUse(Target, Slot.RoutingSlot_0))
                    {
                        mSituation.ExitToiletSituation();
                        return false;
                    }
                }
     
                CancellableByPlayer = false;
                StandardEntry();
                StateMachineClient stateMachine = Target.GetStateMachine(Actor);
                stateMachine.SetParameter("isDirty", Target.Cleanable.NeedsToBeCleaned);
                mCensorEnabled = true;

                OutfitCategories previousCategory = Actor.CurrentOutfitCategory;
                int previousIndex = Actor.CurrentOutfitIndex;

                bool switchOutfit = false;

                Actor.EnableCensor(Sim.CensorType.LowerBody);

                stateMachine.AddOneShotScriptEventHandler(120, TurnOffCensorGrid);
                stateMachine.AddOneShotScriptEventHandler(200, TriggerTrapCallback);
                if (element != null)
                {
                    element.mTimeoutPaused = true;
                }

                if (ShouldSit(Actor))
                {
                    if (Shooless.Settings.mNakedToilet)
                    {
                        Actor.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToBathe);
                        switchOutfit = true;
                    }

                    Target.PutDownSeat(stateMachine);
                    stateMachine.RequestState("x", "peeSitting");
                    if ((Target.SculptureComponent != null) && (Target.SculptureComponent.Material == SculptureComponent.SculptureMaterial.Ice))
                    {
                        Actor.BuffManager.AddElement(BuffNames.Chilly, Origin.FromSittingOnIce);
                    }
                    if (Target.ToiletTuning.AutoFlushes && RandomUtil.RandomChance((float)Toilet.kChanceOfToiletAutoFlushWhileInUse))
                    {
                        stateMachine.RequestState("x", "flushReaction");
                    }
                }
                else
                {
                    Target.PutUpSeat(stateMachine);
                    stateMachine.RequestState("x", "peeStanding");
                }
                BeginCommodityUpdate(CommodityKind.Bladder, 0f);
                BeginCommodityUpdates();

                bool succeeded = false;

                try
                {
                    Actor.Motives.LerpToFill(this, CommodityKind.Bladder, Toilet.kMaxLengthUseToilet);
                    StartStages();
                    OccultImaginaryFriend.GrantMilestoneBuff(Actor, BuffNames.ImaginaryFriendFeelOfPorcelain, Origin.FromImaginaryFriendFirstTime, true, true, false);
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.BuffFailureState | ExitReason.MaxSkillPointsReached | ExitReason.HigherPriorityNext));
                    Actor.BuffManager.UnpauseBuff(BuffNames.ImaginaryFriendFeelOfPorcelain);
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }

                if (succeeded)
                {
                    Motive motive = Actor.Motives.GetMotive(CommodityKind.Bladder);
                    if (motive != null)
                    {
                        motive.PotionBladderDecayOverride = false;
                    }
                }

                if (element != null)
                {
                    element.mTimeoutPaused = false;
                }
                Target.Cleanable.DirtyInc(Actor);

                if (Target.ShouldPutDownSeat(Actor))
                {
                    Target.PutDownSeat(stateMachine);
                }

                InteractionInstance instance = null;
                bool flag2 = Target.Line.MemberCount() > 0x1;
                if ((mSituation == null) || !mSituation.SomeoneDidIntrude)
                {
                    if (Target.ToiletTuning.AutoFlushes)
                    {
                        Target.FlushToilet(Actor, stateMachine, false);
                    }
                    else
                    {
                        Target.ToiletVolume++;
                        if (Target.ShouldFlush(Actor, Autonomous))
                        {
                            Target.FlushToilet(Actor, stateMachine, true);
                        }
                    }

                    if (Target.ShouldWashHands(Actor) && !flag2)
                    {
                        Sink target = Toilet.FindClosestSink(Actor);
                        if (target != null)
                        {
                            instance = Sink.WashHands.Singleton.CreateInstance(target, Actor, GetPriority(), false, true);
                        }
                    }
                }

                stateMachine.RequestState("x", "Exit");
                if (mSituation != null)
                {
                    mSituation.ExitToiletSituation();
                }

                if (switchOutfit)
                {
                    Actor.SwitchToOutfitWithSpin(previousCategory, previousIndex);
                }

                if (flag2)
                {
                    PrivacySituation.RouteToAdjacentRoom(Actor);
                }

                StandardExit();
                if (instance != null)
                {
                    Actor.InteractionQueue.PushAsContinuation(instance, false);
                }

                return succeeded;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : Toilet.UseToilet.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new UseToiletEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Toilet target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}


