using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.TravelerSpace.Helpers;
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
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.Helpers
{
    public class MiniSimDescriptionEx
    {
        public static void AddMiniSims()
        {
            Common.StringBuilder msg = new Common.StringBuilder("AddMiniSims" + Common.NewLine);

            try
            {
                if (MiniSimDescription.sMiniSims == null)
                {
                    MiniSimDescription.sMiniSims = new Dictionary<ulong, MiniSimDescription>();
                }

                if (!Sims3.Gameplay.UI.Responder.Instance.ClockSpeedModel.GameSpeedLocked)
                {
                    Sims3.Gameplay.Gameflow.LockGameSpeed(Sims3.Gameplay.Gameflow.GameSpeed.Pause);
                }

                msg += "A";

                MiniSimDescription.sPendingSimUpdates = 0x0;

                Household activeHouse = Household.ActiveHousehold;
                if (GameStates.TravelHousehold != null)
                {
                    activeHouse = GameStates.TravelHousehold;
                }

                List<SimDescription> list = new List<SimDescription>();
                if ((activeHouse != null) && (GameStates.TravelerIds != null))
                {
                    foreach (ulong num in GameStates.TravelerIds)
                    {
                        list.Add(activeHouse.FindMember(num));
                    }
                }

                msg += "B";

                foreach (SimDescription description in SimDescription.GetSimDescriptionsInWorld())
                {
                    try
                    {
                        if (description.CelebrityManager == null)
                        {
                            description.Fixup();
                        }

                        if (!list.Contains(description))
                        {
                            MiniSims.FixUpForeignPregnantSims(description);
                        }

                        bool flag = MiniSimDescription.UpdateMiniSim(description);

                        bool hasRelationship = false;
                        if (!flag && !list.Contains(description))
                        {
                            foreach (SimDescription description2 in list)
                            {
                                if ((description2 != null) && (Relationship.Get(description, description2, false) != null))
                                {
                                    hasRelationship = true;
                                    break;
                                }
                            }
                        }

                        if (hasRelationship)
                        {
                            MiniSimDescription.sPendingSimUpdates++;
                            MiniSimDescription.MiniSimUpdater updater = new MiniSimDescription.MiniSimUpdater(description);

                            Common.FunctionTask.Perform(updater.UpdateMiniSim);
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(description, null, msg, e);
                    }
                }

                msg += "C";

                while (MiniSimDescription.sPendingSimUpdates > 0x0)
                {
                    SpeedTrap.Sleep();
                }

                msg += "D";
                if (MiniSimDescription.sMiniSims != null)
                {
                    msg += Common.NewLine + MiniSimDescription.sMiniSims.Count;
                }

                Sims3.Gameplay.Gameflow.UnlockGameSpeed();
            }
            catch (Exception e)
            {
                Common.Exception(msg, e);
            }
        }
    }
}
