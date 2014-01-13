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
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.CAS.CAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.CAS
{
    public class CAPUnicornEx
    {
        public static void OnHornColorGridClicked(ItemGrid sender, ItemGridCellClickEvent itemClicked)
        {
            try
            {
                CAPUnicorn ths = CAPUnicorn.gSingleton;
                if (ths == null) return;

                CASPartPreset tag = itemClicked.mTag as CASPartPreset;
                ths.mCurrentHornColorPreset = tag;

                // Custom
                Responder.Instance.CASModel.RequestRemoveCASPart(ths.GetWornHornPart());

                Responder.Instance.CASModel.RequestAddCASPart(tag.mPart, false);
                Responder.Instance.CASModel.RequestCommitPresetToPart(tag.mPart, tag.mPresetString);

                CustomContentIcon childByID = itemClicked.mWin.GetChildByID(0x23, true) as CustomContentIcon;
                if (UIUtils.ExtractCustomContentType(childByID.ContentType) == ResourceKeyContentCategory.kInstalled)
                {
                    ths.mHornColorDeleteButton.Enabled = false;
                }
                else
                {
                    ths.mHornColorDeleteButton.Enabled = true;
                }
                Audio.StartSound("ui_tertiary_button");
            }
            catch (Exception e)
            {
                Common.Exception("OnHornColorGridClicked", e);
            }
        }

        public static void PopulateHornColors()
        {
            try
            {
                CAPUnicorn ths = CAPUnicorn.gSingleton;
                if (ths == null) return;

                CASPartPreset preset;
                ths.mHornColorsGrid.Clear();
                List<CASPart> wornHornParts = new List<CASPart>();

                CASPart wornPart = ths.GetWornHornPart();

                CASLogic singleton = CASLogic.GetSingleton();

                foreach (CASPart part in singleton.mCasParts)
                {
                    if ((part.BodyType == BodyTypes.PetHorn) && 
                        (part.Species == CASAgeGenderFlags.Horse) && 
                        ((part.Age & singleton.Age) != CASAgeGenderFlags.None))
                    {
                        wornHornParts.Add(part);
                    }
                }

                CompositorUtil.GetPatternsFromCASPart(wornPart);
                ObjectDesigner.SetCASPart(wornPart.Key);
                ResourceKey layoutKey = ResourceKey.CreateUILayoutKey("EyeColorPresetGridItem", 0x0);
                string designPreset = null;

                // Custom
                try
                {
                    designPreset = Responder.Instance.CASModel.GetDesignPreset(wornPart);
                }
                catch
                { }

                foreach (CASPart wornHornPart in wornHornParts)
                {
                    uint num = CASUtils.PartDataNumPresets(wornHornPart.Key);

                    int i = 0;

                    for (uint j = 0x0; j < num; j++)
                    {
                        preset = new CASPartPreset(wornHornPart, CASUtils.PartDataGetPresetId(wornHornPart.Key, j), CASUtils.PartDataGetPreset(wornHornPart.Key, j));

                        ths.AddHornPresetGridItem(ths.mHornColorsGrid, layoutKey, preset, wornHornPart, (uint)(j + 0x1));
                        if (CAPUnicorn.HornPresetCompare(designPreset, preset.mPresetString))
                        {
                            ths.mCurrentHornColorPreset = preset;
                            ths.mHornColorsGrid.SelectedItem = i;
                            if (UIUtils.GetCustomContentType(wornHornPart.Key, preset.mPresetId) == ResourceKeyContentCategory.kInstalled)
                            {
                                ths.mHornColorDeleteButton.Enabled = false;
                            }
                            else
                            {
                                ths.mHornColorDeleteButton.Enabled = true;
                            }
                        }

                        i++;
                    }
                }

                if (ths.mHornColorsGrid.SelectedItem == -1)
                {
                    ths.mHornColorDeleteButton.Enabled = false;
                }

                bool flag = ths.mHornColorsGrid.Count > (ths.mHornColorsGrid.VisibleRows * ths.mHornColorsGrid.VisibleColumns);
                Rect area = ths.mHornColorPanel.Area;
                area.Height = flag ? ths.mHornColorPanelHeightWithScrollbars : (ths.mHornColorPanelHeightWithScrollbars - 21f);
                ths.mHornColorPanel.Area = area;
            }
            catch (Exception e)
            {
                Common.Exception("PopulateHornColors", e);
            }
        }
    }
}
