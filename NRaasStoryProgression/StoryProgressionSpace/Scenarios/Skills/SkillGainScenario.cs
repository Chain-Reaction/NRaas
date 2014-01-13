using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public abstract class SkillGainScenario : SimScenario, IHasSkill
    {
        string mSkillName;

        public SkillGainScenario()
        { }
        protected SkillGainScenario(SimDescription sim)
            : base(sim)
        { }
        protected SkillGainScenario(SkillGainScenario scenario)
            : base (scenario)
        {
            mSkillName = scenario.mSkillName;
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        public SkillNames[] CheckSkills
        {   
            // Allowability is checked in GetPotentialSkills()
            get { return new SkillNames[0]; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.TeensAndAdults;
        }

        protected bool HasSkillMetric(Career job)
        {
            if (job == null) return false;

            if (job.CurLevel == null) return false;

            foreach (PerfMetric metric in job.CurLevel.Metrics)
            {
                if (metric is MetricSkillX) return true;
            }

            return false;
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ToddlerOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (!HasSkillMetric(sim.Occupation as Career))
            {
                IncStat("No Metric");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Push Denied");
                return false;
            }
            else if (!Skills.Allow(this, sim))
            {
                IncStat("Skills Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected virtual void GetPotentialSkills(List<SkillNames> skills)
        {
            SimData data = Options.GetSim(Sim);

            Career career = Sim.Occupation as Career;
            if (career != null)
            {
                foreach (PerfMetric metric in career.CurLevel.Metrics)
                {
                    MetricSkillX skillMetric = metric as MetricSkillX;
                    if (skillMetric == null) continue;

                    Skill skill = Sim.SkillManager.GetSkill<Skill>(skillMetric.SkillGuid);
                    if (skill != null)
                    {
                        if (skill.ReachedMaxLevel()) continue;
                    }

                    if (!Skills.AllowSkill(this, Sim, data, skillMetric.SkillGuid)) continue;

                    skills.Add(skillMetric.SkillGuid);
                }
            }

            if (skills.Count == 0)
            {
                if (RandomUtil.RandomChance(AddScoring("GeneralSkilling", Sim)))
                {
                    foreach (Skill staticSkill in SkillManager.SkillDictionary)
                    {
                        Skill skill = Sim.SkillManager.GetSkill<Skill>(staticSkill.Guid);
                        if (skill != null)
                        {
                            if (skill.ReachedMaxLevel()) continue;
                        }

                        if (!Skills.AllowSkill(this, Sim, data, staticSkill.Guid)) continue;

                        skills.Add(staticSkill.Guid);
                    }
                }
            }
        }

        protected abstract bool Perform(SkillNames skill);

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<SkillNames> skills = new List<SkillNames>();
            GetPotentialSkills(skills);

            while (skills.Count > 0)
            {
                SkillNames skillName = RandomUtil.GetRandomObjectFromList(skills);
                skills.Remove(skillName);

                if (Perform(skillName))
                {
                    Skill skill = Sim.SkillManager.GetElement(skillName);
                    if (skill != null)
                    {
                        mSkillName = skill.Name;
                    }
                    return true;
                }
            }

            IncStat("No Choices");
            return false;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            return base.PrintStory(manager, name, parameters, new string[] { mSkillName }, logging);
        }
    }
}
