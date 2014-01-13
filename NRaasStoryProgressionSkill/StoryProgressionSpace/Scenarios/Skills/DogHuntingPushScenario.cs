using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.SimDataElement;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class DogHuntingPushScenario : HuntingPushScenario
    {
        public DogHuntingPushScenario()
        { }
        protected DogHuntingPushScenario(DogHuntingPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "DogHunting";
        }

        public override SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { SkillNames.DogHunting };
            }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if ((SimTypes.IsSelectable(sim)) && (sim.SkillManager.GetSkillLevel(SkillNames.DogHunting) < 1))
            {
                IncStat("Active Skill Denied");
                return false;
            }
            else if (InteractionsEx.HasInteraction<Terrain.SniffOut.SniffOutDefinition>(sim))
            {
                IncStat("Has Interaction");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return sim.IsADogSpecies;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            DogHuntingSkill skill = Sim.SkillManager.AddElement(SkillNames.DogHunting) as DogHuntingSkill;
            if (skill == null)
            {
                IncStat("Skill Fail");
                return false;
            }

            if (Situations.PushInteraction<Terrain>(this, Sim, Terrain.Singleton, Terrain.SniffOut.SniffOutSingleton))
            {
                return true;
            }

            IncStat("Sniff Out Fail");
            return false;
        }

        public override Scenario Clone()
        {
            return new DogHuntingPushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, DogHuntingPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "DogHuntingPush";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP5);
            }
        }
    }
}
