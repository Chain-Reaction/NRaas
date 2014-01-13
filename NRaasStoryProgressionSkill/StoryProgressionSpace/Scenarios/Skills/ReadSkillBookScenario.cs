using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Skills;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class ReadSkillBookScenario : ReadScenario
    {
        public ReadSkillBookScenario()
        { }
        protected ReadSkillBookScenario(ReadSkillBookScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ReadSkillBook";
        }

        protected override bool AllowSkill
        {
            get { return true; }
        }

        protected override bool AllowNormal
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        public override Scenario Clone()
        {
            return new ReadSkillBookScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, ReadSkillBookScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ReadSkillBook";
            }
        }
    }
}
