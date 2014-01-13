using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.TuningValues;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Interactions
{
    public class TakeShowerEx : Shower.TakeShower, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public virtual void OnPreLoad()
        {
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerBasic,Shower.TakeShower.Definition,Definition>(true);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerGen, Shower.TakeShower.Definition, Definition>(true);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerHETech, Shower.TakeShower.Definition, Definition>(true);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerLoft, Shower.TakeShower.Definition, Definition>(true);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerModern, Shower.TakeShower.Definition, Definition>(true);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubShowerModern, Shower.TakeShower.Definition, Definition>(true);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.ShowerCheap, Shower.TakeShower.Definition, Definition>(true);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.ShowerExpensive, Shower.TakeShower.Definition, Definition>(true);
            Tunings.Inject<IShowerable, Shower.TakeShower.Definition, Definition>(true);

            if (!Common.AssemblyCheck.IsInstalled("NRaasShooless"))
            {
                sOldSingleton = Singleton;
                Singleton = new Definition();
            }
        }

        public virtual void AddInteraction(Common.InteractionInjectorList interactions)
        {
            if (!Common.AssemblyCheck.IsInstalled("NRaasShooless"))
            {
                interactions.Replace<IShowerable, Shower.TakeShower.Definition>(Singleton);
            }
        }

        public static Sim.ClothesChangeReason GetOutfitReason(Sim actor)
        {
            if (actor.CareerManager != null)
            {
                if ((actor.Occupation != null) && (actor.CareerManager.CareerHoursTillWork < Sim.kClothingChangeHoursBeforeWork))
                {
                    if (actor.Occupation.HasOpenHours)
                    {
                        return Sim.ClothesChangeReason.GoingOutside;
                    }
                }
            }

            return Sim.ClothesChangeReason.GettingOutOfBath;
        }

        protected virtual Sim.ClothesChangeReason OutfitChoice
        {
            get
            {
                return Sim.ClothesChangeReason.GoingToBathe;
            }
        }

        protected virtual bool EnforcePrivacy
        {
            get
            {
                return !HavingWooHoo;
            }
        }

        public override bool Run()
        {
            try
            {
                if (!Target.SimLine.WaitForTurn(this, SimQueue.WaitBehavior.DefaultEvict, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), Shower.kTimeToWaitToEvict))
                {
                    return false;
                }

                try
                {
                    mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(Actor, OutfitChoice);
                }
                catch
                {
                    return false;
                }

                mSwitchOutfitHelper.Start();
                if (Actor.HasTrait(TraitNames.Hydrophobic))
                {
                    Actor.PlayReaction(ReactionTypes.WhyMe, Target as GameObject, ReactionSpeed.ImmediateWithoutOverlay);
                }
                if (Actor.HasTrait(TraitNames.Daredevil))
                {
                    TraitTipsManager.ShowTraitTip(0xb82d0015b9294260L, Actor, TraitTipsManager.TraitTipCounterIndex.Daredevil, TraitTipsManager.kDaredevilCountOfShowersTaken);
                }

                if (!Actor.RouteToSlotAndCheckInUse(Target, Slot.RoutingSlot_0))
                {
                    return false;
                }

                if (EnforcePrivacy)
                {
                    mSituation = new Shower.ShowerPrivacySituation(this);
                    if (!mSituation.Start())
                    {
                        return false;
                    }
                }

                StandardEntry();
                if (!Actor.RouteToSlot(Target, Slot.RoutingSlot_0))
                {
                    if (mSituation != null)
                    {
                        mSituation.Exit();
                    }

                    StandardExit();
                    return false;
                }
                if (Autonomous)
                {
                    mPriority = new InteractionPriority(InteractionPriorityLevel.UserDirected);
                }
                mSwitchOutfitHelper.Wait(true);
                bool daredevilPerforming = Actor.DaredevilPerforming;
                bool flag2 = Actor.GetCurrentOutfitCategoryFromOutfitInGameObject() == OutfitCategories.Singed;
                EnterStateMachine("Shower", "Enter", "x");
                SetActor("Shower", Target);
                if (mSituation != null)
                {
                    mSituation.StateMachine = mCurrentStateMachine;
                }
                SetParameter("IsShowerTub", Target.IsShowerTub);
                SetParameter("SimShouldCloseDoor", true); // Required to be "true" for the Oufit switcher
                SetParameter("SimShouldClothesChange", ((!daredevilPerforming && !flag2) && !Actor.OccultManager.DisallowClothesChange()) && !Actor.BuffManager.DisallowClothesChange());
                
                bool paramValue = false;
                if ((Target.BoobyTrapComponent != null) && Target.BoobyTrapComponent.CanTriggerTrap(Actor.SimDescription))
                {
                    paramValue = !Actor.OccultManager.DisallowClothesChange() && !Actor.BuffManager.DisallowClothesChange();
                }
                SimDescription description = ((Target.BoobyTrapComponent != null) && (Target.BoobyTrapComponent.TrapSetter != 0L)) ? SimDescription.Find(Target.BoobyTrapComponent.TrapSetter) : null;
                if (((description != null) && description.IsFairy) && Actor.BuffManager.HasElement(BuffNames.TrickedByAFairy))
                {
                    paramValue = false;
                }
                SetParameter("isBoobyTrapped", paramValue);
                mSwitchOutfitHelper.AddScriptEventHandler(this);
                AddOneShotScriptEventHandler(0x3e9, EventCallbackStartShoweringSound);
                if (Actor.HasTrait(TraitNames.Virtuoso) || RandomUtil.RandomChance((float)Target.TuningShower.ChanceOfSinging))
                {
                    AddOneShotScriptEventHandler(0xc8, EventCallbackStartSinging);
                }
                PetStartleBehavior.CheckForStartle(Target as GameObject, StartleType.ShowerOn);
                AnimateSim("Loop Shower");
                Actor.BuffManager.AddElement(BuffNames.SavingWater, Origin.FromShower, ProductVersion.EP2, TraitNames.EnvironmentallyConscious);
                mShowerStage.ResetCompletionTime(GetShowerTime());
                StartStages();
                if (Actor.HasTrait(TraitNames.EnvironmentallyConscious))
                {
                    BeginCommodityUpdate(CommodityKind.Hygiene, Shower.kEnvironmentallyConsciousShowerSpeedMultiplier);
                }
                if (Actor.SimDescription.IsPlantSim)
                {
                    ModifyCommodityUpdate(CommodityKind.Hygiene, Shower.kPlantSimHygieneModifier);
                }

                BeginCommodityUpdates();

                if (paramValue)
                {
                    ApplyBoobyTrapOutfit();
                    if ((description != null) && description.IsFairy)
                    {
                        Actor.BuffManager.AddElement(BuffNames.TrickedByAFairy, Origin.FromFairy);
                    }
                }

                bool succeeded = false;
                try
                {
                    try
                    {
                        Target.SimInShower = Actor;
                        succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), DuringShower, null);
                        if (HavingWooHoo && Actor.HasExitReason(ExitReason.StageComplete))
                        {
                            succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached | ExitReason.StageComplete), DuringShower, null);
                        }
                    }
                    finally
                    {
                        Target.SimInShower = null;
                    }

                    while (HavingWooHoo)
                    {
                        SpeedTrap.Sleep(10);
                    }
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }

                Shower.WaitToLeaveShower(Actor, Target);
                if (succeeded)
                {
                    ShowerEx.ApplyPostShowerEffects(Actor, Target);
                }
                if (paramValue)
                {
                    SetParameter("isBoobyTrapped", false);
                    AddOneShotScriptEventHandler(0xc9, EventCallbackStopSinging);
                    AddOneShotScriptEventHandler(0x3ea, EventCallbackStopShoweringSound);
                    if ((description != null) && description.IsFairy)
                    {
                        AnimateSim("TriggerFairyTrap");
                    }
                    else
                    {
                        AnimateSim("Booby Trap Reaction");
                    }
                    AddOneShotScriptEventHandler(0x3e9, EventCallbackStartShoweringSound);
                    AnimateSim("Loop Shower");
                    RemoveBoobyTrapOutfit();
                    SpeedTrap.Sleep(60);
                }

                try
                {
                    if (flag2 && succeeded)
                    {
                        mSwitchOutfitHelper.Dispose();
                        try
                        {
                            mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(Actor, OutfitChoice);
                            mSwitchOutfitHelper.Start();
                            mSwitchOutfitHelper.Wait(false);
                            mSwitchOutfitHelper.ChangeOutfit();
                        }
                        catch
                        { }
                    }
                    bool flag5 = false;
                    if ((flag2 && succeeded) || (!flag2 && !daredevilPerforming))
                    {
                        SetParameter("SimShouldClothesChange", !Actor.OccultManager.DisallowClothesChange());
                        mSwitchOutfitHelper.Dispose();
                        try
                        {
                            mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(Actor, GetOutfitReason(Actor));
                            mSwitchOutfitHelper.Start();
                            mSwitchOutfitHelper.AddScriptEventHandler(this);
                            mSwitchOutfitHelper.Wait(false);
                        }
                        catch
                        { }

                        flag5 = true;
                    }

                    if (Target.Cleanable != null)
                    {
                        Target.Cleanable.DirtyInc(Actor);
                    }

                    AddOneShotScriptEventHandler(0xc9, EventCallbackStopSinging);
                    AddOneShotScriptEventHandler(0x3ea, EventCallbackStopShoweringSound);
                    if (flag5 && InventingSkill.IsBeingDetonated(Target as GameObject))
                    {
                        SetParameter("SimShouldClothesChange", false);
                        mSwitchOutfitHelper.Abort();
                        mSwitchOutfitHelper.Dispose();
                    }

                    if ((Target.Repairable != null) && (Target.Repairable.UpdateBreakage(Actor)))
                    {
                        Target.StartBrokenFXInAnim(mCurrentStateMachine);
                        AnimateSim("Exit Broken");
                    }
                    else
                    {
                        AnimateSim("Exit Working");
                    }

                    if ((Actor.SimDescription.IsMummy || Actor.DaredevilPerforming) || (Actor.TraitManager.HasElement(TraitNames.Slob) && RandomUtil.RandomChance01(TraitTuning.SlobTraitChanceToLeavePuddle)))
                    {
                        PuddleManager.AddPuddle(Actor.Position);
                    }

                    if (succeeded)
                    {
                        Actor.BuffManager.RemoveElement(BuffNames.GotFleasHuman);
                    }
                }
                finally
                {
                    StandardExit();
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

        public new class Definition : Shower.TakeShower.Definition
        {
            public override string GetInteractionName(Sim actor, IShowerable target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(Shower.TakeShower.Singleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TakeShowerEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}


