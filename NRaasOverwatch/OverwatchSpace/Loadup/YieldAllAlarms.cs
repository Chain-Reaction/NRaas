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
    public class YieldAllAlarms : ImmediateLoadupOption, Common.IWorldQuit
    {
        private static bool sYieldRequired = true;

        public override string GetTitlePrefix()
        {
            return "YieldAllAlarms";
        }

        public override void OnWorldLoadFinished()
        {
            Overwatch.Log(GetTitlePrefix());

            Sims3.Gameplay.Gameflow.GameSpeedChanged -= OnGameSpeedChanged;
            Sims3.Gameplay.Gameflow.GameSpeedChanged += OnGameSpeedChanged;

            new Common.AlarmTask(1, TimeUnit.Minutes, OnYieldAll, 15, TimeUnit.Minutes);
        }

        public void OnWorldQuit()
        {
            Sims3.Gameplay.Gameflow.GameSpeedChanged -= OnGameSpeedChanged;
        }

        private static void OnGameSpeedChanged(Sims3.Gameplay.Gameflow.GameSpeed newSpeed, bool locked)
        {
            sYieldRequired = (newSpeed <= Sims3.Gameplay.Gameflow.GameSpeed.Normal);

            OnYieldAll();
        }

        protected static void OnYieldAll()
        {           
            AlarmManager manager = AlarmManager.Global;

            if (manager == null)
            {
                return;
            }

            foreach (object item in manager.mTimerQueue)
            {
                AlarmManager.Timer timer = item as AlarmManager.Timer;
                if (timer == null) continue;

                timer.YieldRequired = sYieldRequired;
            }
        }
    }
}
