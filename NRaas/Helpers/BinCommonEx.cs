using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Proxies;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public class BinCommonEx
    {
        public delegate int GetLotCost(Lot lot, bool buyFurnished);

        public static bool PayForLot(Household household, Lot lot, bool buyFurnished, GetLotCost onLotCost)
        {
            int cost = onLotCost(lot, buyFurnished);

            if (cost == 0)
            {
                return true;
            }

            if (cost <= household.FamilyFunds)
            {
                household.ModifyFamilyFunds(-cost);
                return true;
            }

            return false;
        }

        public static PlaceResult PlaceFromExportBin(ExportBinContents exportBinItem, Lot lot, PlaceAction action, bool buyFurnished, GetLotCost onLotCost, ref Sim newActiveSim)
        {
            if (exportBinItem == null)
            {
                return PlaceResult.ContentFailure;
            }

            bool flag = exportBinItem.IsLoaded();
            if (!flag)
            {
                ExportBinContentsEx.Import(exportBinItem, false);
            }

            PlaceResult result = PlaceFromExportBin(exportBinItem.Household, exportBinItem, lot, action, buyFurnished, onLotCost, ref newActiveSim);
            if (!flag)
            {
                ExportBinContentsEx.Flush(exportBinItem);
            }

            return result;
        }
        public static PlaceResult PlaceFromExportBin(Household household, ExportBinContents exportBinItem, Lot lot, PlaceAction action, bool buyFurnished, GetLotCost onLotCost, ref Sim newActiveSim)
        {
            if (household == null)
            {
                return PlaceResult.InvalidBinHousehold;
            }
            else if (lot == null)
            {
                return PlaceResult.InvalidLot;
            }
            else if (exportBinItem == null)
            {
                return PlaceResult.ContentFailure;
            }

            bool flag = exportBinItem.IsLoaded();
            if (!flag)
            {
                ExportBinContentsEx.Import(exportBinItem, false);
            }

            PlaceResult contentFailure = PlaceResult.ContentFailure;
            if (household != null)
            {
                if (((action & PlaceAction.PlaceAndPay) != PlaceAction.PlaceOnly) && !PayForLot(household, lot, buyFurnished, onLotCost))
                {
                    if (!flag)
                    {
                        ExportBinContentsEx.Flush(exportBinItem);
                    }
                    Household.CleanupOldIdToNewSimDescriptionMap();
                    return PlaceResult.InsufficientFunds;
                }

                if ((!buyFurnished) && (!lot.IsApartmentLot))
                {
                    lot.MakeLotUnfurnished();
                    Common.Sleep();
                    lot.UpdateCachedValues();
                }

                CreateActors(household, lot, false);
                CreateInventories(household, exportBinItem.HouseholdContents, exportBinItem.IndexMap);
                Common.Sleep();
                BinCommon.UpdateImportedUrnstones(household, lot);
                household.PostImport();

                if ((action & PlaceAction.MoveIn) != PlaceAction.PlaceOnly)
                {
                    BinCommon.MoveIn(household, lot);
                }
                if ((action & PlaceAction.Activate) != PlaceAction.PlaceOnly)
                {
                    newActiveSim = BinCommon.ActivateSim(exportBinItem.Household, lot);
                }

                ThumbnailManager.GenerateHouseholdThumbnail(household.HouseholdId, household.HouseholdId, ThumbnailSizeMask.Large | ThumbnailSizeMask.Medium);
                contentFailure = PlaceResult.Success;
            }

            if (!flag)
            {
                ExportBinContentsEx.Flush(exportBinItem);
            }

            return contentFailure;
        }

        public static PlaceResult PlaceHouseholdFromGameBin(ulong contentId, Lot lot, PlaceAction action, GetLotCost onLotCost)
        {
            HouseholdContents contents = Bin.Singleton.FindHousehold(contentId);
            if (contents != null)
            {
                Household household = contents.Household;
                if (household != null)
                {
                    if (((action & PlaceAction.PlaceAndPay) != PlaceAction.PlaceOnly) && !PayForLot(household, lot, true, onLotCost))
                    {
                        return PlaceResult.InsufficientFunds;
                    }
                    BinCommonEx.CreateActors(household, lot, false);
                    BinCommonEx.CreateInventories(contents);
                    if ((action & PlaceAction.MoveIn) != PlaceAction.PlaceOnly)
                    {
                        BinCommon.MoveIn(household, lot);
                    }
                    Bin.Singleton.RemoveHouseholdFromGameBin(contents);
                    return PlaceResult.Success;
                }
            }
            return PlaceResult.ContentFailure;
        }
 
        public static PlaceResult PlaceHouseholdAndContentsFromGameBin(ulong contentId, Lot lot, PlaceAction action, GetLotCost onLotCost)
        {
            LotContents lotContents = Bin.Singleton.FindLot(contentId);
            if (lotContents != null)
            {
                Household household = lotContents.Household.Household;
                if (household != null)
                {
                    if (((action & PlaceAction.PlaceAndPay) != PlaceAction.PlaceOnly) && !PayForLot(household, lot, true, onLotCost))
                    {
                        return PlaceResult.InsufficientFunds;
                    }

                    CreateActors(household, lot, false);
                    CreateInventories(lotContents.Household);
                    if ((action & PlaceAction.MoveIn) != PlaceAction.PlaceOnly)
                    {
                        BinCommon.MoveIn(household, lot);
                    }

                    Bin.Singleton.RemoveLotFromGameBin(lotContents);
                    return PlaceResult.Success;
                }
            }
            return PlaceResult.ContentFailure;
        }

        public static void CreateInventories(HouseholdContents contents)
        {
            CreateInventories(contents.Household, contents, null);
        }
        public static void CreateInventories(Household household, HouseholdContents contents, List<ulong> indexMap)
        {
            if ((contents != null) && (household != null))
            {
                // Custom Function
                CreateSimInventories(household.AllSimDescriptions, contents, indexMap);

                // Custom Try/Catch
                try
                {
                    BinCommon.CreateFamilyInventories(household, contents);
                }
                catch (Exception e)
                {
                    Common.Exception("CreateFamilyInventories", e);
                }
            }
        }

        public static void CreateSimInventories(List<SimDescription> sims, HouseholdContents contents, List<ulong> indexMap)
        {
            List<Sim> newborns = new List<Sim>();
            for (int i = 0x0; i < sims.Count; i++)
            {
                SimDescription description = sims[i];

                // Custom Try/Catch
                try
                {
                    if ((description != null) && (description.CreatedSim != null))
                    {
                        int index = i;
                        if (indexMap != null)
                        {
                            for (int j = 0x0; j < indexMap.Count; j++)
                            {
                                if (indexMap[j] == description.SimDescriptionId)
                                {
                                    index = j;
                                    break;
                                }
                            }
                        }
                        ulong id = contents.Inventories[index];
                        if (id != 0x0L)
                        {
                            Diploma diploma = description.CreatedSim.Inventory.Find<Diploma>();
                            if ((diploma != null) && !description.CreatedSim.Inventory.TryToRemove(diploma))
                            {
                                diploma = null;
                            }

                            IPhoneSmart smart = null;
                            if (GameUtils.IsInstalled(ProductVersion.EP9))
                            {
                                smart = description.CreatedSim.Inventory.Find<IPhoneSmart>();
                                if ((smart != null) && !description.CreatedSim.Inventory.TryToRemove(smart))
                                {
                                    smart = null;
                                }
                            }

                            DownloadContent.ImportInventory(id, new InventoryProxy(description.CreatedSim.Inventory));
                            DownloadContent.DeleteInventory(id);

                            if (diploma != null)
                            {
                                Inventories.TryToMove(diploma, description.CreatedSim.Inventory, false);
                            }

                            if (smart != null)
                            {
                                IPhoneCell cell = description.CreatedSim.Inventory.Find<IPhoneCell>();
                                if (cell != null)
                                {
                                    cell.Destroy();
                                }
                                description.CreatedSim.Inventory.TryToAdd(smart);
                            }
                        }
                        else
                        {
                            if (description.Toddler)
                            {
                                newborns.Add(description.CreatedSim);
                            }
                            description.CreatedSim.AddInitialObjects(false);
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(description, e);
                }
            }

            // Custom Try/Catch
            try
            {
                OccultImaginaryFriend.DeliverDollToHousehold(newborns, true);
            }
            catch (Exception e)
            {
                Common.Exception("DeliverDollToHousehold", e);
            }
        }

        public static void CreateActors(Household household, Lot lot, bool bAddInitialObjects)
        {
            CreateActors(household.CurrentMembers.AllSimDescriptionList, lot, bAddInitialObjects);
        }
        public static void CreateActors(List<SimDescription> SimDescs, Lot lot, bool bAddInitialObjects)
        {
            List<Sim> sims = new List<Sim>();
            foreach (SimDescription description in SimDescs)
            {
                try
                {
                    description.HomeWorld = GameUtils.GetCurrentWorld();
                    if (description.CreatedSim == null)
                    {
                        if (description.Weight < 0f)
                        {
                            description.ChangeBodyShape(0f, description.Fitness, -description.Weight);
                        }
                        else
                        {
                            description.ChangeBodyShape(description.Weight, description.Fitness, 0f);
                        }

                        SimOutfit outfit = description.GetOutfit(OutfitCategories.Everyday, 0x0);
                        Vector3 position = new Vector3();

                        // Custom
                        Sim sim = Instantiation.Perform(description, position, outfit, null/*bAddInitialObjects*/);
                        if (sim.SimDescription.IsGhost)
                        {
                            Urnstone.SimToPlayableGhost(sim, true);
                        }
                        sims.Add(sim);
                    }
                    else
                    {
                        sims.Add(description.CreatedSim);
                    }

                    description.GetMiniSimForProtection().AddProtection(MiniSimDescription.ProtectionFlag.PartialFromPlayer);
                }
                catch (Exception e)
                {
                    Common.Exception(description, e);
                }
            }

            if (lot != null)
            {
                BinCommon.PlaceSims(sims, lot);
            }
        }

        public static PlaceResult MergeHouseholdFromExportBin(ExportBinContents exportBinItem, Lot lot, bool showConfirmDialog, bool allowOverstuff)
        {
            try
            {
                if (exportBinItem == null)
                {
                    return PlaceResult.ContentFailure;
                }

                bool flag = exportBinItem.IsLoaded();
                if (!flag)
                {
                    ExportBinContentsEx.Import(exportBinItem, false);
                }

                PlaceResult householdPresent = PlaceResult.HouseholdPresent;
                Household household = exportBinItem.Household;
                Household otherHouse = lot.Household;
                if (household == null)
                {
                    householdPresent = PlaceResult.InvalidBinHousehold;
                }

                if (lot == null)
                {
                    householdPresent = PlaceResult.InvalidLot;
                }

                if (exportBinItem == null)
                {
                    householdPresent = PlaceResult.ContentFailure;
                }

                try
                {
                    try
                    {
                        if (((household != null) && (householdPresent == PlaceResult.HouseholdPresent)) && (otherHouse != null))
                        {
                            // Custom
                            if ((!allowOverstuff) && (!household.CanMergeWithHousehold(otherHouse, false)))
                            {
                                SimpleMessageDialog.Show(Common.LocalizeEAString("Ui/Caption/GameEntry/EditTown:MergeWarning"), Common.LocalizeEAString("Ui/Caption/MovingDialog:TooManySims"));
                            }
                            // Custom
                            else if ((!allowOverstuff) && (!household.CanMergeWithHousehold(otherHouse, true)))
                            {
                                SimpleMessageDialog.Show(Common.LocalizeEAString("Ui/Caption/GameEntry/EditTown:MergeWarning"), Common.LocalizeEAString("Ui/Caption/MovingDialog:TooManySims_Pregnant"));
                            }
                            else if (!showConfirmDialog || PlayFlowConfirmMergePetHousehold.Show(BinCommon.CreateInWorldBinInfo(lot.LotId), exportBinItem.BinInfo))
                            {
                                ProgressDialog.Show(Common.LocalizeEAString("Ui/Caption/Global:Processing"));

                                otherHouse.ModifyFamilyFunds(household.FamilyFunds);

                                List<SimDescription> simDescs = new List<SimDescription>();
                                foreach (SimDescription description in household.AllSimDescriptions)
                                {
                                    simDescs.Add(description);
                                }

                                CreateActors(simDescs, null, false);
                                BinCommon.CreateFamilyInventories(household, exportBinItem.HouseholdContents);
                                BinCommon.MoveInventoryInto(household, otherHouse);
                                household.PostImport();

                                List<Sim> sims = new List<Sim>();
                                foreach (SimDescription description2 in simDescs)
                                {
                                    otherHouse.AddSim(description2.CreatedSim);
                                    sims.Add(description2.CreatedSim);
                                }

                                otherHouse.AddWardrobeToWardrobe(household.Wardrobe);
                                otherHouse.AddServiceUniforms(household.ServiceUniforms);
                                CreateSimInventories(simDescs, exportBinItem.HouseholdContents, exportBinItem.IndexMap);
                                BinCommon.PlaceSims(sims, lot);
                                ThumbnailManager.GenerateHouseholdThumbnail(otherHouse.HouseholdId, otherHouse.HouseholdId, ThumbnailSizeMask.Large | ThumbnailSizeMask.Medium);
                                (Sims3.UI.Responder.Instance.EditTownModel as EditTownModel).DirtyWorldBins();
                                Bin.Singleton.FireContentsChanged();
                            }
                        }

                        Household.CleanupOldIdToNewSimDescriptionMap();
                        if (!flag)
                        {
                            ExportBinContentsEx.Flush(exportBinItem);
                        }
                    }
                    finally
                    {
                        ProgressDialog.Close();
                    }
                }
                catch (ExecutionEngineException)
                { }

                return householdPresent;
            }
            catch (Exception e)
            {
                Common.Exception("MergeHouseholdFromExportBin", e);
                return PlaceResult.ContentFailure;
            }
        }
    }
}
