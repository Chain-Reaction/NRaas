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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.CAS
{
    public class CASEyebrowsEx
    {
        public static void PopulateEyebrowGrid()
        {
            try
            {
                CASEyebrows ths = CASEyebrows.gSingleton;
                if (ths == null) return;

                ths.mEyebrowPresetsGrid.Clear();
                CASPart wornPart = ths.GetWornPart(BodyTypes.Eyebrows);
                string designPreset = null;

                // Custom
                try
                {
                    designPreset = Responder.Instance.CASModel.GetDesignPreset(wornPart);
                }
                catch
                { }

                ResourceKey layoutKey = ResourceKey.CreateUILayoutKey("GenericCasItem", 0x0);
                int num = 0x0;
                foreach (CASPart part2 in Responder.Instance.CASModel.GetVisibleCASParts(BodyTypes.Eyebrows))
                {
                    ObjectDesigner.SetCASPart(part2.Key);
                    CASPartPreset preset = null;
                    uint num2 = CASUtils.PartDataNumPresets(part2.Key);
                    for (uint i = 0x0; i < num2; i++)
                    {
                        preset = new CASPartPreset(part2, CASUtils.PartDataGetPresetId(part2.Key, i), CASUtils.PartDataGetPreset(part2.Key, i));
                        ths.AddPresetGridItem(ths.mEyebrowPresetsGrid, layoutKey, preset, part2, i + 0x1);
                        if ((wornPart.Key == part2.Key) && (designPreset == preset.mPresetString))
                        {
                            ths.mCurrentEyebrowPreset = preset;
                            ths.mEyebrowPresetsGrid.SelectedItem = num;
                        }
                        num++;
                    }

                    if (num2 == 0x0)
                    {
                        preset = new CASPartPreset(part2, Responder.Instance.CASModel.GetDesignPreset(part2));
                        if (preset.Valid)
                        {
                            ths.AddPresetGridItem(ths.mEyebrowPresetsGrid, layoutKey, preset, part2, 0x0);
                            if (wornPart.Key == part2.Key)
                            {
                                ths.mCurrentEyebrowPreset = preset;
                                ths.mEyebrowPresetsGrid.SelectedItem = num;
                            }
                            num++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("PopulateEyebrowGrid", e);
            }
        }
    }
}
