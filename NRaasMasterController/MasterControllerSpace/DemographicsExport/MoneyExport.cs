using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Demographics;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.DemographicsExport
{
    public class MoneyExport : Money, IDemographicsExportOption
    {
        protected override OptionResult RunAll(List<Sims3.UI.CAS.IMiniSimDescription> sims)
        {
            Common.WriteLog(GetDetails(sims), false);

            Common.Notify(Common.Localize("Demographics:Exported"));
            return OptionResult.SuccessRetain;
        }
    }
}
