using System.Collections.Generic;
using Sims3.Gameplay;
using Sims3.Gameplay.Core;
using Sims3.UI;
using Sims3.SimIFace;

namespace NRaas.TravelerSpace.Helpers
{
    public class LoadingScreenControl
    {
        public static readonly List<WorldName> sVacationWorldNames = new List<WorldName>
        {
            WorldName.China,
            WorldName.Egypt,
            WorldName.France,
            WorldName.University,
            WorldName.FutureWorld
        };
        public enum LoadingImageType : uint
        {
            None = 0x00,
            Standard = 0x01,
            LastFocusedLot = 0x02,
            LastActiveHousehold = 0x04,            
        }

        public static void HandleLoadingScreen()
        {
            while (!LoadingScreenController.IsLayoutLoaded())
            {
                Common.Sleep();                
            }

            if (LoadingScreenController.sbTravellingHome || !sVacationWorldNames.Contains(LoadingScreenController.sWorldName))
            {
                Responder.Instance.HudModel.PlayLoadLoopAudio(WorldName.Undefined);
            }

            if (LoadingScreenController.sChosenLoadScreen == -1)
            {
                string defaultImg = "";
                string loadingWorld = "";
                string loadingLastLot = "";
                string loadingActiveLot = "";
                bool isEADest = true;
                ProductVersion version = ProductVersion.BaseGame;
                bool changed = false;

                if (GameStates.IsTravelling)
                {
                    string travelWorldName = Responder.Instance.HudModel.LocationName(GameStates.DestinationTravelWorld, true);
                    loadingWorld = travelWorldName;
                    if (LoadingScreenController.sbTravellingHome && GameStates.sTravelData != null)
                    {
                        travelWorldName = GameStates.sTravelData.mHomeWorld;
                        loadingWorld = travelWorldName;
                    }
                    else
                    {
                        if (!sVacationWorldNames.Contains(GameStates.DestinationTravelWorld))
                        {
                            travelWorldName = WorldData.GetLocationName(GameStates.DestinationTravelWorld);
                            loadingWorld = travelWorldName;
                            isEADest = false;
                        }
                    }

                    Text text = LoadingScreenController.sInstance.GetChildByID(116085280u, true) as Text;
                    text.Caption = Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/TravelLoadingScreen:TravelingTo", travelWorldName);
                    
                    changed = true;
                } else if (LoadingScreenController.sbLoadingSaveGame)
                {
                    //Common.WriteLog("Loading Save");
                    loadingWorld = LoadingScreenController.sSaveGameMetadata.mWorldFile;

                    if (!string.IsNullOrEmpty(Traveler.Settings.mLastActiveLot))
                    {
                        loadingActiveLot = Traveler.Settings.mLastActiveLot.ToLower().Replace(" ", "_");
                        //Common.WriteLog("lastActiveLot: " + loadingActiveLot);
                    } else
                    {
                        //Common.WriteLog("LAL EMPTY");
                    }

                    if (!string.IsNullOrEmpty(Traveler.Settings.mLastFocusedLot))
                    {
                        loadingLastLot = Traveler.Settings.mLastFocusedLot.ToLower().Replace(" ", "_");
                        //Common.WriteLog("lastFocusedLot: " + loadingLastLot);
                    }
                    else
                    {
                        //Common.WriteLog("LFL EMPTY");
                    }

                }
                else
                {
                    loadingWorld = LoadingScreenController.sWorldFileMetadata.mWorldFile;
                    loadingWorld = loadingWorld.Remove(loadingWorld.Length - 6);
                }

                if (string.IsNullOrEmpty(defaultImg))
                {                    
                    switch (LoadingScreenController.sWorldName)
                    {
                        case WorldName.IslaParadiso:
                            defaultImg = "ep10_world_loading_screen";
                            version = ProductVersion.EP10;
                            break;
                        case WorldName.MoonlightFalls:
                            defaultImg = "world_loading_EP7World";
                            version = ProductVersion.EP7;
                            break;
                        case WorldName.StarlightShores:
                            defaultImg = "world_loading_EP6World";
                            version = ProductVersion.EP6;
                            break;
                        case WorldName.AppaloosaPlains:
                            defaultImg = "ep5_world_loading_screen";
                            version = ProductVersion.EP5;
                            break;
                        case WorldName.NewDowntownWorld:
                            defaultImg = "world_loading_bridgeport";
                            version = ProductVersion.EP3;
                            break;
                        case WorldName.China:
                            defaultImg = "world_loading_beijing";
                            break;
                        case WorldName.Egypt:
                            defaultImg = "world_loading_cairo";
                            break;
                        case WorldName.France:
                            defaultImg = "world_loading_paris";
                            break;
                        case WorldName.FutureWorld:
                            defaultImg = "world_loading_future";
                            version = ProductVersion.EP11;
                            break;
                        case WorldName.Undefined:
                            isEADest = false;
                            break;
                    }
                }

                if (string.IsNullOrEmpty(defaultImg))
                {
                    defaultImg = "world_loading_" + loadingWorld.ToLower().Replace(' ', '_');
                }

                List<UIImage> defaultImages = new List<UIImage>();
                List<UIImage> travelImages = new List<UIImage>();
                UIImage lastLotImage = null;
                UIImage lastActiveLotImage = null;

                if (!string.IsNullOrEmpty(defaultImg))
                {
                    //Common.WriteLog("defaultImg: " + defaultImg);
                    string imgTest = "";
                    int i = 0;
                    UIImage setImage = null;
                    while (i <= 12)
                    {
                        if (i == 0 && isEADest)
                        {
                            defaultImages.Add(null);
                            i++;
                            continue;
                        }

                        if (i < 6)
                        {
                            if (i != 0)
                            {
                                imgTest = defaultImg + i;
                            } else
                            {
                                imgTest = defaultImg;
                            }

                            UIImage uiImg = UIManager.LoadUIImage(ResourceKey.CreatePNGKey(imgTest, ResourceUtils.ProductVersionToGroupId(version)));

                            if (uiImg != null)
                            {
                                //Common.WriteLog("Found image " + imgTest);
                                defaultImages.Add(uiImg);
                            } else
                            {
                                if (i < 3)
                                {
                                    //Common.WriteLog("Failed to locate " + imgTest);
                                }
                            }
                        }
                        else if (i >= 6 && i < 11 && GameStates.IsTravelling)
                        {
                            imgTest = defaultImg.Replace("loading", "traveling");
                            int travelimg = i - 6;
                            if (travelimg != 0)
                            {
                                imgTest += i;
                            }

                            UIImage uiImg = UIManager.LoadUIImage(ResourceKey.CreatePNGKey(imgTest, ResourceUtils.ProductVersionToGroupId(version)));

                            if (uiImg != null)
                            {
                                //Common.WriteLog("Found image " + imgTest);
                                travelImages.Add(uiImg);
                            }
                        }

                        if (i == 11)
                        {
                            imgTest = defaultImg + "_" + loadingLastLot;

                            lastLotImage = UIManager.LoadUIImage(ResourceKey.CreatePNGKey(imgTest, ResourceUtils.ProductVersionToGroupId(version)));
                        }

                        if (i == 12)
                        {
                            imgTest = defaultImg + "_" + loadingActiveLot;

                            lastActiveLotImage = UIManager.LoadUIImage(ResourceKey.CreatePNGKey(imgTest, ResourceUtils.ProductVersionToGroupId(version)));
                        }
                        
                        i++;
                    }

                    if (Traveler.Settings.mLoadScreenImageType == LoadingImageType.LastFocusedLot && lastLotImage != null)
                    {
                        setImage = lastActiveLotImage;
                    } else if (Traveler.Settings.mLoadScreenImageType == LoadingImageType.LastActiveHousehold && lastActiveLotImage != null)
                    {
                        setImage = lastActiveLotImage;
                    }

                    if (setImage == null)
                    {
                        if (travelImages.Count > 0)
                        {
                            setImage = RandomUtil.GetRandomObjectFromList<UIImage>(travelImages);
                        } else if(defaultImages.Count > 0)
                        {
                            setImage = RandomUtil.GetRandomObjectFromList<UIImage>(defaultImages);
                        }
                    }
                   
                    if (setImage != null)
                    {
                        ImageDrawable imageDrawable = LoadingScreenController.sInstance.Drawable as ImageDrawable;
                        imageDrawable.Image = setImage;
                        changed = true;
                    } else
                    {
                        //Common.WriteLog("setImage is null");
                    }
                }

                if (changed)
                {
                    LoadingScreenController.sInstance.Invalidate();
                }
            }
        }
        
    }
}
