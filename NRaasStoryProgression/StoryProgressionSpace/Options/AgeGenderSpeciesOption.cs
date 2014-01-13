using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay;
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
    public abstract class AgeGenderSpeciesOption : GenericOptionBase.ListedOptionItem<CASAgeGenderFlags, CASAgeGenderFlags>
    {
        static List<CASAgeGenderFlags> sAgeGenderSpecies;

        static AgeGenderSpeciesOption()
        {
            sAgeGenderSpecies = new List<CASAgeGenderFlags>();

            foreach(CASAgeGenderFlags flags in Enum.GetValues(typeof(CASAgeGenderFlags)))
            {
                switch (flags)
                {
                    case CASAgeGenderFlags.None:
                    case CASAgeGenderFlags.AgeMask:
                    case CASAgeGenderFlags.GenderMask:
                    case CASAgeGenderFlags.HandednessMask:
                    case CASAgeGenderFlags.LargeBird:
                    case CASAgeGenderFlags.LeftHanded:
                    case CASAgeGenderFlags.Raccoon:
                    case CASAgeGenderFlags.RightHanded:
                    case CASAgeGenderFlags.SimLeadingHorse:
                    case CASAgeGenderFlags.SimWalkingDog:
                    case CASAgeGenderFlags.SimWalkingLittleDog:
                    case CASAgeGenderFlags.SpeciesMask:
                        break;
                    default:
                        sAgeGenderSpecies.Add(flags);
                        break;
                }
            }
        }

        public AgeGenderSpeciesOption(CASAgeGenderFlags[] defaults)
            : base(new List<CASAgeGenderFlags>(), new List<CASAgeGenderFlags>(defaults))
        { }

        protected override CASAgeGenderFlags ConvertFromString(string value)
        {
            CASAgeGenderFlags result;
            ParserFunctions.TryParseEnum<CASAgeGenderFlags>(value, out result, CASAgeGenderFlags.None);
            return result;
        }

        protected override CASAgeGenderFlags ConvertToValue(CASAgeGenderFlags value, out bool valid)
        {
            valid = true;
            return value;
        }

        protected override string ValuePrefix
        {
            get { return "Boolean"; }
        }

        protected override IEnumerable<CASAgeGenderFlags> GetOptions()
        {
            return sAgeGenderSpecies;
        }
    }
}

