using NRaas.CommonSpace.Replacers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Entertainment;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Helpers
{
    public class ThumbnailHelperEx
    {
        private static bool PreparePromSimsForThumbnail(ulong sim1, ulong sim2)
        {
            List<SimDescription> collection = new List<SimDescription>();
            foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
            {
                if ((sim.ObjectId.mValue == sim1) || (sim.ObjectId.mValue == sim2))
                {
                    /*
                    if (collection.Count == 0x1)
                    {
                        foreach (SimDescription description in collection)
                        {
                            description.SetPartner(sim.SimDescription);
                            sim.SimDescription.SetPartner(description);
                        }
                    }
                    */
                    collection.Add(sim.SimDescription);
                }
            }

            if (collection.Count != 0x0)
            {
                List<SimDescription> descriptions = new List<SimDescription>(collection);
                Hashtable simPosesForPromThumbnnail = ThumbnailHelper.GetSimPosesForPromThumbnnail(collection);
                ThumbnailHelper.BuildSimsForPoses(descriptions, simPosesForPromThumbnnail);
                PromSituation.SetBackdrop();
                ThumbnailManager.SetHouseholdCamera(0x5815de37);
            }
            return true;
        }

        public static bool OnPrepareObject(ObjectGuid templateId, ObjectGuid targetId, int index, uint uintVal1, uint uintVal2, ThumbnailSize size, uint prepareType, uint uintVal3)
        {
            try
            {
                switch (((PrepareType)prepareType))
                {
                    case PrepareType.kPrepareHousehold:
                        try
                        {
                            return ThumbnailHelper.PrepareHouseholdForThumbnail(templateId.Value);
                        }
                        catch (Exception e)
                        {
                            Common.DebugException("PrepareHouseholdForThumbnail", e);
                            return false;
                        }

                    case PrepareType.kPrepareSimWithoutTemplate:
                        {
                            ResourceKey outfitKey = new ResourceKey(targetId.Value, (uint)templateId.Value, uintVal1);
                            bool useCasSimBuilder = uintVal2 != 0x0;
                            int num = index;
                            int ghostIndex = -1;
                            if ((outfitKey.TypeId != 0xdea2951c) && ((index < 0x100) || (index > 0x300)))
                            {
                                if (num > -4)
                                {
                                    num = -1;
                                }
                                if ((index >= 6) && (index < 0x20))
                                {
                                    ghostIndex = index - 0x5;
                                }
                            }
                            return ThumbnailHelper.SetupForSimThumbnailUsingSimBuilder(outfitKey, num, ghostIndex, useCasSimBuilder, size);
                        }
                    case PrepareType.kPrepareLot:
                        return ThumbnailHelper.PrepareLotForThumbnail(templateId.Value);

                    case PrepareType.kPreparePromSims:
                        {
                            Sim sim = GameObject.GetObject(new ObjectGuid(templateId.Value)) as Sim;
                            if ((sim == null) || !sim.IsHorse)
                            {
                                // Custom
                                return PreparePromSimsForThumbnail(templateId.Value, targetId.Value);
                            }
                            return ThumbnailHelper.PrepareEquestrianRaceSimsForThumbnail(templateId.Value, targetId.Value);
                        }
                    case PrepareType.kPreparePhotoBoothSims:
                        return ThumbnailHelper.PreparePhotoBoothSimsForThumbnail(templateId.Value, targetId.Value);

                    case PrepareType.kPrepareSimsUsingObject:
                        return ThumbnailHelper.PrepareThumbnailForSimsUsingObject(templateId.Value);

                    case PrepareType.kPrepareSimsForSelfPhoto:
                        return ThumbnailHelper.PrepareSelfPhotoSimsForThumbnail(templateId.Value, targetId.Value);

                    case PrepareType.kPrepareSculptureSim:
                        return ThumbnailHelper.PrepareSculptureSimForThumbnail(targetId.Value);

                    case PrepareType.kPrepareSimsForServoBotArenaPic:
                        return ThumbnailHelper.PrepareServoBotArenaSimsForThumbnail(templateId.Value);
                }

                if ((((templateId.Value == 0x34aeecbL) || (templateId.Value == 0x358b08aL)) || ((templateId.Value == 0x93d84841L) || (templateId.Value == 0x51df2ddL))) || ((templateId.Value == 0x72683c15L) || (templateId.Value == 0x3555ba8L)))
                {
                    CASAgeGenderFlags ageGender = (CASAgeGenderFlags)uintVal1;
                    bool flag2 = uintVal2 != 0x0;
                    ResourceKey partKey = new ResourceKey(targetId.Value, (uint)templateId.Value, uintVal3);
                    if (flag2)
                    {
                        return ThumbnailHelper.SetupForCASThumbnailUsingCASSimbuilder(partKey, index, ageGender, size);
                    }
                    return ThumbnailHelper.SetupForCASThumbnailUsingSeparateSimbuilder(partKey, index, ageGender, size);
                }

                IScriptProxy proxyPreInit = Simulator.GetProxyPreInit(templateId);
                if (proxyPreInit != null)
                {
                    object target = proxyPreInit.Target;
                    if (target == null)
                    {
                        return false;
                    }
                    Sim sim2 = target as Sim;
                    if (sim2 != null)
                    {
                        if ((index >= 6) && (index < 0x20))
                        {
                            if (!sim2.SimDescription.IsEP11Bot)
                            {
                                uint deathTypeFromMoodID = (uint)SimDescription.GetDeathTypeFromMoodID((MoodID)index);
                                World.ObjectSetGhostState(targetId, deathTypeFromMoodID, (uint)sim2.SimDescription.AgeGenderSpecies);
                            }
                            else
                            {
                                World.ObjectSetGhostState(targetId, 0x17, (uint)sim2.SimDescription.AgeGenderSpecies);
                            }
                        }
                        if (sim2.SimDescription.IsVampire)
                        {
                            World.ObjectSetVisualOverride(targetId, eVisualOverrideTypes.Vampire, null);
                        }
                        else if (sim2.SimDescription.IsWerewolf)
                        {
                            World.ObjectSetVisualOverride(targetId, eVisualOverrideTypes.Werewolf, null);
                        }
                        else if (sim2.SimDescription.IsGenie)
                        {
                            World.ObjectSetVisualOverride(targetId, eVisualOverrideTypes.Genie, null);
                        }
                        if (sim2.SimDescription.IsAlien)
                        {
                            World.ObjectSetVisualOverride(targetId, eVisualOverrideTypes.Alien, null);
                        }

                        SimOutfit outfit = (sim2.Service == null) ? sim2.SimDescription.GetOutfit(OutfitCategories.Everyday, 0x0) : sim2.SimDescription.GetOutfit(OutfitCategories.Career, 0x0);
                        if ((outfit != null) && ((outfit.AgeGenderSpecies & (CASAgeGenderFlags.Child | CASAgeGenderFlags.Teen | CASAgeGenderFlags.YoungAdult | CASAgeGenderFlags.Toddler | CASAgeGenderFlags.Baby | CASAgeGenderFlags.Adult | CASAgeGenderFlags.Elder)) == sim2.SimDescription.Age))
                        {
                            CASUtils.SetOutfitInGameObject(outfit.Key, targetId);
                            ThumbnailHelper.SelectSimPose(index, outfit.AgeGenderSpecies & (CASAgeGenderFlags.Child | CASAgeGenderFlags.Teen | CASAgeGenderFlags.YoungAdult | CASAgeGenderFlags.Toddler | CASAgeGenderFlags.Baby | CASAgeGenderFlags.Adult | CASAgeGenderFlags.Elder), outfit.AgeGenderSpecies & ((CASAgeGenderFlags)0xcf00), 0x0, false);
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnPrepareObject", e);
            }
            return false;
        }
    }
}
