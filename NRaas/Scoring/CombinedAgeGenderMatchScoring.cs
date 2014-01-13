using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class CombinedAgeGenderMatchScoring : CombinedSimScoring<AgeGenderMatchScoring, CASAgeGenderFlags, DualSimScoringParameters>, ISimScoringCache
    {
        Dictionary<CASAgeGenderFlags, List<AgeGenderMatchScoring>> mScoring = new Dictionary<CASAgeGenderFlags, List<AgeGenderMatchScoring>>();

        public CombinedAgeGenderMatchScoring()
        { }

        protected override CASAgeGenderFlags GetValue(SimDescription sim)
        {
            return (sim.Age | sim.Gender);
        }

        public override void Collect(SimDescription obj)
        {
            base.Collect(obj);

            List<IScoring<SimDescription, DualSimScoringParameters>> scoring = new List<IScoring<SimDescription, DualSimScoringParameters>>();

            List<AgeGenderMatchScoring> collectedScoring;
            if (mScoring.TryGetValue(obj.Age | obj.Gender, out collectedScoring))
            {
                foreach (AgeGenderMatchScoring score in collectedScoring)
                {
                    scoring.Add(score);
                }
            }

            mHelper.SetRawScoring(scoring);
        }

        protected override ScoringDelegate GetScoringDelegate(SimDescription actor)
        {
            return new AgeGenderMatchScoring.Delegate(actor).Score;
        }

        public override bool Combine(List<IScoring<SimDescription, DualSimScoringParameters>> scoring)
        {
            if (!base.Combine(scoring)) return false;

            foreach (AgeGenderMatchScoring score in mHelper.RawScoring)
            {
                CASAgeGenderFlags age = score.OtherAgeGender & CASAgeGenderFlags.AgeMask;
                if (age == CASAgeGenderFlags.None)
                {
                    age = CASAgeGenderFlags.AgeMask;
                }

                CASAgeGenderFlags gender = score.OtherAgeGender & CASAgeGenderFlags.GenderMask;
                if (gender == CASAgeGenderFlags.None)
                {
                    gender = CASAgeGenderFlags.GenderMask;
                }

                ICollection<CASAgeGenderFlags> species = score.OtherSpecies;

                List<CASAgeGenderFlags> ages = new List<CASAgeGenderFlags>();
                List<CASAgeGenderFlags> genders = new List<CASAgeGenderFlags>();

                foreach (CASAgeGenderFlags element in Enum.GetValues(typeof(CASAgeGenderFlags)))
                {
                    if (element == CASAgeGenderFlags.AgeMask) continue;

                    if (element == CASAgeGenderFlags.GenderMask) continue;

                    if (element == CASAgeGenderFlags.SpeciesMask) continue;

                    if ((age & element) == element)
                    {
                        ages.Add(element);
                    }

                    if ((gender & element) == element)
                    {
                        genders.Add(element);
                    }
                }

                foreach (CASAgeGenderFlags singleAge in ages)
                {
                    foreach (CASAgeGenderFlags singleGender in genders)
                    {
                        foreach (CASAgeGenderFlags singleSpecies in species)
                        {
                            List<AgeGenderMatchScoring> list;
                            if (!mScoring.TryGetValue(singleAge | singleGender | singleSpecies, out list))
                            {
                                list = new List<AgeGenderMatchScoring>();

                                mScoring.Add(singleAge | singleGender | singleSpecies, list);
                            }

                            list.Add(score);
                        }
                    }
                }
            }

            return true;
        }
    }
}

