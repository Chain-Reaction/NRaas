using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class AgeGenderMatchScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>
    {
        CASAgeGenderFlags mAgeGender;
        List<CASAgeGenderFlags> mSpecies = new List<CASAgeGenderFlags>();

        CASAgeGenderFlags mOtherAgeGender;
        List<CASAgeGenderFlags> mOtherSpecies = new List<CASAgeGenderFlags>();

        public AgeGenderMatchScoring()
        {}

        public override string ToString()
        {
            string result = base.ToString() + ":" + mAgeGender;

            result += "," + new ListToString<CASAgeGenderFlags>().Convert(mSpecies);
            
            result += ":" + mOtherAgeGender;

            result += "," + new ListToString<CASAgeGenderFlags>().Convert(mOtherSpecies);

            return result;
        }

        public CASAgeGenderFlags OtherAgeGender
        {
            get { return mOtherAgeGender; }
        }

        public ICollection<CASAgeGenderFlags> OtherSpecies
        {
            get { return mOtherSpecies; }
        }

        public CASAgeGenderFlags AgeGender
        {
            get { return mAgeGender; }
        }

        public ICollection<CASAgeGenderFlags> Species
        {
            get { return mSpecies; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("AgeGender"))
            {
                error = "AgeGender missing";
                return false;
            }
            else if (!row.Exists("OtherAgeGender"))
            {
                error = "OtherAgeGender missing";
                return false;
            }
            else if (!ParserFunctions.TryParseEnum<CASAgeGenderFlags>(row.GetString("AgeGender"), out mAgeGender, CASAgeGenderFlags.None))
            {
                error = "Unknown AgeGender " + row.GetString("AgeGender");
                return false;
            }
            else if (!ParserFunctions.TryParseEnum<CASAgeGenderFlags>(row.GetString("OtherAgeGender"), out mOtherAgeGender, CASAgeGenderFlags.None))
            {
                error = "Unknown OtherAgeGender " + row.GetString("OtherAgeGender");
                return false;
            }

            StringToSpeciesList converter = new StringToSpeciesList();
            mSpecies = converter.Convert(row.GetString("Species"));
            if (mSpecies == null)
            {
                error = converter.mError;
                return false;
            }

            if (mSpecies.Count == 0)
            {
                mSpecies.Add(CASAgeGenderFlags.Human);
            }

            mOtherSpecies = converter.Convert(row.GetString("OtherSpecies"));
            if (mOtherSpecies == null)
            {
                error = converter.mError;
                return false;
            }

            if (mOtherSpecies.Count == 0)
            {
                mOtherSpecies.Add(CASAgeGenderFlags.Human);
            }

            if ((mAgeGender & CASAgeGenderFlags.AgeMask) == CASAgeGenderFlags.None)
            {
                mAgeGender |= CASAgeGenderFlags.AgeMask;
            }

            if ((mAgeGender & CASAgeGenderFlags.GenderMask) == CASAgeGenderFlags.None)
            {
                mAgeGender |= CASAgeGenderFlags.GenderMask;
            }

            if ((mOtherAgeGender & CASAgeGenderFlags.AgeMask) == CASAgeGenderFlags.None)
            {
                mOtherAgeGender |= CASAgeGenderFlags.AgeMask;
            }

            if ((mOtherAgeGender & CASAgeGenderFlags.GenderMask) == CASAgeGenderFlags.None)
            {
                mOtherAgeGender |= CASAgeGenderFlags.GenderMask;
            }

            return base.Parse(row, ref error);
        }

        public override int Score(DualSimScoringParameters parameters)
        {
            SimDescription other = parameters.Other;

            CASAgeGenderFlags ageGender = other.AgeGenderSpecies;

            if ((mOtherAgeGender & ageGender) != ageGender)
            {
                return 0;
            }
            else if (!mOtherSpecies.Contains(other.Species))
            {
                return 0;
            }

            return base.Score(parameters);
        }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            return IsHit(parameters.Actor.AgeGenderSpecies);
        }

        public bool IsHit(CASAgeGenderFlags ageGender)
        {
            if ((mAgeGender & ageGender) != ageGender) return false;

            return mSpecies.Contains(ageGender & CASAgeGenderFlags.SpeciesMask);
        }

        public class Delegate
        {
            CASAgeGenderFlags mAgeGender;

            public Delegate(SimDescription sim)
            {
                mAgeGender = sim.AgeGenderSpecies;
            }

            public int Score(AgeGenderMatchScoring scoring, DualSimScoringParameters parameters)
            {
                if (!scoring.IsHit(mAgeGender)) return 0;

                return scoring.Score(parameters);
            }
        }
    }
}

