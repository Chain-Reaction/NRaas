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
    public class EditTownInfoPanelEx
    {
        public static void OnSwitchToClick(WindowBase w, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                eventArgs.Handled = true;

                Common.FunctionTask.Perform(SwitchToClick);
            }
            catch (Exception e)
            {
                Common.Exception("OnSwitchToClick", e);
            }
        }

        private static void SwitchToClick()
        {
            if (!Sims3.UI.Responder.Instance.OptionsModel.SaveGameInProgress)
            {
                EditTownInfoPanel ths = EditTownInfoPanel.Instance;

                bool flag = true;
                if ((ths.mModel.ValidActiveHousehold) && (!Mover.Settings.mDreamCatcher))
                {
                    if (!AcceptCancelDialog.Show(Common.LocalizeEAString("Ui/Caption/GameEntry/EditTown:ChangeActiveHousehold")))
                    {
                        flag = false;
                    }
                    else if (!AcceptCancelDialog.Show(Common.LocalizeEAString("Ui/Caption/GameEntry/EditTown:LoseWPWarning")))
                    {
                        flag = false;
                    }
                }

                if ((flag && (ths.mInfo != null)) && (!ths.mModel.IsPlaceLotsWizardFlow || AcceptCancelDialog.Show(Common.LocalizeEAString("Ui/Caption/GameEntry/PlaceEPLotsWizard:CancelPrompt"))))
                {
                    EditTownPuck.Instance.UpdateBackButton(true);
                    ulong householdId = ths.mInfo.HouseholdId;
                    ths.Visible = false;

                    try
                    {
                        ths.mModel.IsSwitchingHouseholds = true;
                        if (!ths.mModel.ExitEditTown(false))
                        {
                            ths.Visible = true;
                            EditTownPuck.Instance.UpdateBackButton(false);
                        }
                        else
                        {
                            // Custom
                            PlayFlowModelEx.ActivateSimScreenMaskedFunc(Sims3.UI.Responder.Instance.PlayFlowModel as PlayFlowModel, householdId);

                            Audio.StartSound("ui_softwindow_close");
                            ths.mModel.IsPlaceLotsWizardFlow = false;
                        }
                    }
                    finally
                    {
                        ths.mModel.IsSwitchingHouseholds = false;
                    }
                }
            }
        }

        public static void OnMergeClick(WindowBase w, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                EditTownInfoPanel ths = EditTownInfoPanel.Instance;

                if (!(EditTownMergeTool.sInstance is EditTownMergeToolEx))
                {
                    EditTownMergeTool.sInstance = new EditTownMergeToolEx();
                }

                EditTownMergeTool.Instance.SetHouseholdToMergeFrom(ths.mInfo);
                EditTownTool.CurrentTool = EditTownMergeTool.Instance;
            }
            catch (Exception e)
            {
                Common.Exception("OnMergeClick", e);
            }
        }

        public static void OnSplitClick(WindowBase w, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                GameEntryMovingModelEx.SplitHousehold(EditTownController.Instance, EditTownInfoPanel.sInstance.mInfo);
            }
            catch (Exception e)
            {
                Common.Exception("OnSplitClick", e);
            }
        }

        public static void OnPlaceClick(WindowBase w, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                EditTownInfoPanel ths = EditTownInfoPanel.Instance;

                if (ths.mInfo != null)
                {
                    if (!(EditTownPlaceTool.sInstance is EditTownPlaceToolEx))
                    {
                        EditTownPlaceTool.sInstance = new EditTownPlaceToolEx();
                    }

                    UIBinInfo mInfo = ths.mInfo;
                    InfoSource mSource = ths.mSource;
                    EditTownPlaceTool.Instance.SetItemToPlace(ths.mInfo, ths.mSource);
                    ths.mTriggerSound = false;
                    try
                    {
                        EditTownTool.CurrentTool = EditTownPlaceTool.Instance;
                        ths.mModel.SetCurrentSelection(mInfo, mSource);
                    }
                    finally
                    {
                        ths.mTriggerSound = true;
                    }

                    if (ths.Visible)
                    {
                        Audio.StartSound("ui_softwindow_close");
                    }

                    ths.Visible = false;
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnPlaceClick", e);
            }
        }
    }
}
