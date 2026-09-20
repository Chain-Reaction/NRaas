using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;

namespace NRaas.TravelerSpace.Helpers
{
    public class EditTownPuckEx
    {
        private readonly EditTownPuck This;

        public EditTownPuckEx(EditTownPuck editTownPuck)
        {
            This = editTownPuck;
        }

        public static void HandleUI()
        {
            EditTownPuck editTownPuck;
            while ((editTownPuck = EditTownPuck.Instance) == null)
            {
                Common.Sleep();
            }
            EditTownPuckEx instance = new EditTownPuckEx(editTownPuck);
            instance.Init();
        }

        private void Init()
        {
            // replace event handler of ReturnToLiveButton
            This.mReturnToLiveButton.Visible = false;
            UIManager.DeRegisterAllEvents(This.mReturnToLiveButton);
            This.mReturnToLiveButton.Click += new UIEventHandler<UIButtonClickEventArgs>(OnReturnToLive);
            This.mReturnToLiveButton.Visible = true;

            Dictionary<WorldName, string> worlds = new Dictionary<WorldName, string>();
            WorldData.GetWorlds(worlds);

            List<WorldName> availableLocations = new List<WorldName>();
            int numItems = 0;
            foreach (WorldName world in worlds.Keys)
            {
                if (world == GameUtils.GetCurrentWorld()) continue;

                string saveFileName;
                if (WorldData.GetSaveFileName(world, out saveFileName, true))
                {
                    if (saveFileName == GameStates.GetCurrentWorldName(true)) continue;

                    if (GameStates.sEditOtherWorldData != null)
                    {
                        if (saveFileName == GameStates.sEditOtherWorldData.mHomeWorldName) continue;
                    }
                    else if (GameStates.sTravelData != null)
                    {
                        if (saveFileName == GameStates.sTravelData.mHomeWorld) continue;
                    }
                }

                if (WorldData.IsEATravelWorld(world))
                {
                    numItems++;
                }
                else
                {
                    if (Traveler.Settings.GetHiddenWorlds(world)) continue;
                }
                availableLocations.Add(world);
            }
            numItems = availableLocations.Count > numItems ? 1 : 0;

            WindowBase childByID = This.GetChildByID(1404022083u, true);
            childByID.Visible = false;
            ItemGrid grid = This.GetChildByID(1404022084u, true) as ItemGrid;
            grid.Clear(true);

            ResourceKey layoutKey = ResourceKey.CreateUILayoutKey("LocationGridItem", 0u);
            if (!This.mModel.IsPlaceLotsWizardFlow)
            {
                if ((GameStates.sTravelData != null && GameStates.GetCurrentWorldName(true) != GameStates.sTravelData.mHomeWorld)
                    || (GameStates.sEditOtherWorldData != null && GameStates.GetCurrentWorldName(true) != GameStates.sEditOtherWorldData.mHomeWorldName))
                {
                    // add home item
                    AddGridItem(grid, layoutKey, WorldName.Undefined);
                    numItems++;
                }
                if (availableLocations.Count > 0)
                {
                    foreach (WorldName current in availableLocations.ToArray())
                    {
                        if (WorldData.IsEATravelWorld(current) && numItems < 6)
                        {
                            // create a item for every EA vacation world
                            AddGridItem(grid, layoutKey, current);
                            numItems++;
                            availableLocations.Remove(current);
                        }
                    }
                    if (availableLocations.Count > 0)
                    {
                        // item for other worlds
                        AddDefaultGridItem(grid, layoutKey, availableLocations);
                    }
                }
                if (numItems > 0)
                {
                    childByID.Visible = true;
                }
            }
        }

        private void OnReturnToLive(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            eventArgs.Handled = true;
            if (!This.mExitingGameEntry)
            {
                This.mExitingGameEntry = true;
                if (This.mIsInPloppablesMode)
                {
                    This.ExitNeighborhoodPloppablesMode(false);
                }
                Common.FunctionTask.Perform(ReturnToLive);
            }
        }

