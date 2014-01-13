using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class TraitMatch : SelectionTestableOptionList<TraitMatch.Item, int, int>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.TraitMatch";
        }

        public class Item : TestableOption<int, int>
        {
            protected static List<TraitNames> GetTraits(SimDescription sim)
            {
                List<TraitNames> traits = new List<TraitNames>();

                if (sim.TraitManager != null)
                {
                    foreach (Trait trait in sim.TraitManager.List)
                    {
                        if (trait.IsReward) continue;

                        traits.Add(trait.Guid);
                    }
                }

                return traits;
            }
            protected static List<TraitNames> GetTraits(MiniSimDescription sim)
            {
                List<TraitNames> traits = new List<TraitNames>();

                if (sim.Traits != null)
                {
                    traits.AddRange(sim.Traits);
                }

                return traits;
            }

            protected static int GetScore(List<TraitNames> traitsA, List<TraitNames> traitsB)
            {
                int score = 0;
                foreach (TraitNames traitA in traitsA)
                {
                    foreach (TraitNames traitB in traitsB)
                    {
                        if (traitA == traitB)
                        {
                            score += 1;
                        }
                        else if (TraitManager.DoTraitsConflict(traitA, traitB))
                        {
                            score -= 1;
                        }
                    }
                }

                return score;
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<int,int> results)
            {
                List<TraitNames> traitsA = GetTraits(me);

                List<TraitNames> traitsB = null;
                if (actor is SimDescription)
                {
                    traitsB = GetTraits(actor as SimDescription);
                }
                else
                {
                    traitsB = GetTraits(actor as MiniSimDescription);
                }

                int score = GetScore(traitsA, traitsB);

                results[score] = score;
                return true;
            }
            public override bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<int, int> results)
            {
                List<TraitNames> traitsA = GetTraits(me);

                List<TraitNames> traitsB = null;
                if (actor is SimDescription)
                {
                    traitsB = GetTraits(actor as SimDescription);
                }
                else
                {
                    traitsB = GetTraits(actor as MiniSimDescription);
                }

                int score = GetScore(traitsA, traitsB);

                results[score] = score;
                return true;
            }

            public override void SetValue(int value, int storeType)
            {
                mValue = value;

                mName = EAText.GetNumberString(value);
            }
        }
    }
}
