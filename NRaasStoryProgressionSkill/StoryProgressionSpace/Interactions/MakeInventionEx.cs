using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Objects.Toys;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class MakeInventionEx : InventionWorkbench.MakeInvention, Common.IPreLoad
    {
        public static readonly new InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<InventionWorkbench, InventionWorkbench.MakeInvention.Definition, Definition>(false);
        }

        public override bool Run()
        {
            try
            {
                if (Target == null)
                {
                    return false;
                }

                if (!Target.RouteToWorkbench(Actor))
                {
                    return false;
                }

                mInventSkill = Actor.SkillManager.AddElement(SkillNames.Inventing) as InventingSkill;

                bool flag = Target.mInventionProgress > 0f;

                Definition interactionDefinition = InteractionDefinition as Definition;

                mMakeMany = true;
                if (flag)
                {
                    string mInventionKey = Target.mInventionKey;

                    if (!InventingSkill.kInventionDataMap.TryGetValue(mInventionKey, out mCurData))
                    {
                        return false;
                    }
                    if (Target.mDummyModel == null)
                    {
                        Target.ScrapCurrentInvention();
                        return false;
                    }
                }
                else
                {
                    if (Target.mDummyModel != null)
                    {
                        Target.CleanupDummyModel();
                    }

                    List<string> choices = new List<string>(mInventSkill.KnownInventions);

                    bool success = false;
                    while (choices.Count > 0)
                    {
                        string choice = RandomUtil.GetRandomObjectFromList(choices);
                        choices.Remove(choice);

                        if (InventingSkill.kInventionDataMap.TryGetValue(choice, out mCurData))
                        {
                            if (mCurData.InventType == InventionType.Toy)
                            {
                                mIsCheapToy = RandomUtil.CoinFlip();
                            }

                            if (!Target.ReserveScrap(mCurData.GetScrapCost(Actor, mIsCheapToy), Actor, mScrapList))
                            {
                                if (mIsCheapToy) continue;

                                mIsCheapToy = true;
                                if (!Target.ReserveScrap(mCurData.GetScrapCost(Actor, mIsCheapToy), Actor, mScrapList))
                                {
                                    continue;
                                }
                            }

                            success = true;
                            break;
                        }
                    }

                    if (!success) return false;

                    Target.mInventionKey = mCurData.MedatorName;
                    Target.mInventionProgress = 0f;
                    Target.mIsMakingCheapToy = mIsCheapToy;
                    if (!Target.ConsumeScrap(mScrapList))
                    {
                        ShowOutOfScrapTNS();
                        return false;
                    }
                }
                if (Target.mWasFinishedByGnome)
                {
                    ShowGnomeFinishedInventionTNS();
                }
                StandardEntry();
                StartStages();
                mTimeUntilModelSwap = GetTimeForNextModelChange();
                EnterStateMachine("WorkbenchInvention", "Enter", "x", "workstation");
                if (Actor.SimDescription.Child)
                {
                    mStool = GlobalFunctions.CreateObjectOutOfWorld("ChildStool") as GameObject;
                    SetActor("stool", mStool);
                }
                SetParameter("shouldSwipe", !flag);
                SetParameter("skillLevel", InventionWorkbench.GetSkillLevelParam(mInventSkill));
                if (!flag)
                {
                    AddSynchronousOneShotScriptEventHandler(0x67, new SacsEventHandler(OnAnimationEvent));
                }
                mTotalTime = GetTimeToCompletion();
                AnimateSim("Loop Invent");
                BeginCommodityUpdates();

                bool succeeded = false;
                try
                {
                    if (Actor.SimDescription.TeenOrAbove)
                    {
                        Actor.SkillManager.StartGainWithoutSkillMeter(SkillNames.Handiness, InventionWorkbench.kHandinessSkillGainRateDuringMake, true);
                    }

                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new Interaction<Sim, InventionWorkbench>.InsideLoopFunction(MakeLoopCallback), mCurrentStateMachine);
                }
                finally
                {
                    if (Actor.SimDescription.TeenOrAbove)
                    {
                        Actor.SkillManager.StopSkillGain(SkillNames.Handiness);
                    }

                    EndCommodityUpdates(succeeded);
                }

                if ((Target.mInventionProgress == 0f) && (Target.mDummyModel != null))
                {
                    Target.mDummyModel.FadeOut(false, true);
                    Target.mDummyModel = null;
                }

                AnimateSim("Exit");
                StandardExit();
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

        public new void MakeLoopCallback(StateMachineClient smc, Interaction<Sim, InventionWorkbench>.LoopData ld)
        {
            try
            {
                Target.mInventionProgress += ld.mDeltaTime / mTotalTime;
                if (Target.mInventionProgress >= 1f)
                {
                    AddSynchronousOneShotScriptEventHandler(0x65, new SacsEventHandler(OnAnimationEvent));
                    AnimateSim("Swipe");
                    if (mMakeMany)
                    {
                        if (!Target.ConsumeScrap(mCurData.GetScrapCost(Actor, mIsCheapToy), Actor))
                        {
                            ShowOutOfScrapTNS();
                            Actor.AddExitReason(ExitReason.Finished);
                        }
                        else
                        {
                            AddSynchronousOneShotScriptEventHandler(0x67, new SacsEventHandler(OnAnimationEvent));
                            SetParameter("skillLevel", InventionWorkbench.GetSkillLevelParam(mInventSkill));
                            AnimateSim("Loop Invent");
                            mFinalModelShown = false;
                            mTimeUntilModelSwap = GetTimeForNextModelChange();
                        }
                    }
                    else
                    {
                        Actor.AddExitReason(ExitReason.Finished);
                    }
                }
                else if (!mFinalModelShown)
                {
                    if (Target.mInventionProgress >= InventionWorkbench.kShowFinalModelPercent)
                    {
                        string miniModelName = mCurData.MiniModelName;
                        ProductVersion prodVersion = mCurData.ProdVersion;
                        Slot finalModelSlot = mCurData.FinalModelSlot;

                        if (Target.mDummyModel == null)
                        {
                            Actor.AddExitReason(ExitReason.Finished);
                        }
                        else
                        {
                            Target.mDummyModel.UnParent();
                            Target.mDummyModel.ParentToSlot(Target, finalModelSlot);
                            Target.mDummyModel.SetModel(miniModelName, prodVersion);
                            mFinalModelShown = true;
                        }
                    }
                    else
                    {
                        mTimeUntilModelSwap -= ld.mDeltaTime;
                        if (mTimeUntilModelSwap <= 0f)
                        {
                            Target.SwapToRandomDummyGeoState(false);
                            mTimeUntilModelSwap = GetTimeForNextModelChange();
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

        private new void OnAnimationEvent(StateMachineClient smc, IEvent evt)
        {
            try
            {
                if (evt.EventId == 0x65)
                {
                    if (Target.mDummyModel != null)
                    {
                        Target.mDummyModel.UnParent();
                        Target.mDummyModel.Destroy();
                    }

                    Target.mDummyModel = null;
                    Target.mInventionProgress = 0f;
                    Target.mWasFinishedByGnome = false;

                    GameObject invention = null;
                    CreateInventionAndAddToInventory(mCurData, Actor, mIsCheapToy, out invention);
                    if (invention != null)
                    {
                        Actor.ShowTNSIfSelectable(TNSNames.inventDiscoveredSomething, Actor, invention, null, Actor.IsFemale, Actor.IsFemale, new object[] { Actor.SimDescription, invention.CatalogName });
                    }
                }
                else if (evt.EventId == 0x67)
                {
                    Target.mDummyModel = GlobalFunctions.CreateObject("genericInvention", ProductVersion.EP2, Vector3.OutOfWorld, 0x0, Vector3.UnitZ, null, null) as GameObject;
                    Target.mDummyModel.ParentToSlot(Target, Slot.ContainmentSlot_0);
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

        private static void CreateInventionAndAddToInventory(InventionData data, Sim Actor, bool isCheapToy, out GameObject invention)
        {
            Simulator.ObjectInitParameters initData = null;
            InventingSkill skill = Actor.SkillManager.GetSkill<InventingSkill>(SkillNames.Inventing);
            bool isImprovedWidget = false;
            if (data.InventType == InventionType.Widget)
            {
                Widget.Awesomeness standard = Widget.Awesomeness.Standard;
                if (skill.OppMasterInventorCompleted)
                {
                    standard = Widget.Awesomeness.Masterful;
                }
                else if (skill.OppWidgetWonderCompleted)
                {
                    standard = Widget.Awesomeness.Improved;
                }
                else
                {
                    float chance = 0f;
                    if (skill.SkillLevel >= InventionWorkbench.kLevelCanMakeImprovedWidget)
                    {
                        chance += InventionWorkbench.kChanceOfImprovedWidget;
                        if (Actor.HasTrait(TraitNames.Eccentric))
                        {
                            chance += InventionWorkbench.kChanceOfImprovedWidgetEccentric;
                        }
                        if (RandomUtil.RandomChance01(chance))
                        {
                            standard = Widget.Awesomeness.Improved;
                        }
                    }
                }
                isImprovedWidget = standard == Widget.Awesomeness.Improved;
                initData = new WidgetInitParameters(standard, Actor.SimDescription, data.InventType, data.MedatorName);
            }

            if ((data.InventType == InventionType.Invention) || (data.InventType == InventionType.Toy))
            {
                initData = new CraftedToyInitParameters(skill.OppMasterInventorCompleted, Actor.SimDescription, data.InventType, isCheapToy);
            }

            invention = GlobalFunctions.CreateObject(data.MedatorName, data.ProdVersion, Vector3.OutOfWorld, 0x0, Vector3.UnitZ, null, initData) as GameObject;
            if (invention != null)
            {
                skill.RegisterInventionMade(data);
                if (data.PutInFamilyInventory)
                {
                    if (!Actor.Household.SharedFamilyInventory.Inventory.TryToAdd(invention))
                    {
                        invention.Destroy();
                        invention = null;
                    }
                }
                else if (!Inventories.TryToMove(invention, Actor))
                {
                    invention.Destroy();
                    invention = null;
                }
                if (invention != null)
                {
                    EventTracker.SendEvent(new CreatedInventionEvent(EventTypeId.kCreatedInvention, Actor, invention, data.InventType, isImprovedWidget));
                    if (data.InventType == InventionType.Widget)
                    {
                        EventTracker.SendEvent(EventTypeId.kInventorMadeWidget, Actor);
                    }
                    if (invention is TimeMachine)
                    {
                        EventTracker.SendEvent(EventTypeId.kInventorMadeTimeMachine, Actor);
                    }
                }
            }
        }

        public new class Definition : InventionWorkbench.MakeInvention.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new MakeInventionEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, InventionWorkbench target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (target.InUse || target.mIsMakingFrankensim)
                {
                    return false;
                }
                if (target.mInventionProgress > 0)
                {
                    if (!a.SkillManager.GetSkill<InventingSkill>(SkillNames.Inventing).KnownInventions.Contains(target.mInventionKey))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}

