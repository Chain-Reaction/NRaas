using NRaas.CommonSpace.Helpers;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Telemetry;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TravelerSpace.Interactions
{
    public class SitInMovingVanToTriggerTravelToUniversityEx : TravelUtil.SitInMovingVanToTriggerTravelToUniversity, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Lot, TravelUtil.SitInMovingVanToTriggerTravelToUniversity.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        private new void TriggerTravelToUniversityWorld()
        {
            RemoveTriggerAlarm();
            List<Sim> allTravelers = new List<Sim>(TravelingSims);
            allTravelers.Add(Actor);

            string reason = null;
            // Custom
            if (Helpers.TravelUtilEx.FinalBoardingCall(Actor.Household, allTravelers, WorldName.University, false, ref reason))
            {
                int travelDuration = (TravelDuration / 7) * Traveler.Settings.mUniversityTermLength;

                ForeignVisitorsSituation.ForceKillForeignVisitorsSituation();
                HolographicProjectionSituation.ForceKillHolographicVisitorsSituation();

                Camera.SetView(CameraView.MapView, false, true);
                int tripLength = (TravelUtil.sOverriddenTripLength > 0x0) ? TravelUtil.sOverriddenTripLength : travelDuration;
                int lastTimeAtUniversity = -2147483648;
                foreach (Sim sim in allTravelers)
                {
                    if (lastTimeAtUniversity < sim.DegreeManager.LastTimeAtUniversity)
                    {
                        lastTimeAtUniversity = sim.DegreeManager.LastTimeAtUniversity;
                    }
                }

                int numDaysSinceLastInDestWorld = -1;
                if (lastTimeAtUniversity >= 0x0)
                {
                    numDaysSinceLastInDestWorld = SimClock.ElapsedCalendarDays() - lastTimeAtUniversity;
                }

                // Custom
                GameStatesEx.TravelToVacationWorld(kUniversityWorldName, mTravelingSimsGuids, tripLength, numDaysSinceLastInDestWorld);
            }
            else
            {
                Actor.ShowTNSIfSelectable(TravelUtil.LocalizeString(Actor.IsFemale, "CantTravelTNS", new object[] { TravelCost }) + Common.NewLine + Common.NewLine + reason, StyledNotification.NotificationStyle.kSystemMessage, ObjectGuid.InvalidObjectGuid);
                Actor.ModifyFunds(TravelCost);

                foreach (Sim sim2 in allTravelers)
                {
                    if (!sim2.IsDying())
                    {
                        sim2.AddExitReason(ExitReason.CanceledByScript);
                    }
                }
            }
        }

        public override bool Run()
        {
            try
            {
                bool flag;
                try
                {
                    Actor.ModifyFunds(-TravelCost);
                    AssurePlayerThatBabiesAndToddlersAreFine();
                    if (RouteGroupToGatheringLocation())
                    {
                        Audio.StartObjectSound(Actor.ObjectId, "sting_ep9_goodbye_move", false);
                        if (CreateMovingVan() && !Target.IsWorldLot)
                        {
                            CreateAndCarrySuitCase();
                            mTriggerWorldTransition = AlarmManager.Global.AddAlarm((float)TravelUtil.kTravelToUniversityWorldTimeout, TimeUnit.Minutes, new AlarmTimerCallback(AddExitReasonToAllSims), "AlarmForceTransitionToUniversityWorld", AlarmType.AlwaysPersisted, Actor);
                            mRoute = Actor.CreateRoute();
                            Target.PlanToLot(mRoute);
                            Actor.DoRouteWithFollowers(mRoute, mAdditionalTravelingSims);
                            PutAwayAndDestorySuitCase(true);
                        }
                    }

                    //Custom
                    new Common.AlarmTask(2, TimeUnit.Minutes, TriggerTravelToUniversityWorld);

                    Traveler.SaveGame();

                    flag = true;
                }
                finally
                {
                    TravelUtil.PlayerMadeTravelRequest = false;
                }
                return flag;
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

        public new class Definition : TravelUtil.SitInMovingVanToTriggerTravelToUniversity.Definition
        {
            public override string GetInteractionName(Sim actor, Lot target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SitInMovingVanToTriggerTravelToUniversityEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
