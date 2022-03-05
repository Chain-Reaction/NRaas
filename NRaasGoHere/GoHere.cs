using NRaas.CommonSpace.Helpers;
using NRaas.GoHereSpace;
using NRaas.GoHereSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class GoHere : Common, Common.IPreLoad, Common.IWorldLoadFinished, Common.IDelayedWorldLoadFinished
    {
        static Common.MethodStore sStoryProgressionAllowPushToLot = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "AllowPushToLot", new Type[] { typeof(SimDescription), typeof(Lot) });

        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        static GoHere()
        {
            Bootstrap();
        }        

        public void OnPreLoad()
        {
            VisitSituation.kForceOutsideResetSendHomeTime = float.MaxValue;
            VisitSituation.kForceOutsideTeleportTime = float.MaxValue;
        }

        public static PersistedSettings Settings
        {
            get
            {
                if (sSettings == null)
                {
                    sSettings = new PersistedSettings();
                }

                return sSettings;
            }
        }

        public static void ResetSettings()
        {
            sSettings = null;
        }

        public void OnWorldLoadFinished()
        {
            kDebugging = Settings.Debugging;
            FilterHelper.kFilterCacheTime = GoHere.Settings.mFilterCacheTime;

            //FilterHelper.UpdateFilters();

            //DoorPortalComponentEx.DoorSettings.ValidateAndSetupDoors();

            Route.AboutToPlanCallback = (Route.AboutToPlanDelegate)Delegate.Combine(Route.AboutToPlanCallback, new Route.AboutToPlanDelegate(DoorPortalComponentEx.AboutToPlanRouteCallback));
            //Route.PostPlanCallback += DoorPortalComponentEx.OnPostPlan;

            if (!GoHere.Settings.mAllowBoatRouting)
            {
                Boat.kDistanceToDestinationSoSimWillBoat = 20000f;
            }
            else
            {
                GoHere.Settings.mOrigBoatRoutingDistance = Boat.kDistanceToDestinationSoSimWillBoat;
            }
        }

        public void OnDelayedWorldLoadFinished()
        {
            DoorPortalComponentEx.DoorSettings.ValidateAndSetupDoors();
        }

        public static bool ExternalAllowPush(SimDescription sim, Lot lot)
        {
            if (lot != null)
            {
                bool allowed = false;
                if (!lot.IsResidentialLot)
                {
                    List<Door> portals = lot.GetObjectsInRoom<Door>(0);
                    foreach (Door obj in portals)
                    {
                        DoorPortalComponentEx.DoorSettings settings = GoHere.Settings.GetDoorSettings(obj.ObjectId, false);
                        if (settings != null)
                        {
                            if (settings.IsSimAllowedThrough(sim.SimDescriptionId))
                            {
                                allowed = true;
                                break;
                            }
                        }
                        else
                        {
                            allowed = true;
                            break;
                        }
                    }
                }
                else
                {
                    Door door = lot.FindFrontDoor();
                    if (door != null)
                    {
                        DoorPortalComponentEx.DoorSettings settings = GoHere.Settings.GetDoorSettings(door.ObjectId, false);
                        if (settings != null)
                        {
                            if (settings.IsSimAllowedThrough(sim.SimDescriptionId))
                            {
                                allowed = true;
                            }
                        }
                        else
                        {
                            allowed = true;
                        }
                    }
                    else
                    {
                        allowed = true;
                    }
                }

                if (!allowed) return allowed;
            }

            if (!sStoryProgressionAllowPushToLot.Valid) return true;

            return sStoryProgressionAllowPushToLot.Invoke<bool>(new object[] { sim, lot });
        }
    }
}
