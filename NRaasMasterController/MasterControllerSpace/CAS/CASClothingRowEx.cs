using NRaas.CommonSpace.Helpers;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Store;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.MasterControllerSpace.CAS
{
    public class CASClothingRowEx
    {
        CASClothingRow mRow;

        bool mAllowMultiple;

        protected CASClothingRowEx(CASClothingRow row, bool allowMultiple)
        {
            mRow = row;
            mAllowMultiple = allowMultiple;
        }

        public static CASClothingRowEx Create(CASClothingRow row, bool allowMultiple)
        {
            if (row.mGridWindows == null) return null;

            CASClothingRowEx rowEx = new CASClothingRowEx(row, allowMultiple);

            rowEx.UpdateSelectedStates(-1);

            for (int i = 0; i < row.mGridWindows.Length; i++)
            {
                if (row.mGridWindows[i] == null) continue;

                row.mGridWindows[i].MouseDown -= row.OnClothingSelect;

                row.mGridWindows[i].MouseDown -= rowEx.OnClothingSelect;
                row.mGridWindows[i].MouseDown += rowEx.OnClothingSelect;
            }

            return rowEx;
        }

        public static void AddClothingItemAndPresets(CASClothingRow ths, List<CASParts.PartPreset> parts, bool allowTemp)
        {
            if (parts.Count == 1)
            {
                ths.AddClothingItemAndPresets(parts[0].mPart, allowTemp);
            }
            else
            {
                int orderIndex = 0x0;

                ths.mHasFilterableContent = false;

                for (int i = 0x0; i < parts.Count; i++)
                {
                    CASParts.PartPreset preset = parts[i];

                    if (ths.AddPresetGridItem(preset, orderIndex, preset.mPresetId))
                    {
                        if (IsWorn(preset.mPart))
                        {
                            ths.mSelectedItem = orderIndex;
                        }

                        orderIndex++;
                    }
                }
            }
        }

        public static ArrayList CreateGridItems(CASClothingRow ths, List<CASParts.PartPreset> parts, bool allowTemp)
        {
            ths.mItems.Clear();
            ths.mTempWindow = null;
            ths.mTempWindowValid = false;
            AddClothingItemAndPresets(ths, parts, allowTemp);
            if (ths.mFeaturedStoreItemBorder != null)
            {
                ths.mFeaturedStoreItemBorder.Visible = !(ths.mObjectOfInterest is CASPart);
            }
            ths.mNumItems = ths.mItems.Count;
            return ths.mItems;
        }

        protected  void RemoveItem(ICASRowController ths, CASPart part)
        {
            if (ths is CASClothingCategory)
            {
                CASClothingCategoryEx.RemoveItem(ths as CASClothingCategory, part);
            }
            else if (ths is CASMakeup)
            {
                CASMakeupEx.RemoveItem(ths as CASMakeup, part);
            }
        }

        protected void SelectItem(ICASRowController ths, CASPart part, CASPartPreset preset)
        {
            if (ths is CASClothingCategory)
            {
                CASClothingCategoryEx.SelectItem(ths as CASClothingCategory, part, preset, mAllowMultiple);
            }
            else if (ths is CASMakeup)
            {
                CASMakeupEx.SelectItem(ths as CASMakeup, part, preset, mAllowMultiple);
            }
        }

        public static bool IsWorn(CASPart part)
        {
            foreach (CASPart worn in CASLogic.GetSingleton().mBuilder.GetWornParts(new BodyTypes[] { part.BodyType }))
            {
                if (worn.Key == part.Key)
                {
                    return true;
                }
            }

            return false;
        }

        public void UpdateSelectedStates(int toggledIndex)
        {
            if (mRow == null) return;

            if (mRow.mGridWindows == null) return;

            for (int i = 0x0; i < CASClothingRow.kNumGridWindows; i++)
            {
                string msg = "UpdateSelectedStates";

                try
                {
                    Window window = mRow.mGridWindows[i] as Window;
                    if (window == null) continue;

                    CASClothingRow.ClothingThumbnail tag = window.Tag as CASClothingRow.ClothingThumbnail;
                    StdDrawable drawable = window.Drawable as StdDrawable;
                    string name = null;
                    if (tag != null)
                    {
                        WindowBase childByID = window.GetChildByID(0x27, true);

                        CASPart cASPart = mRow.CASPart;
                        if (tag.mData is CASPart)
                        {
                            cASPart = (CASPart)tag.mData;
                        }
                        else if (tag.mData is CASPartPreset)
                        {
                            cASPart = ((CASPartPreset)tag.mData).mPart;
                        }

                        if ((mRow.mRowController.IsAccessoryType(cASPart.BodyType)) && (MasterController.Settings.mCompactAccessoryCAS))
                        {
                            msg += "A";

                            bool active = false;
                            if (tag.mIndex == toggledIndex)
                            {
                                active = !childByID.Visible;
                            }
                            else
                            {
                                active = IsWorn(cASPart);
                            }

                            if (active)
                            {
                                msg += "B";

                                tag.mState |= CASClothingRow.WindowState.Active;
                                if ((tag.mState & CASClothingRow.WindowState.Highlighted) == CASClothingRow.WindowState.Normal)
                                {
                                    name = CASClothingRow.kActiveImage;
                                }

                                if (childByID != null)
                                {
                                    childByID.Visible = true;
                                }
                            }
                            else
                            {
                                msg += "C";

                                tag.mState &= ~CASClothingRow.WindowState.Active;
                                if (tag.mState == CASClothingRow.WindowState.Normal)
                                {
                                    name = CASClothingRow.kNormalImage;
                                }

                                if (childByID != null)
                                {
                                    childByID.Visible = false;
                                }
                            }
                        }
                        else if (tag.mIndex == mRow.mSelectedItem)
                        {
                            msg += "D";

                            tag.mState |= CASClothingRow.WindowState.Active;
                            if ((tag.mState & CASClothingRow.WindowState.Highlighted) == CASClothingRow.WindowState.Normal)
                            {
                                name = CASClothingRow.kActiveImage;
                            }

                            msg += "E";

                            if (((cASPart.Key != CASClothingRow.kInvalidCASPart.Key) && (childByID != null)) && mRow.mRowController.IsAccessoryType(cASPart.BodyType))
                            {
                                childByID.Visible = true;
                            }
                        }
                        else
                        {
                            msg += "F";

                            tag.mState &= ~CASClothingRow.WindowState.Active;
                            if (tag.mState == CASClothingRow.WindowState.Normal)
                            {
                                name = CASClothingRow.kNormalImage;
                            }

                            msg += "G";

                            if (((cASPart.Key != CASClothingRow.kInvalidCASPart.Key) && (childByID != null)) && mRow.mRowController.IsAccessoryType(cASPart.BodyType))
                            {
                                childByID.Visible = false;
                            }
                        }
                    }

                    msg += "H";

                    if (name != null)
                    {
                        ResourceKey resKey = ResourceKey.CreatePNGKey(name, 0x0);
                        drawable[DrawableBase.ControlStates.kNormal] = UIManager.LoadUIImage(resKey);
                        window.Invalidate();
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(msg, e);
                }
            }
        }

        public void OnClothingSelect(WindowBase sender, UIMouseEventArgs args)
        {
            try
            {
                CASParts.Wrapper part = null;

                CASClothingRow.ClothingThumbnail tag = sender.Tag as CASClothingRow.ClothingThumbnail;
                if (!mRow.mTempWindowValid || (tag.mIndex != (mRow.mItems.Count - 0x1)))
                {
                    if (tag.mData is CASPart)
                    {
                        part = new CASParts.Wrapper((CASPart)tag.mData);

                        if (args.MouseKey != MouseKeys.kMouseRight)
                        {
                            SelectItem(mRow.RowController, part.mPart, null);
                            mRow.mSelectedItem = (part.mPart.Key.InstanceId != 0x0L) ? tag.mIndex : -1;
                        }
                    }
                    else if (tag.mData is CASPartPreset)
                    {
                        CASPartPreset preset = (CASPartPreset)tag.mData;

                        part = new CASParts.Wrapper(preset.mPart);

                        if ((mRow.mRowController.IsAccessoryType(part.mPart.BodyType)) && ((tag.mState & CASClothingRow.WindowState.Active) != CASClothingRow.WindowState.Normal))
                        {
                            if (args.MouseKey != MouseKeys.kMouseRight)
                            {
                                if (!mAllowMultiple)
                                {
                                    mRow.RowController.RemoveItem(part.mPart.BodyType);
                                }
                                else
                                {
                                    RemoveItem(mRow.RowController, part.mPart);
                                }

                                mRow.mSelectedItem = -1;
                            }
                            else
                            {
                                if (mRow.mRowController is CASClothingCategory)
                                {
                                    (mRow.mRowController as CASClothingCategory).mCurrentPreset = preset;
                                }
                                else if (mRow.mRowController is CASMakeup)
                                {
                                    (mRow.mRowController as CASMakeup).mCurrentPreset = preset;
                                }
                            }
                        }
                        else if (args.MouseKey != MouseKeys.kMouseRight)
                        {
                            SelectItem(mRow.RowController, preset.mPart, preset);
                            mRow.mSelectedItem = tag.mIndex;
                        }
                    }
                    else if ((args.MouseKey != MouseKeys.kMouseRight) && (!(tag.mData is IFeaturedStoreItem)))
                    {
                        mRow.RowController.RemoveItem(mRow.CASPart.BodyType);
                        mRow.mSelectedItem = -1;
                    }

                    if (args.MouseKey != MouseKeys.kMouseRight)
                    {
                        mRow.ClearTempItem();
                        if (mRow.mObjectOfInterest is CASPart)
                        {
                            if (!mAllowMultiple)
                            {
                                mRow.mRowController.OnRowInItemSelected(mRow, mRow.CASPart.BodyType);
                            }
                            UpdateSelectedStates(tag.mIndex);
                        }
                    }
                }
                else if ((mRow.mRowController.IsAccessoryType(mRow.CASPart.BodyType)) && (IsWorn(mRow.CASPart)))
                {
                    if (args.MouseKey != MouseKeys.kMouseRight)
                    {
                        if (!mAllowMultiple)
                        {
                            mRow.RowController.RemoveItem(mRow.CASPart.BodyType);
                        }
                        else
                        {
                            RemoveItem(mRow.RowController, mRow.CASPart);
                        }

                        mRow.mSelectedItem = -1;

                        mRow.ClearTempItem();

                        if (!mAllowMultiple)
                        {
                            mRow.mRowController.OnRowInItemSelected(mRow, mRow.CASPart.BodyType);
                        }
                        UpdateSelectedStates(tag.mIndex);
                    }
                }

                if (args.MouseKey == MouseKeys.kMouseRight)
                {
                    bool remove = ((args.Modifiers & Modifiers.kModifierMaskControl) == Modifiers.kModifierMaskControl);

                    CASBase.Blacklist(part, remove, null);

                    if (remove)
                    {
                        bool compactCAS = false;
                        if (mRow.mRowController.IsAccessoryType(mRow.CASPart.BodyType))
                        {
                            compactCAS = MasterController.Settings.mCompactAccessoryCAS;
                        }
                        else
                        {
                            compactCAS = MasterController.Settings.mCompactClothingCAS;
                        }

                        if (!compactCAS)
                        {
                            for (int i = 0; i < mRow.mItems.Count; i++)
                            {
                                CASClothingRow.ClothingThumbnail thumbnail = mRow.mItems[i] as CASClothingRow.ClothingThumbnail;
                                if (thumbnail != null)
                                {
                                    if ((i + 1) < mRow.mGridWindows.Length)
                                    {
                                        mRow.mGridWindows[i + 1].Visible = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            CASClothingRow.ClothingThumbnail thumbnail = mRow.mItems[tag.mIndex] as CASClothingRow.ClothingThumbnail;
                            if (thumbnail != null)
                            {
                                if ((tag.mIndex + 1) < mRow.mGridWindows.Length)
                                {
                                    mRow.mGridWindows[tag.mIndex + 1].Visible = false;
                                }
                            }
                        }
                    }

                    args.Handled = true;
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnClothingSelect", e);
            }
        }
    }
}
