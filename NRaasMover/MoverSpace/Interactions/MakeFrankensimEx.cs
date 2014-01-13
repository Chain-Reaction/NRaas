using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;

namespace NRaas.MoverSpace.Interactions
{
    public class MakeFrankensimEx : InventionWorkbench.MakeFrankensim, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<InventionWorkbench, InventionWorkbench.MakeFrankensim.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<InventionWorkbench, InventionWorkbench.MakeFrankensim.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public static bool AdoptFrankenSim(Sim creator, Sim frankenSim)
        {
            //if (creator.Household.CanAddSpeciesToHousehold(CASAgeGenderFlags.Human))
            {
                string str = "AdoptFrankenSim";
                InventingSkill skill = creator.SkillManager.GetSkill<InventingSkill>(SkillNames.Inventing);
                if ((skill != null) && skill.OppKnowFrankensimRecipeCompleted)
                {
                    str = "AdoptFrankenSimAgain";
                }

                SimDescription simDescription = frankenSim.SimDescription;
                if (TwoButtonDialog.Show(Localization.LocalizeString("Gameplay/Objects/HobbiesSkills/Inventing/InventionWorkbench:" + str, new object[] { creator }), Localization.LocalizeString("Gameplay/Objects/HobbiesSkills/Inventing/InventionWorkbench:AdoptFrankenSimYes", new object[0x0]), Localization.LocalizeString("Gameplay/Objects/HobbiesSkills/Inventing/InventionWorkbench:AdoptFrankenSimNo", new object[0x0])))
                {
                    VisitSituation situation = VisitSituation.FindVisitSituationInvolvingGuest(frankenSim);
                    if (situation != null)
                    {
                        situation.Exit();
                    }

                    Household.NpcHousehold.Remove(simDescription);
                    creator.Household.Add(simDescription);
                    string str2 = StringInputDialog.Show(Localization.LocalizeString("Gameplay/Objects/HobbiesSkills/Inventing/InventionWorkbench:AdoptFrankenSimNameTitle", new object[0x0]), Localization.LocalizeString("Gameplay/Objects/HobbiesSkills/Inventing/InventionWorkbench:AdoptFrankenSimNamePrompt", new object[] { creator }), frankenSim.FirstName);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        simDescription.FirstName = str2;
                    }

                    frankenSim.OnBecameSelectable();
                    return true;
                }

                Household.NpcHousehold.Remove(simDescription);
                Household household = Household.Create();
                household.Name = simDescription.LastName;
                household.Add(simDescription);
                household.FindSuitableVirtualHome();
                creator.ShowTNSIfSelectable(Localization.LocalizeString("Gameplay/Objects/HobbiesSkills/Inventing/InventionWorkbench:SimBotNotAdopted", new object[] { creator }), StyledNotification.NotificationStyle.kGameMessagePositive, frankenSim.ObjectId);
                Sim.MakeSimGoHome(frankenSim, false);
                return false;
            }

