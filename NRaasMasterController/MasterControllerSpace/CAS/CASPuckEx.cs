using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Settings.CAS;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.CAS.CAB;
using System;
using System.Collections;

namespace NRaas.MasterControllerSpace.CAS
{
    public class CASPuckEx
    {
        private static bool IsHouseholdValid()
        {
            foreach (ISimDescription description in Sims3.UI.Responder.Instance.CASModel.GetSimsInHousehold())
            {
                // Changed
                if (description.TeenOrAbove)
                {
                    return true;
                }
            }
            return false;
        }

        public static void OnOptionsMouseUp(WindowBase sender, UIMouseEventArgs args)
        {
            try
            {
                CASPuck ths = CASPuck.gSingleton;
                if (ths == null) return;

                if (!ths.UiBusy)
                {
                    if (args.MouseKey == MouseKeys.kMouseRight)
                    {
                        MenuTask.Perform();
                    }
                    else
                    {
                        ths.ShowOptionsMenu();
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnOptionsButtonMouseUp", e);
            }
        }

        public static void OnCloseClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                CASPuck ths = CASPuck.gSingleton;
                if (ths == null) return;

                //Common.DebugNotify(delegate { return "UiBusy: " + ths.mUiBusy + Common.NewLine + "LeaveCAS: " + ths.mLeaveCAS; });

                //if (!ths.UiBusy && !ths.mLeaveCAS)
                {
                    ths.UiBusy = true;
                    Simulator.AddObject(new Sims3.UI.OneShotFunctionTask(delegate
                    {
                        string entryKey = (Responder.Instance.CASModel.CASMode == CASMode.Full) ? "Ui/Caption/CAS/ExitDialog:Prompt" : "Ui/Caption/CAS/ExitDialog:AlternatePrompt";
                        if (TwoButtonDialog.Show(Common.LocalizeEAString(entryKey), Common.LocalizeEAString("Ui/Caption/Global:Yes"), Common.LocalizeEAString("Ui/Caption/Global:No")))
                        {
                            CASController singleton = CASController.Singleton;
                            singleton.AllowCameraMovement(false);

                            ICASModel cASModel = Responder.Instance.CASModel;
                            while (cASModel.IsProcessing())
                            {
                                SpeedTrap.Sleep();
                            }

                            Sims.CASBase.sWasCanceled = true;

                            sender.Enabled = false;
                            cASModel.RequestClearChangeReport();
                            singleton.SetCurrentState(CASState.None);
                        }
                        else
                        {
                            ths.UiBusy = false;
                        }
                    }));
                    eventArgs.Handled = true;
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnCloseClick", e);
            }
        }

        private static void CreateSimCallback(CASAgeGenderFlags species)
        {
            CASPuck ths = CASPuck.gSingleton;

            ICASModel cASModel = Responder.Instance.CASModel;
            CASController singleton = CASController.Singleton;

            /*
            if ((species == (CASAgeGenderFlags.None | CASAgeGenderFlags.Human)) && (cASModel.NumSimsInHousehold >= CASPuck.kMaxSimsPerHousehold))
            {
                singleton.ErrorMsg(CASErrorCode.TooManySims);
            }
            else if ((species != (CASAgeGenderFlags.None | CASAgeGenderFlags.Human)) && (cASModel.NumPetsInHousehold >= CASPuck.kMaxPetsPerHousehold))
            {
                singleton.ErrorMsg(CASErrorCode.TooManyPets);
            }
            */
            if (cASModel.NumInHousehold >= CASPuck.kMaxPerHousehold)
            {
                singleton.ErrorMsg(CASErrorCode.TooMany);
            }
            else
            {
                ths.mAttemptingToAdd = true;
                if (species == CASAgeGenderFlags.Human)
                {
                    CASController.Singleton.SetCurrentState(CASState.Summary);
                    ths.mAttemptingToAddSim = true;
                    cASModel.RequestCreateNewSim();
                }
                else
                {
                    CASController.Singleton.SetCurrentState(CASState.PetSummary);
                    ths.mAttemptingToAddPet = true;
                    cASModel.RequestCreateNewPet(species);
                }

                CASPuck.ShowInputBlocker();
            }
            ths.UpdateAddSimButtons();
        }

        private static void CallCreateSimCallback(CASPuck.ControlIDs id)
        {
            switch (id)
            {
                case CASPuck.ControlIDs.CreateHorseButton:
                    CreateSimCallback(CASAgeGenderFlags.Horse);
                    return;

                case CASPuck.ControlIDs.CreateDogButton:
                    CreateSimCallback(CASAgeGenderFlags.Dog);
                    return;

                case CASPuck.ControlIDs.CreateCatButton:
                    CreateSimCallback(CASAgeGenderFlags.Cat);
                    return;

                case CASPuck.ControlIDs.CreateSimButton:
                    CreateSimCallback(CASAgeGenderFlags.Human);
                    return;
            }
        }

        public static void OnCreateSimClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                CASPuck ths = CASPuck.gSingleton;

                Sims3.UI.Function f = null;
                if (ths.mAttemptingToAdd)
                {
                    if (f == null)
                    {
                        f = delegate
                        {
                            while (ths.mAttemptingToAdd)
                            {
                                SpeedTrap.Sleep();
                            }

                            // Custom
                            CallCreateSimCallback((CASPuck.ControlIDs)sender.ID);
                        };
                    }
                    Simulator.AddObject(new Sims3.UI.OneShotFunctionTask(f));
                }
                else
                {
                    // Custom
                    CallCreateSimCallback((CASPuck.ControlIDs)sender.ID);
                }

                ths.HideEditPopupMenu();
                ths.HideAddPopupMenu();
                eventArgs.Handled = true;
            }
            catch (Exception e)
            {
                Common.Exception("OnCreateSimClick", e);
            }
        }

