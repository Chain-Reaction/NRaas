using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    [Persistable]
    public class FishHereEx : FishHere, Common.IPreLoad, Common.IAddInteraction
    {
        public void OnPreLoad()
        {
            Tunings.Inject<Terrain, Sims3.Gameplay.Objects.Fishing.FishHere.Definition, Definition>(true);

            InteractionTuning tuning = Tunings.GetTuning<Terrain, Sims3.Gameplay.Objects.Fishing.FishHere.Definition>();
            if (tuning != null)
            {
                tuning.SetFlags(InteractionTuning.FlagField.DisallowAutonomous, true);
                tuning.SetFlags(InteractionTuning.FlagField.DisallowUserDirected, true);
            }

            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Terrain, FishHere.Definition>(Singleton);
        }

        private new void LoopFunc(StateMachineClient smc, Interaction<Sim, Terrain>.LoopData ld)
        {
            try
            {
                WaterTypes waterType = FishingSpot.GetWaterType(Hit.mType);
                EventTracker.SendEvent(new FishingLoopEvent(EventTypeId.kWentFishing, Actor, waterType, ld.mLifeTime));
                Actor.TryGroupTalk();
                //Actor.TrySinging();
                Fishing skill = Actor.SkillManager.GetSkill<Fishing>(SkillNames.Fishing);
                if (mShowTns && (ld.mLifeTime > kTimeToShowFishingTns))
                {
                    string str;
                    if (BaitInUse != null)
                    {
                        str = Common.LocalizeEAString("Gameplay/Objects/Fishing:NoFishWithBait");
                    }
                    else
                    {
                        str = Common.LocalizeEAString("Gameplay/Objects/Fishing:NoFishNoBait");
                    }
                    Actor.ShowTNSIfSelectable(str, StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid);
                    if ((mLoopLengthForNextFish - ld.mLifeTime) < kTimeToCatchPostTns)
                    {
                        mLoopLengthForNextFish = ld.mLifeTime + kTimeToCatchPostTns;
                    }
                    mShowTns = false;
                }
                
                if (ld.mLifeTime > mLoopLengthForNextFish)
                {
                    mLoopLengthForNextFish += mFishingData.GetNextFishTimeLength(mIsAngler, skill.IsFisherman());
                    FishType none = FishType.None;
                    string baitUsed = null;
                    if (BaitInUse != null)
                    {
                        baitUsed = BaitInUse.Key;
                    }

                    if (!mShowTns)
                    {
                        none = mFishingData.GetFishCaught(Actor, baitUsed);
                    }

                    if ((mSittingInBoatPosture != null) && (none == FishType.Box))
                    {
                        none = FishType.None;
                    }

                    FishType type3 = none;
                    if (type3 == FishType.None)
                    {
                        bool flag = false;
                        if (Actor.TraitManager.HasElement(TraitNames.Clumsy))
                        {
                            flag = RandomUtil.RandomChance01(kChanceOfPlayingLostFishAnimIfClumsy);
                        }
                        else
                        {
                            flag = RandomUtil.RandomChance01(kChanceOfPlayingLostFishAnim);
                        }
                        if (!flag)
                        {
                            return;
                        }
                        AnimateSim("CatchNothing");
                    }
                    else if (type3 == FishType.Box)
                    {
                        int num = 0x0;
                        WorldType currentWorldType = GameUtils.GetCurrentWorldType();
                        WorldName currentWorld = GameUtils.GetCurrentWorld();
                        List<int> list = new List<int>();
                        for (int i = 0x0; i < sBoxData.Count; i++)
                        {
                            BoxData data = sBoxData[i];
                            if (((data.AllowedWorldType == WorldType.Undefined) || (data.AllowedWorldType == currentWorldType)) && (((data.AllowedWorlds == null) || (data.AllowedWorlds.Count == 0x0)) || (data.AllowedWorlds.Contains(currentWorld) || data.AllowedWorlds.Contains(WorldName.Undefined))))
                            {
                                num += sBoxChances[i];
                                list.Add(i);
                            }
                        }
                        int chance = RandomUtil.GetInt(num - 0x1);
                        int num4 = 0x0;
                        foreach (int num5 in list)
                        {
                            num4 = num5;
                            if (chance < sBoxChances[num5])
                            {
                                break;
                            }
                            chance -= sBoxChances[num5];
                        }
                        IGameObject obj2 = GlobalFunctions.CreateObject(sBoxData[num4].ItemName, Vector3.OutOfWorld, 0x1, Vector3.UnitZ);
                        if (obj2 == null)
                        {
                            return;
                        }
                        bool happy = sBoxData[num4].Happy;
                        if (Actor.TraitManager.HasElement(TraitNames.Insane))
                        {
                            happy = !happy;
                        }
                        SetSplashAndDripEffects(EffectSize.Small);
                        if (happy)
                        {
                            AnimateSim("CatchBoxHappy");
                            Actor.Inventory.TryToAdd(obj2, false);
                        }
                        else
                        {
                            AnimateSim("CatchBoxSad");
                            Actor.Inventory.TryToAdd(obj2, false);
                        }
                        Actor.ShowTNSIfSelectable(Common.LocalizeEAString(Actor.IsFemale, "Gameplay/Objects/Fishing:CaughtBox", new object[] { Actor, obj2 }), StyledNotification.NotificationStyle.kGameMessagePositive, obj2.ObjectId, Actor.ObjectId);
                    }
                    else
                    {
                        int num6;
                        string str3;
                        Fish actor = Fish.CreateFish(none, Actor, skill, BaitInUse, out num6, out str3);
                        actor.UpdateVisualState(CatHuntingComponent.CatHuntingModelState.InInventory);
                        mNumberFishCaught++;
                        SetActor("fish", actor);
                        SetSplashAndDripEffects(actor.EffectSize);
                        PlayFishCatchAnimation(num6);
                        if (str3 != null)
                        {
                            Actor.ShowTNSIfSelectable(str3, StyledNotification.NotificationStyle.kGameMessagePositive, actor.ObjectId, Actor.ObjectId);
                        }

                        EventTracker.SendEvent(new CaughtFishEvent(EventTypeId.kCaughtFish, Actor, actor, baitUsed));
                        Actor.Inventory.TryToAdd(actor,false);
                        if ((BaitInUse != null) && RandomUtil.RandomChance(kPercentChanceBaitLost))
                        {
                            Actor.Inventory.SetNotInUse(BaitInUse);
                            Actor.Inventory.RemoveByForce(BaitInUse);
                            IFishBait baitInUse = BaitInUse;
                            BaitInUse = FindProperBait(new Fishing.BaitInfo(baitInUse));
                            if (BaitInUse == null)
                            {
                                ThoughtBalloonManager.BalloonData bd = new ThoughtBalloonManager.BalloonData(baitInUse.GetThoughtBalloonThumbnailKey());
                                bd.mCoolDown = ThoughtBalloonCooldown.Medium;
                                bd.LowAxis = ThoughtBalloonAxis.kDislike;
                                Actor.ThoughtBalloonManager.ShowBalloon(bd);
                                Actor.InteractionQueue.FireQueueChanged();
                                string message = Common.LocalizeEAString(Actor.IsFemale, "Gameplay/Objects/Fishing:NoMoreBait", new object[] { Actor, baitInUse.GetLocalizedName() });
                                Actor.ShowTNSIfSelectable(message, StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, Actor.ObjectId);
                            }
                            baitInUse.Destroy();
                        }
                    }

                    if (!ActiveStage.IsComplete((InteractionInstance) this))
                    {
                        mCurrentStateMachine.SetParameter("skillLevel", skill.GetSkillLevelParameterForJazzGraph());
                        AnimateSim("Fish");
                    }
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.DebugException(Actor, Target, e);
            }
        }

        public override bool Run()
        {
            try
            {
                Vector3 vector4;
                if (Actor.LotCurrent.IsHouseboatLot())
                {
                    mIsHouseboat = true;
                }
                mSittingInBoatPosture = Actor.Posture as SittingInBoat;

                Vector3 mPoint = Hit.mPoint;
                Vector3 newTargetPos = mPoint;
                if (mSittingInBoatPosture == null)
                {
                    if (!mIsHouseboat)
                    {
                        if (IsNearHiddenIsland(mPoint))
                        {
                            Vector3 invalid = Vector3.Invalid;
                            World.GetNearestSwimmingPoint(Actor.Position, ref invalid, ref mPoint);
                        }

                        if (!RouteToFishingLocation(mPoint, Actor, Hit.mType, Hit.mId))
                        {
                            Actor.PlayRouteFailure(new ThumbnailKey(ResourceKey.CreatePNGKey("w_fishing_skill", ResourceUtils.ProductVersionToGroupId(ProductVersion.BaseGame)), ThumbnailSize.Medium));
                            return false;
                        }
                    }
                    else if (!RouteToFishingLocationOnHouseboat(Actor, ref mPoint, Actor, Hit.mType, Hit.mId, ref newTargetPos))
                    {
                        return false;
                    }
                }
                else
                {
                    Boat container = mSittingInBoatPosture.Container as Boat;
                    if (container != null)
                    {
                        mPoint = container.GetFishingPosition(mSittingInBoatPosture);
                        if (World.GetTerrainType(mPoint.x, mPoint.z, container.Level) != 2)
                        {
                            return false;
                        }

                        RoutingComponent routingComponent = container.RoutingComponent;
                        if ((routingComponent == null) || routingComponent.IsRouting)
                        {
                            return false;
                        }
                    }
                }

                if (mIsHouseboat && World.GetHouseboatDisplayToWorldPosition(Actor.LotCurrent.LotId, newTargetPos, out vector4))
                {
                    PrepFishing(mPoint, vector4);
                }
                else
                {
                    PrepFishing(mPoint, mPoint);
                }

                WaterTypes waterType = FishingSpot.GetWaterType(Hit.mType);
                Fishing fishing = Actor.SkillManager.AddElement(SkillNames.Fishing) as Fishing;
                fishing.TypesOfWaterFishedIn = waterType | fishing.TypesOfWaterFishedIn;
                if (!BaitSelected)
                {
                    BaitInfoSelected = fishing.mBaitInfo;
                }
                if (BaitInfoSelected != null)
                {
                    BaitInUse = FindProperBait(BaitInfoSelected);
                    Actor.InteractionQueue.FireQueueChanged();
                }
                else if (fishing.SkillLevel > kNoBaitUsedLesson)
                {
                    Tutorialette.TriggerLesson(Lessons.FishingSkill, Actor);
                }

                BeginCommodityUpdates();

                bool succeeded = false;

                MotiveDelta delta = null;

                try
                {
                    StartStages();
                    if (Actor.TraitManager.HasElement(TraitNames.Angler))
                    {
                        mIsAngler = true;
                        delta = AddMotiveDelta(CommodityKind.Fun, kAnglerFunPerHour);
                    }

                    mLoopLengthForNextFish = mFishingData.GetNextFishTimeLength(mIsAngler, fishing.IsFisherman());
                    mShowTns = mFishingData.ShouldShowFishingTns(Actor, BaitInUse);
                    mCurrentStateMachine.SetParameter("skillLevel", fishing.GetSkillLevelParameterForJazzGraph());
                    if (BaitInUse != null)
                    {
                        fishing.mBaitInfo = new Fishing.BaitInfo(BaitInUse);
                    }
                    else
                    {
                        fishing.mBaitInfo = null;
                    }

                    AnimateSim("Fish");
                    Actor.RegisterGroupTalk();

                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new Interaction<Sim, Terrain>.InsideLoopFunction(LoopFunc), mCurrentStateMachine);

                    Actor.UnregisterGroupTalk();
                    if (BaitInUse != null)
                    {
                        Actor.Inventory.SetNotInUse(BaitInUse);
                    }

                    if (mSittingInBoatPosture == null)
                    {
                        AnimateSim("Exit");
                    }
                    else
                    {
                        AnimateSim("ExitFishing");
                        mSittingInBoatPosture.CurrentStateMachine.RequestState("x", "SimIdle");
                    }
                }
                finally
                {
                    StandardExit(false, false);
                    if (mIsAngler)
                    {
                        RemoveMotiveDelta(delta);
                    }

                    EndCommodityUpdates(succeeded);
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

        public new class Definition : FishHere.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new FishHereEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}

