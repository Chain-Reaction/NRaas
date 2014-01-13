using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.OverwatchSpace.Helpers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Settings
{
    public class ExportSettings : OptionItem, IActionOption
    {
        public ExportSettings()
        { }

        public override string GetTitlePrefix()
        {
            return "ExportSettings";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            IEnumerable<FilePersistenceEx.Item> selection = FilePersistenceEx.GetChoices(Name);
            if (selection == null) return OptionResult.Failure;
             
            Common.StringBuilder builder = new Common.StringBuilder(FilePersistence.sHeader + Common.NewLine + "<Settings>");

            foreach (FilePersistenceEx.Item choice in selection)
            {
                string text = choice.CreateExportString();

                if (!string.IsNullOrEmpty(text))
                {
                    builder.Append(text);
                }
            }

            builder.Append(Common.NewLine + "</Settings>");

            Common.DebugWriteLog(builder);

            FilePersistence.ExportToFile(builder.ToString());

            return OptionResult.SuccessRetain;
        }
    }
}
