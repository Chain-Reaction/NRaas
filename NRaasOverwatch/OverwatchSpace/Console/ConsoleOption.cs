using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Console
{
    public abstract class ConsoleOption : OptionItem, IConsoleOption
    {
        public ConsoleOption()
        { }

        protected List<ConsoleItem> SelectCommands(string title, List<string> choiceList)
        {
            List<ConsoleItem> choices = GetCommands(choiceList);
            if ((choices == null) || (choices.Count == 0)) return null;

            return new List<ConsoleItem>(new CommonSelection<ConsoleItem>(title, choices).SelectMultiple());
        }

        protected List<ConsoleItem> GetCommands(List<string> choiceList)
        {
            List<ConsoleItem> list = new List<ConsoleItem>();

            foreach (string command in Overwatch.Settings.mAutoCommands)
            {
                if (Overwatch.Settings.mStoredCommands.Contains(command)) continue;

                Overwatch.Settings.mStoredCommands.Add(command);
            }

            foreach (string command in Overwatch.Settings.mAlarmCommands)
            {
                if (Overwatch.Settings.mStoredCommands.Contains(command)) continue;

                Overwatch.Settings.mStoredCommands.Add(command);
            }

            foreach (string command in Overwatch.Settings.mStoredCommands)
            {
                list.Add(new ConsoleItem(command, choiceList.Contains(command)));
            }

            return list;
        }

        public class ConsoleItem : ValueSettingOption<string>
        {
            public ConsoleItem(string command, bool value)
                : base(command, command, value ? 1 : 0)
            { }

            public override string DisplayKey
            {
                get { return "Boolean"; }
            }
        }
    }
}
