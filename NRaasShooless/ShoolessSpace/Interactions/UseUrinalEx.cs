using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.ShoolessSpace.Interactions
{
    public class UseUrinalEx : Urinal.UseUrinal, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Urinal, Urinal.UseUrinal.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Urinal, Urinal.UseUrinal.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.UrinalDesigner, Urinal.UseUrinal.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.UrinalCheap, Urinal.UseUrinal.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                BuffInstance element = Actor.BuffManager.GetElement(BuffNames.ReallyHasToPee);
                if ((element != null) && (element.mTimeoutCount <= Urinal.kTimeoutRemainingForBladderEmergency))
                {
                    RequestWalkStyle(Sim.WalkStyle.Run);
                }
                if (!Target.Line.WaitForTurn(this, SimQueue.WaitBehavior.DefaultAllowSubstitution, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), Urinal.kTimeToWaitInLine))
                {
                    return false;
                }
                if (!Target.RouteToUrinalAndCheckInUse(Actor))
                {
                    return false;
                }
                ClearRequestedWalkStyles();

                if (Shooless.Settings.GetPrivacy(Target))
                {
                    mSituation = Urinal.UrinalSituation.Create(Actor, Actor.LotCurrent);
                }

                if (mSituation != null)
                {
                    if (!mSituation.Start())
                    {
                        return false;
                    }
                    if (!Target.RouteToUrinalAndCheckInUse(Actor))
                    {
                        return false;
                    }
                }
                CancellableByPlayer = false;
                StandardEntry();
                mCurrentStateMachine = Target.GetStateMachine(Actor);
                Glass.CarryingGlassPosture posture = Actor.Posture as Glass.CarryingGlassPosture;
                if (posture != null)
                {
                    mDrinkInHand = posture.ObjectBeingCarried as Glass;
                    CarrySystem.ExitCarry(Actor);
                    mDrinkInHand.FadeOut(true);
                    mDrinkInHand.UnParent();
                    Actor.PopPosture();
                    SetParameter("hasDrink", true);
                    SetActor("drink", mDrinkInHand);
                    if (Target.HasDrinkSlot && (Target.GetContainedObject(Slot.ContainmentSlot_0) == null))
                    {
                        mDrinkInHand.ParentToSlot(Target, Slot.ContainmentSlot_0);
                        mDrinkInHand.FadeIn();
                    }
                }
                mCensorEnabled = true;
                Actor.EnableCensor(Sim.CensorType.LowerBody);
                AddOneShotScriptEventHandler(0x78, OnAnimationEvent);
                AnimateSim("use");
                if (element != null)
                {
                    element.mTimeoutPaused = true;
                }
                if (Actor.HasTrait(TraitNames.Inappropriate))
                {
                    mWillFart = RandomUtil.RandomChance01(Urinal.kChanceInappropriateFart);
                    if (mWillFart)
                    {
                        mFartTime = RandomUtil.RandomFloatGaussianDistribution(0.1f, 0.9f);
                    }
                }
                BeginCommodityUpdate(CommodityKind.Bladder, 0f);
                BeginCommodityUpdates();
                bool succeeded = false;

                try
                {
                    Actor.Motives.LerpToFill(this, CommodityKind.Bladder, Urinal.kMaxLengthUseToilet);
                    StartStages();

                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.BuffFailureState | ExitReason.MaxSkillPointsReached | ExitReason.HigherPriorityNext), new Interaction<Sim, Urinal>.InsideLoopFunction(LoopFunc), mCurrentStateMachine);
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

                if (Target.IsCleanable)
                {
                    Target.Cleanable.DirtyInc(Actor);
                }

                bool flag2 = Target.Line.MemberCount() > 0x1;
                InteractionInstance instance = null;
                if ((mSituation == null) || !mSituation.SomeoneDidIntrude)
                {
                    if (Target.AutoFlushes)
                    {
                        Target.FlushToilet(Actor, mCurrentStateMachine, false);
                    }
                    else
                    {
                        Target.ToiletVolume++;
                        if (Target.ShouldFlush(Actor, Autonomous))
                        {
                            Target.FlushToilet(Actor, mCurrentStateMachine, true);
                        }
                    }
                    if (((mDrinkInHand == null) && Urinal.ShouldWashHands(Actor)) && !flag2)
                    {
                        Sink target = Toilet.FindClosestSink(Actor);
                        if (target != null)
                        {
                            instance = Sink.WashHands.Singleton.CreateInstance(target, Actor, GetPriority(), false, true);
                        }
                    }
                }
                AddOneShotScriptEventHandler(0x68, OnAnimationEvent);
                AddOneShotScriptEventHandler(0x64, OnAnimationEvent);
                AnimateSim("exit");
                if (mSituation != null)
                {
                    mSituation.ExitUrinalSituation();
                }
                if (mDrinkInHand != null)
                {
                    CarrySystem.EnterWhileHolding(Actor, mDrinkInHand);
                    Actor.Posture = new Glass.CarryingGlassPosture(Actor, mDrinkInHand);
                    mDrinkInHand.FadeIn();
                }
                if (flag2)
                {
                    PrivacySituation.RouteToAdjacentRoom(Actor);
                }
                StandardExit();
                if (instance != null)
                {
                    Actor.InteractionQueue.PushAsContinuation(instance, true);
                }
                if (!flag2 && (instance == null))
                {
                    Actor.RouteAway(Urinal.kMinDistanceToMoveAwayAfterUsingUrinal, Urinal.kMaxDistanceToMoveAwayAfterUsingUrinal, false, GetPriority(), true, true, true, RouteDistancePreference.NoPreference);
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

        public new class Definition : Urinal.UseUrinal.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new UseUrinalEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Urinal target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}


