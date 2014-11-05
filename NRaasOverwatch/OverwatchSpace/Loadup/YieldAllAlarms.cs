using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class YieldAllAlarms : ImmediateLoadupOption
    {
        public override string GetTitlePrefix()
        {
            return "YieldAllAlarms";
        }

        public override void OnWorldLoadFinished()
        {
            Overwatch.Log(GetTitlePrefix());

            new Common.AlarmTask(1, TimeUnit.Minutes, OnYieldAll, 15, TimeUnit.Minutes);
        }

        protected static void OnYieldAll()
        {
            //if (LoadingScreenController.IsLayoutLoaded()) return;   >>>Originally included in the OnGameSpeedChanged callback which is no longer used. Is this still needed?

            AlarmManager manager = AlarmManager.Global;

            if (manager == null || manager.mTimerQueue == null) return;         

            Sims3.Gameplay.Gameflow.GameSpeed currentGameSpeed = Sims3.Gameplay.Gameflow.CurrentGameSpeed;

            try
            {
                foreach (object item in manager.mTimerQueue)
                {
                    if (item == null) continue;

                    AlarmManager.Timer timer = item as AlarmManager.Timer;
                    if (timer == null) continue;

                    if (timer.Repeating && currentGameSpeed > Sims3.Gameplay.Gameflow.GameSpeed.Normal) continue;

                    timer.YieldRequired = true;
                }
            }
            catch (Exception e)
            {
                Common.Exception("", e);
            }
        }
    }
}
