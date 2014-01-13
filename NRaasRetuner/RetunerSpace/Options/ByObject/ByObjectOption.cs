using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RetunerSpace.Options.ByObject
{
    public class ByObjectOption : InteractionOptionList<ITUN.ListingOption, GameObject>, IByObjectOption
    {
        List<InteractionTuning> mTuning;

        public ByObjectOption(string name, List<InteractionTuning> tuning)
            : base(name.Substring(name.LastIndexOf('.') + 1))
        {
            mTuning = tuning;
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override List<ITUN.ListingOption> GetOptions()
        {
            Dictionary<string, ITUN.ListingOption> nameLookup = new Dictionary<string, ITUN.ListingOption>();

            List<ITUN.ListingOption> results = new List<ITUN.ListingOption>();

            foreach (InteractionTuning tuning in mTuning)
            {
                ITUN.ListingOption option = new ITUN.ListingOption(tuning.ShortInteractionName, tuning);

                ITUN.ListingOption original;
                if (nameLookup.TryGetValue(option.Name, out original))
                {
                    option.AppendKey();

                    original.AppendKey();
                }
                else
                {
                    nameLookup.Add(option.Name, option);
                }

                results.Add(option);
            }

            return results;
        }
    }
}
