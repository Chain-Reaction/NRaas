using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.MasterControllerSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Store;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.CAS
{
    public class CASClothingCategoryEx
    {
        static Tracer sTracer = new Tracer();

        static List<CASClothingRow> sRows = new List<CASClothingRow>();

        static Dictionary<CASClothingRow, bool> sCompleted = new Dictionary<CASClothingRow, bool>();

        public static void OnClothingGridFinishedPopulating()
        {
            try
            {
                sTracer.Perform();
            }
            catch (Exception e)
            {
                Common.Exception("OnClothingGridFinishedPopulating", e);
            }
        } 

        public static void OnUndoRedo()
        {
            try
            {
                CASClothingCategory ths = CASClothingCategory.gSingleton;
                if (ths == null) return;

                if (ths.mClothingTypesGrid == null) return;

                ths.mClothingTypesGrid.SelectedItem = -1;
                List<ItemGridCellItem> items = ths.mClothingTypesGrid.Items;
                int num = 0x0;
                if (ths.mCurrentFocusedRow != null)
                {
                    ths.mCurrentFocusedRow.SetArrowGlow(false);
                }

                ths.mCurrentFocusedRow = null;
                int sAccessoriesSelection = CASClothingCategory.sAccessoriesSelection;
                foreach (ItemGridCellItem item in items)
                {
                    CASClothingRow mWin = item.mWin as CASClothingRow;
                    if (mWin != null)
                    {
                        if (mWin.SelectedItem != -1)
                        {
                            // Custom
                            bool compactCAS = false;
                            if (ths.mCurrentPart == BodyTypes.Accessories)
                            {
                                compactCAS = MasterController.Settings.mCompactAccessoryCAS;
                            }
                            else
                            {
                                compactCAS = MasterController.Settings.mCompactClothingCAS;
                            }

                            if (!compactCAS)
                            {
                                mWin.CreateGridItems(true);
                                mWin.PopulateGrid(true);
                            }

                            if ((ths.mCurrentFocusedRow == null) || (((BodyTypes)sAccessoriesSelection) == mWin.CASPart.BodyType))
                            {
                                if (ths.mCurrentFocusedRow != null)
                                {
                                    ths.mCurrentFocusedRow.SetArrowGlow(false);
                                }
                                ths.mClothingTypesGrid.SelectedItem = num;
                                ths.mCurrentFocusedRow = mWin;
                                ths.mCurrentFocusedRow.SetArrowGlow(true);
                                ths.UpdateButtons(mWin.SelectedType);
                                if (ths.IsAccessoryType(mWin.CASPart.BodyType))
                                {
                                    CASClothingCategory.sAccessoriesSelection = (int)mWin.CASPart.BodyType;
                                }
                                ths.mCurrentPreset = mWin.Selection as CASPartPreset;
                            }
                        }
                        num++;
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnUndoRedo", e);
            }
        }

        public static void OnButtonMouseDown(WindowBase sender, UIMouseEventArgs eventArgs)
        {
            try
            {
                CASClothingCategory ths = CASClothingCategory.gSingleton;
                if (ths == null) return;

                BodyTypes bodyType = BodyTypes.None;
                CASClothingCategory.Category category = CASClothingCategory.Category.Count;
                switch (sender.ID)
                {
                    case 0x5dbcf11:
                        bodyType = BodyTypes.UpperBody;
                        category = CASClothingCategory.Category.Tops;
                        break;
                    case 0x5dbcf12:
                        bodyType = BodyTypes.LowerBody;
                        category = CASClothingCategory.Category.Bottoms;
                        break;
                    case 0x5dbcf13:
                        bodyType = BodyTypes.Shoes;
                        category = CASClothingCategory.Category.Shoes;
                        break;
                    case 0x5dbcf14:
                        bodyType = BodyTypes.FullBody;
                        category = CASClothingCategory.Category.Outfits;
                        break;
                    case 0x5dbcf90:
                        category = CASClothingCategory.Category.Accessories;
                        break;
                    case 0x5dbcf1b:
                        category = CASClothingCategory.Category.CollarBridle;
                        break;
                    case 0x5dbcf1c:
                        category = CASClothingCategory.Category.Saddles;
                        break;
                    default:
                        return;
                }

                if (eventArgs.MouseKey == MouseKeys.kMouseLeft)
                {
                    if (CASClothingCategory.CurrentTypeCategory != category)
                    {
                        ths.SetTypeCategory(category);

                        eventArgs.mHandled = true;
                    }
                }
                else if (eventArgs.MouseKey == MouseKeys.kMouseRight)
                {
                    if (category != CASClothingCategory.Category.Accessories)
                    {
                        ArrayList list = ths.mModel.GetVisibleCASParts(bodyType, (uint)ths.mCurrentStyleCategory);

                        int count = list.Count;
                        if (count > 0)
                        {
                            CASPart part = (CASPart)list[(int)(count * RandomGen.NextDouble())];

                            ths.mModel.RequestAddCASPart(part, false);
                        }
                    }

                    eventArgs.mHandled = true;
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnButtonMouseDown", e);
            }
        }

        protected static bool PopulateGrid()
        {
            CASClothingCategory ths = CASClothingCategory.gSingleton;
            if (ths == null) return false;

            ths.mClothingTypesGrid.Clear();

            ths.mCurrentPreset = null;
            ths.mCurrentFocusedRow = null;
            ths.mTempFocusedRow = null;
            ths.mSelectedType = CASClothingRow.SelectedTypes.None;
            ths.mShareButton.Enabled = false;
            ths.mTrashButton.Enabled = false;
            ths.mSaveButton.Enabled = false;
            ths.mSortButton.Enabled = false;
            ths.mSortButton.Tag = false;

            bool hasFilterableContent = false;

            List<object> parts = new List<object>();

            bool compactCAS = false;
            if (ths.mCurrentPart == BodyTypes.Accessories)
            {
                compactCAS = MasterController.Settings.mCompactAccessoryCAS;
            }
            else
            {
                compactCAS = MasterController.Settings.mCompactClothingCAS;
            }

            if (compactCAS)
            {
                List<CASParts.PartPreset> triPart = new List<CASParts.PartPreset>();
                parts.Add(triPart);

                foreach (object uncastPart in ths.mPartsList)
                {
                    if (uncastPart is CASPart)
                    {
                        CASPart part = (CASPart)uncastPart;

                        uint num2 = CASUtils.PartDataNumPresets(part.Key);

                        if (num2 > 0)
                        {
                            CASParts.PartPreset preset = new CASParts.PartPreset(part, 0);
                            if (!preset.Valid)
                            {
                                preset = null;
                            }

                            if (preset == null) continue;

                            if (!ths.mContentTypeFilter.ObjectMatchesFilter(preset, ref hasFilterableContent)) continue;

                            triPart.Add(preset);
                        }

                        if (triPart.Count == 3)
                        {
                            triPart = new List<CASParts.PartPreset>();
                            parts.Add(triPart);
                        }
                    }
                }
            }
            else
            {
                foreach (object part in ths.mPartsList)
                {
                    if (part is CASPart)
                    {
                        List<CASParts.PartPreset> uniPart = new List<CASParts.PartPreset>();
                        uniPart.Add(new CASParts.PartPreset((CASPart)part));
                        parts.Add(uniPart);
                    }
                    else
                    {
                        parts.Add(part);
                    }
                }
            }

            if (ths.PartPresetsList != null)
            {
                ths.PartPresetsList.Clear();
            }

            sCompleted.Clear();
            sRows.Clear();

            ths.mClothingTypesGrid.ItemRowsChanged -= OnItemRowsChanged;
            ths.mClothingTypesGrid.ItemRowsChanged += OnItemRowsChanged;

            if (parts.Count == 0) return false;

            ResourceKey layoutKey = ResourceKey.CreateUILayoutKey("CASClothingRow", 0x0);
            ths.mClothingTypesGrid.BeginPopulating(AddGridItem, parts, 0x3, layoutKey, null);
            return true;
        }

        protected static void OnItemRowsChanged()
        {
            try
            {
                foreach (CASClothingRow row in sRows)
                {
                    if (sCompleted.ContainsKey(row)) continue;

                    CASClothingRowEx.Create(row, MasterController.Settings.mAllowMultipleAccessories);

                    sCompleted.Add(row, true);
                }

                sRows.Clear();
            }
            catch (Exception e)
            {
                Common.Exception("OnItemRowsChanged", e);
            }
        }

        public static void SelectItem(CASClothingCategory ths, CASPart part, CASPartPreset preset, bool allowMultiple)
        {
            ICASModel cASModel = Responder.Instance.CASModel;
            List<CASPart> wornParts = cASModel.GetWornParts(part.BodyType);

            bool flag = false;
            if (ths.IsAccessoryType(ths.mCurrentPart))
            {
                if ((part.Key == ths.mInvalidCASPart.Key) && (!allowMultiple))
                {
                    ths.RemoveAllPartsOfType(ths.mCurrentPart);
                    CASClothingCategory.sAccessoriesSelection = (int)ths.mCurrentPart;
                }
                else
                {
                    flag = true;
                    CASClothingCategory.sAccessoriesSelection = (int)part.BodyType;
                    CASController.Singleton.SetAccessoryCam(part.BodyType, true);

                    if (!allowMultiple)
                    {
                        if (part.BodyType == BodyTypes.Earrings)
                        {
                            ths.RemoveAllPartsOfType(BodyTypes.LeftEarring);
                            ths.RemoveAllPartsOfType(BodyTypes.RightEarring);
                        }
                        else if ((part.BodyType == BodyTypes.LeftEarring) || (part.BodyType == BodyTypes.RightEarring))
                        {
                            ths.RemoveAllPartsOfType(BodyTypes.Earrings);
                        }
                    }
                }
            }
            else if (!wornParts.Contains(part))
            {
                flag = true;
            }

            if (preset != null)
            {
                ths.mCurrentPreset = preset;
                if (preset.mPresetString != null)
                {
                    if (flag)
                    {
                        ths.mModel.RequestAddCASPart(part, preset.mPresetString);
                    }
                    else
                    {
                        ths.mModel.RequestCommitPresetToPart(part, preset.mPresetString);
                    }
                }
            }
            else if (flag)
            {
                ths.mModel.RequestAddCASPart(part, false);
                CASSelectionGrid.SetSelectionIndex((uint)part.BodyType);
            }
            Audio.StartSound("ui_tertiary_button");
        }

        public static void RemoveItem(CASClothingCategory ths, CASPart part)
        {
            ths.mModel.RequestRemoveCASPart(part);

            Audio.StartSound("ui_tertiary_button");
        }

        public static bool AddGridItem(ItemGrid grid, object current, ResourceKey layoutKey, object context)
        {
            try
            {
                CASClothingCategory ths = CASClothingCategory.gSingleton;
                if (ths == null) return false;

                bool flag = false;
                if (current != null)
                {
                    if (current is List<CASParts.PartPreset>)
                    {
                        List<CASParts.PartPreset> parts = current as List<CASParts.PartPreset>;
                        if (parts.Count > 0)
                        {
                            CASClothingRow row = UIManager.LoadLayout(layoutKey).GetWindowByExportID(0x1) as CASClothingRow;
                            if (row == null) return false;

                            row.UseEp5AsBaseContent = ths.mIsEp5Base;
                            row.CASPart = parts[0].mPart;
                            row.RowController = ths;

                            ArrayList list = CASClothingRowEx.CreateGridItems(row, parts, true);

                            ths.mSortButton.Tag = ((bool)ths.mSortButton.Tag) | row.HasFilterableContent;

                            if (list.Count > 0x0)
                            {
                                sRows.Add(row);

                                grid.AddItem(new ItemGridCellItem(row, null));

                                flag = true;
                                if (row.SelectedItem == -1)
                                {
                                    return flag;
                                }

                                if (ths.IsAccessoryType(row.CASPart.BodyType))
                                {
                                    if (CASClothingRowEx.IsWorn(row.CASPart))
                                    {
                                        if (row.CASPart.BodyType == ((BodyTypes)CASClothingCategory.sAccessoriesSelection))
                                        {
                                            grid.SelectedItem = grid.Count - 0x1;
                                            ths.mSelectedType = row.SelectedType;
                                            CASClothingCategory.sAccessoriesSelection = (int)row.CASPart.BodyType;
                                            ths.mCurrentPreset = row.Selection as CASPartPreset;
                                        }
                                    }
                                    else
                                    {
                                        grid.SelectedItem = grid.Count - 0x1;
                                        ths.mSelectedType = row.SelectedType;
                                        CASClothingCategory.sAccessoriesSelection = (int)row.CASPart.BodyType;
                                        ths.mCurrentPreset = row.Selection as CASPartPreset;
                                    }

                                    return flag;
                                }

                                grid.SelectedItem = grid.Count - 0x1;
                                ths.mSelectedType = row.SelectedType;
                                ths.mCurrentPreset = row.Selection as CASPartPreset;
                            }
                        }

                        return flag;
                    }
                    else
                    {
                        List<object> featured = current as List<object>;
                        if (featured == null)
                        {
                            return flag;
                        }

                        CASClothingRow row = UIManager.LoadLayout(layoutKey).GetWindowByExportID(1) as CASClothingRow;
                        row.ObjectOfInterest = featured;
                        row.RowController = ths;

                        ArrayList items = row.CreateGridItems(true);
                        ths.mSortButton.Tag = ((bool)ths.mSortButton.Tag) | row.HasFilterableContent;
                        if (items.Count > 0)
                        {
                            grid.AddItem(new ItemGridCellItem(row, null));
                        }
                        return true;
                    }
                }

                ths.mContentTypeFilter.UpdateFilterButtonState();

                ths.UpdateButtons(ths.mSelectedType);
                if (CASClothingCategory.OnClothingGridFinishedPopulating != null)
                {
                    CASClothingCategory.OnClothingGridFinishedPopulating();
                }

                return flag;
            }
            catch (Exception e)
            {
                Common.Exception("AddGridItem", e);
                return false;
            }
        }

        public static void OnDesignButtonClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                CASClothingCategory ths = CASClothingCategory.gSingleton;
                if (ths == null) return;

                if (ths.mCurrentPart == BodyTypes.Accessories)
                {
                    if (ths.mCurrentPreset != null)
                    {
                        if (ths.mCurrentPreset.mPart.Key != ths.mInvalidCASPart.Key)
                        {
                            CASCompositorController.Instance.SetTargetObject(ths.mCurrentPreset.mPart);
                            return;
                        }
                    }
                }

                ths.OnDesignButtonClick(sender, eventArgs);
            }
            catch (Exception e)
            {
                Common.Exception("OnDesignButtonClick", e);
            }
        }

        public class DelayedCategoryUpdate : RepeatingTask
        {
            static DelayedCategoryUpdate sTask = null;

            static bool sPopulating = false;

            public DelayedCategoryUpdate()
            { }

            public static void Perform()
            {
                DelayedCategoryUpdate.Create(ref sTask);
            }

            protected void OnPopulate()
            {
                if (sPopulating) return;

                try
                {
                    sPopulating = true;

                    CASLogic logic = CASLogic.GetSingleton();
                    if (logic == null) return;

                    CASClothingCategory category = CASClothingCategory.gSingleton;
                    if (category == null) return;

                    if (category.mClothingTypesGrid == null) return;

                    category.mClothingTypesGrid.ItemRowsChanged -= OnPopulate;

                    // Must be rerun now to load the parts without interference from CASModelProxy
                    category.LoadParts();

                    PopulateGrid();
                }
                finally
                {
                    sPopulating = false;
                }
            }

            protected override bool OnPerform()
            {
                CASLogic logic = CASLogic.GetSingleton();
                if (logic == null) return false;

                CASClothingCategory category = CASClothingCategory.gSingleton;
                if (category == null) return true;

                if (category.mClothingTypesGrid == null) return true;

                if ((category.mClothingTypesGrid.Count > 0) || (!sPopulating))
                {
                    OnPopulate();
                }
                else
                {
                    category.mClothingTypesGrid.ItemRowsChanged -= OnPopulate;
                    category.mClothingTypesGrid.ItemRowsChanged += OnPopulate;
                }
                return false;
            }

            public override void Dispose()
            {
                base.Dispose();

                sTask = null;
            }
        }

        public class Tracer : StackTracer
        {
            static bool sSemaphore = false;

            public Tracer()
            {
                AddTest(typeof(Sims3.UI.CAS.CASClothingCategory), "Void PopulateGrid", OnPerform);
            }

            private static bool OnPerform(StackTrace trace, StackFrame frame)
            {
                if (sSemaphore) return true;

                try
                {
                    sSemaphore = true;

                    //if (CASDresserSheet.gSingleton != null) return true;

                    CASDresserClothing dresser = CASDresserClothing.gSingleton;
                    if (dresser != null)
                    {
                        dresser.mDefaultText.Visible = false;
                    }

                    CASClothingCategory ths = CASClothingCategory.gSingleton;
                    if (ths == null) return true;

                    if (ths.mCategoryText.Caption.Equals(ths.GetClothingStateName(CASClothingState.Career)) && (Responder.Instance.CASModel.OutfitIndex == 0x0))
                    {
                        ths.mCurrentPreset = null;
                        ths.mCurrentFocusedRow = null;
                        ths.mTempFocusedRow = null;
                        ths.mSelectedType = CASClothingRow.SelectedTypes.None;
                        ths.mShareButton.Enabled = false;
                        ths.mTrashButton.Enabled = false;
                        ths.mSaveButton.Enabled = false;
                        ths.mSortButton.Enabled = false;
                        ths.mSortButton.Tag = false;
                        ResourceKey layoutKey = ResourceKey.CreateUILayoutKey("CASClothingRow", 0x0);
                        ths.mClothingTypesGrid.BeginPopulating(ths.AddGridItem, ths.mPartsList, 0x3, layoutKey, null);
                    }

                    return true;
                }
                finally
                {
                    sSemaphore = false;
                }
            }
        }
    }
}
