using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class ExpectedPregnancyPetScenario : ExpectedPregnancyBaseScenario
    {
        public ExpectedPregnancyPetScenario()
        { }
        public ExpectedPregnancyPetScenario(SimDescription sim, SimDescription target)
            : base(sim, target)
        { }
        protected ExpectedPregnancyPetScenario(ExpectedPregnancyPetScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "PetPregnancy";
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            ICollection<SimDescription> targets = base.GetTargets(sim);
            if ((targets != null) && (targets.Count > 0))
            {
                return targets;
            }

            return Sims.Pets;
        }

        protected override bool Allow()
        {
            if (GetValue<BaseChanceOption, int>() <= 0) return false;

            if (!RandomUtil.RandomChance(GetValue<BaseChanceOption,int>() + GetValue<CurrentIncreasedChanceOption,int>()))
            {
                AddValue<CurrentIncreasedChanceOption, int>(GetValue<IncreasedChanceOption, int>());

                IncStat("Chance Fail");
                return false;
            }

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return (!sim.IsHuman);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            SetValue<CurrentIncreasedChanceOption, int>(0);
            return true;
        }

        public override Scenario Clone()
        {
            return new ExpectedPregnancyPetScenario(this);
        }

        public class BaseChanceOption : IntegerScenarioOptionItem<ManagerPregnancy, ExpectedPregnancyPetScenario>, IAdjustForVacationOption
        {
            public BaseChanceOption()
                : base(30)
            { }

            public override string GetTitlePrefix()
            {
                return "PetChanceofAttempt";
            }

            public bool AdjustForVacationTown()
            {
                if (GameUtils.IsUniversityWorld())
                {
                    SetValue(0);
                }

                return true;
            }
        }

        public class IncreasedChanceOption : IntegerManagerOptionItem<ManagerPregnancy>
        {
            public IncreasedChanceOption()
                : base(10)
            { }

            public override string GetTitlePrefix()
            {
                return "IncreasedChancePetPregancy";
            }
        }

        public class CurrentIncreasedChanceOption : IntegerManagerOptionItem<ManagerPregnancy>, IDebuggingOption
        {
            public CurrentIncreasedChanceOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "CurrentIncreasedChancePetPregancy";
            }
        }
    }
}
