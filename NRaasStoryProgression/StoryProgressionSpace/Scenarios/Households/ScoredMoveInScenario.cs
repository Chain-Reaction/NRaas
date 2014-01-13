using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public abstract class ScoredMoveInScenario : MoveInScenario
    {
        SimDescription mSim = null;

        public ScoredMoveInScenario(SimDescription sim, List<SimDescription> movers)
            : base(movers)
        {
            Sim = sim;
        }
        protected ScoredMoveInScenario(ScoredMoveInScenario scenario)
            : base (scenario)
        {
            Sim = scenario.Sim;
        }

        protected abstract bool OnlyFamilyMoveIn
        {
            get;
        }

        protected SimDescription Sim
        {
            get { return mSim; }
            set { mSim = value; }
        }

        protected override bool Sort(List<Household> houses)
        {
            Dictionary<Household, int> candidates = new Dictionary<Household, int>();

            AddStat("Potentials", houses.Count);

            SimDescription oldestSim = Sim;
            foreach(SimDescription sim in Movers)
            {
                if ((oldestSim == null) || (SimTypes.IsOlderThan(sim, oldestSim)))
                {
                    oldestSim = sim;
                }
            }

            foreach (Household house in houses)
            {
                bool olderFound = false;

                foreach (SimDescription other in HouseholdsEx.All(house))
                {
                    if (Deaths.IsDying(other)) continue;

                    if (SimTypes.IsOlderThan(other, oldestSim))
                    {
                        olderFound = true;
                    }

                    int count = 0;

                    if (Flirts.IsCloselyRelated(Sim, other))
                    {
                        if (Sim.Genealogy.Parents.Contains(other.Genealogy))
                        {
                            count += 10;
                        }
                        else
                        {
                            count++;
                        }
                    }
                    else if (OnlyFamilyMoveIn)
                    {
                        continue;
                    }

                    bool checkRel = false;
                    if (other.YoungAdultOrAbove)
                    {
                        checkRel = true;
                    }
                    else if ((Households.AllowSoloMove(Sim)) && (other.TeenOrAbove))
                    {
                        checkRel = true;
                    }
                    
                    if (checkRel)
                    {
                        int rel = 0;

                        Relationship relation = Relationship.Get(Sim, other, false);
                        if (relation != null)
                        {
                            rel = (int)(relation.LTR.Liking / 25);

                            if ((relation.AreRomantic()) && (rel > 0))
                            {
                                rel += 5;
                            }

                            count += rel;
                        }

                        if (Households.AllowSoloMove(Sim))
                        {
                            if (rel < 3) continue;
                        }
                    }

                    if (Sim.Partner == other)
                    {
                        count += 10;
                    }

                    if (!candidates.ContainsKey(house))
                    {
                        candidates.Add(house, count);
                    }
                    else
                    {
                        candidates[house] += count;
                    }
                }

                if (!olderFound)
                {
                    candidates.Remove(house);
                }
            }

            houses.Clear();
            if (candidates.Count > 0)
            {
                ScoringList<Household> scoring = new ScoringList<Household>();
                foreach (KeyValuePair<Household, int> candidate in candidates)
                {
                    AddScoring("", candidate.Value);

                    scoring.Add(candidate.Key, candidate.Value);
                }

                houses.AddRange(scoring.GetBestByMinScore(1));
            }
            else
            {
                IncStat("No Candidates");
            }

            return true;
        }
    }
}
