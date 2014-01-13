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

namespace NRaas.RetunerSpace.Options
{
    public abstract class AgeSpeciesBase : ListedSettingOption<CASAGSAvailabilityFlags, GameObject>
    {
        protected List<CASAGSAvailabilityFlags> mAgeSpecies = null;

        public AgeSpeciesBase(List<CASAGSAvailabilityFlags> ageSpecies)
        {
            mAgeSpecies = ageSpecies;
        }

        protected override string GetValuePrefix()
        {
            return "AgeSpecies";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override Proxy GetList()
        {
            return new ListProxy(mAgeSpecies);
        }

        protected override bool Allow(CASAGSAvailabilityFlags value)
        {
            switch (value)
            {
                case CASAGSAvailabilityFlags.AllAnimalsMask:
                case CASAGSAvailabilityFlags.AllBoatsMask:
                case CASAGSAvailabilityFlags.AllCatsMask:
                case CASAGSAvailabilityFlags.AllDogsMask:
                case CASAGSAvailabilityFlags.AllDomesticAnimalsMask:
                case CASAGSAvailabilityFlags.AllHorsesMask:
                case CASAGSAvailabilityFlags.AllHouseboatsMask:
                case CASAGSAvailabilityFlags.AllLittleDogsMask:
                case CASAGSAvailabilityFlags.AllWildAnimalsMask:
                case CASAGSAvailabilityFlags.GenderMask:
                case CASAGSAvailabilityFlags.HandednessMask:
                case CASAGSAvailabilityFlags.Houseboat6x20:
                case CASAGSAvailabilityFlags.Houseboat6x20Curved:
                case CASAGSAvailabilityFlags.Houseboat8x18Curved:
                case CASAGSAvailabilityFlags.HouseboatLarge:
                case CASAGSAvailabilityFlags.HouseboatMedium:
                case CASAGSAvailabilityFlags.HouseboatSmall:
                case CASAGSAvailabilityFlags.HumanAgeMask:
                case CASAGSAvailabilityFlags.LeftHanded:
                case CASAGSAvailabilityFlags.Paddleboat:
                case CASAGSAvailabilityFlags.RightHanded:
                case CASAGSAvailabilityFlags.Rowboat:
                case CASAGSAvailabilityFlags.Sailboat:
                case CASAGSAvailabilityFlags.Speedboat:
                case CASAGSAvailabilityFlags.WaterScooter:
                case CASAGSAvailabilityFlags.WindsurfBoardChild:
                case CASAGSAvailabilityFlags.WindsurfBoatAdult:
                    return false;
            }

            return Retuner.Allow(value);
        }
        
        public override bool ConvertFromString(string value, out CASAGSAvailabilityFlags newValue)
        {
            return ParserFunctions.TryParseEnum<CASAGSAvailabilityFlags>(value, out newValue, CASAGSAvailabilityFlags.None);
        }

        public override string ConvertToString(CASAGSAvailabilityFlags value)
        {
            return value.ToString();
        }
    }
}
