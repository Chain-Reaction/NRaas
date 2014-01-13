using NRaas.CommonSpace.Helpers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Console
{
    public class RunAlarmCommands : RunCommands, IAlarmOption, IPersistence
    {
        public RunAlarmCommands()
        { }

        public override string GetTitlePrefix()
        {
            return "RunAlarmCommands";
        }

        protected override List<string> Commands
        {
            get 
            {
                return Overwatch.Settings.mAlarmCommands;
            }
        }

        public void PerformAlarm()
        {
            PerformAction(true);
        }

        protected override bool RunImmediate
        {
            get
            {
                return false;
            }
        }

        public void Import(Persistence.Lookup settings)
        {
            Overwatch.Settings.mAlarmCommands = new List<string>(settings.GetStringList("AlarmCommands"));
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("AlarmCommands", Overwatch.Settings.mAlarmCommands);
        }

        public string PersistencePrefix
        {
            get { return null; }
        }
    }
}
