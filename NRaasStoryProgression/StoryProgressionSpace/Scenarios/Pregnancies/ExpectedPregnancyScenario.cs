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
    public class ExpectedPregnancyScenario : ExpectedPregnancyBaseScenario
    {
        public ExpectedPregnancyScenario()
        { }
        public ExpectedPregnancyScenario(SimDescription sim, SimDescription target)
            : base(sim, target)
        { }
        protected ExpectedPregnancyScenario(ExpectedPregnancyScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Pregnancy";
        }

        protected bool AllowSteady (SimDescription sim, SimDescription target)
        {
            if ((sim.IsMarried) || (target.IsMarried)) return true;

            if (!Romances.AllowMarriage(this, sim, Managers.Manager.AllowCheck.Active)) return true;
            if (!Romances.AllowMarriage(this, target, Managers.Manager.AllowCheck.Active)) return true;

            int minimumElapsed = GetValue<EngagementScenario.MinTimeFromPartnerToEngagementOption, int>() + GetValue<CooldownMarriageScenario.MinTimeFromEngagementToMarriageOption, int>();

            if (GetElapsedTime<DayOfLastRomanceOption>(sim) < minimumElapsed)
            {
                IncStat("Elapsed Denied");
                return false;
            }

            if (GetElapsedTime<DayOfLastRomanceOption>(target) < minimumElapsed)
            {
                IncStat("Elapsed Denied");
                return false;
            }

            if (AddScoring ("Marriage", sim, target) <= 0) return true;
            if (AddScoring ("Marriage", target, sim) <= 0) return true;

            return false;
        }

        protected override GatherResult TargetGather(List<Scenario> list, ref bool random)
        {
            GatherResult result = base.TargetGather(list, ref random);

            if ((Target == null) && (result == GatherResult.Failure))
            {
                Lots.IncreaseImmigrationPressure(Pregnancies, GetValue<ImmigrantRequirementScenario.PressureOption, int>());
            }

            return result;
        }

        protected override bool Allow()
        {
            if (GetValue<BaseChanceOption, int>() <= 0) return false;

            if (!Pregnancies.RandomChanceOfAttempt(this, GetValue<BaseChanceOption, int>() + GetValue<CurrentIncreasedChanceOption, int>()))
            {
                AddValue<CurrentIncreasedChanceOption, int>(GetValue<IncreasedChanceOption, int>());

                IncStat("Chance Fail");
                return false;
            }

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return sim.IsHuman;
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (!AllowSteady(Sim, Target))
            {
                IncStat("Expected: Steady Denied");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override GatherResult Gather(List<Scenario> list, ref int continueChance, ref int maximum, ref bool random)
        {
            int mAdditionalBabyCount = Sims.GetDepopulationDanger(false);
            if (mAdditionalBabyCount > 0)
            {
                mAdditionalBabyCount--;
            }

            return base.Gather(list, ref continueChance, ref maximum, ref random);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            SetValue<CurrentIncreasedChanceOption, int>(0);
            return true;
        }

        public override Scenario Clone()
        {
            return new ExpectedPregnancyScenario(this);
        }

        public class BaseChanceOption : IntegerScenarioOptionItem<ManagerPregnancy, ExpectedPregnancyScenario>, IAdjustForVacationOption
        {
            public BaseChanceOption()
                : base(30)
            { }

            public override string GetTitlePrefix()
            {
                return "ChanceofAttempt";
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
                return "IncreasedChancePregancy";
            }
        }

        public class CurrentIncreasedChanceOption : IntegerManagerOptionItem<ManagerPregnancy>, IDebuggingOption
        {
            public CurrentIncreasedChanceOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "CurrentIncreasedChancePregancy";
            }
        }

        public class MinTimeFromBabyToPregnancyOption : Manager.CooldownOptionItem<ManagerPregnancy>
        {
            public MinTimeFromBabyToPregnancyOption()
                : base(3)
            { }

            public override string GetTitlePrefix()
            {
                return "CooldownBetweenPregnancies";
            }
        }

        public class MinTimeFromRomanceToPregnancyOption : Manager.CooldownOptionItem<ManagerPregnancy>
        {
            public MinTimeFromRomanceToPregnancyOption()
                : base(3)
            { }

            public override string GetTitlePrefix()
            {
                return "CooldownMarriagetoPregnancy";
            }
        }
    }
}
