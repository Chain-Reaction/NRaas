using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Situations;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.SimDataElement
{
    public class SkillLevelSimData : OnDemandSimData
    {
        protected Dictionary<SkillNames, Pair<int,float>> mSkillLevels = null;

        public SkillLevelSimData()
        { }

        public bool Adjust(Common.IStatGenerator manager, SkillNames skillName)
        {
            if (mSkillLevels == null) return false;

            Skill skill = Sim.SkillManager.GetElement(skillName);
            if (skill != null)
            {
                int origLevel = 0;
                float origPoints = 0;

                if (mSkillLevels.ContainsKey(skill.Guid))
                {
                    origLevel = mSkillLevels[skill.Guid].First;
                    origPoints = mSkillLevels[skill.Guid].Second;
                }

                int change = skill.SkillLevel - origLevel;
                if (change > 0)
                {
                    manager.AddStat("Lowered: " + skill.Guid, change);

                    skill.SkillLevel = origLevel;
                    skill.SkillPoints = origPoints;

                    skill.UpdateProgress();
                }
            }

            return true;
        }

        public override bool Delayed
        {
            get { return false; }
        }

        public override string ToString()
        {
            Common.StringBuilder text = new Common.StringBuilder(base.ToString());

            if (mSkillLevels == null)
            {
                text += Common.NewLine + "<NoSkills/>";
            }
            else
            {
                foreach (KeyValuePair<SkillNames, Pair<int,float>> skill in mSkillLevels)
                {
                    text.AddXML(skill.Key.ToString(), skill.Value.First + ":" + skill.Value.Second);
                }
            }
            return text.ToString();
        }

        public void UpdateSkill(SkillNames guid)
        {
            if (Sim == null) return;

            if (Sim.SkillManager == null) return;

            Reset();

            int level = 0;
            float points = 0;

            Skill skill = Sim.SkillManager.GetSkill<Skill>(guid);
            if (skill != null)
            {
                level = skill.SkillLevel;
                points = skill.SkillPoints;
            }

            mSkillLevels[guid] = new Pair<int,float>(level,points);
        }

        public override void Reset()
        {
            base.Reset();

            if (Sim == null) return;

            if (Sim.SkillManager == null) return;

            if (mSkillLevels != null) return;

            mSkillLevels = new Dictionary<SkillNames, Pair<int,float>>();

            foreach (Skill skill in Sim.SkillManager.List)
            {
                mSkillLevels.Add(skill.Guid, new Pair<int,float>(skill.SkillLevel, skill.SkillPoints));
            }
        }
    }
}

