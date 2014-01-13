using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class Pregnancies
    {
        public static Pregnancy Start(Sim woman, SimDescription man, bool handlePlantSim)
        {
            if (handlePlantSim)
            {
                if ((SimTypes.IsSelectable(woman)) || (SimTypes.IsSelectable(man)))
                {
                    if (woman.SimDescription.IsPlantSim || man.IsPlantSim)
                    {
                        IGameObject obj2 = GlobalFunctions.CreateObjectOutOfWorld("forbiddenFruit", ProductVersion.EP9, "Sims3.Gameplay.Objects.Gardening.ForbiddenFruit", null);
                        if (obj2 != null)
                        {
                            Inventories.TryToMove(obj2, woman);
                            Audio.StartSound("sting_baby_conception");
                        }

                        return null;
                    }
                }
            }

            AgingManager.Singleton.CancelAgingAlarmsForSim(woman.SimDescription.AgingState);
            Pregnancy p = null;

            if (woman.IsHuman)
            {
                p = new Pregnancy(woman, man);
            }
            else
            {
                p = new PetPregnancy(woman, man);
            }

            p.PreggersAlarm = woman.AddAlarmRepeating(1f, TimeUnit.Hours, p.HourlyCallback, 1f, TimeUnit.Hours, "Hourly Pregnancy Update Alarm", AlarmType.AlwaysPersisted);
            woman.SimDescription.Pregnancy = p;

            EventTracker.SendEvent(new PregnancyEvent(EventTypeId.kGotPregnant, woman, man.CreatedSim, p, null));
            return p;
        }
    }
}

