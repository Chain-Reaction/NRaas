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
using Sims3.Gameplay.Objects.HobbiesSkills;
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
    public class WatchPerformForTipsEx : PerformanceCareer.WatchPerformForTips, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, PerformanceCareer.WatchPerformForTips.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, PerformanceCareer.WatchPerformForTips.Definition>(Singleton);
        }

        protected static bool GiveTip(PerformanceCareer.PerformerPerformForTips ths, Sim tipper, Sim player)
        {
            if (ths.mTipJar == null)
            {
                return false;
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
                return false;
            }
            tipper.PlaySoloAnimation("a2o_guitar_tip_x", true);
            PerformanceCareer occupationAsPerformanceCareer = player.OccupationAsPerformanceCareer;
            int delta = occupationAsPerformanceCareer.CareerLevel * occupationAsPerformanceCareer.Tuning.TipMoneyPerLevel;
            delta = (int)RandomUtil.GetDouble(delta * 0.75, delta * 1.25);
            delta++;

            NRaas.StoryProgression.Main.Money.AdjustFunds(player.SimDescription, "Tips", delta);

            int original = occupationAsPerformanceCareer.mTipsCurrentGig;

            occupationAsPerformanceCareer.MadeTips(delta, false);

            delta = occupationAsPerformanceCareer.mTipsCurrentGig - original;

            NRaas.StoryProgression.Main.Money.AdjustFunds(tipper.SimDescription, "Tips", -delta);

            return true;
        }

        protected void AttemptToGiveATipEx()
        {
            PerformanceCareer.PerformerPerformForTips currentInteraction = Target.CurrentInteraction as PerformanceCareer.PerformerPerformForTips;
            if (currentInteraction != null)
            {
                bool flag = false;
                if (Actor.HasExitReason(ExitReason.Finished))
                {
                    flag = true;
                    Actor.ClearExitReasons();
                }

                if (GiveTip(currentInteraction, Actor, Target))
                {
                    mTippingStatus = Tipping.HasTipped;
                }

                if (flag)
                {
                    Actor.AddExitReason(ExitReason.Finished);
                }
            }
        }

        protected void WatchLoopEx(StateMachineClient smc, InteractionInstance.LoopData ld)
        {
            try
            {
                PerformanceCareer occupationAsPerformanceCareer = Target.OccupationAsPerformanceCareer;
                if (occupationAsPerformanceCareer == null) return;

                int mLifeTime = (int)ld.mLifeTime;
                if ((Actor.IsNPC && (mTippingStatus == Tipping.NoTip)) && (mLifeTime > mGoalTimeToTestTip))
                {
                    if (RandomUtil.RandomChance01(mTipChancePerCheck))
                    {
                        mTippingStatus = Tipping.WillTip;
                    }
                    mGoalTimeToTestTip = mLifeTime + occupationAsPerformanceCareer.Tuning.TimePerTipTest;
                }

                if (mShouldReactNow)
                {
                    ReactToPerformance();
                    mShouldReactNow = false;
                }

                if (mItsAGoodTimeToTip)
                {
                    if (mTippingStatus == Tipping.WillTip)
                    {
                        Vector3 position = Actor.Position;

                        // Custom
                        AttemptToGiveATipEx();

                        RouteBackToViewingPosition(position);
                    }
                    mItsAGoodTimeToTip = false;
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
                PerformanceCareer occupationAsPerformanceCareer = Target.OccupationAsPerformanceCareer;
                if (occupationAsPerformanceCareer != null)
                {
                    if (!RouteToWatch())
                    {
                        return false;
                    }
                    if (!occupationAsPerformanceCareer.PlayingForTips)
                    {
                        return false;
                    }
                    PerformanceCareer.PerformerPerformForTips currentInteraction = Target.CurrentInteraction as PerformanceCareer.PerformerPerformForTips;
                    if (currentInteraction == null)
                    {
                        return false;
                    }
                    LowerPriority();
                    if (currentInteraction.IsPerforming())
                    {
                        AddEventListeners();
                        StandardEntry(false);
                        BeginCommodityUpdates();
                        EnterStateMachine("GenericWatch", "Enter", "x");
                        AnimateSim("NeutralWatchLoop");
                        mFriendGainAlarm = Target.AddAlarmRepeating(occupationAsPerformanceCareer.Tuning.MinutesForLikingGain, TimeUnit.Minutes, new AlarmTimerCallback(GainFriendly), occupationAsPerformanceCareer.Tuning.MinutesForLikingGain, TimeUnit.Minutes, "Friendship Gain for Watch Perform for Tips", AlarmType.AlwaysPersisted);
                        bool succeeded = false;
                        mTipChancePerCheck = occupationAsPerformanceCareer.Tuning.ChanceOfTipPerCareerLevel[occupationAsPerformanceCareer.CareerLevel];
                        
                        foreach (TraitNames names in occupationAsPerformanceCareer.Tuning.TraitsLessLikelyToTip)
                        {
                            if (Actor.TraitManager.HasElement(names))
                            {
                                mTipChancePerCheck *= occupationAsPerformanceCareer.Tuning.LessLikelyToTipMultiplier;
                            }
                        }

                        foreach (TraitNames names2 in occupationAsPerformanceCareer.Tuning.TraitsMoreLikelyToTip)
                        {
                            if (Actor.TraitManager.HasElement(names2))
                            {
                                mTipChancePerCheck *= occupationAsPerformanceCareer.Tuning.MoreLikelyToTipMultiplier;
                            }
                        }

                        mShouldReactNow = false;
                        mItsAGoodTimeToTip = false;
                        mGoalTimeToTestTip = occupationAsPerformanceCareer.Tuning.TimePerTipTest;
                        succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), WatchLoopEx, mCurrentStateMachine);
                        if (mTippingStatus == Tipping.WillTip)
                        {
                            // Custom
                            AttemptToGiveATipEx();
                        }
                        AnimateSim("Exit");
                        EndCommodityUpdates(succeeded);
                        StandardExit(false);
                        return succeeded;
                    }
                }
                return false;
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

        public new class Definition : PerformanceCareer.WatchPerformForTips.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new WatchPerformForTipsEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return (a.Household != target.Household);
            }
        }
    }
}
