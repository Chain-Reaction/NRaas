using NRaas.CommonSpace.Options;
using NRaas.VectorSpace.Booters;
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

namespace NRaas.VectorSpace.Options.Outbreaks.Automated
{
    public class ListingOption : InteractionOptionList<IAutomatedOption, GameObject>, IOutbreaksOption
    {
        public override string GetTitlePrefix()
        {
            return "AutomatedOutbreaksRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<IAutomatedOption> GetOptions()
        {
            List<IAutomatedOption> results = new List<IAutomatedOption>();

            foreach (VectorBooter.Data vector in VectorBooter.Vectors)
            {
                if (!vector.CanOutbreak) continue;

                results.Add(new AutomatedSetting(vector.Guid));
            }

            return results;
        }
    }
}
