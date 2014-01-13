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
    public class SkillMetricScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        SkillNames mSkill;

        public SkillMetricScoring()
        { }

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

        public static bool HasMetric(Career job, SkillNames skill)
        {
            if (job == null) return false;

            if (job.CurLevel == null) return false;

            foreach (PerfMetric metric in job.CurLevel.Metrics)
            {
                MetricSkillX skillMetric = metric as MetricSkillX;
                if (skillMetric == null) continue;

                if (skillMetric.SkillGuid == skill)
                {
                    return true;
                }
            }

            return false;
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }

        public override bool IsHit(SimScoringParameters parameters)
        {
            if (parameters.Actor.CareerManager != null)
            {
                if (HasMetric(parameters.Actor.Occupation as Career, mSkill)) return true;

                if (HasMetric(parameters.Actor.CareerManager.School, mSkill)) return true;
            }

            return false;
        }
    }
}

