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
    public class AddCommand : ConsoleOption
    {
        public AddCommand()
        { }

        public override string GetTitlePrefix()
        {
 	        return "AddConsoleCommand";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            while (true)
            {
                string command = StringInputDialog.Show(Name, Common.Localize("AddConsoleCommand:Prompt"), "", false, 1024);
                if (string.IsNullOrEmpty(command)) return OptionResult.SuccessRetain;

                if (Overwatch.Settings.mStoredCommands.Contains(command))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("AddConsoleCommand:Exists"));
                }
                else
                {
                    Overwatch.Settings.mStoredCommands.Add(command);
                    CommandSystem.ExecuteCommandString(command);
                }
            }
        }
    }
}
