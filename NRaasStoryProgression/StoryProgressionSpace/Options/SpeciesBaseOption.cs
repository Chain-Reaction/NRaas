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
    public abstract class SpeciesBaseOption : AgeGenderSpeciesOption
    {
        protected static CASAgeGenderFlags[] sSpecies = new CASAgeGenderFlags[] { CASAgeGenderFlags.Human, CASAgeGenderFlags.Cat, CASAgeGenderFlags.Dog, CASAgeGenderFlags.LittleDog, CASAgeGenderFlags.Horse };

        public SpeciesBaseOption(CASAgeGenderFlags[] defValue)
            : base(defValue)
        { }

        protected override bool Allow(CASAgeGenderFlags option)
        {
            switch (option)
            {
                case CASAgeGenderFlags.Human:
                case CASAgeGenderFlags.Horse:
                case CASAgeGenderFlags.Cat:
                case CASAgeGenderFlags.Dog:
                case CASAgeGenderFlags.LittleDog:
                    return true;
            }

            return false;
        }

        protected override string GetLocalizationUIKey()
        {
            return "SpeciesAbbreviation";
        }

        protected override string GetLocalizationValueKey()
        {
            return "Species";
        }

        protected override string GetLocalizedValue(CASAgeGenderFlags value, ref ThumbnailKey icon)
        {
            return LocalizeValue(value.ToString());
        }
    }
}

