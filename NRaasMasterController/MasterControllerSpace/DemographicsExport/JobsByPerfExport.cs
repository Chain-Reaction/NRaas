using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Demographics;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.DemographicsExport
{
    public class JobsByPerfExport : JobsByPerf, IDemographicsExportOption
    {
        protected override OptionResult RunAll(List<Sims3.UI.CAS.IMiniSimDescription> sims)
        {
            Common.WriteLog(GetDetails(sims), false);

            Common.Notify(Common.Localize("Demographics:Exported"));
            return OptionResult.SuccessRetain;
        }
    }
}
