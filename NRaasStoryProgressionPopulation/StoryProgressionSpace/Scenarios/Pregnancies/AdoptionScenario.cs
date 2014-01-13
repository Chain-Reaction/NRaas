using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class AdoptionScenario : AdoptionBaseScenario
    {
        public AdoptionScenario()
        { }
        protected AdoptionScenario(AdoptionScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Adoption";
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            if (!Pregnancies.RandomChanceOfAttempt(this, GetValue<BaseChanceOption, int>() + GetValue<CurrentIncreasedChanceOption, int>()))
            {
                AddValue<CurrentIncreasedChanceOption, int>(GetValue<IncreasedChanceOption, int>());

                IncStat("Chance Fail");
                return false;
            }

 	        return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if (sim.Elder)
            {
                if (!ManagerPregnancy.TestNearElderLimit(this, sim, GetValue<NearDeathLimitOption, int>()))
                {
                    IncStat("Near Death Denied");
                    return false;
                }
                else if (ExpectedPregnancyBaseScenario.GetNumLiveChildren(sim) > 0)
                {
                    IncStat("Has Children");
                    return false;
                }
                else if (AddScoring("PreferredBabyCount", sim) <= 0)
                {
                    IncStat("Score Fail");
                    return false;
                }
                else if (sim.Partner != null)
                {
                    if (Pregnancies.Allow(this, sim, sim.Partner, Managers.Manager.AllowCheck.Active))
                    {
                        IncStat("Partner Too Young");
                        return false;
                    }
                    else if (!GetValue<AllowAdoptionOption, bool>(sim.Partner))
                    {
                        IncStat("Partner Adoption Denied");
                        return false;
                    }
                    else if (AddScoring("Partner", ScoringLookup.GetScore("PreferredBabyCount", sim.Partner)) <= 0)
                    {
                        IncStat("Partner Score Fail");
                        return false;
                    }
                }

                foreach (SimDescription other in HouseholdsEx.Humans(sim.Household))
                {
                    if (other.ChildOrBelow)
                    {
                        IncStat("Child in Home");
                        return false;
                    }
                }
            }
            else if ((sim.Partner != null) && (sim.Gender == sim.Partner.Gender))
            {
                // If the sims can have children normally, then don't allow adoption
                if (Pregnancies.Allow(this, sim, sim.Partner, Managers.Manager.AllowCheck.Active))
                {
                    return false;
                }
                else if (!GetValue<AllowAdoptionOption, bool>(sim.Partner))
                {
                    IncStat("Partner Adoption Denied");
                    return false;
                }
                else if (!ExpectedPregnancyBaseScenario.TestPreferredBaby(this, sim, 0))
                {
                    IncStat("Preferred Fail");
                    return false;
                }
                else if (!Pregnancies.TestCooldown(this, sim))
                {
                    IncStat("Cooldown Fail");
                    return false;
                }
            }
            else
            {
                IncStat("Too Young");
                return false;
            }

            return true;
        }

        protected override void GetPossibleSpecies(Household house, List<CASAgeGenderFlags> species)
        {
            species.Add(CASAgeGenderFlags.Human);
        }

        protected override void UpdateDayOfLastOption(SimDescription sim)
        {
            SetElapsedTime<DayOfLastBabyOption>(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            SetValue<CurrentIncreasedChanceOption, int>(0);
            return true;
        }

        public override Scenario Clone()
        {
            return new AdoptionScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerPregnancy, AdoptionScenario>, ManagerPregnancy.IAdoptionOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "Adoption";
            }

            public override bool Value
            {
                get
                {
                    if (!ShouldDisplay()) return false;

                    return base.Value;
                }
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption,int>() <= 0) return false;

 	            return base.ShouldDisplay();
            }
        }

        public class BaseChanceOption : IntegerManagerOptionItem<ManagerPregnancy>, ManagerPregnancy.IAdoptionOption
        {
            public BaseChanceOption()
                : base(30)
            { }

            public override string GetTitlePrefix()
            {
                return "ChanceofAdoption";
            }
        }

        public class IncreasedChanceOption : IntegerManagerOptionItem<ManagerPregnancy>, ManagerPregnancy.IAdoptionOption
        {
            public IncreasedChanceOption()
                : base(10)
            { }

            public override string GetTitlePrefix()
            {
                return "IncreasedChanceAdoption";
            }
        }

        public class CurrentIncreasedChanceOption : IntegerManagerOptionItem<ManagerPregnancy>, IDebuggingOption
        {
            public CurrentIncreasedChanceOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "CurrentIncreasedChanceAdoption";
            }
        }

        public class NearDeathLimitOption : ManagerPregnancy.NearElderLimitBaseOption, ManagerPregnancy.IAdoptionOption
        {
            public NearDeathLimitOption()
                : base(20)
            { }

            public override string GetTitlePrefix()
            {
                return "NearDeathLimit";
            }
        }
    }
}
