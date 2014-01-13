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

namespace NRaas.RetunerSpace.Options.ByAgeSpecies
{
    public class ListingOption : InteractionOptionList<IByAgeSpeciesOption, GameObject>, ITuningOption
    {
        public override string GetTitlePrefix()
        {
            return "ByAgeSpeciesRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<IByAgeSpeciesOption> GetOptions()
        {
            List<IByAgeSpeciesOption> results = new List<IByAgeSpeciesOption>();

            Dictionary<CASAGSAvailabilityFlags, List<InteractionTuning>> objects = new Dictionary<CASAGSAvailabilityFlags, List<InteractionTuning>>();

            foreach (InteractionTuning tuning in InteractionTuning.sAllTunings.Values)
            {
                foreach (CASAGSAvailabilityFlags ageSpecies in Retuner.AgeSpeciesToList(tuning.Availability.AgeSpeciesAvailabilityFlags))
                {
                    List<InteractionTuning> tunings;
                    if (!objects.TryGetValue(ageSpecies, out tunings))
                    {
                        tunings = new List<InteractionTuning>();
                        objects.Add(ageSpecies, tunings);
                    }

                    tunings.Add(tuning);
                }
            }

            foreach (KeyValuePair<CASAGSAvailabilityFlags, List<InteractionTuning>> pair in objects)
            {
                results.Add(new ByAgeSpeciesOption(pair.Key, pair.Value));
            }

            return results;
        }
    }
}
