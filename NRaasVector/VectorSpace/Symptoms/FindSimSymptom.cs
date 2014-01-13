using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.VectorSpace.Booters;
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
using System.Collections.Generic;

namespace NRaas.VectorSpace.Symptoms
{
    public class FindSimSymptom : SymptomBooter.Data
    {
        string mScoring;

        int mMinimum;

        bool mAllowActive;

        public FindSimSymptom(XmlDbRow row)
            : base(row)
        {
            mScoring = row.GetString("Scoring");

            if (!string.IsNullOrEmpty(mScoring))
            {
                if (ScoringLookup.GetScoring(mScoring) == null)
                {
                    BooterLogger.AddError(Guid + " Invalid Scoring: " + mScoring);
                }

                mMinimum = row.GetInt("Minimum", 0);
            }

            mAllowActive = row.GetBool("AllowActive");
        }

        public override void Perform(Sim sim, DiseaseVector vector)
        {
            if (SimTypes.IsDead(sim.SimDescription)) return;

            if (!mAllowActive)
            {
                if (SimTypes.IsSelectable(sim.SimDescription)) return;
            }

            bool found = false;

            foreach (Sim other in sim.LotCurrent.GetSims())
            {
                if (other == sim) continue;

                if (ScoringLookup.GetScore(mScoring, other.SimDescription) >= mMinimum)
                {
                    found = true;
                    break;
                }
            }

            if (found) return;

            List<Lot> lots = new List<Lot>(LotManager.sLots.Values);

            while (lots.Count > 0)
            {
                Lot lot = RandomUtil.GetRandomObjectFromList(lots);
                lots.Remove(lot);

                if (lot.IsWorldLot) continue;

                if (lot == sim.LotCurrent) continue;

                foreach (Sim other in lot.GetSims())
                {
                    if (ScoringLookup.GetScore(mScoring, other.SimDescription) >= mMinimum)
                    {
                        InteractionDefinition definition = null;

                        if (lot.IsCommunityLot)
                        {
                            definition = VisitCommunityLot.Singleton;
                        }
                        else
                        {
                            if (sim.IsGreetedOnLot(lot))
                            {
                                definition = GoToLot.Singleton;
                            }
                            else
                            {
                                definition = VisitLot.Singleton;
                            }
                        }

                        InteractionInstance instance = definition.CreateInstance(lot, sim, new InteractionPriority(InteractionPriorityLevel.Autonomous), true, true);
                        sim.InteractionQueue.Add(instance);

                        return;
                    }
                }              
            }
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Scoring: " + mScoring;
            result += Common.NewLine + " Minimum: " + mMinimum;

            return result;
        }
    }
}
