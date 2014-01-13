using NRaas.CommonSpace.Helpers;
using NRaas.OverwatchSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class ReplaceAgingAlarm : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("ReplaceAgingAlarm");

            if (AgingManager.Singleton == null) return;

            AlarmManager.Global.RemoveAlarm(AgingManager.Singleton.DayPassedAlarm);

            AgingManager.Singleton.DayPassedAlarm = AlarmManager.Global.AddAlarmRepeating(24.1f - SimClock.HoursPassedOfDay, TimeUnit.Hours, DayPassedCallback, 1f, TimeUnit.Days, "AgingManager.DayPassed", AlarmType.NeverPersisted, null);

            Overwatch.Log("  Alarm Replaced");
        }

        public static void DayPassedCallback()
        {
            try
            {
                Common.DebugNotify("Overwatch:ReplaceAgingAlarm:DayPassedCallback");

                AgingManagerEx.DayPassedCallback(AgingManager.Singleton);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception("DayPassedCallback", e);
            }
        }
    }
}
