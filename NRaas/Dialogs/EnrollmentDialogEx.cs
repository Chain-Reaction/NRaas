using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using Sims3.UI.View;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Dialogs
{
    public class EnrollmentDialogEx : EnrollmentDialog
    {
        new static Dictionary<AcademicDegreeNames, string> PartialScholarshipCreditsIconName;
        new static Dictionary<AcademicDegreeNames, string> FullScholarshipCreditsIconName;

        static EnrollmentDialogEx()
        {
            PartialScholarshipCreditsIconName = new Dictionary<AcademicDegreeNames, string>();
            
            PartialScholarshipCreditsIconName[AcademicDegreeNames.Business] = "w_credits_low_business";
            PartialScholarshipCreditsIconName[AcademicDegreeNames.Technology] = "w_credits_low_technology";
            PartialScholarshipCreditsIconName[AcademicDegreeNames.Science] = "w_credits_low_science";
            PartialScholarshipCreditsIconName[AcademicDegreeNames.FineArts] = "w_credits_low_fine_arts";
            PartialScholarshipCreditsIconName[AcademicDegreeNames.Communications] = "w_credits_low_communication";
            PartialScholarshipCreditsIconName[AcademicDegreeNames.PhysEd] = "w_credits_low_phys_ed";

            FullScholarshipCreditsIconName = new Dictionary<AcademicDegreeNames, string>();
            
            FullScholarshipCreditsIconName[AcademicDegreeNames.Business] = "w_credits_high_business";
            FullScholarshipCreditsIconName[AcademicDegreeNames.Technology] = "w_credits_high_technology";
            FullScholarshipCreditsIconName[AcademicDegreeNames.Science] = "w_credits_high_science";
            FullScholarshipCreditsIconName[AcademicDegreeNames.FineArts] = "w_credits_high_fine_arts";
            FullScholarshipCreditsIconName[AcademicDegreeNames.Communications] = "w_credits_high_communication";
            FullScholarshipCreditsIconName[AcademicDegreeNames.PhysEd] = "w_credits_high_phys_ed";
        }

        public EnrollmentDialogEx(SimDescription simDesc, IEnumerable<SimDescription> choices, bool infiniteFunds)
            : base(simDesc)
        {
            if (infiniteFunds)
            {
                mHouseholdFunds = 1000000;
                mStartHouseholdFunds = 1000000;
            }

            if (mModalDialogWindow != null)
            {
                mLeftArrow.Click -= OnArrowClick;
                mLeftArrow.Click += OnArrowClickEx;
                mRightArrow.Click -= OnArrowClick;
                mRightArrow.Click += OnArrowClickEx;
                mSelectedTable.ItemDoubleClicked -= OnGridDoubleClicked;
                mSelectedTable.ItemDoubleClicked += OnGridDoubleClickedEx;
                mSourceTable.ItemDoubleClicked -= OnGridDoubleClicked;
                mSourceTable.ItemDoubleClicked += OnGridDoubleClickedEx;
                mSelectedTable.SelectionChanged -= OnSelectionChanged;
                mSelectedTable.SelectionChanged += OnSelectionChangedEx;
                mDegreeCombo.SelectionChange -= OnDegreeChange;
                mDegreeCombo.SelectionChange += OnDegreeChangeEx;
                mCourseLoadSldr.SliderValueChange -= OnCourseLoadSliderChanged;
                mCourseLoadSldr.SliderValueChange += OnCourseLoadSliderChangedEx;

                ILocalizationModel localizationModel = Responder.Instance.LocalizationModel;
                mCurrentActor = simDesc;
                mSims = new List<PhoneSimPicker.SimPickerInfo>();
                mSelectedSims = new List<PhoneSimPicker.SimPickerInfo>();

                foreach (SimDescription description in choices)
                {
                    if (description.OccupationAsAcademicCareer != null) continue;

                    PhoneSimPicker.SimPickerInfo item = CreateSimPickerInfo(simDesc, description);
                    mSims.Add(item);
                    if (description == simDesc)
                    {
                        mSelectedSims.Add(item);
                    }
                }

                mDegreeCombo.ValueList.Clear();

                List<AcademicDegreeStaticData> list = new List<AcademicDegreeStaticData>(AcademicDegreeManager.sDictionary.Values);
                list.Sort(OnSortByName);

                foreach (AcademicDegreeStaticData data in list)
                {
                    mDegreeCombo.ValueList.Add(data.DegreeName, data);
                }

                mSourceTable.Flush();
                mSelectedTable.Flush();
                RepopulateSourceTable();
                RepopulateSelectedSimTable();
                mSelectedTable.SelectedItem = 0;
                OnSelectionChangedEx(mSelectedTable, null);
                StartEnrollmentMusic();
            }
        }

        protected static int OnSortByName(AcademicDegreeStaticData left, AcademicDegreeStaticData right)
        {
            try
            {
                string leftText = left.DegreeName;
                if (leftText == null) return -1;

                string rightText = right.DegreeName;
                if (rightText == null) return 1;

                return leftText.CompareTo(rightText);
            }
            catch (Exception e)
            {
                Common.Exception("OnSortByName", e);
                return 0;
            }
        }

        public static List<IEnrollmentData> Show(SimDescription actor, IEnumerable<SimDescription> choices, bool infiniteFunds, out int TermLen, out int totalCost)
        {
            if (actor.Household == null)
            {
                TermLen = 0;
                totalCost = 0;
                return null;
            }

            using (EnrollmentDialogEx dialog = new EnrollmentDialogEx(actor, choices, infiniteFunds))
            {
                dialog.StartModal();
                TermLen = dialog.TermLen;
                totalCost = dialog.TotalCost;

                return dialog.Result;
            }
        }
        private void OnCourseLoadSliderChangedEx(WindowBase sender, UIValueChangedEventArgs eventArgs)
        {
            try
            {
                Slider slider = sender as Slider;
                if (slider != null)
                {
                    EnrollmentDialogRowController controller;
                    mCourseLoad = (uint)slider.Value;
                    EnrollmentData enrollmentDataForSim = GetEnrollmentDataForSim(mSelectedSimDesc, out controller);
                    if (enrollmentDataForSim != null)
                    {
                        int num;
                        int num2;
                        FinancialAidtype aidtype;
                        CreditAwardtype awardtype;
                        mHouseholdFunds -= enrollmentDataForSim.CostPerSim;
                        mHouseholdFunds -= enrollmentDataForSim.ScholarshipAmountPerSim;
                        enrollmentDataForSim.CostPerSim = UpdateFunds(enrollmentDataForSim.AcademicDegreeName, mSelectedSimDesc as SimDescription, out num2, out aidtype, out num, out awardtype);
                        enrollmentDataForSim.FinancialAidType = aidtype;
                        enrollmentDataForSim.ScholarshipAmountPerSim = num2;
                        enrollmentDataForSim.CreditsAwardedPerSim = num;
                        enrollmentDataForSim.CourseLoad = mCourseLoad;
                        enrollmentDataForSim.CreditAwardType = awardtype;
                        controller.mTextControlCost.Color = (enrollmentDataForSim.CostPerSim < 0x0) ? RED : BLUE;
                        controller.Cost = UIUtils.FormatMoney(Math.Abs(enrollmentDataForSim.CostPerSim));
                        controller.Scholarship = enrollmentDataForSim.ScholarshipAmountPerSim;
                        controller.AidType = enrollmentDataForSim.FinancialAidType;

                        // Custom
                        UpdateCreditIconEx(enrollmentDataForSim.CreditsAwardedPerSim, enrollmentDataForSim.CreditAwardType, (AcademicDegreeNames)enrollmentDataForSim.AcademicDegreeName);

                        UpdateDegreeProgressBar(mSelectedSimDesc, enrollmentDataForSim.AcademicDegreeName);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnCourseLoadSliderChangedEx", e);
            }
        }

        private void OnDegreeChangeEx(WindowBase sender, UISelectionChangeEventArgs eventArgs)
        {
            try
            {
                EnrollmentDialogRowController controller;
                EnrollmentData enrollmentDataForSim = GetEnrollmentDataForSim(mSelectedSimDesc, out controller);
                if (enrollmentDataForSim != null)
                {
                    int num;
                    int num2;
                    FinancialAidtype aidtype;
                    CreditAwardtype awardtype;
                    mHouseholdFunds -= enrollmentDataForSim.CostPerSim;
                    mHouseholdFunds -= enrollmentDataForSim.ScholarshipAmountPerSim;

                    AcademicDegreeStaticData staticElement = mDegreeCombo.EntryTags[(int)mDegreeCombo.CurrentSelection] as AcademicDegreeStaticData;

                    enrollmentDataForSim.AcademicDegreeName = (ulong)staticElement.AcademicDegreeName;
                    //enrollmentDataForSim.AcademicDegreeName = (ulong)(mDegreeCombo.CurrentSelection + 0x1L);

                    enrollmentDataForSim.CostPerSim = UpdateFunds(enrollmentDataForSim.AcademicDegreeName, mSelectedSimDesc as SimDescription, out num2, out aidtype, out num, out awardtype);
                    enrollmentDataForSim.FinancialAidType = aidtype;
                    enrollmentDataForSim.ScholarshipAmountPerSim = num2;
                    enrollmentDataForSim.CreditsAwardedPerSim = num;
                    enrollmentDataForSim.CreditAwardType = awardtype;
                    enrollmentDataForSim.CourseLoad = this.mCourseLoad;
                    controller.mTextControlCost.Color = (enrollmentDataForSim.CostPerSim < 0x0) ? RED : BLUE;
                    controller.Cost = UIUtils.FormatMoney(Math.Abs(enrollmentDataForSim.CostPerSim));
                    controller.Scholarship = enrollmentDataForSim.ScholarshipAmountPerSim;
                    controller.AidType = enrollmentDataForSim.FinancialAidType;

                    mDegreeDescription.Caption = Responder.Instance.LocalizationModel.LocalizeString(staticElement.DegreeDescKey, new object[0x0]);

                    UpdateCreditIconEx(enrollmentDataForSim.CreditsAwardedPerSim, enrollmentDataForSim.CreditAwardType, (AcademicDegreeNames)enrollmentDataForSim.AcademicDegreeName);
                    //UpdateCreditIcon(enrollmentDataForSim.CreditsAwardedPerSim, (int)enrollmentDataForSim.CreditAwardType, (int)enrollmentDataForSim.AcademicDegreeName);

                    UpdateDegreeProgressBar(mSelectedSimDesc, enrollmentDataForSim.AcademicDegreeName);
                }

                UpdateAcceptAvailability();
            }
            catch(Exception e)
            {
                Common.Exception("OnDegreeChangeEx", e);
            }
        }

        private void OnArrowClickEx(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                mDegreeCombo.SelectionChange -= OnDegreeChangeEx;

                OnArrowClick(sender, eventArgs);

                UpdateSelection();
            }
            catch (Exception e)
            {
                Common.Exception("OnArrowClickEx", e);
            }
        }

        private void OnGridDoubleClickedEx(TableContainer sender, TableRow row)
        {
            try
            {
                mDegreeCombo.SelectionChange -= OnDegreeChangeEx;

                OnGridDoubleClicked(sender, row);

                UpdateSelection();
            }
            catch (Exception e)
            {
                Common.Exception("OnGridDoubleClickedEx", e);
            }
        }

        private void OnSelectionChangedEx(WindowBase sender, UISelectionChangeEventArgs eventArgs)
        {
            try
            {
                mDegreeCombo.SelectionChange -= OnDegreeChangeEx;

                OnSelectionChanged(sender, eventArgs);

                UpdateSelection();
            }
            catch (Exception e)
            {
                Common.Exception("OnSelectionChangedEx", e);
            }
        }

        private void UpdateSelection()
        {
            bool select = false;

            SimDescription sim = mSelectedSimDesc as SimDescription;
            if (sim != null)
            {
                select = true;

                if ((sim.ChildOrBelow) || (sim.IsPet))
                {
                    select = false;
                }
            }

            mDegreeCombo.Enabled = select;
            mCourseLoadSldr.Enabled = select;

            EnrollmentDialogRowController controller;
            EnrollmentData enrollmentDataForSim = GetEnrollmentDataForSim(mSelectedSimDesc, out controller);
            if (enrollmentDataForSim != null)
            {
                if (!select)
                {
                    if (enrollmentDataForSim.CourseLoad != 0)
                    {
                        mCourseLoad = 0;

                        int num;
                        int num2;
                        FinancialAidtype aidtype;
                        CreditAwardtype awardtype;
                        mHouseholdFunds -= enrollmentDataForSim.CostPerSim;
                        mHouseholdFunds -= enrollmentDataForSim.ScholarshipAmountPerSim;
                        enrollmentDataForSim.CostPerSim = UpdateFunds(enrollmentDataForSim.AcademicDegreeName, mSelectedSimDesc as SimDescription, out num2, out aidtype, out num, out awardtype);
                        enrollmentDataForSim.FinancialAidType = aidtype;
                        enrollmentDataForSim.ScholarshipAmountPerSim = num2;
                        enrollmentDataForSim.CreditsAwardedPerSim = num;
                        enrollmentDataForSim.CourseLoad = mCourseLoad;
                        enrollmentDataForSim.CreditAwardType = awardtype;
                        controller.mTextControlCost.Color = (enrollmentDataForSim.CostPerSim < 0) ? RED : BLUE;
                        controller.Cost = UIUtils.FormatMoney(Math.Abs(enrollmentDataForSim.CostPerSim));
                        controller.Scholarship = enrollmentDataForSim.ScholarshipAmountPerSim;
                        controller.AidType = enrollmentDataForSim.FinancialAidType;

                        UpdateCreditIconEx(enrollmentDataForSim.CreditsAwardedPerSim, enrollmentDataForSim.CreditAwardType, (AcademicDegreeNames)enrollmentDataForSim.AcademicDegreeName);
                        UpdateDegreeProgressBar(mSelectedSimDesc, enrollmentDataForSim.AcademicDegreeName);
                    }
                }
                else
                {
                    uint index = 0;
                    for (int i = 0; i < mDegreeCombo.ValueList.Count; i++)
                    {
                        AcademicDegreeStaticData data = mDegreeCombo.EntryTags[i] as AcademicDegreeStaticData;
                        if ((ulong)data.AcademicDegreeName == enrollmentDataForSim.AcademicDegreeName)
                        {
                            index = (uint)i;
                            break;
                        }
                    }

                    mDegreeCombo.SelectionChange -= OnDegreeChange;
                    mDegreeCombo.CurrentSelection = index;
                }
            }

            mDegreeCombo.SelectionChange -= OnDegreeChange;
            mDegreeCombo.SelectionChange += OnDegreeChangeEx;
        }

        private void UpdateCreditIconEx(int CreditsAwardedPerSim, CreditAwardtype creditAwardType, AcademicDegreeNames degreeName)
        {
            if (CreditsAwardedPerSim > 0x0)
            {
                mCreditIconWin.TooltipText = Responder.Instance.LocalizationModel.LocalizeString("Ui/Dialog/Enrollment:CreditToolTip" + ((int)degreeName).ToString() + ((creditAwardType <= 0x0) ? "Partial" : "Full"), new object[] { CreditsAwardedPerSim * 0x6 });
                switch (creditAwardType)
                {
                    case CreditAwardtype.LowCreditAward:
                        if (PartialScholarshipCreditsIconName.ContainsKey(degreeName))
                        {
                            mCreditIconWin.SetImage(ResourceKey.CreatePNGKey(PartialScholarshipCreditsIconName[degreeName], ResourceUtils.ProductVersionToGroupId(ProductVersion.EP9)));
                        }
                        return;

                    case CreditAwardtype.HighCreditAward:
                        if (FullScholarshipCreditsIconName.ContainsKey(degreeName))
                        {
                            mCreditIconWin.SetImage(ResourceKey.CreatePNGKey(FullScholarshipCreditsIconName[degreeName], ResourceUtils.ProductVersionToGroupId(ProductVersion.EP9)));
                        }
                        return;
                }
            }
            else
            {
                mCreditIconWin.SetImage(ResourceKey.kInvalidResourceKey);
                mCreditIconWin.TooltipText = Responder.Instance.LocalizationModel.LocalizeString("Ui/Dialog/Enrollment:NoCreditToolTip", new object[0x0]);
            }
        }
    }
}

