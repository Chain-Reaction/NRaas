using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class OccupationScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        OccupationNames mOccupation;

        string mBranch;

        public OccupationScoring()
        { }

        public override string ToString()
        {
            return base.ToString() + "," + mOccupation;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (row.Exists("CustomOccupation"))
            {
                mOccupation = unchecked((OccupationNames)ResourceUtils.HashString64(row.GetString("CustomOccupation")));
            }
            else
            {
                if (!row.Exists("Occupation"))
                {
                    error = "Occupation missing";
                    return false;
                }
                else if (!ParserFunctions.TryParseEnum<OccupationNames>(row.GetString("Occupation"), out mOccupation, OccupationNames.Undefined))
                {
                    error = "Unknown Occupation " + row.GetString("Occupation");
                    return false;
                }
            }

            mBranch = row.GetString("Branch");

            return base.Parse(row, ref error);
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }

        public override bool IsHit(SimScoringParameters parameters)
        {
            if (parameters.Actor.Occupation != null) 
            {
                Career career = parameters.Actor.Occupation as Career;
                if ((career != null) && (career.CurLevel != null) && (career.CurLevel.DayLength > 0))
                {
                    if (mOccupation == OccupationNames.Any) return true;
                }

                if (parameters.Actor.Occupation.Guid != mOccupation) return false;

                if (string.IsNullOrEmpty(mBranch)) return true;

                return (career.CurLevel.BranchName == mBranch);
            }
            else
            {
                return (mOccupation == OccupationNames.Undefined);
            }
        }
    }
}

