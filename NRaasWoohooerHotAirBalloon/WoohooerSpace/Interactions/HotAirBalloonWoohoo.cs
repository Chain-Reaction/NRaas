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
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Controller;
using Sims3.Store.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class HotAirBalloonWoohoo : HotairBalloon.WooHooSocial, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSimSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySimSingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySimSingleton = new TryForBabyDefinition();

        static readonly InteractionDefinition SafeHotAirBalloonSingleton = new SafeHotAirBalloonDefinition();
        static readonly InteractionDefinition RiskyHotAirBalloonSingleton = new RiskyHotAirBalloonDefinition();
        static readonly InteractionDefinition TryForBabyHotAirBalloonSingleton = new TryForBabyHotAirBalloonDefinition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<HotairBalloon>(SafeHotAirBalloonSingleton);
            interactions.Add<HotairBalloon>(RiskyHotAirBalloonSingleton);
            interactions.Add<HotairBalloon>(TryForBabyHotAirBalloonSingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<Sim, HotairBalloon.WooHooSocial.Definition, ProxyDefinition>(false);

            Tunings.Inject<Sim, HotairBalloon.WooHooSocial.Definition, ProxySimDefinition>(false);

            Woohooer.InjectAndReset<Sim, HotairBalloon.WooHooSocial.Definition, SafeDefinition>(false);

            Woohooer.InjectAndReset<Sim, HotairBalloon.WooHooSocial.Definition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<Sim, HotairBalloon.WooHooSocial.Definition, TryForBabyDefinition>(false);

            Woohooer.InjectAndReset<HotairBalloon, SafeDefinition, SafeHotAirBalloonDefinition>(true);
            Woohooer.InjectAndReset<HotairBalloon, SafeDefinition, RiskyHotAirBalloonDefinition>(true);
            Woohooer.InjectAndReset<HotairBalloon, SafeDefinition, TryForBabyHotAirBalloonDefinition>(true);

            HotairBalloon.WooHooSocial.Singleton = new ProxySimDefinition();
            HotairBalloon.WooHoo.Singleton = SafeHotAirBalloonSingleton;
        }

        public override bool Run()
        {
            try
            {
                if (!SafeToSync())
                {
                    Common.Notify("Fail D");
                    return false;
                }

                IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                if (LinkedInteractionInstance == null)
                {
                    HotairBalloon.WooHooSocial entry = definition.ProxyClone(Target).CreateInstance(Actor, Target, GetPriority(), Autonomous, CancellableByPlayer) as HotairBalloon.WooHooSocial;
                    if (entry == null)
                    {
                        Common.Notify("Fail A");
                        return false;
                    }

                    entry.mIsSocialTarget = true;
                    LinkedInteractionInstance = entry;
                    Target.InteractionQueue.AddNext(entry);
                }

                HotairBalloon.InBalloonPosture posture = Actor.Posture as HotairBalloon.InBalloonPosture;
                if (posture == null)
                {
                    Common.Notify("Fail B");
                    return false;
                }

                if (!StartSync(!mIsSocialTarget))
                {
                    Common.Notify("Fail C");
                    return false;
                }

                StandardEntry();
                BeginCommodityUpdates();
                if (mIsSocialTarget)
                {
                    DoLoop(ExitReason.Finished);
                }
                else
                {
                    StartSocial(CommonWoohoo.GetSocialName(definition.GetStyle(this), Actor));

                    Animation.ForceAnimation(Actor.ObjectId, true);
                    Animation.ForceAnimation(Target.ObjectId, true);

                    if (Rejected)
                    {
                        Target.Posture.CurrentStateMachine.RequestState(true, "x", "ToFromSocial");
                        posture.CurrentStateMachine.RequestState(true, "x", "ToFromSocial");
                        posture.CurrentStateMachine.SetActor("y", Target);
                        CreateProps(posture.CurrentStateMachine);
                        posture.CurrentStateMachine.SetParameter("XSimR", posture.IsXActor ? YesOrNo.no : YesOrNo.yes);
                        posture.CurrentStateMachine.EnterState("x", "EnterSocial");
                        posture.CurrentStateMachine.EnterState("y", "EnterSocial");
                        ThoughtBalloonManager.BalloonData bd = new ThoughtBalloonManager.DoubleBalloonData("balloon_woohoo", "balloon_question");
                        bd.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
                        Actor.ThoughtBalloonManager.ShowBalloon(bd);
                        posture.CurrentStateMachine.RequestState(false, "y", "woohoo rejected");
                        posture.CurrentStateMachine.RequestState(true, "x", "woohoo rejected");
                        bd = new ThoughtBalloonManager.BalloonData("balloon_woohoo");
                        bd.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
                        bd.LowAxis = ThoughtBalloonAxis.kDislike;
                        Target.ThoughtBalloonManager.ShowBalloon(bd);
                        posture.CurrentStateMachine.RequestState(false, "y", "ExitSocial");
                        posture.CurrentStateMachine.RequestState(true, "x", "ExitSocial");
                        posture.CurrentStateMachine.RemoveActor(Target);
                        Target.Posture.CurrentStateMachine.EnterState("x", "ToFromSocial");
                        posture.CurrentStateMachine.EnterState("x", "ToFromSocial");
                        Target.Posture.CurrentStateMachine.EnterState("x", "IdleStand");
                        posture.CurrentStateMachine.EnterState("x", "IdleStand");
                        Actor.GetRelationship(Target, true).LTR.UpdateLiking(HotairBalloon.kWoohooRejectLtrChange);
                        SocialComponent.SetSocialFeedbackForActorAndTarget(CommodityTypes.Friendly, Actor, Target, false, 0x0, LongTermRelationshipTypes.Undefined, LongTermRelationshipTypes.Undefined);
                        SocialCallback.AddRejectedByEx(Actor, Target, GetInteractionName(), null, this);
                    }
                    else
                    {
                        switch (posture.Balloon.mCurrentHeight)
                        {
                            case HotairBalloon.BalloonHeight.OnGround:
                                posture.CurrentStateMachine.SetParameter("Height", SkillLevel.poor);
                                break;

                            case HotairBalloon.BalloonHeight.Height1:
                                posture.CurrentStateMachine.SetParameter("Height", SkillLevel.novice);
                                break;
                        }

                        Sim actor = posture.IsXActor ? Actor : Target;
                        Sim sim2 = posture.IsXActor ? Target : Actor;
                        actor.Posture.CurrentStateMachine.RequestState(true, "x", "ToFromSocial");
                        sim2.Posture.CurrentStateMachine.RequestState(true, "x", "ToFromSocial");
                        sim2.Posture.CurrentStateMachine.SetActor("y", actor);
                        CreateProps(sim2.Posture.CurrentStateMachine);
                        sim2.Posture.CurrentStateMachine.EnterState("x", "EnterSocial");
                        sim2.Posture.CurrentStateMachine.EnterState("y", "EnterSocial");
                        ThoughtBalloonManager.BalloonData data2 = new ThoughtBalloonManager.DoubleBalloonData("balloon_woohoo", "balloon_question");
                        data2.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
                        Actor.ThoughtBalloonManager.ShowBalloon(data2);
                        sim2.Posture.CurrentStateMachine.RequestState(false, "y", "woohoo");
                        sim2.Posture.CurrentStateMachine.RequestState(true, "x", "woohoo");
                        sim2.Posture.CurrentStateMachine.RequestState(false, "y", "ExitSocial");
                        sim2.Posture.CurrentStateMachine.RequestState(true, "x", "ExitSocial");
                        sim2.Posture.CurrentStateMachine.RemoveActor(actor);
                        actor.Posture.CurrentStateMachine.EnterState("x", "ToFromSocial");
                        sim2.Posture.CurrentStateMachine.EnterState("x", "ToFromSocial");
                        Relationship relationship = Actor.GetRelationship(Target, true);
                        relationship.STC.Update(Actor, Target, CommodityTypes.Amorous, HotairBalloon.kSTCIncreaseAfterWoohoo);
                        relationship.LTR.UpdateLiking(-HotairBalloon.kWoohooRejectLtrChange);

                        CommonWoohoo.RunPostWoohoo(Actor, Target, posture.Balloon, definition.GetStyle(this), definition.GetLocation(posture.Balloon), true);

                        if (CommonPregnancy.IsSuccess(Actor, Target, Autonomous, definition.GetStyle(this)))
                        {
                            CommonPregnancy.Impregnate(Actor, Target, Autonomous, definition.GetStyle(this));
                        }

                        actor.Posture.CurrentStateMachine.EnterState("x", "IdleStand");
                        sim2.Posture.CurrentStateMachine.EnterState("x", "IdleStand");

                        Actor.BuffManager.AddElement((BuffNames)(0x9a7f5f1919df0036L), Origin.None);
                        Target.BuffManager.AddElement((BuffNames)(0x9a7f5f1919df0036L), Origin.None);
                    }

                    FinishSocial(CommonWoohoo.GetSocialName(definition.GetStyle(this), Actor), Rejected);
                    Target.AddExitReason(ExitReason.Finished);
                }

                FinishLinkedInteraction(mIsSocialTarget);
                EndCommodityUpdates(Rejected);
                StandardExit();
                WaitForSyncComplete();
                posture.Balloon.PushIdleInteractionOnSim(Actor);
                return true;
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

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<Sim, HotAirBalloonWoohoo, BaseHotAirBalloonDefinition>
        {
            public ProxyDefinition(BaseHotAirBalloonDefinition definition)
                : base(definition)
            { }

            public override Sim ITarget(InteractionInstance interaction)
            {
                return interaction.Target as Sim;
            }
        }

        public class ProxySimDefinition : HotairBalloon.WooHooSocial.Definition
        {
            public ProxySimDefinition()
            { }

            public override bool Test(Sim actor, Sim target, bool autonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return false;
            }
        }

        public abstract class BaseHotAirBalloonDefinition : CommonWoohoo.PotentialDefinition<HotairBalloon>
        {
            public BaseHotAirBalloonDefinition()
            { }
            public BaseHotAirBalloonDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.HotAirBalloon;
            }

            public override Sim GetTarget(Sim actor, HotairBalloon target, InteractionInstance interaction)
            {
                HotairBalloon hotAirBalloon = target as HotairBalloon;

                Sim result = hotAirBalloon.GetOtherSim(actor);
                if (result != null)
                {
                    return result;
                }
                else
                {
                    return base.GetTarget(actor, target, interaction);
                }
            }

            public override bool JoinInProgress(Sim actor, Sim target, HotairBalloon obj, InteractionInstance interaction)
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
                new PushWooHooOrTryForBaby(obj as HotairBalloon, actor, target, true, true, new ProxyDefinition(this));
            }

            protected override bool Satisfies(Sim actor, Sim target, HotairBalloon obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                return true;
            }
        }

        public class PushWooHooOrTryForBaby
        {
            HotairBalloon mObject;

            Sim mActor;
            Sim mTarget;

            InteractionDefinition mDefinition;

            bool mAutonomous;

            bool mTargetFail;

            int mAttempts = 0;

            public PushWooHooOrTryForBaby(HotairBalloon obj, Sim actor, Sim target, bool autonomous, bool pushGetIn, InteractionDefinition definition)
            {
                mObject = obj;
                mActor = actor;
                mTarget = target;
                mDefinition = definition;
                mAutonomous = autonomous;

                if (pushGetIn)
                {
                    HotAirBalloonSit entry = mActor.CurrentInteraction as HotAirBalloonSit;
                    if ((entry == null) || (entry.Target != mObject))
                    {
                        entry = HotAirBalloonSit.Singleton.CreateInstanceWithCallbacks(mObject, mActor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true, null, OnActorCompleted, null) as HotAirBalloonSit;
                        if (!mActor.InteractionQueue.Add(entry)) return;
                    }

                    entry.mIsMaster = true;

                    HotAirBalloonSit instance = mTarget.CurrentInteraction as HotAirBalloonSit;
                    if ((instance == null) || (instance.Target != mObject))
                    {
                        instance = HotAirBalloonSit.Singleton.CreateInstanceWithCallbacks(mObject, mTarget, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true, null, OnCompleted, null) as HotAirBalloonSit;
                        if (!mTarget.InteractionQueue.Add(instance))
                        {
                            mTargetFail = true;
                            return;
                        }
                    }

                    instance.mIsMaster = false;
                    instance.LinkedInteractionInstance = entry;
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
                    List<Sim> sims = new List<Sim>();
                    sims.Add(mObject.GetLeftSim());
                    sims.Add(mObject.GetRightSim());

                    if ((sims.Contains(mActor)) && (sims.Contains(mTarget)))
                    {
                        Common.DebugNotify("OnCompleted C");

                        Sim actor = mActor;
                        Sim target = mTarget;

                        HotairBalloon.InBalloonPosture posture = mActor.Posture as HotairBalloon.InBalloonPosture;
                        if ((posture == null) || (!posture.IsXActor))
                        {
                            actor = mTarget;
                            target = mActor;
                        }

                        InteractionInstance instance = HotairBalloon.Raise.Singleton.CreateInstanceWithCallbacks(mObject, actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true, null, null, OnFailed2);
                        actor.InteractionQueue.Add(instance);

                        instance = mDefinition.CreateInstanceWithCallbacks(target, actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true, null, null, OnFailed2);
                        actor.InteractionQueue.Add(instance);
                    }
                    else
                    {
                        Common.DebugNotify("OnCompleted A");
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

            public class DelayedFailTask : Common.AlarmTask
            {
                PushWooHooOrTryForBaby mTask;

                public DelayedFailTask(PushWooHooOrTryForBaby task)
                    : base(5, TimeUnit.Minutes)
                {
                    mTask = task;
                }

                protected override void OnPerform()
                {
                    mTask.OnCompleted(null, 0);
                }
            }
        }

        public class SafeHotAirBalloonDefinition : BaseHotAirBalloonDefinition
        {
            public SafeHotAirBalloonDefinition()
            { }
            public SafeHotAirBalloonDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.Safe;
            }

            public override string GetInteractionName(Sim actor, HotairBalloon target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, HotairBalloon obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                try
                {
                    if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                    return CommonWoohoo.SatisfiesWoohoo(actor, target, "HotAirBalloonWoohoo", isAutonomous, true, true, ref callback);
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
                return new ProxyDefinition(new SafeHotAirBalloonDefinition(target));
            }
        }

        public class RiskyHotAirBalloonDefinition : BaseHotAirBalloonDefinition
        {
            public RiskyHotAirBalloonDefinition()
            { }
            public RiskyHotAirBalloonDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return CommonWoohoo.WoohooStyle.Risky;
            }

            public override string GetInteractionName(Sim actor, HotairBalloon target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim a, Sim target, HotairBalloon obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonPregnancy.SatisfiesRisky(a, target, "HotAirBalloonRisky", isAutonomous, true, ref greyedOutTooltipCallback);
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
                return new ProxyDefinition(new RiskyHotAirBalloonDefinition(target));
            }
        }

        public class TryForBabyHotAirBalloonDefinition : BaseHotAirBalloonDefinition
        {
            public TryForBabyHotAirBalloonDefinition()
            { }
            public TryForBabyHotAirBalloonDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return CommonWoohoo.WoohooStyle.TryForBaby;
            }

            public override string GetInteractionName(Sim actor, HotairBalloon target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim a, Sim target, HotairBalloon obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonPregnancy.SatisfiesTryForBaby(a, target, "HotAirBalloonTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
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
                return new ProxyDefinition(new TryForBabyHotAirBalloonDefinition(target));
            }
        }    

        public abstract class BaseDefinition : CommonWoohoo.BaseDefinition<Sim, HotAirBalloonWoohoo>, IOverrideStartInteractionBehavior
        {
            public override Sim GetTarget(Sim actor, Sim target, InteractionInstance interaction)
            {
                return target;
            }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.HotAirBalloon;
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

                    if (!isAutonomous && SocialComponent.IsTargetUnavailableForSocialInteraction(target, ref greyedOutTooltipCallback))
                    {
                        if (greyedOutTooltipCallback == null)
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Target Unavailable");
                        }
                        return false;
                    }

                    return true;
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

                    return CommonWoohoo.SatisfiesWoohoo(a, target, "HotAirBalloonWoohoo", isAutonomous, true, true, ref greyedOutTooltipCallback);
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

                    return CommonPregnancy.SatisfiesRisky(a, target, "HotAirBalloonRisky", isAutonomous, true, ref greyedOutTooltipCallback);
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

                    return CommonPregnancy.SatisfiesTryForBaby(a, target, "HotAirBalloonTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
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
    }
}
