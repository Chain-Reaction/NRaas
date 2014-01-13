using Sims3.Gameplay;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Scuba;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.CommonSpace.Helpers
{
    public class SimRoutingComponentEx
    {
        public static Route CreateRoute(SimRoutingComponent ths)
        {
            Route r = null;
            uint simFlags = ths.OwnerSim.SimDescription.SimFlags;
            r = Route.Create(ths.Owner.Proxy, simFlags);
            SetRouteOptions(ths, r, simFlags);
            return r;
        }

        public static Route CreateRoute(SimRoutingComponent ths, CASAgeGenderFlags ageGenderFlags)
        {
            Route r = null;
            uint num = (uint)ageGenderFlags;
            r = Route.Create(ths.Owner.Proxy, num);
            ths.SetRouteOptions(r, num);
            return r;
        }

        public static Route CreateRouteAsAdult(SimRoutingComponent ths)
        {
            Route r = null;
            uint ageGenderFlags = ths.OwnerSim.SimDescription.SimFlags & 0xffffff80;
            ageGenderFlags |= 0x20;
            r = Route.Create(ths.Owner.Proxy, ageGenderFlags);
            SetRouteOptions(ths, r, ageGenderFlags);
            return r;
        }

        public static void SetRouteOptions(SimRoutingComponent ths, Route r, uint ageGenderFlags)
        {
            SimDescription simDescription = ths.OwnerSim.SimDescription;
            Posture previousPosture = ths.OwnerSim.Posture;
            if (!(ths.OwnerSim.CurrentInteraction is IHasCustomRouteOptions))
            {
                r.SetOption(Route.RouteOption.BeginAsCar, (ths.OwnerSim.Parent is Vehicle) && !(ths.OwnerSim.Parent is IBoat));
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

                bool flag = false;
                if (GameUtils.IsInstalled(ProductVersion.EP10))
                {
                    IBoat parent = ths.OwnerSim.Parent as IBoat;
                    if (parent != null)
                    {
                        flag = true;
                        r.Follower = parent.Proxy;
                        r.FollowerAgeGenderSpecies = (uint)parent.GetBoatSpecies();
                        r.SetOption2(Route.RouteOption2.EnablePlanningAsBoat, true);
                        r.SetOption2(Route.RouteOption2.BeginAsBoat, true);
                        r.SetOption2(Route.RouteOption2.UseFollowerStartOrientation, true);
                        r.SetOption(Route.RouteOption.OffsetDestinationForLongAnimals, true);
                        r.AddObjectToIgnoreForRoute(ths.OwnerSim.ObjectId);
                    }

                    bool flag2 = OccultMermaid.IsEveryOneGroupedWithMeATeenOrAboveMermaid(ths.OwnerSim);
                    bool flag3 = false;
                    if (!flag2)
                    {
                        flag3 = ((previousPosture is CarryingChildPosture) || (previousPosture is CarryingPetPosture)) || ths.OwnerSim.Autonomy.SituationComponent.InSituationOfType(typeof(GoHereWithSituation));
                    }

                    bool flag4 = simDescription.ChildOrAbove & !(previousPosture is ScubaDiving);
                    flag4 &= !(ths.OwnerSim.InteractionQueue.GetHeadInteraction() is Lifeguard.GiveCPR);
                    flag4 &= !(ths.OwnerSim.InteractionQueue.GetHeadInteraction() is Lifeguard.FakeInjury);
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
                        r.SetOption2(Route.RouteOption2.RouteAsLifeguard, Lifeguard.ShouldUseRescueSwimWade(ths.OwnerSim));
                    }
                }

                if (GameUtils.IsInstalled(ProductVersion.EP3))
                {
                    r.SetOption(Route.RouteOption.EnableSubwayPlanning, true);
                }

                if (GameUtils.IsInstalled(ProductVersion.EP11)) /*&& GameUtils.IsFutureWorld())*/
                {
                    r.SetOption2(Route.RouteOption2.EnableHoverTrainPlanning, true);
                }

                if (GameUtils.IsInstalled(ProductVersion.EP5) && ((ths.OwnerSim.IsWildAnimal || ths.OwnerSim.IsStray) || (ths.OwnerSim.IsUnicorn || WildHorses.IsWildHorse(ths.OwnerSim))))
                {
                    r.SetOption2(Route.RouteOption2.EnablePlanningAsBoat, false);
                    r.SetOption2(Route.RouteOption2.DestinationMustBeOnLand, true);
                }

                if (ths.SimSatisfiesSpecialConditions())
                {
                    r.SetOption(Route.RouteOption.PassThroughObjects, true);
                    r.SetOption(Route.RouteOption.PassThroughWalls, true);
                }

                if (ths.OwnerSim.HasGhostBuff && !flag)
                {
                    r.SetOption(Route.RouteOption.RouteAsGhost, true);
                }

                if ((GameUtils.IsInstalled(ProductVersion.EP4) && (ths.OwnerSim.CarryingChildPosture != null)) && (Stroller.GetStroller(ths.OwnerSim, ths.OwnerSim.LotCurrent) != null))
                {
                    r.SetOption(Route.RouteOption.PlanUsingStroller, true);
                }

                if (ths.OwnerSim.IsHuman)
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

                if (GameUtils.IsInstalled(ProductVersion.EP8))
                {
                    // Custom
                    if (SimEx.GetOwnedAndUsableVehicle(ths.OwnerSim, ths.OwnerSim.LotHome, false, false, false, true) is CarUFO)
                    //if (OwnerSim.GetOwnedAndUsableVehicle(OwnerSim.LotHome, false, false, false, true) is CarUFO)
                    {
                        r.SetOption(Route.RouteOption.EnableUFOPlanning, true);
                    }
                    if ((ths.OwnerSim.IsHuman && ths.OwnerSim.SimDescription.ChildOrAbove) && PondManager.ArePondsFrozen())
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

                r.ExitReasonsInterrupt = unchecked((int)0xffa9bfff);
                if ((0x0 != (ageGenderFlags & 0x3)) && !ths.OwnerSim.LotCurrent.IsWorldLot)
                {
                    r.SetValidRooms(ths.OwnerSim.LotCurrent.LotId, null);
                }

                if ((ths.OwnerSim.IsHorse && ths.OwnerSim.SimDescription.AdultOrAbove) && !ths.OwnerSim.SimDescription.IsGhost)
                {
                    r.SetOption(Route.RouteOption.RouteAsLargeAnimal, true);
                }

                if (((ths.OwnerSim.IsHorse || ths.OwnerSim.IsDeer) || (ths.OwnerSim.IsFullSizeDog && ths.OwnerSim.SimDescription.AdultOrAbove)) && !ths.OwnerSim.SimDescription.IsGhost)
                {
                    r.SetOption(Route.RouteOption.OffsetDestinationForLongAnimals, true);
                }

                if (ths.OwnerSim.IsHorse && ths.OwnerSim.IsInBeingRiddenPosture)
                {
                    r.SetOption(Route.RouteOption.RouteWhileMounted, true);
                }

                if (previousPosture is LeadingHorsePosture)
                {
                    r.SetOption(Route.RouteOption.EnablePlanningAsCar, false);
                    r.SetOption(Route.RouteOption.RouteAsLargeAnimal, true);
                    r.SetOption(Route.RouteOption.RouteAsSimLeadingHorse, true);
                }

                if (ths.OwnerSim.IsPet && !ths.OwnerSim.IsInBeingRiddenPosture)
                {
                    r.SetOption(Route.RouteOption.IgnoreSidewalkAndLotRestrictions, true);
                }
            }
        }
    }
}