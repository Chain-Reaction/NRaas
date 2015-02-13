using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.RouteDestinations;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public abstract class MusicalInstrumentWatchBase<TTarget> : MusicalInstrument.WatchBase<TTarget>, Common.IPreLoad, Common.IAddInteraction
        where TTarget: MusicalInstrument
    {
        public abstract void OnPreLoad();

        public abstract void AddInteraction(Common.InteractionInjectorList interactions);

        protected static bool GiveTip(BandInstrument ths, Sim tipper, Sim player, int moneyPerLevel, int moneyPerComposition)
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
            BandSkill skill = player.SkillManager.GetSkill<BandSkill>(ths.SkillName);
            int delta = skill.SkillLevel * moneyPerLevel;
            delta += skill.NumberCompositionsPlayed() * moneyPerComposition;
            delta = (int)RandomUtil.GetDouble(delta * 0.75, delta * 1.25);
            delta++;

            RockBand.GiveXpForMoney(player, delta);

            NRaas.StoryProgression.Main.Money.AdjustFunds(player.SimDescription, "Tips", delta);

            int original = skill.mMoneyMadeFromGuitarPlaying;

            skill.MadeTips(delta, false);

            delta = skill.mMoneyMadeFromGuitarPlaying - original;

            NRaas.StoryProgression.Main.Money.AdjustFunds(tipper.SimDescription, "Tips", -delta);

            return true;
        }
        protected static bool GiveTip(SnakeCharmingBasket ths, Sim tipper, Sim player, int moneyPerLevel, int moneyPerComposition)
        {
            if (!ths.CanTip(player)) return false;

            Route r = tipper.CreateRoute();
            RadialRangeDestination destination = new RadialRangeDestination();
            destination.mCenterPoint = ths.Position;
            destination.mConeVector = ths.ForwardVector;
            destination.mFacingPreference = RouteOrientationPreference.TowardsObject;
            destination.mfConeAngle = 3.141593f;
            destination.mfMinRadius = 1.25f;
            destination.mfPreferredSpacing = 0.1f;
            destination.mTargetObject = ths;
            r.AddDestination(destination);
            r.SetValidRooms(0x0L, null);
            r.ExitReasonsInterrupt &= -33;
            r.Plan();
            if (tipper.DoRoute(r))
            {
                int delta = player.SkillManager.GetSkillLevel(SkillNames.SnakeCharming) * moneyPerLevel;
                delta = (int)RandomUtil.GetDouble(delta * 0.75, delta * 1.25);
                delta++;

                NRaas.StoryProgression.Main.Money.AdjustFunds(tipper.SimDescription, "Tips", -delta);

                NRaas.StoryProgression.Main.Money.AdjustFunds(player.SimDescription, "Tips", delta);

                tipper.PlaySoloAnimation("a2o_guitar_tip_x", true);
                SnakeCharmingBasket.CharmForTips currentInteraction = player.CurrentInteraction as SnakeCharmingBasket.CharmForTips;
                if (currentInteraction != null)
                {
                    currentInteraction.MoneyEarnedThisSession += delta;
                }
                return true;
            }
            return false;
        }

        public static bool Tip(Sim actor, Sim player, MusicalInstrument target, int moneyPerLevel, int moneyPerComposition)
        {
            int originalActorFunds = actor.FamilyFunds;
            if (originalActorFunds <= 0) return false;

            int originalTargetFunds = player.FamilyFunds;

            if (target is BandInstrument)
            {
                return GiveTip(target as BandInstrument, actor, player, moneyPerLevel, moneyPerComposition);
            }
            else if (target is SnakeCharmingBasket)
            {
                return GiveTip(target as SnakeCharmingBasket, actor, player, moneyPerLevel, moneyPerComposition);
            }

            return false;
        }

        private new void ExitAndTip(bool keepWatching)
        {
            try
            {
                AnimateSim("AfterWatch");
                Vector3 position = Actor.Position;
                if ((mTippingStatus == Tipping.WillTip) && (mPlayInstance != null))
                {
                    bool flag = false;
                    if (Actor.ExitReason == ExitReason.Finished)
                    {
                        flag = true;
                        Actor.ClearExitReasons();
                    }

                    if (Tip(Actor, mPlayer, Target, Tuning.MoneyPerLevel, Tuning.MoneyPerCompositionPlayed))
                    {
                        mTippingStatus = Tipping.HasTipped;
                    }

                    if (flag)
                    {
                        Actor.AddExitReason(ExitReason.Finished);
                    }
                }

                if (keepWatching)
                {
                    Route r = Actor.CreateRoute();
                    r.PlanToPoint(position);
                    r.DoRouteFail = false;
                    if (!Actor.DoRoute(r))
                    {
                        if (mDanceFloor != null)
                        {
                            mDanceFloor.RemoveFromUseList(Actor);
                            mDanceFloor = null;
                        }
                        DanceFloor danceFloor = null;
                        MusicalInstrument.DoRouteForWatchOrDance(Actor, Target, out danceFloor);
                        mDanceFloor = danceFloor;
                        if (mDanceFloor != null)
                        {
                            mDanceFloor.AddToUseList(Actor);
                        }
                    }
                    Actor.RouteTurnToFace(mPlayer.Position);
                    AnimateSim("Watch");
                    mbIsWatchingRightNow = true;
                }
                else
                {
                    AnimateSim("Exit");
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

        private new void SongFinished(bool stopWatching, bool ableToTip)
        {
            try
            {
                mbIsWatchingRightNow = false;
                if (stopWatching)
                {
                    base.SongFinished(stopWatching, ableToTip);
                }
                else
                {
                    if (mTippingStatus == Tipping.NoTip)
                    {
                        if (RandomUtil.CoinFlip() || ((mPlayer.Occupation is Music) && (mPlayer.Occupation.Level >= Music.GuitarBonusLevel)))
                        {
                            AnimateSim("FinishHappy");
                        }
                        else
                        {
                            AnimateSim("FinishSad");
                        }
                    }
                    else if (Actor.TraitManager.HasElement(TraitNames.Insane))
                    {
                        AnimateSim("FinishSad");
                    }
                    else
                    {
                        AnimateSim("FinishHappy");
                    }

                    ExitAndTip(true);

                    mTippingStatus = Tipping.NoTip;
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

        private new void TippingLoop(StateMachineClient smc, Interaction<Sim, TTarget>.LoopData ld)
        {
            try
            {
                WatchLoopBase(smc, ld);
                if (Actor.IsNPC)
                {
                    int mLifeTime = (int)ld.mLifeTime;
                    if ((mTippingStatus == Tipping.NoTip) && (mLifeTime > mGoalTimeToTestTip))
                    {
                        if (RandomUtil.RandomChance01(mTipChancePerCheck))
                        {
                            mTippingStatus = Tipping.WillTip;
                        }
                        mGoalTimeToTestTip = mLifeTime + Tuning.TimePerTipTest;
                    }
                    if (timeTillTip < mLifeTime)
                    {
                        SongFinished(false, true);
                        timeTillTip = mLifeTime + RandomUtil.RandomFloatGaussianDistribution(Target.Tuning.ShortestSongLengthForTip, Target.Tuning.LongestSongLengthForTip);
                    }
                }
                else if (mTippingStatus == Tipping.WillTip)
                {
                    SongFinished(false, true);
                    mTippingStatus = Tipping.HasTipped;
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
                mPlayer = Target.ActorsUsingMe[0x0];
                bool flag = (mPlayer.Occupation is Music) && (mPlayer.Occupation.Level >= Music.GuitarBonusLevel);
                DanceFloor danceFloor = null;
                if (!MusicalInstrument.DoRouteForWatchOrDance(Actor, Target, out danceFloor))
                {
                    return false;
                }
                mDanceFloor = danceFloor;
                if (mDanceFloor != null)
                {
                    mDanceFloor.AddToUseList(Actor);
                }
                mPlayInstance = mPlayer.CurrentInteraction as MusicalInstrument.PlayInstrument<TTarget>;
                if (mPlayInstance != null)
                {
                    if (!mPlayInstance.Performing)
                    {
                        return false;
                    }
                    AddEventListeners();
                }
                if (flag)
                {
                    BeginCommodityUpdate(CommodityKind.Fun, Music.GuitarBonusFunMultiplier);
                }
                BeginCommodityUpdates();

                bool success = false;

                try
                {
                    AcquireStateMachine(Target.StateMachineName, AnimationPriority.kAPNormal);
                    SetActorAndEnter("x", Actor, "Enter");
                    AddJamControllerWatchListener();
                    AnimateSim("Watch");
                    mbIsWatchingRightNow = true;
                    Target.AddWatcher();
                    mFriendGainAlarm = Target.AddAlarmRepeating(Tuning.MinutesForLikingGain, TimeUnit.Minutes, new AlarmTimerCallback(GainFriendly), Tuning.MinutesForLikingGain, TimeUnit.Minutes, "Friendship Gain for Watch Music Playing", AlarmType.AlwaysPersisted);
                    MusicSkill element = mPlayer.SkillManager.GetElement(Target.SkillName) as MusicSkill;
                    if (element != null)
                    {
                        element.BeingWatched();
                    }
                    if (Target.mPlayingForTips && (Actor.IsOutside || Actor.LotCurrent.IsCommunityLot))
                    {
                        mTipChancePerCheck = Tuning.ChanceOfTipPerLevel[element.SkillLevel];
                        foreach (TraitNames names in Tuning.TraitsLessLikelyToTip)
                        {
                            if (Actor.TraitManager.HasElement(names))
                            {
                                mTipChancePerCheck *= Tuning.LessLikelyToTipMultiplier;
                            }
                        }
                        foreach (TraitNames names2 in Tuning.TraitsMoreLikelyToTip)
                        {
                            if (Actor.TraitManager.HasElement(names2))
                            {
                                mTipChancePerCheck *= Tuning.MoreLikelyToTipMultiplier;
                            }
                        }
                        mGoalTimeToTestTip = Tuning.TimePerTipTest;
                        timeTillTip = RandomUtil.RandomFloatGaussianDistribution(Target.Tuning.ShortestSongLengthForTip, Target.Tuning.LongestSongLengthForTip);
                        success = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new Interaction<Sim, TTarget>.InsideLoopFunction(TippingLoop), mCurrentStateMachine);
                    }
                    else
                    {
                        success = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new Interaction<Sim, TTarget>.InsideLoopFunction(WatchLoopBase), mCurrentStateMachine);
                    }
                    Target.RemoveAlarm(mFriendGainAlarm);
                    Target.RemoveWatcher();
                    SongFinished(true, mTryToTip);
                    RemoveJamControllerWatchListener();
                    if (mPlayInstance != null)
                    {
                        RemoveEventListeners();
                        mPlayInstance = null;
                    }
                }
                finally
                {
                    EndCommodityUpdates(success);
                }

                return success;
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

        protected new abstract class WatchDefinition<T> : InteractionDefinition<Sim, MusicalInstrument, T> 
            where T : MusicalInstrument.WatchBase<TTarget>, new()
        {
            // Methods
            public override bool Test(Sim a, MusicalInstrument target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (target.ActorsUsingMe.Count == 0x0)
                    {
                        return false;
                    }
                    Sim sim = target.ActorsUsingMe[0x0];
                    Relationship relationship = Relationship.Get(a, sim, false);
                    if ((relationship != null) && (relationship.LTR.CurrentLTR == LongTermRelationshipTypes.Enemy))
                    {
                        return false;
                    }
                    if (target.ActorsUsingMe.Contains(a))
                    {
                        return false;
                    }

                    return true;

                    //return CelebrityManager.CanSocialize(a, sim);
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.Exception(a, target, e);
                    return false;
                }
            }
        }
    }
}
