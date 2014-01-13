using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.DemographicsExport;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Demographics
{
    public class ListingOption : OptionList<DemographicOption>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
 	         return "DemographicInteraction";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<DemographicOption> GetOptions()
        {
            List<DemographicOption> results = new List<DemographicOption>();

            foreach (DemographicOption result in base.GetOptions())
            {
                if (result is IDemographicsExportOption) continue;

                results.Add(result);
            }

            return results;
        }
    }
}
