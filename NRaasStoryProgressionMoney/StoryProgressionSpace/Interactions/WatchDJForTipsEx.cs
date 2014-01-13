using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.PerformanceObjects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.RouteDestinations;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class WatchDJForTipsEx : DJTurntable.WatchDJForTips, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<DJTurntable, DJTurntable.WatchDJForTips.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<DJTurntable, DJTurntable.WatchDJForTips.Definition>(Singleton);
        }

        protected static int GiveTipEx(DJTurntable ths, Sim tipper, Sim player)
        {
            if (ths.mTipJar == null)
            {
                return 0;
            }
            Route r = tipper.CreateRoute();
            RadialRangeDestination destination = new RadialRangeDestination();
            destination.mCenterPoint = ths.mTipJar.Position;
            destination.mConeVector = ths.mTipJar.ForwardVector;
            destination.mFacingPreference = RouteOrientationPreference.TowardsObject;
            destination.mfConeAngle = 3.141593f;
            destination.mfMinRadius = 1.25f;
            destination.mfPreferredSpacing = 0.1f;
            destination.mTargetObject = ths.mTipJar;
            r.AddDestination(destination);
            r.SetValidRooms(0x0L, null);
            r.ExitReasonsInterrupt &= -33;
            r.Plan();
            if (!tipper.DoRoute(r))
            {
                return 0;
            }
            tipper.PlaySoloAnimation("a2o_guitar_tip_x", true);

            int delta = 0;

            int dJLevel = ths.GetDJLevel(player);
            if (((dJLevel >= 0x0) && (dJLevel <= 0x5)) && (dJLevel >= DJTurntable.kMakeATipLevel))
            {
                delta = DJTurntable.kTipAmountPerLevel[dJLevel];
            }

            delta = (int)RandomUtil.GetDouble(delta * 0.75, delta * 1.25);
            delta++;

            NRaas.StoryProgression.Main.Money.AdjustFunds(player.SimDescription, "Tips", delta);

            NRaas.StoryProgression.Main.Money.AdjustFunds(tipper.SimDescription, "Tips", -delta);

            return delta;
        }

        protected void AttemptToGiveATipEx()
        {
            bool flag = false;
            if (Actor.HasExitReason(ExitReason.Finished))
            {
                flag = true;
                Actor.ClearExitReasons();
            }
            int income = GiveTipEx(Target, Actor, Target.DJOwnerSim);
            if (income > 0x0)
            {
                mTippingStatus = Tipping.HasTipped;
                if (Target.DJOwnerSim != null)
                {
                    EventTracker.SendEvent(new EarnTipsWithDJBoothEvent(EventTypeId.kEarnXTipsWithYourDJBooth, Target.DJOwnerSim, income));
                }
            }
            if (flag)
            {
                Actor.AddExitReason(ExitReason.Finished);
            }
        }

        protected void WatchLoopEx(StateMachineClient smc, InteractionInstance.LoopData ld)
        {
            try
            {
                int mLifeTime = (int)ld.mLifeTime;
                if ((base.Actor.IsNPC && (mTippingStatus == Tipping.NoTip)) && (mLifeTime > mGoalTimeToTestTip))
                {
                    if (RandomUtil.RandomChance01(mTipChancePerCheck))
                    {
                        mTippingStatus = Tipping.WillTip;
                    }
                    mGoalTimeToTestTip = mLifeTime + TimePerTipTest;
                }
                if (mShouldReactNow)
                {
                    ReactToPerformance();
                    mShouldReactNow = false;
                }
                if (mTippingStatus == Tipping.WillTip)
                {
                    Vector3 position = Actor.Position;

                    //Custom
                    AttemptToGiveATipEx();

                    RouteBackToViewingPosition(position);
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
                if (!RouteToWatch())
                {
                    return false;
                }
                LowerPriority();
                StandardEntry(false);
                BeginCommodityUpdates();
                EnterStateMachine("GenericWatch", "Enter", "x");
                AnimateSim("NeutralWatchLoop");
                mFriendGainAlarm = Target.AddAlarmRepeating(MinutesForLikingGain, TimeUnit.Minutes, new AlarmTimerCallback(GainFriendly), MinutesForLikingGain, TimeUnit.Minutes, "Friendship Gain for Watch Perform for Tips", AlarmType.AlwaysPersisted);
                bool succeeded = false;
                if (Target.PlayingForTips)
                {
                    mTipChancePerCheck = Target.GetChanceForATip(Target.DJOwnerSim);
                    foreach (TraitNames names in TraitsLessLikelyToTip)
                    {
                        if (Actor.TraitManager.HasElement(names))
                        {
                            mTipChancePerCheck *= LessLikelyToTipMultiplier;
                        }
                    }
                    foreach (TraitNames names2 in TraitsMoreLikelyToTip)
                    {
                        if (Actor.TraitManager.HasElement(names2))
                        {
                            mTipChancePerCheck *= MoreLikelyToTipMultiplier;
                        }
                    }
                }
                mShouldReactNow = false;
                mGoalTimeToTestTip = TimePerTipTest;
                succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), WatchLoopEx, mCurrentStateMachine);
                if (mTippingStatus == Tipping.WillTip)
                {
                    AttemptToGiveATipEx();
                }
                AnimateSim("Exit");
                EndCommodityUpdates(succeeded);
                StandardExit(false);
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

        public new class Definition : DJTurntable.WatchDJForTips.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new WatchDJForTipsEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, DJTurntable target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, DJTurntable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                if (target.DJOwnerSim == null) return false;

                return (a.Household != target.DJOwnerSim.Household);
            }
        }
    }
}
