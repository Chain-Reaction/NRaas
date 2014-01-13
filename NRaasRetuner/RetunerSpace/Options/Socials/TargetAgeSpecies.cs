using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RetunerSpace.Options.Socials
{
    public class TargetAgeSpecies : AgeSpeciesBase, ISocialOption
    {
        ActionData mData;

        public TargetAgeSpecies(ActionData data)
            : base(GetSettings(data))
        {
            mData = data;
        }

        public override string GetTitlePrefix()
        {
            return "TargetAgeSpecies";
        }

        protected static List<CASAGSAvailabilityFlags> GetSettings(ActionData data)
        {
            SeasonSettings.ActionDataSetting settings = Retuner.SeasonSettings.GetSettings(data, false);
            if (settings != null)
            {
                List<CASAGSAvailabilityFlags> result;
                if (settings.GetTargetAgeSpecies(out result)) return result;
            }
            return Retuner.AgeSpeciesToList(data.TargetAgeSpeciesAllowed);
        }

        protected override void PrivatePerform(IEnumerable<Item> results)
        {
            base.PrivatePerform(results);

            Retuner.SeasonSettings.GetSettings(mData, true).SetTargetAgeSpecies(Retuner.SeasonSettings.Key, mData, mAgeSpecies);
        }
    }
}
