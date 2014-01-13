using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.MasterControllerSpace.Dialogs
{
    public class PartyPickerDialogEx : PartyPickerDialog
    {
        public PartyPickerDialogEx(PartyType partyTypes, List<PhoneSimPicker.SimPickerInfo> sims, ThumbnailKey simThumb, bool isPartyAtHome, float curfewStart, float curfewEnd, float venueOpenTime, float venueCloseTime, ClothingType restrictClothingTypeTo, bool isHostFemale)
            : base(AdjustedTypes(partyTypes), sims, simThumb, isPartyAtHome, curfewStart, curfewEnd, venueOpenTime, venueCloseTime, restrictClothingTypeTo, isHostFemale)
        {
            mPartyTypeCombo.ValueList.Clear();

            if ((PartyType.kHouseParty & partyTypes) != PartyType.kNone)
            {
                if (isPartyAtHome)
                {
                    mPartyTypeCombo.ValueList.Add(Localization.LocalizeString("Ui/Caption/Party:House", new object[0]), PartyType.kHouseParty);
                }
                else
                {
                    mPartyTypeCombo.ValueList.Add(Localization.LocalizeString("Ui/Caption/Party:Street", new object[0]), PartyType.kHouseParty);
                }
            }
            if ((PartyType.kWedding & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Localization.LocalizeString("Ui/Caption/Party:Wedding", new object[0]), PartyType.kWedding);
            }
            if ((PartyType.kBirthday & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Localization.LocalizeString("Ui/Caption/Party:Birthday", new object[0]), PartyType.kBirthday);
            }
            if ((PartyType.kFuneral & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Localization.LocalizeString("Ui/Caption/Party:Funeral", new object[0]), PartyType.kFuneral);
            }
            if ((PartyType.kCampaign & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Localization.LocalizeString("Ui/Caption/Party:Campaign", new object[0]), PartyType.kCampaign);
            }
            if ((PartyType.kBachelorParty & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Localization.LocalizeString(isHostFemale, "Ui/Caption/Party:Bachelor", new object[0]), PartyType.kBachelorParty);
            }
            if ((PartyType.kTeenParty & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Localization.LocalizeString("Ui/Caption/Party:Teen", new object[0]), PartyType.kTeenParty);
            }
            if ((PartyType.kChildSlumberParty & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Localization.LocalizeString(isHostFemale, "Ui/Caption/Party:Slumber", new object[0]), PartyType.kChildSlumberParty);
            }
            if ((PartyType.kTeenSlumberParty & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Localization.LocalizeString(isHostFemale, "Ui/Caption/Party:Slumber", new object[0]), PartyType.kTeenSlumberParty);
            }
            if ((PartyType.kPoolParty & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Localization.LocalizeString(isHostFemale, "Ui/Caption/Party:Pool", new object[0]), PartyType.kPoolParty);
            }
            if ((PartyType.kFeastParty & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Localization.LocalizeString(isHostFemale, "Ui/Caption/Party:Feast", new object[0]), PartyType.kFeastParty);
            }
            if ((PartyType.kCostumeParty & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Localization.LocalizeString(isHostFemale, "Ui/Caption/Party:CostumeParty", new object[0]), PartyType.kCostumeParty);
            }
            if ((PartyType.kGiftGivingParty & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Localization.LocalizeString(isHostFemale, "Ui/Caption/Party:GiftGivingParty", new object[0]), PartyType.kGiftGivingParty);
            }
            if ((PartyType.kJuiceKeggerParty & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Localization.LocalizeString(isHostFemale, "Ui/Caption/Party:JuiceKegger", new object[0]), PartyType.kJuiceKeggerParty);
            }
            if ((PartyType.kTailgatingParty & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Common.Localize("PartyType:Tailgating", isHostFemale), PartyType.kTailgatingParty);
            }
            if ((PartyType.kBonfire & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Localization.LocalizeString(isHostFemale, "Ui/Caption/Party:Bonfire", new object[0]), PartyType.kBonfire);
            }
            if ((PartyType.kVideoGameLANParty & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Common.Localize("PartyType:LANParty", isHostFemale), PartyType.kVideoGameLANParty);
            }
            if ((PartyType.kMasqueradeBall & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Common.Localize("PartyType:MasqueradeBall", isHostFemale), PartyType.kMasqueradeBall);
            }
            if ((PartyType.kVictoryParty & partyTypes) != PartyType.kNone)
            {
                mPartyTypeCombo.ValueList.Add(Common.Localize("PartyType:VictoryParty", isHostFemale), PartyType.kVictoryParty);
            }

            mPartyTypeCombo.SelectionChange -= OnTypeChange;
            mPartyTypeCombo.SelectionChange += OnTypeChangeEx;

            mLeftArrow.Click -= OnArrowClick;
            mLeftArrow.Click += OnArrowClickEx;

            mRightArrow.Click -= OnArrowClick;
            mRightArrow.Click += OnArrowClickEx;

            Button button = mModalDialogWindow.GetChildByID(0x5ef6bd7, true) as Button;
            button.Click -= OnFilterClick;
            button.Click += OnFilterClickEx;

            button = mModalDialogWindow.GetChildByID(0x5ef6bd8, true) as Button;
            button.Click -= OnFilterClick;
            button.Click += OnFilterClickEx;

            mInviteesTable.ItemDoubleClicked -= OnGridDoubleClicked;
            mInviteesTable.ItemDoubleClicked += OnGridDoubleClickedEx;

            mSourceTable.ItemDoubleClicked -= OnGridDoubleClicked;
            mSourceTable.ItemDoubleClicked += OnGridDoubleClickedEx;
        }

        public static PartyType AdjustedTypes(PartyType partyTypes)
        {
            PartyType result = partyTypes;

            partyTypes &= ~(PartyType.kTailgatingParty | PartyType.kVideoGameLANParty | PartyType.kMasqueradeBall | PartyType.kVictoryParty);

            return result;
        }

        private void RepopulateSourceTableEx()
        {
            int num = -1;
            int column = -1;

            mSourceTable.GetFirstVisibleCell(ref column, ref num);
            mSourceTable.Clear();
            foreach (PhoneSimPicker.SimPickerInfo info in mSims)
            {
                ISimDescription simDescription = info.SimDescription as ISimDescription;

                bool enabled = true;

                if (MasterController.Settings.mPartyAgeFilter)
                {
                    switch (this.Result.mPartyType)
                    {
                        case PartyType.kChildSlumberParty:
                            enabled = simDescription.Child;
                            break;

                        case PartyType.kTeenSlumberParty:
                        case PartyType.kTeenParty:
                            enabled = simDescription.Teen;
                            break;

                        case PartyType.kJuiceKeggerParty:
                        case PartyType.kBachelorParty:
                        case PartyType.kTailgatingParty:
                        case PartyType.kMasqueradeBall:
                        case PartyType.kVictoryParty:
                            enabled = simDescription.YoungAdultOrAbove;
                            break;

                        case PartyType.kBonfire:
                        case PartyType.kVideoGameLANParty:
                            enabled = simDescription.ChildOrAbove;
                            break;
                    }
                }

                if (((mFilter != FilterType.kCoworkers) || info.CoWorker) && ((mFilter != FilterType.kFriends) || info.Friend))
                {
                    TableRow row = mSourceTable.CreateRow();
                    PartyPickerRowController controller = new PartyPickerRowController(row, mSourceTable, info);
                    row.RowController = controller;
                    controller.Enabled = enabled;
                    mSourceTable.AddRow(row);
                }
            }
            mSourceTable.ClearSelection();
            mRightArrow.Enabled = false;
            mSourceTable.Flush();
            mSourceTable.ScrollRowToTop(num);
        }

        private void RepopulateTargetTableEx()
        {
            mInviteesTable.Clear();
            mNumChildrenInvited = 0x0;

            foreach (PhoneSimPicker.SimPickerInfo info in mInvitees)
            {
                ISimDescription simDescription = info.SimDescription as ISimDescription;
                TableRow row = mInviteesTable.CreateRow();
                PartyPickerRowController controller = new PartyPickerRowController(row, mInviteesTable, info);
                row.RowController = controller;
                mInviteesTable.AddRow(row);
                if ((simDescription != null) && simDescription.ChildOrBelow)
                {
                    mNumChildrenInvited++;
                }
            }
            ValidateParty();
        }

        private void OnTypeChangeEx(WindowBase sender, UISelectionChangeEventArgs eventArgs)
        {
            ComboBox box = sender as ComboBox;
            if (box != null)
            {
                mResult.PartyType = (PartyType)box.EntryTags[(int)eventArgs.NewIndex];
            }
            if (mResult.mPartyType != PartyType.kNone)
            {
                mClothingCombo.Enabled = true;
                mDressCodeText.TextColor = new Color(0xff000000);
                mResult.ClothingType = mRestrictClothingTypeTo;
                PopulateClothingCombo();
                RepopulateTargetTableEx();
                RepopulateSourceTableEx();
                UpdateSourceAvailability();
                ValidateParty();
                eventArgs.Handled = true;
            }
            else
            {
                mOkayButton.Enabled = false;
            }
        }

        private void OnArrowClickEx(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            if (sender.ID == 0x5ef6bd4)
            {
                PartyPickerRowController rowController = mInviteesTable.GetRow(mInviteesTable.SelectedItem).RowController as PartyPickerRowController;
                PhoneSimPicker.SimPickerInfo item = rowController.Info;
                mInvitees.Remove(item);
                mSims.Add(item);
            }
            else
            {
                PartyPickerRowController controller2 = mSourceTable.GetRow(mSourceTable.SelectedItem).RowController as PartyPickerRowController;
                PhoneSimPicker.SimPickerInfo info = controller2.Info;
                mSims.Remove(info);
                mInvitees.Add(info);
            }
            UpdateSourceAvailability();
            mLeftArrow.Enabled = false;
            mRightArrow.Enabled = false;
            RepopulateSourceTableEx();
            RepopulateTargetTableEx();
            ValidateParty();
        }

        private void OnFilterClickEx(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            mFilterLabel.Caption = sender.TooltipText;
            switch (sender.ID)
            {
                case 0x5ef6bd6:
                    mFilter = FilterType.kAll;
                    break;

                case 0x5ef6bd7:
                    mFilter = FilterType.kFriends;
                    break;

                case 0x5ef6bd8:
                    mFilter = FilterType.kCoworkers;
                    break;
            }
            RepopulateSourceTableEx();
            eventArgs.Handled = true;
        }

        private void OnGridDoubleClickedEx(TableContainer sender, TableRow row)
        {
            bool flag = false;
            if (sender.ID == 0x5ef6bd0)
            {
                PartyPickerRowController rowController = row.RowController as PartyPickerRowController;
                PhoneSimPicker.SimPickerInfo item = rowController.Info;
                mInvitees.Remove(item);
                mSims.Add(item);
                flag = true;
            }
            else if (mInviteesTable.NumberRows < mMaxGuests)
            {
                PartyPickerRowController controller2 = row.RowController as PartyPickerRowController;
                if (controller2.Enabled)
                {
                    PhoneSimPicker.SimPickerInfo info = controller2.Info;
                    mSims.Remove(info);
                    mInvitees.Add(info);
                    flag = true;
                }
            }
            if (flag)
            {
                Audio.StartSound("ui_panel_shift");
                mLeftArrow.Enabled = false;
                mRightArrow.Enabled = false;
                UpdateSourceAvailability();
                RepopulateSourceTableEx();
                RepopulateTargetTableEx();
                ValidateParty();
            }
        }

        public new static PartyInfo Show(PartyType partyTypes, List<PhoneSimPicker.SimPickerInfo> sims, ThumbnailKey simThumb, bool isPartyAtHome, float curfewStart, float curfewEnd, float venueOpenTime, float venueCloseTime, ClothingType restrictClothingTypeTo, bool isHostFemale)
        {
            int originalMax = kDefaultMaxAllowed;
            kDefaultMaxAllowed = int.MaxValue;

            try
            {
                using (PartyPickerDialogEx dialog = new PartyPickerDialogEx(partyTypes, sims, simThumb, isPartyAtHome, curfewStart, curfewEnd, venueOpenTime, venueCloseTime, restrictClothingTypeTo, isHostFemale))
                {
                    dialog.StartModal();
                    if (dialog.Result == null)
                    {
                        return null;
                    }
                    return dialog.Result;
                }
            }
            finally
            {
                kDefaultMaxAllowed = originalMax;
            }
        }
    }
}

