using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerPregnancy : Manager
    {
        List<SimDescription> mPregnantSims = null;

        static ManagerPregnancy()
        { }

        public ManagerPregnancy(Main manager)
            : base (manager)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Pregnancy";
        }

        public List<SimDescription> PregnantSims
        {
            get 
            {
                if (mPregnantSims == null)
                {
                    mPregnantSims = new List<SimDescription>();

                    foreach (SimDescription sim in Sims.All)
                    {
                        if (sim.IsPregnant)
                        {
                            mPregnantSims.Add(sim);
                        }
                    }
                }
                return mPregnantSims; 
            }
        }

        public void ResetPregnantSims()
        {
            mPregnantSims = null;
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerPregnancy>(this).Perform(initial);
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if (initialPass)
            {
                InteractionTuning tuning = Tunings.GetTuning<RabbitHole, Pregnancy.GoToHospital.Definition>();
                if (tuning != null)
                {
                    tuning.Availability.Teens = true;
                    tuning.Availability.Adults = true;
                    tuning.Availability.Elders = true;
                }

                tuning = Tunings.GetTuning<Lot, Pregnancy.HaveBabyHome.Definition>();
                if (tuning != null)
                {
                    tuning.Availability.Teens = true;
                    tuning.Availability.Adults = true;
                    tuning.Availability.Elders = true;
                }

                tuning = Tunings.GetTuning<RabbitHole, Pregnancy.HaveBabyHospital.Definition>();
                if (tuning != null)
                {
                    tuning.Availability.Teens = true;
                    tuning.Availability.Adults = true;
                    tuning.Availability.Elders = true;
                }
            }

            if ((ProgressionEnabled) && (fullUpdate))
            {
                ResetPregnantSims();
            }

            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public bool Allow(IScoringGenerator stats, SimDescription actor, SimDescription target, AllowCheck check)
        {
            if (!Allow(stats, actor, check)) return false;

            if (!Allow(stats, target, check)) return false;

            if (!Flirts.CanHaveAutonomousRomance(stats, actor, target, ((check & AllowCheck.Active) == AllowCheck.Active)))
            {
                return false;
            }

            SimData actorData = GetData(actor);
            SimData targetData = GetData(target);

            if (!actorData.Allowed<AllowCasteCanBePregnantOption>(targetData, false))
            {
                stats.IncStat("Can Be Caste Denied");
                return false;
            }
            else if (actorData.Disallowed<DisallowCasteCanBePregnantOption>(targetData, false))
            {
                stats.IncStat("Can Be Caste Denied");
                return false;
            } 
            else if (!actorData.Allowed<AllowCastePregnancyOption>(targetData, true))
            {
                stats.IncStat("Participation Caste Denied");
                return false;
            }
            else if (actorData.Disallowed<DisallowCastePregnancyOption>(targetData, true))
            {
                stats.IncStat("Participation Caste Denied");
                return false;
            }
            else if (!DualAllow(stats, actorData, targetData, check))
            {
                return false;
            }

            return true;
        }
        public bool Allow(IScoringGenerator stats, Sim sim)
        {
            return PrivateAllow(stats, sim);
        }
        public bool Allow(IScoringGenerator stats, SimDescription sim)
        {
            return PrivateAllow(stats, sim);
        }
        protected bool Allow(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            return PrivateAllow(stats, sim, check);
        }

        protected override bool PrivateAllow(IScoringGenerator stats, SimDescription sim, SimData settings, AllowCheck check)
        {
            if (sim.Household == null)
            {
                stats.IncStat("Allow: No Home");
                return false;
            }

            if (!settings.GetValue<AllowPregnancyParticipationOption, bool>())
            {
                stats.IncStat("Allow: Participation Denied");
                return false;
            }

            return true;
        }

        public bool AllowImpregnation(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            if (!Allow(stats, sim, check)) return false;
            
            if (SimTypes.IsTourist(sim))
            {
                stats.IncStat("Allow: Tourist");
                return false;
            }

            if ((check & AllowCheck.Active) == AllowCheck.Active)
            {
                if (sim.Household.IsTravelHousehold)
                {
                    stats.IncStat("Allow: Travel Household");
                    return false;
                }
            }

            if (sim.LotHome == null)
            {
                if (!GetValue<NewcomerGoneScenario.AllowHomelessMoveInOptionV2, bool>())
                {
                    stats.IncStat("Allow: Homeless Move In Denied");
                    return false;
                }
            }

            if (SimTypes.IsServiceOrRole(sim, false))
            {
                if (SimTypes.IsOccult(sim, Sims3.UI.Hud.OccultTypes.ImaginaryFriend))
                {
                    stats.IncStat("Allow: Service Imaginary Denied");
                    return false;
                }
            }

            if (!GetValue<AllowCanBePregnantOption, bool>(sim))
            {
                stats.IncStat("Allow: Can Be Pregnant Denied");
                return false;
            }

            if ((sim.IsHuman) && ((check & AllowCheck.UserDirected) == AllowCheck.None))
            {
                if (!TestNearElderLimit(stats, sim, GetValue<NearElderLimitOption, int>())) return false;
            }

            return true;
        }

        public static bool TestNearElderLimit(Common.IStatGenerator stats, SimDescription sim, int limit)
        {
            if ((sim.AdultOrAbove) && (limit > 0))
            {
                int daysToTransition = (int)(AgingManager.Singleton.AgingYearsToSimDays(AgingManager.GetMaximumAgingStageLength(sim)) - AgingManager.Singleton.AgingYearsToSimDays(sim.AgingYearsSinceLastAgeTransition));

                if (daysToTransition < limit)
                {
                    stats.IncStat("Near Elder Denied");
                    return false;
                }
            }

            return true;
        }

        public bool RandomChanceOfAttempt(Common.IStatGenerator stats, float baseChance)
        {
            stats.AddStat("Base Chance", baseChance);

            baseChance += (Sims.GetDepopulationDanger(false) * 10);
            if (Households.FamiliesPacked && (Lots.FreeLotRatio < 0.2f))
            {
                baseChance /= 3f;
            }

            stats.AddStat("Adjusted Chance", baseChance);

            return RandomUtil.RandomChance(baseChance);
        }

        public override Scenario GetImmigrantRequirement(ImmigrationRequirement requirement)
        {
            return new ImmigrantRequirementScenario(requirement);
        }

        public override void RemoveSim(ulong sim)
        {
            base.RemoveSim(sim);

            RemoveSim(mPregnantSims, sim);
        }

        public bool TestCooldown(Common.IStatGenerator stats, SimDescription sim)
        {
            if (sim.Partner == null)
            {
                stats.IncStat("No Partner");
                return false;
            }
            else if (stats.AddScoring("Pregnancy Cooldown", TestElapsedTime<DayOfLastRomanceOption, ExpectedPregnancyScenario.MinTimeFromRomanceToPregnancyOption>(sim)) < 0)
            {
                stats.AddStat("Too Soon After Romance", GetElapsedTime<DayOfLastRomanceOption>(sim));
                return false;
            }
            else if (stats.AddScoring("Pregnancy Cooldown", TestElapsedTime<DayOfLastRomanceOption, ExpectedPregnancyScenario.MinTimeFromRomanceToPregnancyOption>(sim.Partner)) < 0)
            {
                stats.AddStat("Too Soon After Romance", GetElapsedTime<DayOfLastRomanceOption>(sim.Partner));
                return false;
            }
            else if (stats.AddScoring("New Baby Cooldown", TestElapsedTime<DayOfLastBabyOption, ExpectedPregnancyScenario.MinTimeFromBabyToPregnancyOption>(sim)) < 0)
            {
                stats.AddStat("Too Soon After Baby", GetElapsedTime<DayOfLastBabyOption>(sim));
                return false;
            }
            else if (stats.AddScoring("New Baby Cooldown", TestElapsedTime<DayOfLastBabyOption, ExpectedPregnancyScenario.MinTimeFromBabyToPregnancyOption>(sim.Partner)) < 0)
            {
                stats.AddStat("Too Soon After Baby", GetElapsedTime<DayOfLastBabyOption>(sim.Partner));
                return false;
            }
            return true;
        }

        public class Updates : AlertLevelOption<ManagerPregnancy>
        {
            public Updates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "Stories";
            }
        }

        public class DebugOption : DebugLevelOption<ManagerPregnancy>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        public class SpeedOption : SpeedBaseOption<ManagerPregnancy>
        {
            public SpeedOption()
                : base(500, true)
            { }
        }

        public class DumpStatsOption : DumpStatsBaseOption<ManagerPregnancy>
        {
            public DumpStatsOption()
                : base(5)
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerPregnancy>
        {
            public DumpScoringOption()
            { }
        }

        public class TicksPassedOption : TicksPassedBaseOption<ManagerPregnancy>
        {
            public TicksPassedOption()
            { }
        }

        public class NearElderLimitOption : NearElderLimitBaseOption
        {
            public NearElderLimitOption()
                : base(10)
            { }

            public override string GetTitlePrefix()
            {
                return "NearElderLimit";
            }
        }

        public abstract class NearElderLimitBaseOption : IntegerManagerOptionItem<ManagerPregnancy>, ISpeedBaseOption
        {
            public NearElderLimitBaseOption(int defValue)
                : base(defValue)
            { }

            public bool AdjustsForAgeSpan
            {
                get { return true; }
            }

            public bool AdjustsForSpeed
            {
                get { return false; }
            }

            protected override string GetPrompt()
            {
                float agingYears = 0f;
                agingYears += AgingManager.Singleton.GetAgingStageLengthForSpecies(CASAgeGenderFlags.Human, CASAgeGenderFlags.Baby);
                agingYears += AgingManager.Singleton.GetAgingStageLengthForSpecies(CASAgeGenderFlags.Human, CASAgeGenderFlags.Toddler);

                int childAge = (int)AgingManager.Singleton.AgingYearsToSimDays(agingYears);

                agingYears += AgingManager.Singleton.GetAgingStageLengthForSpecies(CASAgeGenderFlags.Human, CASAgeGenderFlags.Child);

                int teenAge = (int)AgingManager.Singleton.AgingYearsToSimDays(agingYears);

                return Common.Localize("NearElderLimit:Prompt", IsFemaleLocalization(), new object[] { childAge, teenAge });
            }
        }

        public interface IAdoptionOption : INotRootLevelOption
        { }

        public class AdoptionListingOption : NestingManagerOptionItem<ManagerPregnancy, IAdoptionOption>
        {
            public AdoptionListingOption()
            { }

            public override string GetTitlePrefix()
            {
                return "AdoptionListing";
            }
        }
    }
}

