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
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Services;
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
    public class UnicornPoolEx : Common.IDelayedWorldLoadFinished
    {
        public void OnDelayedWorldLoadFinished()
        {
            StartSpawnAlarm();
        }

        public static void StartSpawnAlarm()
        {
            Household petHousehold = Household.PetHousehold;
            if (petHousehold != null)
            {
                AlarmManager.Global.RemoveAlarm(UnicornPool.sSpawnAlarm);
                UnicornPool.sSpawnAlarm = AlarmManager.Global.AddAlarmDay(UnicornPool.kSpawnAlarmTime, ~DaysOfTheWeek.None, SpawnAlarmCallBack, "PetPoolAlarm", AlarmType.NeverPersisted, petHousehold);
            }
        }

        private static void SpawnAlarmCallBack()
        {
            try
            {
                if (Household.ActiveHousehold == null) return;

                UnicornPool.sSpawnAlarm = AlarmHandle.kInvalidHandle;
                if (RandomUtil.RandomChance01(UnicornPool.kChanceOfSpawningUnicorn))
                {
                    using (BaseWorldReversion reversion = new BaseWorldReversion())
                    {
                        UnicornPool.SpawnNPCUnicorn(false);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("SpawnAlarmCallBack", e);
            }
        }
     }
}
