using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.TravelerSpace.Helpers
{
    public class FutureDescendantHouseholdInfoEx
    {
        public static int CalculateHouseholdFamilyScore(FutureDescendantService.FutureDescendantHouseholdInfo ths, Common.StringBuilder results)
        {
            results.Append(Common.NewLine + "CalculateHouseholdFamilyScore");

            int householdSizeTraitScore = 0x0;
            foreach (ulong num2 in ths.mProgenitorSimIds)
            {
                IMiniSimDescription iMiniSimDescription = SimDescription.GetIMiniSimDescription(num2);
                if (iMiniSimDescription == null) continue;

                results.Append(Common.NewLine + " Progenitor: " + iMiniSimDescription.FullName);

                MiniSimDescription miniSim = iMiniSimDescription as MiniSimDescription;
                SimDescription fullSim = iMiniSimDescription as SimDescription;
                if (fullSim != null)
                {
                    foreach (Trait trait in fullSim.TraitManager.List)
                    {
                        results.Append(Common.NewLine + "  " + trait.TraitName(fullSim.IsFemale) + " : " + trait.DescendantFamilySizeModifier);

                        householdSizeTraitScore += trait.DescendantFamilySizeModifier;
                    }
                }
                else if (miniSim != null)
                {
                    foreach (TraitNames names in miniSim.Traits)
                    {
                        Trait traitFromDictionary = TraitManager.GetTraitFromDictionary(names);

                        results.Append(Common.NewLine + "  " + traitFromDictionary.TraitName(miniSim.IsFemale) + " : " + traitFromDictionary.DescendantFamilySizeModifier);

                        householdSizeTraitScore += traitFromDictionary.DescendantFamilySizeModifier;
                    }
                }
            }

            results.Append(Common.NewLine + " Score : " + householdSizeTraitScore);

            int result = FutureDescendantService.CalculateDesiredHouseholdSize(householdSizeTraitScore);

            results.Append(Common.NewLine + " Result : " + result);

            return result;
        }

        public static bool CreateAndAddDescendant(FutureDescendantService.FutureDescendantHouseholdInfo ths)
        {
            Household descendantHousehold = ths.DescendantHousehold;
            if (descendantHousehold == null) return false;

            List<SimDescription> potentialParents = new List<SimDescription>();
            foreach (SimDescription description in descendantHousehold.SimDescriptions)
            {
                if (description.YoungAdultOrAbove)
                {
                    potentialParents.Add(description);
                }
            }

            bool noParents;
            SimDescription child = GenerateOffspring(potentialParents, out noParents);
            if (child == null)
            {
                if (noParents)
                {
                    if (Instantiate(ths) == null) return false;
                }
            }
            else
            {
                descendantHousehold.Add(child);
                ths.mHouseholdMembers.Add(child.SimDescriptionId);

                SimUtils.HouseholdCreationSpec.UpdateHouseholdLTR(ref descendantHousehold, descendantHousehold.Name, false);
            }

            return true;
        }

        protected static SimDescription CreateProgenitor(ulong id, out bool unpacked)
        {
            unpacked = false;

            if (id == 0) return null;

            SimDescription sim = SimDescription.Find(id);

            if (sim == null)
            {
                MiniSimDescription msd = MiniSimDescription.Find(id);
                if (msd == null) return null;

                // Custom
                sim = MiniSims.UnpackSimAndUpdateRel(msd);
                if (sim == null) return null;

                Household.CreateTouristHousehold();
                Household.TouristHousehold.AddTemporary(sim);
                msd.Instantiated = true;

                if (sim.AgingState != null)
                {
                    sim.AgingState.MergeTravelInformation(msd);
                }

                SpeedTrap.Sleep();
                unpacked = true;
            }
                
            sim.Fixup();

            return sim;
        }

        protected static void WeightAndFitness(SimDescription mom, SimDescription dad, out float weight, out float fitness)
        {
            float minWeight = mom.Weight;
            float maxWeight = dad.Weight;

            if (minWeight > maxWeight)
            {
                float temp = minWeight;
                minWeight = maxWeight;
                maxWeight = temp;
            }

            float minFitness = mom.Fitness;
            float maxFitness = dad.Fitness;

            if (minFitness > maxFitness)
            {
                float temp = minFitness;
                minFitness = maxFitness;
                maxFitness = temp;
            }

            weight = RandomUtil.GetFloat(minWeight, maxWeight);
            fitness = RandomUtil.GetFloat(minFitness, maxFitness);
        }

        protected static SimDescription CreateDescendant(ulong momID, ulong dadID, Household household, CASAgeGenderFlags age, CASAgeGenderFlags gender)
        {
            bool momUnpacked;
            SimDescription mom = CreateProgenitor(momID, out momUnpacked);
            if (mom == null) return null;

            bool dadUnpacked;
            SimDescription dad = CreateProgenitor(dadID, out dadUnpacked);
            if (dad == null) return null;

            float weight, fitness;
            WeightAndFitness(mom, dad, out weight, out fitness);

            SimUtils.SimCreationSpec spec = new SimUtils.SimCreationSpec();
            spec.Weight = weight;
            spec.Fitness = fitness;
            spec.Age = age;
            spec.Gender = gender;
            spec.Normalize();
            SimDescription sim = spec.Instantiate(mom, dad, false, WorldName.FutureWorld);

            if (dadUnpacked && (dad != null))
            {
                dad.PackUpToMiniSimDescription();
            }

            if (momUnpacked && (mom != null))
            {
                mom.PackUpToMiniSimDescription();
            }

            sim.TraitManager.AddHiddenElement(TraitNames.DescendantHiddenTrait);
            household.Add(sim);

            return sim;
        }

        public static Household Instantiate(FutureDescendantService.FutureDescendantHouseholdInfo ths)
        {
            ulong mom1ID = ths.mProgenitorSimIds[0];
            ulong dad1ID = 0;

            ulong mom2ID = 0;
            ulong dad2ID = 0;

            if (ths.mProgenitorSimIds.Count >= 2)
            {
                dad1ID = ths.mProgenitorSimIds[1];

                if (ths.mProgenitorSimIds.Count >= 3)
                {
                    mom2ID = ths.mProgenitorSimIds[2];

                    if (ths.mProgenitorSimIds.Count >= 4)
                    {
                        dad2ID = ths.mProgenitorSimIds[3];
                    }
                }
            }

            Household household = ths.DescendantHousehold;

            int currentDesiredHouseholdSize = ths.mCurrentDesiredHouseholdSize;

            if (household != null)
            {
                currentDesiredHouseholdSize -= household.NumMembers;
            }

            if (currentDesiredHouseholdSize <= 0x0) return null;

            if (household == null)
            {
                household = Household.Create();
            }

            List<SimDescription> potentialParents = new List<SimDescription>();
            string familyName = null;

            CASAgeGenderFlags age = CASAgeGenderFlags.YoungAdult | CASAgeGenderFlags.Adult;
            if (RandomUtil.RandomChance(FutureDescendantService.kPercentChanceOfElderlyHousehold))
            {
                age = CASAgeGenderFlags.Elder;
            }

            List<SimDescription> dispose = new List<SimDescription>();

            if (currentDesiredHouseholdSize > 0x0)
            {
                SimDescription mom = CreateDescendant(mom1ID, dad1ID, household, age, CASAgeGenderFlags.Female | CASAgeGenderFlags.Male);
                if (mom == null) return null;

                potentialParents.Add(mom);
                ths.mHouseholdMembers.Add(mom.SimDescriptionId);
                currentDesiredHouseholdSize--;
                familyName = mom.LastName;
            }

            if (currentDesiredHouseholdSize > 0x0)
            {
                bool disposeSpouse = false;

                Household spouseHouse = household;
                if (RandomUtil.RandomChance(FutureDescendantService.kPercentChanceOfSingleParentHousehold))
                {
                    spouseHouse = Household.TouristHousehold;
                    disposeSpouse = true;
                }

                CASAgeGenderFlags gender = potentialParents[0].Gender;
                if (!RandomUtil.RandomChance(FutureDescendantService.kPercentChanceOfSameSexHousehold))
                {
                    gender = (potentialParents[0].Gender == CASAgeGenderFlags.Male) ? CASAgeGenderFlags.Female : CASAgeGenderFlags.Male;
                }

                SimDescription dad = CreateDescendant(mom1ID, dad1ID, spouseHouse, potentialParents[0].Age, gender);
                if (dad == null) return null;

                if (disposeSpouse)
                {
                    dispose.Add(dad);
                }
                else
                {
                    ths.mHouseholdMembers.Add(dad.SimDescriptionId);
                    currentDesiredHouseholdSize--;
                }

                potentialParents.Add(dad);
            }

            while (currentDesiredHouseholdSize > 0x0)
            {
                bool noParents;
                SimDescription child = GenerateOffspring(potentialParents, out noParents);
                if (child != null)
                {
                    household.Add(child);
                    ths.mHouseholdMembers.Add(child.SimDescriptionId);
                }

                currentDesiredHouseholdSize--;
            }

            SimUtils.HouseholdCreationSpec.UpdateHouseholdLTR(ref household, familyName, false);
            ths.mHouseholdName = FutureDescendantService.LocalizeString(false, "DescendantHouseholdId", new object[] { familyName });
            household.Name = ths.mHouseholdName;
            ths.mHouseholdId = household.HouseholdId;

            foreach (SimDescription sim in dispose)
            {
                sim.Dispose(true, true, true);
            }

            return household;
        }

        public static SimDescription GenerateOffspring(List<SimDescription> potentialParents, out bool noParents)
        {
            SimDescription choiceMom;
            SimDescription choiceDad;
            SimUtils.SimCreationSpec.ChooseParents(potentialParents, out choiceDad, out choiceMom);

            noParents = ((choiceMom == null) || (choiceDad == null) || (choiceMom == choiceDad));

            if (noParents)
            {
                return null;
            }

            CASAgeGenderFlags age = CASAgeGenderFlags.Teen | CASAgeGenderFlags.Child;
            if ((choiceMom.AdultOrAbove) && (choiceDad.AdultOrAbove))
            {
                age |= CASAgeGenderFlags.YoungAdult;
            }

            float weight, fitness;
            WeightAndFitness(choiceMom, choiceDad, out weight, out fitness);

            SimUtils.SimCreationSpec spec3 = new SimUtils.SimCreationSpec();
            spec3.Age = age;

            spec3.Weight = weight;
            spec3.Fitness = fitness;

            spec3.Normalize();

            SimDescription child = spec3.Instantiate(choiceDad, choiceMom, true);
            if (child != null)
            {
                child.TraitManager.AddHiddenElement(TraitNames.DescendantHiddenTrait);
                if ((choiceMom != null) && (choiceDad != null))
                {
                    SimUtils.HouseholdCreationSpec.InitializeRomance(choiceMom, choiceDad, child, choiceMom.LastName);
                }
                else if (choiceMom != null)
                {
                    child.LastName = choiceMom.LastName;
                }
                else if (choiceDad != null)
                {
                    child.LastName = choiceDad.LastName;
                }
            }

            return child;
        }
    }
}