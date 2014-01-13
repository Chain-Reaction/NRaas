using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.CommonSpace.Scoring;
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
    public class AttendCoursePushScenario : SkillGainScenario
    {
        public AttendCoursePushScenario()
        { }
        protected AttendCoursePushScenario(AttendCoursePushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "AttendCoursePush";
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool Perform(SkillNames skill)
        {
            foreach (RabbitHole hole in Sims3.Gameplay.Queries.GetObjects<RabbitHole>())
            {
                foreach (InteractionObjectPair pair in hole.Interactions)
                {
                    RabbitHole.AttendClassInRabbitHole.Definition interaction = pair.InteractionDefinition as RabbitHole.AttendClassInRabbitHole.Definition;
                    if (interaction == null) continue;

                    if (interaction.Skill != skill) continue;

                    if (Situations.PushInteraction(this, Sim, hole, interaction))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new AttendCoursePushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, AttendCoursePushScenario>
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerSkill main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    foreach (BookSkillData data in BookData.BookSkillDataList.Values)
                    {
                        data.UnlockSkill = true;
                    }
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "AttendCoursePush";
            }
        }
    }
}
