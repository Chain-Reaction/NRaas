using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.InteractionsShared;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Interactions
{
    public class CuddleSeatedWooHooSaunaEx : SaunaClassic.CuddleSeatedWooHooSauna, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static new readonly InteractionDefinition TryForBoySingleton = new TryForBabyDefinition(TryingFor.TryForBoy);
        static new readonly InteractionDefinition TryForGirlSingleton = new TryForBabyDefinition(TryingFor.TryForGirl);

        static readonly InteractionDefinition SafeSaunaSingleton = new SafeSaunaDefinition();
        static readonly InteractionDefinition RiskySaunaSingleton = new RiskySaunaDefinition();
        static readonly InteractionDefinition TryForBoySaunaSingleton = new TryForBabySaunaDefinition(TryingFor.TryForBoy);
        static readonly InteractionDefinition TryForGirlSaunaSingleton = new TryForBabySaunaDefinition(TryingFor.TryForGirl);

        public void OnPreLoad()
        {
            InteractionTuning tuning = Woohooer.InjectAndReset<Sim, SaunaClassic.CuddleSeatedWooHooSauna.Definition, SafeDefinition>(false);
            if (tuning != null)
            {
                tuning.Availability.SetFlags(Availability.FlagField.DisallowedIfPregnant, false);
            }

            Woohooer.InjectAndReset<Sim, SaunaClassic.CuddleSeatedWooHooSauna.Definition, ProxyDefinition>(false);
            Woohooer.InjectAndReset<Sim, SaunaClassic.CuddleSeatedWooHooSauna.Definition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<Sim, SaunaClassic.CuddleSeatedWooHooSauna.Definition, TryForBabyDefinition>(false);

            Woohooer.InjectAndReset<SaunaClassic, SafeSaunaDefinition, RiskySaunaDefinition>(true);
            Woohooer.InjectAndReset<SaunaClassic, SafeSaunaDefinition, TryForBabySaunaDefinition>(true);

            SaunaClassic.CuddleSeatedWooHooSauna.Singleton = SafeSingleton;
            SaunaClassic.CuddleSeatedWooHooSauna.TryForBoySingleton = TryForBoySingleton;
            SaunaClassic.CuddleSeatedWooHooSauna.TryForGirlSingleton = TryForGirlSingleton;
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(RiskySingleton);

            interactions.Add<SaunaClassic>(SafeSaunaSingleton);
            interactions.Add<SaunaClassic>(RiskySaunaSingleton);
            interactions.AddNoDupTest<SaunaClassic>(TryForBoySaunaSingleton);
            interactions.AddNoDupTest<SaunaClassic>(TryForGirlSaunaSingleton);
        }

        private void OnBabyCheckEventEx(StateMachineClient smc, IEvent evt)
        {
            try
            {
                ISaunaWooHooDefinition definition = InteractionDefinition as ISaunaWooHooDefinition;

                if (CommonPregnancy.IsSuccess(Actor, Target, Autonomous, definition.GetStyle(this)))
                {
                    Pregnancy pregnancy = CommonPregnancy.Impregnate(Actor, Target, Autonomous, definition.GetStyle(this));
                    if (pregnancy != null)
                    {
                        switch (RandomUtil.GetWeightedIndex(SaunaClassic.kBabyTraitChance))
                        {
                            case 1:
                                pregnancy.SetForcedBabyTrait(TraitNames.Hydrophobic);
                                break;

                            case 2:
                                pregnancy.SetForcedBabyTrait(TraitNames.PartyAnimal);
                                break;
                        }

                        Audio.StartSound("sting_baby_conception");
                        if (definition.TryingFor == TryingFor.TryForBoy)
                        {
                            pregnancy.mGender = CASAgeGenderFlags.Male;
                        }
                        else if (definition.TryingFor == TryingFor.TryForGirl)
                        {
                            pregnancy.mGender = CASAgeGenderFlags.Female;
                        }
                    }
                }
            }
            catch (ResetException)
            {
                throw;
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
                ISaunaWooHooDefinition definition = InteractionDefinition as ISaunaWooHooDefinition;

                if (!StartSync())
                {
                    DoResume();
                    return false;
                }

                StandardEntry(false);
                mSauna = Actor.Posture.Container as SaunaClassic;
                if (Actor == mSauna.GetLeftSim())
                {
                    IHasSeatingGroup container = Actor.Posture.Container as IHasSeatingGroup;
                    Seat seat = container.SeatingGroup[Actor];
                    Seat seat2 = container.SeatingGroup[Target];
                    if ((seat == null) || (seat2 == null))
                    {
                        Actor.AddExitReason(ExitReason.FailedToStart);
                        return false;
                    }

                    // Custom
                    string socialName = CommonWoohoo.GetSocialName(definition.GetStyle(this), Actor);

                    ReturnInstance.EnsureMaster();
                    mCurrentStateMachine = ReturnInstance.mCurrentStateMachine;
                    StartSocial(socialName);
                    InitiateSocialUI(Actor, Target);
                    SaunaClassic.CuddleSeatedWooHooSauna linkedInteractionInstance = LinkedInteractionInstance as SaunaClassic.CuddleSeatedWooHooSauna;
                    linkedInteractionInstance.Rejected = Rejected;
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
                        mCurrentStateMachine.RequestState(null, "ExitSitting");

                        // Custom
                        CommonWoohoo.TestNakedOutfit(Woohooer.Settings.mNakedOutfitSaunaWoohoo, Actor, Target);

                        if (Stand.Singleton.CreateInstance(mSauna, Actor, Actor.InheritedPriority(), false, false).RunInteraction())
                        {
                            // Custom
                            if (CommonWoohoo.NeedPrivacy(false, Actor, Target))
                            {
                                mSituation = new WooHoo.WooHooPrivacySituation(this);
                                mPrivacyFailed = !mSituation.Start();
                            }
                            else
                            {
                                mPrivacyFailed = false;
                            }

                            linkedInteractionInstance.mPrivacyFailed = mPrivacyFailed;
                            if (!mPrivacyFailed && Actor.RouteToSlot(mSauna, Slot.RoutingSlot_14))
                            {
                                EnterStateMachine("Sauna_store", "SimEnter", "x");
                                SetActor("saunaX", seat.Host);
                                SetActor("saunaY", seat2.Host);
                                AddOneShotScriptEventHandler(0x384, OnAnimationEvent);
                                AnimateSim("PourWater");
                                AnimateSim("SimExit");

                                if (SaunaSit.WoohooSingleton.CreateInstance(mSauna, Actor, Actor.InheritedPriority(), false, false).RunInteraction())
                                {
                                    RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.MoreWoohoo);
                                    EnterStateMachine("sauna_store", "SimEnter", "x", "y");
                                    SetActor("saunaX", mSauna);
                                    mCurrentStateMachine.AddOneShotScriptEventHandler(0x6e, OnAnimationEvent);
                                    mCurrentStateMachine.AddOneShotScriptEventHandler(0x6f, OnAnimationEvent);
                                    mCurrentStateMachine.AddOneShotScriptEventHandler(0x78, OnAnimationEvent);
                                    mCurrentStateMachine.AddOneShotScriptEventHandler(0x79, OnAnimationEvent);
                                    mCurrentStateMachine.AddOneShotScriptEventHandler(0x70, OnBabyCheckEventEx);

                                    SetActor("saunaX", seat.Host);
                                    SetActor("saunaY", seat2.Host);
                                    if (Actor == mSauna.GetLeftSim())
                                    {
                                        SetParameter("IsMirrored", true);
                                        SetParameter("SuffixX", mSauna.mSeatingGroup[Actor].IKSuffix);
                                        SetParameter("SuffixY", mSauna.mSeatingGroup[Target].IKSuffix);
                                    }
                                    else
                                    {
                                        SetParameter("IsMirrored", false);
                                        SetParameter("SuffixY", mSauna.mSeatingGroup[Actor].IKSuffix);
                                        SetParameter("SuffixX", mSauna.mSeatingGroup[Target].IKSuffix);
                                    }

                                    AnimateJoinSims("Woohoo");
                                    AnimateJoinSims("SimExit");
                                    RockGemMetalBase.HandleNearbyWoohoo(Actor, RockGemMetalBase.HowMuchWooHoo.LessWoohoo);

                                    // Custom
                                    CommonWoohoo.RunPostWoohoo(Actor, Target, mSauna, definition.GetStyle(this), definition.GetLocation(mSauna), true);

                                    Target.Motives.ChangeValue(CommodityKind.Fun, SaunaClassic.kWooHooFunBump);
                                    Target.Motives.ChangeValue(CommodityKind.Social, SaunaClassic.kWooHooSocialBump);
                                    Target.Motives.ChangeValue(CommodityKind.Hygiene, SaunaClassic.kWooHooHygieneBump);
                                    Actor.Motives.ChangeValue(CommodityKind.Fun, SaunaClassic.kWooHooFunBump);
                                    Actor.Motives.ChangeValue(CommodityKind.Social, SaunaClassic.kWooHooSocialBump);
                                    Actor.Motives.ChangeValue(CommodityKind.Hygiene, SaunaClassic.kWooHooHygieneBump);
                                }
                            }
                        }

                        FinishSocial(socialName, true);
                        Actor.AddExitReason(ExitReason.StageComplete);
                        Target.AddExitReason(ExitReason.StageComplete);
                    }
                }
                else
                {
                    DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                }

                FinishLinkedInteraction(IsMaster);
                bool succeeded = !Rejected && !mPrivacyFailed;
                EndCommodityUpdates(succeeded);
                StandardExit(false, false);
                InvokeDoResumeOnCleanup = false;
                if (!mPrivacyFailed)
                {
                    Actor.SimDescription.SetFirstWooHoo();
                }

                if ((Rejected && !IsMaster) && Stand.Singleton.CreateInstance(mSauna, Actor, Actor.InheritedPriority(), false, false).RunInteraction())
                {
                    Actor.PlayReaction(ReactionTypes.Embarrassed, ReactionSpeed.AfterInteraction);
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
            SaunaClassic mObject;

            Sim mActor;
            Sim mTarget;

            InteractionDefinition mDefinition;

            bool mAutonomous;

            int mAttempts = 0;

            bool mTargetFail = false;

            public PushWooHooOrTryForBaby(SaunaClassic obj, Sim actor, Sim target, bool autonomous, bool pushGetIn, InteractionDefinition definition)
            {
                mObject = obj;
                mActor = actor;
                mTarget = target;
                mDefinition = definition;
                mAutonomous = autonomous;

                if (pushGetIn)
                {
                    SaunaSit entry = mActor.CurrentInteraction as SaunaSit;
                    if ((entry == null) || (entry.Target != mObject))
                    {
                        entry = SaunaSit.WoohooSingleton.CreateInstanceWithCallbacks(mObject, mActor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true, null, OnActorCompleted, null) as SaunaSit;
                        if (!mActor.InteractionQueue.Add(entry)) return;
                    }

                    entry.mIsMaster = true;

                    SaunaSit instance = mTarget.CurrentInteraction as SaunaSit;
                    if ((instance == null) || (instance.Target != mObject))
                    {
                        instance = SaunaSit.WoohooSingleton.CreateInstanceWithCallbacks(mObject, mTarget, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true, null, OnCompleted, null) as SaunaSit;
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
                if (mTargetFail) return;

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

                        InteractionInstance instance = SaunaClassic.StartSaunaSeatedCuddleA.Singleton.CreateInstanceWithCallbacks(mTarget, mActor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true, null, OnCompleted2, OnFailed2);
                        mActor.InteractionQueue.Add(instance);
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

        public interface ISaunaWooHooDefinition : IWooHooDefinition
        {
            TryingFor TryingFor
            {
                get;
            }
        }

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<Sim, CuddleSeatedWooHooSaunaEx, BaseSaunaDefinition>, ISaunaWooHooDefinition
        {
            public ProxyDefinition(BaseSaunaDefinition definition)
                : base(definition)
            { }

            public override Sim ITarget(InteractionInstance interaction)
            {
                return interaction.Target as Sim;
            }

            public TryingFor TryingFor
            {
                get { return Definition.TryingFor; }
            }
        }

        public abstract class BaseSaunaDefinition : CommonWoohoo.PotentialDefinition<SaunaClassic>, ISaunaWooHooDefinition
        {
            public BaseSaunaDefinition()
            { }
            public BaseSaunaDefinition(Sim target)
                : base(target)
            { }

            public abstract TryingFor TryingFor
            {
                get;
            }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.Sauna;
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { (SaunaClassic.LocalizeString(isFemale, "WoohooPath", new object[0x0]) + Localization.Ellipsis) };
            }

            public override Sim GetTarget(Sim actor, SaunaClassic target, InteractionInstance interaction)
            {
                List<Sim> sims = new List<Sim>(target.ActorsUsingMe);
                sims.Remove(actor);

                if (sims.Count == 1)
                {
                    if (sims[0].Posture is SittingPosture) return null;

                    return sims[0];
                }
                else
                {
                    return base.GetTarget(actor, target, interaction);
                }
            }

            public override bool JoinInProgress(Sim actor, Sim target, SaunaClassic obj, InteractionInstance interaction)
            {
                List<Sim> sims = new List<Sim>();
                sims.Add(obj.GetLeftSim());
                sims.Add(obj.GetRightSim());

                if ((sims.Contains(actor)) && (sims.Contains(target)))
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
                new PushWooHooOrTryForBaby(obj as SaunaClassic, actor, target, true, true, new ProxyDefinition(this));
            }

            protected override bool Satisfies(Sim actor, Sim target, SaunaClassic obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                return true;
            }
        }

        public class SafeSaunaDefinition : BaseSaunaDefinition
        {
            public SafeSaunaDefinition()
            { }
            public SafeSaunaDefinition(Sim target)
                : base(target)
            { }

            public override TryingFor TryingFor
            {
                get { return SaunaClassic.CuddleSeatedWooHooSauna.TryingFor.TryForNothing; }
            }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.Safe;
            }

            public override string GetInteractionName(Sim actor, SaunaClassic target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim a, Sim target, SaunaClassic obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonWoohoo.SatisfiesWoohoo(a, target, "SaunaWoohoo", isAutonomous, true, true, ref greyedOutTooltipCallback);
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                }
                return false;
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new SafeSaunaDefinition(target));
            }
        }

        public class RiskySaunaDefinition : BaseSaunaDefinition
        {
            public RiskySaunaDefinition()
            { }
            public RiskySaunaDefinition(Sim target)
                : base(target)
            { }

            public override TryingFor TryingFor
            {
                get { return SaunaClassic.CuddleSeatedWooHooSauna.TryingFor.TryForNothing; }
            }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return CommonWoohoo.WoohooStyle.Risky;
            }

            public override string GetInteractionName(Sim actor, SaunaClassic target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }

            protected override bool Satisfies(Sim a, Sim target, SaunaClassic obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonPregnancy.SatisfiesRisky(a, target, "SaunaRisky", isAutonomous, true, ref greyedOutTooltipCallback);
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                }
                return false;
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new RiskySaunaDefinition(target));
            }
        }

        public class TryForBabySaunaDefinition : BaseSaunaDefinition
        {
            TryingFor mTryingFor;

            public TryForBabySaunaDefinition(TryingFor tryingFor)
            {
                mTryingFor = tryingFor;
            }
            public TryForBabySaunaDefinition(Sim target, TryingFor tryingFor)
                : base(target)
            {
                mTryingFor = tryingFor;
            }

            public override TryingFor TryingFor
            {
                get { return mTryingFor; }
            }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return CommonWoohoo.WoohooStyle.TryForBaby;
            }

            public override string GetInteractionName(Sim actor, SaunaClassic target, InteractionObjectPair iop)
            {
                if (mTryingFor == SaunaClassic.CuddleSeatedWooHooSauna.TryingFor.TryForNothing)
                {
                    return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
                }
                else
                {
                    return SaunaClassic.LocalizeString(actor.IsFemale, mTryingFor.ToString(), new object[] { actor, target });
                }
            }

            protected override bool Satisfies(Sim a, Sim target, SaunaClassic obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonPregnancy.SatisfiesTryForBaby(a, target, "SaunaTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                }
                return false;
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new TryForBabySaunaDefinition(target, mTryingFor));
            }
        }

        public abstract class BaseDefinition : CommonWoohoo.BaseDefinition<Sim, CuddleSeatedWooHooSaunaEx>, ISaunaWooHooDefinition
        {
            public BaseDefinition()
            { }

            public override Sim GetTarget(Sim actor, Sim target, InteractionInstance interaction)
            {
                return target;
            }

            public abstract TryingFor TryingFor
            {
                get;
            }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.Sauna;
            }

            public override int Attempts
            {
                set { }
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { (SaunaClassic.LocalizeString(isFemale, "WoohooPath", new object[0x0]) + Localization.Ellipsis) };
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
                        if (!(posture.Container is SaunaClassic))
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Actor Not In Sauna");
                            return false;
                        }
                    }

                    posture = target.Posture as SittingPosture;
                    if (posture != null)
                    {
                        if (!(posture.Container is SaunaClassic))
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Target Not In Sauna");
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

            public override TryingFor TryingFor
            {
                get { return SaunaClassic.CuddleSeatedWooHooSauna.TryingFor.TryForNothing; }
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

                    return CommonWoohoo.SatisfiesWoohoo(a, target, "SaunaWoohoo", isAutonomous, true, true, ref greyedOutTooltipCallback);
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

            public override TryingFor TryingFor
            {
                get { return SaunaClassic.CuddleSeatedWooHooSauna.TryingFor.TryForNothing; }
            }

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

                    return CommonPregnancy.SatisfiesRisky(a, target, "SaunaRisky", isAutonomous, true, ref greyedOutTooltipCallback);
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                }
                return false;
            }
        }

        public class TryForBabyDefinition : BaseDefinition
        {
            TryingFor mTryingFor;

            public TryForBabyDefinition(TryingFor tryingFor)
            {
                mTryingFor = tryingFor;
            }

            public override TryingFor TryingFor
            {
                get { return mTryingFor; }
            }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return CommonWoohoo.WoohooStyle.TryForBaby;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                if (mTryingFor == SaunaClassic.CuddleSeatedWooHooSauna.TryingFor.TryForNothing)
                {
                    return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
                }
                else
                {
                    return SaunaClassic.LocalizeString(actor.IsFemale, mTryingFor.ToString(), new object[] { actor, target });
                }
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    return CommonPregnancy.SatisfiesTryForBaby(a, target, "SaunaTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
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
                get { return CommonWoohoo.WoohooLocation.Sauna; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is SaunaClassic;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<SaunaClassic>(new Predicate<SaunaClassic>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<SaunaClassic>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (sim.IsFrankenstein) return false;

                return Woohooer.Settings.mAutonomousSauna;
            }

            public bool TestUse(SaunaClassic obj)
            {
                if (!TestRepaired(obj)) return false;

                if (obj.GetLeftSim() != null) return false;

                if (obj.GetRightSim() != null) return false;

                return true;
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (SaunaClassic obj in actor.LotCurrent.GetObjects<SaunaClassic>(new Predicate<SaunaClassic>(TestUse)))
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
                        return new SafeSaunaDefinition(target);
                    case CommonWoohoo.WoohooStyle.Risky:
                        return new RiskySaunaDefinition(target);
                    case CommonWoohoo.WoohooStyle.TryForBaby:
                        return new TryForBabySaunaDefinition(target, SaunaClassic.CuddleSeatedWooHooSauna.TryingFor.TryForNothing);
                }

                return null;
            }
        }
    }
}


