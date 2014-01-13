using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Console
{
    public abstract class RunCommands : ConsoleOption, IActionOption
    {
        public RunCommands()
        { }

        protected abstract List<string> Commands
        {
            get;
        }

        protected abstract bool RunImmediate
        {
            get;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<ConsoleItem> choices = SelectCommands(Name, Commands);
            if ((choices == null) || (choices.Count == 0)) return OptionResult.Failure;

            foreach (ConsoleItem choice in choices)
            {
                if (Commands.Contains(choice.Value))
                {
                    Commands.Remove(choice.Value);
                }
                else
                {
                    Commands.Add(choice.Value);

                    if (RunImmediate)
                    {
                        CommandSystem.ExecuteCommandString(choice.Value);
                    }
                }
            }

            return OptionResult.SuccessRetain;
        }

        protected void PerformAction(bool prompt)
        {
            try
            {
                string msg = null;

                foreach (string command in Commands)
                {
                    CommandSystem.ExecuteCommandString(command);

                    msg += command + Common.NewLine;
                }

                if ((msg != null) && (prompt))
                {
                    Overwatch.AlarmNotify(msg);
                }
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }
    }
}
