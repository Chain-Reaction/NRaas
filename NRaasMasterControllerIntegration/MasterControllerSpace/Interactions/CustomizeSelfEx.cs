using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class CustomizeSelfEx : BotMakingStation.CustomizeSelf, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<BotMakingStation, BotMakingStation.CustomizeSelf.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<BotMakingStation, BotMakingStation.CustomizeSelf.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                RobotForms form;
                if (!Actor.SimDescription.IsEP11Bot)
                {
                    return false;
                }
                if (!Actor.RouteToSlotAndCheckInUse(Target, BotMakingStation.kBotPodRoutingSlot))
                {
                    return false;
                }
                StandardEntry();
                BeginCommodityUpdates();
                EnterStateMachine("BotMakingStation", "Enter", "x");
                SetActor("BotMakingStation", Target);
                CASRobotData supernaturalData = Actor.SimDescription.SupernaturalData as CASRobotData;
                if (supernaturalData != null)
                {
                    form = supernaturalData.Form;
                    SetParameter("IsHoverBot", supernaturalData.Form == RobotForms.Hovering);
                }
                else
                {
                    form = RobotForms.Humanoid;
                    SetParameter("IsHoverBot", false);
                }
                mCurrentStateMachine.RequestState(true, "x", "OpenDoor");
                CancellableByPlayer = false;
                AnimateSim("StayOpen");
                if (!Target.RouteInsidePod(Actor))
                {
                    AnimateSim("ExitBotOnly");
                    EndCommodityUpdates(true);
                    StandardExit();
                    return false;
                }
                if (Actor.HasExitReason(ExitReason.CancelExternal))
                {
                    AnimateSim("ExitBotOnly");
                    StandardExit();
                    return false;
                }

                AnimateSim("CustomizeSelf");
                CASChangeReporter.Instance.ClearChanges();
                SimDescription simDescription = Actor.SimDescription;
                try
                {
                    new Sims.Advanced.EditInCAS(false).Perform(new GameHitParameters<GameObject>(Actor, Actor, GameObjectHit.NoHit));

                    while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                    {
                        SpeedTrap.Sleep();
                    }

                    CASChangeReporter.Instance.SendChangedEvents(Actor);
                    CASChangeReporter.Instance.ClearChanges();
                    if (!CASChangeReporter.Instance.CasCancelled)
                    {
                        Actor.ModifyFunds(-BotMakingStation.kCostToCustomizeServoBot);
                    }

                    (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).NotifySimChanged(Actor.ObjectId);
                    AnimateSim("ExitBotOnly");
                }
                finally
                {
                    supernaturalData = Actor.SimDescription.SupernaturalData as CASRobotData;
                    if (supernaturalData.Form != form)
                    {
                        if (supernaturalData.Form == RobotForms.Hovering)
                        {
                            Actor.TraitManager.RemoveElement(TraitNames.BipedBot);
                            Actor.TraitManager.AddHiddenElement(TraitNames.HoverBot);
                        }
                        else
                        {
                            Actor.TraitManager.RemoveElement(TraitNames.HoverBot);
                            Actor.TraitManager.AddHiddenElement(TraitNames.BipedBot);
                        }
                    }
                }

                EndCommodityUpdates(true);
                StandardExit();
                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }

            return false;
        }

        public new class Definition : BotMakingStation.CustomizeSelf.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CustomizeSelfEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, BotMakingStation target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
