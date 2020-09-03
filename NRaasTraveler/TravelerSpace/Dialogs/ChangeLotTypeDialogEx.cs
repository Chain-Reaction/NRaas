using NRaas.CommonSpace.Helpers;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.TravelerSpace.Dialogs
{
    public class ChangeLotTypeDialogEx : EditTownController.ChangeLotTypeDialog
    {
        public ChangeLotTypeDialogEx(LotType currentLotType, CommercialLotSubType currentCommercialSubType, ResidentialLotSubType currentResidentialSubType, Vector2 position, ModalDialog.PauseMode pauseMode, bool isHouseboatLot)
            : base(currentLotType, currentCommercialSubType, currentResidentialSubType, position, pauseMode, isHouseboatLot)
        {
            PopulateComboBox(currentLotType == LotType.Residential, isHouseboatLot);

            Button childByID = mModalDialogWindow.GetChildByID(3, true) as Button;
            childByID.Click -= OnLotTypeClick;
            childByID.Click += OnLotTypeClickEx;

            childByID = mModalDialogWindow.GetChildByID(2, true) as Button;
            childByID.Click -= OnLotTypeClick;
            childByID.Click += OnLotTypeClickEx;
        }

        private void OnLotTypeClickEx(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                eventArgs.Handled = true;
                PopulateComboBox(sender.ID == 2, bIsHouseboatLot);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception("OnLotTypeClick", e);
            }
        }

        protected static int OnSort(KeyValuePair<ResidentialLotSubType, string> left, KeyValuePair<ResidentialLotSubType, string> right)
        {
            if (left.Value == null) return -1;
            if (right.Value == null) return 1;

            return left.Value.CompareTo(right.Value);
        }
        protected static int OnSort(KeyValuePair<CommercialLotSubType, string> left, KeyValuePair<CommercialLotSubType, string> right)
        {
            if (left.Value == null) return -1;
            if (right.Value == null) return 1;

            return left.Value.CompareTo(right.Value);
        }

        private new void PopulateComboBox(bool isResidential, bool isHouseboat)
        {
            if (isResidential)
            {
                mNewLotType = LotType.Residential;
                mCombo.ValueList.Clear();

                List<KeyValuePair<ResidentialLotSubType, string>> list = new List<KeyValuePair<ResidentialLotSubType,string>>(Responder.Instance.EditTownModel.ResidentialSubTypes);
                list.Sort(OnSort);

                foreach (KeyValuePair<ResidentialLotSubType, string> pair in list)
                {
                    if (pair.Key == mCurrentResidentialSubType)
                    {
                        mCombo.ValueList.Add(pair.Value, pair.Key);
                        mCombo.CurrentSelection = (uint)(mCombo.ValueList.Count - 1);
                    }
                    else if (pair.Key != ResidentialLotSubType.kEP10_PrivateLot)
                    {
                        mCombo.ValueList.Add(pair.Value, pair.Key);
                    }
                }

                mOkButton.Enabled = true;
            }
            else
            {
                mNewLotType = LotType.Commercial;
                mCombo.ValueList.Clear();

                if (mCurrentCommercialSubType == CommercialLotSubType.kCommercialUndefined)
                {
                    mCombo.ValueList.Add(Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/EditTown/ChangeLotType:ChooseLotType", new object[0]), CommercialLotSubType.kCommercialUndefined);
                    mCombo.CurrentSelection = (uint)(mCombo.ValueList.Count - 1);
                    mOkButton.Enabled = false;
                }

                List<KeyValuePair<CommercialLotSubType, string>> list = new List<KeyValuePair<CommercialLotSubType, string>>(Responder.Instance.EditTownModel.GetCommercialSubTypes(false, isHouseboat));
                list.Sort(OnSort);

                foreach (KeyValuePair<CommercialLotSubType, string> pair in list)
                {
                    mCombo.ValueList.Add((pair.Key == CommercialLotSubType.kEP11_BaseCampFuture ? pair.Value + " (" + ProductVersions.GetLocalizedName(ProductVersion.EP11) + ")" : pair.Value), pair.Key);
                    if (pair.Key == mCurrentCommercialSubType)
                    {
                        mCombo.CurrentSelection = (uint)(mCombo.ValueList.Count - 1);
                        mOkButton.Enabled = true;
                    }
                }
            }
            mCombo.CurrentSelection = mCombo.CurrentSelection;
            mCombo.Invalidate();
            mCombo.Enabled = mCombo.ValueList.Count > 1;
        }

        public static new bool Show(ref LotType currentType, ref CommercialLotSubType currentCommercialSubType, ref ResidentialLotSubType currentResidentialSubType, ref string lotTypeName, bool isHouseboatLot)
        {
            return Show(ref currentType, ref currentCommercialSubType, ref currentResidentialSubType, ref lotTypeName, new Vector2(-1f, -1f), ModalDialog.PauseMode.PauseSimulator, isHouseboatLot);
        }
        public static new bool Show(ref LotType currentType, ref CommercialLotSubType currentCommercialSubType, ref ResidentialLotSubType currentResidentialSubType, ref string lotTypeName, Vector2 position, ModalDialog.PauseMode pauseMode, bool isHouseboatLot)
        {
            if (ModalDialog.EnableModalDialogs)
            {
                using (ChangeLotTypeDialogEx dialog = new ChangeLotTypeDialogEx(currentType, currentCommercialSubType, currentResidentialSubType, position, pauseMode, isHouseboatLot))
                {
                    dialog.StartModal();
                    currentType = dialog.mCurrentLotType;
                    currentCommercialSubType = dialog.mCurrentCommercialSubType;
                    currentResidentialSubType = dialog.mCurrentResidentialSubType;
                    lotTypeName = dialog.mLotTypeName;
                    return dialog.Result;
                }
            }
            return false;
        }
    }
}