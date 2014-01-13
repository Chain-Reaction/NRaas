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
    public class ListingOption : InteractionOptionList<IByObjectOption, GameObject>, ITuningOption
    {
        public override string GetTitlePrefix()
        {
            return "ByObjectRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public static List<IByObjectOption> GetOptions(IEnumerable<InteractionTuning> allTunings)
        {
            List<IByObjectOption> results = new List<IByObjectOption>();

            Dictionary<string, List<InteractionTuning>> objects = new Dictionary<string, List<InteractionTuning>>();

            foreach (InteractionTuning tuning in allTunings)
            {
                string name = tuning.FullObjectName;

                List<InteractionTuning> tunings;
                if (!objects.TryGetValue(name, out tunings))
                {
                    tunings = new List<InteractionTuning>();
                    objects.Add(name, tunings);
                }

                tunings.Add(tuning);
            }

            foreach (KeyValuePair<string, List<InteractionTuning>> pair in objects)
            {
                results.Add(new ByObjectOption(pair.Key, pair.Value));
            }

            return results;
        }

        public override List<IByObjectOption> GetOptions()
        {
            return GetOptions(InteractionTuning.sAllTunings.Values);
        }
    }
}
