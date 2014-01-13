using NRaas.CommonSpace.Helpers;
using NRaas.GoHereSpace.Helpers;
using NRaas.GoHereSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class VisitCommunityLotEx : VisitCommunityLot, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;
        static InteractionDefinition sOldDateSingleton;
        static InteractionDefinition sNoAdditionalActionsSingleton;

        static List<CommercialLotSubType> sAcceptableCommercialLotSubTypes;

        public virtual void OnPreLoad()
        {
            Tunings.Inject<Lot, VisitCommunityLot.Definition, Definition>(false);
            Tunings.Inject<Lot, VisitCommunityLot.DateDefinition, DateDefinition>(false);

            sOldSingleton = Singleton;
            sOldDateSingleton = DateSingleton;
            sNoAdditionalActionsSingleton = NoAdditionalActionsSingleton;

            Singleton = new Definition();
            DateSingleton = new DateDefinition();
            NoAdditionalActionsSingleton = new Definition(false);

            sAcceptableCommercialLotSubTypes = new List<CommercialLotSubType>(Sim.GoForWalkWithDog.kAcceptableCommercialLotSubTypes);
        }

        public virtual void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Lot, VisitCommunityLot.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                Definition interactionDefinition = InteractionDefinition as Definition;
                if (interactionDefinition != null)
                {
                    mLookForAutonomousActionsUponArrival = interactionDefinition.mLookForAutonomousActionsUponArrival;
                }
                else
                {
                    mLookForAutonomousActionsUponArrival = true;
                }

                if ((Target.CommercialLotSubType == CommercialLotSubType.kEP10_Resort) && 
                    (Actor.IsHuman) && 
                    (Actor.SimDescription.ChildOrAbove) && 
                    (Actor.LotHome == null) && 
                    (Autonomous) && 
                    (NumFollowers == 0) && 
                    (Actor.GetSituationOfType<Situation>() == null) && 
                    (!Actor.SimDescription.HasActiveRole) && 
                    (Actor.Service == null))
                {
                    IResortTower[] objects = Target.GetObjects<IResortTower>();
                    if (objects.Length > 0)
                    {
                        IResortTower randomObjectFromList = RandomUtil.GetRandomObjectFromList<IResortTower>(objects);
                        InteractionInstance instance = randomObjectFromList.GetExitTowerDefinition().CreateInstance(randomObjectFromList, Actor, new InteractionPriority(InteractionPriorityLevel.Autonomous), Autonomous, false);
                        if (Actor.InteractionQueue.PushAsContinuation(instance, false))
                        {
                            if (!Actor.Household.IsServiceNpcHousehold)
                            {
                                foreach (SimDescription description in Actor.Household.SimDescriptions)
                                {
                                    if ((description.IsHuman && description.ChildOrAbove) && (description.CreatedSim == null))
                                    {
                                        Sim actor = description.InstantiateOffScreen(Target);
                                        InteractionInstance entry = randomObjectFromList.GetExitTowerDefinition().CreateInstance(randomObjectFromList, actor, new InteractionPriority(InteractionPriorityLevel.Autonomous), Autonomous, false);
                                        actor.InteractionQueue.Add(entry);
                                    }
                                }
                            }
                            return true;
                        }
                    }
                }

                if ((Target.CommercialLotSubType == CommercialLotSubType.kEP10_Diving) && Actor.SimDescription.Child)
                {
                    return false;
                }

                if (!CarpoolManager.WaitForCarpool(Actor))
                {
                    return false;
                }

                if (ShouldReenqueueWithStandingPrecondition())
                {
                    ChildUtils.SetPosturePrecondition(this, CommodityKind.Standing, new CommodityKind[0x0]);
                    return Actor.InteractionQueue.PushAsContinuation(this, false);
                }

                if (mFollowers == null)
                {
                    if (InteractionDefinition is DateDefinition)
                    {
                        mFollowers = new List<Sim>();
                    }
                    else
                    {
                        mFollowers = GetFollowers(Actor);
                        if (Actor.IsNPC)
                        {
                            if (mFollowers.Count == 0)
                            {
                                Sim sim;
                                if (HorseManager.NpcShouldRideHorse(Actor, out sim))
                                {
                                    mFollowers.Add(sim);
                                }
                            }
                            else if ((Autonomous && RandomUtil.RandomChance01(Sim.GoForWalkWithDog.kChanceOfAutonomousDogWalk)) && sAcceptableCommercialLotSubTypes.Contains(Target.CommercialLotSubType))
                            {
                                foreach (Sim sim2 in new List<Sim>(mFollowers))
                                {
                                    if (Sim.GoForWalkWithDog.ActorCanWalkTarget(Actor, sim2))
                                    {
                                        InteractionInstance entry = Sim.GoForWalkWithDog.Singleton.CreateInstance(sim2, Actor, GetPriority(), true, false);
                                        (entry as Sim.GoForWalkWithDog).TargetLot = Target;
                                        Actor.InteractionQueue.AddNext(entry);
                                        return true;
                                    }
                                }
                            }

                            foreach (Sim sim3 in new List<Sim>(mFollowers))
                            {
                                GroupingSituation.StartGroupingSituation(Actor, sim3, false);
                            }
                        }
                        else
                        {
                            GroupingSituation situationOfType = Actor.GetSituationOfType<GroupingSituation>();
                            if ((situationOfType != null) && !situationOfType.ConfirmLeaveGroup(Actor, "Gameplay/Situations/GroupingSituation:ContinueVisitAloneInteraction"))
                            {
                                return false;
                            }
                        }
                    }
                }

                if (Sim.SwitchToFormalOutfitIfCocktail(Actor, Target))
                {
                    foreach (Sim sim in new List<Sim> (mFollowers))
                    {
                        sim.OutfitCategoryToUseForRoutingOffLot = OutfitCategories.Formalwear;
                    }
                }

                Occupation occupation = Actor.Occupation;
                if (((occupation != null) && (occupation.ActiveCareerLotID == Target.LotId)) && occupation.IsAllowedToWork())
                {
                    Actor.Occupation.EnsureSimHasOccupationOutfit();
                    if (Actor.Posture == Actor.Standing)
                    {
                        Actor.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToWork);
                    }
                    else
                    {
                        OutfitCategories categories;
                        Actor.GetOutfitForClothingChange(Sim.ClothesChangeReason.GoingToWork, out categories);
                        Actor.OutfitCategoryToUseForRoutingOffLot = categories;
                    }
                }

                bool flag2 = false;

                MetaAutonomyVenueType metaAutonomyVenueType = Target.LotCurrent.GetMetaAutonomyVenueType();

                // Custom
                if ((mFollowers.Count == 0) && (GoHereEx.Teleport.Perform(Actor, Target, true)))
                {
                    flag2 = true;
                }
                else
                {
                    Door door = Target.FindFrontDoor();
                    if (door != null)
                    {
                        bool wantToBeOutside = true;
                        if (Target.IsOpenVenue())
                        {
                            wantToBeOutside = false;
                        }

                        Route r = null;
                        Target.RouteToFrontDoor(Actor, Sim.MinDistanceFromDoorWhenVisiting, Sim.MaxDistanceFromDoorWhenGoingInside, ref door, wantToBeOutside, ref r, false);
                        if (r != null)
                        {
                            r.DoRouteFail = false;
                            flag2 = GoHereSpace.Helpers.SimRoutingComponentEx.DoRouteWithFollowers(Actor.SimRoutingComponent, r, mFollowers);
                            Lot.ValidateFollowers(mFollowers);
                        }
                    }
                }

                if (!flag2)
                {
                    Actor.RemoveExitReason(ExitReason.RouteFailed);
                    Route route = Actor.CreateRoute();
                    if (Autonomous && mLookForAutonomousActionsUponArrival)
                    {
                        bool flag4 = RandomUtil.RandomChance(Lot.ChanceOfGoingToCommunityLotByCar);
                        if (!flag4)
                        {
                            route.SetOption(Route.RouteOption.EnablePlanningAsCar, flag4);
                        }
                    }
                    route.SetRouteMetaType(Route.RouteMetaType.GoCommunityLot);

                    if ((metaAutonomyVenueType == MetaAutonomyVenueType.Diving) && Actor.SimDescription.IsMermaid)
                    {
                        Lot lotCurrent = Actor.LotCurrent;
                        if ((lotCurrent != null) && lotCurrent.IsHouseboatLot())
                        {
                            route.SetOption2(Sims3.SimIFace.Route.RouteOption2.EnablePlanningAsBoat, true);
                        }
                    }

                    Target.PlanToLotEx(route);
                    if (!route.PlanResult.Succeeded())
                    {
                        foreach (Shell shell in Target.GetObjects<Shell>())
                        {
                            route = shell.GetRouteToShell(Actor);
                            if (route.PlanResult.Succeeded())
                            {
                                break;
                            }
                        }
                    }

                    // Custom Function
                    flag2 = GoHereSpace.Helpers.SimRoutingComponentEx.DoRouteWithFollowers(Actor.SimRoutingComponent, route, mFollowers);
                    Lot.ValidateFollowers(mFollowers);
                }

                if (flag2)
                {
                    NumSuccess++;
                    if (Autonomous)
                    {
                        Target.MakeSimDoSomethingAfterArrivingAtVenue(metaAutonomyVenueType, Actor);
                        if (mFollowers != null)
                        {
                            foreach (Sim sim3 in new List<Sim>(mFollowers))
                            {
                                Target.MakeSimDoSomethingAfterArrivingAtVenue(metaAutonomyVenueType, sim3);
                            }
                        }
                    }

                    // Custom check
                    if ((!GoHere.Settings.DisallowAutoGroup(Actor)) && 
                        (Actor.Household != null) && 
                        (!GroupingSituation.DoesGroupingSituationExistForFamily(Actor)))
                    {
                        bool selectable = false;

                        if (SimTypes.IsSelectable(Actor))
                        {
                            selectable = true;
                        }
                        else
                        {
                            foreach(Sim follower in new List<Sim>(mFollowers))
                            {
                                if (SimTypes.IsSelectable(follower))
                                {
                                    selectable = true;
                                    break;
                                }
                            }
                        }

                        // Stop the romantic prompt from appearing
                        GroupingSituation situation = Actor.GetSituationOfType<GroupingSituation>();
                        if (situation != null)
                        {
                            if (situation.Participants == null)
                            {
                                situation.Exit();
                            }

                            if (!selectable)
                            {
                                situation.RomanticGrouping = false;
                            }
                        }

                        GroupingSituation.StartGroupingSitatuationWithFollowers(Actor, mFollowers);
                    }
                    return flag2;
                }

                if (((Actor.ExitReason & ExitReason.HigherPriorityNext) == ExitReason.None) && ((Actor.ExitReason & ExitReason.UserCanceled) == ExitReason.None))
                {
                    NumFail++;
                }
                return flag2;
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

        public new class Definition : VisitCommunityLot.Definition
        {
            public Definition()
            { }
            public Definition(bool lookForAutonomousActionsUponArrival)
                : base (lookForAutonomousActionsUponArrival)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new VisitCommunityLotEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, Lot target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous)
                {
                    if (!GoHere.Settings.AllowPush(a, target))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Allow Push Fail");
                        return false;
                    }
                }

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }

        public new class DateDefinition : VisitCommunityLot.DateDefinition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new VisitCommunityLotEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
