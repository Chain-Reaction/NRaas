using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;

namespace NRaas.TravelerSpace.Helpers
{
    public class WildHorsePoolEx : Common.IDelayedWorldLoadFinished
    {
        public void OnDelayedWorldLoadFinished()
        {
            StartAlarm();
        }

        public static void StartAlarm()
        {
            PetPool pool;
            if ((!Sims3.SimIFace.Environment.HasEditInGameModeSwitch) && PetPoolManager.TryGetPetPool(PetPoolType.WildHorse, out pool))
            {
                WildHorsePool pool2 = pool as WildHorsePool;
                if (pool2 != null)
                {
                    pool2.mWildHorseInstantiationInProgress = false;

                    AlarmManager.Global.RemoveAlarm(WildHorsePool.mHerdBehaviorAlarm);
                    WildHorsePool.mHerdBehaviorAlarm = AlarmManager.Global.AddAlarmRepeating(WildHorsePool.kHerdBehaviorFrequency, TimeUnit.Minutes, HerdBehaviorCallback, WildHorsePool.kHerdBehaviorFrequency, TimeUnit.Minutes, "WildHorseHerdBehavior", AlarmType.NeverPersisted, Household.PetHousehold);

                    AlarmManager.Global.AlarmWillYield(WildHorsePool.mHerdBehaviorAlarm);

                    WildHorsePool.sTimestampSinceLastLotChange = SimClock.CurrentTime();
                }
            }
        }

        private static void HerdBehaviorCallback()
        {
            try
            {
                using (BaseWorldReversion reversion = new BaseWorldReversion())
                {
                    PetPool pool;
                    if (!PetPoolManager.TryGetPetPool(PetPoolType.WildHorse, out pool)) return;

                    WildHorsePool ths = pool as WildHorsePool;

                    ths.HerdBehaviorCallback();
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception("HerdBehaviorCallback", e);
            }
        }
    }
}