        private static bool ShowRequiredItemsDialogTask(CASPuck ths)
        {
            int currentPreviewSim = ths.CurrentPreviewSim;
            ICASModel cASModel = Sims3.UI.Responder.Instance.CASModel;
            cASModel.OnSimUpdated -= ths.OnSimUpdated;
            cASModel.OnSimUpdated += ths.OnSimUpdatedRequiredItems;

            ths.mSimUpdated = false;
            //cASModel.RequestUpdateCurrentSim(true);
            CASLogic.CASOperationStack.Instance.Push(new UpdateCurrentSimOperationEx(true));
            while (!ths.mSimUpdated)
            {
                SpeedTrap.Sleep();
            }

            cASModel.OnSimUpdated -= ths.OnSimUpdatedRequiredItems;
            cASModel.OnSimUpdated += ths.OnSimUpdated;

            /*
            if (IsPetOnlyHousehold())
            {
                if (!AcceptCancelDialog.Show(Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/CAS/Family:PetOnlyHousehold", new object[0x0])))
                {
                    return false;
                }
            }
            else */if (!IsHouseholdValid())
            {
                SimpleMessageDialog.Show(null, Sims3.UI.Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/CAS/Family:InvalidHousehold", new object[0x0]), ModalDialog.PauseMode.PauseSimulator);
                return false;
            }

            cASModel.OnSimPreviewChange -= ths.OnSimPreviewChange;
            cASModel.OnSimPreviewChange += ths.OnSimPreviewChangeRequiredItems;
            ths.mInputBlocker.Visible = true;
            cASModel.RequestClearStack();

            cASModel.OnUndoRedoStackChanged -= ths.OnUndoRedoStackChanged;
            ths.mUndoButton.Enabled = false;
            ths.mRedoButton.Enabled = false;
            bool flag = true;
            int index = 0x0;
            foreach (ISimDescription description in cASModel.GetSimsInHousehold())
            {
                if (CASRequiredItemsDialog.ShouldShow(description))
                {
                    ths.mSimPreviewChanged = false;
                    cASModel.RequestSetPreviewSim(index);
                    ths.SelectSimButton(index);
                    while (!ths.mSimPreviewChanged || cASModel.IsProcessing())
                    {
                        SpeedTrap.Sleep();
                    }

                    CASController.Singleton.SetFullbodyCam(true);

                    ths.UiBusy = false;
                    if (!CASRequiredItemsDialog.Show())
                    {
                        flag = false;
                    }
                    ths.UiBusy = true;
                }

                if (description.TeenOrBelow)
                {
                    description.SimLifetimeWish = 0x0;
                }
                index++;
            }

            ths.mInputBlocker.Visible = false;
            cASModel.OnSimPreviewChange -= ths.OnSimPreviewChangeRequiredItems;
            cASModel.OnSimPreviewChange += ths.OnSimPreviewChange;
            cASModel.OnUndoRedoStackChanged += ths.OnUndoRedoStackChanged;

            cASModel.RequestSetPreviewSim(currentPreviewSim);
            ths.SelectSimButton(currentPreviewSim);
            if (!flag)
            {
                string str;
                if (GameUtils.IsInstalled(ProductVersion.EP5))
                {
                    str = Responder.Instance.LocalizationModel.LocalizeString("Ui/Tooltip/CAS/Puck:RequiredItemsMissingEP5", new object[0x0]);
                }
                else
                {
                    str = Responder.Instance.LocalizationModel.LocalizeString("Ui/Tooltip/CAS/Puck:RequiredItemsMissing", new object[0x0]);
                }

                SimpleMessageDialog.Show(null, str, ModalDialog.PauseMode.PauseSimulator, new Vector2(-1f, -1f), "ui_error", "ui_hardwindow_close");
            }

            return flag;
        }

        private static void AcceptSimCallback(CASPuck ths)
        {
            CASController.Singleton.AllowSimClicking(false);
            CASController.Singleton.AllowCameraMovement(false);

            //Responder.Instance.CASModel.RequestSaveSimToWorld();
            CASLogic.CASOperationStack.Instance.Push(new SaveSimToWorldOperationEx());

            ths.mLeaveCAS = true;
        }

        public static void OnAcceptSim(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                CASPuck ths = CASPuck.Instance;

                if (!ths.UiBusy)
                {
                    ths.UiBusy = true;
                    CASExitLoadScreen.DisableQuit = true;

                    // Custom
                    if (CASLogic.Instance.OutfitCategory == OutfitCategories.Career)
                    {
                        CASLogic.Instance.OutfitCategory = OutfitCategories.Everyday;

                        SpeedTrap.Sleep();
                    }

                    if (Responder.Instance.CASModel.CASMode == CASMode.CreateABot)
                    {
                        Function f = delegate
                        {
                            CASController.Singleton.SetCurrentState(CASState.BotSummary);
                            if (ths.ShowCABRequiredItemsDialogTask())
                            {
                                // Custom
                                AcceptSimCallback(ths);
                            }
                            else
                            {
                                ths.UiBusy = false;
                            }
                        };

                        Simulator.AddObject(new OneShotFunctionTask(f));
                    }
                    else
                    {
                        // Custom
                        AcceptSimCallback(ths);
                    }
                }
                eventArgs.Handled = true;
            }
            catch (Exception exception)
            {
                Common.Exception("OnAcceptHousehold", exception);
            }
        }

