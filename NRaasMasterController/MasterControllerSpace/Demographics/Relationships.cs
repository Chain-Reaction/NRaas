using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Demographics
{
    public class Relationships : DemographicOption
    {
        public override string GetTitlePrefix()
        {
            return "Relationships";
        }

        protected string GetDetails(List<IMiniSimDescription> sims)
        {
            GenderBin teenSingles = new GenderBin();
            GenderBin residentSingles = new GenderBin();
            GenderBin serviceSingles = new GenderBin();
            GenderBin adultMarried = new GenderBin();
            GenderBin elderMarried = new GenderBin();
            GenderBin steadyTeens = new GenderBin();
            GenderBin steadyAdults = new GenderBin();
            GenderBin singlePregnancy = new GenderBin();
            GenderBin partnerPregnancy = new GenderBin();
            GenderBin friends = new GenderBin();
            GenderBin flirts = new GenderBin();
            GenderBin dislikes = new GenderBin();
            GenderBin enemies = new GenderBin();
            GenderBin singleParents = new GenderBin();
            GenderBin oneParent = new GenderBin();
            GenderBin strandedCouples = new GenderBin();

            GenderBin noFriends = new GenderBin();
            GenderBin noFlirts = new GenderBin();
            GenderBin noEnemies = new GenderBin();
            GenderBin noDislikes = new GenderBin();

            GenderCount flirtCount = new GenderCount();
            GenderCount dislikeCount = new GenderCount();
            GenderCount enemyCount = new GenderCount();
            GenderCount friendCount = new GenderCount();

            bool includesHuman = false;

            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription member = miniSim as SimDescription;
                if (member == null) continue;

                if (member.IsHuman)
                {
                    includesHuman = true;
                }

                if (NRaas.CommonSpace.Helpers.Relationships.GetParents(member).Count == 1)
                {
                    oneParent.Add(member);
                }

                if (member.Partner == null)
                {
                    if (member.Teen)
                    {
                        teenSingles.Add(member);
                    }
                    else if (member.YoungAdultOrAbove)
                    {
                        if ((member.Household == null) || (SimTypes.IsSpecial(member)))
                        {
                            serviceSingles.Add(member);
                        }
                        else
                        {
                            residentSingles.Add(member);
                        }
                    }

                    foreach (SimDescription child in NRaas.CommonSpace.Helpers.Relationships.GetChildren(member))
                    {
                        if (child.TeenOrBelow)
                        {
                            singleParents.Add(member);
                        }
                    }
                }
                else if (member.IsMarried)
                {
                    if (member.Elder)
                    {
                        elderMarried.Add(member);
                    }
                    else
                    {
                        adultMarried.Add(member);
                    }

                    if (member.LotHome != member.Partner.LotHome)
                    {
                        if (((!member.IsDead) || (member.IsPlayableGhost)) &&
                            ((!member.Partner.IsDead) || (member.Partner.IsPlayableGhost)))
                        {
                            strandedCouples.Add(member);
                        }
                    }
                }
                else if (member.YoungAdultOrAbove)
                {
                    steadyAdults.Add(member);
                }
                else
                {
                    steadyTeens.Add(member);
                }

                if (member.IsPregnant)
                {
                    if (member.Partner == null)
                    {
                        singlePregnancy.Add(member);
                    }
                    else
                    {
                        partnerPregnancy.Add(member);
                    }
                }

                bool bFlirt = false, bFriend = false, bEnemy = false, bDislike = false;

                List<Relationship> relations = new List<Relationship>(Relationship.GetRelationships(member));
                if (relations != null)
                {
                    foreach (Relationship relation in relations)
                    {
                        if (member.TeenOrAbove)
                        {
                            if (relation.AreRomantic())
                            {
                                flirts.Add(member);

                                flirtCount.Add(member);

                                bFlirt = true;
                            }
                        }

                        if (member.ChildOrAbove)
                        {
                            if (relation.AreFriends())
                            {
                                friends.Add(member);

                                friendCount.Add(member);

                                bFriend = true;
                            }
                            else if ((relation.LTR.Liking < -75) || (relation.AreEnemies()))
                            {
                                enemies.Add(member);

                                enemyCount.Add(member);

                                bEnemy = true;
                            }
                            else if (relation.LTR.Liking < 0)
                            {
                                dislikes.Add(member);

                                dislikeCount.Add(member);

                                bDislike = true;
                            }
                        }
                    }
                }

                if (member.TeenOrAbove)
                {
                    if (!bFlirt)
                    {
                        noFlirts.Add(member);
                    }
                }
                if (member.ChildOrAbove)
                {
                    if (!bFriend)
                    {
                        noFriends.Add(member);
                    }
                    if (!bEnemy)
                    {
                        noEnemies.Add(member);
                    }
                    if (!bDislike)
                    {
                        noDislikes.Add(member);
                    }
                }
            }

            string msg = Common.Localize("Relationships:BodyHeader");

            List<object> objects = new List<object>();

            if (includesHuman)
            {
                objects.Add(teenSingles.ToString());

                msg += Common.Localize("Relationships:TeenSingle", false, objects.ToArray());
            }

            objects.Clear();
            objects.Add(residentSingles.ToString());
            objects.Add(serviceSingles.ToString());

            msg += Common.Localize("Relationships:Single", false, objects.ToArray());

            objects.Clear();
            objects.Add(adultMarried.ToString());
            objects.Add(elderMarried.ToString());

            msg += Common.Localize("Relationships:Married", false, objects.ToArray());

            if (includesHuman)
            {
                objects.Clear();
                objects.Add(steadyTeens.ToString());
                objects.Add(steadyAdults.ToString());

                msg += Common.Localize("Relationships:Steady", false, objects.ToArray());
            }

            objects.Clear();
            objects.Add(singlePregnancy.ToString());
            objects.Add(partnerPregnancy.ToString());

            msg += Common.Localize("Relationships:Pregnant", false, objects.ToArray());

            if (includesHuman)
            {
                objects.Clear();
                objects.Add(strandedCouples.ToString());
                objects.Add(singleParents.ToString());
                objects.Add(oneParent.ToString());

                msg += Common.Localize("Relationships:Family", false, objects.ToArray());
            }

            objects.Clear();
            objects.Add(friends.ToString());
            objects.Add(noFriends.ToString());
            objects.Add(friendCount.ToString(friends));

            msg += Common.Localize("Relationships:Friends", false, objects.ToArray());

            objects.Clear();
            objects.Add(dislikes.ToString());
            objects.Add(noDislikes.ToString());
            objects.Add(dislikeCount.ToString(friends));

            msg += Common.Localize("Relationships:Dislikes", false, objects.ToArray());

            objects.Clear();
            objects.Add(flirts.ToString());
            objects.Add(noFlirts.ToString());
            objects.Add(flirtCount.ToString(flirts));

            msg += Common.Localize("Relationships:Flirts", false, objects.ToArray());

            objects.Clear();
            objects.Add(enemies.ToString());
            objects.Add(noEnemies.ToString());
            objects.Add(enemyCount.ToString(enemies));

            msg += Common.Localize("Relationships:Enemies", false, objects.ToArray());

            return msg;
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            SimpleMessageDialog.Show(Common.Localize ("Relationships:Header"), GetDetails(sims));
            return OptionResult.SuccessRetain;
        }

        protected class GenderBin
        {
            public Dictionary<SimDescription, bool> mMales = new Dictionary<SimDescription, bool>();
            public Dictionary<SimDescription, bool> mFemales = new Dictionary<SimDescription, bool>();

            public GenderBin()
            { }

            public void Add(SimDescription sim)
            {
                if (sim.IsMale)
                {
                    if (!mMales.ContainsKey(sim))
                    {
                        mMales.Add(sim, true);
                    }
                }
                else
                {
                    if (!mFemales.ContainsKey(sim))
                    {
                        mFemales.Add(sim, true);
                    }
                }
            }

            public override string ToString()
            {
                return mMales.Count.ToString() + ", " + mFemales.Count.ToString();
            }
        }

        protected class GenderCount
        {
            public int mMales = 0;
            public int mFemales = 0;

            public GenderCount()
            { }

            public void Add(SimDescription sim)
            {
                if (sim.IsMale)
                {
                    mMales++;
                }
                else
                {
                    mFemales++;
                }
            }

            public override string ToString()
            {
                return mMales.ToString() + ", " + mFemales.ToString();
            }


            public string ToString(GenderBin bin)
            {
                string msg = null;
                if (bin.mMales.Count == 0)
                {
                    msg += "0, ";
                }
                else
                {
                    msg += (mMales / bin.mMales.Count).ToString() + ", ";
                }
                if (bin.mFemales.Count == 0)
                {
                    msg += "0";
                }
                else
                {
                    msg += (mFemales / bin.mFemales.Count).ToString();
                }
                return msg;
            }
        }
    }
}
