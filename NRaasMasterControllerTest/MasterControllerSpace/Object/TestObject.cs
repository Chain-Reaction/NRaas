using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.PerformanceObjects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Scuba;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Object
{
    public class TestObject : OptionItem, IObjectOption
    {
        public override string GetTitlePrefix()
        {
            return "TestObject";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Common.StringBuilder msg = new Common.StringBuilder("Run");

            try
            {
                Terrain terrain = parameters.mTarget as Terrain;
                if (terrain != null)
                {
                    foreach (Sim sim in LotManager.Actors)
                    {
                        Common.TestSpan span = Common.TestSpan.CreateSimple();

                        Route r = null;

                        uint simFlags = sim.SimDescription.SimFlags;

                        try
                        {
                            r = Route.Create(sim.Proxy, simFlags);
                        }
                        finally
                        {
                            msg += Common.NewLine + sim.FullName;
                            msg += Common.NewLine + " " + sim.SimDescription.mSimFlags;
                            msg += Common.NewLine + " " + span.Duration;
                        }

                        SimDescription simDescription = sim.SimDescription;
                        Posture previousPosture = sim.Posture;

                        SimRoutingComponent routingComponent = sim.RoutingComponent as SimRoutingComponent;

                        bool flag = false;

                        span = Common.TestSpan.CreateSimple();
                        try
                        {
                            //routingComponent.SetRouteOptions(route, simFlags);

                            if (!(sim.CurrentInteraction is IHasCustomRouteOptions))
                            {
                                r.SetOption(Route.RouteOption.BeginAsCar, (sim.Parent is Vehicle) && !(sim.Parent is IBoat));
                                r.SetOption(Route.RouteOption.EnablePlanningAsCar, simDescription.ChildOrAbove);
                                r.SetOption(Route.RouteOption.DisablePlanningAsPedestrian, false);
                                r.SetOption(Route.RouteOption.PushSimsAtDestination, true);
                                r.SetOption(Route.RouteOption.UseAutoSlotFootprintLocking, true);
                                r.SetOption(Route.RouteOption.ReplanToFindObstacleWhenPathPlanFails, true);
                                r.SetOption(Route.RouteOption.IgnoreParent, false);
                                r.SetOption(Route.RouteOption.IgnoreChildren, false);
                                r.SetOption(Route.RouteOption.CreateSubPaths, true);
                                r.SetOption(Route.RouteOption.CheckForFootprintsNearGoals, true);
                                r.SetOption(Route.RouteOption.PenalizeGoalsOnDifferentLevels, true);
                                if (GameUtils.IsInstalled(ProductVersion.EP10))
                                {
                                    IBoat parent = sim.Parent as IBoat;
                                    if (parent != null)
                                    {
                                        flag = true;
                                        r.Follower = parent.Proxy;
                                        r.FollowerAgeGenderSpecies = (uint)parent.GetBoatSpecies();
                                        r.SetOption2(Route.RouteOption2.EnablePlanningAsBoat, true);
                                        r.SetOption2(Route.RouteOption2.BeginAsBoat, true);
                                        r.SetOption2(Route.RouteOption2.UseFollowerStartOrientation, true);
                                        r.SetOption(Route.RouteOption.OffsetDestinationForLongAnimals, true);
                                        r.AddObjectToIgnoreForRoute(sim.ObjectId);
                                    }
                                    bool flag2 = OccultMermaid.IsEveryOneGroupedWithMeATeenOrAboveMermaid(sim);
                                    bool flag3 = false;
                                    if (!flag2)
                                    {
                                        flag3 = ((previousPosture is CarryingChildPosture) || (previousPosture is CarryingPetPosture)) || sim.Autonomy.SituationComponent.InSituationOfType(typeof(GoHereWithSituation));
                                    }
                                    bool flag4 = simDescription.ChildOrAbove & !(previousPosture is ScubaDiving);
                                    flag4 &= !(sim.InteractionQueue.GetHeadInteraction() is Lifeguard.GiveCPR);
                                    flag4 &= !(sim.InteractionQueue.GetHeadInteraction() is Lifeguard.FakeInjury);
                                    flag4 &= !(simDescription.CreatedByService is GrimReaper);
                                    flag4 &= !flag2 || flag3;
                                    flag4 &= !(previousPosture is BeingRiddenPosture) && !(previousPosture is RidingPosture);
                                    bool flag5 = simDescription.ChildOrAbove && simDescription.IsHuman;
                                    flag5 &= !(previousPosture is CarryingChildPosture) && !(previousPosture is CarryingPetPosture);
                                    r.SetOption(Route.RouteOption.EnableWaterPlanning, flag5);
                                    r.SetOption2(Route.RouteOption2.DestinationMustBeOnLand, !flag5);
                                    r.SetOption2(Route.RouteOption2.EnablePlanningAsBoat, (parent != null) || flag4);
                                    if (flag2 && flag5)
                                    {
                                        r.SetOption2(Route.RouteOption2.RouteAsMermaid, true);
                                    }
                                    else
                                    {
                                        r.SetOption2(Route.RouteOption2.RouteAsLifeguard, Lifeguard.ShouldUseRescueSwimWade(sim));
                                    }
                                }

                                // Cut

                                if (GameUtils.IsInstalled(ProductVersion.Undefined | ProductVersion.EP3))
                                {
                                    r.SetOption(Route.RouteOption.EnableSubwayPlanning, true);
                                }
                                if (GameUtils.IsInstalled(ProductVersion.Undefined | ProductVersion.EP5) && ((sim.IsWildAnimal || sim.IsStray) || (sim.IsUnicorn || WildHorses.IsWildHorse(sim))))
                                {
                                    r.SetOption2(Route.RouteOption2.EnablePlanningAsBoat, false);
                                    r.SetOption2(Route.RouteOption2.DestinationMustBeOnLand, true);
                                }
                                if (routingComponent.SimSatisfiesSpecialConditions())
                                {
                                    r.SetOption(Route.RouteOption.PassThroughObjects, true);
                                    r.SetOption(Route.RouteOption.PassThroughWalls, true);
                                }
                                if (sim.HasGhostBuff && !flag)
                                {
                                    r.SetOption(Route.RouteOption.RouteAsGhost, true);
                                }
                                if ((GameUtils.IsInstalled(ProductVersion.Undefined | ProductVersion.EP4) && (sim.CarryingChildPosture != null)) && (Stroller.GetStroller(sim, sim.LotCurrent) != null))
                                {
                                    r.SetOption(Route.RouteOption.PlanUsingStroller, true);
                                }
                                if (sim.IsHuman)
                                {
                                    SwimmingInPool pool = previousPosture as SwimmingInPool;
                                    if (pool != null)
                                    {
                                        if (pool.ContainerIsOcean)
                                        {
                                            r.SetOption(Route.RouteOption.EnableWaterPlanning, true);
                                        }
                                    }
                                    else
                                    {
                                        Ocean.PondAndOceanRoutingPosture posture2 = previousPosture as Ocean.PondAndOceanRoutingPosture;
                                        if ((posture2 != null) && (posture2.WalkStyleToUse == Sim.WalkStyle.Wade))
                                        {
                                            r.SetOption(Route.RouteOption.EnableWaterPlanning, true);
                                        }
                                    }
                                }

                                // Cut

                                if (GameUtils.IsInstalled(ProductVersion.EP8))
                                {
                                    if (SimEx.GetOwnedAndUsableVehicle(sim, sim.LotHome, false, false, false, true) is CarUFO)
                                    {
                                        r.SetOption(Route.RouteOption.EnableUFOPlanning, true);
                                    }

                                    if ((sim.IsHuman && sim.SimDescription.ChildOrAbove) && PondManager.ArePondsFrozen())
                                    {
                                        bool flag6 = true;
                                        while (previousPosture != null)
                                        {
                                            if (previousPosture.Satisfaction(CommodityKind.Standing, null) <= 0f)
                                            {
                                                flag6 = false;
                                                break;
                                            }
                                            previousPosture = previousPosture.PreviousPosture;
                                        }
                                        if (flag6)
                                        {
                                            r.SetOption(Route.RouteOption.EnablePondPlanning, true);
                                        }
                                    }
                                }
                            }
                        }
                        finally
                        {
                            msg += Common.NewLine + " B " + span.Duration;
                        }

                        span = Common.TestSpan.CreateSimple();
                        try
                        {
                            if (!(sim.CurrentInteraction is IHasCustomRouteOptions))
                            {
                                r.ExitReasonsInterrupt = unchecked ((int)0xffa9bfff);
                                if ((0x0 != (simFlags & 0x3)) && !sim.LotCurrent.IsWorldLot)
                                {
                                    r.SetValidRooms(sim.LotCurrent.LotId, null);
                                }
                                if ((sim.IsHorse && sim.SimDescription.AdultOrAbove) && !sim.SimDescription.IsGhost)
                                {
                                    r.SetOption(Route.RouteOption.RouteAsLargeAnimal, true);
                                }
                                if (((sim.IsHorse || sim.IsDeer) || (sim.IsFullSizeDog && sim.SimDescription.AdultOrAbove)) && !sim.SimDescription.IsGhost)
                                {
                                    r.SetOption(Route.RouteOption.OffsetDestinationForLongAnimals, true);
                                }
                                if (sim.IsHorse && sim.IsInBeingRiddenPosture)
                                {
                                    r.SetOption(Route.RouteOption.RouteWhileMounted, true);
                                }
                                if (previousPosture is LeadingHorsePosture)
                                {
                                    r.SetOption(Route.RouteOption.EnablePlanningAsCar, false);
                                    r.SetOption(Route.RouteOption.RouteAsLargeAnimal, true);
                                    r.SetOption(Route.RouteOption.RouteAsSimLeadingHorse, true);
                                }
                                if (sim.IsPet && !sim.IsInBeingRiddenPosture)
                                {
                                    r.SetOption(Route.RouteOption.IgnoreSidewalkAndLotRestrictions, true);
                                }
                            }

                            //route = sim.CreateRoute();
                        }
                        finally
                        {
                            msg += Common.NewLine + " C " + span.Duration;
                        }
                    }
                }

                AcademicTextBook book = parameters.mTarget as AcademicTextBook;
                if (book != null)
                {
                    Sim a = parameters.mActor as Sim;

                    GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                    if ((a.OccupationAsAcademicCareer != null) || !book.mRequireAcademics)
                    {
                        if (book.TestRockAndRead(a, false, ref greyedOutTooltipCallback))
                        {
                            msg += Common.NewLine + "A";
                        }
                        if (a.Inventory.Contains(parameters.mTarget) && !(a.Posture is Sim.StandingPosture))
                        {
                            if (book.TestReadBook(a, false, ref greyedOutTooltipCallback))
                            {
                                msg += Common.NewLine + "B";

                            }
                        }
                        if (a.GetObjectInRightHand() == parameters.mTarget)
                        {
                            if (book.TestReadBook(a, false, ref greyedOutTooltipCallback))
                            {
                                msg += Common.NewLine + "C";
                            }
                        }

                        msg += Common.NewLine + "D";
                    }

                    msg += Common.NewLine + "E";
                }

                CommonDoor door = parameters.mTarget as CommonDoor;
                if (door != null)
                {
                    if (door.mObjComponents != null)
                    {
                        msg += Common.NewLine + "Count: " + door.mObjComponents.Count;

                        for (int i = 0x0; i < door.mObjComponents.Count; i++)
                        {
                            msg += Common.NewLine + door.mObjComponents[i].BaseType;

                            if (door.mObjComponents[i].BaseType == typeof(PortalComponent))
                            {
                                msg += Common.NewLine + " Found";

                                door.mObjComponents[i].Dispose();
                                door.mObjComponents.RemoveAt(i);
                                break;
                            }
                        }
                        if (door.mObjComponents.Count == 0x0)
                        {
                            door.mObjComponents = null;
                        }
                    }
                }

                HarvestPlant plant = parameters.mTarget as HarvestPlant;
                if (plant != null)
                {
                    msg += Common.NewLine + "mBarren: " + plant.mBarren;
                    msg += Common.NewLine + "mGrowthState: " + plant.mGrowthState;
                    msg += Common.NewLine + "mDormant: " + plant.mDormant;
                    msg += Common.NewLine + "mLeaflessState: " + plant.mLeaflessState;
                    msg += Common.NewLine + "mLifetimeHarvestablesYielded: " + plant.mLifetimeHarvestablesYielded;
                    msg += Common.NewLine + "NumLifetimeHarvestables: " + plant.PlantDef.NumLifetimeHarvestables;
                    msg += Common.NewLine + "Yield: " + plant.GetYield();
                }
            }
            catch (Exception e)
            {
                GameHitParameters<GameObject>.Exception(parameters, msg, e);
            }
            finally
            {
                Common.DebugWriteLog(msg);
            }

            return OptionResult.SuccessClose;
        }

        public static int OnSort(KeyValuePair<Sim, float> left, KeyValuePair<Sim, float> right)
        {
            return left.Value.CompareTo(right.Value);
        }
    }
}
