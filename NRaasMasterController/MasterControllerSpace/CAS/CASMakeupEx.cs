using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.MasterControllerSpace.Sims;
using NRaas.MasterControllerSpace.Proxies;
using Sims3.Gameplay.CAS;
using Sims3.Metadata;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Store;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.MasterControllerSpace.CAS
{
    public class CASMakeupEx
    {
        public static void OnOpacitySliderChange(WindowBase sender, UIValueChangedEventArgs args)
        {
            try
            {
                CASMakeup ths = CASMakeup.gSingleton;
                if (ths == null) return;

                Slider slider = sender as Slider;
                if (slider != null)
                {
                    CASPart part = (CASPart)ths.mGridMakeupParts.SelectedTag;

                    ths.SetMakeupOpacity(part, ((float)slider.Value) / ((float)slider.MaxValue), true);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnOpacitySliderChange", e);
            }
        }

        public static void OnOpacitySliderMouseUp(WindowBase sender, UIMouseEventArgs args)
        {
            try
            {
                CASMakeup ths = CASMakeup.gSingleton;
                if (ths == null) return;

                Slider slider = sender as Slider;
                if (slider != null)
                {
                    CASPart part = (CASPart)ths.mGridMakeupParts.SelectedTag;

                    ths.SetMakeupOpacity(part, ((float)slider.Value) / ((float)slider.MaxValue), true);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnOpacitySliderMouseUp", e);
            }
        }

        public static void OnGridPresetsClick(ItemGrid sender, ItemGridCellClickEvent itemClicked)
        {
            try
            {
                CASMakeup ths = CASMakeup.gSingleton;
                if (ths == null) return;

                ths.UpdatePresetState();
                if (itemClicked.mTag is ResourceKey)
                {
                    ResourceKey mTag = (ResourceKey)itemClicked.mTag;
                    ColorInfo info = ColorInfo.FromResourceKey(mTag);

                    CASPart part = (CASPart)ths.mGridMakeupParts.SelectedTag;

                    ths.SetMakeupColors(part, info.Colors, true, false);
                    Audio.StartSound("ui_tertiary_button");

                    ths.mCurrentPreset = new CASPartPreset(part, null);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnGridPresetsClick", e);
            }
        }

        public static void OnButtonDesignClick(WindowBase sender, UIButtonClickEventArgs args)
        {
            try
            {
                Common.FunctionTask.Perform(ShowColorPickerTask);
            }
            catch (Exception e)
            {
                Common.Exception("OnButtonDesignClick", e);
            }
        }

        private static void OnColorsChanged(Color[] colors)
        {
            try
            {
                CASMakeup ths = CASMakeup.gSingleton;
                if (ths == null) return;

                SetMakeupColors(ths, CASMakeup.sCategory, colors, false);
            }
            catch (Exception e)
            {
                Common.Exception("OnColorsChanged", e);
            }
        }

        private static void SetMakeupColors(CASMakeup ths, BodyTypes type, Color[] colors, bool finalize)
        {
            ths.SetMakeupColors(ths.mCurrentPreset.mPart, colors, finalize, false);
        }

        private static void OnDialogClosed(bool accept, bool colorChanged, Color[] colors)
        {
            try
            {
                CASMakeup ths = CASMakeup.gSingleton;
                if (ths == null) return;

                if (CASMakeup.sCategory == BodyTypes.CostumeMakeup)
                {
                    if (accept)
                    {
                        SetMakeupColors(ths, CASMakeup.sCategory, colors, true);
                        List<ItemGridCellItem> items = ths.mGridCostumeParts.Items;
                        int num = 0x0;
                        foreach (ItemGridCellItem item in items)
                        {
                            CASClothingRow mWin = item.mWin as CASClothingRow;
                            if (mWin != null)
                            {
                                mWin.ShowSelectedItem(false);
                                if (mWin.SelectedItem != -1)
                                {
                                    mWin.CreateGridItems(true);
                                    mWin.PopulateGrid(true);
                                    ths.mGridCostumeParts.SelectedItem = num;
                                    ths.mCurrentFocusedRow = mWin;
                                    ths.mCurrentFocusedRow.SetArrowGlow(true);
                                    ths.mCurrentPreset = mWin.Selection as CASPartPreset;
                                    ths.UpdateCostumePresetState();
                                }
                                num++;
                            }
                        }
                    }
                    else if (colorChanged)
                    {
                        Responder.Instance.CASModel.RequestUndo();
                        Responder.Instance.CASModel.RequestClearRedoOperations();
                    }
                }
                else if (!accept)
                {
                    if (colorChanged)
                    {
                        Responder.Instance.CASModel.RequestUndo();
                        Responder.Instance.CASModel.RequestClearRedoOperations();
                    }
                }
                else
                {
                    SetMakeupColors(ths, CASMakeup.sCategory, colors, true);
                    ths.mGridMakeupPresets.SelectedItem = -1;
                    int index = 0x0;
                    List<ItemGridCellItem> list2 = ths.mGridMakeupPresets.Items;
                    Vector3[] vecColors = new Vector3[colors.Length];
                    foreach (Color color in colors)
                    {
                        vecColors[index] = CompositorUtil.ColorToVector3(color);
                        index++;
                    }

                    index = 0x0;
                    foreach (ItemGridCellItem item2 in list2)
                    {
                        ResourceKey mTag = (ResourceKey)item2.mTag;
                        ColorInfo colorInfo = ColorInfo.FromResourceKey(mTag);
                        if (CASMakeup.CompareColors(vecColors, colorInfo))
                        {
                            ths.mGridMakeupPresets.SelectedItem = index;
                            break;
                        }
                        index++;
                    }
                    ths.UpdatePresetState();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnDialogClosed", e);
            }
        }

        private static void OnColorsSaved(Color[] colors)
        {
            try
            {
                CASMakeup ths = CASMakeup.gSingleton;
                if (ths == null) return;

                bool flag = false;
                bool flag2 = false;
                if (CASMakeup.sCategory != BodyTypes.CostumeMakeup)
                {
                    foreach (ItemGridCellItem item in ths.mGridMakeupPresets.Items)
                    {
                        flag2 = true;
                        ResourceKey mTag = (ResourceKey)item.mTag;
                        ColorInfo info = ColorInfo.FromResourceKey(mTag);
                        for (int i = 0x0; i < info.Colors.Length; i++)
                        {
                            Vector3 vector = CompositorUtil.ColorToVector3(colors[i]);
                            Vector3 v = CompositorUtil.ColorToVector3(info.Colors[i]);
                            if (!vector.IsSimilarTo(v))
                            {
                                flag2 = false;
                                break;
                            }
                        }
                        if (flag2)
                        {
                            break;
                        }
                    }

                    if (!flag2)
                    {
                        ColorInfo info2 = new ColorInfo();
                        info2.Usage = ColorInfo.PreferredUse.Makeup;
                        switch (CASMakeup.sCategory)
                        {
                            case BodyTypes.FirstFace:
                                info2.UsageSubCategory = ColorInfo.eUsageSubCategory.MakeupLipstick;
                                break;

                            case BodyTypes.EyeShadow:
                                info2.UsageSubCategory = ColorInfo.eUsageSubCategory.MakeupEyeshadow;
                                break;

                            case BodyTypes.EyeLiner:
                                info2.UsageSubCategory = ColorInfo.eUsageSubCategory.MakeupEyeliner;
                                break;

                            case BodyTypes.Blush:
                                info2.UsageSubCategory = ColorInfo.eUsageSubCategory.MakeupBlush;
                                break;
                        }
                        info2.Colors = colors;
                        flag = info2.SaveMakeupPreset(info2.UsageSubCategory) != ResourceKey.kInvalidResourceKey;
                        ths.PopulatePresetsGrid(CASMakeup.sCategory, ths.mCurrentPreset.mPart, ths.mButtonFilter.Selected);
                    }
                }
                else
                {
                    CASPart wornPart = ths.mCurrentPreset.mPart;

                    ObjectDesigner.SetCASPart(wornPart.Key);
                    Vector3[] makeupVectorColors = ths.GetMakeupVectorColors(wornPart);
                    uint num = CASUtils.PartDataNumPresets(wornPart.Key);
                    for (uint j = 0x0; j < num; j++)
                    {
                        KeyValuePair<string, Dictionary<string, Complate>> presetEntryFromPresetString = (KeyValuePair<string, Dictionary<string, Complate>>)SimBuilder.GetPresetEntryFromPresetString(CASUtils.PartDataGetPreset(wornPart.Key, j));
                        Vector3[] vectorArray2 = ths.GetMakeupVectorColors(presetEntryFromPresetString);
                        flag2 = true;
                        for (uint k = 0x0; k < vectorArray2.Length; k++)
                        {
                            if (!makeupVectorColors[k].IsSimilarTo(vectorArray2[k]))
                            {
                                flag2 = false;
                                break;
                            }
                        }
                        if (flag2)
                        {
                            break;
                        }
                    }

                    if (!flag2)
                    {
                        uint index = ObjectDesigner.AddDesignPreset(Responder.Instance.CASModel.GetDesignPreset(wornPart));
                        flag = index != uint.MaxValue;
                        CASClothingRow row = ths.FindRow(wornPart);
                        if (row != null)
                        {
                            row.CreateGridItems(true);
                            row.PopulateGrid(true);
                        }
                        ths.mButtonCostumeFilter.Tag = true;
                        ths.mContentTypeFilter.UpdateFilterButtonState();
                        ThumbnailKey key = new ThumbnailKey(wornPart.Key, (int)CASUtils.PartDataGetPresetId(wornPart.Key, index), (uint)wornPart.BodyType, (uint)wornPart.AgeGenderSpecies, ThumbnailSize.Large);
                        ThumbnailManager.InvalidateThumbnail(key);
                    }
                }

                if (flag)
                {
                    CASController.Singleton.ErrorMsg(CASErrorCode.SaveSuccess);
                }
                else if (flag2)
                {
                    Simulator.AddObject(new OneShotFunctionTask(delegate
                    {
                        string messageText = Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/CAS/Hair:SaveDuplicate", new object[0x0]);
                        SimpleMessageDialog.Show(null, messageText, ModalDialog.PauseMode.PauseTask, new Vector2(-1f, -1f), "ui_error", "ui_hardwindow_close");
                    }));
                }
                else
                {
                    CASController.Singleton.ErrorMsg(CASErrorCode.SaveFailed);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnColorsSaved", e);
            }
        }

        protected static void ShowColorPickerTask()
        {
            try
            {
                CASMakeup ths = CASMakeup.gSingleton;
                if (ths == null) return;

                CASPart part = new CASPart();
                if (CASMakeup.sCategory == BodyTypes.CostumeMakeup)
                {
                    if (ths.mCurrentPreset == null)
                    {
                        ICASModel cASModel = new CASModelProxy(Responder.Instance.CASModel);
                        List<CASPart> wornParts = cASModel.GetWornParts(CASMakeup.sCategory);

                        if (wornParts.Count > 0)
                        {
                            part = wornParts[wornParts.Count - 1];

                            ths.mCurrentPreset = new CASPartPreset(part, null);
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    ths.mCurrentPreset = new CASPartPreset((CASPart)ths.mGridMakeupParts.SelectedTag, null);
                }

                CASMultiColorPickerDialog.OnColorsChanged += OnColorsChanged;
                CASMultiColorPickerDialog.OnColorsSaved += OnColorsSaved;
                CASMultiColorPickerDialog.OnDialogClosed += OnDialogClosed;
                if (CASMakeup.sCategory == BodyTypes.CostumeMakeup)
                {
                    ths.Tick -= ths.OnTick;
                }
                Color[] makeupColors = GetMakeupColors(ths, CASMakeup.sCategory);
                if (makeupColors != null)
                {
                    CASMultiColorPickerDialog.Show(makeupColors, makeupColors.Length, true, null);
                }
                if (CASMakeup.sCategory == BodyTypes.CostumeMakeup)
                {
                    ths.Tick += ths.OnTick;
                }
                CASMultiColorPickerDialog.OnColorsChanged -= OnColorsChanged;
                CASMultiColorPickerDialog.OnColorsSaved -= OnColorsSaved;
                CASMultiColorPickerDialog.OnDialogClosed -= OnDialogClosed;
            }
            catch (Exception e)
            {
                Common.Exception("ShowColorPickerTask", e);
            }
        }

        private static Color[] GetMakeupColors(CASMakeup ths, BodyTypes type)
        {
            Vector3[] makeupVectorColors = GetMakeupVectorColors(ths, type);
            if (makeupVectorColors == null)
            {
                return null;
            }
            int length = makeupVectorColors.Length;
            if (CASController.Singleton.ShowAllMakeupPresetsCheat)
            {
                length = 0x4;
            }
            Color[] colorArray = new Color[length];
            for (int i = 0x0; i < makeupVectorColors.Length; i++)
            {
                colorArray[i] = CompositorUtil.Vector3ToColor(makeupVectorColors[i]);
            }
            for (int j = makeupVectorColors.Length; j < length; j++)
            {
                colorArray[j] = new Color(0xffffffff);
            }
            return colorArray;
        }

        private static Vector3[] GetMakeupVectorColors(CASMakeup ths, BodyTypes type)
        {
            return ths.GetMakeupVectorColors(ths.mCurrentPreset.mPart);
        }

        public static void SelectItem(CASMakeup ths, CASPart part, CASPartPreset preset, bool allowMultiple)
        {
            ICASModel cASModel = new CASModelProxy(Responder.Instance.CASModel);
            List<CASPart> wornParts = cASModel.GetWornParts(part.BodyType);
            bool flag = false;
            if ((part.Key == ths.kInvalidCASPart.Key) && (!allowMultiple))
            {
                foreach (CASPart part2 in wornParts)
                {
                    cASModel.RequestRemoveCASPart(part2);
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
                        cASModel.RequestAddCASPart(part, preset.mPresetString);
                    }
                    else
                    {
                        cASModel.RequestCommitPresetToPart(part, preset.mPresetString);
                    }
                }
            }
            else if (flag)
            {
                ths.mCurrentPreset = new CASPartPreset(part, null);

                cASModel.RequestAddCASPart(part, false);
            }

            ths.UpdateCostumePresetState();
            Audio.StartSound("ui_tertiary_button");
        }

        public static void RemoveItem(CASMakeup ths, CASPart part)
        {
            ICASModel cASModel = new CASModelProxy(Responder.Instance.CASModel);
            cASModel.RequestRemoveCASPart(part);

            /*
            foreach (CASPart part in cASModel.GetWornParts(partType))
            {
                cASModel.RequestRemoveCASPart(part);
            }
            */
            ths.mCurrentPreset = null;
            ths.UpdateCostumePresetState();
            Audio.StartSound("ui_tertiary_button");
        }

        public static void OnButtonTabClick(WindowBase sender, UIButtonClickEventArgs args)
        {
            try
            {
                CASMakeup ths = CASMakeup.gSingleton;
                if (ths == null) return;

                switch (sender.ID)
                {
                    case 0x104:
                        SetCategory(BodyTypes.CostumeMakeup);
                        break;
                    default:
                        ths.OnButtonTabClick(sender, args);
                        break;
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnButtonTabClick", e);
            }
        }

        private static void SetCategory(BodyTypes category)
        {
            CASMakeup ths = CASMakeup.gSingleton;

            ths.mWindowCostume.Visible = false;
            ths.mWindowMakeup.Visible = false;
            CASFacialDetails gSingleton = CASFacialDetails.gSingleton;
            switch (category)
            {
                case BodyTypes.FirstFace:
                case BodyTypes.EyeShadow:
                case BodyTypes.EyeLiner:
                case BodyTypes.Blush:
                    ths.mWindowMakeup.Visible = true;
                    gSingleton.SetLongPanel(true);
                    ths.Tick -= ths.OnTick;
                    ths.mButtonCostumeFilter.Selected = false;
                    ths.mContentTypeFilter.Visible = false;
                    break;

                case BodyTypes.CostumeMakeup:
                    ths.mWindowCostume.Visible = true;
                    gSingleton.SetLongPanel(false);
                    gSingleton.SetShortPanelHeight((float)570f);
                    if (!(ths.GetWornPart(category).Key != ths.kInvalidCASPart.Key))
                    {
                        ths.mButtonDesignCostume.Enabled = false;
                        break;
                    }
                    ths.mButtonDesignCostume.Enabled = true;

                    ths.UpdateCostumePresetState();
                    ths.Tick -= ths.OnTick;
                    ths.Tick += ths.OnTick;

                    break;

                default:
                    break;
            }

            CASMakeup.sCategory = category;
            ths.HideUnusedIcons();
            PopulatePartsGrid(ths, CASMakeup.sCategory);
            ths.PopulatePresetsGrid(CASMakeup.sCategory, ths.GetWornPart(CASMakeup.sCategory), ths.mButtonFilter.Selected);
        }

        private static void PopulatePartsGrid(CASMakeup ths, BodyTypes category)
        {
            if (category != BodyTypes.CostumeMakeup)
            {
                ItemGrid mGridMakeupParts = ths.mGridMakeupParts;
                mGridMakeupParts.Clear();
                CASPart wornPart = ths.GetWornPart(category);
                List<object> visibleParts = ths.GetVisibleParts(category);
                ResourceKey layoutKey = ResourceKey.CreateUILayoutKey("GenericCasItem", 0x0);
                foreach (object obj2 in visibleParts)
                {
                    CASPart part = (CASPart)obj2;
                    ths.AddPartsGridItem(mGridMakeupParts, layoutKey, part);
                    if (part.Key == wornPart.Key)
                    {
                        mGridMakeupParts.SelectedItem = mGridMakeupParts.Count - 0x1;
                    }
                }
                ths.mButtonColor.Enabled = (mGridMakeupParts.SelectedItem != -1) && (mGridMakeupParts.SelectedItem != 0x0);
                ths.mButtonDelete.Enabled = ths.mButtonColor.Enabled;
            }
            else
            {
                ItemGrid mGridCostumeParts = ths.mGridCostumeParts;
                mGridCostumeParts.Clear();
                ths.mCurrentPreset = null;
                ths.mCurrentFocusedRow = null;
                ths.mTempFocusedRow = null;
                List<object> objectList = ths.GetVisibleParts(category);
                bool shouldEnableCatalogProductFilter = false;
                CASPart objectToNotRemove = ths.GetWornPart(category);
                ths.mContentTypeFilter.FilterObjects(objectList, out shouldEnableCatalogProductFilter, objectToNotRemove);
                ths.mButtonCostumeFilter.Enabled = false;
                ths.mButtonCostumeFilter.Tag = shouldEnableCatalogProductFilter;
                foreach (CASPart part4 in objectList)
                {
                    if (!(part4.Key == ths.kInvalidCASPart.Key))
                    {
                        CASClothingRow row = UIManager.LoadLayout(ResourceKey.CreateUILayoutKey("CASClothingRow", 0x0)).GetWindowByExportID(0x1) as CASClothingRow;
                        row.CASPart = part4;
                        row.RowController = ths;
                        if (row.CreateGridItems(true).Count > 0x0)
                        {
                            mGridCostumeParts.AddItem(new ItemGridCellItem(row, null));
                            if (row.SelectedItem != -1)
                            {
                                ths.mCurrentPreset = row.Selection as CASPartPreset;
                            }
                        }

                        CASClothingRowEx.Create(row, MasterController.Settings.mAllowMultipleMakeup);
                    }
                }
                ths.mContentTypeFilter.UpdateFilterButtonState();
            }
        }

        public static void OnGridPartsClick(ItemGrid sender, ItemGridCellClickEvent args)
        {
            try
            {
                CASMakeup ths = CASMakeup.gSingleton;
                if (ths == null) return;

                if (args.mTag is CASPart)
                {
                    CASModelProxy cASModel = new CASModelProxy(Responder.Instance.CASModel);

                    List<CASPart> wornParts = cASModel.GetWornParts(CASMakeup.sCategory);

                    CASPart tag = (CASPart)args.mTag;
                    if (tag.Key == ths.kInvalidCASPart.Key)
                    {
                        foreach (CASPart wornPart in wornParts)
                        {
                            cASModel.RequestRemoveCASPart(wornPart);
                        }
                    }
                    else if (wornParts.Contains(tag))
                    {
                        if (args.mButton == MouseKeys.kMouseRight)
                        {
                            cASModel.RequestRemoveCASPart(tag);
                        }
                        else
                        {
                            Color[] makeupColors = ths.GetMakeupColors(CASMakeup.sCategory);
                            ths.SetMakeupColors(tag, makeupColors, false, false);
                        }
                    }
                    else
                    {
                        cASModel.RequestAddCASPart(tag, false);

                        ths.mCurrentPreset = new CASPartPreset(tag, null);
                    }

                    Audio.StartSound("ui_tertiary_button");
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnGridPartsClick", e);
            }
        }
    }
}
