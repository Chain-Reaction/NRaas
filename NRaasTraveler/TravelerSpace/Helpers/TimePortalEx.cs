using NRaas.CommonSpace.Booters;
using NRaas.TravelerSpace.CareerMergers;
using Sims3.Gameplay;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.TravelerSpace.Helpers
{
    public class TimePortalEx
    {
        public static bool TravelToFuture(TimePortal ths, Sim Actor, List<Sim> Followers, List<ulong> SimGuids)
        {
            // Custom
            string reason = null;
            if (TravelUtilEx.FinalBoardingCall(Actor.Household, Followers, WorldName.FutureWorld, false, ref reason))
            {
                new TravelControl(ths, Actor, Followers, SimGuids);

                Traveler.SaveGame();

                return true;
            }

            Actor.ShowTNSIfSelectable(Localization.LocalizeString(Actor.IsFemale, "Gameplay/Visa/TravelUtil:CantTravelFutureTNS", new object[0x0]) + Common.NewLine + Common.NewLine + reason, StyledNotification.NotificationStyle.kSystemMessage, ObjectGuid.InvalidObjectGuid);
            return false;
        }

        private static void CleanUpReservedVehicles(Sim sim, List<Sim> followers)
        {
            CleanUpReservedVehicle(sim);
            foreach (Sim sim2 in followers)
            {
                CleanUpReservedVehicle(sim2);
            }
        }
        private static void CleanUpReservedVehicle(Sim sim)
        {
            Vehicle reservedVehicle = sim.GetReservedVehicle();
            if (((reservedVehicle != null) && (sim.Inventory != null)) && !sim.Inventory.Contains(reservedVehicle))
            {
                sim.Inventory.TryToAdd(reservedVehicle);
                reservedVehicle.RemoveFromWorld();
                sim.SetReservedVehicle(null);
            }
        }

        public class TravelControl : Common.AlarmTask
        {
            TimePortal mPortal;

            Sim mActor;

            List<Sim> mFollowers;
            
            List<ulong> mSimGuids;

            public TravelControl(TimePortal ths, Sim Actor, List<Sim> Followers, List<ulong> SimGuids)
                : base(2, TimeUnit.Minutes)
            {
                mPortal = ths;
                mActor = Actor;
                mFollowers = Followers;
                mSimGuids = SimGuids;
            }

            protected override void OnPerform()
            {
                if (!mSimGuids.Contains(mActor.SimDescription.SimDescriptionId))
                {
                    mSimGuids.Add(mActor.SimDescription.SimDescriptionId);
                }

                ForeignVisitorsSituation.ForceKillForeignVisitorsSituation();
                HolographicProjectionSituation.ForceKillHolographicVisitorsSituation();
                Camera.SetView(CameraView.MapView, false, true);

                // Custom
                CleanUpReservedVehicles(mActor, mFollowers);

                if (mPortal != null)
                {
                    mPortal.StopActiveFX();
                }

                // Custom
                GameStatesEx.TravelToVacationWorld(WorldName.FutureWorld, mSimGuids, 0x0, 0x0);
                CauseEffectService.GetInstance().SetDepartureTimePortal(mPortal.ObjectId);
            }
        }
    }
}