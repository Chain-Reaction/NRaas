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
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.RealEstate;
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
    public class EditTownPlaceToolEx : EditTownPlaceTool
    {
        public override void OnToolSelected()
        {
            try
            {
                if (mModel.IsPlaceLotsWizardFlow)
                {
                    mLotTypesToPrompt = mModel.LotTypesToPromptFromInterface;
                    mLotTypesToForcePlace = mModel.LotTypesToForceLocation;
                    mPlacingLot = 0x0;
                    GetWizardLotToPlace();
                    if (!mLocalCancel)
                    {
                        mLocalCancel = true;
                        EditTownPuck.Instance.CancelClicked += OnSkipLot;
                    }
                }
                else
                {
                    mLotTypesToPrompt = null;
                    mLotTypesToForcePlace = null;
                    SetupCursor();
                    SetupMapTags();
                    EnableCancel(true);
                }
                EditTownPuck.Instance.EnableFilterButtons(false);
            }
            catch (Exception e)
            {
                Common.Exception("OnToolSelected", e);
            }
        }

        private new void SetupMapTags()
        {
            if (mInfo != null)
            {
                if (mInfo.LotId == ulong.MaxValue)
                {
                    if (GameUtils.IsOnVacation() || GameUtils.IsUniversityWorld())
                    {
                        mModel.ShowMaptags(MaptagTypes.Fraternity | MaptagTypes.Sorority | MaptagTypes.Dormitory | MaptagTypes.UnoccupiedPort | MaptagTypes.Apartment | MaptagTypes.DontShowActiveEver | MaptagTypes.SelectedApartment | MaptagTypes.SelectedHousehold | MaptagTypes.AvailableHousehold | MaptagTypes.AvailableEmptyHouse | MaptagTypes.SelectedEmptyHouse | MaptagTypes.UnavailableEmptyHouse | MaptagTypes.UnavailableHousehold);
                    }
                    else
                    {
                        // Custom
                        mModel.ShowMaptags(MaptagTypes.UnoccupiedPort | MaptagTypes.Apartment | MaptagTypes.SelectedApartment /*| MaptagTypes.DontShowOwnedEver*/ | MaptagTypes.SelectedHousehold | MaptagTypes.AvailableHousehold | MaptagTypes.AvailableEmptyHouse | MaptagTypes.ActiveHousehold | MaptagTypes.SelectedEmptyHouse | MaptagTypes.UnavailableEmptyHouse | MaptagTypes.UnavailableHousehold);
                    }
                }
                else if (mModel.IsPlaceLotsWizardFlow)
                {
                    mModel.ShowMaptags(MaptagTypes.SelectedOwnableLot | MaptagTypes.Last | MaptagTypes.Apartment | MaptagTypes.SelectedApartment | MaptagTypes.SelectedCommunity | MaptagTypes.AvailableEmptyLot | MaptagTypes.SelectedEmptyLot | MaptagTypes.EmptyCommunityLot | MaptagTypes.OwnableLot | MaptagTypes.CommunityLot);
                }
                else if (mInfo.LotTypeFilter == 0x8)
                {
                    if (mInfo.CommercialLotSubType == CommercialLotSubType.kEP10_Diving)
                    {
                        mModel.ShowMaptags(MaptagTypes.DivingLot);
                    }
                    else
                    {
                        EditTownMaptagController.Instance.ShowEmptyCommunityLots();
                    }
                }
                else if (mInfo.HouseholdId == ulong.MaxValue)
                {
                    // Custom
                    MaptagTypes filter = MaptagTypes.SelectedOwnableLot | MaptagTypes.UnavailableOwnableLot | MaptagTypes.DontShowMismatchedHouseboatLots /*| MaptagTypes.DontShowOwnedEver*/ | MaptagTypes.AvailableEmptyLot | MaptagTypes.SelectedEmptyLot | MaptagTypes.OwnableLot | MaptagTypes.UnavailableEmptyLot;
                    if (mInfo.IsHouseboatLot)
                    {
                        filter |= MaptagTypes.UnoccupiedPort;
                    }
                    mModel.ShowMaptags(filter);
                }
                else
                {
                    // Custom
                    MaptagTypes types2 = MaptagTypes.DontShowMismatchedHouseboatLots /*| MaptagTypes.DontShowOwnedEver*/ | MaptagTypes.AvailableEmptyLot | MaptagTypes.SelectedEmptyLot | MaptagTypes.UnavailableEmptyLot;
                    if (mInfo.IsHouseboatLot)
                    {
                        types2 |= MaptagTypes.UnoccupiedPort;
                    }
                    mModel.ShowMaptags(types2);
                }
                mModel.AllowLockedLots = true;
            }
        }

        private new void OnSelection(UIBinInfo lotInfo)
        {
            if (lotInfo.HouseholdId == ulong.MaxValue)
            {
                if ((mInfo != null) && (mFrom != InfoSource.Unknown))
                {
                    LotValidity lotValidity = mModel.GetLotValidity(lotInfo.LotId);

                    // EA Standard does not distinguish between vacation and private lots, messing up [StoryProgression] rental systems
                    foreach (PropertyData data in RealEstateManager.AllPropertiesFromAllHouseholds())
                    {
                        if (data.PropertyType == RealEstatePropertyType.VacationHome) continue;

                        if (data.LotId == lotInfo.LotId)
                        {
                            Simulator.AddObject(new OneShotFunctionWithParams(WarnLotGeneric, lotInfo));
                            return;
                        }
                    }

                    mModel.CenterCamera(lotInfo.LotId);
                    if (mInfo.LotId != ulong.MaxValue)
                    {
                        bool allow = false;
                        if (lotValidity == LotValidity.Valid)
                        {
                            allow = true;
                        }
                        else if ((Mover.Settings.mAllowGreaterThanEight) && (!mModel.LotContentsWillFit(mInfo, lotInfo.LotId)))
                        {
                            allow = true;
                        }

                        if (allow)
                        {
                            mPlacing = true;
                            EditTownController.Instance.ShowUI(false);
                            UIManager.DarkenBackground(true);
                            Sims3.UI.Responder.Instance.DisableQuit();
                            Simulator.AddObject(new OneShotFunctionWithParams(PlaceLotTask, lotInfo));
                        }
                        else if (!mModel.LotContentsWillFit(mInfo, lotInfo.LotId))
                        {
                            Simulator.AddObject(new OneShotFunctionWithParams(WarnTooBig, lotInfo));
                        }
                        else
                        {
                            Simulator.AddObject(new OneShotFunctionWithParams(WarnLotGeneric, lotInfo));
                        }
                    }
                    else if (mInfo.HouseholdId != ulong.MaxValue)
                    {
                        if (lotValidity == LotValidity.Unavailable) 
                        {
                            if ((!Mover.Settings.mFreeRealEstate) && (mInfo.HouseholdFunds < lotInfo.LotWorth))
                            {
                                Simulator.AddObject(new OneShotFunctionWithParams(WarnInsufficientFunds, lotInfo));
                                return;
                            }
                        }

                        if (lotValidity != LotValidity.Invalid)
                        {
                            mPlacing = true;
                            Simulator.AddObject(new OneShotFunctionWithParams(ConfirmPurchaseTaskEx, lotInfo));
                            return;
                        }
                    }
                }
            }
            else if (mInfo.LotId == ulong.MaxValue)
            {
                if (mFrom == InfoSource.Clipboard)
                {
                    mModel.CenterCamera(lotInfo.LotId);
                    GameEntryMovingModelEx.MergeHouseholds(EditTownController.Instance, mInfo, lotInfo);
                }
                else
                {
                    mModel.CenterCamera(lotInfo.LotId);
                    Simulator.AddObject(new OneShotFunctionWithParams(PlaceHouseholdTaskEx, lotInfo));
                }
            }
        }

        private void ConfirmPurchaseTaskEx(object lotInfoParam)
        {
            try
            {
                UIBinInfo info = lotInfoParam as UIBinInfo;
                if ((info != null) && (info.LotId != ulong.MaxValue))
                {
                    int worth = 0;
                    if (!Mover.Settings.mFreeRealEstate)
                    {
                        Lot lot = LotManager.GetLot(info.LotId);
                        if (lot != null)
                        {
                            worth = info.LotWorth;
                        }
                        else
                        {
                            worth = Mover.GetLotCost(lot);
                        }
                    }

                    if (EditTownConfirmPurchaseLot.Show(info.LotId, worth, mInfo.HouseholdFunds) != EditTownConfirmPurchaseLot.Result.Cancel)
                    {
                        PlaceHouseholdEx(mInfo, info.LotId);
                    }
                }

                mPlacing = false;
            }
            catch (Exception e)
            {
                Common.Exception("ConfirmPurchaseTaskEx", e);
            }
        }

        private void PlaceHouseholdEx(UIBinInfo householdInfo, ulong lotId)
        {
            try
            {
                if (householdInfo != null)
                {
                    if ((!Mover.Settings.mFreeRealEstate) && (!EditTownModelEx.PayForLot(mModel as EditTownModel, householdInfo, lotId)))
                    {
                        WarnInsufficientFunds(householdInfo);
                        return;
                    }

                    if (mFrom == InfoSource.Clipboard)
                    {
                        EditTownModelEx.PlaceFromGameBin(mModel as EditTownModel, householdInfo, lotId, PlaceAction.MoveIn);
                    }
                    else
                    {
                        EditTownModelEx.PlaceFromExportBin(mModel as EditTownModel, householdInfo, lotId, PlaceAction.MoveIn);
                    }
                    mModel.SetCurrentSelection(null, InfoSource.Unknown);
                }

                EditTownTool.CurrentTool = EditTownDefaultTool.Instance;
            }
            catch (Exception e)
            {
                Common.Exception("PlaceHouseholdEx", e);
            }
        }

        private void PlaceHouseholdTaskEx(object lotInfoParam)
        {
            try
            {
                UIBinInfo info = lotInfoParam as UIBinInfo;
                if ((info != null) && (info.LotId != ulong.MaxValue))
                {
                    EditTownModelEx.PlaceFromExportBin(mModel as EditTownModel, mInfo, info.LotId, PlaceAction.MoveIn);
                    mModel.SetCurrentSelection(null, InfoSource.Unknown);
                    mPlacing = false;
                    EditTownTool.CurrentTool = EditTownDefaultTool.Instance;
                }
                else
                {
                    mPlacing = false;
                }
            }
            catch (Exception e)
            {
                Common.Exception("PlaceHouseholdTaskEx", e);
            }
        }

        public override void OnMaptagSelection(ulong id, MaptagTypes maptagType, Vector2 mouseClickScreen)
        {
            try
            {
                if (!mPlacing)
                {
                    UIBinInfo lotInfo = FindBinInfoForLot(id, maptagType);
                    if (lotInfo != null)
                    {
                        OnSelection(lotInfo);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnMaptagSelection", e);
            }
        }

        public override bool OnLotPick(ulong id)
        {
            try
            {
                if (mPlacing)
                {
                    return false;
                }

                UIBinInfo lotInfo = FindBinInfoForLot(id, MaptagTypes.None);
                if (lotInfo == null)
                {
                    return false;
                }

                if (mModel.IsLotLocked(lotInfo.LotId) && !World.IsEditInGameFromWBMode())
                {
                    Simulator.AddObject(new Sims3.UI.OneShotFunctionTask(delegate
                    {
                        SimpleMessageDialog.Show(null, Sims3.UI.Responder.Instance.LocalizationModel.LocalizeString("Ui/Tooltip/Maptag:Locked", new object[0x0]), ModalDialog.PauseMode.PauseTask);
                    }));
                    return false;
                }

                OnSelection(lotInfo);
                return true;
            }
            catch (Exception e)
            {
                Common.Exception("OnLotPick", e);
                return false;
            }
        }
    }     
}
