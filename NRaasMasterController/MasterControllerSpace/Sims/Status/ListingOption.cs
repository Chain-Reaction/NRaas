using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Sims.Export;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Status
{
    public class ListingOption : OptionList<IStatusOption>, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "StatusSimInteraction";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<IStatusOption> GetOptions()
        {
            List<IStatusOption> results = new List<IStatusOption>();

            foreach (IStatusOption result in base.GetOptions())
            {
                if (result is IExportOption) continue;

                results.Add(result);
            }

            return results;
        }
    }
}