        private static void AcceptHouseholdCallback(CASPuck ths)
        {
            if (!ths.AllSimsNamed())
            {
                CASController.Singleton.ErrorMsg(CASErrorCode.AllSimsNeedName);
            }
            else
            {
                ICASModel cASModel = Responder.Instance.CASModel;
                if (cASModel.CASMode == CASMode.Full)
                {
                    if (cASModel.HouseholdName == null)
                    {
                        cASModel.HouseholdName = Responder.Instance.CASModel.LastName;
                    }

                    //cASModel.RequestUpdateCurrentSim(true);
                    CASLogic.CASOperationStack.Instance.Push(new UpdateCurrentSimOperationEx(true));

                    cASModel.SetHouseholdStartingFunds();
                    cASModel.RequestClearStack();

                    ths.mGotoCAF = true;
                }
            }
        }

        public static void OnAcceptHousehold(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                CASPuck ths = CASPuck.Instance;

                if (!ths.UiBusy && !ths.mAttemptingToAddSim)
                {
                    Sims.CASBase.StoreChanges();

                    ths.UiBusy = true;
                    Common.FunctionTask.Perform(delegate
                    {
                        if (Responder.Instance.CASModel.Species == CASAgeGenderFlags.Human)
                        {
                            CASController.Singleton.SetCurrentState(CASState.Summary);
                        }
                        else
                        {
                            CASController.Singleton.SetCurrentState(CASState.PetSummary);
                        }

                        if (ShowRequiredItemsDialogTask(ths))
                        {
                            ths.AcceptHouseholdCallback();
                        }
                        else
                        {
                            ths.UiBusy = false;
                        }

                        /*
                        // Custom
                        if (ShowRequiredItemsDialogTask(ths))
                        {
                            AcceptHouseholdCallback(ths);
                        }
                        else
                        {
                            ths.UiBusy = false;
                        }
                        */
                    });
                }
                eventArgs.Handled = true;
            }
            catch (Exception exception)
            {
                Common.Exception("OnAcceptHousehold", exception);
            }
        }

        public static bool CanCreateChild()
        {
            int humans = 0, dogs = 0, cats = 0, horses = 0;
            ICASModel cASModel = Responder.Instance.CASModel;
            foreach (ISimDescription sim in Responder.Instance.CASModel.GetSimsInHousehold())
            {
                if (sim.Age >= CASAgeGenderFlags.YoungAdult)
                {
                    if (sim.IsHuman)
                    {
                        humans++;
                    }
                    else if (sim.IsADogSpecies)
                    {
                        dogs++;
                    }
                    else if (sim.IsCat)
                    {
                        cats++;
                    }
                    else
                    {
                        horses++;
                    }
                }
            }
            return ((humans > 1) || (cats > 1) || (dogs > 1) || (horses > 1));
        }

        public static void OnSimUpdated(int simIndex)
        {
            try
            {
                CASPuck ths = CASPuck.gSingleton;
                if (ths == null) return;

                if (Responder.Instance.CASModel == null) return;

                if ((Responder.Instance.CASModel.CASMode == CASMode.Full) && !Responder.Instance.CASModel.EditingExistingSim())
                {
                    if (ths.mGotoGenetics)
                    {
                        ths.mGotoGenetics = false;
                        if (CanCreateChild())
                        {
                            CASController.Singleton.SetCurrentState(new CASState(CASTopState.Genetics, CASMidState.None, CASPhysicalState.None, CASClothingState.None));
                        }
                    }
                }

                ths.OnSimUpdated(simIndex);
            }
            catch (Exception e)
            {
                Common.Exception("OnSimUpdated", e);
            }
        }

        public static void OnSimPreviewChange(int simIndex)
        {
            try
            {
                CASPuck ths = CASPuck.gSingleton;
                if (ths == null) return;

                ths.HideEditPopupMenu();
                if ((simIndex >= 0) && (simIndex < ths.mSimButtons.Length))
                {
                    ths.OnSimPreviewChange(simIndex);
                }

                ths.ResetPanelCategories();
            }
            catch (Exception e)
            {
                Common.Exception("OnSimPreviewChange", e);
            }
        }

        public class MenuTask : Common.FunctionTask
        {
            protected MenuTask()
            { }

            public static void Perform()
            {
                new MenuTask().AddToSimulator();
            }

            protected override void OnPerform()
            {
                new InteractionOptionList<ICASOption, GameObject>.AllList(Common.Localize("CASInteraction:MenuName"), true).Perform(new GameHitParameters<GameObject>(Sim.ActiveActor, Sim.ActiveActor, GameObjectHit.NoHit));

                CASBase.InitAvailableParts();
            }
        }
    }
}
