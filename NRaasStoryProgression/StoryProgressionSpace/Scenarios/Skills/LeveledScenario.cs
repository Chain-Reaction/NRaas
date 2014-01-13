using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class LeveledScenario : SimEventScenario<HasGuidEvent<SkillNames>>, IHasSkill, IFormattedStoryScenario
    {
        public static event UpdateDelegate OnPerform;

        int mLevel = 0;

        public LeveledScenario()
        { }
        protected LeveledScenario(LeveledScenario scenario)
            : base (scenario)
        {
            mLevel = scenario.mLevel;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "SkillIncrease";
        }

        public Manager GetFormattedStoryManager()
        {
            return StoryProgression.Main.Skills;
        }

        public int SkillLevel
        {
            get { return mLevel; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override ListenHandleType HandleImmediately
        {
            get { return ListenHandleType.Immediate; } // Must be stack traceable
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        public SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { Event.Guid };
            }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSkillLevelUp);
        }

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            HasGuidEvent<SkillNames> skillEvent = e as HasGuidEvent<SkillNames>;
            if (skillEvent == null) return null;

            if (skillEvent.Actor == null) return null;

            if (skillEvent.Actor.SkillManager == null) return null;

            mLevel = skillEvent.Actor.SkillManager.GetSkillLevel(skillEvent.Guid);

            return base.Handle(e, ref result);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            if ((Event == null) || (Event.Guid == SkillNames.None))
            {
                IncStat("No Skill");
                return false;
            }

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.Household == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (SimTypes.IsSpecial(sim))
            {
                IncStat("Special");
                return false;
            }
            else if (sim.SkillManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if ((sim.AssignedRole != null) && (sim.LotHome == null))
            {
                IncStat("Homeless Role");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!Main.SecondCycle)
            {
                IncStat("First Cycle");
                return false;
            }

            if (OnPerform != null)
            {
                OnPerform(this, frame);
            }
            else
            {
                if (Sim.SkillManager != null)
                {
                    Skill skill = Sim.SkillManager.GetElement(Event.Guid);
                    if (skill != null)
                    {
                        if (skill.IsHiddenSkill())
                        {
                            IncStat("Hidden Skill");
                        }
                        else
                        {
                            Add(frame, new StoryScenario(Sim, Event.Guid, mLevel), ScenarioResult.Success);
                        }
                    }
                }
            }
            return false;
        }

        public override Scenario Clone()
        {
            return new LeveledScenario (this);
        }

        public class Option : BooleanEventOptionItem<ManagerSkill, LeveledScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SkillLeveled";
            }
        }

        public class StoryScenario : SimScenario
        {
            SkillNames mSkill;

            int mLevel;

            public StoryScenario(SimDescription sim, SkillNames skill, int level)
                : base(sim)
            {
                mSkill = skill;
                mLevel = level;
            }
            protected StoryScenario(StoryScenario scenario)
                : base(scenario)
            {
                mSkill = scenario.mSkill;
                mLevel = scenario.mLevel;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                switch (type)
                {
                    case PrefixType.Story:
                        return null;
                    case PrefixType.Summary:
                        return "SkillIncrease";
                    default:
                        return "SkillIncreaseStory";
                }
            }

            protected override bool Progressed
            {
                get { return false; }
            }

            protected override bool CheckBusy
            {
                get { return false; }
            }

            protected override bool Matches(Scenario scenario)
            {
                StoryScenario storyScenario = scenario as StoryScenario;
                if (storyScenario == null) return false;

                if (mSkill != storyScenario.mSkill) return false;
                if (mLevel != storyScenario.mLevel) return false;

                return base.Matches(scenario);
            }

            protected override ICollection<SimDescription> GetSims()
            {
                return null;
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                return true;
            }

            protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
            {
                if (manager == null)
                {
                    manager = Skills;
                }

                Skill skill = Sim.SkillManager.GetElement(mSkill);
                if (skill == null)
                {
                    return null;
                }

                if (extended == null)
                {
                    extended = new string[] { skill.Name, EAText.GetNumberString(mLevel) };
                }

                if (mLevel == 0)
                {
                    text = skill.Description;
                }
                else
                {
                    string entryKey = skill.LevelUpStrings[mLevel];
                    if (!string.IsNullOrEmpty(entryKey))
                    {
                        text = Common.LocalizeEAString(skill.SkillOwner.IsFemale, "Gameplay/Skills/Skill:SkillLevelIncreasedDialogText", new object[] { skill.SkillOwner, skill.Name, mLevel }) + Common.NewLine + Common.NewLine + Common.LocalizeEAString(skill.SkillOwner.IsFemale, entryKey, new object[] { skill.SkillOwner });
                    }
                    else
                    {
                        text = Common.LocalizeEAString(skill.SkillOwner.IsFemale, "Gameplay/Skills/Skill:SkillLevelIncreasedDialogText", new object[] { skill.SkillOwner, skill.Name, mLevel });
                    }
                }

                ManagerStory.Story story = base.PrintFormattedStory(manager, text, summaryKey, parameters, extended, logging);

                if (story != null)
                {
                    story.mOverrideImage = skill.NonPersistableData.DreamsAndPromisesIcon;
                    story.mOverrideVersion = skill.NonPersistableData.SkillProductVersion;
                }

                return story;
            }

            public override Scenario Clone()
            {
                return new StoryScenario(this);
            }
        }
    }
}
