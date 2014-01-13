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
    public class CreateServobotEx : BotMakingStation.CreateServobot, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<BotMakingStation, BotMakingStation.CreateServobot.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<BotMakingStation, BotMakingStation.CreateServobot.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if (!Actor.RouteToSlotAndCheckInUse(Target, BotMakingStation.kOperatorRoutingSlot))
                {
                    return false;
                }

                StandardEntry();
                BeginCommodityUpdates();
                CASAgeGenderFlags adult = CASAgeGenderFlags.Adult;
                CASAgeGenderFlags gender = CASAgeGenderFlags.None | CASAgeGenderFlags.Male;
                RobotForms humanoid = RobotForms.Humanoid;
                SimDescription simDescription = OccultRobot.MakeRobot(adult, gender, humanoid);
                if (simDescription == null)
                {
                    StandardExit();
                    return false;
                }

                FutureSkill.SetupReactToFutureTech(Target);
                EnterStateMachine("BotMakingStation", "Enter", "x");
                SetActor("BotMakingStation", Target);
                Target.SetGeometryState("on");
                Animate("x", "CreateServoBot");
                bool flag = false;
                Household.NpcHousehold.Add(simDescription);

                try
                {
                    new Sims.Advanced.EditInCAS(true).Perform(new GameHitParameters<SimDescriptionObject>(Sim.ActiveActor, new SimDescriptionObject(simDescription), GameObjectHit.NoHit));

                    while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                    {
                        SpeedTrap.Sleep();
                    }

                    flag |= CASChangeReporter.Instance.CasCancelled;
                    Household.NpcHousehold.Remove(simDescription);
                    Target.LotCurrent.SetDisplayLevel(LotManager.GetBestLevelToDisplayForSim(Actor, Target.LotCurrent));
                    Household household = Actor.Household;

                    /* Overstuffed
                    if (!household.CanAddSimDescriptionToHousehold(simDescription))
                    {
                        flag = true;
                    }
                    */
                    if (flag)
                    {
                        AnimateSim("CancelCreateServoBot");
                        simDescription.Dispose();
                        AnimateSim("Exit");
                    }
                    else
                    {
                        CASRobotData supernaturalData = simDescription.SupernaturalData as CASRobotData;
                        supernaturalData.CreatorSim = Actor.SimDescription.SimDescriptionId;
                        supernaturalData.BotQualityLevel = Target.GetCreationLevel(Actor);
                        household.Add(simDescription);
                        Sim sim = null;
                        Slot[] containmentSlots = Target.GetContainmentSlots();
                        if ((containmentSlots != null) && (containmentSlots.Length > 0x0))
                        {
                            Vector3 slotPosition = Target.GetSlotPosition(containmentSlots[0x0]);
                            Vector3 forwardOfSlot = Target.GetForwardOfSlot(containmentSlots[0x0]);
                            sim = Genetics.InstantiateRobotAndGrantPhone(simDescription, slotPosition);
                            sim.SetPosition(slotPosition);
                            sim.SetForward(forwardOfSlot);
                        }
                        else
                        {
                            Vector3 position = Actor.Position;
                            Vector3 forwardVector = Actor.ForwardVector;
                            sim = simDescription.Instantiate(position);
                            GlobalFunctions.FindGoodLocationNearby(sim, ref position, ref forwardVector);
                            sim.SetPosition(position);
                            sim.SetForward(forwardVector);
                        }

                        Actor.Genealogy.AddChild(sim.Genealogy);
                        InteractionQueue interactionQueue = sim.InteractionQueue;
                        mLinkedInteraction = BotMakingStation.BeCreated.Singleton.CreateInstance(Target, sim, new InteractionPriority(InteractionPriorityLevel.CriticalNPCBehavior), false, false) as BotMakingStation.BeCreated;
                        mLinkedInteraction.SyncTarget = Actor;
                        interactionQueue.AddNext(mLinkedInteraction);
                        Actor.SynchronizationRole = Sim.SyncRole.Initiator;
                        Actor.SynchronizationTarget = sim;
                        Actor.SynchronizationLevel = Sim.SyncLevel.Routed;
                        if (!Actor.WaitForSynchronizationLevelWithSim(sim, Sim.SyncLevel.Started, 40f))
                        {
                            Actor.ClearSynchronizationData();
                            AnimateJoinSims("CompleteCreateServoBot");
                            AnimateSim("ExitWithBot");
                            Target.SetGeometryState("off");
                            EndCommodityUpdates(false);
                            StandardExit();
                            return false;
                        }

                        SetActorAndEnter("y", sim, "BotEnter");
                        CASRobotData data2 = simDescription.SupernaturalData as CASRobotData;
                        if (data2 != null)
                        {
                            SetParameter("IsHoverBot", data2.Form == RobotForms.Hovering);
                        }
                        else
                        {
                            SetParameter("IsHoverBot", false);
                        }

                        Actor.ModifyFunds(-BotMakingStation.kCostToBuildServoBot);
                        AnimateJoinSims("CompleteCreateServoBot");
                        AnimateSim("ExitWithBot");
                        BotBuildingSkill element = Actor.SkillManager.GetElement(SkillNames.BotBuilding) as BotBuildingSkill;
                        sim.SimDescription.TraitChipManager.UpgradeNumTraitChips(element.GetSimMaxAllowedUpgradeSlots());
                        EventTracker.SendEvent(EventTypeId.kCreatedBot, Actor, sim);
                        element.OnBotCreated();
                    }
                }
                catch
                {
                    simDescription.Dispose();
                }

                Target.SetGeometryState("off");
                EndCommodityUpdates(!flag);
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

        public new class Definition : BotMakingStation.CreateServobot.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CreateServobotEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, BotMakingStation target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, BotMakingStation target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                /*
                if (!a.Household.CanAddSpeciesToHousehold(CASAgeGenderFlags.Human, 0x1, true))
                {
                    string localizedString = BotMakingStation.LocalizeString(a.IsFemale, "HouseholdIsFull", new object[0x0]);
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(localizedString);
                    return false;
                }
                */
                if ((a != null) && (a.SkillManager != null))
                {
                    BotBuildingSkill element = a.SkillManager.GetElement(SkillNames.BotBuilding) as BotBuildingSkill;
                    if ((element != null) && (element.SkillLevel > BotMakingStation.kMinSkillToCreateServoBot))
                    {
                        if (a.FamilyFunds >= BotMakingStation.kCostToBuildServoBot)
                        {
                            if (target.InUse && !target.mActorsUsingMe.Contains(a))
                            {
                                greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(BotMakingStation.LocalizeString("StationInUse", new object[0x0]));
                                return false;
                            }
                            return true;
                        }

                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(BotMakingStation.LocalizeString(a.IsFemale, "LackTheSimoleansToCreateServobot", new object[0x0]));
                    }
                    else
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(BotMakingStation.LocalizeString(a.IsFemale, "LackTheRequiredSkillLevelToCreateAServobot", new object[0x0]));
                    }
                }
                return false;
            }
        }
    }
}
