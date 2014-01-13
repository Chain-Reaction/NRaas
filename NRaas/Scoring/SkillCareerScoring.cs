using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class SkillCareerScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        SkillNames mSkill;

        public SkillCareerScoring()
        { }
        public SkillCareerScoring(int hit, int miss, SkillNames skill)
            : base(hit, miss)
        {
            mSkill = skill;
        }

        public override string ToString()
        {
            return base.ToString() + "," + mSkill;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!base.Parse(row, ref error)) return false;

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

        public override bool IsHit(SimScoringParameters parameters)
        {
            SkillBasedCareer career = parameters.Actor.OccupationAsSkillBasedCareer;
            if (career != null) 
            {
                SkillBasedCareerStaticData skillData = career.GetOccupationStaticDataForSkillBasedCareer();
                if (skillData != null)
                {
                    return (skillData.CorrespondingSkillName == mSkill);
                }
            }

            return (mSkill == SkillNames.None);
        }
    }
}

