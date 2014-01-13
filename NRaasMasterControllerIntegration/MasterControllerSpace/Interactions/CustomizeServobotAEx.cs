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
    public class CustomizeServobotAEx : BotMakingStation.CustomizeServobotA, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<BotMakingStation, BotMakingStation.CustomizeServobotA.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<BotMakingStation, BotMakingStation.CustomizeServobotA.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                RobotForms form;
                if (SelectedObjects == null)
                {
                    return false;
                }

                List<Sim> selectedObjectsAsSims = GetSelectedObjectsAsSims();
                if (selectedObjectsAsSims.Count == 0x0)
                {
                    return false;
                }

                Sim actor = selectedObjectsAsSims[0x0];
                if (actor == null)
                {
                    return false;
                }

                LinkedInteractionInstance = new BotMakingStation.CustomizeServobotB.Definition(Actor).CreateInstance(Target, actor, new InteractionPriority(InteractionPriorityLevel.CriticalNPCBehavior), Autonomous, CancellableByPlayer);
                if (!actor.InteractionQueue.AddNext(LinkedInteractionInstance))
                {
                    return false;
                }

                Actor.SynchronizationRole = Sim.SyncRole.Initiator;
                Actor.SynchronizationLevel = Sim.SyncLevel.Started;
                Actor.SynchronizationTarget = actor;
                if (!(Actor.SkillManager.GetElement(SkillNames.BotBuilding) is BotBuildingSkill))
                {
                    Actor.SkillManager.AddElement(SkillNames.BotBuilding);
                    BotBuildingSkill element = Actor.SkillManager.GetElement(SkillNames.BotBuilding) as BotBuildingSkill;
                }

                if (!Actor.RouteToSlotAndCheckInUse(Target, BotMakingStation.kOperatorRoutingSlot))
                {
                    return false;
                }

                Actor.LoopIdle();
                Actor.SynchronizationLevel = Sim.SyncLevel.Routed;
                if (!Actor.WaitForSynchronizationLevelWithSimAndIgnoreSocialTest(actor, Sim.SyncLevel.Routed, 100f))
                {
                    FinishLinkedInteraction(true);
                    return false;
                }

                StandardEntry();
                BeginCommodityUpdates();
                EnterStateMachine("BotMakingStation", "Enter", "x");
                SetActor("BotMakingStation", Target);
                CASRobotData supernaturalData = actor.SimDescription.SupernaturalData as CASRobotData;
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

                Target.SetGeometryState("on");
                mCurrentStateMachine.RequestState(true, "x", "OpenDoor");
                Actor.SynchronizationLevel = Sim.SyncLevel.Committed;
                AnimateSim("StayOpen");
                if (!Actor.WaitForSynchronizationLevelWithSim(actor, Sim.SyncLevel.Committed, 100f))
                {
                    SetActor("y", actor);
                    mCurrentStateMachine.RequestState(true, "x", "ExitWithBot");
                    FinishLinkedInteraction(true);
                    EndCommodityUpdates(true);
                    StandardExit();
                    return false;
                }

                SetActor("y", actor);
                mCurrentStateMachine.RequestState(true, "x", "CustomizeServobot");
                CASLogic singleton = CASLogic.GetSingleton();
                try
                {
                    new Sims.Advanced.EditInCAS(false).Perform(new GameHitParameters<GameObject>(actor, actor, GameObjectHit.NoHit));

                    while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                    {
                        SpeedTrap.Sleep();
                    }
                    if (!CASChangeReporter.Instance.CasCancelled && CASChangeReporter.Instance.GetPropertyChanged(CASChangeReporter.ChangeFlags.Any))
                    {
                        Actor.ModifyFunds(-BotMakingStation.kCostToCustomizeServoBot);
                    }
                    mCurrentStateMachine.RequestState(true, "x", "ExitWithBot");
                    Target.SetGeometryState("off");
                    Actor.SynchronizationLevel = Sim.SyncLevel.Completed;
                    FinishLinkedInteraction(true);
                }
                finally
                {
                    supernaturalData = actor.SimDescription.SupernaturalData as CASRobotData;
                    if (supernaturalData.Form != form)
                    {
                        if (supernaturalData.Form == RobotForms.Hovering)
                        {
                            actor.TraitManager.RemoveElement(TraitNames.BipedBot);
                            actor.TraitManager.AddHiddenElement(TraitNames.HoverBot);
                        }
                        else
                        {
                            actor.TraitManager.RemoveElement(TraitNames.HoverBot);
                            actor.TraitManager.AddHiddenElement(TraitNames.BipedBot);
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

        public new class Definition : BotMakingStation.CustomizeServobotA.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CustomizeServobotAEx();
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
