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
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Demographics
{
    public class Population : DemographicOption
    {
        public override string GetTitlePrefix()
        {
            return "Population";
        }

        protected string GetDetails(List<IMiniSimDescription> sims)
        {
            int npcs = 0, homeless = 0, tourists = 0, residents = 0;

            Dictionary<CASAgeGenderFlags, GenderItem> ages = new Dictionary<CASAgeGenderFlags, GenderItem>();

            Dictionary<OccultTypes, GenderItem> occults = new Dictionary<OccultTypes, GenderItem>();

            GenderItem childless = new GenderItem();
            GenderItem married = new GenderItem();

            GenderItem fertile = new GenderItem();

            int malePregnancies = 0, femalePregnancies = 0, unknownPregnancies = 0;

            int totalParents = 0;
            float totalChildren = 0;

            bool includesHuman = false;

            Dictionary<Household, bool> houses = new Dictionary<Household, bool>();

            Dictionary<CASAgeGenderFlags, bool> species = new Dictionary<CASAgeGenderFlags, bool>();

            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription member = miniSim as SimDescription;
                if (member == null) continue;

                Household household = member.Household;
                if (household == null) continue;

                if (!household.IsSpecialHousehold)
                {
                    houses[household] = true;
                }

                species[member.Species] = true;

                if (member.IsHuman)
                {
                    includesHuman = true;
                }

                if (member.OccultManager != null)
                {
                    foreach (OccultTypes type in Enum.GetValues(typeof(OccultTypes)))
                    {
                        if (member.OccultManager.HasOccultType(type))
                        {
                            GenderItem item;
                            if (!occults.TryGetValue(type, out item))
                            {
                                item = new GenderItem();
                                occults.Add(type, item);
                            }

                            item.Inc(member.IsFemale);
                        }
                    }
                }

                if (SimTypes.IsService(household))
                {
                    npcs++;
                }
                else if (household.IsTouristHousehold)
                {
                    tourists++;
                }
                else if (household.LotHome == null)
                {
                    homeless++;
                }
                else
                {
                    residents++;

                    if (member.IsPregnant)
                    {
                        if (member.Pregnancy.mGender == CASAgeGenderFlags.Male)
                        {
                            malePregnancies++;
                        }
                        else if (member.Pregnancy.mGender == CASAgeGenderFlags.Female)
                        {
                            femalePregnancies++;
                        }
                        else
                        {
                            unknownPregnancies++;
                        }

                        residents++;
                    }

                    int numChildren = NRaas.CommonSpace.Helpers.Relationships.GetChildren(member).Count;
                    if (numChildren > 0)
                    {
                        totalChildren += numChildren;
                        totalParents++;
                    }
                    else
                    {
                        if ((member.YoungAdult) || (member.Adult))
                        {
                            childless.Inc(member.IsFemale);
                        }
                    }

                    if ((member.Partner != null) && (member.Partner.CreatedSim != null))
                    {
                        married.Inc(member.IsFemale);
                    }

                    GenderItem age;
                    if (!ages.TryGetValue(member.Age, out age))
                    {
                        age = new GenderItem();
                        ages.Add(member.Age, age);
                    }

                    age.Inc(member.IsFemale);

                    if ((member.YoungAdult) || (member.Adult))
                    {
                        fertile.Inc(member.IsFemale);
                    }

                    if (member.Pregnancy != null)
                    {
                        totalChildren++;
                    }
                }
            }

            long males = malePregnancies;
            long females = femalePregnancies;

            float adults = 0;
            float children = 0;

            foreach (KeyValuePair<CASAgeGenderFlags, GenderItem> item in ages)
            {
                males += item.Value.mMale;
                females += item.Value.mFemale;

                if ((item.Key == CASAgeGenderFlags.YoungAdult) || (item.Key == CASAgeGenderFlags.Adult))
                {
                    adults += item.Value.mMale + item.Value.mFemale;
                }
                else if (item.Key != CASAgeGenderFlags.Elder)
                {
                    children += item.Value.mMale + item.Value.mFemale;
                }
            }

            string msg = null;

            List<object> objects = new List<object>();

            objects.Add(houses.Count);
            objects.Add(residents);
            objects.Add(npcs);
            objects.Add(homeless);
            objects.Add(tourists);
            objects.Add(males);
            objects.Add(females);

            msg += Common.Localize("Population:General", false, objects.ToArray());

            msg += Common.Localize("Population:Pregnancies", false, new object[] { malePregnancies, femalePregnancies, unknownPregnancies });

            objects.Clear();

            CASAgeGenderFlags[] ageFlags = null;

            if (includesHuman)
            {
                ageFlags = new CASAgeGenderFlags[] { CASAgeGenderFlags.Baby, CASAgeGenderFlags.Toddler, CASAgeGenderFlags.Child, CASAgeGenderFlags.Teen, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Adult, CASAgeGenderFlags.Elder };
            }
            else
            {
                ageFlags = new CASAgeGenderFlags[] { CASAgeGenderFlags.Child, CASAgeGenderFlags.Adult, CASAgeGenderFlags.Elder };
            }

            foreach (CASAgeGenderFlags age in ageFlags)
            {
                GenderItem item;
                if (ages.TryGetValue(age, out item))
                {
                    objects.Add(item.mMale);
                    objects.Add(item.mFemale);
                }
                else
                {
                    objects.Add(0);
                    objects.Add(0);
                }
            }

            if (includesHuman)
            {
                msg += Common.Localize("Population:AgeBreakdown", false, objects.ToArray());
            }
            else
            {
                msg += Common.Localize("Population:PetAgeBreakdown", false, objects.ToArray());
            }

            objects.Clear();
            objects.Add(married.mMale);
            objects.Add(married.mFemale);
            objects.Add(childless.mMale);
            objects.Add(childless.mFemale);

            msg += Common.Localize("Population:FamilyBreakdown", false, objects.ToArray());

            if (species.Count == 1)
            {
                if (totalParents > 0)
                {
                    msg += Common.Localize("Population:ChildToParent", false, new object[] { (totalChildren / totalParents) });
                }
            }

            string occultText = null;

            foreach (KeyValuePair<OccultTypes, GenderItem> occult in occults)
            {
                occultText += Common.Localize("Population:Occult", false, new object[] { OccultTypeHelper.GetLocalizedName(occult.Key), occult.Value.mMale, occult.Value.mFemale });
            }

            if (!string.IsNullOrEmpty(occultText))
            {
                msg += Common.NewLine + occultText;
            }

            if (species.Count == 1)
            {
                if (children > 0f)
                {
                    msg += Common.Localize("Population:AdultToChild", false, new object[] { adults, children, (adults / children) });
                }

                if (includesHuman)
                {
                    float adultLength = AgingManager.GetAgingStageLength(CASAgeGenderFlags.Human, CASAgeGenderFlags.YoungAdult);
                    adultLength += AgingManager.GetAgingStageLength(CASAgeGenderFlags.Human, CASAgeGenderFlags.Adult);

                    float childLength = AgingManager.GetAgingStageLength(CASAgeGenderFlags.Human, CASAgeGenderFlags.Baby);
                    childLength += AgingManager.GetAgingStageLength(CASAgeGenderFlags.Human, CASAgeGenderFlags.Toddler);
                    childLength += AgingManager.GetAgingStageLength(CASAgeGenderFlags.Human, CASAgeGenderFlags.Child);
                    childLength += AgingManager.GetAgingStageLength(CASAgeGenderFlags.Human, CASAgeGenderFlags.Teen);

                    if (childLength > 0f)
                    {
                        msg += Common.Localize("Population:PopGrowth", false, new object[] { adultLength, childLength, (adultLength / childLength) });
                    }
                }
            }

            return msg;
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            Sims3.UI.SimpleMessageDialog.Show(Common.Localize("Population:Header"), GetDetails(sims));
            return OptionResult.SuccessRetain;
        }

        public class GenderItem
        {
            public int mMale;
            public int mFemale;

            public void Inc(bool isFemale)
            {
                if (isFemale)
                {
                    mFemale++;
                }
                else
                {
                    mMale++;
                }
            }
        }
    }
}
