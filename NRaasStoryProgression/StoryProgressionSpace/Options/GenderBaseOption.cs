using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public abstract class GenderBaseOption : AgeGenderSpeciesOption
    {
        public GenderBaseOption(CASAgeGenderFlags[] defValue)
            : base(defValue)
        { }

        protected override bool Allow(CASAgeGenderFlags option)
        {
            switch (option)
            {
                case CASAgeGenderFlags.Female:
                case CASAgeGenderFlags.Male:
                    return true;
            }

            return false;
        }

        protected override string GetLocalizationUIKey()
        {
            return "GenderAbbreviation";
        }

        protected override string GetLocalizationValueKey()
        {
            return "FirstBornGender";
        }

        protected override string GetLocalizedValue(CASAgeGenderFlags value, ref ThumbnailKey icon)
        {
            return LocalizeValue(value.ToString());
        }
    }
}

