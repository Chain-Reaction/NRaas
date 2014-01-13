using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.CAS
{
    public class CASTattooExpanded : Common.IPreLoad
    {
        static WindowBase sBodyPanel;
        static WindowBase sFacePanel;
        static WindowBase sLegPanel;

        static bool sInitialized = false;

        delegate void CameraAdjustment(CASTattoo ths);

        static Dictionary<TattooID, CameraAdjustment> sCameraAdjustments = new Dictionary<TattooID, CameraAdjustment>();

        static CASTattooExpanded()
        {
            sCameraAdjustments.Add(TattooID.TattooFullBody, OnFrontTopCam);
            sCameraAdjustments.Add(TattooID.TattooBreastUpperL, OnFrontTopCam);
            sCameraAdjustments.Add(TattooID.TattooBreastUpperR, OnFrontTopCam);
            sCameraAdjustments.Add(TattooID.TattooHipLeft, OnFrontTopCam);
            sCameraAdjustments.Add(TattooID.TattooHipRight, OnFrontTopCam);
            sCameraAdjustments.Add(TattooID.TattooLowerBelly, OnFrontTopCam);
            sCameraAdjustments.Add(TattooID.TattooThighFrontL, OnFrontTopCam);
            sCameraAdjustments.Add(TattooID.TattooThighFrontR, OnFrontTopCam);
            sCameraAdjustments.Add(TattooID.TattooCalfFrontL, OnFrontTopCam);
            sCameraAdjustments.Add(TattooID.TattooCalfFrontR, OnFrontTopCam);

            sCameraAdjustments.Add(TattooID.TattooNippleR, OnFrontTopCam);
            sCameraAdjustments.Add(TattooID.TattooNippleL, OnFrontTopCam);
            sCameraAdjustments.Add(TattooID.TattooPubic, OnFrontTopCam);

            sCameraAdjustments.Add(TattooID.TattooFullFace, OnFaceCam);
            sCameraAdjustments.Add(TattooID.TattooThroat, OnFaceCam);
            sCameraAdjustments.Add(TattooID.TattooCheekLeft, OnFaceCam);
            sCameraAdjustments.Add(TattooID.TattooCheekRight, OnFaceCam);
            sCameraAdjustments.Add(TattooID.TattooForehead, OnFaceCam);

            sCameraAdjustments.Add(TattooID.TattooHandLeft, OnLeftHandCam);
            sCameraAdjustments.Add(TattooID.TattooHandRight, OnRightHandCam);

            sCameraAdjustments.Add(TattooID.TattooPalmLeft, OnLeftPalmCam);
            sCameraAdjustments.Add(TattooID.TattooPalmRight, OnRightPalmCam);

            sCameraAdjustments.Add(TattooID.TattooRibsLeft, OnLeftRibsCam);
            sCameraAdjustments.Add(TattooID.TattooRibsRight, OnRightRibsCam);

            sCameraAdjustments.Add(TattooID.TattooShoulderBackL, OnBackTopCam);
            sCameraAdjustments.Add(TattooID.TattooShoulderBackR, OnBackTopCam);
            sCameraAdjustments.Add(TattooID.TattooLowerLowerBack, OnBackTopCam);
            sCameraAdjustments.Add(TattooID.TattooButtLeft, OnBackTopCam);
            sCameraAdjustments.Add(TattooID.TattooButtRight, OnBackTopCam);
            sCameraAdjustments.Add(TattooID.TattooThighBackL, OnBackTopCam);
            sCameraAdjustments.Add(TattooID.TattooThighBackR, OnBackTopCam);
            sCameraAdjustments.Add(TattooID.TattooCalfBackL, OnBackTopCam);
            sCameraAdjustments.Add(TattooID.TattooCalfBackR, OnBackTopCam);

            sCameraAdjustments.Add(TattooID.TattooAnkleOuterL, OnLeftAnkleCam);
            sCameraAdjustments.Add(TattooID.TattooAnkleOuterR, OnRightAnkleCam);

            sCameraAdjustments.Add(TattooID.TattooFootLeft, OnFootCam);
            sCameraAdjustments.Add(TattooID.TattooFootRight, OnFootCam);
        }

        public void OnPreLoad()
        {
            Sims.CASBase.OnExternalShowUI += OnShowUI;
        }

        private static void PopulateTunedScales(CASTattoo ths)
        {
            try
            {
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooFullBody, TattooTuning.kDefaultFullBodyScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooFullFace, TattooTuning.kDefaultFullFaceScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooShoulderBackL, TattooTuning.kDefaultShoulderBackLScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooShoulderBackR, TattooTuning.kDefaultShoulderBackRScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooLowerLowerBack, TattooTuning.kDefaultLowerLowBackScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooButtLeft, TattooTuning.kDefaultButtLeftScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooButtRight, TattooTuning.kDefaultButtRightScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooHandLeft, TattooTuning.kDefaultHandLeftScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooHandRight, TattooTuning.kDefaultHandRightScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooPalmLeft, TattooTuning.kDefaultPalmLeftScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooPalmRight, TattooTuning.kDefaultPalmRightScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooThroat, TattooTuning.kDefaultThroatScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooRibsLeft, TattooTuning.kDefaultRibsLeftScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooRibsRight, TattooTuning.kDefaultRibsRightScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooBreastUpperL, TattooTuning.kDefaultBreastUpperLScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooBreastUpperR, TattooTuning.kDefaultBreastUpperRScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooHipLeft, TattooTuning.kDefaultHipLeftScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooHipRight, TattooTuning.kDefaultHipRightScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooThighFrontL, TattooTuning.kDefaultThighFrontLScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooThighFrontR, TattooTuning.kDefaultThighFrontRScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooThighBackL, TattooTuning.kDefaultThighBackLScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooThighBackR, TattooTuning.kDefaultThighBackRScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooCalfFrontL, TattooTuning.kDefaultCalfFrontLScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooCalfFrontR, TattooTuning.kDefaultCalfFrontRScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooCalfBackL, TattooTuning.kDefaultCalfBackLScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooCalfBackR, TattooTuning.kDefaultCalfBackRScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooAnkleOuterL, TattooTuning.kDefaultAnkleOuterLScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooAnkleOuterR, TattooTuning.kDefaultAnkleOuterRScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooFootLeft, TattooTuning.kDefaultFootLeftScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooFootRight, TattooTuning.kDefaultFootRightScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooCheekLeft, TattooTuning.kDefaultCheekLeftScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooCheekRight, TattooTuning.kDefaultCheekRightScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooForehead, TattooTuning.kDefaultForeheadScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooLowerBelly, TattooTuning.kDefaultLowerBellyScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooPubic, TattooTuning.kDefaultPubicScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooNippleL, TattooTuning.kDefaultNippleLeftScales);
                ths.mTunedScales.Add((CASTattoo.TattooID)TattooID.TattooNippleR, TattooTuning.kDefaultNippleRightScales);
            }
            catch (Exception e)
            {
                Common.Exception("PopulateTunedScales - Possible Interference from a Core-Mod", e);
            }
        }

        private static void SetActiveTattooType(CASTattoo ths, CASTattoo.TattooID tattooID, bool forceRefresh)
        {
            if ((ths.mActiveTattooID != tattooID) || forceRefresh)
            {
                ths.mActiveTattooID = tattooID;
                bool flag = false;
                foreach (CASPart part in ths.mTattooParts)
                {
                    if (part.Key.InstanceId == (ulong)ths.mActiveTattooID)
                    {
                        ths.mActiveTattooPart = part;
                        flag = true;
                        break;
                    }
                }

                if (flag)
                {
                    SetTattooCam(ths, ths.mActiveTattooID);
                    ths.UpdateCurrentPreset();
                    ths.ClearTemplates();
                    ths.PopulateTattooGrid(true);
                }
            }
        }

        public static void Init(CASTattoo ths)
        {
            sLegPanel = ths.GetChildByID((uint)ControlIDs.LegPanel, true);
            sFacePanel = ths.GetChildByID((uint)ControlIDs.FacePanel, true);
            sBodyPanel = ths.GetChildByID((uint)ControlIDs.BodyPanel, true);

            ths.mAcceptButton.Click -= ths.OnAcceptClick;
            ths.mAcceptButton.Click += OnAcceptClick;

            ths.mAdvancedModeButton.Click -= ths.OnAdvancedClick;
            ths.mAdvancedModeButton.Click += OnAdvancedClick;

            Button childByID = ths.GetChildByID(0x92fa02e, true) as Button;
            childByID.Click -= ths.OnCancelClick;
            childByID.Click += OnCancelClick;

            foreach (CASTattoo.ControlIDs id in Enum.GetValues(typeof(CASTattoo.ControlIDs)))
            {
                Button button = ths.GetChildByID((uint)id, true) as Button;
                if (button != null)
                {
                    button.Click -= ths.OnNavButtonClicked;
                    button.Click += OnNavButtonClicked;

                }
            }
            
            foreach (ControlIDs id in Enum.GetValues(typeof(ControlIDs)))
            {
                Button button = ths.GetChildByID((uint)id, true) as Button;
                if (button != null)
                {
                    ths.mNavButtons.Remove((CASTattoo.ControlIDs)id);
                    ths.mNavButtons.Add((CASTattoo.ControlIDs)id, button);

                    button.Click -= OnNavButtonClicked;
                    button.Click += OnNavButtonClicked;
                }
            }

            CASFacialDetails.gSingleton.SetTattooPanel();

            PopulateTunedScales(ths);

            OnNavButtonClickedHelper();

            SetTattooCam(ths, ths.mActiveTattooID);
        }

        protected static void OnShowUI(bool initialized)
        {
            try
            {
                if (!initialized)
                {
                    sInitialized = false;
                }

                CASTattoo ths = CASTattoo.gSingleton;
                if (ths == null) return;

                if (CASController.Singleton.CurrentState.mPhysicalState != CASPhysicalState.Tattoos) return;

                if (!sInitialized)
                {
                    sInitialized = true;

                    Init(ths);
                }

                ths.UINodeShutdown -= UINodeShutdownCallback;
                ths.UINodeShutdown += UINodeShutdownCallback;

                Responder.Instance.CASModel.UndoSelected -= ths.OnUndo;
                Responder.Instance.CASModel.RedoSelected -= ths.OnRedo;

                Responder.Instance.CASModel.UndoSelected -= OnUndo;
                Responder.Instance.CASModel.RedoSelected -= OnRedo;

                if ((CASController.gSingleton.CurrentState.mPhysicalState == CASPhysicalState.Tattoos) && (!ths.AdvancedMode))
                {
                    Responder.Instance.CASModel.UndoSelected += OnUndo;
                    Responder.Instance.CASModel.RedoSelected += OnRedo;
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnShowUI", e);
            }
        }

        public static void UINodeShutdownCallback(CASState newState)
        {
            CASTattoo ths = CASTattoo.gSingleton;
            if (ths == null) return;

            if (newState.mPhysicalState != CASPhysicalState.Tattoos)
            {
                ths.UINodeShutdown -= UINodeShutdownCallback;

                Responder.Instance.CASModel.UndoSelected -= OnUndo;
                Responder.Instance.CASModel.RedoSelected -= OnRedo;

                sInitialized = false;
            }
        }

        private static void OnAcceptClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                CASTattoo ths = CASTattoo.gSingleton;

                if (ths.HasAnyPartsInUse())
                {
                    Responder.Instance.CASModel.RequestFinalizeCommitPresetToPart();
                    AdvancedMode = false;
                    ths.PopulateTattooGrid(false, true);
                    ths.mTattooGrid.ShowSelectedItem(false);
                }
                else
                {
                    ths.ClearAllParts();
                    ths.ClearTemplates();
                    ths.CurrentPreset.mPresetId = 0x0;
                    ths.CurrentPreset.mPresetString = CASUtils.PartDataGetPreset(ths.mActiveTattooPart.Key, 0x0);
                    AdvancedMode = false;
                    ths.mRemoveTattooOnUndo = true;
                    Responder.Instance.CASModel.RequestUndo();
                }
                eventArgs.Handled = true;
            }
            catch (Exception e)
            {
                Common.Exception("OnAcceptClick", e);
            }
        }

        private static void OnAdvancedClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                AdvancedMode = true;
                eventArgs.Handled = true;
            }
            catch (Exception e)
            {
                Common.Exception("OnAdvancedClick", e);
            }
        }

        private static void OnCancelClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                CASTattoo ths = CASTattoo.gSingleton;

                bool flag = ths.mUndoRedoQueueIndex > 0x1;
                AdvancedMode = false;
                if (flag)
                {
                    Responder.Instance.CASModel.RequestUndo();
                }
                else
                {
                    ths.PopulateTattooGrid(false, ths.mWasCustom);
                }
                eventArgs.Handled = true;
            }
            catch (Exception e)
            {
                Common.Exception("OnCancelClick", e);
            }
        }

        private static bool AdvancedMode
        {
            set
            {
                try
                {
                    CASTattoo ths = CASTattoo.gSingleton;

                    if (value != ths.mAdvancedMode)
                    {
                        ths.AdvancedMode = value;

                        if (value)
                        {
                            Responder.Instance.CASModel.UndoSelected -= OnUndo;
                            Responder.Instance.CASModel.RedoSelected -= OnRedo;
                        }
                        else
                        {
                            OnNavButtonClickedHelper();
                            ths.UpdateRemoveAllButton();

                            Responder.Instance.CASModel.UndoSelected -= ths.OnUndo;
                            Responder.Instance.CASModel.RedoSelected -= ths.OnRedo;

                            Responder.Instance.CASModel.UndoSelected += OnUndo;
                            Responder.Instance.CASModel.RedoSelected += OnRedo;
                            if (ths.mTriggerHandle != 0x0)
                            {
                                ths.RemoveTriggerHook(ths.mTriggerHandle);
                                ths.mTriggerHandle = 0x0;
                            }
                            ths.TriggerDown -= ths.OnTriggerDown;
                            CASController.Singleton.AllowSimHighlight = true;
                            ths.EffectFinished -= ths.OnGlideFinished;
                            ths.EffectFinished += ths.OnGlideFinished;
                        }
                        CASFacialDetails.gSingleton.Glide(!value);
                        ths.Glide(!value);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception("AdvancedMode", e);
                }
            }
        }

        private static void OnRedo()
        {
            try
            {
                CASTattoo ths = CASTattoo.gSingleton;

                SetActiveTattooType(ths, ths.mActiveTattooID, true);
                ths.UpdateRemoveAllButton();
            }
            catch (Exception e)
            {
                Common.Exception("OnRedo", e);
            }
        }

        private static void OnUndo()
        {
            try
            {
                CASTattoo ths = CASTattoo.gSingleton;

                if (ths.mRemoveTattooOnUndo)
                {
                    ths.OnUndo();
                }
                else
                {
                    ths.UpdateRemoveAllButton();
                    SetActiveTattooType(ths, ths.mActiveTattooID, true);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnUndo", e);
            }
        }

        private static void OnNavButtonClicked(WindowBase sender, UIButtonClickEventArgs args)
        {
            try
            {
                CASTattoo ths = CASTattoo.gSingleton;
                if (!ths.AdvancedMode)
                {
                    Simulator.AddObject(new OneShotFunctionTask(OnNavButtonClickedHelper));
                }
                args.Handled = true;
            }
            catch (Exception e)
            {
                Common.Exception("OnNavButtonClicked", e);
            }
        }

        private static void OnNavButtonClickedHelper()
        {
            try
            {
                CASTattoo ths = CASTattoo.gSingleton;
                if (ths == null) return;

                ths.LeftRightEnabled = true;

                if (ths.mArmPanel == null) return;
                ths.mArmPanel.Visible = false;

                if (ths.mChestPanel == null) return;
                ths.mChestPanel.Visible = false;

                if (ths.mBackPanel == null) return;
                ths.mBackPanel.Visible = false;

                if (sLegPanel == null) return;
                sLegPanel.Visible = false;

                if (sFacePanel == null) return;
                sFacePanel.Visible = false;

                if (sBodyPanel == null) return;
                sBodyPanel.Visible = false;

                ths.ActiveLayer = 0;
                if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.LegGroupButton].Selected)
                {
                    sLegPanel.Visible = true;
                    if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.AnkleButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, CASTattoo.TattooID.TattooAnkleLeft, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, CASTattoo.TattooID.TattooAnkleRight, false);
                        }
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.ThighFrontButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooThighFrontL, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooThighFrontR, false);
                        }
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.ThighBackButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooThighBackL, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooThighBackR, false);
                        }
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.CalfFrontButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooCalfFrontL, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooCalfFrontR, false);
                        }
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.CalfBackButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooCalfBackL, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooCalfBackR, false);
                        }
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.AnkleOuterButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooAnkleOuterL, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooAnkleOuterR, false);
                        }
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.FootButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooFootLeft, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooFootRight, false);
                        }
                    }
                }
                else if (ths.mNavButtons[CASTattoo.ControlIDs.ArmGroupButton].Selected)
                {
                    ths.mArmPanel.Visible = true;
                    if (ths.mNavButtons[CASTattoo.ControlIDs.WristButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, CASTattoo.TattooID.TattooWristTopLeft, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, CASTattoo.TattooID.TattooWristTopRight, false);
                        }
                    }
                    else if (ths.mNavButtons[CASTattoo.ControlIDs.ForearmButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, CASTattoo.TattooID.TattooForearmLeft, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, CASTattoo.TattooID.TattooForearmRight, false);
                        }
                    }
                    else if (ths.mNavButtons[CASTattoo.ControlIDs.BicepButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, CASTattoo.TattooID.TattooBicepLeft, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, CASTattoo.TattooID.TattooBicepRight, false);
                        }
                    }
                    else if (ths.mNavButtons[CASTattoo.ControlIDs.ShoulderButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, CASTattoo.TattooID.TattooShoulderLeft, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, CASTattoo.TattooID.TattooShoulderRight, false);
                        }
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.HandButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooHandLeft, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooHandRight, false);
                        }
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.PalmButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooPalmLeft, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooPalmRight, false);
                        }
                    }
                }
                else if (ths.mNavButtons[CASTattoo.ControlIDs.ChestGroupButton].Selected)
                {
                    ths.mChestPanel.Visible = true;
                    if (ths.mNavButtons[CASTattoo.ControlIDs.ChestButton].Selected)
                    {
                        SetActiveTattooType(ths, CASTattoo.TattooID.TattooChest, false);
                    }
                    else if (ths.mNavButtons[CASTattoo.ControlIDs.BellyButton].Selected)
                    {
                        SetActiveTattooType(ths, CASTattoo.TattooID.TattooBellybutton, false);
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.ThroatButton].Selected)
                    {
                        SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooThroat, false);
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.RibsButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooRibsLeft, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooRibsRight, false);
                        }
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.UpperBreastButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooBreastUpperL, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooBreastUpperR, false);
                        }
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.HipButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooHipLeft, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooHipRight, false);
                        }
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.LowerBellyButton].Selected)
                    {
                        SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooLowerBelly, false);
                    }
                }
                else if (ths.mNavButtons[CASTattoo.ControlIDs.BackGroupButton].Selected)
                {
                    ths.mBackPanel.Visible = true;
                    if (ths.mNavButtons[CASTattoo.ControlIDs.UpperBackButton].Selected)
                    {
                        SetActiveTattooType(ths, CASTattoo.TattooID.TattooUpperBack, false);
                    }
                    else if (ths.mNavButtons[CASTattoo.ControlIDs.LowerBackButton].Selected)
                    {
                        SetActiveTattooType(ths, CASTattoo.TattooID.TattooLowerBack, false);
                    }
                    else if (ths.mNavButtons[CASTattoo.ControlIDs.FullBackButton].Selected)
                    {
                        SetActiveTattooType(ths, CASTattoo.TattooID.TattooFullBack, false);
                    }
                    else if (ths.mNavButtons[CASTattoo.ControlIDs.NeckButton].Selected)
                    {
                        SetActiveTattooType(ths, CASTattoo.TattooID.TattooNeck, false);
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.ShoulderBackButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooShoulderBackL, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooShoulderBackR, false);
                        }
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.LowerLowerBack].Selected)
                    {
                        SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooLowerLowerBack, false);
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.ButtCheekButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooButtLeft, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooButtRight, false);
                        }
                    }
                }
                else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.FaceGroupButton].Selected)
                {
                    sFacePanel.Visible = true;
                    if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.FullFaceButton].Selected)
                    {
                        SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooFullFace, false);
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.CheekButton].Selected)
                    {
                        if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooCheekLeft, false);
                        }
                        else
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooCheekRight, false);
                        }
                    }
                    else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.ForeheadButton].Selected)
                    {
                        SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooForehead, false);
                    }
                }
                else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.BodyGroupButton].Selected)
                {
                    if (!NRaas.MasterController.Settings.mNakedTattoo)
                    {
                        ths.LeftRightEnabled = false;
                        sBodyPanel.Visible = false;

                        SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooFullBody, false);
                    }
                    else
                    {
                        sBodyPanel.Visible = true;

                        if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.FullBodyButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooFullBody, false);
                        }
                        else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.NippleButton].Selected)
                        {
                            if (ths.mNavButtons[CASTattoo.ControlIDs.LeftButton].Selected)
                            {
                                SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooNippleL, false);
                            }
                            else
                            {
                                SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooNippleR, false);
                            }
                        }
                        else if (ths.mNavButtons[(CASTattoo.ControlIDs)ControlIDs.PubicButton].Selected)
                        {
                            SetActiveTattooType(ths, (CASTattoo.TattooID)TattooID.TattooPubic, false);
                        }
                    }
                }

                ths.SaveState();
            }
            catch (Exception e)
            {
                Common.Exception("OnNavButtonClickedHelper", e);
            }
        }

        public static void OnLeftHandCam(CASTattoo ths)
        {
            try
            {
                CASController.Singleton.SetHandCam(true);
                CASPuck.TryRotateSimTowards(-1.256637f);
            }
            catch (Exception e)
            {
                Common.Exception("OnLeftHandCam", e);
            }
        }

        public static void OnRightHandCam(CASTattoo ths)
        {
            try
            {
                CASController.Singleton.SetHandCam(true);
                CASPuck.TryRotateSimTowards(1.256637f);
            }
            catch (Exception e)
            {
                Common.Exception("OnRightHandCam", e);
            }
        }

        public static void OnLeftPalmCam(CASTattoo ths)
        {
            try
            {
                CASController.Singleton.SetHandCam(true);
                CASPuck.TryRotateSimTowards(2.55f);
            }
            catch (Exception e)
            {
                Common.Exception("OnLeftPalmCam", e);
            }
        }

        public static void OnRightPalmCam(CASTattoo ths)
        {
            try
            {
                CASController.Singleton.SetHandCam(true);
                CASPuck.TryRotateSimTowards(-2.55f);
            }
            catch (Exception e)
            {
                Common.Exception("OnRightPalmCam", e);
            }
        }

        public static void OnLeftRibsCam(CASTattoo ths)
        {
            try
            {
                CASController.Singleton.SetTopCam(true);
                CASPuck.TryRotateSimTowards(-0.8f);
            }
            catch (Exception e)
            {
                Common.Exception("OnLeftRibsCam", e);
            }
        }

        public static void OnRightRibsCam(CASTattoo ths)
        {
            try
            {
                CASController.Singleton.SetTopCam(true);
                CASPuck.TryRotateSimTowards(0.8f);
            }
            catch (Exception e)
            {
                Common.Exception("OnRightRibsCam", e);
            }
        }

        public static void OnLeftAnkleCam(CASTattoo ths)
        {
            try
            {
                CASController.Singleton.SetAnkleCam(true);
                CASPuck.TryRotateSimTowards(-2.55f);
            }
            catch (Exception e)
            {
                Common.Exception("OnLeftAnkleCam", e);
            }
        }

        public static void OnRightAnkleCam(CASTattoo ths)
        {
            try
            {
                CASController.Singleton.SetAnkleCam(true);
                CASPuck.TryRotateSimTowards(2.55f);
            }
            catch (Exception e)
            {
                Common.Exception("OnRightAnkleCam", e);
            }
        }

        public static void OnFootCam(CASTattoo ths)
        {
            try
            {
                CASController.Singleton.SetAnkleCam(true);
                CASPuck.TryRotateSimTowards(0f);
            }
            catch (Exception e)
            {
                Common.Exception("OnFootCam", e);
            }
        }

        public static void OnFaceCam(CASTattoo ths)
        {
            try
            {
                CASController.Singleton.SetFaceCam(true);
                CASPuck.TryRotateSimTowards(0f);
            }
            catch (Exception e)
            {
                Common.Exception("OnFaceCam", e);
            }
        }

        public static void OnFrontTopCam(CASTattoo ths)
        {
            try
            {
                CASController.Singleton.SetTopCam(true);
                CASPuck.TryRotateSimTowards(0f);
            }
            catch (Exception e)
            {
                Common.Exception("OnFrontTopCam", e);
            }
        }

        public static void OnBackTopCam(CASTattoo ths)
        {
            try
            {
                CASController.Singleton.SetTopCam(true);
                CASPuck.TryRotateSimTowards(3.141593f);
            }
            catch (Exception e)
            {
                Common.Exception("OnBackTopCam", e);
            }
        }

        private static void SetTattooCam(CASTattoo ths, CASTattoo.TattooID tattooId)
        {
            CameraAdjustment adjustment;
            if (!sCameraAdjustments.TryGetValue((TattooID)tattooId, out adjustment))
            {
                ths.SetTattooCam(tattooId);
                return;
            }

            adjustment(ths);
        }

        public enum TattooID : ulong
        {
            TattooAnkleOuterL = 9853066347037867536L,
            TattooAnkleOuterR = 9853066347037867534L,
            TattooBreastUpperL = 14823567372817051672L,
            TattooBreastUpperR = 14823567372817051654L,
            TattooButtLeft = 14853680289739310853L,
            TattooButtRight = 14853680289739310875L,
            TattooCalfBackL = 13424233391748939576L,
            TattooCalfBackR = 13424233391748939558L,
            TattooCalfFrontL = 13424228993702426852L,
            TattooCalfFrontR = 13424228993702426874L,
            TattooCheekLeft = 0x6cccf2e9b615ff36L,
            TattooCheekRight = 0x6cccf2e9b615ff28L,
            TattooFootLeft = 10341513421333182696L,
            TattooFootRight = 10341513421333182710L,
            TattooForehead = 15210891427693956328L,
            TattooFullBody = 9607101428575224919L,
            TattooFullFace = 0x343813e5e87d9b1bL,
            TattooHandLeft = 17989267758382557469L,
            TattooHandRight = 17989267758382557443L,
            TattooHipLeft = 0x42e368e5f00c3a25L,
            TattooHipRight = 0x42e368e5f00c3a3bL,
            TattooLowerBelly = 0x5f002887ab7cec46L,
            TattooLowerLowerBack = 0x1750e08416599dL,
            TattooPalmLeft = 0x7b623afd6dd3433cL,
            TattooPalmRight = 0x7b623afd6dd34322L,
            TattooRibsLeft = 9262616950795245542L,
            TattooRibsRight = 9262616950795245560L,
            TattooShoulderBackL = 17723613198624675282L,
            TattooShoulderBackR = 17723613198624675276L,
            TattooThighBackL = 0x49e230e4b7463efaL,
            TattooThighBackR = 0x49e230e4b7463ee4L,
            TattooThighFrontL = 0x49e234e4b746450eL,
            TattooThighFrontR = 0x49e234e4b7464510L,
            TattooThroat = 15713298597947011534L,
            TattooPubic = 0x134036fd32c9c2a9L,
            TattooNippleL = 0x5412f73123f4519aL,
            TattooNippleR = 0x5412f73123f45184L,
        }

        public enum ControlIDs : uint
        {
            AnkleButton = 0x92fa04b,
            AnkleOuterButton = 0x92fa050,
            BodyGroupButton = 0x92fa040,
            BodyPanel = 0x92fa065,
            ButtCheekButton = 0x92fa044,
            CalfBackButton = 0x92fa04f,
            CalfFrontButton = 0x92fa04e,
            CheekButton = 0x92fa053,
            FaceGroupButton = 0x92fa041,
            FacePanel = 0x92fa064,
            FootButton = 0x92fa051,
            ForeheadButton = 0x92fa054,
            FullBodyButton = 0x92fa056,
            FullFaceButton = 0x92fa052,
            HandButton = 0x92fa045,
            HipButton = 0x92fa04a,
            LegGroupButton = 0x92fa030,
            LegPanel = 0x92fa063,
            LowerBellyButton = 0x92fa055,
            LowerLowerBack = 0x92fa043,
            NippleButton = 0x92fa058,
            PalmButton = 0x92fa046,
            PubicButton = 0x92fa057,
            RibsButton = 0x92fa048,
            ShoulderBackButton = 0x92fa042,
            ThighBackButton = 0x92fa04d,
            ThighFrontButton = 0x92fa04c,
            ThroatButton = 0x92fa047,
            UpperBreastButton = 0x92fa049,
        }
    }
}