        private void ReturnToLive()
        {
            if (Responder.Instance.HudModel != null && Responder.Instance.OptionsModel != null && !Responder.Instance.OptionsModel.SaveGameInProgress && Responder.Instance.HudModel.IsGameEntryState())
            {
                if (WorldData.GetWorldType(GameUtils.GetCurrentWorld()) == WorldType.Vacation && !This.mModel.IsAnyLotBaseCampEP1())
                {
                    ILocalizationModel localizationModel = Responder.Instance.LocalizationModel;
                    string titleText = localizationModel.LocalizeString("Ui/Caption/Global:Failed", new object[0]);
                    string messageText = localizationModel.LocalizeString("Ui/Caption/GameEntry/EditTown/EP01:NeedBasecamp", new object[0]);
                    SimpleMessageDialog.Show(titleText, messageText, ModalDialog.PauseMode.PauseSimulator, new Vector2(-1f, -1f), "ui_error", "ui_hardwindow_close");
                }
                else
                {
                    if (WorldData.GetWorldType(GameUtils.GetCurrentWorld()) == WorldType.Future && !This.mModel.IsAnyLotBaseCampFutureEP11())
                    {
                        ILocalizationModel localizationModel2 = Responder.Instance.LocalizationModel;
                        string titleText2 = localizationModel2.LocalizeString("Ui/Caption/Global:Failed", new object[0]);
                        string messageText2 = localizationModel2.LocalizeString("Ui/Caption/GameEntry/EditTown/EP11:NeedBasecampFuture", new object[0]);
                        SimpleMessageDialog.Show(titleText2, messageText2, ModalDialog.PauseMode.PauseSimulator, new Vector2(-1f, -1f), "ui_error", "ui_hardwindow_close");
                    }
                    else
                    {
                        if (!This.mModel.IsPlaceLotsWizardFlow || AcceptCancelDialog.Show(Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/GameEntry/PlaceEPLotsWizard:CancelPrompt", new object[0])))
                        {
                            This.HidePanels();
                            This.UpdateBackButton(true);
                            if (!EditTownModelEx.ExitEditTown(Responder.Instance.EditTownModel as EditTownModel, false))
                            {
                                This.UpdateBackButton(false);
                            }
                            else
                            {
                                This.mModel.IsPlaceLotsWizardFlow = false;
                            }
                        }
                    }
                }
            }
            This.mExitingGameEntry = false;
        }

        private void OnGridItemMouseDown(WindowBase sender, UIMouseEventArgs eventArgs)
        {
            if (!This.mExitingGameEntry)
            {
                This.mExitingGameEntry = true;
                Simulator.AddObject(new OneShotFunctionWithParams(GotoWorldTask, sender));
            }
        }

        private void GotoWorldTask(object inObject)
        {
            WindowBase windowBase = inObject as WindowBase;
            if (Responder.Instance.HudModel != null && Responder.Instance.OptionsModel != null && windowBase != null && !Responder.Instance.OptionsModel.SaveGameInProgress && Responder.Instance.HudModel.IsGameEntryState())
            {
                if (WorldData.GetWorldType(GameUtils.GetCurrentWorld()) == WorldType.Vacation && !This.mModel.IsAnyLotBaseCampEP1())
                {
                    ILocalizationModel localizationModel = Responder.Instance.LocalizationModel;
                    string titleText = localizationModel.LocalizeString("Ui/Caption/Global:Failed", new object[0]);
                    string messageText = localizationModel.LocalizeString("Ui/Caption/GameEntry/EditTown/EP01:NeedBasecamp", new object[0]);
                    SimpleMessageDialog.Show(titleText, messageText, ModalDialog.PauseMode.PauseSimulator, new Vector2(-1f, -1f), "ui_error", "ui_hardwindow_close");
                }
                else
                {
                    if (WorldData.GetWorldType(GameUtils.GetCurrentWorld()) == WorldType.Future && !This.mModel.IsAnyLotBaseCampFutureEP11())
                    {
                        ILocalizationModel localizationModel2 = Responder.Instance.LocalizationModel;
                        string titleText2 = localizationModel2.LocalizeString("Ui/Caption/Global:Failed", new object[0]);
                        string messageText2 = localizationModel2.LocalizeString("Ui/Caption/GameEntry/EditTown/EP11:NeedBasecampFuture", new object[0]);
                        SimpleMessageDialog.Show(titleText2, messageText2, ModalDialog.PauseMode.PauseSimulator, new Vector2(-1f, -1f), "ui_error", "ui_hardwindow_close");
                    }
                    else
                    {
                        This.HidePanels();
                        This.UpdateBackButton(true);

                        // Custom
                        WorldName world = WorldName.Undefined;
                        string worldName = null;
                        ICollection<WorldName> worlds = windowBase.Tag as ICollection<WorldName>;
                        if (worlds != null)
                        {
                            // mulitple worlds (DefaultGridItem)
                            List<WorldItem> options = new List<WorldItem>();
                            foreach (WorldName value in worlds)
                            {
                                ResourceKey iconKey = ResourceKey.CreatePNGKey(WorldData.GetWorldInfoIconKey(value), 0u);
                                options.Add(new WorldItem(value, iconKey));
                            }

                            WorldItem selection = new CommonSelection<WorldItem>(Common.Localize("EditTownPuckEx:SelectTownCaption"), options).SelectSingle();
                            if (selection != null)
                            {
                                world = selection.Value;
                                worldName = selection.Name;
                            }
                        }
                        else
                        {
                            // single world 
                            world = (WorldName)windowBase.Tag;
                            worldName = windowBase.TooltipText;
                        }

                        if (worldName != null && AcceptCancelDialog.Show(Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/EditTown/Puck:EditLocationPrompt", new object[]
                        {
                            worldName
                        })) && EditTownController.PromptForNonEmptyClipboard())
                        {
                            GameStatesEx.EditWorld(world, false);
                            LotManager.sWorldHasDiveLots = false;
                            foreach (Lot allLot in LotManager.AllLots)
                            {
                                allLot.CalculateMetaAutonomyTypeAndConsiderAddingToPublicMetaObjects();
                                allLot.UpdateVirtualResidentialSlots();
                                if (!LotManager.sWorldHasDiveLots && allLot.CommercialLotSubType == CommercialLotSubType.kEP10_Diving)
                                {
                                    LotManager.sWorldHasDiveLots = true;
                                }
                            }
                            if (!World.IsEditInGameFromWBMode())
                            {
                                Household.FindSuitableServiceAndTownieAccomodations();
                            }
                            if (Household.ActiveHousehold != null)
                            {
                                foreach (Sim allActor in Household.ActiveHousehold.AllActors)
                                {
                                    if (allActor.MapTagManager != null)
                                    {
                                        allActor.MapTagManager.Reset();
                                    }
                                }
                            }
                            LotManager.ForceReplanOfAllSimRoutes();
                        }
                        else
                        {
                            This.UpdateBackButton(false);
                        }
                    }
                }
            }
            This.mExitingGameEntry = false;
        }

        private void AddGridItem(ItemGrid grid, ResourceKey layoutKey, WorldName world)
        {
            Window window = UIManager.LoadLayout(layoutKey).GetWindowByExportID(1) as Window;
            if (window != null)
            {
                grid.AddItem(new ItemGridCellItem(window, null));
                Window window2 = window.GetChildByID(2u, false) as Window;
                if (window2 != null)
                {
                    ImageDrawable imageDrawable = window2.Drawable as ImageDrawable;
                    string text = Responder.Instance.HudModel.LocationIconName(world);
                    if (imageDrawable != null && text != null)
                    {
                        imageDrawable.Image = UIManager.LoadUIImage(ResourceKey.CreatePNGKey(text, 0u));
                    }
                    window.MouseDown += new UIEventHandler<UIMouseEventArgs>(OnGridItemMouseDown);
                    string value = Responder.Instance.HudModel.LocationName(world);
                    if (!string.IsNullOrEmpty(value))
                    {
                        window.TooltipText = Responder.Instance.HudModel.LocationName(world, true);
                    }
                    window.Tag = world;
                }
            }
        }

        private void AddDefaultGridItem(ItemGrid grid, ResourceKey layoutKey, ICollection<WorldName> worlds)
        {
            Window window = UIManager.LoadLayout(layoutKey).GetWindowByExportID(1) as Window;
            if (window != null)
            {
                grid.AddItem(new ItemGridCellItem(window, null));
                Window window2 = window.GetChildByID(2u, false) as Window;
                if (window2 != null)
                {
                    ImageDrawable imageDrawable = window2.Drawable as ImageDrawable;
                    if (imageDrawable != null)
                    {
                        imageDrawable.Image = UIManager.LoadUIImage(ResourceKey.CreatePNGKey("glb_i_other", 0u));
                    }
                    window.MouseDown += new UIEventHandler<UIMouseEventArgs>(OnGridItemMouseDown);
                    window.TooltipText = Common.Localize("EditTownPuckEx:DefaultGridItemTooltipText");
                    window.Tag = worlds;
                }
            }
        }

        private class WorldItem : CommonOptionItem
        {
            protected WorldName mValue;

            public WorldItem()
            { }

            public WorldItem(WorldName value, ResourceKey iconKey)
                : base(WorldData.IsEATravelWorld(value) ? Responder.Instance.HudModel.LocationName(value, true) : WorldData.GetLocationName(value), -1, iconKey)
            {
                mValue = value;
            }

            public virtual WorldName Value
            {
                get
                {
                    return mValue;
                }
            }

            public override string DisplayValue
            {
                get
                {
                    return null;
                }
            }
        }
    }
}
