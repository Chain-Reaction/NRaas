using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class PersistenceEx : Persistence
    {
        public override void Import(Lookup settings)
        {
            Dictionary<string, OptionItem> optionLookup = new Dictionary<string, OptionItem>();
            StoryProgression.Main.GetOptionLookup(optionLookup);

            foreach(KeyValuePair<string,OptionItem> option in optionLookup)
            {
                string value = settings.GetString(option.Key);
                if (value == null) continue;

                try
                {
                    option.Value.PersistValue = value;
                }
                catch (Exception e)
                {
                    Common.Exception(option.Key, e);
                }
            }

            StoryProgression.Main.Options.ImportCastes(settings);

            foreach (OptionItem option in optionLookup.Values)
            {
                IAfterImportOptionItem afterOption = option as IAfterImportOptionItem;
                if (afterOption == null) continue;

                afterOption.PerformAfterImport();
            }
        }

        public override void Export(Lookup settings)
        {
            List<OptionItem> allOptions = new List<OptionItem>();
            StoryProgression.Main.GetOptions(allOptions, false, IsExportable);

            StoryProgression.Main.Options.ExportCastes(settings);

            foreach (OptionItem option in StoryProgression.Main.Options.GetOptions(StoryProgression.Main, "Town", false))
            {
                allOptions.Add(option);
            }

            foreach (OptionItem option in StoryProgression.Main.Options.GetImmigrantOptions(StoryProgression.Main))
            {
                allOptions.Add(option);
            }

            foreach (OptionItem option in allOptions)
            {
                try
                {
                    List<string> names = GetExportKey(option);
                    if ((names == null) || (names.Count == 0)) continue;

                    settings.Add(names[0], option.GetExportValue());
                }
                catch (Exception e)
                {
                    Common.Exception(option.Name, e);
                }
            }
        }

        public override string PersistencePrefix
        {
            get { return ""; }
        }

        public static List<string> GetExportKey(OptionItem option)
        {
            List<string> values = new List<string>();

            string text = option.GetStoreKey();
            if (!string.IsNullOrEmpty(text))
            {
                values.Add(text);

                values.Add("Options" + text);
            }

            text = option.GetLocalizationKey();
            if (!string.IsNullOrEmpty(text))
            {
                values.Add(text);
            }

            return values;
        }

        public static bool IsExportable(OptionItem option)
        {
            if (option is INotExportableOption) return false;

            return true;
        }
    }
}

