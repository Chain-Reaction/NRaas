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
    public class ExportBinContentsEx
    {
        public static List<ulong> CreateIndexMap(Household house)
        {
            List<ulong> indexMap = new List<ulong>();
            if (house != null)
            {
                foreach (SimDescription description in house.AllSimDescriptions)
                {
                    indexMap.Add(description.SimDescriptionId);
                }
            }

            return indexMap;
        }

        public static void Import(ExportBinContents ths, bool origImport)
        {
            switch (ths.mExportBinType)
            {
                case ExportBinType.Household:
                    ulong lotId = DownloadContent.ImportHouseholdContentsFromExportBin(ths.mPackageName);
                    if (lotId != 0x0L)
                    {
                        // Custom
                        HouseholdContentsProxy proxy = new HouseholdContentsProxy();
                        if (DownloadContent.ImportHouseholdContents(proxy, lotId))
                        {
                            ths.mHouseholdContents = proxy.Contents;
                            ths.mHouseholdContents.ContentId = lotId;
                        }
                    }
                    break;
                case ExportBinType.HouseholdLot:
                case ExportBinType.Lot:
                    ulong oldLotContentId = 0x0L;
                    ulong lotContendId = DownloadContent.ImportLotContentsFromExportBin(ths.mPackageName, ref oldLotContentId);
                    if (lotContendId != 0x0L)
                    {
                        if (ths.mExportBinType == ExportBinType.HouseholdLot)
                        {
                            // Custom
                            HouseholdContentsProxy proxy = new HouseholdContentsProxy();
                            ths.mHouseholdContents = proxy.Contents;

                            if (DownloadContent.ImportHouseholdContents(proxy, lotContendId))
                            {
                                ths.mHouseholdContents.ContentId = lotContendId;
                            }
                        }

                        LotContents contents2 = new LotContents(ths.mPackageName, lotContendId, ths.mHouseholdContents, ths.mLotContentsWorth, ths.LotType, ths.LotContentsSizeX, ths.LotContentsSizeY, ths.IsHouseboatLot, ths.mLotMaxLevel, oldLotContentId, ths.mResidentialLotSubType, ths.mCommercialLotSubType);
                        ths.mLotContents = contents2;
                    }
                    break;
            }

            if (origImport || (ths.mIndexMap == null))
            {
                ths.mIndexMap = CreateIndexMap(ths.Household);
            }
        }

        public static void Flush(ExportBinContents ths)
        {
            switch (ths.mExportBinType)
            {
                case ExportBinType.Household:
                    if (ths.mHouseholdContents != null)
                    {
                        DownloadContent.DeleteHouseholdContents(ths.mHouseholdContents.ContentId);
                    }
                    ths.mHouseholdContents = null;
                    break;
                case ExportBinType.HouseholdLot:
                    if (ths.mLotContents != null)
                    {
                        DownloadContent.DeleteLotContents(ths.mLotContents.ContentId);
                    }
                    ths.mLotContents = null;
                    ths.mHouseholdContents = null;
                    break;
                case ExportBinType.Lot:
                    if (ths.mLotContents != null)
                    {
                        DownloadContent.DeleteLotContents(ths.mLotContents.ContentId);
                    }
                    ths.mLotContents = null;
                    ths.mHouseholdContents = null;
                    break;
            }

            Household.CleanupOldIdToNewSimDescriptionMap();
        }
    }
}
