using NRaas.StoryProgressionSpace.Managers;
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
    public interface IAlarmScenario : Common.IStatGenerator
    {
        StoryProgressionObject Manager
        { set; }

        string UnlocalizedName
        { get; }

        IAlarmOwner Owner
        { get; }

        bool Test();

        void Fire();

        AlarmManagerReference SetupAlarm(IAlarmHandler alarms);
    }
}

