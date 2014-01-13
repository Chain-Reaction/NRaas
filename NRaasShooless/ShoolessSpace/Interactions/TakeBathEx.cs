using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.ShoolessSpace.Interactions
{
    public class TakeBathEx : Bathtub.TakeBath, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public virtual void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Bathtub, Bathtub.TakeBath.Definition>(Singleton);
        }

        public virtual void OnPreLoad()
        {
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.CornerBathtub, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubClawfoot, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubModern, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubRectangle, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubShowerModern, Bathtub.TakeBath.Definition, Definition>(false);
            Tunings.Inject<Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubValue, Bathtub.TakeBath.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public static bool Perform(Bathtub.TakeBath ths)
        {
            if (!ths.Target.Line.WaitForTurn(ths, SimQueue.WaitBehavior.DefaultEvict, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), Bathtub.kTimeToWaitForBath))
            {
                return false;
            }

            bool flag = ths.Actor.GetCurrentOutfitCategoryFromOutfitInGameObject() == OutfitCategories.Singed;
            try
            {
                ths.mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(ths.Actor, Sim.ClothesChangeReason.GoingToBathe);
            }
            catch
            {
                return false;
            }

            ths.mSwitchOutfitHelper.Start();

            int num;
            if (!ths.Actor.RouteToSlotListAndCheckInUse(ths.Target, ths.Target.RouteEnterSlots, out num))
            {
                return false;
            }

            if (Shooless.Settings.GetPrivacy(ths.Target))
            {
                ths.mSituation = new Bathtub.BathtubPrivacySituation(ths);
                if (!ths.mSituation.Start())
                {
                    return false;
                }
            }

            ths.StandardEntry();
            if (ths.Actor.HasTrait(TraitNames.Hydrophobic))
            {
                ths.Actor.PlayReaction(ReactionTypes.WhyMe, ths.Target, ThoughtBalloonAxis.kDislike, ReactionSpeed.ImmediateWithoutOverlay);
            }
            if (!ths.Actor.RouteToSlot(ths.Target, ths.Target.RouteEnterSlots, out num))
            {
                if (ths.mSituation != null)
                {
                    ths.mSituation.Exit();
                }
                ths.StandardExit();

                return false;
            }

            if (ths.Autonomous)
            {
                ths.mPriority = new InteractionPriority(InteractionPriorityLevel.UserDirected);
            }

            ths.mSlotIndex = num;
            if (ths.mSituation != null)
            {
                ths.mSituation.DeferShoo = true;
            }

            ths.mSwitchOutfitHelper.Wait(true);
            ths.mCurrentStateMachine = ths.Target.GetStateMachine(ths.Actor, "Enter_Working");
            ths.mSwitchOutfitHelper.AddScriptEventHandler(ths.mCurrentStateMachine);
            ths.TakeBathInteractionPreLoop();
            if (ths.mSituation != null)
            {
                ths.mSituation.StateMachine = ths.mCurrentStateMachine;
            }

            ths.StartStages();
            if (ths.Actor.SimDescription.IsPlantSim)
            {
                ths.ModifyCommodityUpdate(CommodityKind.Hygiene, Bathtub.kPlantSimHygieneModifier);
            }
            ths.BeginCommodityUpdates();

            bool succeeded = false;

            try
            {
                ths.Actor.RegisterGroupTalk();
                if (ths.mSituation != null)
                {
                    ths.mSituation.DeferShoo = false;
                    if (ths.mSituation.SomeoneDidIntrude)
                    {
                        ths.mSituation.ReactToIntrusion();
                    }
                }

                succeeded = ths.DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), ths.DuringBath, null);

                if (succeeded)
                {
                    if (ths.Actor.HasTrait(TraitNames.Hydrophobic))
                    {
                        ths.Actor.PlayReaction(ReactionTypes.Cry, ths.Target, ThoughtBalloonAxis.kDislike, ReactionSpeed.AfterInteraction);
                    }
                    ths.Actor.BuffManager.RemoveElement(BuffNames.Singed);
                    ths.Actor.BuffManager.RemoveElement(BuffNames.SingedElectricity);
                    ths.Actor.BuffManager.RemoveElement(BuffNames.GotFleasHuman);
                    ths.Actor.SimDescription.RemoveFacePaint();
                    if (flag)
                    {
                        ths.mSwitchOutfitHelper.Dispose();
                        try
                        {
                            ths.mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(ths.Actor, Sim.ClothesChangeReason.GoingToBathe);
                            ths.mSwitchOutfitHelper.Start();
                            ths.mSwitchOutfitHelper.Wait(false);
                            ths.mSwitchOutfitHelper.ChangeOutfit();
                        }
                        catch
                        { }
                    }
                    ths.Actor.Motives.SetMax(CommodityKind.Hygiene);
                }

                if (!flag || (flag && succeeded))
                {
                    ths.mCurrentStateMachine.SetParameter("changeClothes", !ths.Actor.OccultManager.DisallowClothesChange());
                    ths.mSwitchOutfitHelper.Dispose();
                    try
                    {
                        ths.mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(ths.Actor, TakeShowerEx.GetOutfitReason(ths.Actor));
                        ths.mSwitchOutfitHelper.Start();
                        ths.mSwitchOutfitHelper.AddScriptEventHandler(ths.mCurrentStateMachine);
                        ths.mSwitchOutfitHelper.Wait(false);
                    }
                    catch
                    { }
                }
                ths.Actor.UnregisterGroupTalk();
            }
            finally
            {
                ths.EndCommodityUpdates(succeeded);
            }

            if (ths.mSituation != null)
            {
                ths.mSituation.StateMachine = null;
            }

            ths.TakeBathInteractionPostLoop();
            ths.StandardExit();
            return succeeded;
        }

        public override bool Run()
        {
            try
            {
                return Perform(this);
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

        public new class Definition : Bathtub.TakeBath.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TakeBathEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Bathtub target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}


