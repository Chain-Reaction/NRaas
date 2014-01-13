using NRaas.CommonSpace.Tasks;
using NRaas.MasterControllerSpace.CAS;
using NRaas.MasterControllerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Tutorial;
using Sims3.Metadata;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.CAS.CAP;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Tasks
{
    public class EditTownCAS : Common.IPreLoad
    {
        public void OnPreLoad()
        {
            EditTownCASTask.Create<EditTownCASTask>();
        }

        protected class EditTownCASTask : RepeatingTask
        {
            protected override bool OnPerform()
            {
                Common.StringBuilder msg = new Common.StringBuilder("EditTownCASTask" + Common.NewLine);

                try
                {
                    {
                        EditTownLibraryPanel panel = EditTownLibraryPanel.Instance;
                        if ((panel != null) && (panel.mCASButton != null))
                        {
                            panel.mCASButton.Click -= panel.OnCASClick;

                            panel.mCASButton.Click -= EditTownLibraryPanelEx.OnCASClick;
                            panel.mCASButton.Click += EditTownLibraryPanelEx.OnCASClick;
                        }
                    }

                    msg += "A";

                    {
                        PlayFlowMenuPanel playFlow = PlayFlowMenuPanel.gSingleton;
                        if ((playFlow != null) && (playFlow.mCASButton != null))
                        {
                            playFlow.mCASButton.Click -= playFlow.OnMenuButtonClick;

                            playFlow.mCASButton.Click -= PlayFlowMenuPanelEx.OnMenuButtonClick;
                            playFlow.mCASButton.Click += PlayFlowMenuPanelEx.OnMenuButtonClick;
                        }
                    }

                    msg += "C";

                    {
                        CASClothingCategory category = CASClothingCategory.gSingleton;
                        if (category != null)
                        {
                            category.mTopsButton.Click -= category.OnCategoryButtonClick;
                            category.mBottomsButton.Click -= category.OnCategoryButtonClick;
                            category.mShoesButton.Click -= category.OnCategoryButtonClick;
                            category.mOutfitsButton.Click -= category.OnCategoryButtonClick;
                            category.mAccessoriesButton.Click -= category.OnCategoryButtonClick;
                            category.mHorseBridlesButton.Click -= category.OnCategoryButtonClick;
                            category.mHorseSaddleButton.Click -= category.OnCategoryButtonClick;

                            category.mTopsButton.MouseDown -= CASClothingCategoryEx.OnButtonMouseDown;
                            category.mTopsButton.MouseDown += CASClothingCategoryEx.OnButtonMouseDown;
                            category.mBottomsButton.MouseDown -= CASClothingCategoryEx.OnButtonMouseDown;
                            category.mBottomsButton.MouseDown += CASClothingCategoryEx.OnButtonMouseDown;
                            category.mShoesButton.MouseDown -= CASClothingCategoryEx.OnButtonMouseDown;
                            category.mShoesButton.MouseDown += CASClothingCategoryEx.OnButtonMouseDown;
                            category.mOutfitsButton.MouseDown -= CASClothingCategoryEx.OnButtonMouseDown;
                            category.mOutfitsButton.MouseDown += CASClothingCategoryEx.OnButtonMouseDown;
                            category.mAccessoriesButton.MouseDown -= CASClothingCategoryEx.OnButtonMouseDown;
                            category.mAccessoriesButton.MouseDown += CASClothingCategoryEx.OnButtonMouseDown;
                            category.mHorseBridlesButton.MouseDown -= CASClothingCategoryEx.OnButtonMouseDown;
                            category.mHorseBridlesButton.MouseDown += CASClothingCategoryEx.OnButtonMouseDown;
                            category.mHorseSaddleButton.MouseDown -= CASClothingCategoryEx.OnButtonMouseDown;
                            category.mHorseSaddleButton.MouseDown += CASClothingCategoryEx.OnButtonMouseDown;
                        }
                    }

                    msg += "D";

                    CASPuck puck = CASPuck.gSingleton;
                    if (puck != null)
                    {
                        CASFamilyScreen familyScreen = CASFamilyScreen.gSingleton;
                        if (familyScreen != null)
                        {
                            Window topLevel = familyScreen.mFamilyTopLevelWin;

                            uint index = 0;
                            WindowBase child = topLevel.GetChildByIndex(index);
                            while (child != null)
                            {
                                CAFThumb thumb = child as CAFThumb;
                                if (thumb != null)
                                {
                                    thumb.DragDrop -= familyScreen.OnCAFThumbDragDrop;
                                    thumb.DragDrop -= CASFamilyScreenEx.OnCAFThumbDragDrop;
                                    thumb.DragDrop += CASFamilyScreenEx.OnCAFThumbDragDrop;
                                }

                                index++;
                                child = topLevel.GetChildByIndex(index);
                            }
                        }

                        if (puck.mGeneticsButton != null)
                        {
                            puck.mGeneticsButton.Enabled = CASPuckEx.CanCreateChild();
                        }

                        ICASModel cASModel = Responder.Instance.CASModel;
                        if (cASModel != null)
                        {
                            if ((MasterController.Settings.mAllowOverStuffed) && (cASModel.NumInHousehold < CASPuck.kMaxPerHousehold))
                            {
                                if (puck.mCreateHorseButton != null)
                                {
                                    puck.mCreateHorseButton.Enabled = true;

                                    puck.mCreateHorseButton.Click -= puck.OnCreateSimClick;
                                    puck.mCreateHorseButton.Click -= CASPuckEx.OnCreateSimClick;
                                    puck.mCreateHorseButton.Click += CASPuckEx.OnCreateSimClick;
                                }

                                if (puck.mCreateDogButton != null)
                                {
                                    puck.mCreateDogButton.Enabled = true;

                                    puck.mCreateDogButton.Click -= puck.OnCreateSimClick;
                                    puck.mCreateDogButton.Click -= CASPuckEx.OnCreateSimClick;
                                    puck.mCreateDogButton.Click += CASPuckEx.OnCreateSimClick;
                                }

                                if (puck.mCreateCatButton != null)
                                {
                                    puck.mCreateCatButton.Enabled = true;

                                    puck.mCreateCatButton.Click -= puck.OnCreateSimClick;
                                    puck.mCreateCatButton.Click -= CASPuckEx.OnCreateSimClick;
                                    puck.mCreateCatButton.Click += CASPuckEx.OnCreateSimClick;
                                }

                                if (puck.mCreateSimButton != null)
                                {
                                    puck.mCreateSimButton.Enabled = true;

                                    puck.mCreateSimButton.Click -= puck.OnCreateSimClick;
                                    puck.mCreateSimButton.Click -= CASPuckEx.OnCreateSimClick;
                                    puck.mCreateSimButton.Click += CASPuckEx.OnCreateSimClick;
                                }
                            }

                            cASModel.OnSimUpdated -= puck.OnSimUpdated;
                            cASModel.OnSimUpdated -= CASPuckEx.OnSimUpdated;
                            cASModel.OnSimUpdated += CASPuckEx.OnSimUpdated;

                            cASModel.OnSimPreviewChange -= puck.OnSimPreviewChange;
                            cASModel.OnSimPreviewChange -= CASPuckEx.OnSimPreviewChange;
                            cASModel.OnSimPreviewChange += CASPuckEx.OnSimPreviewChange;
                        }
                    }

                    msg += "E";

                    CASCompositorController controller = CASCompositorController.sController;
                    if (controller != null)
                    {
                        if (controller.mColorsDragButton != null)
                        {
                            controller.mColorsDragButton.MouseDown -= controller.OnMaterialsColorDragMouseDown;
                            controller.mColorsDragButton.MouseDown -= CASCompositorControllerEx.OnMaterialsColorDragMouseDown;
                            controller.mColorsDragButton.MouseDown += CASCompositorControllerEx.OnMaterialsColorDragMouseDown;

                            controller.mColorsDragButton.MouseUp -= controller.OnMaterialsColorDragMouseUp;
                            controller.mColorsDragButton.MouseUp -= CASCompositorControllerEx.OnMaterialsColorDragMouseUp;
                            controller.mColorsDragButton.MouseUp += CASCompositorControllerEx.OnMaterialsColorDragMouseUp;

                            for (uint j = 0x0; j < 0x4; j++)
                            {       
                                controller.mColorsPopupButton[j].MouseDown -= controller.OnMaterialsColorGridMouseDown;
                                controller.mColorsPopupButton[j].MouseDown -= CASCompositorControllerEx.OnMaterialsColorGridMouseDown;
                                controller.mColorsPopupButton[j].MouseDown += CASCompositorControllerEx.OnMaterialsColorGridMouseDown;

                                controller.mColorsPopupButton[j].MouseUp -= controller.OnMaterialsColorGridMouseUp;
                                controller.mColorsPopupButton[j].MouseUp -= CASCompositorControllerEx.OnMaterialsColorGridMouseUp;
                                controller.mColorsPopupButton[j].MouseUp += CASCompositorControllerEx.OnMaterialsColorGridMouseUp;
                            }
                        }

                        if (controller.mMaterialsSkewerPatternButton.Length == 4)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                if (controller.mMaterialsSkewerPatternButton[i] == null) continue;

                                controller.mMaterialsSkewerPatternButton[i].MouseDown -= controller.OnMaterialsSkewerGridMouseDown;

                                controller.mMaterialsSkewerPatternButton[i].MouseDown -= CASCompositorControllerEx.OnMaterialsSkewerGridMouseDown;
                                controller.mMaterialsSkewerPatternButton[i].MouseDown += CASCompositorControllerEx.OnMaterialsSkewerGridMouseDown;

                                controller.mMaterialsSkewerPatternButton[i].MouseUp -= controller.OnMaterialsSkewerGridMouseUp;

                                controller.mMaterialsSkewerPatternButton[i].MouseUp -= CASCompositorControllerEx.OnMaterialsSkewerGridMouseUp;
                                controller.mMaterialsSkewerPatternButton[i].MouseUp += CASCompositorControllerEx.OnMaterialsSkewerGridMouseUp;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(msg, e);
                }
                return true;
            }
        }
    }
}
