using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Options
{
    public class ExportTuning : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "ExportTuning";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Common.WriteLog(Tempest.Settings.ToXMLString(), false);

            SimpleMessageDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Success"));

            return OptionResult.SuccessRetain;
        }
    }
}
