using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Interactions
{
    public class GoToSchoolInRabbitHoleEx : GoToSchoolInRabbitHole, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        // LoganDownUnder_Aussomedays_BEGIN
        // added tracking counters so slot reassignment will perform in a sequential loop
        private static int[] startCount = new int[4];
        //

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, GoToSchoolInRabbitHole.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<RabbitHole, GoToSchoolInRabbitHole.Definition>(Singleton);
        }

        protected void OnChangeOutfit()
        {
            if (Actor.Posture == Actor.Standing)
            {
                Actor.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToSchool);
            }
            else if (Actor.LotCurrent != Target.LotCurrent)
            {
                OutfitCategories categories;
                Actor.GetOutfitForClothingChange(Sim.ClothesChangeReason.GoingToSchool, out categories);
                Actor.OutfitCategoryToUseForRoutingOffLot = categories;
            }
        }

        // replaces the method from RabbitHole class - identical except for new slot reassignment code
        public bool RouteNearEntranceAndEnterRabbitHole(Sim a, RabbitHole.IRabbitHoleFollowers inst, RabbitHole.BeforeEnteringRabbitHoleDelegate beforeEntering, bool canUseCar, Route.RouteMetaType routeMetaType, bool playRouteFailure)
        {
            if ((Target.RabbitHoleProxy.EnterSlots.Count == 0) || (Target.RabbitHoleProxy.ExitSlots.Count == 0))
            {
                return false;
            }
            if (a.IsInRidingPosture && ((Target.RabbitHoleProxy.MountedEnterSlots.Count == 0) || (Target.RabbitHoleProxy.MountedExitSlots.Count == 0)))
            {
                return false;
            }
            List<Sim> list = new List<Sim>();
            CarryingChildPosture posture = a.Posture as CarryingChildPosture;
            if (posture != null)
            {
                list.Add(posture.Child);
            }
            if (inst != null)
            {
                if (inst.SimFollowers != null)
                {
                    foreach (Sim sim in inst.SimFollowers)
                    {
                        posture = sim.Posture as CarryingChildPosture;
                        if (posture != null)
                        {
                            list.Add(posture.Child);
                        }
                    }
                    foreach (Sim sim2 in list)
                    {
                        inst.AddFollower(sim2);
                    }
                }
                else if (list.Count > 0)
                {
                    inst.AddFollower(list[0]);
                }
            }
            bool flag = false;
            Route item = null;
            Sim parent = null;
            if (a.IsInRidingPosture)
            {
                if (a.Parent is Sim)
                {
                    parent = a.Parent as Sim;
                }
                item = parent.CreateRoute();
                item.ExecutionFromNonSimTaskIsSafe = true;
            }
            else
            {
                item = a.CreateRoute();
            }
            bool flag2 = false;
            int kRouteAttemptsToEnterRabbitHole = RabbitHole.kRouteAttemptsToEnterRabbitHole;
            Slot slotToUse = Slot.None;
            while ((kRouteAttemptsToEnterRabbitHole > 0) && !flag2)
            {
                kRouteAttemptsToEnterRabbitHole--;
                try
                {
                    Target.RabbitHoleProxy.ActiveEntryRoutes.Add(item);
                    if (playRouteFailure)
                    {
                        item.DoRouteFail = kRouteAttemptsToEnterRabbitHole == 0;
                    }
                    item.SetOption(Route.RouteOption.EnablePlanningAsCar, canUseCar);
                    if (Target.mGuid == RabbitHoleType.Subway)
                    {
                        item.SetOption(Route.RouteOption.EnableSubwayPlanning, false);
                    }
                    if (Target.mGuid == RabbitHoleType.HoverTrainStation)
                    {
                        item.SetOption2(Route.RouteOption2.EnableHoverTrainPlanning, false);
                    }
                    item.SetRouteMetaType(routeMetaType);
                    foreach (Slot slot2 in Target.RabbitHoleProxy.EnterSlots)
                    {
                        item.AddObjectToIgnoreForRoute(Target.RabbitHoleProxy.SlotToSlotInfo[slot2].Footprint.ObjectId);
                    }
                    foreach (Slot slot3 in Target.RabbitHoleProxy.MountedEnterSlots)
                    {
                        item.AddObjectToIgnoreForRoute(Target.RabbitHoleProxy.SlotToSlotInfo[slot3].Footprint.ObjectId);
                    }
                    item.PlanToSlot(Target.RabbitHoleProxy, (a.IsInRidingPosture ? Target.RabbitHoleProxy.MountedEnterSlots : Target.RabbitHoleProxy.EnterSlots).ToArray());
                    if (!item.PlanResult.Succeeded())
                    {
                        item.DoRouteFail = playRouteFailure;
                        if (a.IsInRidingPosture)
                        {
                            return parent.DoRoute(item);
                        }
                        return a.DoRoute(item);
                    }
                    slotToUse = (Slot)item.PlanResult.mDestSlotNameHash;

                    // Slot Reassignment
                    if ((Target.RabbitHoleProxy.EnterSlots.Count > 1) && (Target.RabbitHoleProxy.EnterSlots.Count <= 5))
                    {
                        slotToUse = ReassignSlot(Target.RabbitHoleProxy.EnterSlots.Count);
                    }
                    List<Sim> followers = (inst == null) ? null : inst.SimFollowers;
                    if ((!flag && (followers != null)) && ((followers.Count > 1) || ((followers.Count == 1) && (followers[0].Posture.Container != a))))
                    {
                        //if (RouteOutside(a, followers, this.RabbitHoleProxy.EnterSlots[0x0], false, true, true, false))
                        if (Target.RouteOutside(a, followers, Target.RabbitHoleProxy.EnterSlots[0], false, true, true, false))
                        {
                            flag = true;
                            flag2 = true;
                        }
                    }
                    else
                    {
                        //flag2 = RouteOutside(a, followers, none, false, true, false, false);
                        flag2 = Target.RouteOutside(a, followers, slotToUse, false, true, false, false);
                    }
                    if (!flag2 && (kRouteAttemptsToEnterRabbitHole > 0))
                    {
                        if (IntroTutorial.TutorialSim == a)
                        {
                            break;
                        }
                        a.RemoveExitReason(ExitReason.RouteFailed);
                        if (a.HasExitReason(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
                        {
                            break;
                        }
                        a.LoopIdle();
                        Simulator.Sleep((uint)SimClock.ConvertToTicks(RandomUtil.GetFloat(RabbitHole.kMinSimMinutesToSleepOnFailedRouteAttempt, RabbitHole.kMaxSimMinutesToSleepOnFailedRouteAttempt), TimeUnit.Minutes));
                    }
                    continue;
                }
                finally
                {
                    Target.RabbitHoleProxy.ActiveEntryRoutes.Remove(item);
                }
            }
            item = null;
            if (((a.ExitReason & ExitReason.HigherPriorityNext) == ExitReason.None) && ((a.ExitReason & ExitReason.UserCanceled) == ExitReason.None))
            {
                if (flag2)
                {
                    RabbitHole.NumSuccess++;
                }
                else
                {
                    RabbitHole.NumFail++;
                }
            }
            if (!flag2)
            {
                return flag2;
            }
            if ((beforeEntering != null) && !beforeEntering())
            {
                List<Sim> simFollowers = null;
                if (inst != null)
                {
                    simFollowers = inst.SimFollowers;
                }
                //RouteOutside(a, simFollowers, RandomUtil.GetRandomObjectFromList<Slot>(a.IsInRidingPosture ? this.RabbitHoleProxy.MountedEnterSlots : this.RabbitHoleProxy.EnterSlots), false, false, true, true);
                Target.RouteOutside(a, simFollowers, RandomUtil.GetRandomObjectFromList<Slot>(a.IsInRidingPosture ? Target.RabbitHoleProxy.MountedEnterSlots : Target.RabbitHoleProxy.EnterSlots), false, false, true, true);
                return false;
            }
            LeadingHorsePosture posture2 = a.Posture as LeadingHorsePosture;
            if (posture2 != null)
            {
                Sim container = posture2.Container as Sim;
                LeadingHorsePosture.ReleaseHorseFromLeadingPosture(a, container, false);
                Target.AnimateEnterRabbitHole(a, slotToUse, true, routeMetaType);
                Target.AnimateEnterRabbitHole(container, slotToUse, true, routeMetaType);
                return flag2;
            }
            return Target.AnimateEnterRabbitHole(a, slotToUse, true, routeMetaType);
        }

        public Slot ReassignSlot(int numberOfEntrySlots)
        {
            string startDoor = "Route_Entrance_Start_Door";
            if (numberOfEntrySlots == 2)
            {
                int slotCount = startCount[0];
                switch (slotCount)
                {
                    case 0:
                        startCount[0]++;
                        return Slots.Hash(startDoor + "0");
                    case 1:
                        startCount[0] = 0;
                        return Slots.Hash(startDoor + "2");
                }
            }
            if (numberOfEntrySlots == 3)
            {
                int slotCount = startCount[1];
                switch (slotCount)
                {
                    case 0:
                        startCount[1]++;
                        return Slots.Hash(startDoor + "0");
                    case 1:
                        startCount[1]++;
                        return Slots.Hash(startDoor + "2");
                    case 2:
                        startCount[1] = 0;
                        return Slots.Hash(startDoor + "4");
                }
            }
            if (numberOfEntrySlots == 4)
            {
                int slotCount = startCount[2];
                switch (slotCount)
                {
                    case 0:
                        startCount[2]++;
                        return Slots.Hash(startDoor + "0");
                    case 1:
                        startCount[2]++;
                        return Slots.Hash(startDoor + "2");
                    case 2:
                        startCount[2]++;
                        return Slots.Hash(startDoor + "4");
                    case 3:
                        startCount[2] = 0;
                        return Slots.Hash(startDoor + "6");
                }
            }
            if (numberOfEntrySlots == 5)
            {
                int slotCount = startCount[3];
                switch (slotCount)
                {
                    case 0:
                        startCount[3]++;
                        return Slots.Hash(startDoor + "0");
                    case 1:
                        startCount[3]++;
                        return Slots.Hash(startDoor + "2");
                    case 2:
                        startCount[3]++;
                        return Slots.Hash(startDoor + "4");
                    case 3:
                        startCount[3]++;
                        return Slots.Hash(startDoor + "6");
                    case 4:
                        startCount[3] = 0;
                        return Slots.Hash(startDoor + "8");
                }
            }
            return Slots.Hash(startDoor + "0");
        }
        // LoganDownUnder_Aussomedays_END

        public override bool RouteNearEntranceAndIntoBuilding(bool canUseCar, Route.RouteMetaType routeMetaType)
        {
            try
            {
                GoToSchoolInRabbitHoleHelper.PreRouteNearEntranceAndIntoBuilding(this, canUseCar, routeMetaType, OnChangeOutfit);

                //return Target.RouteNearEntranceAndEnterRabbitHole(Actor, this, BeforeEnteringRabbitHole, canUseCar, routeMetaType, true);
                return RouteNearEntranceAndEnterRabbitHole(Actor, this, BeforeEnteringRabbitHole, canUseCar, routeMetaType, true);
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

        public override bool InRabbitHole()
        {
            try
            {
                if (!GoToSchoolInRabbitHoleHelper.PreInRabbitholeLoop(this)) return false;

                bool succeeded = DoLoop(ExitReason.StageComplete, LoopDelegate, null);

                AfterschoolActivity activity = null;
                bool hasAfterschoolActivity = false;

                bool detention = false;
                bool fieldTrip = false;

                GoToSchoolInRabbitHoleHelper.PostInRabbitHoleLoop(this, ref succeeded, ref detention, ref fieldTrip, ref activity, ref hasAfterschoolActivity);

                if (detention && !fieldTrip)
                {
                    succeeded = DoLoop(ExitReason.StageComplete, LoopDelegate, null);
                }

                InteractionInstance.InsideLoopFunction afterSchoolLoop = null;
                GoToSchoolInRabbitHoleHelper.PostDetentionLoop(this, succeeded, detention, fieldTrip, activity, hasAfterschoolActivity, ref afterSchoolLoop);

                if (afterSchoolLoop != null)
                {
                    succeeded = DoLoop(ExitReason.StageComplete, afterSchoolLoop, mCurrentStateMachine);
                }
                else
                {
                    succeeded = DoLoop(ExitReason.StageComplete);
                }

                GoToSchoolInRabbitHoleHelper.PostAfterSchoolLoop(this, succeeded, activity, afterSchoolLoop);

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

        public new class Definition : GoToSchoolInRabbitHole.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GoToSchoolInRabbitHoleEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}

