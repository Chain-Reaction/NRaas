using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
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
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class CatchPreyEx : CatHuntingComponent.CatchPrey, Common.IPreLoad, IFormattedStoryScenario
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<ICatPrey, CatHuntingComponent.CatchPrey.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.SkillThresholdValue = 0;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public Manager GetFormattedStoryManager()
        {
            return StoryProgression.Main.Skills;
        }

        public string UnlocalizedName
        {
            get { return "CatchPrey"; }
        }

        private void DoExitWithPreyOutcomeEx()
        {
            ICatPrey target = Target;
            IInsectJig jig = Target as IInsectJig;
            if (jig != null)
            {
                DestroyPrey = true;
                target = CatHuntingComponent.CreateTerrarium(jig.InsectType);
                if (target != null)
                {
                    target.SetPosition(Target.Position);
                    target.SetForward(Target.ForwardVector);
                    target.AddToWorld();
                    target.UpdateVisualState(CatHuntingComponent.CatHuntingModelState.Carried);
                    SetActor("prey", target);
                }
            }
            else
            {
                DestroyPrey = false;
            }
            if (target != null)
            {
                target.SetOpacity(1f, 0f);
                AnimateSim("Exit With Prey");
                if (target.CatHuntingComponent != null)
                {
                    target.CatHuntingComponent.SetCatcher(Actor);
                }

                Sim inventoryActor = Actor;

                if (!SimTypes.IsSelectable(Actor))
                {
                    SimDescription head = SimTypes.HeadOfFamily(Actor.Household);
                    if ((head != null) && (head.CreatedSim != null))
                    {
                        inventoryActor = head.CreatedSim;
                    }
                }

                // Custom
                if (inventoryActor.Inventory.TryToAdd(target, false))
                {
                    target.UpdateVisualState(CatHuntingComponent.CatHuntingModelState.InInventory);

                    // Custom
                    StoryProgression.Main.Skills.Notify("CatchPrey", Actor, CatHuntingComponent.LocalizeString(Actor.IsFemale, "CatchPreySuccess", new object[] { Actor, Target.GetLocalizedName() }));

                    foreach (Sim sim in Actor.Household.Sims)
                    {
                        EventTracker.SendEvent(new Event(EventTypeId.kArkBuilder, sim, Target));
                    }

                    if (SimTypes.IsSelectable(Actor))
                    {
                        TryPushPresentTo(target);
                    }
                }
                else
                {
                    if (jig != null)
                    {
                        target.Destroy();
                    }
                    DestroyPrey = true;
                }
            }
            else
            {
                DoPreyGoneOutcome();
            }
        }

        public override bool Run()
        {
            try
            {
                if (!FromEatPreyInteraction && !Actor.RouteToObjectRadius(Target, kInitialRouteDistance))
                {
                    return false;
                }
                if (Target.InUse)
                {
                    return false;
                }

                StandardEntry();
                mHuntingSkill = Actor.SkillManager.AddElement(SkillNames.CatHunting) as CatHuntingSkill;
                if (!TryPlaceJigOnPrey())
                {
                    StandardExit();
                    return false;
                }
                if (!RouteToJig())
                {
                    if (Actor.Posture is PouncePosture)
                    {
                        Actor.PopPosture();
                    }
                    StandardExit();
                    return false;
                }
                EnterStateMachine("CatHunt", "Enter", "x");
                SetActor("prey", Target);
                AddOneShotScriptEventHandler(0xc8, new SacsEventHandler(OnAnimationEvent));
                AnimateSim("Loop");
                Target.UpdateVisualState(CatHuntingComponent.CatHuntingModelState.Carried);
                if (Actor.HasTrait(TraitNames.IndependentPet))
                {
                    BeginCommodityUpdate(CommodityKind.Fun, TraitTuning.IndependentPetTraitFunMultiplier);
                }
                BeginCommodityUpdates();
                if ((SimToPresentToID != 0x0L) && SimDescription.Find(SimToPresentToID).TraitManager.HasElement(TraitNames.CatPerson))
                {
                    ModifyCommodityUpdate(CommodityKind.SkillCatHunting, TraitTuning.kPreferredPetSkillGainBoost);
                }
                DoTimedLoop(kFightLength, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                EndCommodityUpdates(true);
                if (Actor.Posture is PouncePosture)
                {
                    Actor.PopPosture();
                }
                EventTracker.SendEvent(EventTypeId.kGoHuntingCat, Actor);
                if (InteractionDefinition is FailurePreyDefinition)
                {
                    DoFailureObjectOutcome();
                }
                else if ((FromEatPreyInteraction || ForceCatchPrey) || RollDiceForCatchingPrey())
                {
                    mHuntingSkill.RegisterCaughtPrey(Target);
                    if (FromEatPreyInteraction || ((SimToPresentToID == 0x0L) && (Actor.Motives.GetValue(CommodityKind.Hunger) < kHungerThreshold)))
                    {
                        DoEatPreyOutcome();
                    }
                    else
                    {
                        // Custom
                        DoExitWithPreyOutcomeEx();
                    }
                }
                else
                {
                    RunFailureBehavior();
                }
                StandardExit(Target.IsActorUsingMe(Actor));
                return true;
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

        private new class Definition : CatHuntingComponent.CatchPrey.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CatchPreyEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, ICatPrey target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, ICatPrey target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (SimTypes.IsSelectable(a))
                {
                    if (a.SkillManager.GetSkillLevel(SkillNames.CatHunting) < 1)
                    {
                        return false;
                    }
                }

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}

