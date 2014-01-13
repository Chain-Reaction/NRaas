using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.CAS
{
    public interface ICASFacialBlendPanelAdjustment
    {
        void Reset();

        bool Perform();

        void ClosingDown();
    }

    public abstract class CASFacialBlendPanelAdjustment<T> : ICASFacialBlendPanelAdjustment
        where T : CASFacialBlendPanel, ICASUINode
    {
        Dictionary<Slider, Pair<Text, string>> mSlidersToText = new Dictionary<Slider, Pair<Text, string>>();

        bool mPerformed = false;

        static bool sCoreChecked = false;
        static bool sCoreHandled = false;

        public CASFacialBlendPanelAdjustment()
        { }

        protected abstract T GetPanel();

        protected abstract CASPhysicalState GetState();

        protected abstract List<CategoryGrids> GetCategories(T panel);

        protected abstract FacialBlendRegions GetRegion();

        protected abstract void SetAdvanced(T panel);

        public void Reset()
        {
            mPerformed = false;
        }

        public bool Perform()
        {
            if (mPerformed) return true;

            T panel = GetPanel();
            if (panel == null) return false;

            if (!MasterController.Settings.mOverrideCoreSliders)
            {
                if (!sCoreChecked)
                {
                    sCoreChecked = true;
                    sCoreHandled = (panel.mSliderData.Length != 25);
                }

                if (sCoreHandled) return false;
            }

            if (CASController.Singleton.CurrentState.mPhysicalState != GetState()) return false;

            panel.UINodeShutdown -= UINodeShutdownCallback;
            panel.UINodeShutdown += UINodeShutdownCallback;

            mPerformed = true;

            List<CategoryGrids> categories = GetCategories(panel);

            if (categories != null)
            {
                if (panel.mSliderData.Length == 25)
                {
                    panel.mSliderData = new CASFacialBlendPanel.SliderData[100];
                }

                mSlidersToText.Clear();

                foreach (CategoryGrids category in categories)
                {
                    category.mGrid.Clear();
                }

                foreach (CategoryGrids category in categories)
                {
                    PopulateSliderGrid(panel, category.mGrid, GetRegion(), category.mCategories);
                }
            }

            SetAdvanced(panel);

            return true;
        }

        public void ClosingDown()
        {
            T panel = GetPanel();
            if (panel == null) return;

            panel.UINodeShutdown -= UINodeShutdownCallback;
        }

        public void UINodeShutdownCallback(CASState newState)
        {
            T panel = GetPanel();
            if (panel == null) return;

            if (newState.mPhysicalState != GetState())
            {
                panel.UINodeShutdown -= UINodeShutdownCallback;

                mPerformed = false;
            }
        }

        public static int OnBlendSort(FacialBlendData left, FacialBlendData right)
        {
            string leftText = StringTable.GetLocalizedString(left.mLocalizationKey);
            string rightText = StringTable.GetLocalizedString(right.mLocalizationKey);

            if ((leftText == null) || (rightText == null)) return 0;

            leftText = leftText.ToLower().Trim();
            rightText = rightText.ToLower().Trim();

            return leftText.CompareTo(rightText);
        }

        private bool PopulateSliderGrid(CASFacialBlendPanel ths, ItemGrid grid, FacialBlendRegions regionFilter, List<FacialBlendCategories> categoryFilter)
        {
            ResourceKey itemKey = ResourceKey.CreateUILayoutKey("CASBodySlider", 0x0);

            List<FacialBlendData> data = new List<FacialBlendData>();

            List<BlendUnit> list = new List<BlendUnit>(CASController.Singleton.BlendUnits);
            foreach (BlendUnit unit in list)
            {
                if ((unit.Region == regionFilter) && (categoryFilter.Contains(unit.Category)))
                {
                    FacialBlendData facialBlendData = ths.GetFacialBlendData(unit);
                    if (facialBlendData == null) continue;

                    data.Add(facialBlendData);
                }
            }

            data.Sort(new Comparison<FacialBlendData>(OnBlendSort));

            foreach (FacialBlendData facialBlendData in data)
            {
                AddSliderGridItem(ths, grid, itemKey, facialBlendData);
            }

            grid.ForceCellTooltips = CASController.Singleton.DebugTooltips;
            if (grid.Count > grid.VisibleRows)
            {
                Window childByID = grid.GetChildByID(0x62ec8d0, false) as Window;
                if (childByID != null)
                {
                    childByID.Visible = true;
                }
            }

            if (grid.Count == 0x0)
            {
                grid.Clear();
            }
            return true;
        }

        public static string GetTitle(string caption, float value)
        {
            string title = caption;

            if (!string.IsNullOrEmpty(title))
            {
                title = title.Trim();
            }

            if (MasterController.Settings.mShowCASSliderValues)
            {
                title += " (" + EAText.GetNumberString(value) + ")";
            }

            return title;
        }

        public void OnSliderChanged(WindowBase sender, UIValueChangedEventArgs eventArgs)
        {
            try
            {
                GetPanel().OnSliderChanged(sender, eventArgs);

                Slider slider = sender as Slider;
                if (slider != null)
                {
                    Pair<Text, string> caption;
                    if (mSlidersToText.TryGetValue(slider, out caption))
                    {
                        caption.First.Caption = GetTitle(caption.Second, slider.Value);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSliderChanged", e);
            }
        }

        public void OnSliderMouseUp(WindowBase sender, UIMouseEventArgs args)
        {
            try
            {
                GetPanel().OnSliderMouseUp(sender, args);

                if ((args.Modifiers & Modifiers.kModifierMaskShift) != 0)
                {
                    Slider slider = sender as Slider;
                    if (slider == null) return;

                    new InputSliderDataTask(this, slider).AddToSimulator();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSliderMouseUp", e);
            }
        }

        private void AddSliderGridItem(CASFacialBlendPanel ths, ItemGrid grid, ResourceKey itemKey, FacialBlendData sliderData)
        {
            WindowBase windowByExportID = UIManager.LoadLayout(itemKey).GetWindowByExportID(0x1);
            if (windowByExportID != null)
            {
                string caption = StringTable.GetLocalizedString(sliderData.mLocalizationKey);

                int dataValue = (int)Math.Round((double)(sliderData.Value * 256f));

                Text childByID = windowByExportID.GetChildByID(0x2, true) as Text;
                if (childByID != null)
                {
                    childByID.Caption = GetTitle(caption, dataValue);
                }

                Slider slider = windowByExportID.GetChildByID(0x4, true) as Slider;
                if (slider != null)
                {
                    slider.SliderValueChange += OnSliderChanged;
                    slider.MouseUp += OnSliderMouseUp;

                    mSlidersToText.Add(slider, new Pair<Text, string>(childByID, caption));

                    if (sliderData.mBidirectional)
                    {
                        slider.MinValue = -256 * NRaas.MasterController.Settings.mSliderMultiple;
                    }
                    else
                    {
                        slider.MinValue = 0x0;
                    }

                    slider.MaxValue = 256 * NRaas.MasterController.Settings.mSliderMultiple;

                    slider.Value = dataValue;
                    if (CASController.Singleton.DebugTooltips)
                    {
                        windowByExportID.TooltipText = StringTable.GetLocalizedString(sliderData.mLocalizationKey);
                    }

                    grid.AddItem(new ItemGridCellItem(windowByExportID, sliderData));
                    if (ths.mNumSliders < ths.mSliderData.Length)
                    {
                        ths.mSliderData[ths.mNumSliders].mSlider = slider;
                        ths.mSliderData[ths.mNumSliders].mData = sliderData;
                        ths.mNumSliders++;
                    }
                }
            }
        }

        public class CategoryGrids
        {
            public readonly List<FacialBlendCategories> mCategories;

            public readonly ItemGrid mGrid;

            public CategoryGrids(FacialBlendCategories category, ItemGrid grid)
            {
                mGrid = grid;
                mCategories = new List<FacialBlendCategories>();
                mCategories.Add(category);
            }
            public CategoryGrids(FacialBlendCategories[] categories, ItemGrid grid)
            {
                mGrid = grid;
                mCategories = new List<FacialBlendCategories>(categories);
            }
        }

        public class InputSliderDataTask : Common.FunctionTask
        {
            CASFacialBlendPanelAdjustment<T> mPanel;

            Slider mSlider;

            public InputSliderDataTask(CASFacialBlendPanelAdjustment<T> panel, Slider slider)
            {
                mPanel = panel;
                mSlider = slider;
            }

            protected override void OnPerform()
            {
                Pair<Text, string> caption;
                if (mPanel.mSlidersToText.TryGetValue(mSlider, out caption))
                {
                    string text = StringInputDialog.Show(caption.Second, caption.Second, mSlider.Value.ToString());
                    if (!string.IsNullOrEmpty(text))
                    {
                        int value = 0;
                        if (int.TryParse(text, out value))
                        {
                            mSlider.Value = value;

                            mPanel.OnSliderChanged(mSlider, new UIValueChangedEventArgs());
                            mPanel.OnSliderMouseUp(mSlider, new UIMouseEventArgs());
                        }
                    }
                }
            }
        }
    }
}
