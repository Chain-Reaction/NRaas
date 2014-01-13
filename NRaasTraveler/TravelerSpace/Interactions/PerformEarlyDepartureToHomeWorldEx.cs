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
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
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
    public class PerformEarlyDepartureToHomeWorldEx : GameStates.PerformEarlyDepartureToHomeWorld, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Sim, GameStates.PerformEarlyDepartureToHomeWorld.Definition, Definition>(false);
            tuning.Availability.Teens = true;

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        public override bool Run()
        {
            try
            {
                Actor.PlaySoloAnimation("a_react_waveA_standing_x", true);
                EventTracker.SendEvent(new TravelUtil.TravelEvent(EventTypeId.kReturnedFromVacation, Actor, null, GameStates.sTravelData.mDestWorld, GameStates.sTravelData.mCurrentDayOfTrip));
                if (GameStates.sTravelData.mEarlyDepartures == null)
                {
                    GameStates.sTravelData.mEarlyDepartures = new List<Sim>();
                }

                GameStates.sTravelData.mEarlyDepartures.Add(Actor);

                if (Households.AllHumans(Actor.Household).Count == 0x1)
                {
                    GameStatesEx.UpdateTelemetryAndTriggerTravelBackToHomeWorld();
                    return true;
                }

                if (Actor.IsActiveSim)
                {
                    LotManager.SelectNextSim();
                }

                Actor.SimDescription.DnPExportData = new DnPExportData(Actor.SimDescription);
                if (Actor.OpportunityManager != null)
                {
                    Actor.OpportunityManager.StoreOpportunitiesForTravel();
                }

                if (Actor.BuffManager != null)
                {
                    Actor.BuffManager.StoreBuffsForTravel(Actor.SimDescription);
                }

                Actor.Household.RemoveSim(Actor);
                Actor.NullDnPManager();
                Actor.Motives.FreezeDecayEverythingExcept(new CommodityKind[0x0]);
                Actor.Autonomy.IncrementAutonomyDisabled();

                try
                {
                    Actor.DisablePieMenuOnSim = true;

                    if (DestinationLot != null)
                    {
                        Route r = Actor.CreateRoute();
                        r.DoRouteFail = false;
                        DestinationLot.PlanToLot(r);
                        Actor.DoRoute(r);
                    }

                    Actor.FadeOut(true, false, 1f);
                }
                finally
                {
                    Actor.DisablePieMenuOnSim = false;
                }
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
            finally
            {
                TravelUtil.PlayerMadeTravelRequest = false;
            }
            return false;
        }

        public new class Definition : GameStates.PerformEarlyDepartureToHomeWorld.Definition
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new PerformEarlyDepartureToHomeWorldEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
