using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.TravelerSpace.Dialogs;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.Helpers
{
    public class EditTownControllerEx
    {
        public static void ChangeLotType(UIBinInfo info)
        {
            Simulator.AddObject(new OneShotFunctionWithParams(new FunctionWithParam(ChangeLotTypeTask), info));
        }

        public static void ChangeLotTypeTask(object obj)
        {
            try
            {
                EditTownController ths = EditTownController.Instance;

                UIBinInfo info = obj as UIBinInfo;
                if ((info != null) && (info.LotId != ulong.MaxValue))
                {
                    ths.mModel.SetCurrentSelection(null, InfoSource.Unknown);
                    ILocalizationModel localizationModel = Sims3.UI.Responder.Instance.LocalizationModel;
                    LotType lotType = info.LotType;
                    CommercialLotSubType commercialLotSubType = info.CommercialLotSubType;
                    ResidentialLotSubType residentialLotSubType = info.ResidentialLotSubType;
                    string lotTypeName = "";

                    if (ChangeLotTypeDialogEx.Show(ref lotType, ref commercialLotSubType, ref residentialLotSubType, ref lotTypeName, info.IsHouseboatLot))
                    {
                        if (((lotType == LotType.Commercial) && (commercialLotSubType == CommercialLotSubType.kEP1_BaseCamp)) && ths.mModel.IsAnyLotBaseCamp())
                        {
                            string titleText = Common.LocalizeEAString("Ui/Caption/Global:Failed");
                            string messageText = Common.LocalizeEAString("Ui/Caption/GameEntry/EditTown/EP01:BaseCampExists");
                            SimpleMessageDialog.Show(titleText, messageText, ModalDialog.PauseMode.PauseSimulator, new Vector2(-1f, -1f), "ui_error", "ui_hardwindow_close");
                        }
                        else if (((lotType == LotType.Commercial) && (commercialLotSubType == CommercialLotSubType.kEP11_BaseCampFuture)) && ths.mModel.IsAnyLotBaseCamp())
                        {
                            string str4 = localizationModel.LocalizeString("Ui/Caption/Global:Failed", new object[0]);
                            string str5 = localizationModel.LocalizeString("Ui/Caption/GameEntry/EditTown/EP11:BaseCampFutureExists", new object[0]);
                            SimpleMessageDialog.Show(str4, str5, ModalDialog.PauseMode.PauseSimulator, new Vector2(-1f, -1f), "ui_error", "ui_hardwindow_close");
                        }
                        else if (EditTownModelEx.ChangeLotType(info.LotId, false, lotType, commercialLotSubType, residentialLotSubType))
                        {
                            EditTownController.AlertLotTypeChangeSuccess(info, localizationModel, lotType, lotTypeName);
                        }
                        else
                        {
                            string promptText = string.Empty;
                            if (commercialLotSubType == CommercialLotSubType.kEP10_Resort)
                            {
                                promptText = localizationModel.LocalizeString("Ui/Caption/GameEntry/EditTown:LotTypeResortFailed", new object[] { info.LotAddress });
                            }
                            else
                            {
                                promptText = localizationModel.LocalizeString((lotType == LotType.Commercial) ? "Ui/Caption/GameEntry/EditTown:LotTypeCommunityFailed" : "Ui/Caption/GameEntry/EditTown:LotTypeResidentialFailed", new object[] { info.LotAddress });
                            }

                            if (AcceptCancelDialog.Show(promptText))
                            {
                                if (EditTownModelEx.ChangeLotType(info.LotId, true, lotType, commercialLotSubType, residentialLotSubType))
                                {
                                    EditTownController.AlertLotTypeChangeSuccess(info, localizationModel, lotType, lotTypeName);
                                    EditTownMaptagController.Instance.ResetMaptags();
                                }
                                else
                                {
                                    string str5 = Common.LocalizeEAString("Ui/Caption/Global:Failed");
                                    promptText = Common.LocalizeEAString("Ui/Caption/GameEntry/EditTown:LotTypeChangeFailed");
                                    SimpleMessageDialog.Show(str5, promptText, ModalDialog.PauseMode.PauseSimulator, new Vector2(-1f, -1f), "ui_error", "ui_hardwindow_close");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("ChangeLotTypeTask", e);
            }
        }
    }
}
