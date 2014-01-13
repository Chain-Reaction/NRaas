using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MoverSpace.Helpers
{
    public class EditTownModelEx
    {
        public static bool PayForLot(EditTownModel ths, UIBinInfo info, ulong lotId)
        {
            Lot lot = LotManager.GetLot(lotId);
            if (lot != null)
            {
                ExportBinContents contents = ths.FindExportBinContents(info.ContentId);
                if (contents != null)
                {
                    bool flag = contents.IsLoaded();
                    if (!flag)
                    {
                        ExportBinContentsEx.Import(contents, true);
                    }

                    Household household = contents.Household;
                    if (household != null)
                    {
                        return BinCommonEx.PayForLot(household, lot, true, Mover.GetLotCost);
                    }

                    if (!flag)
                    {
                        ExportBinContentsEx.Flush(contents);
                    }
                }
                else if (info.LotId != ulong.MaxValue)
                {
                    LotContents contents2 = Bin.Singleton.FindLot(info.LotId);
                    if ((contents2 != null) && (contents2.Household != null))
                    {
                        Household household2 = contents2.Household.Household;
                        if (household2 != null)
                        {
                            return BinCommonEx.PayForLot(household2, lot, true, Mover.GetLotCost);
                        }
                    }
                }
                else if (info.HouseholdId != ulong.MaxValue)
                {
                    Household household3 = Household.Find(info.HouseholdId);
                    if (household3 != null)
                    {
                        return BinCommonEx.PayForLot(household3, lot, true, Mover.GetLotCost);
                    }
                }
            }
            return false;
        }

        public static PlaceResult PlaceFromGameBin(EditTownModel ths, UIBinInfo info, ulong lotId, PlaceAction action)
        {
            PlaceResult contentFailure = PlaceResult.ContentFailure;
            Lot lot = LotManager.GetLot(lotId);
            if (lot == null)
            {
                return PlaceResult.InvalidLot;
            }
            else if (lot.Household != null)
            {
                return PlaceResult.HouseholdPresent;
            }

            ths.GetInWorldHouseholdBinInfoList();
            contentFailure = PlaceResult.ContentFailure;
            if (info.HouseholdId != ulong.MaxValue)
            {
                BinCommon.KickSimsOffLot(lot, true);
                if (info.LotId != ulong.MaxValue)
                {
                    contentFailure = BinCommonEx.PlaceHouseholdAndContentsFromGameBin(info.ContentId, lot, action, Mover.GetLotCost);
                }
                else
                {
                    contentFailure = BinCommonEx.PlaceHouseholdFromGameBin(info.ContentId, lot, action, Mover.GetLotCost);
                }

                if (contentFailure != PlaceResult.Success)
                {
                    return contentFailure;
                }

                ths.mGameBin.Remove(info);
                if (ths.GameBinChanged != null)
                {
                    ths.GameBinChanged(ths, null);
                }

                info = BinCommon.CreateInWorldBinInfo(lot.Household, lot);
                if (info != null)
                {
                    ths.mInWorldHouseholdBin.Add(info);
                }
            }
            return contentFailure;
        }

        public static PlaceResult PlaceFromExportBin(EditTownModel ths, UIBinInfo info, ulong lotId, PlaceAction action)
        {
            return PlaceFromExportBin(ths, info, lotId, action, true);
        }
        public static PlaceResult PlaceFromExportBin(EditTownModel ths, UIBinInfo info, ulong lotId, PlaceAction action, bool showConfirmDialog)
        {
            try
            {
                PlaceResult contentFailure = PlaceResult.ContentFailure;
                Lot lot = LotManager.GetLot(lotId);
                if (lot == null)
                {
                    return PlaceResult.InvalidLot;
                }

                ExportBinContents exportBinItem = ths.FindExportBinContents(info.ContentId);
                if (exportBinItem != null)
                {
                    ths.GetInWorldHouseholdBinInfoList();
                    ths.GetInWorldLotBinInfoList();
                    Sim newActiveSim = null;
                    if (lot.Household != null)
                    {
                        return BinCommonEx.MergeHouseholdFromExportBin(exportBinItem, lot, showConfirmDialog, Mover.Settings.mAllowGreaterThanEight);
                    }

                    BinCommon.KickSimsOffLot(lot, true);
                    ProgressDialog.Show(Localization.LocalizeString("Ui/Caption/Global:Processing", new object[0x0]));

                    try
                    {
                        try
                        {
                            contentFailure = BinCommonEx.PlaceFromExportBin(exportBinItem, lot, action, true, Mover.GetLotCost, ref newActiveSim);
                            
                            ExportBinContentsEx.Flush(exportBinItem);

                            UIBinInfo item = BinCommon.CreateInWorldBinInfo(lot.Household, lot);
                            if (item != null)
                            {
                                if (info.HouseholdId != ulong.MaxValue)
                                {
                                    ths.mInWorldHouseholdBin.Add(item);
                                }
                                else
                                {
                                    ths.mInWorldLotBin.Add(item);
                                }
                            }
                        }
                        finally
                        {
                            ProgressDialog.Close();
                        }
                    }
                    catch (ExecutionEngineException)
                    { }
                }
                return contentFailure;
            }
            catch (Exception e)
            {
                Common.Exception("PlaceFromExportBin", e);
                return PlaceResult.ContentFailure;
            }
        }
    }
}
