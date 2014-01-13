using NRaas.CommonSpace.Interactions;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class RegisterCommands : DebugEnablerInteraction<GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public RegisterCommands()
        { }

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            list.Add(new InteractionObjectPair(Singleton, obj));
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<RegisterCommands>
        {
            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("RegisterCommands:MenuName");
            }
        }

        protected static void RegisterGame(string name, string description, CommandHandler handler)
        {
            Commands.CommandInfo info = new Commands.CommandInfo(Commands.CommandType.Cheat, handler, description);

            if (Commands.sGameCommands == null) return;

            if (Commands.sGameCommands.mCommands == null) return;

            if (Commands.sGameCommands.mCommands.ContainsKey(name)) return;

            Commands.sGameCommands.mCommands.Add(name, info);
            if (Cheats.sTestingCheatsEnabled)
            {
                CommandSystem.RegisterCommand(name, description, handler, false);
            }
        }

        protected static void RegisterWorld(string name, string description, CommandHandler handler)
        {
            Commands.CommandInfo info = new Commands.CommandInfo(Commands.CommandType.Cheat, handler, description);

            if (Commands.sWorldCommands == null) return;

            if (Commands.sWorldCommands.mCommands == null) return;

            if (Commands.sWorldCommands.mCommands.ContainsKey(name)) return;

            Commands.sWorldCommands.mCommands.Add(name, info);
            if (Cheats.sTestingCheatsEnabled)
            {
                CommandSystem.RegisterCommand(name, description, handler, false);
            }
        }

        public override bool Run()
        {
            return Perform();
        }

        public static int OnSelectSim(object[] parameters)
        {
            try
            {
                List<Sim> sims = new List<Sim>(Sims3.Gameplay.Queries.GetObjects<Sim>());
                if (sims.Count > 0)
                {
                    PlumbBob.ForceSelectActor(sims[0]);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSelectSim", e);
            }
            return 0;
        }

        public static int OnMakeSim(object[] parameters)
        {
            try
            {
                Sim actor = Sim.MakeRandomSim(Service.GetPositionInRandomLot(LotManager.GetWorldLot()), CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Male);
                if (actor != null)
                {
                    PlumbBob.ForceSelectActor(actor);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnMakeSim", e);
            }
            return 0;
        }

        public static int OnEnableLifeEvents(object[] parameters)
        {
            try
            {
                bool bRet = true;
                Commands.ParseParamAsBool(parameters, out bRet, true);

                LifeEventManager.sIsLifeEventManagerEnabled = bRet;

                Camera.ToggleMapView();
            }
            catch (Exception e)
            {
                Common.Exception("OnTest", e);
            }

            return 0;
        }

        public static int OnResetAll(object[] parameters)
        {
            try
            {
                foreach (GameObject obj in Sims3.Gameplay.Queries.GetObjects<GameObject>())
                {
                    obj.SetObjectToReset();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnResetAll", e);
            }

            return 0;
        }

        public static int OnTest(object[] parameters)
        {
            try
            {
                Camera.ToggleMapView();
            }
            catch (Exception e)
            {
                Common.Exception("OnTest", e);
            }

            return 0;
        }

        public static void TransitionToEditTown()
        {
            try
            {
                if (PlumbBob.Singleton == null)
                {
                    PlumbBob.Startup();
                }

                PlumbBob.DoSelectActor(null, true);

                GameStates.TransitionToEditTown();
            }
            catch (Exception e)
            {
                Common.Exception("TransitionToEditTown", e);
            }
        }

        public static int OnForceHUD(object[] parameters)
        {
            try
            {
                Common.FunctionTask.Perform(TransitionToEditTown);
            }
            catch (Exception e)
            {
                Common.Exception("OnForceHUD", e);
            }

            return 0;
        }

        public static int OnRestartMotives(object[] parameters)
        {
            try
            {
                foreach (SimDescription sim in Household.EverySimDescription())
                {
                    Sim createdSim = sim.CreatedSim;
                    if (createdSim == null) continue;

                    if (sim.Household == null) continue;

                    if (sim.Household.IsSpecialHousehold) continue;

                    if (createdSim.Autonomy == null)
                    {
                        DebugEnabler.Notify("No Autonomy", sim.CreatedSim.ObjectId);
                        continue;
                    }

                    bool success = false;

                    if (createdSim.Autonomy.mLastTime > SimClock.ElapsedTime(TimeUnit.Minutes))
                    {
                        createdSim.Autonomy.mLastTime = 0;
                        success = true;
                    }

                    if (createdSim.Autonomy.Motives == null)
                    {
                        createdSim.Autonomy.mMotives = new Motives(createdSim, createdSim.Autonomy.mInteractionScorer);
                        success = true;
                    }

                    if (createdSim.Autonomy.Motives.GetMotive(CommodityKind.Hunger) == null)
                    {
                        createdSim.Autonomy.RecreateAllMotives();
                        success = true;
                    }

                    if (success)
                    {
                        DebugEnabler.Notify("Restarted Motive", sim.CreatedSim.ObjectId);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnRestartMotives", e);
            }

            return 0;
        }

        public static bool Perform()
        {
            try
            {
                if (Commands.sWorldCommands == null) return false;

                if (Commands.sWorldCommands.mCommands == null) return false;

                RegisterWorld("enableLifeEvents", "enables or disables the life event manager system", OnEnableLifeEvents);
                RegisterWorld("test", "runs a process I was using to test stuff", OnTest);
                RegisterWorld("forcehud", "forces the plumbbob to a given sim", OnForceHUD);
                RegisterWorld("restartmotives", "restart the motives of all residents", OnRestartMotives);
                RegisterWorld("selectsim", "selects a random sim in town", OnSelectSim);
                RegisterWorld("makesim", "spawn a random sim", OnMakeSim);
                RegisterWorld("resetall", "soft-reset of all objects in game", OnResetAll);

                RegisterGame("SaveSimAsLooseFile", "Sets the save sim mode. Usage: SaveSimAsLooseFile [true|false] or [1|0] (default is false)", Commands.OnSetSaveSimMode);
                RegisterGame("ExportAsEAContent", "Exports content as EA content. Usage: ExportAsEAContent [true|false] or [1|0] (default is false)", Commands.OnSetEAExportMode);
                RegisterGame("ExportAsPaidContent", "Exports content as Paid content. Usage: ExportAsPaidContent [true|false] or [1|0] (default is true)", Commands.OnSetEAExportModeAsPaidContent);
                RegisterGame("ShowInteractions", "Shows the names of the interactions above all the Sims' heads", Commands.OnShowInteractions);
                RegisterGame("Select", "Select a particular sim", Commands.OnSelectSim);
                RegisterGame("RefreshDnP", "Re-loads static Dreams and Promises data", Commands.OnRefreshDreamsAndPromises);
                RegisterGame("DnPOutputPrimitives", "outputs all DnP primitives so that they can be pasted into the dreamNames enum", Commands.OnOutputDnPPrimitives);
                RegisterGame("AssertOnRetestingDnPNodes", "Sets the game to assert if the DnP System removes a node when it retests the visible ones", Commands.OnSetDnPRetestAssert);
                RegisterGame("AssertOnDnPRefProblems", "Asserts when there are problems (duplicates) in the DnP reference List", Commands.OnSetDnPReferenceListAssert);
                RegisterGame("ToggleStringId", "Shows the string key after localized text", Commands.OnToggleStringID);
                RegisterGame("TriggerShareTutorial", "Triggers the sharing tutorialette", Commands.OnTriggerSharingTutorial);
                RegisterGame("RefreshOpportunities", "Re-loads static opportunities data", Commands.OnRefreshOpportuniites);
                RegisterGame("SetHouseholdName", "Sets the localization key for the active household's name", Commands.OnSetHouseholdName);
                RegisterGame("SetHouseholdDescription", "Sets the localization key for the active household's description", Commands.OnSetHouseholdDescription);
                RegisterGame("ResetLifetimeHappiness", "Resets the lifetime happiness of all the sims in the active household", Commands.OnResetLifetimeHappiness);
                RegisterGame("dialogs", "Usage: dialogs [on|off]. Enables/disables dialogs, for soak testing.", Commands.OnDialogs);
                RegisterGame("spawnerwarnings", "Usage: spawnerwarnings [on|off]. Enables/disables 'spawned object proximity warning' dialogs.", Commands.OnSpawnerWarnings);
                RegisterGame("nextLoadingScreen", "Usage: nextLoadingScreen [layoutName]. Forces the next interactive loading screen to be the one specified.", Commands.OnSetNextLoadingScreen);
                RegisterGame("traffic", "When turned off, the traffic manager doesn't simulate. Usage Traffic: [on/off]", Commands.OnTraffic);

                RegisterWorld("resetDnP", "resets the dreams and promises manager of the currently selected sim, using -all will reset all sims in current house's DnP managers", Commands.OnResetDnP);
                RegisterWorld("cleanhouseholds", "Cleans up non-instantiated Sims in all households (except for the Service household)", Commands.OnCleanHouseholds);
                RegisterWorld("piemenu", "piemenu [head|fail|types] [on|off]: toggle pie menu head, display of all failure reasons, or definition class names", Commands.OnPieMenu);
                RegisterWorld("lifetimehappiness", "Adds 50000 spendable lifetime happiness points to the selected sim.", Commands.OnLifetimeHappiness);
                RegisterWorld("lookat", "only [on/off] - only run lookats for the actor currently selected.  scores [on/off] - display lookat scores", Commands.OnLookAt);
                RegisterWorld("killplantableobjects", "Deletes all ingredients and plantable non-ingredients in the world.", Commands.OnKillPlantableObjects);
                RegisterWorld("refreshmotives", "Refreshes motives if they have got corrupted", Commands.OnRefreshMotives);
                RegisterWorld("refreshhouseholdthumbnails", "[s,m,l,xl,all] Refreshes all of the world's household thumbnails", Commands.OnRefreshHouseholdThumbnails);
                RegisterWorld("refreshlotthumbnails", "[s,m,l,xl,all] [roof] [houseboats] Refreshes all of the world's lot thumbnails", Commands.OnRefreshLotThumbnails);
                RegisterWorld("refreshsimthumbnails", "[s,m,l,xl,all] Refreshes all of the world's sim thumbnails", Commands.OnRefreshSimThumbnails);
                RegisterWorld("sacsdump", "dump managed sacs data", Commands.OnSacsDump);
                RegisterWorld("playobjectsound", "Plays the given audio clip as a positional sound on the active Sim", Commands.OnPlayObjectSound);
                RegisterWorld("stopobjectsounds", "Kills all object sounds started by playobjectsound", Commands.OnStopObjectSounds);
                RegisterWorld("esrbchecks", "[all|clothing|children] [on|off] turns periodic ESRB violation checks on or off.  Assumes all if type omitted. Toggles if on/off omitted.", Commands.OnEsrbChecks);
                RegisterWorld("loginteractions", "Logs all interactions on all Sims to the InteractionQueue channel.", Commands.OnLogInteractions);
                RegisterWorld("findsimwithoutfit", "[instanceId] Finds Sim with given outfit", Commands.OnFindSimWithOutfit);
                RegisterWorld("showbroadcast", "Toggles debug draw of all gameplay broadcasters. Usage: showbroadcast", Commands.OnBroadcastDebug);
                RegisterWorld("showwalkdist", "Toggles debug draw of walk style ranges. Usage: showwalkdist", Commands.OnWalkStyleDebug);
                RegisterWorld("zoom", "Camera zooms to the specified object id.", Commands.OnCameraZoom);
                RegisterWorld("storyprogression", "Sets the story progression mode.  Usage: storyprogression <Disabled|Enabled|Accelerated>", Commands.OnStoryProgression);
                RegisterWorld("meta", "Toggles debug display of meta/local (for autonomy debugging).  Usage: meta", Commands.OnShowMetaAutonomyCommand);
                RegisterWorld("debuglots", "Toggles debug display of lots (for meta-autonomy debugging).  Usage: debuglots", Commands.OnDebugLots);
                RegisterWorld("worldbuilderimport", "Imports character data from a spreadsheet that is passed in as the first parameter", Commands.WorldBuilderImport);
                RegisterWorld("worldbuilderoutput", "Will output the information about the current world needed for the worldbuilder spreadsheet", Commands.WorldBuilderOutputInfo);
                RegisterWorld("nukecruftysims", "As long as you have a CharacterImport.xml resource loaded, this cheat ensures that the number of sims and households in the world are consistent with the xml.", Commands.NukeCruftySims);
                RegisterWorld("SaveUniform", "Saves out parts of the current outfit", CASController.OnSaveUniformCommand);
                RegisterWorld("LoadUniform", "Overlay current sim with named uniform", CASController.OnLoadUniformCommand);
                RegisterWorld("DeleteAllObjects", "Deletes objs of given type. Use: DeleteAllObjects [FullTypeName,Assembly] Ex:DeleteAllObjects Sims3.Gameplay.Objects.Toys.Football,Sims3GameplayObjects", Commands.OnDeleteAllObjects);
                RegisterWorld("exterminate", "Kills <count> random Sims.", Commands.OnExterminate);
                RegisterWorld("spawn", "Creates <count> random Sims.", Commands.OnSpawn);
                RegisterWorld("debugSocial", "true/false turns social debugging on and off", Conversation.OnDebugSocialCommand);
                RegisterWorld("killobject", "Deletes a game object. Usage: killobject <object id>", Commands.OnKillObject);
                RegisterWorld("routedebuglog", "Toggles the route debug log on/off. You can also turn it off or on explictly by passing parameter \"on\" or \"off\". Usage: routedebuglog [\"on\" or \"off\"]", Commands.OnRouteDebugLog);
                RegisterWorld("DisplayLotPackageFileName", "on/off - switches lot tooltips on and off", LotManager.OnLotTooltipsCommand);
                RegisterWorld("money", "Sets family funds of the selected Sim. Usage: 'money' gives $1000000, 'money <number>' gives $number.", Commands.OnMoney);
                RegisterWorld("purgegenealogy", "Purges distant genealogy relations", Commands.OnPurgeDistantGenealogyRelations);
                RegisterWorld("logobjects", "Logs all objects on the current or specified lot. Usage: logobjects [lot]", Commands.OnLogObjectsOnLot);
                RegisterWorld("stressreset", "Stresses resetting an object.  Usage: stressreset [objectId]", Commands.OnStressReset);
                RegisterWorld("resetservicenpctraits", "Iterates through the current service NPC sims and resets their traits. Usage: resetservicenpctraits", Commands.OnServiceNpcTraitReset);
                RegisterWorld("babyboom", "Adds a baby and a toddler to every household in the world.", Commands.OnBabyBoom);
                RegisterWorld("interactioninfo", "Adds interactioninformation to queue and Sim mouseover text.", Commands.OnInteractionInfo);
                RegisterWorld("BuildBuyEnabledForLot", "enables build buy for the current active lot or specified lot. Usage: BuildBuyEnabledForLot [true/false][lot-Optional]", Commands.OnBuildBuyEnabledForLot);
                RegisterWorld("ancientCoinCount", "Sets family ancient count total. Usage: 'ancientCoinCount' displays current amount, 'ancientCointCOunt <number>' gives $number.", Commands.OnAncientCointCount);
                RegisterWorld("worldname", "Overrides world name. Usage: worldname [" + ParserFunctions.MakeCommaSeparatedList(Enum.GetNames(typeof(WorldName))).Replace("Undefined", "off") + "]", Commands.OnWorldName);
                RegisterWorld("visalevel", "Sets the visa level for the current world. Usage: 'visalevel' displays current visa level, 'visalevel <WorldName - Optional><number>' sets $number as the visa level for the current world or specified world(optional).", Commands.OnVisaLevel);
                RegisterWorld("lights", "Sets light intensities. Usage: 'lights [all|outdoor] [intensity]'.", Commands.OnLights);
                RegisterWorld("visaPoints", "Sets the visa point for the current world. Usage: 'visapoints <WorldName - Optional><number>' sets $number as the visa points for the current world or specified world(optional).", Commands.OnVisaPoints);
                RegisterWorld("cleanUpNectarBottles", "Meant to be run from world builder- destroys all nectar bottles that are in inventories", Commands.CleanUpNectarBottles);
                RegisterWorld("removeallpuddles", "Removes all puddles in the world", Commands.OnCleanupAllPuddles);
                RegisterWorld("AlwaysAllowBuildBuy", "When enabled, build mode and buy mode won't disable themselves during fires and burglary. Usage: AlwaysAllowBuildBuy [true/false]", Commands.OnAlwaysAllowBuildBuy);
                
                RegisterWorld("makePet", "Usage: makePet <ad|al|ac|ah|cd|cc|ch> <outfit name> <male|female>.  For example: \"makePet ad ProtoDog\" will add an adult dog with the ProtoDog outfit to the current household. Gender parameter is optional.", Commands.OnMakePet);
                RegisterWorld("makePetLocal", "Usage: makePetLocal <ad|al|ac|ah|cd|cc|ch> <outfit name> <male|female>.  Used for testing outfits on your local machine. For example: \"makePet ad ProtoDog\" will add an adult dog with the ProtoDog outfit to the current household. Gender parameter is optional.", Commands.OnMakePetLocal);
                RegisterWorld("exportOutfitXML", "Dump current outfit of CAS Sim in XML format ", CASController.OnExportOutfitXML);
                RegisterWorld("showNpcRoles", "Usage: 'showNpcRoles <RoleType> <Color.Preset>, Show all NPC Roles in the world. Role type is optional for only showing locations of NPCs that have that role. Color is for color to show for that specific roleType", Commands.OnShowNPCRoles);
                RegisterWorld("showPetPool", "Usage: 'showPetPool <PetPoolType> <Color.Preset>, Show all Pet Pools that are in the world. Pool type is options for ", Commands.OnShowPetPools);
                RegisterWorld("showPetPoolStats", "Show the number of the pets in each pool", Commands.OnShowPetPoolStats);
                RegisterWorld("visualizeSims", "Usage: 'visualizeSims <Category>, Shows all sims registered to the visualization category. Available categories: <All><SimWalkDog><None> You can get the category name from GPEs", Commands.OnVisualizeSims);
                RegisterWorld("fixupPetPools", "This cheat is for EP5 world only. It ensures that the PetPoolManager mapping of types to simdescriptions is consistent.", Commands.FixupPetPools);
                RegisterWorld("showSimStats", "This cheat is to assist understanding the sim instatiation histogram. This helps Design/GPEs figure our why there could be a surge in sim instantiation. Usage: 'showSimStats off' stops the command.", Commands.ShowSimStats);

                RegisterWorld("ShowPerformanceScore", "Usage: ShowPerformanceScore [on|off]. Shows performance score values via TNS messages", Commands.OnPerformanceMeterDebug);
                RegisterWorld("SetPerformanceScore", "Usage: SetPerformanceScore [value]. Sets performance score to value", Commands.OnPerformanceMeterSetValueDebug);

                RegisterWorld("ExportSuper", "Usage: ExportSupernaturalData to an xml file.", CASController.OnExportSupernaturalData);

                RegisterWorld("resetEveryone", "Usage: resetEveryone. Sets all sims in the world to reset", Commands.OnResetEveryone);
                RegisterWorld("tryStartNpcPoolParty", "Usage: 'tryStartNpcPoolParty, Try to start an Npc pool party 5 hours from now", Commands.OnTryStartNpcPoolParty);
                RegisterWorld("tryStartNpcCostumeParty", "Usage: 'tryStartNpcCostumeParty, Try to start an Npc costume party 5 hours from now", Commands.OnTryStartNpcCostumeParty);
                RegisterWorld("tryStartNpcHouseParty", "Usage: 'tryStartNpcHouseParty, Try to start an Npc house party 5 hours from now", Commands.OnTryStartNpcHouseParty);
                RegisterWorld("tryStartNpcFeastParty", "Usage: 'tryStartNpcFeastParty, Try to start an Npc feast party 5 hours from now", Commands.OnTryStartNpcFeastParty);
                RegisterWorld("holidayHouseLights", "Usage: 'holidayHouseLights [on|off], ", Commands.OnToggleHolidayHouseLights);
                RegisterWorld("computerSpecs", "Usage: 'computerSpecs'", Commands.OnGetComputerSpecs);
                RegisterWorld("holidayHouseLightCount", "'holidayHouseLightCount'", Commands.OnGetHolidayHouseLightCount);
                RegisterWorld("randomizeClouds", "Usage: 'randomizeClouds, or randomizeclouds [float 0-1]" , Commands.OnRandomizeClouds);
                RegisterWorld("allowUmbrellasInAllSocials", "Usage: 'allowUmbrellasInAllSocials (defaults to on), or allowUmbrellasInAllSocials On or allowUmbrellasInAllSocials Off", Commands.OnAllowUmbrellasInAllSocials);
                RegisterWorld("tryStartNpcJuiceKeggerParty", "Usage: 'tryStartNpcJuiceKeggerParty, Try to start an Npc party 5 hours from now", Commands.OnTryStartNpcJuiceKeggerParty);
                RegisterWorld("tryStartNpcBonfireParty", "Usage: 'tryStartNpcBonfireParty, Try to start an Npc party 5 hours from now", Commands.OnTryStartNpcBonfireParty);
                RegisterWorld("tryStartNpcVideoGameLANParty", "Usage: 'tryStartNpcVideoGameLANParty, Try to start an Npc party 5 hours from now", Commands.OnTryStartNpcVideoGameLANParty);
                RegisterWorld("tryStartNpcMasqueradeBallParty", "Usage: 'tryStartNpcMasqueradeBallParty, Try to start an Npc party 5 hours from now", Commands.OnTryStartNpcMasqueradeBallParty);
                RegisterWorld("tryStartNpcTailgatingParty", "Usage: 'tryStartNpcTailgatingParty, Try to start an Npc party 5 hours from now", Commands.OnTryStartNpcTailgatingParty);
                RegisterWorld("tryStartNpcVictoryParty", "Usage: 'tryStartNpcVictoryParty, Try to start an Npc party 5 hours from now", Commands.OnTryStartNpcVictoryParty);
                RegisterWorld("visualizeHotSpots", "Usage: visualizeHotSpots [on|off]. Shows or hides hot spots (green) and dead zones (red) in the world with a line and a circle shooting from the meta object.", Commands.OnVisualizeHotSpots);
                RegisterWorld("makeRobot", "Usage: makeRobot <outfitname|male|female> <humanoid|hovering>,  Add robot to the current household. Parameters are optional, randomize gender and outfit if not provided.", Commands.OnMakeRobot);
                RegisterWorld("makePlumbots", "Usage: makeRobotTownies, Convert townies to Robot form", Commands.OnApplyPlumbotOutfitsToSims);

                RegisterWorld("situationinfo", "Adds situation information to Sim mouseover text.", Commands.OnSituationInfo);
                RegisterWorld("marineLifeCreation", "Usage: 'marineLifeCreation [on|off], game defaults to on.", Commands.OnToggleMarineLifeCreation);
                RegisterWorld("MakeResortReviews", "Usage: 'MakeResortReviews <male|female>, Clear out existing resort reviews for the current active lot, and make all appropriate male or female ones", Commands.OnMakeResortReviews);
                RegisterWorld("visualizeBoats", "Usage: visualizeBoats [on|off], shows all the boats in the world and some useful stats.", Commands.OnVisualizeBoats);

                SpeedTrap.Sleep();
            }
            catch (Exception e)
            {
                Common.Exception("Perform", e);
            }
            return true;
        }
    }
}