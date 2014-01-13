using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class HouseholdBreakdown
    {
        public enum ChildrenMove
        {
            Stay = 0,
            Go = 1,
            Scoring = 2,
            RelatedStay = 3,
            RelatedGo = 4
        }

        private List<SimDescription> mSims = new List<SimDescription>();

        private List<SimDescription> mStaying = new List<SimDescription>();
        private List<SimDescription> mGoing = new List<SimDescription>();

        public List<SimDescription> Going
        {
            get
            {
                return mGoing;
            }
        }

        public int GetStayingCount(ref int humans, ref int pets)
        {
            return Households.NumSimsIncludingPregnancy(mStaying, ref humans, ref pets);
        }

        public bool NoneStaying
        {
            get { return (Households.NumSimsIncludingPregnancy(mStaying) == 0); }
        }

        public int GetGoingCount(ref int humans, ref int pets)
        {
            return Households.NumSimsIncludingPregnancy(mGoing, ref humans, ref pets);
        }

        public bool NoneGoing
        {
            get { return (Households.NumSimsIncludingPregnancy(mGoing) == 0); }
        }

        public bool SimGoing
        {
            get
            {
                foreach (SimDescription sim in mSims)
                {
                    if (!mGoing.Contains(sim)) return false;
                }
                return true;
            }
        }

        public Lot SimLot
        {
            get
            {
                foreach (SimDescription sim in mSims)
                {
                    if (sim.LotHome != null)
                    {
                        return sim.LotHome;
                    }
                }
                return null;
            }
        }

        public HouseholdBreakdown(StoryProgressionObject manager, IScoringGenerator stats, string tag, SimDescription me, ChildrenMove children, bool ignoreHead)
        {
            if (me != null)
            {
                mSims.Add(me);
            }

            Perform(manager, stats, tag, mSims, children, ignoreHead);

            Tally(stats, tag, ignoreHead);
        }
        public HouseholdBreakdown(StoryProgressionObject manager, IScoringGenerator stats, string tag, SimDescription a, SimDescription b, ChildrenMove children, bool ignoreHead)
        {
            if (a != null)
            {
                mSims.Add(a);
            }

            if (b != null)
            {
                mSims.Add(b);
            }

            if ((mSims.Count > 1) && (a.Household != b.Household))
            {
                List<SimDescription> sims = new List<SimDescription>();
                sims.Add(a);

                Perform(manager, stats, tag, sims, children, ignoreHead);

                sims = new List<SimDescription>();
                sims.Add(b);

                Perform(manager, stats, tag, sims, children, ignoreHead);
            }
            else
            {
                Perform(manager, stats, tag, mSims, children, ignoreHead);
            }

            Tally(stats, tag, ignoreHead);
        }

        protected void Tally(IScoringGenerator stats, string tag, bool ignoreHead)
        {
            if (!Common.kDebugging) return;

            string prefix = tag + " Break";
            if (ignoreHead)
            {
                prefix += " Ignore";
            }

            int humans = 0, pets = 0;

            stats.AddStat(prefix + ": Sims", mSims.Count);
            stats.AddStat(prefix + ": Going", GetGoingCount(ref humans, ref pets));
            stats.AddStat(prefix + ": Going Human", humans);
            stats.AddStat(prefix + ": Going Pets", pets);
            stats.AddStat(prefix + ": Staying", GetStayingCount(ref humans, ref pets));
            stats.AddStat(prefix + ": Staying Human", humans);
            stats.AddStat(prefix + ": Staying Pets", pets);

            if (SimGoing)
            {
                stats.IncStat(prefix + ": SimGoing");
            }
        }

        protected bool ScoreChildMove(IScoringGenerator stats, string tag, SimDescription child, List<SimDescription> movingSims)
        {
            if (child.Genealogy.Parents.Count == 0)
            {
                stats.IncStat(tag + " ChildMove: No Parents");
                return false;
            }

            int firstScore = 0;
            bool firstMoving = false;

            SimDescription mom = null, dad = null;
            Relationships.GetParents(child, out mom, out dad);

            if (mom != null)
            {
                firstMoving = movingSims.Contains(mom);

                if ((firstMoving) || (mom.Household == child.Household))
                {
                    firstScore = stats.AddScoring(tag + " ChildMove", ScoringLookup.GetScore("ParentChild", child, mom));
                }
                else
                {
                    return true;
                }
            }

            int secondScore = 0;
            bool secondMoving = false;

            if (dad != null)
            {
                secondMoving = movingSims.Contains(dad);

                if ((secondMoving) || (dad.Household == child.Household))
                {
                    secondScore = stats.AddScoring(tag + " ChildMove", ScoringLookup.GetScore("ParentChild", child, dad));
                }
                else
                {
                    return true;
                }
            }

            if (firstScore > secondScore)
            {
                return firstMoving;
            }
            else
            {
                return secondMoving;
            }
        }

        protected static bool IsPartner(SimDescription me, List<SimDescription> sims, out SimDescription partner)
        {
            foreach (SimDescription sim in sims)
            {
                if (sim.Partner == me)
                {
                    partner = sim;
                    return true;
                }
            }

            partner = null;
            return false;
        }

        protected void Perform(StoryProgressionObject manager, IScoringGenerator stats, string tag, List<SimDescription> movers, ChildrenMove children, bool ignoreHead)
        {
            if (movers.Count == 0) return;

            SimDescription focus = movers[0];

            if (focus.Household == null)
            {
                stats.IncStat(tag + " Break: No Home");

                mGoing.AddRange(movers);
                return;
            }
            else if (SimTypes.IsSpecial(focus))
            {
                stats.IncStat(tag + " Break: Special");

                mGoing.AddRange(movers);
                return;
            }
            else
            {
                bool adult = false;
                foreach (SimDescription sim in movers)
                {
                    if (manager.Households.AllowGuardian(sim))
                    {
                        adult = true;
                        break;
                    }
                }

                if (!adult)
                {
                    stats.IncStat(tag + " Break: Child Transfer");

                    mStaying.AddRange(Households.All(focus.Household));

                    foreach (SimDescription sim in movers)
                    {
                        if (SimTypes.IsSelectable(sim)) continue;

                        mStaying.Remove(sim);

                        mGoing.Add(sim);
                    }

                    return;
                }

                List<SimDescription> going = new List<SimDescription>();
                List<SimDescription> staying = new List<SimDescription>();
                List<SimDescription> houseChildrenPets = new List<SimDescription>();

                bool ancestral = manager.GetValue<IsAncestralOption, bool>(focus.Household);

                SimDescription head = null;
                if (!ignoreHead)
                {
                    head = SimTypes.HeadOfFamily(focus.Household);
                }
                else if (ancestral)
                {
                    stats.IncStat(tag + " Break: Ancestral Head Denied");

                    mStaying.AddRange(Households.All(focus.Household));
                    return;
                }

                foreach (SimDescription sim in Households.All(focus.Household))
                {
                    stats.IncStat(sim.FullName, Common.DebugLevel.Logging);

                    SimDescription partner = null;
                    if (SimTypes.IsSelectable(sim))
                    {
                        stats.IncStat(tag + " Break: Active");

                        staying.Add(sim);
                    }
                    else if (!manager.Households.Allow(stats, sim, 0))
                    {
                        stats.IncStat(tag + " Break: User Denied");

                        staying.Add(sim);
                    }
                    else if (IsPartner(sim, staying, out partner))
                    {
                        stats.IncStat(tag + " Break: Partner Stay");

                        staying.Add(sim);
                    }
                    else if ((IsPartner(sim, movers, out partner)) || (IsPartner(sim, going, out partner)))
                    {
                        if ((head == sim) && (Households.NumHumans(focus.Household) != 1))
                        {
                            stats.IncStat(tag + " Break: Partner Go Denied");

                            going.Remove(partner);

                            staying.Add(sim);

                            if (!staying.Contains(partner))
                            {
                                staying.Add(partner);
                            }
                        }
                        else
                        {
                            stats.IncStat(tag + " Break: Partner Go");

                            going.Add(sim);
                        }
                    }
                    else if (movers.Contains(sim))
                    {
                        if ((head == sim) && (Households.NumHumans(focus.Household) != 1))
                        {
                            stats.IncStat(tag + " Break: Go Denied");

                            staying.Add(sim);
                        }
                        else
                        {
                            stats.IncStat(tag + " Break: Go");

                            going.Add(sim);
                        }
                    }
                    else if (head == sim)
                    {
                        stats.IncStat(tag + " Break: Head Stay");

                        staying.Add(sim);
                    }
                    else if ((sim.YoungAdultOrAbove) && (!sim.IsPet))
                    {
                        stats.IncStat(tag + " Break: Stay");

                        staying.Add(sim);
                    }
                    else
                    {
                        houseChildrenPets.Add(sim);
                    }
                }

                List<SimDescription> extraChildrenPets = new List<SimDescription>();
                foreach (SimDescription child in houseChildrenPets)
                {
                    bool bGoing = false;
                    bool bMatch = false;

                    if (child.IsPet)
                    {
                        int goingLiking = int.MinValue;
                        foreach (SimDescription foci in going)
                        {
                            if (child.LastName == foci.LastName)
                            {
                                bMatch = true;

                                int liking = ManagerSim.GetLTR(child, foci);
                                if (goingLiking < liking)
                                {
                                    goingLiking = liking;
                                }
                            }
                        }

                        int stayingLiking = int.MinValue;
                        foreach (SimDescription foci in staying)
                        {
                            if (child.LastName == foci.LastName)
                            {
                                bMatch = true;

                                int liking = ManagerSim.GetLTR(child, foci);
                                if (stayingLiking < liking)
                                {
                                    stayingLiking = liking;
                                }
                            }
                        }

                        if (goingLiking > stayingLiking)
                        {
                            bGoing = true;
                        }
                    }
                    else
                    {
                        if (children == ChildrenMove.RelatedStay)
                        {
                            foreach (SimDescription parent in Relationships.GetParents(child))
                            {
                                if (staying.Contains(parent))
                                {
                                    bMatch = true;
                                }
                            }
                        }
                        else if (children == ChildrenMove.RelatedGo)
                        {
                            foreach (SimDescription parent in Relationships.GetParents(child))
                            {
                                if (going.Contains(parent))
                                {
                                    bMatch = true;
                                }
                            }
                        }
                        else if (children != ChildrenMove.Stay)
                        {
                            foreach (SimDescription foci in going)
                            {
                                if (Relationships.GetChildren(foci).Contains(child))
                                {
                                    bMatch = true;
                                    if ((children != ChildrenMove.Scoring) || (ScoreChildMove(stats, tag, child, going)))
                                    {
                                        bGoing = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    stats.IncStat(child.FullName, Common.DebugLevel.Logging);

                    if ((!bMatch) && (manager.Households.AllowSoloMove(child)))
                    {
                        stats.IncStat(tag + " Break: Teen Stay");

                        staying.Add(child);
                    }
                    else if (bGoing)
                    {
                        if (children == ChildrenMove.Go)
                        {
                            stats.IncStat(tag + " Break: Child Go");
                        }
                        else
                        {
                            stats.IncStat(tag + " Break: Child Scoring Go");
                        }

                        going.Add(child);
                    }
                    else
                    {
                        if (children == ChildrenMove.Stay)
                        {
                            stats.IncStat(tag + " Break: Child Stay");
                        }
                        else if (bMatch)
                        {
                            stats.IncStat(tag + " Break: Child Scoring Stay");
                        }

                        extraChildrenPets.Add(child);
                    }
                }

                bool foundAdult = false, foundBlood = false;
                foreach (SimDescription sim in staying)
                {
                    if (manager.Deaths.IsDying(sim)) continue;

                    if (!manager.Households.AllowGuardian(sim)) continue;

                    if (ancestral)
                    {
                        if (Relationships.IsCloselyRelated(head, sim, false))
                        {
                            foundBlood = true;
                        }
                    }

                    foundAdult = true;
                }

                if ((!foundAdult) || ((ancestral) && (!foundBlood)))
                {
                    stats.AddStat(tag + " Break: Extra", extraChildrenPets.Count);

                    going.AddRange(extraChildrenPets);

                    if (ancestral)
                    {
                        stats.IncStat(tag + " Break: Ancestral");

                        mStaying.AddRange(going);
                        return;
                    }
                }

                mStaying.AddRange(staying);

                foreach (SimDescription sim in mStaying)
                {
                    going.Remove(sim);
                }

                mGoing.AddRange(going);
            }
        }
    }
}

