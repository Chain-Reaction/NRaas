using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public class HomeInspection
    {
        public enum Reason
        {
            NoFridge,
            NoDouble,
            NoBeds,
            TooFewBeds,
            TooFewDouble,
            TooFewSingle,
            TooFewCribs,
            TooFewStalls,
            TooFewPetBeds,
            TooFewBotBeds,
        }

        int mDoubleBeds;
        int mSingleBeds;
        int mCribs;
        int mFridges;
        int mStalls;
        int mPetBeds;
        int mFairyHouses;
        int mBotBeds;

        bool mEmpty = true;

        public HomeInspection(Lot lot)
        {
            foreach (GameObject obj in lot.GetObjects<GameObject>())
            {
                if (obj is IBedDouble)
                {
                    IBedDouble bed = obj as IBedDouble;
                    if (bed.NumberOfSpots() > 1)
                    {
                        mDoubleBeds++;
                    }
                }
                else if (obj is IBedSingle)
                {
                    IBedSingle bed = obj as IBedSingle;

                    mSingleBeds += bed.NumberOfSpots();
                }
                else if (obj is ICrib)
                {
                    mCribs++;
                }
                else if ((obj is IFridge) || (obj is IFutureFoodSynthesizer))
                {
                    mFridges++;
                }
                else if (obj is IBoxStall)
                {
                    mStalls++;
                }
                else if ((obj is IPetBed) || (obj is IPetHouse))
                {
                    mPetBeds++;
                }
                else if (obj is FairyHouse)
                {
                    mFairyHouses++;
                }
                else if (obj is IBotBed)
                {
                    mBotBeds++;
                }
                else if ((obj is IMailbox) || (obj is IResidentialTrashCan))
                {
                    continue;
                }

                mEmpty = false;
            }
        }

        public List<Result> Satisfies(Household house)
        {
            List<SimDescription> sims = null;
            if (house != null)
            {
                sims = house.AllSimDescriptions;
            }

            return Satisfies(sims);
        }
        public List<Result> Satisfies(ICollection<SimDescription> sims)
        {
            List<Result> results = new List<Result>();

            if (mEmpty) return results;

            if (mFridges <= 0)
            {
                results.Add(new Result(Reason.NoFridge, 0, 0));
            }

            if (mDoubleBeds <= 0)
            {
                results.Add(new Result(Reason.NoDouble, 0, 0));
            }

            if ((mDoubleBeds + mSingleBeds) <= 0)
            {
                results.Add(new Result(Reason.NoBeds, 0, 0));
            }

            if (sims != null)
            {
                int doubles = 0, singles = 0, fairySingles = 0, cribs = 0, stalls = 0, petBeds = 0, botbeds = 0;

                Dictionary<SimDescription, bool> partnerLookup = new Dictionary<SimDescription, bool>();

                foreach (SimDescription sim in sims)
                {
                    if (sim.IsHuman && !sim.IsEP11Bot)
                    {
                        if ((sim.IsPregnant) &&
                            ((sim.Partner == null) || (!sims.Contains(sim.Partner))))
                        {
                            cribs++;
                        }

                        if ((sim.Partner != null) && (!partnerLookup.ContainsKey(sim)) && (!partnerLookup.ContainsKey(sim.Partner)))
                        {
                            partnerLookup.Add(sim, true);
                            partnerLookup.Add(sim.Partner, true);

                            if (sims.Contains(sim.Partner))
                            {
                                doubles++;
                            }
                        }
                        else if (sim.ToddlerOrBelow)
                        {
                            cribs++;
                        }
                        else if (!partnerLookup.ContainsKey(sim))
                        {
                            if (sim.IsFairy)
                            {
                                fairySingles++;
                            }
                            else
                            {
                                singles++;
                            }
                        }
                    }
                    else if (sim.IsHorse)
                    {
                        stalls++;
                        if (sim.IsPregnant)
                        {
                            stalls++;
                        }
                    }
                    else if ((sim.IsCat) || (sim.IsADogSpecies))
                    {
                        petBeds++;
                        if (sim.IsPregnant)
                        {
                            petBeds++;
                        }
                    }
                    else if (sim.IsEP11Bot)
                    {
                        if (sim.TraitManager != null && !sim.TraitManager.HasElement(Sims3.Gameplay.ActorSystems.TraitNames.SolarPoweredChip))
                        {
                            botbeds++;
                        }
                    }
                }

                if (mDoubleBeds < doubles)
                {
                    results.Add(new Result(Reason.TooFewDouble, mDoubleBeds, doubles));
                }

                // Excess fairies go into the single bed check
                int fairyExtra = (fairySingles - mFairyHouses);
                if (fairyExtra > 0)
                {
                    singles += fairyExtra;
                }

                int singleBeds = (mDoubleBeds - doubles);
                if (singleBeds < 0)
                {
                    singleBeds = 0;
                }
                singleBeds += mSingleBeds;

                if (singleBeds < singles)
                {
                    results.Add(new Result(Reason.TooFewSingle, singleBeds, singles));
                }

                if (mCribs < cribs)
                {
                    results.Add(new Result(Reason.TooFewCribs, mCribs, cribs));
                }

                if (((mDoubleBeds * 2) + mSingleBeds) < ((doubles * 2) + singles))
                {
                    results.Add(new Result(Reason.TooFewBeds, (mDoubleBeds * 2) + mSingleBeds, (doubles * 2) + singles));
                }

                if (mStalls < stalls)
                {
                    results.Add(new Result(Reason.TooFewStalls, mStalls, stalls));
                }

                if (mPetBeds < petBeds)
                {
                    results.Add(new Result(Reason.TooFewPetBeds, mPetBeds, petBeds));
                }

                if (mBotBeds < botbeds)
                {
                    results.Add(new Result(Reason.TooFewBotBeds, mBotBeds, botbeds));
                }
            }

            return results;
        }

        public class Result
        {
            public readonly Reason mReason;

            public readonly int mRequired;
            public readonly int mExisting;

            public Result(Reason reason, int existing, int required)
            {
                mReason = reason;
                mRequired = required;
                mExisting = existing;
            }
        }
    }
}
