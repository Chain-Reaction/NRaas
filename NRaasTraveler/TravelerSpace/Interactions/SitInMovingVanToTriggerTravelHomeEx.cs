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
    public class SitInMovingVanToTriggerTravelHomeEx : TravelUtil.SitInMovingVanToTriggerTravelHome, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Lot, TravelUtil.SitInMovingVanToTriggerTravelHome.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        private new static void TriggerTravelToHomeWorld()
        {
            SimpleMessageDialog.Show(TravelUtil.LocalizeString("TermOverCaption", new object[0x0]), TravelUtil.LocalizeString("TermOverText", new object[0x0]), ModalDialog.PauseMode.PauseSimulator);
            GameStatesEx.UpdateTelemetryAndTriggerTravelBackToHomeWorld();
        } 

        public override bool Run()
        {
            try
            {
                bool flag2;
                bool flag = false;
                try
                {
                    //Sims3.Gameplay.Gameflow.Singleton.DisableSave(this, "Ui/Caption/HUD/DisasterSaveError:Traveling");

                    flag = RouteGroupToGatheringLocation();
                    if (flag)
                    {
                        Audio.StartObjectSound(Actor.ObjectId, "sting_ep9_leave_univ", false);
                        flag = CreateMovingVan();
                        if (flag)
                        {
                            if (!Target.IsWorldLot)
                            {
                                CreateAndCarrySuitCase();
                                mTriggerWorldTransition = AlarmManager.Global.AddAlarm((float) TravelUtil.kTravelToUniversityWorldTimeout, TimeUnit.Minutes, AddExitReasonToAllSims, "AlarmForceTransitionToHomeWorld", AlarmType.AlwaysPersisted, Actor);
                                mRoute = Actor.CreateRoute();
                                Target.PlanToLot(mRoute);
                                Actor.DoRouteWithFollowers(mRoute, mAdditionalTravelingSims);
                                PutAwayAndDestorySuitCase(true);
                            }

                            if (!HandleEarlySingleDeparture(Actor))
                            {
                                Actor.SimDescription.IsDroppingOut = false;
                                foreach (Sim sim in mAdditionalTravelingSims)
                                {
                                    if (sim != Actor)
                                    {
                                        sim.SimDescription.IsDroppingOut = false;
                                    }
                                }

                                // Custom
                                Common.FunctionTask.Perform(TriggerTravelToHomeWorld);
                            }
                            else
                            {
                                Actor.SimDescription.IsDroppingOut = false;
                                foreach (Sim sim2 in mAdditionalTravelingSims)
                                {
                                    if (sim2 != Actor)
                                    {
                                        HandleEarlySingleDeparture(sim2);
                                        sim2.SimDescription.IsDroppingOut = false;
                                    }
                                }
                            }
                            flag = true;
                        }
                    }
                    flag2 = flag;
                }
                finally
                {
                    //Sims3.Gameplay.Gameflow.Singleton.EnableSave(this);
                    TravelUtil.PlayerMadeTravelRequest = false;
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
            }
            finally
            {
                TravelUtil.PlayerMadeTravelRequest = false;
            }
            return false;
        }

        public new class Definition : TravelUtil.SitInMovingVanToTriggerTravelHome.Definition
        {
            public override string GetInteractionName(Sim actor, Lot target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SitInMovingVanToTriggerTravelHomeEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
