using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
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
    public class HotTubWooHoo : HotTubBase.CuddleSeatedWooHoo, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        new static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        static readonly InteractionDefinition SafeTubSingleton = new SafeTubDefinition();
        static readonly InteractionDefinition RiskyTubSingleton = new RiskyTubDefinition();
        static readonly InteractionDefinition TryForBabyTubSingleton = new TryForBabyTubDefinition();

        public HotTubWooHoo()
        { }

        public void OnPreLoad()
        {
            InteractionTuning tuning = Woohooer.InjectAndReset<Sim, HotTubBase.CuddleSeatedWooHoo.Definition, SafeDefinition>(false);
            if (tuning != null)
            {
                tuning.Availability.SetFlags(Availability.FlagField.DisallowedIfPregnant, false);
            }

            Woohooer.InjectAndReset<Sim, HotTubBase.CuddleSeatedWooHoo.Definition, ProxyDefinition>(false);

            Woohooer.InjectAndReset<Sim, HotTubBase.CuddleSeatedWooHoo.TryForBabyDefinition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<Sim, HotTubBase.CuddleSeatedWooHoo.TryForBabyDefinition, TryForBabyDefinition>(false);

            Woohooer.InjectAndReset<HotTubBase, SafeTubDefinition, RiskyTubDefinition>(true);
            Woohooer.InjectAndReset<HotTubBase, SafeTubDefinition, TryForBabyTubDefinition>(true);

            HotTubBase.CuddleSeatedWooHoo.Singleton = SafeSingleton;
            HotTubBase.CuddleSeatedWooHoo.TryForBabySingleton = TryForBabySingleton;
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(RiskySingleton);

            interactions.Add<HotTubBase>(SafeTubSingleton);
            interactions.Add<HotTubBase>(RiskyTubSingleton);
            interactions.Add<HotTubBase>(TryForBabyTubSingleton);
        }

        private new void OnAnimationEvent(StateMachineClient smc, IEvent evt)
        {
            HotTubBase container = Actor.Posture.Container as HotTubBase;
            if (evt.EventId == 0x6e)
            {
                container.StartWoohooFX();
                StartJealousyBroadcaster();
            }
            else if (evt.EventId == 0x6f)
            {
                container.StopWoohooFX();
            }
        }

        protected void OnBabyCheck(StateMachineClient smc, IEvent evt)
        {
            IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

            if (CommonPregnancy.IsSuccess(Actor, Target, Autonomous, definition.GetStyle(this)))
            {
                Pregnancy pregnancy = CommonPregnancy.Impregnate(Actor, Target, Autonomous, definition.GetStyle(this));
                if (pregnancy != null)
                {

                    HotTubBase container = Actor.Posture.Container as HotTubBase;

                    switch (RandomUtil.GetWeightedIndex(container.BabyTraitChance))
                    {
                        case 1:
                            pregnancy.SetForcedBabyTrait(TraitNames.Hydrophobic);
                            break;
                        case 2:
                            pregnancy.SetForcedBabyTrait(TraitNames.PartyAnimal);
                            break;
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
                    mReactToSocialBroadcaster = new ReactionBroadcaster(Actor, BedWoohoo.sBroadcastParams, SocialComponentEx.ReactToJealousEventHigh);
                    CommonWoohoo.CheckForWitnessedCheating(Actor, Target, !Rejected);
                }

                if (IsMaster)
                {
                    HotTubWooHoo linked = LinkedInteractionInstance as HotTubWooHoo;
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

        public override bool Run()
        {
            try
            {
                if (!StartSync())
                {
                    return false;
                }

                bool flag2 = false;

                StandardEntry(false);
                BeginCommodityUpdates();

                bool succeeded = false;

                try
                {
                    if (IsMaster)
                    {
                        HotTubBase container = Actor.Posture.Container as HotTubBase;
                        container.mSimsAreWooHooing = true;
                        ReturnInstance.EnsureMaster();
                        mCurrentStateMachine = ReturnInstance.mCurrentStateMachine;

                        IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition; ;

                        string socialName = CommonWoohoo.GetSocialName(definition.GetStyle(this), Actor);
                        StartSocial(socialName);

                        Actor.SocialComponent.StartSocializingWith(Target);

                        Dictionary<Sim, SocialRule> effects = new Dictionary<Sim, SocialRule>();
                        SocialEffect = Actor.Conversation.UpdateOnSelectingInteraction(Actor, Target, Autonomous, CommonWoohoo.GetSocialName(definition.GetStyle(this), Actor), null, null, effects);
                        Rejected = (SocialEffect == null) || (!SocialEffect.LHS.IsSocialAccepted());

                        InitiateSocialUI(Actor, Target);
                        (LinkedInteractionInstance as NestedCuddleInteraction).Rejected = Rejected;
                        if (Rejected)
                        {
                            mCurrentStateMachine.RequestState(null, "Woo Hoo Reject");
                            mCurrentStateMachine.RequestState(null, "ExitSitting");
                            FinishSocial(socialName, true);
                            FinishSocialContext();
                            Actor.BuffManager.AddElement(BuffNames.WalkOfShame, Origin.FromRejectedWooHooOffHome);
                        }
                        else
                        {
                            if (CommonWoohoo.NeedPrivacy(false, Actor, Target))
                            {
                                mSituation = new WooHoo.WooHooPrivacySituation(this);
                                flag2 = !mSituation.Start();
                            }

                            if (!flag2)
                            {
                                RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);
                                mCurrentStateMachine.AddOneShotScriptEventHandler(0x6e, OnAnimationEvent);
                                mCurrentStateMachine.AddOneShotScriptEventHandler(0x6f, OnAnimationEvent);
                                mCurrentStateMachine.AddOneShotScriptEventHandler(0x78, OnAnimationEvent);
                                mCurrentStateMachine.AddOneShotScriptEventHandler(0x79, OnAnimationEvent);
                                mCurrentStateMachine.AddOneShotScriptEventHandler(0x70, OnBabyCheck);

                                mCurrentStateMachine.RequestState(null, "Woo Hoo Accept");
                                PuddleManager.AddPuddle(Actor.Posture.Container.Position);
                                RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.LessWoohoo);

                                CommonWoohoo.RunPostWoohoo(Actor, Target, container, definition.GetStyle(this), definition.GetLocation(container), true);
                            }

                            FinishSocial(socialName, true);
                        }
                    }
                    else
                    {
                        DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                    }
                    FinishLinkedInteraction(IsMaster);
                    succeeded = !Rejected && !flag2;
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                    StandardExit(false, false);
                }

                if (Rejected)
                {
                    InvokeDoResumeOnCleanup = false;
                }
                else if (!mPrivacyFailed)
                {
                    Actor.SimDescription.SetFirstWooHoo();
                }

                WaitForSyncComplete();
                return !Rejected;
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

        public class PushWooHooOrTryForBaby
        {
            HotTubBase mObject;

            Sim mActor;
            Sim mTarget;

            InteractionDefinition mDefinition;

            bool mAutonomous;

            bool mTargetFail;

            int mAttempts = 0;

            public PushWooHooOrTryForBaby(HotTubBase obj, Sim actor, Sim target, bool autonomous, bool pushGetIn, InteractionDefinition definition)
            {
                mObject = obj;
                mActor = actor;
                mTarget = target;
                mDefinition = definition;
                mAutonomous = autonomous;

                if (pushGetIn)
                {
                    InteractionDefinition getInDefinition = HotTubGetIn.Singleton;
                    if (Woohooer.Settings.mNakedOutfitHotTub)
                    {
                        InteractionInstanceParameters parameters = new InteractionInstanceParameters(new InteractionObjectPair(HotTubGetIn.SkinnyDipSingleton, mObject), actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true);

                        bool success = true;

                        GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                        if (!IUtil.IsPass(HotTubGetIn.SkinnyDipSingleton.Test(ref parameters, ref greyedOutTooltipCallback)))
                        {
                            success = false;
                        }

                        if (success)
                        {
                            parameters = new InteractionInstanceParameters(new InteractionObjectPair(HotTubGetIn.SkinnyDipSingleton, mObject), target, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true);

                            if (!IUtil.IsPass(HotTubGetIn.SkinnyDipSingleton.Test(ref parameters, ref greyedOutTooltipCallback)))
                            {
                                success = false;
                            }
                        }

                        if (success)
                        {
                            getInDefinition = HotTubGetIn.SkinnyDipSingleton;
                        }
                    }

                    mActor.GreetSimOnMyLotIfPossible(mTarget);
                    HotTubGetIn entry = getInDefinition.CreateInstanceWithCallbacks(mObject, mActor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true, null, OnActorCompleted, null) as HotTubGetIn;
                    entry.mIsMaster = true;
                    if (mActor.InteractionQueue.Add(entry))
                    {
                        InteractionInstance instance = getInDefinition.CreateInstanceWithCallbacks(mObject, mTarget, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true, null, OnCompleted, null);

                        if (mTarget.InteractionQueue.Add(instance))
                        {
                            instance.LinkedInteractionInstance = entry;
                        }
                        else
                        {
                            mTargetFail = true;
                        }
                    }
                }
                else
                {
                    OnCompleted(mActor, 1);
                }
            }

            public void OnActorCompleted(Sim a, float value)
            {
                if (!mTargetFail) return;

                OnCompleted(a, value);
            }

            public void OnCompleted(Sim a, float value)
            {
                try
                {
                    if (mActor.CurrentInteraction == null)
                    {
                        Common.DebugNotify("OnCompleted A");

                        OnFailed2(mActor, 0);
                    }
                    else if (mTarget.CurrentInteraction == null)
                    {
                        Common.DebugNotify("OnCompleted B");

                        OnFailed2(mTarget, 0);
                    }
                    else if (mActor.CurrentInteraction.Target == mTarget.CurrentInteraction.Target)
                    {
                        Common.DebugNotify("OnCompleted C");

                        InteractionInstance instance = StartSeatedCuddleEx.Singleton.CreateInstanceWithCallbacks(mTarget, mActor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true, null, OnCompleted2, OnFailed2);
                        mActor.InteractionQueue.Add(instance);
                    }
                    else
                    {
                        Common.DebugNotify("OnCompleted D");
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(mActor, mTarget, e);
                }
            }

            public void OnCompleted2(Sim a, float value)
            {
                Common.DebugNotify("OnCompleted2");

                new DelayedSucccessTask(this);
            }

            public void OnCompleted3(Sim a, float value)
            {
                try
                {
                    Common.DebugNotify("OnCompleted3");

                    if (mAttempts < 3)// && (mAutonomous))
                    {
                        mAttempts++;

                        InteractionInstance instance = mDefinition.CreateInstanceWithCallbacks(mTarget, mActor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true, null, null, OnFailed2);
                        mActor.InteractionQueue.Add(instance);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(mActor, mTarget, e);
                }
            }

            public void OnFailed2(Sim a, float value)
            {
                Common.DebugNotify("OnFailed2");

                if (mAttempts < 3)// && (mAutonomous))
                {
                    mAttempts++;

                    new DelayedFailTask(this);
                }
            }

            public class DelayedSucccessTask : Common.AlarmTask
            {
                PushWooHooOrTryForBaby mTask;

                public DelayedSucccessTask(PushWooHooOrTryForBaby task)
                    : base(5, TimeUnit.Minutes)
                {
                    mTask = task;
                }

                protected override void OnPerform()
                {
                    mTask.OnCompleted3(null, 0);
                }
            }

            public class DelayedFailTask : Common.AlarmTask
            {
                PushWooHooOrTryForBaby mTask;

                public DelayedFailTask(PushWooHooOrTryForBaby task)
                    : base (5, TimeUnit.Minutes)
                {
                    mTask = task;
                }

                protected override void OnPerform()
                {
                    mTask.OnCompleted(null, 0);
                }
            }
        }

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<Sim, HotTubWooHoo, BaseTubDefinition>
        {
            public ProxyDefinition(BaseTubDefinition definition)
                : base(definition)
            { }

            public override Sim ITarget(InteractionInstance interaction)
            {
                return interaction.Target as Sim;
            }
        }

        public abstract class BaseTubDefinition : CommonWoohoo.PotentialDefinition<HotTubBase>
        {
            public BaseTubDefinition()
            { }
            public BaseTubDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.HotTub;
            }

            public override Sim GetTarget(Sim actor, HotTubBase target, InteractionInstance interaction)
            {
                List<Sim> sims = new List<Sim>(target.ActorsUsingMe);
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

            public override bool JoinInProgress(Sim actor, Sim target, HotTubBase obj, InteractionInstance interaction)
            {
                if ((obj.ActorsUsingMe.Contains(actor)) && (obj.ActorsUsingMe.Contains(target)))
                {
                    new PushWooHooOrTryForBaby(obj, actor, target, true, false, new ProxyDefinition(this));
                    return true;
                }
                else
                {
                    return base.JoinInProgress(actor, target, obj, interaction);
                }
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                new PushWooHooOrTryForBaby(obj as HotTubBase, actor, target, true, true, new ProxyDefinition(this));
            }

            protected override bool Satisfies(Sim actor, Sim target, HotTubBase obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                return true;
            }
        }

        public class SafeTubDefinition : BaseTubDefinition
        {
            public SafeTubDefinition()
            { }
            public SafeTubDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.Safe;
            }

            public override string GetInteractionName(Sim actor, HotTubBase target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim a, Sim target, HotTubBase obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonWoohoo.SatisfiesWoohoo(a, target, "HotTubWoohoo", isAutonomous, true, true, ref greyedOutTooltipCallback);
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                }
                return false;
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new SafeTubDefinition(target));
            }
        }

        public class RiskyTubDefinition : BaseTubDefinition
        {
            public RiskyTubDefinition()
            { }
            public RiskyTubDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return CommonWoohoo.WoohooStyle.Risky;
            }

            public override string GetInteractionName(Sim actor, HotTubBase target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim a, Sim target, HotTubBase obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonPregnancy.SatisfiesRisky(a, target, "HotTubRisky", isAutonomous, true, ref greyedOutTooltipCallback);
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                }
                return false;
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new RiskyTubDefinition(target));
            }
        }

        public class TryForBabyTubDefinition : BaseTubDefinition
        {
            public TryForBabyTubDefinition()
            { }
            public TryForBabyTubDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return CommonWoohoo.WoohooStyle.TryForBaby;
            }

            public override string GetInteractionName(Sim actor, HotTubBase target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim a, Sim target, HotTubBase obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonPregnancy.SatisfiesTryForBaby(a, target, "HotTubTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                }
                return false;
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new TryForBabyTubDefinition(target));
            }
        }

        public abstract class BaseDefinition : CommonWoohoo.BaseDefinition<Sim, HotTubWooHoo>
        {
            public BaseDefinition()
            { }

            public override Sim GetTarget(Sim actor, Sim target, InteractionInstance interaction)
            {
                return target;
            }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.HotTub;
            }

            public override int Attempts
            {
                set { }
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!NestedSocialInteraction<CuddleSeated>.Test(a, target))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Not Cuddle Seated");
                        return false;
                    }

                    SittingPosture posture = a.Posture as SittingPosture;
                    if (posture != null)
                    {
                        if (!(posture.Container is HotTubBase))
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Actor Not In Hot Tub");
                            return false;
                        }
                    }

                    posture = target.Posture as SittingPosture;
                    if (posture != null)
                    {
                        if (!(posture.Container is HotTubBase))
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Target Not In Hot Tub");
                            return false;
                        }
                    }

                    return true;
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
            public SafeDefinition()
            { }

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

                    return CommonWoohoo.SatisfiesWoohoo(a, target, "HotTubWoohoo", isAutonomous, true, true, ref greyedOutTooltipCallback);
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

                    return CommonPregnancy.SatisfiesRisky(a, target, "HotTubRisky", isAutonomous, true, ref greyedOutTooltipCallback);
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                }
                return false;
            }
        }

        public new class TryForBabyDefinition : BaseDefinition
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

                    return CommonPregnancy.SatisfiesTryForBaby(a, target, "HotTubTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                }
                return false;
            }
        }

        public class LocationControl : WoohooLocationControl
        {
            public override CommonWoohoo.WoohooLocation Location
            {
                get { return CommonWoohoo.WoohooLocation.HotTub; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is HotTubBase;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<HotTubBase>(new Predicate<HotTubBase>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<IHotTub>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (sim.IsFrankenstein) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP3)) return false;
                }

                return Woohooer.Settings.mAutonomousHotTub;
            }

            public bool TestUse(HotTubBase obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0 && obj.InWorld);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (HotTubBase obj in actor.LotCurrent.GetObjects<HotTubBase>(new Predicate<HotTubBase>(TestUse)))
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
                        return new HotTubWooHoo.SafeTubDefinition(target);
                    case CommonWoohoo.WoohooStyle.Risky:
                        return new HotTubWooHoo.RiskyTubDefinition(target);
                    case CommonWoohoo.WoohooStyle.TryForBaby:
                        return new HotTubWooHoo.TryForBabyTubDefinition(target);
                }

                return null;
            }
        }
    }
}
