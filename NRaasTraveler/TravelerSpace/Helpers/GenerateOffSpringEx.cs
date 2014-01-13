using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.Helpers
{
    public class GenerateOffspringEx
    {
        private ulong mDad;
        private ulong mHouseholdId;
        private ulong mMom;
        private SimUtils.SimCreationSpec mSim;
        private int numDyingSims;

        public GenerateOffspringEx(SimDescription forcedParent, List<SimDescription> dyingSims, float daysGone)
        {
            CASAgeGenderFlags daysGoneAges = GetDaysGoneAges(daysGone);
            
            bool university = false;
            if (GameUtils.IsUniversityWorld())
            {
                university = true;
                if ((daysGoneAges & CASAgeGenderFlags.Teen) == CASAgeGenderFlags.None)
                {
                    return;
                }
            }

            SimDescription parent = null;
            SimDescription partner = null;
            if (forcedParent.IsMale)
            {
                partner = forcedParent;
            }
            else
            {
                parent = forcedParent;
            }

            Household household = forcedParent.Household;
            if (household != null)
            {
                mHouseholdId = household.HouseholdId;
                mSim = new SimUtils.SimCreationSpec();
                if ((parent == null) && (partner != null))
                {
                    if ((partner.Partner != null) && (partner.Gender != partner.Partner.Gender))
                    {
                        parent = partner.Partner;
                    }
                    else if (partner.Partner == null)
                    {
                        parent = FindPartner(partner, dyingSims);
                    }
                }
                if ((partner == null) && (parent != null))
                {
                    if ((parent.Partner != null) && (parent.Gender != parent.Partner.Gender))
                    {
                        partner = parent.Partner;
                    }
                    else if (parent.Partner == null)
                    {
                        partner = FindPartner(parent, dyingSims);
                    }
                }
                if (((parent != null) && !parent.IsMummy) && ((partner != null) && !partner.IsMummy))
                {
                    numDyingSims = 0;
                    int numPetsRemoved = 0;
                    bool flag = false;
                    foreach (SimDescription description3 in household.AllSimDescriptions)
                    {
                        if (dyingSims.Contains(description3))
                        {
                            if (description3.IsHuman)
                            {
                                numDyingSims++;
                            }
                            else
                            {
                                numPetsRemoved++;
                            }
                        }
                        else if (description3.TeenOrAbove)
                        {
                            flag = true;
                        }
                    }

                    if (household.CanSimBeAddedIfXSimAndYPetsRemoved(numDyingSims, numPetsRemoved))
                    {
                        if (university)
                        {
                            mSim.Age = CASAgeGenderFlags.YoungAdult;
                            mSim.Age |= daysGoneAges;
                            mSim.Age &= ~CASAgeGenderFlags.Child;
                        }
                        else
                        {
                            if (!flag)
                            {
                                mSim.Age = CASAgeGenderFlags.Teen;
                            }
                            else
                            {
                                mSim.Age = CASAgeGenderFlags.Child;
                            }

                            mSim.Age |= daysGoneAges;
                        }

                        mSim.Normalize();

                        if (parent != null)
                        {
                            mMom = parent.SimDescriptionId;
                        }
                        if (partner != null)
                        {
                            mDad = partner.SimDescriptionId;
                        }
                    }
                }
            }
        }

        public static CASAgeGenderFlags GetDaysGoneAges(float daysGone)
        {
            float childAge = AgingManager.GetAgingStageLength(CASAgeGenderFlags.Human, CASAgeGenderFlags.Child);
            float teenAge = childAge + AgingManager.GetAgingStageLength(CASAgeGenderFlags.Human, CASAgeGenderFlags.Teen);
            float youngAge = teenAge + AgingManager.GetAgingStageLength(CASAgeGenderFlags.Human, CASAgeGenderFlags.YoungAdult);
            float adultAge = youngAge + AgingManager.GetAgingStageLength(CASAgeGenderFlags.Human, CASAgeGenderFlags.Adult);

            CASAgeGenderFlags age = CASAgeGenderFlags.None;

            if (daysGone > childAge)
            {
                age |= CASAgeGenderFlags.Teen;
            }
            if (daysGone > teenAge)
            {
                age |= CASAgeGenderFlags.YoungAdult;
            }
            if (daysGone > youngAge)
            {
                age |= CASAgeGenderFlags.Adult;
            }
            if (daysGone > adultAge)
            {
                age |= CASAgeGenderFlags.Elder;
            }

            return age;
        }

        public void Execute()
        {
            Household household = Household.Find(mHouseholdId);
            SimDescription mom = SimDescription.Find(mMom);
            SimDescription dad = SimDescription.Find(mDad);
            SimDescription simDescription = mSim.Instantiate(dad, mom);
            if (simDescription != null)
            {
                household.Add(simDescription);
                simDescription.SendSimHome();
                if (((mom != null) && (dad != null)) && (mom.Partner != dad))
                {
                    SimUtils.HouseholdCreationSpec.InitializeRomance(mom, dad, simDescription, simDescription.LastName);
                }
            }
        }

        private SimDescription FindPartner(SimDescription parent, List<SimDescription> dyingSims)
        {
            SimDescription description = null;
            float liking = -100f;

            List<SimDescription> list = new List<SimDescription>();

            foreach (Relationship relationship in Relationship.GetRelationships(parent))
            {
                SimDescription otherSimDescription = relationship.GetOtherSimDescription(parent);
                if (otherSimDescription.Household == null) continue;

                if (otherSimDescription.Household.IsPreviousTravelerHousehold) continue;

                if (otherSimDescription.Household.IsTravelHousehold) continue;

                if (((parent.Gender != otherSimDescription.Gender) && (otherSimDescription.Partner == null)) && !otherSimDescription.IsMummy)
                {
                    if (relationship.AreRomantic())
                    {
                        list.Add(otherSimDescription);
                    }
                    else if ((!Relationships.IsCloselyRelated(parent, otherSimDescription, false) && parent.CheckAutonomousGenderPreference(otherSimDescription)) && (otherSimDescription.CheckAutonomousGenderPreference(parent) && (relationship.LTR.Liking > liking)))
                    {
                        liking = relationship.LTR.Liking;
                        description = otherSimDescription;
                    }
                }
            }

            if (list.Count > 0x0)
            {
                return list[RandomUtil.GetInt(list.Count - 0x1)];
            }

            foreach (SimDescription description3 in dyingSims)
            {
                if ((((parent.Gender != description3.Gender) && (description3.Partner == null)) && (!description3.IsMummy && !Relationships.IsCloselyRelated(parent, description3, false))) && (parent.CheckAutonomousGenderPreference(description3) && description3.CheckAutonomousGenderPreference(parent)))
                {
                    list.Add(description3);
                }
            }

            if (list.Count > 0x0)
            {
                return list[RandomUtil.GetInt(list.Count - 0x1)];
            }

            return description;
        }

        public bool IsValid()
        {
            SimDescription description = SimDescription.Find(mMom);
            SimDescription description2 = SimDescription.Find(mDad);
            return (((mHouseholdId != 0x0L) && (mSim != null)) && ((description != null) && (description2 != null)));
        }

        public static void PossiblyGenerateOffspring(List<SimDescription> dyingSims, float daysGone)
        {
            foreach (SimDescription description in dyingSims)
            {
                try
                {
                    if (RandomUtil.RandomChance01(GenerateOffspring.kPercentChanceOffspring))
                    {
                        int count = RandomUtil.GetInt(GenerateOffspring.kMinimumOffspring, GenerateOffspring.kMaximumOffspring);
                        for (int i = 0x0; i < count; i++)
                        {
                            GenerateOffspringEx offspring = new GenerateOffspringEx(description, dyingSims, daysGone);
                            if (offspring.IsValid())
                            {
                                offspring.Execute();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(description, e);
                }
            }
        }
    }
}
