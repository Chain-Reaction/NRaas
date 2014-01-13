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
    public class AgeGenderScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        CASAgeGenderFlags mAgeGender = CASAgeGenderFlags.None;

        List<CASAgeGenderFlags> mSpecies = new List<CASAgeGenderFlags>();

        public AgeGenderScoring()
        { }

        public override string ToString()
        {
            string result = base.ToString() + "," + mAgeGender;

            result += "," + new ListToString<CASAgeGenderFlags>().Convert(mSpecies);

            return result;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("AgeGender"))
            {
                error = "AgeGender Missing";
            }
            else
            {
                if (!ParserFunctions.TryParseEnum<CASAgeGenderFlags>(row.GetString("AgeGender"), out mAgeGender, CASAgeGenderFlags.None))
                {
                    error = "Unknown AgeGender " + row.GetString("AgeGender");
                    return false;
                }
            }

            StringToSpeciesList converter = new StringToSpeciesList();

            mSpecies = converter.Convert(row.GetString("Species"));
            if (mSpecies == null)
            {
                error = converter.mError;
                return false;
            }

            return base.Parse(row, ref error);
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }

        public override bool IsHit(SimScoringParameters parameters)
        {
            if ((mAgeGender & CASAgeGenderFlags.AgeMask) != CASAgeGenderFlags.None)
            {
                if ((parameters.Actor.Age & mAgeGender) != parameters.Actor.Age)
                {
                    return false;
                }
            }

            if ((mAgeGender & CASAgeGenderFlags.GenderMask) != CASAgeGenderFlags.None)
            {
                if ((parameters.Actor.Gender & mAgeGender) != parameters.Actor.Gender)
                {
                    return false;
                }
            }

            if (mSpecies.Count > 0)
            {
                if (!mSpecies.Contains(parameters.Actor.Species))
                {
                    return false;
                }
            }

            return true;
        }
    }
}

