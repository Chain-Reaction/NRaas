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
    public abstract class ExpectedPregnancyBaseScenario : HaveBabyBaseScenario
    {
        int mAdditionalBabyCount = 0;

        int mNumberOfKidsBetween = 0;

        public ExpectedPregnancyBaseScenario()
        { }
        public ExpectedPregnancyBaseScenario(SimDescription sim, SimDescription target)
            : base(sim, target)
        { }
        protected ExpectedPregnancyBaseScenario(ExpectedPregnancyBaseScenario scenario)
            : base (scenario)
        {
            mAdditionalBabyCount = scenario.mAdditionalBabyCount;
            mNumberOfKidsBetween = scenario.mNumberOfKidsBetween;
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        public static int GetNumLiveChildren (SimDescription sim)
        {
            int total = 0;

            foreach(SimDescription child in Relationships.GetChildren(sim))
            {
                if (SimTypes.IsDead(child)) continue;

                total++;
            }

            return total;
        }

        public static bool TestPreferredBaby(IScoringGenerator stats, SimDescription sim, int additionalBabyCount)
        {
            int simPreferredBabies = stats.AddScoring("PreferredBabyCount", sim);

            if (sim.Genealogy == null)
            {
                stats.IncStat("No Genealogy");
                return false;
            }
            else if ((simPreferredBabies + additionalBabyCount) <= GetNumLiveChildren(sim))
            {
                stats.AddStat("Additional Children", additionalBabyCount);
                stats.AddStat("Preferred Children", (simPreferredBabies + additionalBabyCount));
                stats.AddStat("Actual Children", GetNumLiveChildren(sim));
                stats.AddStat("Enough Children", GetNumLiveChildren(sim) - simPreferredBabies);
                return false;
            }

            if (sim.Partner == null)
            {
                return (!sim.IsHuman);
            }
            else
            {
                int partnerPreferredBabies = stats.AddScoring("PreferredBabyCount", sim.Partner);

                if (sim.Partner.Genealogy == null)
                {
                    stats.IncStat("No Genealogy");
                    return false;
                }

                if ((partnerPreferredBabies + additionalBabyCount) <= GetNumLiveChildren(sim.Partner))
                {
                    stats.AddStat("Additional Children", additionalBabyCount);
                    stats.AddStat("Prefered Children", (partnerPreferredBabies + additionalBabyCount));
                    stats.AddStat("Actual Children", GetNumLiveChildren(sim.Partner));
                    stats.AddStat("Enough Children", GetNumLiveChildren(sim.Partner) - partnerPreferredBabies);
                    return false;
                }
            }
            return true;
        }

        protected ICollection<SimDescription> FindPotentialMothers(int additionalBabyCount)
        {
            List<SimDescription> choices = new List<SimDescription>();

            foreach (SimDescription sim in Sims.All)
            {
                using (Common.TestSpan span = new Common.TestSpan(Scenarios, "FindPotentialMothers"))
                {
                    if (!AllowSpecies(sim)) continue;

                    IncStat(sim.FullName, Common.DebugLevel.Logging);

                    if ((sim.IsHuman) && (sim.Partner == null))
                    {
                        IncStat("Unpartnered");
                    }
                    else if (sim.IsPregnant)
                    {
                        IncStat("Already Pregnant");
                    }
                    else if (!Pregnancies.Allow(this, sim, sim.Partner, Managers.Manager.AllowCheck.Active))
                    {
                        //IncStat("Allow Fail");
                        continue;
                    }
                    else if (!TestPreferredBaby(this, sim, additionalBabyCount))
                    {
                        //IncStat("Preferred Fail");
                        continue;
                    }
                    else if (!Pregnancies.TestCooldown(this, sim))
                    {
                        //IncStat("Cooldown Fail");
                        continue;
                    }
                    else
                    {
                        choices.Add(sim);
                    }
                }
            }

            AddStat("Fertile Choices", choices.Count);

            return new SimScoringList(this, "PotentialMother", choices, false).GetBestByMinScore(0);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return FindPotentialMothers(mAdditionalBabyCount);
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            if (sim.Partner == null)
            {
                if (sim.IsHuman) return null;

                return Sims.Pets;
            }
            else
            {
                List<SimDescription> list = new List<SimDescription>();
                list.Add(sim.Partner);
                return list;
            }
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (Sims.HasEnough(this, sim))
            {
                IncStat("Maximum Reached");
                return false;
            }
            else if (sim.IsPregnant)
            {
                IncStat("Couple Pregnant");
                return false;
            }
            else if (!Pregnancies.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            
            return base.CommonAllow(sim);
        }

        private static int NumberOfKidsBetween(SimDescription mother, SimDescription father)
        {
            if (mother.Genealogy == null) return 0;

            if (father.Genealogy == null) return 0;

            int num = 0;
            foreach (Genealogy genealogy in mother.Genealogy.Children)
            {
                if (genealogy.Parents.Contains(father.Genealogy))
                {
                    num++;
                }
            }
            return num;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            Romances.AddWoohooerNotches(Sim, Target, false, true);

            Add(frame, new ExpectedMarriageScenario(Sim, Target), ScenarioResult.Start);
            Add(frame, new SuccessScenario(), ScenarioResult.Start);

            mNumberOfKidsBetween = NumberOfKidsBetween(Sim, Target);

            AddStat("Preferred Baby Count", (ScoringLookup.GetScore("PreferredBabyCount", Sim) + ScoringLookup.GetScore("PreferredBabyCount", Target)) / 2);

            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Pregnancies;
            }

            if (parameters == null)
            {
                parameters = new object[] { Sim, Target, mNumberOfKidsBetween };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }
    }
}
