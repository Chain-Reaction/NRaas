using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.CAS;
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
    public class CASHairEx
    {
        protected static List<CASParts.Wrapper> GetVisibleCASParts(CASLogic ths, BodyTypes bodyType)
        {
            return GetVisibleCASParts(ths, bodyType, ths.GetForgivingCategoryMask(ths.mCurrentOutfitCategory));
        }
        protected static List<CASParts.Wrapper> GetVisibleCASParts(CASLogic ths, BodyTypes bodyType, uint categories)
        {
            uint alteredCategories = categories;
            ths.AdjustAvailableCategoriesForCASMode(ref alteredCategories);

            SimBuilder builder = ths.mBuilder;

            CASAgeGenderFlags age = builder.Age;
            CASAgeGenderFlags gender = builder.Gender;
            CASAgeGenderFlags species = builder.Species;

            List<CASParts.Wrapper> list = new List<CASParts.Wrapper>();
            foreach(CASParts.Wrapper part in CASBase.HairParts)
            {
                InvalidPartBase.Reason reason = InvalidPartBooter.Allow(part, age, gender, species, false, (OutfitCategories)categories);
                if (reason != InvalidPartBase.Reason.None) continue;

                if ((part.mPart.BodyType == bodyType) && OutfitUtils.PartMatchesSim(builder, alteredCategories, part.mPart))
                {
                    list.Add(part);
                }
            }
            return list;
        }

        public static void RefreshHairGrid()
        {
            try
            {
                CASHair ths = CASHair.gSingleton;
                if (ths == null) return;

                ths.PopulateHairPresetsInternal();
                PopulateTypesGrid();
            }
            catch (Exception e)
            {
                Common.Exception("OnUndo", e);
            }
        }

        public static void OnUndo()
        {
            try
            {
                LoadParts(CASHair.gSingleton);
                PopulateTypesGrid();
            }
            catch (Exception e)
            {
                Common.Exception("OnUndo", e);
            }
        }

        public static void OnRedo()
        {
            try
            {
                LoadParts(CASHair.gSingleton);
                PopulateTypesGrid();
            }
            catch (Exception e)
            {
                Common.Exception("OnRedo", e);
            }
        }

        private static void LoadParts(CASHair ths)
        {
            if (ths == null) return;

            ths.mPartsList.Clear();
            foreach (CASParts.Wrapper part in GetVisibleCASParts(CASLogic.GetSingleton(), BodyTypes.Hair))
            {
                if (ths.PartMatchesHairType(part.mPart))
                {
                    ths.mPartsList.Add(part.mPart);
                }
            }
        }

        public static void RequestRandomPart(bool hat)
        {
            List<CASParts.Wrapper> fullList = GetVisibleCASParts(CASLogic.GetSingleton(), BodyTypes.Hair);

            List<CASParts.Wrapper> list = new List<CASParts.Wrapper>();
            foreach (CASParts.Wrapper part in fullList)
            {
                if (hat != CASHair.PartIsHat(part.mPart)) continue;

                list.Add(part);
            }

            int count = list.Count;
            if (count > 0)
            {
                CASParts.Wrapper part = list[(int)(count * RandomGen.NextDouble())];

                Sims3.UI.Responder.Instance.CASModel.RequestAddCASPart(part.mPart, false);
            }
        }

        private static void PopulateTypesGrid()
        {
            CASHair ths = CASHair.gSingleton;

            LoadParts(ths); 
            PopulateTypesGrid(ths);
        }
        private static void PopulateTypesGrid(CASHair ths)
        {
            if (ths == null) return;

            ICASModel cASModel = Responder.Instance.CASModel;
            Color[] colors = new Color[] { new Color(0x0), new Color(0x0), new Color(0x0), new Color(0x0) };
            ths.mHairTypesGrid.Clear();
            CASPart wornPart = ths.GetWornPart();
            ResourceKey resKey = ResourceKey.CreateUILayoutKey("GenericCasItem", 0x0);

            bool isHat = false;
            bool flag = false;
            if (ths.mHairType == CASHair.HairType.Hat)
            {
                ths.mHatsShareButton.Enabled = false;
                ths.mHatsDeleteButton.Enabled = false;
                ths.mDesignButton.Enabled = CASHair.PartIsHat(wornPart);

                isHat = true;
            }

            bool shouldEnableCatalogProductFilter = false;

            List<object> objectList = Responder.Instance.StoreUI.GetCASFeaturedStoreItems(BodyTypes.Hair, cASModel.OutfitCategory, (cASModel.Age | cASModel.Species) | cASModel.Gender, isHat);
            ths.mContentTypeFilter.FilterObjects(objectList, out shouldEnableCatalogProductFilter);

            if (!MasterController.Settings.mCompactHatCAS)
            {
                foreach (object obj2 in objectList)
                {
                    IFeaturedStoreItem item = obj2 as IFeaturedStoreItem;

                    if ((ths.mHairType == CASHair.HairType.Hat) == (0x0 != (item.CategoryFlags & 0x400000)))
                    {
                        WindowBase windowByExportID = UIManager.LoadLayout(resKey).GetWindowByExportID(0x1);
                        if (windowByExportID != null)
                        {
                            windowByExportID.Tag = item;
                            windowByExportID.GetChildByID(0x23, true);
                            Window childByID = windowByExportID.GetChildByID(0x20, true) as Window;
                            if (childByID != null)
                            {
                                ImageDrawable drawable = childByID.Drawable as ImageDrawable;
                                if (drawable != null)
                                {
                                    drawable.Image = UIUtils.GetUIImageFromThumbnailKey(item.ThumbKey);
                                    childByID.Invalidate();
                                }
                            }
                            childByID = windowByExportID.GetChildByID(0x300, true) as Window;
                            childByID.Tag = item;
                            childByID.CreateTooltipCallbackFunction = ths.StoreItemCreateTooltip;
                            childByID.Visible = true;
                            childByID = windowByExportID.GetChildByID(0x303, true) as Window;
                            childByID.Visible = item.IsSale;

                            Button button = windowByExportID.GetChildByID(0x301, true) as Button;
                            button.Caption = item.PriceString;
                            button.Tag = windowByExportID;
                            button.Click += ths.OnBuyButtonClick;
                            button.FocusAcquired += ths.OnBuyButtonFocusAcquired;
                            button.FocusLost += ths.OnBuyButtonFocusLost;
                            ths.mHairTypesGrid.AddItem(new ItemGridCellItem(windowByExportID, item));
                        }
                    }
                }
            }

            foreach (CASPart part2 in ths.mPartsList)
            {
                bool isWardrobePart = Responder.Instance.CASModel.ActiveWardrobeContains(part2);
                uint num3 = CASUtils.PartDataNumPresets(part2.Key);
                ResourceKeyContentCategory customContentType = UIUtils.GetCustomContentType(part2.Key);
                if (!UIUtils.IsContentTypeDisabled(UIUtils.GetCustomContentType(part2.Key)))
                {
                    ObjectDesigner.SetCASPart(part2.Key);
                    string designPreset = ObjectDesigner.GetDesignPreset(ObjectDesigner.GetDesignPresetIndexFromId(ObjectDesigner.DefaultPresetId));
                    if (string.IsNullOrEmpty(designPreset))
                    {
                        ResourceKey key2 = new ResourceKey(part2.Key.InstanceId, 0x333406c, part2.Key.GroupId);
                        designPreset = Simulator.LoadXMLString(key2);
                    }

                    CASPartPreset preset = new CASPartPreset(part2, designPreset);
                    string str2 = "";
                    string str3 = "";
                    if (wornPart.Key == preset.mPart.Key)
                    {
                        str2 = Responder.Instance.CASModel.GetDesignPreset(wornPart);
                        str3 = CASUtils.ReplaceHairColors(str2, colors);
                    }

                    if (preset.Valid && ((ths.mHairType == CASHair.HairType.Hair) || (ObjectDesigner.DefaultPresetId == uint.MaxValue)))
                    {
                        ths.AddHairTypeGridItem(ths.mHairTypesGrid, resKey, preset, isWardrobePart, ref shouldEnableCatalogProductFilter);
                        if ((preset.mPart.Key == wornPart.Key) && ((ths.mHairType == CASHair.HairType.Hair) || CASUtils.DesignPresetCompare(str2, designPreset)))
                        {
                            ths.mHairTypesGrid.SelectedItem = ths.mHairTypesGrid.Count - 1;
                            flag = true;
                        }
                    }

                    if (ths.mHairType == CASHair.HairType.Hat)
                    {
                        if (MasterController.Settings.mCompactHatCAS)
                        {
                            num3 = 1;
                        }

                        for (int i = 0x0; i < num3; i++)
                        {
                            uint presetId = CASUtils.PartDataGetPresetId(part2.Key, (uint)i);
                            customContentType = UIUtils.GetCustomContentType(part2.Key, presetId);

                            preset = new CASPartPreset(part2, presetId, CASUtils.PartDataGetPreset(part2.Key, (uint)i));
                            if (preset.Valid)
                            {
                                bool flag4 = ths.AddHairTypeGridItem(ths.mHairTypesGrid, resKey, preset, isWardrobePart, ref shouldEnableCatalogProductFilter);
                                if ((wornPart.Key == preset.mPart.Key) && CASUtils.DesignPresetCompare(str3, CASUtils.ReplaceHairColors(preset.mPresetString, colors)))
                                {
                                    ths.mSavedPresetId = preset.mPresetId;
                                    flag = true;
                                    if (flag4)
                                    {
                                        ths.mHairTypesGrid.SelectedItem = ths.mHairTypesGrid.Count - 1;
                                        if (ObjectDesigner.IsUserDesignPreset((uint)i))
                                        {
                                            ths.mHatsShareButton.Enabled = true;
                                            ths.mHatsDeleteButton.Enabled = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ths.mHairTypesGrid.Tag = shouldEnableCatalogProductFilter;
            if (ths.mHairStylesGrid.Tag == null)
            {
                ths.mHairStylesGrid.Tag = false;
            }

            ths.mSortButton.Tag = ((bool)ths.mHairTypesGrid.Tag) ? (true) : (bool)ths.mHairStylesGrid.Tag;
            if (flag)
            {
                ths.mSaveButton.Enabled = false;
            }
            else if ((ths.mHairType == CASHair.HairType.Hat) && CASHair.PartIsHat(wornPart))
            {
                WindowBase win = UIManager.LoadLayout(resKey).GetWindowByExportID(0x1);
                if (win != null)
                {
                    Window window2 = win.GetChildByID(0x20, true) as Window;
                    if (window2 != null)
                    {
                        window2.Visible = false;
                    }
                    window2 = win.GetChildByID(0x24, true) as Window;
                    if (window2 != null)
                    {
                        window2.Visible = true;
                    }
                    ths.mHairTypesGrid.AddTempItem(new ItemGridCellItem(win, null));
                }
                ths.mSaveButton.Enabled = true;
            }
            ths.mUndoOnDelete = false;
            ths.mContentTypeFilter.UpdateFilterButtonState();
        }

        private static void SetHairTypeCategory(CASHair ths, CASHair.HairType cat)
        {
            try
            {
                ths.mHairType = cat;
                CASPart wornPart = ths.GetWornPart();
                ths.mHatsCustomWindow.Visible = ths.mHairType == CASHair.HairType.Hat;
                CASPhysical.gSingleton.SetLongPanel(ths.mHairType == CASHair.HairType.Hair);
                if (ths.mHairType == CASHair.HairType.Hat)
                {
                    ths.mHairColorPanel.Position = new Vector2(ths.mHairColorPanel.Position.x, 421f);
                    CASPhysical.gSingleton.SetShortPanelHeight(ths.mHatsCustomWindow.Area.Height);
                }
                else
                {
                    ths.mHairColorPanel.Position = new Vector2(ths.mHairColorPanel.Position.x, 395f);
                }

                ths.mDesignButton.Enabled = CASHair.PartIsHat(wornPart);
                LoadParts(ths);
                PopulateTypesGrid(ths);
                if (ths.mHairType == CASHair.HairType.Hair)
                {
                    ths.ReselectCurrentHairPresetItem();
                }
            }
            catch (Exception e)
            {
                Common.Exception("SetHairTypeCategory", e);
            }
        }

        private static void HideUnusedIcons(CASHair ths)
        {
            List<Button> list = new List<Button>();
            int num = 0x0;
            int num2 = 0x0;
            foreach (CASParts.Wrapper part in GetVisibleCASParts(CASLogic.GetSingleton(), BodyTypes.Hair))
            {
                if (CASHair.PartIsHat(part.mPart))
                {
                    num2++;
                }
                else
                {
                    num++;
                }
            }

            Button childByID = ths.GetChildByID(0x5dbb905, true) as Button;
            if (num > 0x0)
            {
                childByID.Visible = true;
                list.Add(childByID);
            }
            else
            {
                childByID.Visible = false;
            }

            childByID = ths.GetChildByID(0x5dbb906, true) as Button;
            if (num2 > 0x0)
            {
                childByID.Visible = true;
                list.Add(childByID);
            }
            else
            {
                childByID.Visible = false;
            }

            float count = list.Count;
            float num4 = (210f - ((count * 0.5f) * 42f)) - (((count - 1f) * 0.5f) * -2f);
            float num5 = num4;
            foreach (Button button2 in list)
            {
                button2.Position = new Vector2(num5, button2.Position.y);
                num5 += 40f;
            }
        }

        public static void InitialPopulate(CASHair ths)
        {
            HideUnusedIcons(ths);

            if (CASHair.PartIsHat(ths.GetWornPart()))
            {
                SetHairTypeCategory(ths, CASHair.HairType.Hat);
            }
            else
            {
                SetHairTypeCategory(ths, CASHair.HairType.Hair);
            }
        }

        public static void OnTypesGridItemClicked(WindowBase sender, ItemGridCellClickEvent itemClicked)
        {
            try
            {
                CASHair ths = CASHair.gSingleton;

                CASPartPreset part = itemClicked.mTag as CASPartPreset;
                if (part != null)
                {
                    if (itemClicked.mButton == MouseKeys.kMouseLeft)
                    {
                        ths.mSavedPresetId = part.mPresetId;
                        if (part.mPresetString != null)
                        {
                            if (part.mPart.Key != ths.GetWornPart().Key)
                            {
                                Responder.Instance.CASModel.RequestAddCASPart(part.mPart, part.mPresetString);
                            }
                            else
                            {
                                Responder.Instance.CASModel.RequestCommitPresetToPart(part.mPart, part.mPresetString);
                            }
                        }

                        ths.mHairStylesGrid.SelectedItem = -1;
                        ths.mDesignButton.Enabled = CASHair.PartIsHat(part.mPart);
                        ths.mHairTypesGrid.RemoveTempItem();
                        ths.mSaveButton.Enabled = false;
                        ObjectDesigner.SetCASPart(part.mPart.Key);
                        if (UIUtils.GetCustomContentType(part.mPart.Key, part.mPresetId) == ResourceKeyContentCategory.kLocalUserCreated)
                        {
                            ths.mHatsDeleteButton.Enabled = true;
                            ths.mHatsShareButton.Enabled = true;
                            ths.mUndoOnDelete = true;
                        }
                        else
                        {
                            ths.mUndoOnDelete = false;
                            ths.mHatsDeleteButton.Enabled = false;
                            ths.mHatsShareButton.Enabled = false;
                        }

                        Audio.StartSound("ui_tertiary_button");
                    }
                    else if (itemClicked.mButton == MouseKeys.kMouseRight)
                    {
                        CASBase.Blacklist(new CASParts.Wrapper(part.mPart), ((itemClicked.mModifiers & Modifiers.kModifierMaskControl) == Modifiers.kModifierMaskControl), PopulateTypesGrid);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnTypesGridItemMouseDown", e);
            }
        }

        public static void OnHairButtonMouseDown(WindowBase sender, UIMouseEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.MouseKey == MouseKeys.kMouseLeft)
                {
                    CASHair ths = CASHair.gSingleton;

                    SetHairTypeCategory(ths, CASHair.HairType.Hair);
                }
                else if (eventArgs.MouseKey == MouseKeys.kMouseRight)
                {
                    RequestRandomPart(false);

                    eventArgs.mHandled = true;
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnHairButtonMouseDown", e);
            }
        }

        public static void OnHatsButtonMouseDown(WindowBase sender, UIMouseEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.MouseKey == MouseKeys.kMouseLeft)
                {
                    CASHair ths = CASHair.gSingleton;

                    SetHairTypeCategory(ths, CASHair.HairType.Hat);
                }
                else if (eventArgs.MouseKey == MouseKeys.kMouseRight)
                {
                    RequestRandomPart(true);

                    eventArgs.mHandled = true;
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnHatsButtonMouseDown", e);
            }
        }
    }
}
