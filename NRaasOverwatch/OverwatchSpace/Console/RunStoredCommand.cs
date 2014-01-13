using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Console
{
    public class RunStoredCommand : ConsoleOption, IPersistence
    {
        public RunStoredCommand()
        { }

        public override string GetTitlePrefix()
        {
            return "RunStoredCommand";
        }

        protected override OptionResult Run(GameHitParameters< GameObject> parameters)
        {
            List<ConsoleItem> choices = SelectCommands(Name, NRaas.Overwatch.Settings.mStoredCommands);
            if ((choices == null) || (choices.Count == 0)) return OptionResult.Failure;

            foreach (ConsoleItem choice in choices)
            {
                try
                {
                    CommandSystem.ExecuteCommandString(choice.Name);
                }
                catch (Exception e)
                {
                    Common.Exception(choice.Name, e);
                }
            }

            return OptionResult.SuccessClose;
        }

        public void Import(Persistence.Lookup settings)
        {
            Overwatch.Settings.mStoredCommands = new List<string> (settings.GetStringList("StoredCommands"));
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("StoredCommands", Overwatch.Settings.mStoredCommands);
        }

        public string PersistencePrefix
        {
            get { return null; }
        }
    }
}
