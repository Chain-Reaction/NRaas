using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.States
{
    public class InWorldSubStateEx
    {
        public static void PlaceLotWizardCheck(InWorldSubState ths)
        {
            try
            {
                if ((GameStates.IsNewGame) && (GameStates.HasTravelData) && (!WorldData.IsPreviousWorld()))
                {
                    WorldData.SetPreviousWorld();

                    WorldName currentWorld = GameUtils.GetCurrentWorld();

                    switch (currentWorld)
                    {
                        case WorldName.China:
                        case WorldName.Egypt:
                        case WorldName.France:
                            break;
                        default:
                            if ((GameStates.sSingleton != null) && (GameStates.sSingleton.mInWorldState != null) && (GameStates.sSingleton.mInWorldState.mSubStates[0xb] != null))
                            {
                                bool isChangingWorlds = GameStates.sIsChangingWorlds;

                                WorldType currentWorldType = GameUtils.GetCurrentWorldType();

                                try
                                {
                                    GameUtils.WorldNameToType.Remove(currentWorld);
                                    GameUtils.WorldNameToType.Add(currentWorld, WorldType.Base);

                                    GameStates.sIsChangingWorlds = false;

                                    BinModel.Singleton.PopulateExportBin();

                                    EditTownModel.ClearPlaceLotsWizardFlags();

                                    Dictionary<CommercialLotSubType, string> lotTypesToPrompt = EditTownModel.LotTypesToPrompt;

                                    if (lotTypesToPrompt.Count != 0x0)
                                    {
                                        if (BinModel.Singleton.IsClear())
                                        {
                                            BinModel.Singleton.PopulateExportBin();
                                        }

                                        string promptText = Common.LocalizeEAString("Ui/Caption/GameEntry/PlaceEPLotsWizard:EnterPrompt") + '\n';

                                        try
                                        {
                                            InWorldSubState.AutoVenuPlacementData.Parse();

                                            foreach (KeyValuePair<CommercialLotSubType, string> pair in lotTypesToPrompt)
                                            {
                                                promptText = promptText + '\n' + pair.Value;
                                                if (InWorldSubState.AutoVenuPlacementData.HasPlacementDataForWorldName(World.GetWorldFileName()))
                                                {
                                                    UIBinInfo lotToPlaceInfo = null;
                                                    UIBinInfo targetLotInfo = null;
                                                    LotRotationAngle kLotRotateAuto = LotRotationAngle.kLotRotateAuto;
                                                    List<IExportBinContents> exportBinContents = BinModel.Singleton.ExportBinContents;
                                                    if (ths.FindRequiredEPVenuePlacementInfo(pair.Key, exportBinContents, out lotToPlaceInfo, out targetLotInfo, out kLotRotateAuto))
                                                    {
                                                        EditTownModel.AddForcedLocationLotToPlace(pair.Key, lotToPlaceInfo, targetLotInfo, kLotRotateAuto);
                                                    }
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            InWorldSubState.AutoVenuPlacementData.Shutdown();
                                        }

                                        if (TwoButtonDialog.Show(promptText, Common.LocalizeEAString("Ui/Caption/GameEntry/PlaceEPLotsWizard:Accept"), Common.LocalizeEAString("Ui/Caption/GameEntry/PlaceEPLotsWizard:Decline")))
                                        {
                                            GameStates.sSingleton.mInWorldState.mSubStates[0xb].PlaceRequiredEPVenues();
                                        }
                                    }
                                }
                                finally
                                {
                                    GameStates.sIsChangingWorlds = isChangingWorlds;

                                    GameUtils.WorldNameToType.Remove(currentWorld);
                                    GameUtils.WorldNameToType.Add(currentWorld, currentWorldType);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    ths.PlaceLotWizardCheck();
                }
            }
            catch (Exception e)
            {
                Common.Exception("PlaceLotWizardCheck", e);
            }
        }
    }
}
