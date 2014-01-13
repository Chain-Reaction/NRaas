using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class BedWoohoo : Sims3.Gameplay.Objects.Beds.WooHoo, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeWoohootySimSingleton = new SafeDefinition("WooHooty Call");

        static readonly InteractionDefinition SafeSimSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySimSingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySimSingleton = new TryForBabyDefinition();

        static readonly InteractionDefinition SafeBedSingleton = new SafeBedDefinition();
        static readonly InteractionDefinition RiskyBedSingleton = new RiskyBedDefinition();
        static readonly InteractionDefinition TryForBabyBedSingleton = new TryForBabyBedDefinition();

        public static ReactionBroadcasterParams sBroadcastParams = null;

        public BedWoohoo()
        {
            if (sBroadcastParams == null)
            {
                sBroadcastParams = Conversation.ReactToSocialParams.Clone();
                sBroadcastParams.AffectBroadcasterRoomOnly = Woohooer.sWasAffectBroadcasterRoomOnly;
            }
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            //interactions.Add<Sim>(SafeSimSingleton);
            interactions.Add<Sim>(RiskySimSingleton);
            //interactions.Add<Sim>(TryForBabySimSingleton);

            interactions.Add<IBedDouble>(SafeBedSingleton);
            interactions.Add<IBedDouble>(RiskyBedSingleton);
            interactions.Add<IBedDouble>(TryForBabyBedSingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<Sim, WooHoo.Definition, ProxyDefinition>(false);

            Woohooer.InjectAndReset<Sim, WooHoo.Definition, SafeDefinition>(false);
            
            Woohooer.InjectAndReset<Sim, TryForBaby.Definition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<Sim, TryForBaby.Definition, TryForBabyDefinition>(false);

            Woohooer.InjectAndReset<Bed, SafeDefinition, SafeBedDefinition>(true);
            Woohooer.InjectAndReset<Bed, SafeDefinition, RiskyBedDefinition>(true);
            Woohooer.InjectAndReset<Bed, SafeDefinition, TryForBabyBedDefinition>(true);

            WooHoo.Singleton = SafeSimSingleton;
            TryForBaby.Singleton = TryForBabySimSingleton;
        }

        protected void OnPregnancyEvent(StateMachineClient sender, IEvent evt)
        {
            IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

            if (CommonPregnancy.IsSuccess(Actor, Target, Autonomous, definition.GetStyle(this)))
            {
                Pregnancy pregnancy = CommonPregnancy.Impregnate(Actor, Target, Autonomous, definition.GetStyle(this));
                if (pregnancy != null)
                {
                    if (Actor.Posture.Container is HeartShapedBed)
                    {
                        pregnancy.SetForcedBabyTrait(TraitNames.Excitable);
                    }
                    else if (Actor.Posture.Container is Igloo)
                    {
                        pregnancy.SetForcedBabyTrait(TraitNames.LovesTheCold);
                    }
                }
            }
        }

        private new void StartJealousyBroadcaster()
        {
            try
            {
                if (mReactToSocialBroadcaster == null)
                {
                    mReactToSocialBroadcaster = new ReactionBroadcaster(Actor, sBroadcastParams, SocialComponentEx.ReactToJealousEventHigh);
                    CommonWoohoo.CheckForWitnessedCheating(Actor, Target, !Rejected);
                }

                if (IsMaster)
                {
                    BedWoohoo linked = LinkedInteractionInstance as BedWoohoo;
                    if (linked != null)
                    {
                        linked.StartJealousyBroadcaster();
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
        }

        public static bool IsOwner(Sim sim, IBedDouble target)
        {
            if (target.BedOwners().Count > 0)
            {
                if (!target.BedOwners().Contains(sim)) return false;
            }

            return true;
        }

        protected bool CanSleep(Sim sim, IBedDouble target)
        {
            InteractionInstance nextInteraction = sim.InteractionQueue.GetNextInteraction();                            
            bool flag = nextInteraction != null;
            bool flag2 = (flag && (nextInteraction.PosturePreconditions != null)) && nextInteraction.PosturePreconditions.ContainsPosture(CommodityKind.Sleeping);

            if ((((mSituation != null) && mSituation.SomeoneDidIntrude) || (flag && !flag2)) || (target is BedDreamPod))
            {
                return false;
            }

            if (sim.LotHome == sim.LotCurrent)
            {
                if (!IsOwner(sim, target)) return false;
            }

            return BedSleep.CanSleep(sim, true);
        }

        public override bool Run()
        {
            try
            {
                if ((Woohooer.Settings.mHideWoohoo) &&
                    (Woohooer.Settings.mRiskyBabyMadeChanceV2[PersistedSettings.GetSpeciesIndex(Actor)] <= 0) &&
                    (Woohooer.Settings.mTryForBabyMadeChanceV2[PersistedSettings.GetSpeciesIndex(Actor)] <= 0))
                {
                    if (CommonPregnancy.Impregnate(Actor, Target, Autonomous, CommonWoohoo.WoohooStyle.TryForBaby) != null)
                    {
                        Woohooer.Notify(Common.LocalizeEAString("NRaas.Woohooer:BabyMade"), Actor.ObjectId, Target.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
                    }

                    return true;
                }

                Actor.GreetSimOnMyLotIfPossible(Target);
                if (StartBedCuddleA.GetCuddleType(Actor, Target) == StartBedCuddleA.CuddleType.CuddleTargetOnDifferentBed)
                {
                    ChildUtils.SetPosturePrecondition(this, CommodityKind.Relaxing, new CommodityKind[] { CommodityKind.NextToTarget });
                    Actor.InteractionQueue.PushAsContinuation(this, true);
                    return true;
                }

                BedMultiPart container = null;

                try
                {
                    if (Actor.Posture == null)
                    {
                        return false;
                    }

                    if (!Actor.Posture.Satisfies(CommodityKind.Relaxing, null))
                    {
                        return false;
                    }

                    if (!SafeToSync())
                    {
                        return false;
                    }

                    container = Actor.Posture.Container as BedMultiPart;
                    if (container == null)
                    {
                        return false;
                    }

                    if (IsMaster && (ReturnInstance == null))
                    {
                        EnterStateMachine("BedSocials", "FromRelax", "x", "y");
                        AddPersistentScriptEventHandler(0x0, EventCallbackChangeVisibility);
                        SetActor("bed", container);
                        container.PartComp.GetPartSimIsIn(Actor).SetPartParameters(mCurrentStateMachine);
                        WooHoo interaction = InteractionDefinition.CreateInstance(Actor, Target, GetPriority(), false, CancellableByPlayer) as WooHoo;
                        interaction.IsMaster = false;
                        interaction.LinkedInteractionInstance = this;
                        ChildUtils.SetPosturePrecondition(interaction, CommodityKind.Relaxing, new CommodityKind[] { CommodityKind.NextToTarget });
                        Target.InteractionQueue.AddNext(interaction);
                        if (Target.Posture.Container != Actor.Posture.Container)
                        {
                            Actor.LookAtManager.SetInteractionLookAt(Target, 0xc8, LookAtJointFilter.TorsoBones);
                            Actor.Posture.CurrentStateMachine.RequestState("x", "callOver");
                        }
                        Actor.SynchronizationLevel = Sim.SyncLevel.Started;
                        Actor.SynchronizationTarget = Target;
                        Actor.SynchronizationRole = Sim.SyncRole.Initiator;
                        if (!StartSync(IsMaster))
                        {
                            return false;
                        }
                        if (!Actor.WaitForSynchronizationLevelWithSim(Target, Sim.SyncLevel.Routed, 30f))
                        {
                            return false;
                        }

                        // StartSocial tanks social motives on inactives
                        //StartSocialContext();
                        Actor.SocialComponent.StartSocializingWith(Target);
                    }
                    else if (!StartSync(IsMaster))
                    {
                        return false;
                    }
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.DebugException(Actor, Target, e);
                    return false;
                }

                StandardEntry(false);
                BeginCommodityUpdates();

                bool succeeded = false;

                try
                {
                    if (IsMaster)
                    {
                        if (CommonWoohoo.NeedPrivacy(container.InherentlyProvidesPrivacy, Actor, Target))
                        {
                            mSituation = new WooHooPrivacySituation(this);
                            if (!mSituation.Start())
                            {
                                FinishLinkedInteraction();
                                PostLoop();
                                if (ReturnInstance == null)
                                {
                                    InteractionInstance instance = BedRelax.Singleton.CreateInstance(Actor.Posture.Container, Actor, GetPriority(), true, true);
                                    Actor.InteractionQueue.PushAsContinuation(instance, true);
                                }
                                else
                                {
                                    DoResume();
                                }

                                WooHoo linkedInteractionInstance = LinkedInteractionInstance as WooHoo;
                                if (linkedInteractionInstance != null)
                                {
                                    if (ReturnInstance == null)
                                    {
                                        InteractionInstance instance2 = BedRelax.Singleton.CreateInstance(Target.Posture.Container, Target, GetPriority(), true, true);
                                        Target.InteractionQueue.PushAsContinuation(instance2, true);
                                    }
                                    else
                                    {
                                        linkedInteractionInstance.DoResume();
                                    }
                                    linkedInteractionInstance.Failed = true;
                                }
                                WaitForSyncComplete();
                                return false;
                            }
                        }

                        IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                        Actor.LookAtManager.ClearInteractionLookAt();
                        Target.LookAtManager.ClearInteractionLookAt();
                        if (ReturnInstance != null)
                        {
                            ReturnInstance.EnsureMaster();
                            mCurrentStateMachine = ReturnInstance.mCurrentStateMachine;
                        }
                        StartSocial(CommonWoohoo.GetSocialName(definition.GetStyle(this), Actor));
                        InitiateSocialUI(Actor, Target);

                        WooHoo linked = LinkedInteractionInstance as WooHoo;
                        if (linked != null)
                        {
                            linked.Rejected = Rejected;
                        }

                        if (Rejected)
                        {
                            if (Actor.Posture.Container == Target.Posture.Container)
                            {
                                ThoughtBalloonManager.BalloonData bd = new ThoughtBalloonManager.DoubleBalloonData("balloon_woohoo", "balloon_question");
                                bd.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
                                Actor.ThoughtBalloonManager.ShowBalloon(bd);
                                AddOneShotScriptEventHandler(0x194, ShowRejectBalloonAndEnqueueRouteAway);
                                mCurrentStateMachine.RequestState(false, "x", "WooHooReject");
                                mCurrentStateMachine.RequestState(true, "y", "WooHooReject");
                                mCurrentStateMachine.RequestState(true, null, "ToRelax");
                            }
                        }
                        else
                        {
                            mCurrentStateMachine.AddOneShotScriptEventHandler(0x6e, OnPregnancyEvent);

                            mCurrentStateMachine.AddOneShotScriptEventHandler(0x6e, EventCallbackChangeClothes);
                            string wooHooEffectName = container.TuningBed.WooHooEffectName;
                            if (!string.IsNullOrEmpty(wooHooEffectName))
                            {
                                mWooHooEffect = VisualEffect.Create(wooHooEffectName);
                                mWooHooEffect.ParentTo(container, Slots.Hash("_FX_0"));
                                AddOneShotScriptEventHandler(0xc8, EventCallbackWooHoo);
                                AddOneShotScriptEventHandler(0xc9, EventCallbackWooHoo);
                            }

                            if (container is BedDreamPod)
                            {
                                AddOneShotScriptEventHandler(0xc8, EventCallbackDreamPodWooHoo);
                                AddOneShotScriptEventHandler(0xc9, EventCallbackDreamPodWooHoo);
                            }

                            Sim.ClothesChangeReason reason = Sim.ClothesChangeReason.GoingToBed;
                            if ((Woohooer.Settings.mNakedOutfitBed) && (!container.IsOutside))
                            {
                                reason = Sim.ClothesChangeReason.GoingToBathe;

                                Woohooer.Settings.AddChange(Actor);
                                Woohooer.Settings.AddChange(Target);
                            }

                            mHelperX = new Sim.SwitchOutfitHelper(Actor, reason);
                            mHelperY = new Sim.SwitchOutfitHelper(Target, reason);
                            mHelperX.Start();
                            mHelperY.Start();
                            mJealousyAlarm = AlarmManager.Global.AddAlarm(kJealousyBroadcasterDelay, TimeUnit.Minutes, StartJealousyBroadcaster, "StartJealousyBroadcaster", AlarmType.DeleteOnReset, container);
                            container.PreWooHooBehavior(Actor, Target, this);
                            mCurrentStateMachine.RequestState(false, "x", "WooHoo");
                            mCurrentStateMachine.RequestState(true, "y", "WooHoo");
                            container.PostWooHooBehavior(Actor, Target, this);
                            Relationship.Get(Actor, Target, true).STC.Update(Actor, Target, CommodityTypes.Amorous, kSTCIncreaseAfterWoohoo);

                            if (CanSleep(Actor, container))
                            {
                                SleepAfter = true;
                            }
                            else
                            {
                                SleepAfter = false;
                                container.PartComp.GetPartSimIsIn(Actor).BedMade = true;
                            }

                            if (CanSleep(Target, container))
                            {
                                (LinkedInteractionInstance as WooHoo).SleepAfter = true;
                            }
                            else
                            {
                                (LinkedInteractionInstance as WooHoo).SleepAfter = false;
                                container.PartComp.GetPartSimIsIn(Target).BedMade = true;
                            }

                            /*
                            if (SleepAfter)
                            {
                                mCurrentStateMachine.RequestState(null, "ToSleep");
                            }
                            else*/
                            {
                                mCurrentStateMachine.RequestState(null, "ToRelax");
                            }

                            CommonWoohoo.RunPostWoohoo(Actor, Target, container, definition.GetStyle(this), definition.GetLocation(container), true);

                            if (container is BedDoubleHover)
                            {
                                Actor.BuffManager.AddElement(BuffNames.MeterHighClub, Origin.FromWooHooOnHoverBed);
                                Target.BuffManager.AddElement(BuffNames.MeterHighClub, Origin.FromWooHooOnHoverBed);
                            }

                            if (container is BedDreamPod)
                            {
                                Actor.BuffManager.AddElement(BuffNames.DoubleDreaming, Origin.FromWooHooInDreamPod);
                                Target.BuffManager.AddElement(BuffNames.DoubleDreaming, Origin.FromWooHooInDreamPod);
                            }
                        }
                        FinishSocial(CommonWoohoo.GetSocialName(definition.GetStyle(this), Actor), true);
                        CleanupSituation();
                        Actor.AddExitReason(ExitReason.Finished);
                    }
                    else
                    {
                        container = Target.Posture.Container as BedMultiPart;
                        if (container == null)
                        {
                            return false;
                        }
                        PartComponent<BedData> partComp = container.PartComp;
                        if (partComp.GetSimInOtherPart(Target) == null)
                        {
                            int num;
                            BedData otherPart = partComp.GetOtherPart(partComp.GetPartSimIsIn(Target));
                            if (!Actor.RouteToSlotListAndCheckInUse(container, otherPart.RoutingSlot, out num))
                            {
                                Actor.AddExitReason(ExitReason.RouteFailed);
                                return false;
                            }

                            Actor.SynchronizationLevel = Sim.SyncLevel.Routed;
                            if (Rejected)
                            {
                                Actor.PlaySoloAnimation("a2a_bed_relax_cuddle_reject_standing_y", true);
                                Actor.RouteAway(kMinDistanceToMoveAwayWhenRejected, kMaxDistanceToMoveAwayWhenRejected, true, new InteractionPriority(InteractionPriorityLevel.Zero), false, true, true, RouteDistancePreference.NoPreference);
                                return true;
                            }
                            if (!otherPart.RelaxOnBed(Actor, "Enter_BedRelax_" + otherPart.StateNameSuffix))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            Actor.SynchronizationLevel = Sim.SyncLevel.Routed;
                        }

                        DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                        if (!Actor.HasExitReason(ExitReason.Finished))
                        {
                            PostLoop();
                            WaitForMasterInteractionToFinish();
                        }
                    }
                    PostLoop();
                    WaitForSyncComplete();

                    succeeded = !Failed && !Rejected;
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                    StandardExit(false, false);
                }

                if (succeeded)
                {
                    VisitSituation situation = VisitSituation.FindVisitSituationInvolvingGuest(Actor);
                    VisitSituation situation2 = VisitSituation.FindVisitSituationInvolvingGuest(Target);
                    if ((situation != null) && (situation2 != null))
                    {
                        situation.GuestStartingInappropriateAction(Actor, 3.5f);
                        situation2.GuestStartingInappropriateAction(Target, 3.5f);
                    }
                }

                if (succeeded && SleepAfter)
                {
                    //container.GetPartContaining(Actor).StateMachine = null;
                    if (!Actor.InteractionQueue.HasInteractionOfType(BedSleep.Singleton))
                    {
                        InteractionInstance instance = BedSleep.Singleton.CreateInstance(container, Actor, GetPriority(), Autonomous, CancellableByPlayer);
                        Actor.InteractionQueue.PushAsContinuation(instance, true);
                    }
                    if ((VisitSituation.FindVisitSituationInvolvingGuest(Target) != null) && Actor.IsAtHome)
                    {
                        SocialCallback.OnStayOver(Actor, Target, false);
                    }
                    else if ((VisitSituation.FindVisitSituationInvolvingGuest(Actor) != null) && Target.IsAtHome)
                    {
                        SocialCallback.OnStayOver(Target, Actor, false);
                    }
                }
                else if (!IsOwner(Actor, container))
                {
                    InteractionInstance instance = Actor.Posture.GetStandingTransition();
                    if (instance != null)
                    {
                        Actor.InteractionQueue.Add(instance);
                    }
                }

                return succeeded;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Woohooer.Settings.AddChange(Actor);
                Woohooer.Settings.AddChange(Target);

                Common.Exception(Actor, Target, exception);
                return false;
            }
        }

        public static bool CanCuddleOnBedOfSimA(Sim a, Sim b)
        {
            try
            {
                if (a.Posture == null) return false;

                BedMultiPart container = a.Posture.Container as BedMultiPart;
                if (container == null)
                {
                    return false;
                }
                Sim simInOtherPart = container.PartComp.GetSimInOtherPart(a);
                if ((simInOtherPart != null) && (simInOtherPart != b))
                {
                    return false;
                }
                if (!CanShareBed(a, b))
                {
                    return false;
                }
                if (simInOtherPart == null)
                {
                    return (container.UseCount == 1);
                }
                return (container.UseCount == 2);
            }
            catch(Exception e)
            {
                Common.Exception(a, b, e);
            }

            return false;
        }

        protected static bool CanShareBed(Sim newSim, Sim simUsingBed)
        {
            if (simUsingBed != null)
            {
                WooHoo runningInteraction = simUsingBed.InteractionQueue.RunningInteraction as WooHoo;
                if ((runningInteraction != null) && (newSim != runningInteraction.Target))
                {
                    return false;
                }
            }

            return true;
        }

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<Sim, BedWoohoo, BaseBedDefinition>
        {
            public ProxyDefinition(BaseBedDefinition definition)
                : base(definition)
            { }

            public override Sim ITarget(InteractionInstance interaction)
            {
                return interaction.Target as Sim;
            }
        }

        public abstract class BaseBedDefinition : CommonWoohoo.PotentialDefinition<IBedDouble>
        {
            public BaseBedDefinition()
            { }
            public BaseBedDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                if (obj is Tent)
                {
                    return CommonWoohoo.WoohooLocation.Tent;
                }
                else if (obj is Igloo)
                {
                    return CommonWoohoo.WoohooLocation.Igloo;
                }
                else
                {
                    return CommonWoohoo.WoohooLocation.Bed;
                }
            }

            public override Sim GetTarget(Sim actor, IBedDouble target, InteractionInstance interaction)
            {
                BedMultiPart bed = target as BedMultiPart;

                List<Sim> sims = new List<Sim>(bed.ActorsUsingMe);
                sims.Remove(actor);

                if (sims.Count == 1)
                {
                    return sims[0];
                }
                else
                {
                    return base.GetTarget(actor, target, interaction);
                }
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                IBedDouble bed = obj as IBedDouble;

                actor.GreetSimOnMyLotIfPossible(target);

                InteractionPriority userPriority = new InteractionPriority(InteractionPriorityLevel.UserDirected);
                InteractionInstance entry = EnterRelaxingEx.WoohooSingleton.CreateInstance(bed, actor, userPriority, false, true);
                if (actor.InteractionQueue.Add(entry))
                {
                    InteractionPriority autoPriority = new InteractionPriority(InteractionPriorityLevel.Autonomous);

                    InteractionInstance instance2 = EnterRelaxingEx.WoohooSingleton.CreateInstance(bed, target, autoPriority, false, true);
                    target.InteractionQueue.Add(instance2);

                    InteractionInstance instance3 = new ProxyDefinition(this).CreateInstance(target, actor, userPriority, false, true);
                    instance3.GroupId = entry.GroupId;
                    actor.InteractionQueue.Add(instance3);
                }
            }

            protected override bool Satisfies(Sim actor, Sim target, IBedDouble obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (actor.Posture == null) return false;

                if (actor.Posture.Satisfies(CommodityKind.Relaxing, obj)) return false;

                if (actor.CurrentInteraction is ISleeping) return false;

                if (target.Posture == null) return false;

                if (target.Posture.Satisfies(CommodityKind.Relaxing, obj)) return false;

                if (target.CurrentInteraction is ISleeping) return false;

                if (obj.UseCount > 0)
                {
                    if ((!obj.IsActorUsingMe(actor)) && (!obj.IsActorUsingMe(target)))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public class SafeBedDefinition : BaseBedDefinition
        {
            public SafeBedDefinition()
            { }
            public SafeBedDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.Safe;
            }

            public override string GetInteractionName(Sim actor, IBedDouble target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, IBedDouble obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                try
                {
                    if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                    return CommonWoohoo.SatisfiesWoohoo(actor, target, "BedWoohoo", isAutonomous, true, true, ref callback);
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Common.Exception(actor, target, exception);
                }
                return false;
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new SafeBedDefinition(target));
            }
        }

        public class RiskyBedDefinition : BaseBedDefinition
        {
            public RiskyBedDefinition()
            { }
            public RiskyBedDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return CommonWoohoo.WoohooStyle.Risky;
            }

            public override string GetInteractionName(Sim actor, IBedDouble target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim a, Sim target, IBedDouble obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonPregnancy.SatisfiesRisky(a, target, "BedRisky", isAutonomous, true, ref greyedOutTooltipCallback);
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                    return false;
                }
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new RiskyBedDefinition(target));
            }
        }

        public class TryForBabyBedDefinition : BaseBedDefinition
        {
            public TryForBabyBedDefinition()
            { }
            public TryForBabyBedDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return CommonWoohoo.WoohooStyle.TryForBaby;
            }

            public override string GetInteractionName(Sim actor, IBedDouble target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim a, Sim target, IBedDouble obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonPregnancy.SatisfiesTryForBaby(a, target, "BedTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                    return false;
                }
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new TryForBabyBedDefinition(target));
            }
        }    

        public abstract class BaseDefinition : CommonWoohoo.BaseDefinition<Sim, BedWoohoo>, IOverrideStartInteractionBehavior
        {
            public override Sim GetTarget(Sim actor, Sim target, InteractionInstance interaction)
            {
                return target;
            }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.Bed;
            }

            public override int Attempts
            {
                set {}
            }

            public void StartInteraction(Sim actor, IGameObject paramTarget)
            {
                try
                {
                    Sim target = paramTarget as Sim;
                    if ((actor.Conversation != null) && (actor.Conversation != target.Conversation))
                    {
                        actor.SocialComponent.LeaveConversation();
                    }
                }
                catch (Exception exception)
                {
                    Common.Exception(actor, paramTarget, exception);
                }
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (a == target)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("You cannot Woohoo with yourself!");
                        return false;
                    }

                    if (a.CurrentInteraction is ISleeping)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Actor Sleeping");
                        return false;
                    }

                    if (target.CurrentInteraction is ISleeping)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Target Sleeping");
                        return false;
                    }

                    if (!a.Posture.Satisfies(CommodityKind.Relaxing, target))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Actor Not Relaxing On Bed");
                        return false;
                    }

                    if (!isAutonomous && SocialComponent.IsTargetUnavailableForSocialInteraction(target, ref greyedOutTooltipCallback))
                    {
                        if (greyedOutTooltipCallback == null)
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Target Unavailable");
                        }
                        return false;
                    }

                    if (CanCuddleOnBedOfSimA(a, target))
                    {
                        return true;
                    }
                    else if (CanCuddleOnBedOfSimA(target, a))
                    {
                        return true;
                    }
                    else
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Cuddle Fail");
                        return false;
                    }
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                }
                return false;
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                throw new NotImplementedException();
            }
        }

        public class SafeDefinition : BaseDefinition
        {
            public string mSocialName;

            public SafeDefinition()
            {
                mSocialName = "WooHoo";
            }
            public SafeDefinition(string socialName)
            {
                mSocialName = socialName;
            }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.Safe;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonWoohoo.SatisfiesWoohoo(a, target, "BedWoohoo", isAutonomous, true, true, ref greyedOutTooltipCallback);
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                }
                return false;
            }
        }

        public class RiskyDefinition : BaseDefinition
        {
            public RiskyDefinition()
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return CommonWoohoo.WoohooStyle.Risky;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonPregnancy.SatisfiesRisky(a, target, "BedRisky", isAutonomous, true, ref greyedOutTooltipCallback);
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                    return false;
                }
            }
        }

        public class TryForBabyDefinition : BaseDefinition
        {
            public TryForBabyDefinition()
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return CommonWoohoo.WoohooStyle.TryForBaby;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonPregnancy.SatisfiesTryForBaby(a, target, "BedTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                    return false;
                }
            }
        }

        public class LocationControl : WoohooLocationControl
        {
            public override CommonWoohoo.WoohooLocation Location
            {
                get { return CommonWoohoo.WoohooLocation.Bed; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is BedDouble;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<BedDouble>(new Predicate<BedDouble>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<BedDouble>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                return Woohooer.Settings.mAutonomousBed;
            }

            protected bool TestUse(BedDouble obj)
            {
                if (!TestRepaired(obj)) return false;

                if (obj.UseCount > 0) return false;

                if (obj.NumberOfSpots() < 2) return false;

                return true;
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<BedDouble> objects = new List<BedDouble>();

                BedDouble bed = actor.Bed as BedDouble;
                if (bed != null)
                {
                    if ((bed.LotCurrent == actor.LotCurrent) && (TestUse(bed)))
                    {
                        objects.Add(bed);
                    }
                }

                bed = target.Bed as BedDouble;
                if (bed != null)
                {
                    if ((bed.LotCurrent == target.LotCurrent) && (TestUse(bed)))
                    {
                        objects.Add(bed);
                    }
                }

                if (objects.Count == 0)
                {
                    objects = actor.LotCurrent.GetObjects<BedDouble>(new Predicate<BedDouble>(TestUse));
                }

                List<GameObject> results = new List<GameObject>();

                foreach (GameObject obj in objects)
                {
                    if ((testFunc != null) && (!testFunc(obj, null))) continue;

                    results.Add(obj);
                }

                return results;
            }

            public override InteractionDefinition GetInteraction(Sim actor, Sim target, CommonWoohoo.WoohooStyle style)
            {
                switch (style)
                {
                    case CommonWoohoo.WoohooStyle.Safe:
                        return new SafeBedDefinition(target);
                    case CommonWoohoo.WoohooStyle.Risky:
                        return new RiskyBedDefinition(target);
                    case CommonWoohoo.WoohooStyle.TryForBaby:
                        return new TryForBabyBedDefinition(target);
                }

                return null;
            }
        }

        public class TentLocationControl : WoohooLocationControl
        {
            public override CommonWoohoo.WoohooLocation Location
            {
                get { return CommonWoohoo.WoohooLocation.Tent; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is Tent;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<Tent>(new Predicate<Tent>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<Tent>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP1)) return false;
                }

                return Woohooer.Settings.mAutonomousTent;
            }

            public bool TestUse(Tent obj)
            {
                if (!TestRepaired(obj)) return false;

                if (obj.UseCount > 0) return false;

                if (obj.NumberOfSpots() < 2) return false;

                if (!obj.Placed) return false;

                return true;
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (Tent obj in actor.LotCurrent.GetObjects<Tent>(new Predicate<Tent>(TestUse)))
                {
                    if ((testFunc != null) && (!testFunc(obj, null))) continue;

                    results.Add(obj);
                }

                return results;
            }

            public override InteractionDefinition GetInteraction(Sim actor, Sim target, CommonWoohoo.WoohooStyle style)
            {
                switch (style)
                {
                    case CommonWoohoo.WoohooStyle.Safe:
                        return new SafeBedDefinition(target);
                    case CommonWoohoo.WoohooStyle.Risky:
                        return new RiskyBedDefinition(target);
                    case CommonWoohoo.WoohooStyle.TryForBaby:
                        return new TryForBabyBedDefinition(target);
                }

                return null;
            }
        }

        public class IglooLocationControl : WoohooLocationControl
        {
            public override CommonWoohoo.WoohooLocation Location
            {
                get { return CommonWoohoo.WoohooLocation.Igloo; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is Igloo;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<Igloo>(new Predicate<Igloo>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<Igloo>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP8)) return false;
                }

                return Woohooer.Settings.mAutonomousIgloo;
            }

            public bool TestUse(Igloo obj)
            {
                if (!TestRepaired(obj)) return false;

                if (obj.UseCount > 0) return false;

                if (obj.NumberOfSpots() < 2) return false;

                return true;
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (Igloo obj in actor.LotCurrent.GetObjects<Igloo>(new Predicate<Igloo>(TestUse)))
                {
                    if ((testFunc != null) && (!testFunc(obj, null))) continue;

                    results.Add(obj);
                }

                return results;
            }

            public override InteractionDefinition GetInteraction(Sim actor, Sim target, CommonWoohoo.WoohooStyle style)
            {
                switch (style)
                {
                    case CommonWoohoo.WoohooStyle.Safe:
                        return new SafeBedDefinition(target);
                    case CommonWoohoo.WoohooStyle.Risky:
                        return new RiskyBedDefinition(target);
                    case CommonWoohoo.WoohooStyle.TryForBaby:
                        return new TryForBabyBedDefinition(target);
                }

                return null;
            }
        }
    }
}
