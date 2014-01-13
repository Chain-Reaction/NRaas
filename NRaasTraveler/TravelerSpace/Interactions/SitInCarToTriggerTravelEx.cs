using NRaas.CommonSpace.Helpers;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Telemetry;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.TravelerSpace.Interactions
{
    public class SitInCarToTriggerTravelEx : TravelUtil.SitInCarToTriggerTravel, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Lot, TravelUtil.SitInCarToTriggerTravel.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
                tuning.Availability.WorldRestrictionType = WorldRestrictionType.None;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        protected new void TriggerTravelToVacationWorld()
        {
            RemoveTriggerAlarm();

            List<Sim> allTravelers = new List<Sim>(Followers);
            allTravelers.Add(Actor);

            string reason = null;
            // Custom
            if ((TravelingSimGuids.Count == 0) || (Helpers.TravelUtilEx.FinalBoardingCall(Actor.Household, allTravelers, DestinationWorldName, false, ref reason)))
            {
                ForeignVisitorsSituation.ForceKillForeignVisitorsSituation();
                HolographicProjectionSituation.ForceKillHolographicVisitorsSituation();

                int lastTimeOnVacation = -2147483648;
                foreach (Sim sim in allTravelers)
                {
                    if (lastTimeOnVacation < sim.VisaManager.LastTimeOnVacation)
                    {
                        lastTimeOnVacation = sim.VisaManager.LastTimeOnVacation;
                    }
                }
                int numDaysSinceLastInDestWorld = -1;
                if (lastTimeOnVacation > 0)
                {
                    numDaysSinceLastInDestWorld = SimClock.ElapsedCalendarDays() - lastTimeOnVacation;
                }

                Camera.SetView(CameraView.MapView, false, true);
                int tripLength = (TravelUtil.sOverriddenTripLength > 0x0) ? TravelUtil.sOverriddenTripLength : TravelDuration;

                // Custom
                GameStatesEx.TravelToVacationWorld(DestinationWorldName, TravelingSimGuids, tripLength, numDaysSinceLastInDestWorld);

                TelemetryStats.VacationTelemetryInfo vacationTelemetryInfo = new TelemetryStats.VacationTelemetryInfo();
                vacationTelemetryInfo.LeavingHomeWorld = true;
                vacationTelemetryInfo.WorldId = DestinationWorldName;

                int num2 = 0x0;

                if (TravelingSimGuids.Count > 0)
                {
                    vacationTelemetryInfo.NumberOfSimsInHoushold = Households.NumSims(Actor.Household);
                    vacationTelemetryInfo.NumberOfSimsThatDidTravel = allTravelers.Count;

                    foreach (Sim sim in Households.AllSims(Actor.Household))
                    {
                        // Custom
                        if (CommonSpace.Helpers.TravelUtilEx.CheckForReasonsToFailTravel(sim.SimDescription, Traveler.Settings.mTravelFilter, DestinationWorldName, false, false) == null)
                        {
                            num2++;
                        }
                    }
                }
                else
                {
                    vacationTelemetryInfo.NumberOfSimsInHoushold = 0;
                    vacationTelemetryInfo.NumberOfSimsThatDidTravel = 0;
                }

                vacationTelemetryInfo.NumberOfSimsAbleToTravel = num2;
                vacationTelemetryInfo.VisaLevels = new PairedListDictionary<ulong, int>();
                foreach (Sim sim2 in allTravelers)
                {
                    int visaLevel = sim2.VisaManager.GetVisaLevel(DestinationWorldName);
                    vacationTelemetryInfo.VisaLevels.Add(sim2.SimDescription.SimDescriptionId, visaLevel);
                }

                vacationTelemetryInfo.TravelDateAndTime = SimClock.CurrentTime();
                EventTracker.SendEvent(new VacationInfoEvent(EventTypeId.kVacationTelemetryInfo, vacationTelemetryInfo));
            }
            else
            {
                if (DestinationWorldName == WorldName.FutureWorld)
                {
                    Actor.ShowTNSIfSelectable(TravelUtil.LocalizeString(Actor.IsFemale, "CantTravelFutureTNS", new object[] { TravelCost }), StyledNotification.NotificationStyle.kSystemMessage, ObjectGuid.InvalidObjectGuid);
                }
                else
                {
                    Actor.ShowTNSIfSelectable(TravelUtil.LocalizeString(Actor.IsFemale, "CantTravelTNS", new object[] { TravelCost }) + Common.NewLine + Common.NewLine + reason, StyledNotification.NotificationStyle.kSystemMessage, ObjectGuid.InvalidObjectGuid);
                }

                Actor.ModifyFunds(TravelCost);

                foreach (Sim sim3 in allTravelers)
                {
                    if (!sim3.IsDying())
                    {
                        sim3.AddExitReason(ExitReason.CanceledByScript);
                    }
                }
            }
        }

        public override bool Run()
        {
            try
            {
                if (Actor.FamilyFunds >= TravelCost)
                {
                    if (TravelingSimGuids.Count > 0)
                    {
                        Actor.ModifyFunds(-TravelCost);

                        AssurePlayerThatBabiesAndToddlersAreFine();

                        Boat.MakeSimExitAndParkBoatIfPossible(Actor);
                        if (Followers != null)
                        {
                            foreach (Sim sim in Followers)
                            {
                                Boat.MakeSimExitAndParkBoatIfPossible(sim);
                            }
                        }

                        if (!Target.IsWorldLot)
                        {
                            mTriggerWorldTransition = AlarmManager.Global.AddAlarm((float)TravelUtil.kTravelToVacationWorldTimeout, TimeUnit.Minutes, AddExitReasonToAllSims, "Travel to vacation world", AlarmType.AlwaysPersisted, Actor);
                            Route r = Actor.CreateRoute();
                            Target.PlanToLot(r);
                            Actor.DoRouteWithFollowers(r, Followers);
                        }
                    }

                    new Common.AlarmTask(2, TimeUnit.Minutes, TriggerTravelToVacationWorld);

                    Traveler.SaveGame();
                    return true;
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
            finally
            {
                TravelUtil.PlayerMadeTravelRequest = false;
            }
            return false;
        }

        public new class Definition : TravelUtil.SitInCarToTriggerTravel.Definition
        {
            public override string GetInteractionName(Sim actor, Lot target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SitInCarToTriggerTravelEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
