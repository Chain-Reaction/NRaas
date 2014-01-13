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
    public class TriggerTravelBackToHomeWorldEx : TravelUtil.TriggerTravelBackToHomeWorld, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Sim, TravelUtil.TriggerTravelBackToHomeWorld.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        public override bool Run()
        {
            try
            {
                if (mbEveryoneLeaves)
                {
                    foreach (Sim sim in Households.AllSims(Household.ActiveHousehold))
                    {
                        UpdateRomancesAndVisaManager(sim);
                    }

                    // Calls custom function
                    Common.FunctionTask.Perform(GameStatesEx.UpdateTelemetryAndTriggerTravelBackToHomeWorld);
                    return true;
                }

                if ((mDepartingSims == null) || !IsValidEarlyDeparture())
                {
                    return false;
                }

                Lot destinationLotForEarlyDepartureSims = GetDestinationLotForEarlyDepartureSims();

                foreach (Sim sim2 in mDepartingSims)
                {
                    UpdateRomancesAndVisaManager(sim2);
                    GameStates.PerformEarlyDepartureToHomeWorld entry = GameStates.PerformEarlyDepartureToHomeWorld.Singleton.CreateInstance(sim2, sim2, new InteractionPriority(InteractionPriorityLevel.MaxDeath), false, false) as GameStates.PerformEarlyDepartureToHomeWorld;
                    if (entry != null)
                    {
                        entry.DestinationLot = destinationLotForEarlyDepartureSims;
                        sim2.InteractionQueue.Add(entry);
                    }
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

        public new class Definition : TravelUtil.TriggerTravelBackToHomeWorld.Definition
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TriggerTravelBackToHomeWorldEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
