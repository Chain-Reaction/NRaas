using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
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
using Sims3.Gameplay.Objects.PerformanceObjects;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.RouteDestinations;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public class ShowStageEx
    {
        public delegate void Logger(string text);

        public static void Cleanup(ShowStage stage, Logger log)
        {
            if (stage.mSlaveList == null)
            {
                stage.mSlaveList = new List<IShowSlave>();
            }

            for (int i = stage.mSlaveList.Count - 1; i >= 0; i--)
            {
                IShowSlave slave = stage.mSlaveList[i];

                if (slave == null)
                {
                    stage.mSlaveList.RemoveAt(i);

                    if (log != null)
                    {
                        log(" Null Slave Removed");
                    }
                }
                else
                {
                    ShowStage.WatchTheShow watch = slave as ShowStage.WatchTheShow;
                    if (watch != null)
                    {
                        if (watch.Actor == null)
                        {
                            stage.mSlaveList.RemoveAt(i);

                            if (log != null)
                            {
                                log(" Broken WatchTheShow Removed");
                            }
                        }
                    }
                }
            }

            StoreChanges(stage, log);
        }

        public static void StoreChanges(ShowStage stage, Logger log)
        {
            try
            {
                if ((stage.mSimThatSetupStage == null) && (stage.mPerformingSim == null))
                {
                    List<Slot> remove = new List<Slot>();

                    foreach (KeyValuePair<Slot, ObjectWithOrientation> pair in stage.mVenueStageLayout)
                    {
                        if (pair.Value == null) continue;

                        if (pair.Value.mGameObject == null)
                        {
                            remove.Add(pair.Key);
                        }
                        else if (pair.Value.mGameObject.Proxy == null)
                        {
                            remove.Add(pair.Key);
                        }
                    }

                    foreach (Slot slot in remove)
                    {
                        stage.mVenueStageLayout.Remove(slot);
                    }

                    stage.LoadStage();
                    stage.mVenueStageLayout.Clear();
                    stage.SaveStage(stage.mVenueStageLayout);

                    if (log != null)
                    {
                        log(" Stored Stage Changes");
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(stage, e);
            }
        }
    }
}
