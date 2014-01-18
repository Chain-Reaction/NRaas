using NRaas.CommonSpace.Proxies;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using System;

namespace NRaas.CommonSpace.Helpers
{
    public class BinEx
    {
        public static bool ImportSim(SimDescription desc, Vector3 simPos, ulong inventoryIndex)
        {
            if (desc.Weight < 0f)
            {
                desc.ChangeBodyShape(0f, desc.Fitness, -desc.Weight);
            }
            else
            {
                desc.ChangeBodyShape(desc.Weight, desc.Fitness, 0f);
            }

            desc.PushAgingEnabledToAgingManager();

            Sim createdSim = desc.CreatedSim;
            if (createdSim == null)
            {
                createdSim = Instantiation.Perform(desc, simPos, null, null);
                if (createdSim == null)
                {
                    FixInvisibleTask.Perform(desc, false);

                    createdSim = Instantiation.Perform(desc, simPos, null, null);
                    if (createdSim == null) return false;
                }
            }

            try
            {
                Bin.ImportInventory(inventoryIndex, createdSim.Inventory);

                createdSim.GrantSpecialObjects(false, false);
            }
            catch (Exception e)
            {
                Common.Exception(createdSim, e);
            }

            return true;
        }

        public static HouseholdContents ImportHouseholdForTravel()
        {
            try
            {
                ulong lotId = DownloadContent.ImportHouseholdContentsFromTravelBin();
                if (lotId != 0x0L)
                {
                    HouseholdContentsProxy householdContents = new HouseholdContentsProxy();
                    if (DownloadContent.ImportHouseholdContents(householdContents, lotId))
                    {
                        householdContents.Contents.ContentId = lotId;
                        return householdContents.Contents;
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("ImportHouseholdForTravel", e);
            }

            return null;
        }

        public static string ExportHousehold(Bin ths, Household household, bool includeLotContents)
        {
            return ExportHousehold(ths, household, includeLotContents, false);
        }
        public static string ExportHousehold(Bin ths, Household household, bool includeLotContents, bool isMovingPacked)
        {
            try
            {
                string str = null;
                if (GameUtils.IsInstalled(ProductVersion.EP4))
                {
                    OccultImaginaryFriend.ForceHouseholdImaginaryFriendsBackToInventory(household);
                }

                foreach (Sim sim in household.AllActors)
                {
                    sim.SetObjectToReset();
                }

                Common.Sleep();

                if (includeLotContents)
                {
                    Lot lotHome = household.LotHome;
                    if (lotHome != null)
                    {
                        int num = household.FamilyFunds;
                        int num2 = World.GetEmptyLotWorth(lotHome.LotId) + ((int)World.GetLotAdditionalPropertyValue(lotHome.LotId));
                        household.SetFamilyFunds(household.FamilyFunds + num2, false);
                        EditTownModel.SendObjectsToProperLot(lotHome);
                        ulong contentId = DownloadContent.StoreLotContents(lotHome, lotHome.LotId);
                        if (contentId != 0x0L)
                        {
                            ThumbnailHelper.GenerateLotThumbnailSet(lotHome.LotId, contentId, ThumbnailSizeMask.ExtraLarge);
                            ThumbnailHelper.GenerateLotThumbnail(lotHome.LotId, contentId, 0x0, ThumbnailSizeMask.Large | ThumbnailSizeMask.Medium);
                            ThumbnailSizeMask mask = ThumbnailSizeMask.Large | ThumbnailSizeMask.ExtraLarge | ThumbnailSizeMask.Medium;
                            ThumbnailManager.GenerateHouseholdThumbnail(household.HouseholdId, contentId, mask);
                            ths.GenerateSimThumbnails(household, contentId, ThumbnailSizeMask.Large | ThumbnailSizeMask.ExtraLarge);
                            HouseholdContentsProxy contents = new HouseholdContentsProxy(household);
                            if (DownloadContent.StoreHouseholdContents(contents, contentId))
                            {
                                str = DownloadContent.ExportLotContentsToExportBin(contentId);
                            }
                            ThumbnailManager.InvalidateLotThumbnails(lotHome.LotId, contentId, ThumbnailSizeMask.ExtraLarge);
                            ThumbnailManager.InvalidateLotThumbnailsForGroup(lotHome.LotId, contentId, ThumbnailSizeMask.Medium, 0x0);
                            ThumbnailManager.InvalidateHouseholdThumbnail(household.HouseholdId, contentId, mask);
                            ths.InvalidateSimThumbnails(household, contentId);
                        }
                        household.SetFamilyFunds(num, false);
                    }
                    return str;
                }

                int familyFunds = household.FamilyFunds;
                int realEstateFunds = 0;
                if (household.RealEstateManager != null)
                {
                    foreach (IPropertyData data in household.RealEstateManager.AllProperties)
                    {
                        realEstateFunds += data.TotalValue;
                    }
                }
                if (household.LotHome != null)
                {
                    int lotWorth = 0;
                    if (isMovingPacked)
                    {
                        lotWorth = World.GetUnfurnishedLotWorth(household.LotHome.LotId) + realEstateFunds;
                    }
                    else
                    {
                        lotWorth = World.GetLotWorth(household.LotHome.LotId) + realEstateFunds;
                    }

                    household.SetFamilyFunds(household.FamilyFunds + lotWorth, false);
                }

                if (household.FamilyFunds < 0x4e20)
                {
                    household.SetFamilyFunds(0x4e20, false);
                }

                ulong cacheId = DownloadContent.GenerateGUID();
                HouseholdContentsProxy householdContents = new HouseholdContentsProxy(household);
                householdContents.Contents.ContentId = cacheId;
                ThumbnailSizeMask sizeMask = ThumbnailSizeMask.Large | ThumbnailSizeMask.ExtraLarge | ThumbnailSizeMask.Medium;
                ThumbnailManager.GenerateHouseholdThumbnail(household.HouseholdId, cacheId, sizeMask);
                ths.GenerateSimThumbnails(household, cacheId, ThumbnailSizeMask.Large | ThumbnailSizeMask.ExtraLarge);
                if (DownloadContent.StoreHouseholdContents(householdContents, cacheId))
                {
                    str = DownloadContent.ExportLotContentsToExportBin(cacheId);
                }

                ThumbnailManager.InvalidateHouseholdThumbnail(household.HouseholdId, cacheId, sizeMask);
                ths.InvalidateSimThumbnails(household, cacheId);
                household.SetFamilyFunds(familyFunds, false);
                return str;
            }
            catch (Exception e)
            {
                Common.Exception("ExportHousehold", e);
                return null;
            }
        }

        public static bool ExportHouseholdForTravel(Household household)
        {
            try
            {
                foreach (Sim sim in Households.AllSims(household))
                {
                    sim.SetObjectToReset();

                    DreamCatcher.PruneDreamManager(sim);
                }

                Common.Sleep();

                ulong id = DownloadContent.GenerateGUID();
                HouseholdContentsProxy householdContents = new HouseholdContentsProxy(household);
                householdContents.Contents.ContentId = id;
                if (DownloadContent.StoreHouseholdContents(householdContents, id))
                {
                    DownloadContent.ExportLotContentsToTravelBin(id);
                    return true;
                }
            }
            catch (Exception e)
            {
                Common.Exception("ExportHouseholdForTravel", e);
            }

            return false;
        }
    }
}
