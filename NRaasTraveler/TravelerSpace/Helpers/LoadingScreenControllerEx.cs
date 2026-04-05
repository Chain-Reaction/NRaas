using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sims3.Gameplay;
using Sims3.Gameplay.Core;
using Sims3.UI;
using Sims3.SimIFace;

namespace NRaas.TravelerSpace.Helpers
{
	public class LoadingScreenControllerEx
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
			None = 0x00u,
			Standard = 0x01u,
			LastFocusedLot = 0x02u,
			LastActiveHousehold = 0x04u,
		}

		public static void LoadTravellingLoadingScreen(string travelWorldName, WorldName worldName, bool travellingHome, bool isFirstTimeTravelingToFuture)
		{
			if (!LoadingScreenController.IsLayoutLoaded())
			{
				LoadingScreenController.sTravelWorldName = travelWorldName;
				LoadingScreenController.sbLoadingSaveGame = false;
				LoadingScreenController.sWorldName = worldName;
				LoadingScreenController.sbTravellingHome = travellingHome;
				LoadingScreenController.ResetStatics();
				LoadingScreenController.sFirstTimeTravelingToFuture = isFirstTimeTravelingToFuture;
				if ((worldName == WorldName.China || worldName == WorldName.Egypt || worldName == WorldName.France) && !travellingHome)
				{
					LoadingScreenController.sOverrideGameTipsFilename = "GameTipsEP1" + worldName.ToString();
				}
				else
				{
					if (worldName == WorldName.University && !travellingHome)
					{
						LoadingScreenController.sOverrideGameTipsFilename = "GameTipsEP9" + worldName.ToString();
					}
					else
					{
						if (worldName == WorldName.FutureWorld && !travellingHome)
						{
							LoadingScreenController.sOverrideGameTipsFilename = "GameTipsEP11" + worldName.ToString();
						}
						else
						{
							LoadingScreenController.sOverrideGameTipsFilename = string.Empty;
						}
					}
				}
				if (travellingHome || !sVacationWorldNames.Contains(worldName))
				{
					Responder.Instance.HudModel.PlayLoadLoopAudio(WorldName.Undefined);
				}
				else
				{
					Responder.Instance.HudModel.PlayLoadLoopAudio(worldName);
				}
				if (!LoadingScreenController.CreateInteractiveLoadingScreenIfEnabled() && LoadingScreenController.sLayout == null)
				{
					ResourceKey resKey = ResourceKey.CreateUILayoutKey("TravelLoadingScreen", 0u);
					LoadingScreenController.sLayout = UIManager.LoadLayoutAndAddToWindow(resKey, UICategory.Tooltips);
				}

				try
				{
					LoadingScreenControllerEx.HandleLoadingScreen();
				}
				catch (Exception exception)
				{
					Common.Exception("", exception);
				}
			}
		}

		public static void HandleLoadingScreen()
		{
			while (LoadingScreenController.Instance == null && !LoadingScreenController.IsLayoutLoaded())
			{
				Common.Sleep();
			}

			if (LoadingScreenController.sChosenLoadScreen != -1)
			{
				return;
			}

			string worldNameStr;
			string lastActiveLotName = string.Empty;
			string lastFocusedLotName = string.Empty;
			bool replaceTravelingScreen = false;
			WorldName worldName = LoadingScreenController.sWorldName;

			if (GameStates.IsEditingOtherTown)
			{
				//Common.WriteLog("EditingOtherTown");
				if (GameStates.sEditOtherWorldData.mState == GameStates.EditOtherWorldData.EditOtherWorldState.EditHomeWorld)
				{
					worldNameStr = GameStates.sEditOtherWorldData.mHomeWorldName;
					replaceTravelingScreen = true;
				}
				else
				{
					bool replaceCaption = false;

					if (GameStates.sEditOtherWorldData.mState == GameStates.EditOtherWorldData.EditOtherWorldState.ReturnToLiveMode)
					{
						worldNameStr = GameStates.sEditOtherWorldData.mWorldIStartedEditingInName;
						if (GameStates.sEditOtherWorldData.mHomeWorldName != GameStates.sEditOtherWorldData.mWorldIStartedEditingInName)
						{
							replaceCaption = !sVacationWorldNames.Contains(worldName);
						}
						else
						{
							replaceTravelingScreen = !sVacationWorldNames.Contains(worldName);
						}
					}
					else
					{
						worldNameStr = Responder.Instance.HudModel.LocationName(GameStates.sEditOtherWorldData.mDestWorld, true);
						if (!sVacationWorldNames.Contains(GameStates.sEditOtherWorldData.mDestWorld))
						{
							worldNameStr = WorldData.GetLocationName(GameStates.sEditOtherWorldData.mDestWorld);
							replaceCaption = true;
						}
					}

					if (replaceCaption)
					{
						Text text = LoadingScreenController.Instance.GetChildByID(116085280u, true) as Text;
						text.Caption = Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/TravelLoadingScreen:TravelingTo", new object[]
						{
							WorldData.GetLocationName(worldName)
						});
						replaceTravelingScreen = true;
					}
				}
			}
			else if (GameStates.IsTravelling || GameStates.IsOnVacation)
			{
				worldNameStr = Responder.Instance.HudModel.LocationName(GameStates.DestinationTravelWorld, true);
				if (LoadingScreenController.sbTravellingHome)
				{
					//Common.WriteLog("Traveling Home");
					worldName = WorldName.Undefined;
					worldNameStr = GameStates.sTravelData.mHomeWorld;
				}
				else if (!sVacationWorldNames.Contains(GameStates.DestinationTravelWorld))
				{
					//Common.WriteLog("Traveling non Vacation World");
					worldNameStr = WorldData.GetLocationName(GameStates.DestinationTravelWorld);

					Text text = LoadingScreenController.Instance.GetChildByID(116085280u, true) as Text;
					text.Caption = Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/TravelLoadingScreen:TravelingTo", new object[]
					{
						worldNameStr
					});
					replaceTravelingScreen = true;
				}
			}
			else if (LoadingScreenController.sbLoadingSaveGame)
			{
				//Common.WriteLog("Loading Save");
				worldNameStr = LoadingScreenController.sSaveGameMetadata.mWorldFile;
				if (!string.IsNullOrEmpty(Traveler.Settings.mLastActiveLot))
				{
					lastActiveLotName = Traveler.Settings.mLastActiveLot.ToLower().Replace(" ", "_");
					//Common.WriteLog("lastActiveLot: " + lastActiveLotName);
				}
				else
				{
					//Common.WriteLog("LAL EMPTY");
				}


				if (!string.IsNullOrEmpty(Traveler.Settings.mLastFocusedLot))
				{
					lastFocusedLotName = Traveler.Settings.mLastFocusedLot.ToLower().Replace(" ", "_");
					//Common.WriteLog("lastFocusedLot: " + lastFocusedLotName);
				}
				else
				{
					//Common.WriteLog("LFL EMPTY");
				}
			}
			else
			{
				//Common.WriteLog("Loading from World File");
				worldNameStr = LoadingScreenController.sWorldFileMetadata.mWorldFile;
				worldNameStr = worldNameStr.Remove(worldNameStr.Length - 6);
			}

			string screenImageResourceName = string.Empty;
			ProductVersion version = ProductVersion.BaseGame;
			worldNameStr = worldNameStr.ToLower();

			if (worldName != WorldName.China && !Regex.Match(worldNameStr, @"^china(_0x0859db4c)?$").Success)
			{
				if (worldName != WorldName.Egypt && !Regex.Match(worldNameStr, @"^egypt(_0x0859db48)?$").Success)
				{
					if (worldName != WorldName.France && !Regex.Match(worldNameStr, @"^france(_0x0859db50)?$").Success)
					{
						if (worldName != WorldName.TwinBrook && !Regex.Match(worldNameStr, @"^twinbrook(_0x09b610fa)?$").Success)
						{
							if (worldName != WorldName.NewDowntownWorld && !Regex.Match(worldNameStr, @"^bridgeport(_0x09ffe3d7)?$").Success)
							{
								if (worldName != WorldName.AppaloosaPlains && !Regex.Match(worldNameStr, @"^appaloosa ?plains(_0x0c50c56d)?$").Success)
								{
									if (worldName != WorldName.StarlightShores && !Regex.Match(worldNameStr, @"^starlight shores(_0x09b610ff)?$").Success)
									{
										if (worldName != WorldName.MoonlightFalls && !Regex.Match(worldNameStr, @"^moonlight falls(_0x09b61110)?$").Success)
										{
											if (worldName != WorldName.University && !Regex.Match(worldNameStr, @"^sims university(_0x0e41c954)?$").Success)
											{
												if (worldName != WorldName.IslaParadiso && !Regex.Match(worldNameStr, @"^isla ?paradiso(_0x0c50c382)?$").Success)
												{
													if (worldName == WorldName.FutureWorld || Regex.Match(worldNameStr, @"^oasis landing(_0x0f36012a)?$").Success)
													{
														screenImageResourceName = "world_loading_future";
														version = ProductVersion.EP11;
													}
												}
												else
												{
													screenImageResourceName = "ep10_world_loading_screen";
													version = ProductVersion.EP10;
												}
											}
											else
											{
												screenImageResourceName = "world_loading_university";
												version = ProductVersion.EP9;
											}
										}
										else
										{
											screenImageResourceName = "world_loading_EP7World";
											version = ProductVersion.EP7;
										}
									}
									else
									{
										screenImageResourceName = "world_loading_EP6World";
										version = ProductVersion.EP6;
									}
								}
								else
								{
									screenImageResourceName = "ep5_world_loading_screen";
									version = ProductVersion.EP5;
								}
							}
							else
							{
								screenImageResourceName = "world_loading_bridgeport";
								version = ProductVersion.EP3;
							}
						}
						else
						{
							screenImageResourceName = "world_loading_twinbrook";
						}
					}
					else
					{
						screenImageResourceName = "world_loading_paris";
						version = ProductVersion.EP1;
					}
				}
				else
				{
					screenImageResourceName = "world_loading_cairo";
					version = ProductVersion.EP1;
				}
			}
			else
			{
				screenImageResourceName = "world_loading_beijing";
				version = ProductVersion.EP1;
			}

			bool ignoreEAWorldScreen = !string.IsNullOrEmpty(screenImageResourceName) && !replaceTravelingScreen;

			if (string.IsNullOrEmpty(screenImageResourceName))
			{
				// if (worldName != WorldName.SunsetValley && !Regex.Match(worldNameStr, @"^sunset valley(_0x0859db3c)?$").Success)
				// {
				// 	if (worldName != WorldName.RiverView && !Regex.Match(worldNameStr, @"^riverview(_0x0859db43)?$").Success)
				// 	{
				// 		if (!Regex.Match(worldNameStr, @"^barnacle bay(_0x08866eb8)?$").Success)
				// 		{
				// 			if (!Regex.Match(worldNameStr, @"^hidden springs(_0x08866eb8)?$").Success)
				// 			{
				// 				if (!Regex.Match(worldNameStr, @"^lunar lakes(_0x08866eb8)?$").Success)
				// 				{
				// 					if (!Regex.Match(worldNameStr, @"^lucky palms(_0x08866eb8)?$").Success)
				// 					{
				// 						if (worldName != WorldName.DOT06 && !Regex.Match(worldNameStr, @"^sunlit tides(_0x0de07c78)?$").Success)
				// 						{
				// 							if (worldName != WorldName.DOT07 && !Regex.Match(worldNameStr, @"^monte vista(_0x0de07c83)?$").Success)
				// 							{
				// 								if (worldName != WorldName.DOT08 && !Regex.Match(worldNameStr, @"^aurora skies(_0x0de07c8b)?$").Success)
				// 								{
				// 									if (worldName != WorldName.DOT09 && !Regex.Match(worldNameStr, @"^dragon valley(_0x0de07c9c)?$").Success)
				// 									{
				// 										if (worldName != WorldName.DOT10 && !Regex.Match(worldNameStr, @"^midnight hollow(_0x0de07e7d)?$").Success)
				// 										{
				// 											if (worldName != WorldName.DOT11 && !Regex.Match(worldNameStr, @"^roaring heights(_0x0de07e86)?$").Success)
				// 											{
																if (Regex.Match(worldNameStr, @"^.+_0x[0-9a-f]{8}$").Success)
																{
																	worldNameStr = worldNameStr.Remove(worldNameStr.Length - 11);
																}
																screenImageResourceName = "world_loading_" + worldNameStr.Replace(' ', '_');
				// 											}
				// 											else
				// 											{
				// 												screenImageResourceName = "world_loading_rh";
				// 											}
				// 										}
				// 										else
				// 										{
				// 											screenImageResourceName = "world_loading_mh";
				// 										}
				// 									}
				// 									else
				// 									{
				// 										screenImageResourceName = "world_loading_dv";
				// 									}
				// 								}
				// 								else
				// 								{
				// 									screenImageResourceName = "world_loading_as";
				// 								}
				// 							}
				// 							else
				// 							{
				// 								screenImageResourceName = "world_loading_mv";
				// 							}
				// 						}
				// 						else
				// 						{
				// 							screenImageResourceName = "world_loading_st";
				// 						}
				// 					}
				// 					else
				// 					{
				// 						screenImageResourceName = "world_loading_lp";
				// 					}
				// 				}
				// 				else
				// 				{
				// 					screenImageResourceName = "world_loading_ll";
				// 				}
				// 			}
				// 			else
				// 			{
				// 				screenImageResourceName = "world_loading_hs";
				// 			}
				// 		}
				// 		else
				// 		{
				// 			screenImageResourceName = "world_loading_bb";
				// 		}
				// 	}
				// 	else
				// 	{
				// 		screenImageResourceName = "world_loading_riverview";
				// 	}
				// }
				// else
				// {
				// 	screenImageResourceName = "basegame_world_loading";
				// }
			}

			List<UIImage> list_loadingImages = new List<UIImage>();
			List<UIImage> list_travelingImages = new List<UIImage>();
			UIImage lastFocusedLotImage = null;
			UIImage lastActiveLotImage = null;
			UIImage screenImage = null;

			//Common.WriteLog("screenImageResourceName: " + screenImageResourceName);

			for (int num = 0; num <= 12; num++)
			{
				if (num == 0 && ignoreEAWorldScreen)
				{
					list_loadingImages.Add(null);
					continue;
				}
				if (num < 6)
				{
					string resourceName = (num == 0) ? screenImageResourceName : (screenImageResourceName + num);
					UIImage image = UIManager.LoadUIImage(ResourceKey.CreatePNGKey(resourceName, ResourceUtils.ProductVersionToGroupId(version)));
					if (num == 0 || (image != null && (Traveler.Settings.mLoadScreenImageType & LoadingImageType.Standard) == LoadingImageType.Standard))
					{
						if (image != null)
						{
							//Common.WriteLog("Found image " + resourceName);
						}
						list_loadingImages.Add(image);
					}
					else if ((Traveler.Settings.mLoadScreenImageType & LoadingImageType.Standard) != LoadingImageType.Standard)
					{
						if (num < 3)
						{
							//Common.WriteLog("Failed to locate " + resourceName);
						}
					}
				}
				else if (num >= 6 && num < 11 && GameStates.IsTravelling)
				{
					string resourceName = screenImageResourceName.Replace("loading", "traveling");
					if ((num - 6) != 0)
					{
						resourceName += (num - 6);
					}
					UIImage image = UIManager.LoadUIImage(ResourceKey.CreatePNGKey(resourceName, ResourceUtils.ProductVersionToGroupId(version)));
					if (image != null && (Traveler.Settings.mLoadScreenImageType & LoadingImageType.Standard) == LoadingImageType.Standard)
					{
						//Common.WriteLog("Found image " + resourceName);
						list_travelingImages.Add(image);
					}
				}
				if (num == 11)
				{
					string resourceName = screenImageResourceName + "_" + lastFocusedLotName;
					lastFocusedLotImage = UIManager.LoadUIImage(ResourceKey.CreatePNGKey(resourceName, ResourceUtils.ProductVersionToGroupId(version)));
				}
				if (num == 12)
				{
					string resourceName = screenImageResourceName + "_" + lastActiveLotName;
					lastActiveLotImage = UIManager.LoadUIImage(ResourceKey.CreatePNGKey(resourceName, ResourceUtils.ProductVersionToGroupId(version)));
				}
			}

			if ((Traveler.Settings.mLoadScreenImageType & LoadingImageType.LastFocusedLot) == LoadingImageType.LastFocusedLot && lastFocusedLotImage != null)
			{
				screenImage = lastFocusedLotImage;
			}
			else if ((Traveler.Settings.mLoadScreenImageType & LoadingImageType.LastActiveHousehold) == LoadingImageType.LastActiveHousehold && lastActiveLotImage != null)
			{
				screenImage = lastActiveLotImage;
			}

			if (screenImage == null)
			{
				if (list_travelingImages.Count > 0)
				{
					screenImage = RandomUtil.GetRandomObjectFromList(list_travelingImages);
				}
				else if (list_loadingImages.Count > 0)
				{
					screenImage = RandomUtil.GetRandomObjectFromList(list_loadingImages);
				}
			}

			if (screenImage != null)
			{
				(LoadingScreenController.Instance.Drawable as ImageDrawable).Image = screenImage;
				LoadingScreenController.Instance.Invalidate();
			}
			else
			{
				//Common.WriteLog("screenImage is null");
			}
		}
	}
}