            /*
            string message = Localization.LocalizeString("Gameplay/Objects/HobbiesSkills/Inventing/InventionWorkbench:AdoptFrankenFullHousehold", new object[] { creator });
            creator.ShowTNSIfSelectable(message, StyledNotification.NotificationStyle.kGameMessageNegative);
            Sim.MakeSimGoHome(frankenSim, false);
            return false;
            */
        }

        public override bool Run()
        {
            try
            {
                if (!Target.RouteToWorkbench(Actor))
                {
                    return false;
                }

                mMakeFemale = Target.mIsMakingFemaleFrankensim;
                mInventSkill = Actor.SkillManager.AddElement(SkillNames.Inventing) as InventingSkill;
                bool flag = Target.mInventionProgress > 0f;
                if (!flag)
                {
                    Definition interactionDefinition = InteractionDefinition as Definition;
                    mMakeFemale = interactionDefinition.MakeFemale;
                    Target.mIsMakingFemaleFrankensim = mMakeFemale;
                    ConsumeIngredients();
                }

                StandardEntry();
                Target.mIsMakingFrankensim = true;
                Target.mFrankenSimInventorSimId = Actor.SimDescription.SimDescriptionId;
                StartStages();
                mTimeUntilModelSwap = GetTimeForNextModelChange();
                EnterStateMachine("WorkbenchInvention", "Enter", "x", "workstation");
                SetParameter("shouldSwipe", !flag);
                SetParameter("skillLevel", InventionWorkbench.GetSkillLevelParam(mInventSkill));
                if (!flag)
                {
                    AddSynchronousOneShotScriptEventHandler(0x67, OnAnimationEvent);
                }

                mTotalTime = GetTimeToCompletion();
                AnimateSim("Loop Invent");
                BeginCommodityUpdates();
                if (Actor.SimDescription.TeenOrAbove)
                {
                    Actor.SkillManager.StartGainWithoutSkillMeter(SkillNames.Handiness, InventionWorkbench.kHandinessSkillGainRateDuringMake, true);
                }

                bool succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), MakeLoopCallback, mCurrentStateMachine);
                if (Actor.SimDescription.TeenOrAbove)
                {
                    Actor.SkillManager.StopSkillGain(SkillNames.Handiness);
                }

                EndCommodityUpdates(succeeded);
                AnimateSim("Exit");
                if (!mRecipeKnown && !Actor.OpportunityManager.HasOpportunity(OpportunityNames.EP2_SkillInventing_Frankensim4))
                {
                    Target.ScrapCurrentInvention();
                }

                if (Target.mInventionProgress >= 1f)
                {
                    Sims3.Gameplay.Gameflow.SetGameSpeed(Sims3.Gameplay.Gameflow.GameSpeed.Normal, Sims3.Gameplay.Gameflow.SetGameSpeedContext.Gameplay);
                    RouteAwayFromTable();
                    Target.mDummyModel.UnParent();
                    Target.mDummyModel.Destroy();
                    Target.mDummyModel = null;
                    Target.mInventionProgress = 0f;
                    Target.mIsMakingFrankensim = false;
                    Target.mFrankenSimInventorSimId = 0x0L;
                    Target.mIsMakingFemaleFrankensim = false;
                    Target.mWasFinishedByGnome = false;
                    Target.mIsMakingCheapToy = false;
                    CASAgeGenderFlags gender = mMakeFemale ? CASAgeGenderFlags.Female : CASAgeGenderFlags.Male;
                    mFrankensim = OccultFrankenstein.CreateFrankenStein(Actor, gender);
                    mFrankensim.FadeOut(false, false, 0f);
                    mFrankensim.GreetSimOnLot(Target.LotCurrent);
                    mFrankensim.SetPosition(Target, Slot.RoutingSlot_1);
                    mFrankensim.SetForward((Vector3)(Slots.GetForwardOfSlot(Target.ObjectId, Slot.RoutingSlot_1) * -1f));
                    mFrankensim.AddToWorld();
                    InventingSkill skill = Actor.SkillManager.GetSkill<InventingSkill>(SkillNames.Inventing);
                    if (!skill.OppKnowFrankensimRecipeCompleted)
                    {
                        EventTracker.SendEvent(EventTypeId.kDiscoveredNewInvention, Actor, mFrankensim);
                    }
                    mCurrentStateMachine = StateMachineClient.Acquire(mFrankensim, "WorkbenchFrankensim", AnimationPriority.kAPDefault);
                    SetActor("workstation", Target);
                    SetActor("x", mFrankensim);
                    SetActor("y", Actor);
                    AddSynchronousOneShotScriptEventHandler(0x65, OnAnimationEvent);
                    EnterState("x", "Enter");
                    AnimateSim("Exit");
                    AdoptFrankenSim(Actor, mFrankensim);
                    skill.KnowsFrankensimRecipe = true;
                    skill.TestForNewLifetimeOpp();
                    EventTracker.SendEvent(EventTypeId.kFrankensimLearned, Actor);
                }

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

        public new class Definition : InventionWorkbench.MakeFrankensim.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new MakeFrankensimEx();
                result.Init(ref parameters);
                return result;
            }
        }
    }
}
