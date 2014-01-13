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
    public class RemoveCommand : ConsoleOption
    {
        public RemoveCommand()
        { }

        public override string GetTitlePrefix()
        {
            return "RemoveConsoleCommand";
        }

        protected override OptionResult Run(GameHitParameters< GameObject> parameters)
        {
            List<ConsoleItem> choices = SelectCommands(Name, NRaas.Overwatch.Settings.mStoredCommands);
            if ((choices == null) || (choices.Count == 0)) return OptionResult.Failure;

            foreach (ConsoleItem choice in choices)
            {
                NRaas.Overwatch.Settings.mStoredCommands.Remove(choice.Name);
                NRaas.Overwatch.Settings.mAlarmCommands.Remove(choice.Name);
                NRaas.Overwatch.Settings.mAutoCommands.Remove(choice.Name);
            }

            return OptionResult.SuccessRetain;
        }
    }
}
