using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace
{
    public abstract class BooleanAlarmOptionItem<TManager,TScenario> : BooleanManagerOptionItem<TManager>, IScenarioOptionItem, IAfterImportOptionItem
        where TManager : Manager
        where TScenario : Scenario, IAlarmScenario, new()
    {
        AlarmManagerReference mAlarm;

        public BooleanAlarmOptionItem(bool value)
            : base(value)
        { }

        public Scenario GetScenario()
        {
            return new TScenario();
        }

        public StoryProgressionObject GetManager()
        {
            return Manager;
        }

        public void PerformAfterImport()
        {
            RestartAlarm();
        }

        public void RestartAlarm()
        {
            if (mAlarm != null)
            {
                mAlarm.Dispose();
                mAlarm = null;
            }

            PrivateStartup();
        }

        protected override void PrivateStartup()
        {
            mAlarm = Manager.AddAlarm(new TScenario(), false);
        }
    }
}
