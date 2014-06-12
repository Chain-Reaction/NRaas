using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Interfaces;
using NRaas.OverwatchSpace.Loadup;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Settings
{
    public class DisableFullMoonLightingSetting : BooleanOption
    {
        public override string GetTitlePrefix()
        {
            return "DisableFullMoonLighting";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mDisableFullMoonLighting;
            }
            set
            {
                NRaas.Overwatch.Settings.mDisableFullMoonLighting = value;

                if (!value)
                {
                    if (SimClock.IsTimeBetweenTimes(SimClock.CurrentTime().Hour, World.GetSunsetTime() + LunarCycleManager.kLunarEffectsDelayHours, World.GetSunriseTime()))
                    {
                        LunarCycleManager.TriggerLunarLighting();
                    }
                    else
                    {
                        LunarCycleManager.mLunarEffectsAlarm = AlarmManager.Global.AddAlarmDay(World.GetSunsetTime() + LunarCycleManager.kLunarEffectsDelayHours, ~DaysOfTheWeek.None, new AlarmTimerCallback(LunarCycleManager.TriggerLunarLighting), "Lunar Cycle Alarm", AlarmType.NeverPersisted, null);
                    }
                }
                else
                {                    
                    if (World.GetLunarPhase() == 0)
                    {
                        World.SetLightingValues(LightTuning.DefaultLighting);
                    }
                }
            }
        }
    }
}