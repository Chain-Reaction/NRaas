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

namespace NRaas.VectorSpace.Options.Enabled
{
    public class ListingOption : InteractionOptionList<IEnabledOption, GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "Enabled";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<IEnabledOption> GetOptions()
        {
            List<IEnabledOption> results = new List<IEnabledOption>();

            foreach (VectorBooter.Data vector in VectorBooter.Vectors)
            {
                if (!vector.CanOutbreak) continue;

                results.Add(new EnabledSetting(vector.Guid));
            }

            return results;
        }
    }
}
