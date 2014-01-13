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
    public class SpongeBathEx : Sink.SpongeBath, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sink, Sink.SpongeBath.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Sink, Sink.SpongeBath.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                try
                {
                    mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(Actor, Sim.ClothesChangeReason.GoingToBathe);
                }
                catch
                {
                    return false;
                }

                mSwitchOutfitHelper.Start();
                if (!Target.RouteToSink(this))
                {
                    return false;
                }

                if (Shooless.Settings.GetPrivacy(Target))
                {
                    mSituation = new Sink.SpongeBathPrivacySituation(this);
                    if (!mSituation.Start())
                    {
                        return false;
                    }
                }

                Matrix44 transform = Actor.Transform;
                if (!Actor.RouteToMatrix(ref transform))
                {
                    if (mSituation != null)
                    {
                        mSituation.Exit();
                    }
                    return false;
                }
                StandardEntry();
                mSwitchOutfitHelper.Wait(true);
                bool flag = Actor.GetCurrentOutfitCategoryFromOutfitInGameObject() == OutfitCategories.Singed;
                Target.mSimUsingSink = Actor;
                mCurrentStateMachine = Target.GetStateMachine(Actor, "Enter");
                if (mSituation != null)
                {
                    mSituation.StateMachine = mCurrentStateMachine;
                }
                SetParameter("SimShouldClothesChange", !flag && !Actor.OccultManager.DisallowClothesChange());
                mSwitchOutfitHelper.AddScriptEventHandler(this);
                AddOneShotScriptEventHandler(0x3e9, new SacsEventHandler(EventCallbackStartWaterSound));
                bool paramValue = (Target.BoobyTrapComponent != null) ? Target.BoobyTrapComponent.CanTriggerTrap(Actor.SimDescription) : false;
                mCurrentStateMachine.SetParameter("isBoobyTrapped", paramValue);
                AnimateSim("Loop SpongeBath");
                StartStages();
                Target.StartDisgustEffect();
                BeginCommodityUpdates();

                bool succeeded = false;
                if (paramValue)
                {
                    Target.TriggerTrap(Actor);
                }
                else
                {
                    try
                    {
                        Sink.CheckToAddChillyBuff(Actor, Target);
                        succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), LoopDel, null);
                        Actor.BuffManager.UnpauseBuff(BuffNames.Chilly);
                    }
                    finally
                    {
                        EndCommodityUpdates(succeeded);
                    }

                    if (succeeded)
                    {
                        Actor.BuffManager.RemoveElement(BuffNames.Singed);
                        Actor.BuffManager.RemoveElement(BuffNames.SingedElectricity);
                        Actor.BuffManager.RemoveElement(BuffNames.GotFleasHuman);
                    }
                    if (!flag || (flag && succeeded))
                    {
                        SetParameter("SimShouldClothesChange", !Actor.OccultManager.DisallowClothesChange());
                        mSwitchOutfitHelper.Dispose();

                        try
                        {
                            mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(Actor, Sim.ClothesChangeReason.GettingOutOfBath);
                            mSwitchOutfitHelper.Start();
                            mSwitchOutfitHelper.AddScriptEventHandler(this);
                            mSwitchOutfitHelper.Wait(false);
                        }
                        catch
                        { }
                    }
                }

                AddOneShotScriptEventHandler(0x3ea, new SacsEventHandler(EventCallbackStopWaterSound));
                Target.UpdateDirtyBreakAndPickExitState(mCurrentStateMachine, Actor);
                Target.StopDisgustEffect();
                Target.mSimUsingSink = null;
                TraitFunctions.CheckForNeuroticAnxiety(Actor, TraitFunctions.NeuroticTraitAnxietyType.Sink);
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

        public new class Definition : Sink.SpongeBath.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SpongeBathEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sink target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}


