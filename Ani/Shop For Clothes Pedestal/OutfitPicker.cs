using System.Collections.Generic;
using Sims3.Gameplay.Actors;
using Sims3.UI;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Miscellaneous.Shopping.ani_ClothingPedestal;
using System.Collections;
using System;

namespace ani_ClothingPedestal
{
    public static class OutfitPicker
    {
        public static void PopulatePieMenuPickerWithOutfits(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
        {
           OutfitPicker.PopulatePieMenuPickerWithOutfits(parameters.Target as CustomPedestal, parameters.Actor as Sim, out listObjs, out headers, out NumSelectableRows, null);
        }

        public static void PopulatePieMenuPickerWithOutfits(CustomPedestal pedestal, Sim actor, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows, OutfitUtils.GreyedOutOutfitCallback greyed)
        {
            if (pedestal == null)
                CMShopping.PrintMessage("Pedestal null");
            NumSelectableRows = 1;
            headers = new List<ObjectPicker.HeaderInfo>();
            listObjs = new List<ObjectPicker.TabInfo>();
            headers.Add(new ObjectPicker.HeaderInfo("Ui/Caption/ObjectPicker:Outfit", "Ui/Tooltip/ObjectPicker:Outfit"));
            headers[0].Width = 250;
            if (actor != null)
            {
                SimDescription simDescription = actor.SimDescription;
                ResourceKey resourceKey = CASUtils.GetOutfitInGameObject(actor.ObjectId);
                if (simDescription.HasSupernaturalOutfit(resourceKey) && simDescription.CreatedSim != null)
                {
                    OutfitCategories currentOutfitCategory = simDescription.CreatedSim.CurrentOutfitCategory;
                    int currentOutfitIndex = simDescription.CreatedSim.CurrentOutfitIndex;
                    SimOutfit outfit = simDescription.GetOutfit(currentOutfitCategory, currentOutfitIndex);
                    if (outfit != null && outfit.Key != ResourceKey.kInvalidResourceKey)
                    {
                        resourceKey = outfit.Key;
                    }
                }
                ObjectPicker.TabInfo tabInfo = new ObjectPicker.TabInfo("glb_all_r2", Localization.LocalizeString("Ui/Caption/ObjectPicker:All", new object[0]), new List<ObjectPicker.RowInfo>());
                string text = Localization.LocalizeString("Ui/Caption/ObjectPicker:Everyday", new object[0]);
                string text2 = Localization.LocalizeString("Ui/Caption/ObjectPicker:Formalwear", new object[0]);
                string text3 = Localization.LocalizeString("Ui/Caption/ObjectPicker:Sleepwear", new object[0]);
                string text4 = null;
                string text5 = null;
                string text6 = null;
                string text7 = null;
                string text8 = null;
                string text9 = null;
                string text10 = null;
                ObjectPicker.TabInfo tabInfo2 = new ObjectPicker.TabInfo(actor.IsHorse ? "cas_acc_riding" : "shopping_clothing_everyday_r2", text, new List<ObjectPicker.RowInfo>());
                ObjectPicker.TabInfo tabInfo3 = new ObjectPicker.TabInfo("shopping_clothing_formal_r2", text2, new List<ObjectPicker.RowInfo>());
                ObjectPicker.TabInfo tabInfo4 = new ObjectPicker.TabInfo("shopping_clothing_sleepwear_r2", text3, new List<ObjectPicker.RowInfo>());
                ObjectPicker.TabInfo tabInfo5 = null;
                ObjectPicker.TabInfo tabInfo6 = null;
                ObjectPicker.TabInfo tabInfo7 = null;
                ObjectPicker.TabInfo tabInfo8 = null;
                ObjectPicker.TabInfo tabInfo9 = null;
                ObjectPicker.TabInfo tabInfo10 = null;
                ObjectPicker.TabInfo tabInfo11 = null;
                ObjectPicker.TabInfo tabInfo12 = null;
                if (simDescription.ToddlerOrAbove)
                {
                    if (!simDescription.Toddler)
                    {
                        text4 = Localization.LocalizeString("Ui/Caption/ObjectPicker:Swimwear", new object[0]);
                        text5 = Localization.LocalizeString("Ui/Caption/ObjectPicker:Athletic", new object[0]);
                        tabInfo5 = new ObjectPicker.TabInfo("shopping_clothing_swimwear_r2", text4, new List<ObjectPicker.RowInfo>());
                        tabInfo6 = new ObjectPicker.TabInfo("shopping_clothing_athletic_r2", text5, new List<ObjectPicker.RowInfo>());
                    }
                    if (GameUtils.IsInstalled(ProductVersion.EP8))
                    {
                        text10 = Localization.LocalizeString("Ui/Caption/ObjectPicker:Outerwear", new object[0]);
                        tabInfo12 = new ObjectPicker.TabInfo("cas_clothing_i_outerwear", text10, new List<ObjectPicker.RowInfo>());
                    }
                }
                if (simDescription.IsHorse)
                {
                    text7 = Localization.LocalizeString("Ui/Caption/ObjectPicker:Racing", new object[0]);
                    tabInfo9 = new ObjectPicker.TabInfo("cas_acc_racing", text7, new List<ObjectPicker.RowInfo>());
                    text8 = Localization.LocalizeString("Ui/Caption/ObjectPicker:Jumping", new object[0]);
                    tabInfo10 = new ObjectPicker.TabInfo("cas_acc_jumping", text8, new List<ObjectPicker.RowInfo>());
                    text9 = Localization.LocalizeString("Ui/Caption/ObjectPicker:BridleOnly", new object[0]);
                    tabInfo11 = new ObjectPicker.TabInfo("cas_acc_horse_bridles", text9, new List<ObjectPicker.RowInfo>());
                }
                SkillManager skillManager = actor.SkillManager;
                if (skillManager != null && skillManager.HasElement(SkillNames.MartialArts) && !simDescription.IsVisuallyPregnant)
                {
                    headers[0].Width += 50;
                    text6 = Localization.LocalizeString("Ui/Caption/ObjectPicker:MartialArts", new object[0]);
                    tabInfo7 = new ObjectPicker.TabInfo("w_simple_martialarts_skill_s", text6, new List<ObjectPicker.RowInfo>());
                }
                if (actor.Occupation != null)
                {
                    SimOutfit outfit2 = simDescription.GetOutfit(OutfitCategories.Career, 0);
                    if (outfit2 != null)
                    {
                        headers[0].Width += 50;
                        string tabText = Localization.LocalizeString("Ui/Caption/ObjectPicker:Miscellaneous", new object[0]);
                        tabInfo8 = new ObjectPicker.TabInfo("glb_i_options32", tabText, new List<ObjectPicker.RowInfo>());
                    }
                }
                string textLabel = Localization.LocalizeString("Ui/Caption/ObjectPicker:Career", new object[0]);
                if (!actor.IsFoal)
                {
                    OutfitPicker.PopulatePieMenuPickerWithCategoryOutfits(pedestal, simDescription, resourceKey, tabInfo, OutfitCategories.Everyday, text, tabInfo2, greyed);
                }
                OutfitPicker.PopulatePieMenuPickerWithCategoryOutfits(pedestal, simDescription, resourceKey, tabInfo, OutfitCategories.Formalwear, text2, tabInfo3, greyed);
                OutfitPicker.PopulatePieMenuPickerWithCategoryOutfits(pedestal, simDescription, resourceKey, tabInfo, OutfitCategories.Sleepwear, text3, tabInfo4, greyed);
                OutfitPicker.PopulatePieMenuPickerWithCategoryOutfits(pedestal, simDescription, resourceKey, tabInfo, OutfitCategories.Swimwear, text4, tabInfo5, greyed);
                OutfitPicker.PopulatePieMenuPickerWithCategoryOutfits(pedestal, simDescription, resourceKey, tabInfo, OutfitCategories.Athletic, text5, tabInfo6, greyed);
                if (tabInfo12 != null)
                {
                    OutfitPicker.PopulatePieMenuPickerWithCategoryOutfits(pedestal, simDescription, resourceKey, tabInfo, OutfitCategories.Outerwear, text10, tabInfo12, greyed);
                }
                OutfitPicker.PopulatePieMenuPickerWithCategoryOutfits(pedestal, simDescription, resourceKey, tabInfo, OutfitCategories.MartialArts, text6, tabInfo7, greyed);
                OutfitPicker.PopulatePieMenuPickerWithCategoryOutfits(pedestal, simDescription, resourceKey, tabInfo, OutfitCategories.Career, textLabel, tabInfo8, greyed);
                OutfitPicker.PopulatePieMenuPickerWithCategoryOutfits(pedestal, simDescription, resourceKey, tabInfo, OutfitCategories.Bridle, text9, tabInfo11, greyed);
                Racing racing = skillManager.GetElement(SkillNames.Racing) as Racing;
                if (racing != null && racing.HasRacingSaddleOutfit)
                {
                    OutfitPicker.PopulatePieMenuPickerWithCategoryOutfits(pedestal, simDescription, resourceKey, tabInfo, OutfitCategories.Racing, text7, tabInfo9, greyed);
                }
                Jumping jumping = skillManager.GetElement(SkillNames.Jumping) as Jumping;
                if (jumping != null && jumping.HasJumpOutfit)
                {
                    OutfitPicker.PopulatePieMenuPickerWithCategoryOutfits(pedestal, simDescription, resourceKey, tabInfo, OutfitCategories.Jumping, text8, tabInfo10, greyed);
                }
                listObjs.Add(tabInfo);
                if (tabInfo2.RowInfo.Count > 0)
                {
                    listObjs.Add(tabInfo2);
                }
                if (tabInfo3.RowInfo.Count > 0)
                {
                    listObjs.Add(tabInfo3);
                }
                if (tabInfo4.RowInfo.Count > 0)
                {
                    listObjs.Add(tabInfo4);
                }
                if (tabInfo5 != null && tabInfo5.RowInfo.Count > 0)
                {
                    listObjs.Add(tabInfo5);
                }
                if (tabInfo6 != null && tabInfo6.RowInfo.Count > 0)
                {
                    listObjs.Add(tabInfo6);
                }
                if (tabInfo12 != null && tabInfo12.RowInfo.Count > 0)
                {
                    listObjs.Add(tabInfo12);
                }
                if (tabInfo7 != null && tabInfo7.RowInfo.Count > 0)
                {
                    listObjs.Add(tabInfo7);
                }
                if (tabInfo11 != null && tabInfo11.RowInfo.Count > 0)
                {
                    listObjs.Add(tabInfo11);
                }
                if (tabInfo9 != null && tabInfo9.RowInfo.Count > 0)
                {
                    listObjs.Add(tabInfo9);
                }
                if (tabInfo10 != null && tabInfo10.RowInfo.Count > 0)
                {
                    listObjs.Add(tabInfo10);
                }
                if (tabInfo8 != null && tabInfo8.RowInfo.Count > 0)
                {
                    listObjs.Add(tabInfo8);
                }
            }
        }
    
        // Sims3.Gameplay.CAS.OutfitUtils
        public static void PopulatePieMenuPickerWithCategoryOutfits(CustomPedestal pedestal, SimDescription simDesc, ResourceKey currentOutfitKey, ObjectPicker.TabInfo allTab, OutfitCategories category, string textLabel, ObjectPicker.TabInfo tab, OutfitUtils.GreyedOutOutfitCallback greyed)
        {
            if (tab == null)
            {
                return;
            }
            int num = 1;

            simDesc = new SimDescription();
            if (simDesc == null)
            {
                throw new Exception("ChangeOutfit:  sim doesn't have a description!");
            }
            //CMShopping.PrintMessage(Target.DisplayCategory.ToString());

            pedestal.PedestalOutfitsSaveTo(simDesc);

            if (pedestal.DisplayCategory == category)
            {
                foreach (SimOutfit simOutfit in simDesc.GetOutfits(category))
                {
                    if (simOutfit.Key != currentOutfitKey)
                    {
                        ObjectPicker.RowInfo rowInfo = new ObjectPicker.RowInfo(simOutfit.Key, new List<ObjectPicker.ColumnInfo>());
                        ThumbnailKey thumbnail = new ThumbnailKey(simOutfit, -4, ThumbnailSize.Medium, simDesc.IsHorse ? ThumbnailCamera.Side : ThumbnailCamera.Body, (uint)simDesc.AgeGenderSpecies);
                        rowInfo.ColumnInfo.Add(new ObjectPicker.ThumbAndTextColumn(thumbnail, textLabel + " " + num));
                        allTab.RowInfo.Add(rowInfo);
                        tab.RowInfo.Add(rowInfo);
                    }
                    num++;
                }
            }
         
            //OutfitCategoryMap pedestalOutfits = pedestal.GetPedestalOutfits(pedestal.mDisplayType);
            //foreach (OutfitCategories outfitCategories in pedestalOutfits.Keys)
            //{
            //    ArrayList arrayList = pedestalOutfits[category] as ArrayList;
            //    CMShopping.PrintMessage("arraylist: " + arrayList.Count);
            //    for (int i = 0; i < arrayList.Count; i++)
            //    {
            //        CMShopping.PrintMessage(category + " " + i);
            //        SimOutfit simOutfit = (SimOutfit)arrayList[i];
            //        ObjectPicker.RowInfo rowInfo = new ObjectPicker.RowInfo(simOutfit.Key, new List<ObjectPicker.ColumnInfo>());
            //        ThumbnailKey thumbnail = new ThumbnailKey(simOutfit, -4, ThumbnailSize.Medium, simDesc.IsHorse ? ThumbnailCamera.Side : ThumbnailCamera.Body, (uint)simDesc.AgeGenderSpecies);
            //        rowInfo.ColumnInfo.Add(new ObjectPicker.ThumbAndTextColumn(thumbnail, textLabel + " " + num));
            //        allTab.RowInfo.Add(rowInfo);
            //        tab.RowInfo.Add(rowInfo);

            //        //simDesc.AddOutfit((SimOutfit)arrayList[i], outfitCategories);
            //    }
            //}


            //foreach (SimOutfit simOutfit in simDesc.GetOutfits(category))
            //{
            //    if (simOutfit.Key != currentOutfitKey)
            //    {
            //        ObjectPicker.RowInfo rowInfo = new ObjectPicker.RowInfo(simOutfit.Key, new List<ObjectPicker.ColumnInfo>());
            //        ThumbnailKey thumbnail = new ThumbnailKey(simOutfit, -4, ThumbnailSize.Medium, simDesc.IsHorse ? ThumbnailCamera.Side : ThumbnailCamera.Body, (uint)simDesc.AgeGenderSpecies);
            //        rowInfo.ColumnInfo.Add(new ObjectPicker.ThumbAndTextColumn(thumbnail, textLabel + " " + num));
            //        allTab.RowInfo.Add(rowInfo);
            //        tab.RowInfo.Add(rowInfo);
            //    }
            //    num++;
            //}
        }
    }
}
