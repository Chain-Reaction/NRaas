using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.RetunerSpace.Options.Tunable;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RetunerSpace.Options
{
    public class ExportAllTuning : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "ExportAllTuning";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Common.kDebugging) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Common.StringBuilder result = new Common.StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");

            List<TunableNamespaceOption> options = new Tunable.ListingOption().GetOptions();
            options.Sort(CommonOptionItem.SortByName);

            result += Common.NewLine + "<Tuning>";

            foreach (TunableNamespaceOption option in options)
            {
                option.Export(result);
            }

            result += Common.NewLine + "</Tuning>";

            Common.WriteLog(result.ToString(), false);

            SimpleMessageDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Success"));

            return OptionResult.SuccessRetain;
        }
    }
}
