using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class MiniSims
    {
        public enum Results
        {
            Success,
            Failure,
            Unnecessary,
        }

        public static void UpdateRelationsThumbnails(Sim sim)
        {
            MiniSimDescription active = null;

            if ((sim != null) && (sim.SimDescription != null))
            {
                active = Find(sim.SimDescription.SimDescriptionId);
            }

            if (active != null)
            {
                foreach (MiniRelationship relation in new List<MiniRelationship>(active.MiniRelationships))
                {
                    UpdateThumbnailKey(Find(relation.GetOtherSimDescriptionId(active)));
                }
            }
        }

        public static ThumbnailKey GetThumbnailKey(IMiniSimDescription ths, ThumbnailSize size, int thumbIndex)
        {
            try
            {
                if (ths == null) return ThumbnailKey.kInvalidThumbnailKey;

                MiniSimDescription miniSim = ths as MiniSimDescription;
                if (miniSim != null)
                {
                    if (!ThumbnailManager.KeyExistsInDB(miniSim.mThumbKey))
                    {
                        SimDescription sim = MiniSims.UnpackSim(miniSim);

                        ThumbnailKey thumbnailKey = sim.GetThumbnailKey(ThumbnailSize.Large, 0x0);

                        try
                        {
                            sim.Dispose(false, true);
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim, e);
                        }

                        return thumbnailKey;
                    }

                    return miniSim.mThumbKey;
                }
                else if ((ths.CASGenealogy == null) || (ths.CASGenealogy.IsAlive()))
                {
                    return ths.GetThumbnailKey(size, thumbIndex);
                }
                else
                {
                    return ths.GetDeceasedThumbnailKey(size, thumbIndex);
                }
            }
            catch (Exception e)
            {
                Common.Exception(ths.FullName, e);
                return ThumbnailKey.kInvalidThumbnailKey;
            }
        }

        private static List<SimDescription> CreateNewbornsBeforePacking(Pregnancy ths, float bonusMoodPoints, bool bAddToFamily, int householdSimMembers, int householdPetMembers)
        {
            MiniSimDescription description = null;
            SimDescription simDescription;
            if ((ths.mDad == null) || ths.mDad.HasBeenDestroyed)
            {
                simDescription = SimDescription.Find(ths.DadDescriptionId);
                if (simDescription == null)
                {
                    description = Find(ths.DadDescriptionId);
                    if (description != null)
                    {
                        simDescription = UnpackSim(description, true);
                        if (simDescription != null)
                        {
                            simDescription.Genealogy.SetSimDescription(simDescription);
                        }
                    }
                }
            }
            else
            {
                simDescription = ths.mDad.SimDescription;
            }

            List<SimDescription> list = new List<SimDescription>();

            if (simDescription != null)
            {
                float averageMoodForBirth = ths.GetAverageMoodForBirth(simDescription, bonusMoodPoints);
                Random pregoRandom = new Random(ths.mRandomGenSeed);
                int num2 = ths.GetNumForBirth(simDescription, pregoRandom, householdSimMembers, householdPetMembers);
                for (int i = 0x0; i < num2; i++)
                {
                    ths.DetermineGenderOfBaby();
                    CASAgeGenderFlags mGender = ths.mGender;
                    ths.mGender = CASAgeGenderFlags.None;
                    SimDescription child = null;

                    if (ths is PetPregnancy)
                    {
                        child = GeneticsPet.MakePetDescendant(simDescription, ths.mMom.SimDescription, CASAgeGenderFlags.Adult, mGender, simDescription.Species, pregoRandom, true, GeneticsPet.SetName.SetNameNonInteractive, i, OccultTypes.None);
                    }
                    else
                    {
                        CASAgeGenderFlags age = CASAgeGenderFlags.Child;
                        if (GameUtils.IsUniversityWorld())
                        {
                            age = CASAgeGenderFlags.YoungAdult;
                        }

                        child = Genetics.MakeDescendant(simDescription, ths.mMom.SimDescription, age, mGender, averageMoodForBirth, pregoRandom, false, true, true, ths.mMom.SimDescription.HomeWorld, false);
                    }

                    child.WasCasCreated = false;
                    if (bAddToFamily)
                    {
                        ths.mMom.Household.Add(child);
                        Sim s = child.Instantiate(ths.mMom.Position);
                        ths.CheckForGhostBaby(s);
                    }

                    list.Add(child);
                }

                if (description != null)
                {
                    simDescription.Dispose(true, true);
                }
            }

            return list;
        }

        private static void CreateBabyBeforePacking(Pregnancy ths)
        {
            try
            {
                Sim mom = ths.Mom;
                if (mom == null) return;

                if ((GameUtils.IsOnVacation()) || (GameUtils.IsUniversityWorld()))
                {
                    Household household = mom.Household;
                    if (household != null)
                    {
                        int simCount = 0x0;
                        int petCount = 0x0;
                        household.GetNumberOfSimsAndPets(true, out simCount, out petCount);

                        foreach (SimDescription description in CreateNewbornsBeforePacking(ths, 0f, true, simCount, petCount))
                        {
                            MiniSimDescription.AddMiniSim(description);
                        }

                        household.InvalidateThumbnail();

                        List<SimDescription> simDescriptions = household.AllSimDescriptions;
                        foreach (SimDescription description2 in simDescriptions)
                        {
                            MiniSimDescription description3 = Find(description2.SimDescriptionId);
                            if (description3 != null)
                            {
                                description3.UpdateHouseholdMembers(simDescriptions);
                            }
                        }

                        Common.Sleep();
                    }
                }
                else
                {
                    MiniSimDescription description4 = Find(mom.SimDescription.SimDescriptionId);
                    if (description4 != null)
                    {
                        int householdSimMembers = 0x1;
                        int householdPetMembers = 0x0;
                        List<ulong> currentMembers = new List<ulong>();
                        if (description4.HouseholdMembers != null)
                        {
                            householdSimMembers = description4.NumSimMemberIncludingPregnant;
                            householdPetMembers = description4.NumPetMemberIncludingPregnant;
                            currentMembers.AddRange(description4.HouseholdMembers);
                        }
                        else
                        {
                            currentMembers.Add(description4.SimDescriptionId);
                        }

                        List<SimDescription> list4 = CreateNewbornsBeforePacking(ths, 0f, false, householdSimMembers, householdPetMembers);
                        while (list4.Count > 0x0)
                        {
                            SimDescription desc = list4[0x0];
                            list4.RemoveAt(0x0);

                            if (desc != null)
                            {
                                MiniSimDescription.AddMiniSim(desc);

                                MiniSimDescription miniSim = Find(desc.SimDescriptionId);
                                if (miniSim != null)
                                {
                                    miniSim.mMotherDescId = description4.SimDescriptionId;
                                }

                                currentMembers.Add(desc.SimDescriptionId);
                                desc.Dispose();
                            }
                        }

                        foreach (ulong num2 in currentMembers)
                        {
                            MiniSimDescription description7 = Find(num2);
                            if (description7 != null)
                            {
                                description7.UpdateHouseholdMembers(currentMembers);
                            }
                        }
                    }
                }

                if (mom.SimDescription != null)
                {
                    mom.SimDescription.ClearPregnancyData();

                    if (!mom.HasBeenDestroyed)
                    {
                        mom.SwitchToOutfitWithoutSpin(OutfitCategories.Everyday);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(ths.mMom, ths.mDad, e);
            }
        }

        public static void FixUpForeignPregnantSims(SimDescription desc)
        {
            //if ((GameUtils.GetWorldType(desc.HomeWorld) == WorldType.Vacation) && (desc.Pregnancy != null))
            if((desc.HomeWorld == WorldName.Egypt || desc.HomeWorld == WorldName.France || desc.HomeWorld == WorldName.China || desc.HomeWorld == WorldName.University) && (desc.Pregnancy != null))
            {
                if (SimTypes.IsTourist(desc)) return;

                if (desc.Pregnancy.Dad != null)
                {
                    MidlifeCrisisManager.OnHadChild(desc.Pregnancy.Dad.SimDescription);
                }

                CreateBabyBeforePacking(desc.Pregnancy);
            }
        }

        public static void PackSim(SimDescription desc)
        {
            MiniSimDescription miniSim;
            if (MiniSimDescription.sMiniSims.TryGetValue(desc.SimDescriptionId, out miniSim))
            {
                FixUpForeignPregnantSims(desc);

                miniSim.SetMemberFields(desc, true);
                GenerateCrossWorldThumbnail(miniSim, desc, true);
                if (desc.Household != null)
                {
                    desc.Household.RemoveTemporary(desc);
                }

                miniSim.Instantiated = false;
                desc.Dispose(false, true);
            }
        }

        public static void PackUpToMiniSimDescription(SimDescription ths)
        {
            if (ths.CreatedSim != null)
            {
                ths.CreatedSim.Destroy();
                Common.Sleep();
            }
            if (Find(ths.SimDescriptionId) != null)
            {
                SimDescription father = null;

                if (ths.Pregnancy != null)
                {
                    if ((ths.Pregnancy.mDad == null) || ths.Pregnancy.mDad.HasBeenDestroyed)
                    {
                        if (SimDescription.Find(ths.Pregnancy.DadDescriptionId) == null)
                        {
                            MiniSimDescription description = Find(ths.Pregnancy.DadDescriptionId);
                            if (description != null)
                            {
                                father = UnpackSim(description);
                                father.Genealogy.SetSimDescription(father);

                                Household.TouristHousehold.AddTemporary(father);
                            }
                        }
                    }
                }

                PackSim(ths);

                if (father != null)
                {
                    father.Dispose(true, true);
                }
            }
            else
            {
                MiniSimDescription.AddMiniSim(ths);
                ths.Household.RemoveTemporary(ths);

                MiniSimDescription miniSim;
                if (MiniSimDescription.sMiniSims.TryGetValue(ths.SimDescriptionId, out miniSim))
                {
                    GenerateCrossWorldThumbnail(miniSim, ths, true);
                }

                ths.Dispose(true, true);
            }

            ths.mPackingDescriptionTask = null;
            if (!GameStates.IsTravelling)
            {
                (Sims3.Gameplay.UI.Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).OnSimCurrentWorldChanged(false, ths);
            }

            ths.ClearOutfits(true);
        }

        public static void GenerateCrossWorldThumbnail(MiniSimDescription miniSim, SimDescription sim, bool forceGeneration)
        {
            if (forceGeneration || (sim.HomeWorld == GameUtils.GetCurrentWorld()))
            {
                ThumbnailTechnique technique = sim.IsDead ? ThumbnailTechnique.Sepia : ThumbnailTechnique.Default;

                ResourceKey travelThumbnailOutfitForSim = MiniSimDescription.GetTravelThumbnailOutfitForSim(sim);
                if (travelThumbnailOutfitForSim.InstanceId != 0x0L)
                {
                    ThumbnailManager.GenerateHouseholdSimThumbnail(travelThumbnailOutfitForSim, travelThumbnailOutfitForSim.InstanceId, 0x0, ThumbnailSizeMask.Large, technique, false, true, sim.AgeGenderSpecies);
                    ThumbnailManager.GenerateTravelSimThumbnail(travelThumbnailOutfitForSim, miniSim.mThumbKey.mDescKey.InstanceId, ThumbnailSizeMask.Large, technique);
                }
                else
                {
                    SimOutfit outfit = sim.GetOutfit(OutfitCategories.Everyday, 0);
                    if ((outfit != null) && (outfit.IsValid))
                    {
                        sim.mDefaultOutfitKey = outfit.Key;

                        ThumbnailManager.GenerateHouseholdSimThumbnail(sim.DefaultOutfitKey, sim.DefaultOutfitKey.InstanceId, 0, ThumbnailSizeMask.Large, technique, false, true, sim.AgeGenderSpecies);
                        ThumbnailManager.GenerateTravelSimThumbnail(sim.DefaultOutfitKey, miniSim.mThumbKey.mDescKey.InstanceId, ThumbnailSizeMask.Large, technique);
                    }
                }
            }
        }

        public static Results UpdateThumbnailKey(MiniSimDescription ths)
        {
            SimDescription sim = null;

            Results result = UpdateThumbnailKey(ths, ref sim);
            if (result == Results.Unnecessary) return result;

            if (sim != null)
            {
                try
                {
                    bool agingEnabled = sim.HasFlags(SimDescription.FlagField.AgingEnabled);
                    if (AgingManager.Singleton == null)
                    {
                        // Stops the game from pushing or popping the aging when there is no Aging Manager available
                        sim.SetFlags(SimDescription.FlagField.AgingEnabled, false);
                    }

                    try
                    {
                        sim.Dispose(false, true);
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(sim, e);
                    }
                    finally
                    {
                        sim.SetFlags(SimDescription.FlagField.AgingEnabled , agingEnabled);
                    }

                    if (sim.Household != null)
                    {
                        sim.Household.Remove(sim);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);
                }
            }

            return result;
        }
        public static Results UpdateThumbnailKey(MiniSimDescription ths, ref SimDescription sim)
        {
            try
            {
                if (ths == null) return Results.Unnecessary;

                if (ThumbnailManager.KeyExistsInDB(ths.mThumbKey)) return Results.Unnecessary;

                sim = SimDescription.Find(ths.SimDescriptionId);
                if (sim != null) return Results.Unnecessary;

                sim = UnpackSim(ths, false);
                if (sim == null) return Results.Failure;

                // There is an error creating thumbnails for foreign imaginary friends
                if (sim.OccultManager != null)
                {
                    if (sim.OccultManager.mOccultList != null)
                    {
                        OccultTypeHelper.Remove(sim, OccultTypes.ImaginaryFriend, true);
                    }
                    else if (sim.OccultManager.HasOccultType(OccultTypes.ImaginaryFriend))
                    {
                        sim.OccultManager.mCurrentOccultTypes &= ~OccultTypes.ImaginaryFriend;
                    }
                }

                if (ths.mTravelKey == ResourceKey.kInvalidResourceKey) return Results.Failure;

                ths.mThumbKey = new ThumbnailKey(ths.mTravelKey, 0x0, 0x0, 0x0, ThumbnailSize.Large);
                ths.mThumbKey.mDescKey.GroupId = 0x0;

                GenerateCrossWorldThumbnail(ths, sim, true);

                if (!ThumbnailManager.KeyExistsInDB(ths.mThumbKey))
                {
                    ths.mThumbKey = new ThumbnailKey(sim.DefaultOutfitKey, ThumbnailSize.Large);
                }

                if (ThumbnailManager.KeyExistsInDB(ths.mThumbKey))
                {
                    return Results.Success;
                }
                else
                {
                    return Results.Failure;
                }
            }
            catch (Exception e)
            {
                Common.Exception(ths.FullName, e);
                return Results.Failure;
            }
        }

        public static SimDescription UnpackSim(MiniSimDescription ths)
        {
            return UnpackSim(ths, false);
        }
        public static SimDescription UnpackSim(MiniSimDescription ths, bool updateGenealogy)
        {
            try
            {
                if (ths == null) return null;

                // Calling ImportSimDescription prior to the Aging Manager being available is invalid, don't allow it
                if (AgingManager.Singleton == null) return null;

                SimDescription desc = new SimDescription();
                ResourceKeyContentCategory installed = ResourceKeyContentCategory.kInstalled;

                DownloadContent.ImportSimDescription(ths.mTravelKey, desc, ref installed);

                desc.SimDescriptionId = ths.mSimDescriptionId;

                if (desc.CareerManager != null)
                {
                    // Fixup for careers require a household, which is not set until later in this process

                    desc.CareerManager.mJob = null;
                    desc.CareerManager.mSchool = null;
                }

                desc.Fixup();

                if (updateGenealogy && !GameStates.IsTravelling)
                {
                    if (desc.DefaultOutfitKey == ResourceKey.kInvalidResourceKey)
                    {
                        SimOutfit outfit = desc.GetOutfit(OutfitCategories.Everyday, 0x0);
                        if ((outfit == null) || (!outfit.IsValid))
                        {
                            desc.Dispose(false, false);
                            return null;
                        }

                        desc.UpdateFromOutfit(OutfitCategories.Everyday);
                    }
                    desc.CASGenealogy = ths.CASGenealogy;
                }

                Corrections.CleanupBrokenSkills(desc, null);

                OccultTypeHelper.ValidateOccult(desc, null);

                return desc;
            }
            catch (Exception e)
            {
                Common.Exception(ths.FullName, e);
                return null;
            }
        }

        public static bool ProtectedAddHousehold(Household house, SimDescription sim)
        {
            if ((house == null) || (sim == null)) return false;

            SimDescription altered = null;
            if (house.AllSimDescriptions.Count > 0)
            {
                altered = house.AllSimDescriptions[0];
            }
            else
            {
                altered = sim;
            }

            using (HomeworldReversion reversion = new HomeworldReversion(altered))
            {
                if (sim.GetMiniSimForProtection() == null)
                {
                    MiniSims.EnsureProperHomeworld(sim.SimDescriptionId);
                }

                house.Add(sim);
            }

            return true;
        }

        public class HomeworldReversion : IDisposable
        {
            WorldName mOldWorld;

            SimDescription mSim;

            public HomeworldReversion(SimDescription sim)
            {
                mSim = sim;
                if (sim != null)
                {
                    mOldWorld = sim.HomeWorld;
                    sim.mHomeWorld = WorldName.UserCreated;
                }
            }

            public void Dispose()
            {
                if (mSim != null)
                {
                    mSim.mHomeWorld = mOldWorld;
                }
            }
        }

        public static void EnsureProperHomeworld(ulong id)
        {
            MiniSimDescription description;
            if (MiniSimDescription.sMiniSims.TryGetValue(id, out description))
            {
                if (!GameUtils.IsWorldInstalled(description.HomeWorld))
                {
                    description.mHomeWorld = WorldName.UserCreated;
                }
            }
        }

        public static MiniSimDescription Find(ulong descId)
        {
            MiniSimDescription description;
            if ((MiniSimDescription.sMiniSims != null) && MiniSimDescription.sMiniSims.TryGetValue(descId, out description))
            {
                return description;
                /*
                if (GameUtils.IsWorldInstalled(description.mHomeWorld))
                {
                    return description;
                }
                if (description.mHomeWorld == WorldName.UserCreated)
                {
                    return description;
                }
                */
            }
            return null;
        }

        private static void UpdateInWorldRelationships(MiniSimDescription ths, SimDescription myDesc)
        {
            foreach (MiniRelationship relationship in ths.mMiniRelationships)
            {
                MiniSimDescription description = Find(relationship.SimDescriptionId);
                if ((description != null) && description.Instantiated)
                {
                    SimDescription y = SimDescription.Find(relationship.SimDescriptionId);
                    if (y == null)
                    {
                        description.Instantiated = false;
                    }
                    else
                    {
                        Relationship unsafely;
                        MiniRelationship mrB = description.FindMiniRelationship(ths, false);
                        
                        // Custom
                        if (mrB != null)
                        {
                            if (GameStates.IsTravelling)
                            {
                                unsafely = Relationship.GetUnsafely(myDesc, y);
                            }
                            else
                            {
                                unsafely = Relationship.Get(myDesc, y, true);
                            }
                            unsafely.UpdateFromMiniRelationship(relationship, mrB);
                        }
                    }
                }
            }
        }

        public static SimDescription UnpackSimAndUpdateRel(MiniSimDescription ths)
        {
            try
            {
                SimDescription myDesc = UnpackSim(ths, true);
                if (myDesc != null)
                {
                    bool isLifeEventManagerEnabled = LifeEventManager.sIsLifeEventManagerEnabled;

                    try
                    {
                        // Stops the Memories system from interfering with creation
                        LifeEventManager.sIsLifeEventManagerEnabled = false;

                        UpdateInWorldRelationships(ths, myDesc);
                    }
                    finally
                    {
                        LifeEventManager.sIsLifeEventManagerEnabled = isLifeEventManagerEnabled;
                    }

                    SimDescription partner = SimDescription.Find(ths.PartnerId);
                    if ((partner != null) && (partner.IsValidDescription))
                    {
                        myDesc.Partner = partner;
                        partner.Partner = myDesc;
                    }
                }
                return myDesc;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(ths.FullName, e);
                return null;
            }
        }

        public static SimDescription ImportWithCheck(MiniSimDescription miniSim)
        {
            // Custom
            //   Importing a sim that already exists in town causes issues with the long-term relationship system, don't do it
            SimDescription simDescription = SimDescription.Find(miniSim.SimDescriptionId);
            if (simDescription == null)
            {
                simDescription = UnpackSimAndUpdateRel(miniSim);
                if (simDescription != null)
                {
                    Household.CreateTouristHousehold();
                    Household.TouristHousehold.AddTemporary(simDescription);

                    miniSim.Instantiated = true;

                    (Sims3.UI.Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).OnSimCurrentWorldChanged(true, miniSim);

                    if (simDescription.AgingState == null)
                    {
                        simDescription.AgingState = new AgingState(simDescription);
                    }
                    simDescription.AgingState.MergeTravelInformation(miniSim);

                    simDescription.PushAgingEnabledToAgingManager();
                }
            }

            return simDescription;
        }
    }
}

