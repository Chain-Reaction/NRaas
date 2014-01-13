using NRaas.CommonSpace.Helpers;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Telemetry;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.TravelerSpace.Interactions
{
    public class SitInCarToTriggerMoveEx : MovingWorldUtil.SitInCarToTriggerMove, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Lot, MovingWorldUtil.SitInCarToTriggerMove.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        protected void TriggerMoveToNewWorldEx()
        {
            RemoveTriggerAlarm();
            List<Sim> allTravelers = new List<Sim>(Followers);
            allTravelers.Add(Actor);

            // Custom
            string reason = null;
            if (Helpers.TravelUtilEx.FinalBoardingCall(Actor.Household, allTravelers, WorldName.Undefined, true, ref reason))
            {
                ForeignVisitorsSituation.ForceKillForeignVisitorsSituation();
                HolographicProjectionSituation.ForceKillHolographicVisitorsSituation();
                Camera.SetView(CameraView.MapView, false, true);

                // Custom
                GameStatesEx.MoveToNewWorld(DestinationWorldName, TravelingSimGuids, mMovingWorldSaved, mPackFurniture);
            }
            else
            {
                GameStates.WorldMoveRequested = false;
                Actor.ShowTNSIfSelectable(Common.LocalizeEAString(Actor.IsFemale, "Gameplay/Visa/TravelUtil:ForceMoveCancel") + Common.NewLine + Common.NewLine + reason, StyledNotification.NotificationStyle.kSystemMessage, ObjectGuid.InvalidObjectGuid);
                foreach (Sim sim in allTravelers)
                {
                    if (!sim.IsDying())
                    {
                        sim.AddExitReason(ExitReason.CanceledByScript);
                    }
                }
            }
        }

        public override bool Run()
        {
            try
            {
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
                    mTriggerWorldTransition = AlarmManager.Global.AddAlarm(90f, TimeUnit.Minutes, AddExitReasonToAllSims, "Travel to outside world", AlarmType.AlwaysPersisted, Actor);
                    mTaxi = Vehicle.CreateTaxi();
                    mRoute = Actor.CreateRoute();
                    Target.PlanToLot(mRoute);
                    Actor.DoRouteWithFollowers(mRoute, Followers, GoHereWithSituation.OnFailBehavior.RemoveFromWorld, Vector3.Invalid);
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
                // Custom
                TriggerMoveToNewWorldEx();
            }

            return false;
        }

        public new class Definition : MovingWorldUtil.SitInCarToTriggerMove.Definition
        {
            public override string GetInteractionName(Sim actor, Lot target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SitInCarToTriggerMoveEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
