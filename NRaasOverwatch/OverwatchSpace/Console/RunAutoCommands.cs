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
    public class RunAutoCommands : RunCommands, Common.IDelayedWorldLoadFinished, IPersistence
    {
        public RunAutoCommands()
        { }

        public override string GetTitlePrefix()
        {
            return "RunAutoCommands";
        }

        protected override List<string> Commands
        {
            get 
            {
                return NRaas.Overwatch.Settings.mAutoCommands;
            }
        }

        public void OnDelayedWorldLoadFinished()
        {
            PerformAction(false);
        }

        protected override bool RunImmediate
        {
            get
            {
                return true;
            }
        }

        public void Import(Persistence.Lookup settings)
        {
            Overwatch.Settings.mAutoCommands = new List<string>(settings.GetStringList("AutoCommands"));
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("AutoCommands", Overwatch.Settings.mAutoCommands);
        }

        public string PersistencePrefix
        {
            get { return null; }
        }
    }
}
