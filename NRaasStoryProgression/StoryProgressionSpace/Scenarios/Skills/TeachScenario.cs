using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public abstract class TeachScenario : RelationshipScenario, IHasSkill
    {
        string mSkillName;

        public TeachScenario()
            : base (25)
        { }
        protected TeachScenario(TeachScenario scenario)
            : base (scenario)
        { }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        protected abstract SkillNames Skill
        {
            get;
        }

        public SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { Skill };
            }
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Skills.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (sim.SkillManager == null)
            {
                IncStat("No Manager");
                return false;
            }

            return (base.CommonAllow(sim));
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<Skill> skills = new List<Skill>();

            SkillNames specificSkill = Skill;

            foreach (Skill skill in Sim.SkillManager.List)
            {
                if (skill.IsHiddenSkill()) continue;

                if (!Target.SkillManager.IsPlayerVisible(skill.Guid)) continue;

                if (specificSkill != SkillNames.None)
                {
                    if (specificSkill != skill.Guid) continue;
                }

                if ((skill.SkillLevel - 1) > Target.SkillManager.GetSkillLevel(skill.Guid))
                {
                    skills.Add(skill);
                }
            }

            if (skills.Count == 0) return false;

            Skill choice = RandomUtil.GetRandomObjectFromList(skills);

            Skill newSkill = Target.SkillManager.GetSkill<Skill>(choice.Guid);
            if (newSkill == null)
            {
                newSkill = Target.SkillManager.AddElement(choice.Guid);
            }

            newSkill.ForceGainPointsForLevelUp();

            mSkillName = choice.Name;

            return (mSkillName != null);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                parameters = new object[] { Sim, Target, mSkillName };
            }

            if (extended == null)
            {
                extended = new string[] { mSkillName };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }
    }
}
