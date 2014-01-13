using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class SkillLevelScoring : Scoring<SimDescription, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        SkillNames mSkill;

        int mScore;

        public SkillLevelScoring()
        { }

        public override string ToString()
        {
            return base.ToString() + "," + mSkill + ", " + mScore;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("Score"))
            {
                error = "Score missing";
                return false;
            }

            mScore = row.GetInt("Score", 0);

            if (row.Exists("CustomSkill"))
            {
                mSkill = SkillManager.sSkillEnumValues.ParseEnumValue(row.GetString("CustomSkill"));
                if (mSkill == SkillNames.None)
                {
                    //error = "CustomSkill unknown: " + row.GetString("CustomSkill");
                    return false;
                }
            }
            else
            {
                mSkill = SkillManager.sSkillEnumValues.ParseEnumValue(row.GetString("Skill"));
                if (mSkill == SkillNames.None)
                {
                    error = "Skill unknown: " + row.GetString("Skill");
                    return false;
                }
            }

            return true;
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }
        public override int Score(SimScoringParameters parameters)
        {
            if (parameters.Actor.SkillManager == null) return 0;

            return parameters.Actor.SkillManager.GetSkillLevel(mSkill) * mScore;
        }
    }
}

