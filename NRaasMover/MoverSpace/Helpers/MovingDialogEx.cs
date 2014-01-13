using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects.Island;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System.Collections.Generic;

namespace NRaas.MoverSpace.Helpers
{
    public class MovingDialogEx
    {
        public static bool Show(IMovingModel model)
        {
            if (IntroTutorial.IsRunning && !IntroTutorial.AreYouExitingTutorial())
            {
                return false;
            }

            if (ScreenGrabController.InProgress || VideoGrabHelper.InUse)
            {
                return false;
            }

            UserToolUtils.OnClose();
            Sims3.Gameplay.UI.Responder.Instance.HudModel.CloseSecondaryInventory();
            PopupMenu.CloseOptions();
            Tutorialette.TriggerLesson(Lessons.Moving, null);
            List<IMapTagPickerInfo> mapTagPickerInfos = null;
            string titleText = Localization.LocalizeString("Ui/Caption/MovingDialog:ChooseLocation", new object[0]);
            string confirmText = Localization.LocalizeString("Ui/Caption/MovingDialog:ChooseConfirm", new object[0]);
            MoveDialogBase.Result running = MoveDialogBase.Result.Running;
            ulong maxValue = ulong.MaxValue;
            HouseboatSize houseboatSize = HouseboatSize.None;
            if (!MoveDialog.Show(model))
            {
                return false;
            }

            SceneMgrWindow sceneWindow = UIManager.GetSceneWindow();
            if (sceneWindow != null)
            {
                sceneWindow.StopForwardingAllEvents();
            }

            do
            {
                switch (running)
                {
                    case MoveDialogBase.Result.LotSelect:
                    case MoveDialogBase.Result.HouseSelect:
                    case MoveDialogBase.Result.LotAndHouseSelect:
                    case MoveDialogBase.Result.PortSelect:
                        running = MoveDialog.RunWithPickedLot(maxValue, houseboatSize);
                        break;

                    default:
                        running = MoveDialog.Run();
                        break;
                }

                switch (running)
                {
                    case MoveDialogBase.Result.LotSelect:
                    case MoveDialogBase.Result.HouseSelect:
                    case MoveDialogBase.Result.LotAndHouseSelect:
                    case MoveDialogBase.Result.PortSelect:
                        if (mapTagPickerInfos == null)
                        {
                            mapTagPickerInfos = new List<IMapTagPickerInfo>();
                        }
                        else
                        {
                            mapTagPickerInfos.Clear();
                        }

                        bool isSource = model.SourceIsSelectedHousehold();
                        int householdBuyingPowerFunds = model.GetHouseholdBuyingPowerFunds(isSource);
                        bool flag3 = true;
                        if (running == MoveDialogBase.Result.PortSelect)
                        {
                            flag3 = ChangeHouseboatDialog.Show(false, ref houseboatSize, true);
                        }

                        if (flag3)
                        {
                            foreach (Lot lot in LotManager.AllLots)
                            {
                                if ((!lot.IsWorldLot && lot.IsResidentialLot) && (lot.IsApartmentLot || (lot.Household == null)))
                                {
                                    if (running == MoveDialogBase.Result.PortSelect)
                                    {
                                        if ((lot.CommercialLotSubType == CommercialLotSubType.kEP10_Port) && lot.IsUnoccupiedPortLot())
                                        {
                                            HouseboatInfo info = HouseboatUtils.GetInfo(houseboatSize);
                                            MovingToPortMapTagPickerLotInfo item = new MovingToPortMapTagPickerLotInfo(lot, MapTagType.PortLot, info.Price);
                                            mapTagPickerInfos.Add(item);
                                        }
                                    }
                                    else if (((!lot.IsWorldLot && lot.IsResidentialLot) && (lot.ResidentialLotSubType != ResidentialLotSubType.kEP10_PrivateLot)) && ((lot.IsApartmentLot || (lot.Household == null)) && !UnchartedIslandMarker.DoesLotHaveUnchartedIslandMarker(lot)))
                                    {
                                        if (lot.IsApartmentLot)
                                        {
                                            if ((running == MoveDialogBase.Result.HouseSelect) && ((lot.Household == null) || !model.HouseholdIsSource(lot.Household.HouseholdId)))
                                            {
                                                MapTagPickerLotInfo item = new MapTagPickerLotInfo(lot, MapTagType.Apartment);
                                                mapTagPickerInfos.Add(item);
                                            }
                                        }
                                        else if (World.LotIsEmpty(lot.LotId) && lot.IsLotEmptyFromObjects())
                                        {
                                            if (running != MoveDialogBase.Result.HouseSelect)
                                            {
                                                MapTagPickerLotInfo info;

                                                // Custom
                                                if (Mover.GetLotCost(lot) <= householdBuyingPowerFunds)
                                                {
                                                    info = new MapTagPickerLotInfo(lot, MapTagType.AvailableEmptyLot);
                                                }
                                                else
                                                {
                                                    info = new MapTagPickerLotInfo(lot, MapTagType.UnavailableEmptyLot);
                                                }

                                                mapTagPickerInfos.Add(info);
                                            }
                                        }
                                        else if (running != MoveDialogBase.Result.LotSelect)
                                        {
                                            MapTagPickerLotInfo info2;

                                            // Custom
                                            if (Mover.GetLotCost(lot) <= householdBuyingPowerFunds)
                                            {
                                                info2 = new MapTagPickerLotInfo(lot, MapTagType.AvailableEmptyHouse);
                                            }
                                            else
                                            {
                                                info2 = new MapTagPickerLotInfo(lot, MapTagType.UnavailableEmptyHouse);
                                            }
                                            mapTagPickerInfos.Add(info2);
                                        }
                                    }
                                }
                            }
                        }
                        IMapTagPickerInfo info3 = null;

                        if (flag3)
                        {
                            bool flag4;
                            info3 = MapTagPickerDialog.Show(mapTagPickerInfos, titleText, confirmText, null, false, 0f, false, out flag4);
                        }
                        if (info3 != null)
                        {
                            maxValue = info3.LotId;
                        }
                        break;
                }
            }
            while ((running != MoveDialogBase.Result.Accepted) && (running != MoveDialogBase.Result.Cancelled));

            if (sceneWindow != null)
            {
                sceneWindow.StartForwardingEventsToGame();
            }

            MoveDialog.Shutdown();
            return (running == MoveDialogBase.Result.Accepted);
        }
    }
}
