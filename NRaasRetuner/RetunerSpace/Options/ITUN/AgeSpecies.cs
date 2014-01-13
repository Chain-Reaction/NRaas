using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RetunerSpace.Options.ITUN
{
    public class AgeSpecies : AgeSpeciesBase, IITUNOption
    {
        InteractionTuning mTuning;

        public AgeSpecies(InteractionTuning tuning)
            : base(GetSettings(tuning))
        {
            mTuning = tuning;
        }

        public override string GetTitlePrefix()
        {
            return "AgeSpecies";
        }

        protected static List<CASAGSAvailabilityFlags> GetSettings(InteractionTuning tuning)
        {
            SeasonSettings.ITUNSettings settings = Retuner.SeasonSettings.GetSettings(tuning, false);
            if (settings != null)
            {
                List<CASAGSAvailabilityFlags> result;
                if (settings.GetAgeSpecies(out result)) return result;
            }

            return Retuner.AgeSpeciesToList(tuning.Availability.AgeSpeciesAvailabilityFlags);
        }

        protected override void PrivatePerform(IEnumerable<Item> results)
        {
            base.PrivatePerform(results);

            Retuner.SeasonSettings.GetSettings(mTuning, true).SetAgeSpecies(Retuner.SeasonSettings.Key, mTuning, mAgeSpecies);
        }
    }
}
