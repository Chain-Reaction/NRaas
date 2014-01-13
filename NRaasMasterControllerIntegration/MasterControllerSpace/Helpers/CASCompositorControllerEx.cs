using NRaas.CommonSpace.Tasks;
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
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Helpers
{
    public class CASCompositorControllerEx
    {
        public static void OnMaterialsColorGridMouseDown(WindowBase sender, UIMouseEventArgs eventArgs)
        {
            try
            {
                CASCompositorController ths = CASCompositorController.sController;

                if ((ths.mCurrentDesignObject != null) && (ths.mMaterialSkewerSelectedPattern != -1))
                {
                    if (eventArgs.MouseKey == MouseKeys.kMouseRight)
                    {
                        bool random = ((eventArgs.Modifiers & (Modifiers.kModifierMaskShift | Modifiers.kModifierMaskControl)) != Modifiers.kModifierMaskNone);

                        int index = ((int)sender.ID) - 0x1202a;

                        ProcessAlterColor(index, random);

                        if (sender.Enabled)
                        {
                            eventArgs.Handled = true;
                        }
                        return;
                    }
                }

                ths.OnMaterialsColorGridMouseDown(sender, eventArgs);
            }
             catch (Exception e)
             {
                 Common.Exception("OnMaterialsColorDragMouseDown", e);
             }
        }

        public static void OnMaterialsColorGridMouseUp(WindowBase sender, UIMouseEventArgs eventArgs)
        {
            try
            {
                CASCompositorController ths = CASCompositorController.sController;

                // Custom
                if ((eventArgs.MouseKey == MouseKeys.kMouseLeft) || (eventArgs.MouseKey == MouseKeys.kMouseRight))
                {
                    UIManager.ReleaseCapture(InputContext.kICMouse, sender);
                    ths.ClearClickEvent();
                }
                else if (sender.Enabled)
                {
                    eventArgs.Handled = true;
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnMaterialsColorGridMouseUp", e);
            }
        }

        public static void OnMaterialsColorDragMouseDown(WindowBase sender, UIMouseEventArgs eventArgs)
        {
            try
            {
                CASCompositorController ths = CASCompositorController.sController;

                if ((ths.mCurrentDesignObject != null) && (ths.mMaterialSkewerSelectedPattern != -1))
                {
                    if (eventArgs.MouseKey == MouseKeys.kMouseRight)
                    {
                        bool random = ((eventArgs.Modifiers & (Modifiers.kModifierMaskShift | Modifiers.kModifierMaskControl)) != Modifiers.kModifierMaskNone);

                        ProcessAlterColor(random);

                        if (sender.Enabled)
                        {
                            eventArgs.Handled = true;
                        }
                        return;
                    }
                }

                ths.OnMaterialsColorDragMouseDown(sender, eventArgs);
            }
            catch (Exception e)
            {
                Common.Exception("OnMaterialsColorDragMouseDown", e);
            }
        }

        public static void OnMaterialsColorDragMouseUp(WindowBase sender, UIMouseEventArgs eventArgs)
        {
            try
            {
                CASCompositorController ths = CASCompositorController.sController;

                // Custom
                if ((eventArgs.MouseKey == MouseKeys.kMouseLeft) || (eventArgs.MouseKey == MouseKeys.kMouseRight))
                {
                    UIManager.ReleaseCapture(InputContext.kICMouse, sender);
                    ths.ClearClickEvent();
                }
                else if (sender.Enabled)
                {
                    eventArgs.Handled = true;
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnMaterialsColorDragMouseUp", e);
            }
        }

        public static void OnMaterialsSkewerGridMouseDown(WindowBase sender, UIMouseEventArgs eventArgs)
        {
            Common.StringBuilder msg = new Common.StringBuilder("OnMaterialsSkewerGridMouseDown");

            try
            {
                CASCompositorController ths = CASCompositorController.sController;
                if (ths == null) return;

                if ((ths.mCurrentDesignObject == null) || (ths.mMaterialSkewerSelectedPattern == -1))
                {
                    msg += "A";

                    if (sender.Enabled)
                    {
                        eventArgs.Handled = true;
                    }
                }
                else if (ths.mCurrentDesignObject.Processing)
                {
                    msg += "B";

                    ths.mMaterialToSelect = ((int)sender.ID) - 0x1300a;
                    Audio.StartSound("ui_tertiary_button");
                    if (sender.Enabled)
                    {
                        eventArgs.Handled = true;
                    }
                }
                else if ((sender as Button).Enabled)
                {
                    msg += "C";

                    ths.mClickedWin = sender;
                    ths.mMouseClickPos = sender.WindowToScreen(eventArgs.MousePosition);

                    // Must be after mClickedWin is set above
                    int selection = ((int)ths.mClickedWin.ID) - 0x1300a;

                    bool selected = (selection == ths.MaterialSkewerSelectedPattern);

                    ths.MaterialSelect(selection, true);

                    if (eventArgs.MouseKey == MouseKeys.kMouseLeft)
                    {
                        msg += "D";

                        ths.mPickupScript = Simulator.AddObject(new Sims3.Gameplay.OneShotFunctionTask(ths.StartMaterialDrag, StopWatch.TickStyles.Seconds, CASCompositorController.kPickupDelay));
                        UIManager.SetCaptureTarget(InputContext.kICMouse, sender);

                        if ((eventArgs.Modifiers & (Modifiers.kModifierMaskShift | Modifiers.kModifierMaskControl)) != Modifiers.kModifierMaskNone)
                        {
                            Common.FunctionTask.Perform(OnProcessRandomizeColor);
                        }
                    }
                    else if (eventArgs.MouseKey == MouseKeys.kMouseRight)
                    {
                        msg += "E";

                        if ((selected) && ((eventArgs.Modifiers & (Modifiers.kModifierMaskShift | Modifiers.kModifierMaskControl)) != Modifiers.kModifierMaskNone))
                        {
                            Common.FunctionTask.Perform(OnProcessRandomizeCategory);
                        }
                        else
                        {
                            Common.FunctionTask.Perform(OnProcessRandomizeMaterial);
                        }
                    }

                    msg += "E";

                    eventArgs.Handled = true;
                }
            }
            catch (Exception e)
            {
                Common.Exception(msg, e);
            }
        }

        public static void OnMaterialsSkewerGridMouseUp(WindowBase sender, UIMouseEventArgs eventArgs)
        {
            try
            {
                CASCompositorController ths = CASCompositorController.sController;

                // Custom
                if ((eventArgs.MouseKey == MouseKeys.kMouseLeft) || (eventArgs.MouseKey == MouseKeys.kMouseRight))
                {
                    UIManager.ReleaseCapture(InputContext.kICMouse, sender);
                    ths.ClearClickEvent();
                }
                else if (sender.Enabled)
                {
                    eventArgs.Handled = true;
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnMaterialsSkewerGridMouseUp", e);
            }
        }

        private static void OnProcessRandomizeCategory()
        {
            CASCompositorController ths = CASCompositorController.sController;
            if (ths == null) return;

            List<string> materials = new List<string>();

            for (int i = 0; i < ths.mMaterialComboBox.ValueList.Count; i++)
            {
                materials.Add(ths.mMaterialComboBox.EntryTags[i] as string);
            }

            if (materials.Count > 0)
            {
                ths.SetFilter(RandomUtil.GetRandomObjectFromList(materials));

                ths.mMaterialsWindowGrid.Clear();
                ths.PopulateMaterialsBinGrid();

                while (!ths.Enabled)
                {
                    SpeedTrap.Sleep();
                }
            }

            OnProcessRandomizeMaterial();
        }

        private static void OnProcessRandomizeMaterial()
        {
            CASCompositorController ths = CASCompositorController.sController;
            if (ths == null) return;

            int selection = ths.MaterialSkewerSelectedPattern;
            if (selection >= 0x0)
            {
                List<Complate> choices = new List<Complate>();

                foreach (ItemGridCellItem item in ths.mMaterialsWindowGrid.Items)
                {
                    Complate choice = item.mTag as Complate;
                    if (choice == null) continue;

                    choices.Add(choice);
                }

                if (choices.Count > 0)
                {
                    ths.ApplyMaterial(RandomUtil.GetRandomObjectFromList(choices).Clone() as Complate, selection, false, false);

                    ProcessAlterColor(true);
                }
            }
        }

        private static void OnProcessRandomizeColor()
        {
            ProcessAlterColor(false);
        }

        private static void ProcessAlterColor(bool random)
        {
            CASCompositorController ths = CASCompositorController.sController;
            if (ths == null) return;

            int selection = ths.MaterialSkewerSelectedPattern;
            if (selection >= 0x0)
            {
                Color[] colors = new Color[4];

                for (int i = 0; i < 4; i++)
                {
                    colors[i] = AlterColor(ths.mMultiColorsThumb[i].ShadeColor, random);
                }

                ths.ApplyMaterial(colors, selection, true, true);

                ths.RepopulateColors(true);
            }
        }

        private static void ProcessAlterColor(int index, bool random)
        {
            CASCompositorController ths = CASCompositorController.sController;
            if (ths == null) return;

            int selection = ths.MaterialSkewerSelectedPattern;
            if (selection >= 0x0)
            {
                Color[] colors = new Color[4];

                for (int i = 0; i < 4; i++)
                {
                    colors[i] = ths.mMultiColorsThumb[i].ShadeColor;
                }

                colors[index] = AlterColor(colors[index], random);

                ths.ApplyMaterial(colors, selection, true, true);

                ths.RepopulateColors(true);
            }
        }

        private static Color AlterColor(Color origColor, bool random)
        {
            if (random)
            {
                return new Color(RandomUtil.GetInt(255), RandomUtil.GetInt(255), RandomUtil.GetInt(255));
            }
            else
            {
                // mSavedColors is a Vector color stored as RGB ratios of 0 to 1

                Vector3 hsv = CompositorUtil.ColorShifter.ColorToHsv(CompositorUtil.ColorToVector3(origColor));

                /*
                hsv.x += RandomUtil.GetFloat(-0.01f, 0.01f);
                if (hsv.x > 1)
                {
                    hsv.x = 1;
                }
                else if (hsv.x < 0)
                {
                    hsv.x = 0;
                }
                */

                hsv.y += RandomUtil.GetFloat(-0.25f, 0.25f);
                if (hsv.y > 1)
                {
                    hsv.y = 1;
                }
                else if (hsv.y < 0.01)
                {
                    hsv.y = 0.01f;
                }

                hsv.z += RandomUtil.GetFloat(-0.25f, 0.25f);
                if (hsv.z > 1)
                {
                    hsv.z = 1;
                }
                else if (hsv.z < 0.01)
                {
                    hsv.z = 0.01f;
                }

                return CompositorUtil.Vector3ToColor(CompositorUtil.ColorShifter.HsvToColor(hsv));
            }
        }
    }
}
