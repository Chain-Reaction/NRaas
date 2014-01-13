using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class VersionStamp : Common.ProtoVersionStamp
    {
        public static readonly string sNamespace = "NRaas.Common";

        public static void ResetSettings()
        { }

        /*
         * \</KEY\>[:Wh]+\<STR\>
         * </KEY>\n<STR>
         * 
         * \</STR\>\n\n[:Wh]+\<KEY\>
         * </STR>\n\n<KEY>
         * 
\|\| \<span style\=\"background\-color\: \#ffff00\;\"\>{[0-9a-zA-Z\-]*}\<\/span\> \|\| \<span style\=\"background-color\: \#ffff00\;\"\>\[\[{[a-zA-Z0-9 ]*}{ Phase [a-zA-Z]*}\]\]\<\/span\> \|\|\= \<span style\=\"background-color\: \#ffff00\;\"\>{[0-9]*}\<\/span\> \|\| \<span style\=\"background-color\: \#ffff00\;\"\>\[\[ Revision Notes
|| <span style="background-color: #ffff00;">\1</span> || <span style="background-color: #ffff00;">[[\2\3]]</span> ||= <span style="background-color: #ffff00;">\4</span> || [[file:NRaas_\2_V\4.zip]] || <span style="background-color: #ffff00;">[[\2 Revision Notes
         * 
\|\| \<span style\=\"background-color\: \#00ff00\;\"\>{[0-9a-zA-Z\-]*}\<\/span\> \|\| \[\[{[a-zA-Z0-9 ]*}{ Phase [a-zA-Z]*}\]\] \|\|\= \<span style\=\"background-color\: \#00ff00\;\"\>{[0-9]*}\<\/span\> \|\| \<span style\=\"background-color\: \#00ff00\;\"\>Patch Update
|| <span style="background-color: #00ff00;">\1</span> || [[\2\3]] ||= <span style="background-color: #00ff00;">\4</span> || [[file:NRaas_\2_V\4.zip]] || <span style="background-color: #00ff00;">Patch Update

\|\| {[0-9a-zA-Z\-]*} \|\| \[\[{[a-zA-Z0-9 ]*}{ Phase [a-zA-Z]*}\]\] \|\|\= {[0-9]*} \|\| 
|| \1 || [[\2\3]] ||= \4 || [[file:NRaas_\2_V\4.zip]] || 
         */

        /*
         * Interpreter errors:
         *  Do not mix multiple function overrides with different number of template parameters
         * 
        MasterController
            CASClothingRow
        StoryProgression
        */

        /* TODO
         * 
         * .SimDescriptions
         * 
    Sims3.Gameplay.BinCommon
        Porter
    Sims3.Gameplay.GameStates
        Traveler
    Sims3.Gameplay.EditTownModel
        Traveler.ChangeLotType
    Sims3.Gameplay.ExportBinContents
        Porter
    Sims3.Gameplay.GameEntryMovingModel
        Mover.IsLotValid
    Sims3.Gameplay.Inventory
        Common.InventoryProxy
    Sims3.Gameplay.InWorldState
        Traveler
    Sims3.Gameplay.LiveModeState
        Traveler
    Sims3.Gameplay.PlayFlowState
        Traveler
    Sims3.Gameplay.TravelArrivalState
        Traveler
    Sims3.Gameplay.TravelDepartureState
        Traveler
    Sims3.Gameplay.Abstracts.GameObject
        Selector.OnPick
    Sims3.Gameplay.Abstracts.GameObject
        StoryProgression.NotifySellBase
    Sims3.Gameplay.Abstracts.RabbitHole.WooHooInRabbitHoleWith
         Woohooer 
    Sims3.Gameplay.Abstracts.RabbitHole.VisitRabbitHole
         Woohooer 
    Sims3.Gameplay.Abstracts.RabbitHole.VisitRabbitHoleWith
         Woohooer 
    Sims3.Gameplay.ActiveCareer.ActiveCareers.PrivateEyeInteractions.BreakIntoHouse.Definition
         Career
    Sims3.Gameplay.Actors.Sim.IsSupervised
        StoryProgression
    Sims3.Gameplay.Actors.Sim.AnimationClipDataForCAS
        Animator
    Sims3.Gameplay.Actors.Sim.AskToBehave
         GoHere
    Sims3.Gameplay.Actors.Sim.CustomizeCollarAndCoats
         MasterControllerIntegration
    Sims3.Gameplay.Actors.Sim.GiveGift
         MasterControllerIntegration
    Sims3.Gameplay.Actors.Sim.PlanOutfit
         MasterControllerIntegration
    Sims3.Gameplay.Actors.Sim.PlaySpecificLoopingAnimation
        Animator
    Sims3.Gameplay.Actors.Sim.PlaySpecificIdle
        Animator
    Sims3.Gameplay.Actors.Sim.ReadSomethingInInventory
         OnceRead
    Sims3.Gameplay.Actors.Sim.TurnToFaceAndAskToLeave
         GoHere
    Sims3.Gameplay.Actors.SimRoutingComponent
         GoHere
    Sims3.Gameplay.ActorSystems.AgingManager
         Overwatch
    Sims3.Gameplay.ActorSystems.AgingState .MergeTravelInformation
         Traveler
    Sims3.Gameplay.ActorSystems.BuffManager .ParseBuffData
         Common 
    Sims3.Gameplay.ActorSystems.Children.CaregiverRoutingMonitor
         GoHere
    Sims3.Gameplay.ActorSystems.OccultImaginaryFriend.OfferToTurnReal
         Woohooer 
    Sims3.Gameplay.ActorSystems.Pregnancy+HaveBabyHome
         Woohooer 
    Sims3.Gameplay.ActorSystems.Pregnancy+HaveBabyHospital
         Woohooer 
    Sims3.Gameplay.ActorSystems.Pregnancy+HaveLitter
         Woohooer
    Sims3.Gameplay.Careers.AfterschoolActivity
         Career
    Sims3.Gameplay.Careers.Career
         Career.ParseCareerData
    Sims3.Gameplay.Careers.SkillBasedCareer
         Career.ParseSkillBasedCareers
    Sims3.Gameplay.CAS.Household
        Common .PrepareToBecomeActiveHousehold
        Porter
    Sims3.Gameplay.CAS.MiniSimDescription
        Register.GetVacationWorldSimDescriptions
        RelationshipPanel.OnPickFromPanel
        Traveler.AddMinSims
    Sims3.Gameplay.CAS.SimDescription
        Common.Instantiate
        Dreamer.IncrementLifetimeHappiness
        RelationshipPanel.OnPickFromPanel
    Sims3.Gameplay.CAS.SimUtils.SimCreationSpec
        StoryProgression
    Sims3.Gameplay.Core.Bin
        Porter . ExportHousehold
        Traveler . ImportSim
    Sims3.Gameplay.Core.Commands
        DebugEnabler.RegisterGameCommands        
        DebugEnabler.RegisterWorldCommands
    Sims3.Gameplay.Core.HouseholdContents
        Porter
    Sims3.Gameplay.Core.Lot
        Selector.OnPick
    Sims3.Gameplay.Core.PostBoxJobBoard.CheckTheBoard
        Traveler
    Sims3.Gameplay.Core.Terrain.CatFishHere
         StoryProgression
    Sims3.Gameplay.Core.Terrain.GoHere
         GoHere
    Sims3.Gameplay.Core.Terrain.GoHereWith
         GoHere
    Sims3.Gameplay.Core.Terrain.SniffOut
         StoryProgression
    Sims3.Gameplay.Core.Terrain.TeleportMeHere.Definition
         GoHere
    Sims3.Gameplay.Core.ThumbnailHelper
        Overwatch
    Sims3.Gameplay.Core.VisitCommunityLot
         GoHere
    Sims3.Gameplay.Core.VisitCommunityLotWith
         GoHere
    Sims3.Gameplay.Core.VisitLot
         GoHere
    Sims3.Gameplay.Core.VisitLotWith
         GoHere
    Sims3.Gameplay.DreamsAndPromises.ActiveDreamNode
        Dreamer.OnCompletion
    Sims3.Gameplay.DreamsAndPromises.DreamsAndPromisesManager
        Dreamer.PartialUpdate
        Dreamer.Update
    Sims3.Gameplay.InteractionsShared.Sit
        WoohooerSauna.SaunaSit
    Sims3.Gameplay.Moving.GameplayMovingModel
        Mover.IsLotValid
    Sims3.Gameplay.Skills.DogHuntingSkill
        Careers
    Sims3.Gameplay.ObjectComponents.CatHuntingComponent.CatchPrey
        StoryProgression
    Sims3.Gameplay.ObjectComponents.RoutingComponent
        GoHere
    Sims3.Gameplay.ObjectComponents.SittableComponent
        WoohoorSauna
    Sims3.Gameplay.Objects.Beds.BedMultiPart
        SleepFreedom.CanShareBed
    Sims3.Gameplay.Objects.Beds.CuddleRelaxing
         Woohooer 
    Sims3.Gameplay.Objects.Beds.StartBedCuddleA
         Woohooer 
    Sims3.Gameplay.Objects.Beds.WooHoo
         Woohooer 
    Sims3.Gameplay.Objects.Book
         OnceRead
    Sims3.Gameplay.Objects.Book_PutAway.Definition
        LeaveToddlerBook
    Sims3.Gameplay.Objects.BookData
         Career
    Sims3.Gameplay.Objects.Bookshelf_ReadSomething
         OnceRead
    Sims3.Gameplay.Objects.Bookshelf_ReadToToddler.Definition
         OnceRead
    Sims3.Gameplay.Objects.BookToddler_ReadWithMenu.Definition
         OnceRead
    Sims3.Gameplay.Objects.Decorations.Mirror.ChangeAppearance
         MasterControllerIntegration
    Sims3.Gameplay.Objects.ElevatorDoors.WooHoo
         Woohooer 
    Sims3.Gameplay.Objects.Electronics.Phone
         Woohooer.ConsiderGeneratingPhoneCall
         Woohooer.FindCallParticipants
         Woohooer.ScheduleNextCall
    Sims3.Gameplay.Objects.Electronics.Computer.Move
         Mover
    Sims3.Gameplay.Objects.Electronics.Computer.SignChildUpForAfterschoolClass
         Career
    Sims3.Gameplay.Objects.Electronics.Computer.SignUpForAfterschoolClass
         Career
    Sims3.Gameplay.Objects.Electronics.Stereo+DanceTogetherA
         Woohooer
    Sims3.Gameplay.Objects.Electronics.Phone.CallInviteOverForeignVisitorsFromRelationPanel
         RelationshipPanel
    Sims3.Gameplay.Objects.Electronics.Phone.CallInviteOverSparTournamentChallenger
         Career
    Sims3.Gameplay.Objects.Electronics.Phone.CallToMove
         Mover
    Sims3.Gameplay.Objects.Electronics.Phone.CallToQuitWork.Definition 
         Career
    Sims3.Gameplay.Objects.Environment.Treehouse.WooHoo
         Woohooer 
    Sims3.Gameplay.Objects.Fishing.FishHere
         StoryProgression
    Sims3.Gameplay.Objects.FoodObjects
         Overwatch.LoadStoreData
    Sims3.Gameplay.Objects.HobbiesSkills.Inventing.InventionWorkbench.MakeFrankensim
         Woohooer 
    Sims3.Gameplay.Objects.HobbiesSkills.Inventing.TimeMachine.WooHoo
         Woohooer 
    Sims3.Gameplay.Objects.HobbieSkills.StylingStation.GetMakeover
         MasterControllerIntegration
    Sims3.Gameplay.Objects.HobbieSkills.StylingStation.MakeoverSelf
         MasterControllerIntegration
    Sims3.Gameplay.Objects.HobbieSkills.TattooChair.GiveTattoo
         MasterControllerIntegration
    Sims3.Gameplay.Objects.HobbieSkills.TattooChair.GiveTattooToSelf
         MasterControllerIntegration
    Sims3.Gameplay.Objects.Miscellaneous.ActorTrailer.WooHoo
         Woohooer 
    Sims3.Gameplay.Objects.Miscellaneous.TrashcanOutside.Rummage
         Career
    Sims3.Gameplay.Objects.Plumbing.Bathtub.ScubaAdventure
        Shooless
    Sims3.Gameplay.Objects.Plumbing.Bathtub.TakeBath
        Shooless
    Sims3.Gameplay.Objects.Plumbing.Bathtub.TakeBubbleBath
        Shooless
    Sims3.Gameplay.Objects.Plumbing.HotTubBase.AskToJoinHotTub.Definition
         Woohooer 
    Sims3.Gameplay.Objects.Plumbing.HotTubBase.CuddleSeatedWooHoo
         Woohooer 
    Sims3.Gameplay.Objects.Plumbing.HotTubBase.GetIn.SkinnyDipDefinition
         Woohooer 
    Sims3.Gameplay.Objects.Plumbing.Shower.TakeShower
        Shooless
        Woohooer 
    Sims3.Gameplay.Objects.Plumbing.Shower.WooHoo
         Woohooer 
    Sims3.Gameplay.Objects.Plumbing.Sink.SpongeBath
        Shooless
    Sims3.Gameplay.Objects.Plumbing.Toilet.UseToilet
        Shooless
    Sims3.Gameplay.Objects.Plumbing.Urinal.UseUrinal
        Shooless
    Sims3.Gameplay.Objects.Rabbitholes.DaySpa.GetTattooInRabbitHole
         MasterControllerIntegration
    Sims3.Gameplay.Objects.RabbitHoles.EquestrianCenter.BreedMare
         Woohooer 
    Sims3.Gameplay.Objects.RabbitHoles.Hospital
         MasterControllerIntegration.StartPlasticSurgeryCAS
    Sims3.Gameplay.Objects.RabbitHoles.Hospital.PlasticSurgery
         MasterControllerIntegration
    Sims3.Gameplay.Objects.RabbitHoles.SchoolRabbitHole.QuitAfterschoolClass
         Career
    Sims3.Gameplay.Objects.RabbitHoles.SchoolRabbitHole.SignChildUpForAfterschoolClass
         Career
    Sims3.Gameplay.Objects.RabbitHoles.SchoolRabbitHole.SignUpForAfterschoolClass
         Career
    Sims3.Gameplay.Objects.RabbitHoles.SchoolRabbitHole.TakeChildOutOfAfterschoolClass
         Career
    Sims3.Gameplay.Objects.ReadBookChooser.Definition
         OnceRead
    Sims3.Gameplay.Objects.Saddle.EditOutfit
         MasterControllerIntegration
    Sims3.Gameplay.Objects.Seating.CuddleSeated
         Woohooer 
    Sims3.Gameplay.Objects.ShelvesStorage.Dresser.CreateAnOutfit
         MasterControllerIntegration
    Sims3.Gameplay.Objects.TombObjects.Sarcophagus.WooHoo
         Woohooer 
    Sims3.Gameplay.Objects.Plumbing.HotTubBase.SkinnyDipFromHotTub.Definition
         Woohooer 
    Sims3.Gameplay.Objects.Seating.MultiSeatObject
         WoohooerSauna
    Sims3.Gameplay.Objects.Seating.StartSeatedCuddleA
         Woohooer 
    Sims3.Gameplay.Objects.Spawners.Gem
         StoryProgression
    Sims3.Gameplay.Objects.Spawners.Metal
         StoryProgression
    Sims3.Gameplay.Objects.Vehicles.CarRoutingComponent
        GoHere
    Sims3.Gameplay.Objects.Vehicles.GoHereWithSituation
         GoHere
    Sims3.Gameplay.Objects.Vehicles.FoodTruck
        Traffic
    Sims3.Gameplay.Objects.Vehicles.FoodTruckBase
        Traffic
    Sims3.Gameplay.Objects.Vehicles.FoodTruckManager
        Traffic
    Sims3.Gameplay.Objects.Vehicles.TrafficManager
        Traffic
    Sims3.Gameplay.Objects.Vehicles.IceCreamTruckManager.UpdateIceCreamTruckTask
        Traffic
    Sims3.Gameplay.Opportunities.OpportunityManager                    
         Career
    Sims3.Gameplay.Pools.AskToJoinSkinnyDipping.Definition
        Woohooer 
    Sims3.Gameplay.Pools.GetInPool.SkinnyDipDefinition
        Woohooer 
    Sims3.Gameplay.Pools.SwimHere.SkinnyDipDefinition
        Woohooer 
    Sims3.Gameplay.Roles.Role
        Register.IsSimGoodForRole
    Sims3.Gameplay.Roles.RoleData
        Register
    Sims3.Gameplay.Roles.RoleManager
        Register.UpdateAndGetRolesThatNeedPeople
    Sims3.Gameplay.Roles.RoleManagerTask
        Register.AddMoreInWorldSims
        Register.FillForeignRole
        Register.Simulate
    Sims3.Gameplay.Situations.YouShould
        Woohooer 
    Sims3.Gameplay.Skills.MartialArts
        Careers.GetAffinity
    Sims3.Gameplay.Skills.SkillManager
        Common.ParseSkillData
    Sims3.Gameplay.Socializing.CommodityData
        Woohooer 
    Sims3.Gameplay.Socializing.SocialRuleLHS
        Common
    Sims3.Gameplay.Socializing.SocialManager
        Common.ParseAction
        Common.ParseData
    Sims3.Gameplay.StoryProgression.GenerateOffspring
        Traveler
    Sims3.Gameplay.Tasks.PickObjectTask
         Selector
    Sims3.Gameplay.Tasks.SelectObjectTask
         Selector
    Sims3.Gameplay.Visa.TravelUtil
        Traveler.CanSimTriggerTravelToHomeWorld
        Traveler.CanSimTriggerTravelToVacationWorld
        Traveler.CheckForReasonsToFailTravel
        Traveler.LocateHomeAndPlaceSimsAtVacationWorld
        Traveler.GetPartnershipBonuses
        Traveler.ShowTravelToVacationWorldDialog
    Sims3.Metadata.Pattern
        Overwatch
    Sims3.UI.CAS.CAF
        MasterController
    Sims3.UI.CAS.CAFThumb
        MasterController
    Sims3.UI.CAS.CASClothing
        MasterController
    Sims3.UI.CAS.CASClothingCategory
        MasterController
    Sims3.UI.CAS.CASClothingRow
        MasterController
    Sims3.UI.CAS.CASCompositorController
         MasterControllerIntegration.OnMaterialsSkewerGridMouseDown
         MasterControllerIntegration.OnMaterialsSkewerGridMouseUp
    Sims3.UI.CAS.CASGenetics
         MasterControllerIntegration.OnFatherThumbnailClick
         MasterControllerIntegration.OnMotherThumbnailClick
    Sims3.UI.CAS.CASPuck
         MasterController
         MasterControllerIntegration.OnSimUpdated
         MasterControllerIntegration.OnSimPreviewChange
    Sims3.UI.CAS.ICASModel
        Common
    Sims3.UI.CAS.IMiniSimDescription
        Common
    Sims3.UI.FamilyTreeDialog
        MasterController
    Sims3.UI.GameEntry.EditTownController
        Traveler.ChangeLotType
        Traveler.ChangeLotTypeTask
    Sims3.UI.GameEntry.EditTownLibraryPanel
         MasterControllerIntegration.OnCASClick
    Sims3.UI.GameEntry.PlayFlowMenuPanel
         MasterControllerIntegration.OnMenuButtonClick
    Sims3.UI.Gameplay.MainMenu
        NoCD
    Sims3.UI.Hud.HudController
        Traveler
    Sims3.UI.Hud.IHudModel    
        Common
    Sims3.UI.Hud.RelationshipsPanel
        RelationshipPanel.OnSimOnLotMouseUp
        RelationshipPanel.OnSimNotOnLotMouseUp
    Sims3.UI.Hud.MotivesPanel
        Hybrid
    Sims3.UI.Hud.Skewer
        PortraitPanel
    Sims3.UI.Hud.SimDisplay
        Hybrid
    Sims3.UI.PartyPickerDialog
        MasterController
    Sims3.UI.NotificationManager
        SecondImage
    Sims3.UI.TripPlannerDialog
        Traveler
         * 
         */

        /* Changes
         * 
         * CanSimTreatAsHome vs LotHome
         * lot.IsControllable
         * Rabbithole.CollectMoney ?
         * public uint SaveGame(LoadSaveFileInfo info, bool isOnVacation)
         * CASModel.IsEditingUniform
         * Mermaid CAS Mode
         * public static int kNumberOfPlanAttemptsForStuckSim = 10;
         * public bool HouseholdOwnsResidentialLot(Lot toCheck)
         * LotManager.AllLotsWithoutCommonExceptions
         * FromWooHooUnderwaterCave = 0x796100541b597910L,
         * kWooHooInAllInOneBathroom = 0x587,
         * public void EnsureVacationDays(float hours)
         * 
         */
        public static readonly int sVersion = 1;
    }
}
