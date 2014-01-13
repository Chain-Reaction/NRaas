using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class OtherSkillLevelScoring : Scoring<SimDescription, DualSimScoringParameters>
    {
        SkillNames mSkill;

        int mScore;

        public OtherSkillLevelScoring()
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
                    error = "Skill Unknown: " + row.GetString("Skill");
                    return false;
                }
            }

            return true;
        }

        public override int Score(DualSimScoringParameters parameters)
        {
            if (parameters.Other.SkillManager == null) return 0;

            return parameters.Other.SkillManager.GetSkillLevel(mSkill) * mScore;
        }
    }
}

