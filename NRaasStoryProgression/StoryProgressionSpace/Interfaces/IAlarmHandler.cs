using NRaas.StoryProgressionSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interfaces
{
    public interface IAlarmHandler
    {
        AlarmManagerReference AddAlarm(IAlarmScenario scenario, float time);
        AlarmManagerReference AddAlarm(IAlarmScenario scenario, float time, TimeUnit units);
        AlarmManagerReference AddAlarmDelayed(IAlarmScenario scenario, float repeatTime, TimeUnit repeatUnit);
        AlarmManagerReference AddAlarmImmediate(IAlarmScenario scenario, float repeatTime, TimeUnit repeatUnit);
        AlarmManagerReference AddAlarmDay(IAlarmScenario scenario, float hourOfDay);
        AlarmManagerReference AddAlarmDay(IAlarmScenario scenario, float hourOfDay, DaysOfTheWeek days);
    }
}

