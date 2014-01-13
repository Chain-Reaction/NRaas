using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Situations;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.ShoolessSpace.Interactions
{
    public class UseUberToiletEx : UberToilet.UseToilet, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<UberToilet>(PrivacyInteraction.Singleton);
            interactions.Replace<UberToilet, UberToilet.UseToilet.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<UberToilet, UberToilet.UseToilet.Definition, Definition>(true);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                BuffInstance element = Actor.BuffManager.GetElement(BuffNames.ReallyHasToPee);
                if ((element != null) && (element.mTimeoutCount <= UberToilet.kTimeoutRemainingForBladderEmergency))
                {
                    RequestWalkStyle(Sim.WalkStyle.Run);
                }
                else if (!Target.Line.WaitForTurn(this, SimQueue.WaitBehavior.DefaultAllowSubstitution, ~(ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), UberToilet.kTimeToWaitInLine))
                {
                    return false;
                }
                else if (!Actor.RouteToSlotAndCheckInUse(Target, Slot.RoutingSlot_0))
                {
                    return false;
                }

                ClearRequestedWalkStyles();

                // Custom
                if (Shooless.Settings.GetPrivacy(Target))
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
                StateMachineClient smc = StateMachineClient.Acquire(Actor, "ubertoilet_store");
                smc.AddInterest<TraitNames>(TraitNames.HotHeaded);
                smc.SetActor("x", Actor);
                smc.SetActor("ubertoilet", Target);
                smc.EnterState("x", "Enter");
                smc.SetParameter("isDirty", Target.Cleanable.NeedsToBeCleaned);
                smc.SetParameter("isBroken", Target.Repairable.Broken);
                mCensorEnabled = true;
                Actor.EnableCensor(Sim.CensorType.LowerBody);
                smc.AddOneShotScriptEventHandler(0x78, TurnOffCensorGrid);
                smc.AddOneShotScriptEventHandler(0xc8, TriggerTrapCallback);
                if (element != null)
                {
                    element.mTimeoutPaused = true;
                }

                bool flag = false;
                if (ShouldSit(Actor))
                {
                    flag = true;
                    Target.PutDownSeat(smc);
                    smc.RequestState("x", "peeSitting");
                    if ((Target.SculptureComponent != null) && (Target.SculptureComponent.Material == SculptureComponent.SculptureMaterial.Ice))
                    {
                        Actor.BuffManager.AddElement(BuffNames.Chilly, Origin.FromSittingOnIce);
                    }
                    if (Target.ToiletTuning.AutoFlushes && RandomUtil.RandomChance((float)UberToilet.kChanceOfToiletAutoFlushWhileInUse))
                    {
                        smc.RequestState("x", "flushReaction");
                    }
                }
                else
                {
                    Target.PutUpSeat(smc);
                    smc.RequestState("x", "peeStanding");
                }

                if (!Target.Repairable.Broken && (Target.mToiletOnStatus == UberToilet.ToiletOnStatus.Auto))
                {
                    Target.StartMusic();
                }

                BeginCommodityUpdate(CommodityKind.Bladder, 0f);
                BeginCommodityUpdates();
                Actor.Motives.LerpToFill(this, CommodityKind.Bladder, UberToilet.kMaxLengthUseToilet);
                StartStages();
                OccultImaginaryFriend.GrantMilestoneBuff(Actor, BuffNames.ImaginaryFriendFeelOfPorcelain, Origin.FromImaginaryFriendFirstTime, true, true, false);
                bool succeeded = DoLoop(~(ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.BuffFailureState | ExitReason.MaxSkillPointsReached | ExitReason.HigherPriorityNext));
                Actor.BuffManager.UnpauseBuff(BuffNames.ImaginaryFriendFeelOfPorcelain);
                EndCommodityUpdates(succeeded);
                if (succeeded)
                {
                    Actor.Motives.GetMotive(CommodityKind.Bladder).PotionBladderDecayOverride = false;
                }

                if (element != null)
                {
                    element.mTimeoutPaused = false;
                }

                Target.Cleanable.DirtyInc(Actor);
                if (Target.ShouldPutDownSeat(Actor))
                {
                    Target.PutDownSeat(smc);
                }

                InteractionInstance instance = null;
                bool flag3 = Target.Line.MemberCount() > 0x1;
                if (Target.Repairable.Broken)
                {
                    if (flag)
                    {
                        smc.RequestState("x", "electricBreakSitting");
                    }
                    else
                    {
                        smc.RequestState("x", "electricBreakStanding");
                    }
                }

                if ((mSituation == null) || !mSituation.SomeoneDidIntrude)
                {
                    if (!Target.ToiletTuning.AutoFlushes)
                    {
                        Target.ToiletVolume++;
                        if (Target.ShouldFlush(Actor, Autonomous))
                        {
                            Target.FlushToilet(Actor, smc, true);
                        }
                    }

                    if (Target.ShouldWashHands(Actor) && !flag3)
                    {
                        Sink target = Toilet.FindClosestSink(Actor);
                        if (target != null)
                        {
                            instance = Sink.WashHands.Singleton.CreateInstance(target, Actor, GetPriority(), false, true);
                        }
                    }
                }

                if (Target.mToiletOnStatus == UberToilet.ToiletOnStatus.Auto)
                {
                    Target.StopMusic();
                }

                if (!Target.Repairable.Broken)
                {
                    if (Target.Upgradable.SelfCleaning && Target.mSelfCleanOn)
                    {
                        smc.RequestState("x", "autoClean");
                        Target.Cleanable.ForceClean();
                        Actor.BuffManager.AddElement(BuffNames.Relaxed, Origin.None);
                        Actor.Motives.ChangeValue(CommodityKind.Hygiene, UberToilet.kChanceOfRelaxed);
                    }
                    else if (RandomUtil.RandomChance(UberToilet.kChanceOfRelaxed))
                    {
                        Actor.BuffManager.AddElement(BuffNames.Relaxed, Origin.None);
                    }
                }

                smc.RequestState("x", "Exit");
                if (mSituation != null)
                {
                    mSituation.ExitToiletSituation();
                }

                if (flag3)
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

        public new class Definition : UberToilet.UseToilet.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new UseUberToiletEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, UberToilet target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}


