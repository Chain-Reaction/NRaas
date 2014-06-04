using NRaas.CommonSpace;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Helpers
{
    public class SprinklerEx : Sprinkler, Common.IExitBuildBuy, Common.IWorldLoadFinished
    {
        public new void TurnOnUpgradedSprinkler()
        {
            if ((SeasonsManager.sInstance == null || (SeasonsManager.CurrentSeason != Season.Winter)) && (((base.Upgradable != null) && !this.TurnedOn) && (!this.OnOffChanging && base.Upgradable.AutoWater)))
            {
                this.TurnOnSprinkler();
                if (this.TurnedOn)
                {
                    this.AutoTurnedOn = true;
                }
            }
        }

        public void SetSprinklerAlarm(ISprinkler sprinkler)
        {
            Sprinkler waterThingy = sprinkler as Sprinkler;
            AlarmManager.Global.RemoveAlarm(waterThingy.mAutoWaterAlarm);
            waterThingy.mAutoWaterAlarm = AlarmHandle.kInvalidHandle;

            waterThingy.mAutoWaterAlarm = base.AddAlarmDay(kAutoWaterTimeOfDay, ~DaysOfTheWeek.None, new AlarmTimerCallback(this.TurnOnUpgradedSprinkler), "Auto Water Sprinkler On", AlarmType.NeverPersisted);
        }

        public void OnWorldLoadFinished()
        {
            foreach (ISprinkler sprinkler in Sims3.Gameplay.Queries.GetObjects<ISprinkler>())
            {
                this.SetSprinklerAlarm(sprinkler);
            }
        }

        public void OnExitBuildBuy(Lot lot)
        {
            foreach (ISprinkler sprinkler in lot.GetObjects<ISprinkler>())
            {
                this.SetSprinklerAlarm(sprinkler);
            }
        }
    }
}
