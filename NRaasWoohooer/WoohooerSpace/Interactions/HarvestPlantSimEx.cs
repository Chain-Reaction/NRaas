using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class HarvestPlantSimEx : ForbiddenFruitTree.HarvestPlantSim, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<ForbiddenFruitTree, ForbiddenFruitTree.HarvestPlantSim.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<ForbiddenFruitTree, ForbiddenFruitTree.HarvestPlantSim.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                if (!Target.RouteSimToMeAndCheckInUse(Actor) || !HarvestPlant.HarvestTest(Target, Actor))
                {
                    return false;
                }

                Target.RemoveHarvestStateTimeoutAlarm();
                StandardEntry();
                BeginCommodityUpdates();
                Soil dummyIk = null;
                StateMachineClient client = null;

                bool allowChild = false;

                if (Actor.SimDescription.YoungAdultOrAbove)
                {
                    allowChild = true;
                }
                else if ((Actor.SimDescription.Teen) && (Woohooer.Settings.mUnlockTeenActions))
                {
                    allowChild = true;
                }

                if ((!Autonomous) && (allowChild) && RandomUtil.RandomChance01(kChanceToHavePlantSimBaby))
                {
                    client = Target.CreateStateMachine(Actor, out dummyIk);
                    mDummyIk = dummyIk;
                    Sim newBorn = GetNewBorn();
                    Relationship.Get(Actor, newBorn, true).LTR.ForceChangeState(LongTermRelationshipTypes.Friend);
                    if (newBorn.BridgeOrigin != null)
                    {
                        newBorn.BridgeOrigin.MakeRequest();
                        newBorn.BridgeOrigin = null;
                    }

                    if (client != null)
                    {
                        IGameObject actor = GlobalFunctions.CreateObjectOutOfWorld("plantSimHarvestable", ProductVersion.EP9, "Sims3.Gameplay.Core.Null", null);
                        client.SetActor("harvestable", actor);
                        client.SetActor("y", newBorn);
                        client.EnterState("x", "Enter Standing");
                        Target.SetGrowthState(PlantGrowthState.Planted);
                        client.RequestState("x", "HaveAPlantSimBaby");
                        Pregnancy.MakeBabyVisible(newBorn);
                        client.RequestState("x", "Exit Standing");
                        actor.RemoveFromWorld();
                        actor.Destroy();
                    }

                    if (Actor.IsSelectable)
                    {
                        OccultImaginaryFriend.DeliverDollToHousehold(new List<Sim>(new Sim[] { newBorn }));
                    }

                    ChildUtils.CarryChild(Actor, newBorn, true);
                    EventTracker.SendEvent(EventTypeId.kBornFromTheSoil, newBorn);
                }
                else
                {
                    client = Target.CreateStateMachine(Actor, out dummyIk);
                    mDummyIk = dummyIk;
                    bool hasHarvested = true;
                    if (Actor.IsInActiveHousehold)
                    {
                        hasHarvested = false;
                        foreach (SimDescription description in Actor.Household.SimDescriptions)
                        {
                            Gardening skill = description.SkillManager.GetSkill<Gardening>(SkillNames.Gardening);
                            if ((skill != null) && skill.HasHarvested())
                            {
                                hasHarvested = true;
                                break;
                            }
                        }
                    }

                    IGameObject obj3 = GlobalFunctions.CreateObjectOutOfWorld("plantForbiddenFruit", ProductVersion.EP9, "Sims3.Gameplay.Core.Null", null);
                    if (client != null)
                    {
                        client.SetActor("harvestable", obj3);
                        client.EnterState("x", "Enter Standing");
                        client.RequestState("x", "HaveAFruit");
                    }
                    Target.DoHarvest(Actor, hasHarvested, null);
                    Target.SetGrowthState(PlantGrowthState.Planted);
                    if (client != null)
                    {
                        client.RequestState("x", "Exit Standing");
                    }
                    obj3.RemoveFromWorld();
                    obj3.Destroy();
                }

                EndCommodityUpdates(true);
                StandardExit();
                Target.RemoveFromWorld();
                Target.Destroy();
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

        public new class Definition : ForbiddenFruitTree.HarvestPlantSim.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new HarvestPlantSimEx();
                result.Init(ref parameters);
                return result;
            }

            public override string GetInteractionName(Sim actor, HarvestPlant target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
