using Sims3.Gameplay.Objects.Decorations;
using System.Collections.Generic;
using TS3Apartments;
using Sims3.SimIFace;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.InteractionsShared;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace.VideoRecording;
using Sims3.Gameplay.UI;

namespace Sims3.Gameplay.Objects.Miscellaneous.TS3Apartments
{
    public class Controller : Painting
    {
        #region intialization stuff
        public static AbstractArtObject.ViewTuning kViewTuning = new AbstractArtObject.ViewTuning();

        public override AbstractArtObject.ViewTuning TuningView
        {
            get { return Controller.kViewTuning; }
        }

        public List<ApartmentFamily> Families;

        [Persistable(false)]
        public AlarmHandle mBills = AlarmHandle.kInvalidHandle;
        private static bool firstLoad = true;


        public override void OnCreation()
        {
            base.OnCreation();
            this.Families = new List<ApartmentFamily>();
        }

        public override void OnStartup()
        {
            base.OnStartup();

            #region remove art interactions
            if (GameUtils.IsInstalled(ProductVersion.EP5))
            {
                base.RemoveInteractionByType(Sim.FreakOutAtObject.Singleton);
            }
            if (GameUtils.IsInstalled(ProductVersion.EP9))
            {
                base.RemoveInteractionByType(TraitFunctions.CritiqueArt.Singleton);
            }

            base.RemoveInteractionByType(ViewObjects.Singleton);
            base.RemoveInteractionByType(ScanWorkOfArt.Singleton);
            #endregion

            base.AddInteraction(CreateFamily.Singleton);
            base.AddInteraction(SetActiveFamily.Singleton);
            base.AddInteraction(EditFamily.Singleton);
            base.AddInteraction(ResetLot.Singleton);
            base.AddInteraction(ShowActiveFamilyInfo.Singleton);           
            base.AddInteraction(StopAdvertisingRoommates.Singleton);
            base.AddInteraction(DeleteFamily.Singleton);
            
            //Comment out from final version
         //   base.AddInteraction(TestInteraction.Singleton);

            //Startup things
            StartUpOrSwitch();


        }

        #region Dispose
        /// <summary>
        /// Merge family and if this is the only object, also delete alarm
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            ApartmentController.ResetLot(this);

            //If no more objects exist, remove alarm
            List<Controller> cList = new List<Controller>(Sims3.Gameplay.Queries.GetObjects<Controller>());
            if (cList == null || (cList != null && cList.Count == 0))
                AlarmManager.Global.RemoveAlarm(this.mBills);


        }
        #endregion Dispose

        #region StartUpOrSwitch
        /// <summary>
        /// Start up things when the game is loaded or a new controller is added
        /// </summary>
        public void StartUpOrSwitch()
        {
            try
            {
                //Only do this part for the active family
                if (this.LotCurrent.Household != null && this.LotCurrent.Household.IsActive)
                {
                   // List<Abstracts.Door> doors = ApartmentController.ReturnLockedDoors(this);

                    //set the active households rent
                    ApartmentFamily activeFamily = this.Families.Find(
                        delegate(ApartmentFamily f) { return f.IsActive == true; });
                    if (activeFamily != null)
                    {
                        if (firstLoad)
                        {
                            firstLoad = false;

                            if (ControllerSettings.alwaysDisableRoommateService)
                            {
                                //Event Hanlders
                                World.OnAddLotPostLoadCallback += new System.EventHandler(World_OnWorldQuitEventHandler);

                                ApartmentController.StopAcceptingRoommates();
                            }
                          //  ApartmentController.LoadActiveHousehold(activeFamily, this);
                        }
                    }
                }

                //Handle rent payment
                DaysOfTheWeek daysOfTheWeek = DaysOfTheWeek.None;
                DaysOfTheWeek[] array = Mailbox.kBillingDaysOfWeek;
                for (int i = 0; i < array.Length; i++)
                {
                    DaysOfTheWeek daysOfTheWeek2 = array[i];
                    daysOfTheWeek |= daysOfTheWeek2;
                }

                if (this.mBills == AlarmHandle.kInvalidHandle)
                    this.mBills = AlarmManager.Global.AddAlarmDay(ControllerSettings.timeOfRent, ControllerSettings.rentDay, new AlarmTimerCallback(RentAlarm), "anisBillAndRentAlarm", AlarmType.NeverPersisted, null);

            }
            catch (System.Exception ex)
            {
                CommonMethods.PrintMessage("bill handling: " + ex.Message);
            }
        }
       
        #endregion StartUpOrSwitch

        #region Events
        /// <summary>
        /// World quitting, remove listeners
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void World_OnWorldQuitEventHandler(object sender, System.EventArgs e)
        {
            ApartmentController.StopAcceptingRoommates();
        }
        #endregion Events

        #region RentAlarm
        /// <summary>
        /// Handle rent payments
        /// </summary>
        public static void RentAlarm()
        {
            try
            {
                List<Controller> cList = new List<Controller>(Sims3.Gameplay.Queries.GetObjects<Controller>());
                if (cList != null && cList.Count > 0)
                {
                    Controller activeController = cList.Find(delegate(Controller c) { return c.LotCurrent.LotId == Household.ActiveHousehold.LotId; });
                    if (activeController != null)
                    {
                        ApartmentFamily af = activeController.Families.Find(delegate(ApartmentFamily f) { return f.IsActive == true; });

                        if (af != null)
                        {
                            if (Household.ActiveHousehold.FamilyFunds >= af.Rent)
                            {
                                Household.ActiveHousehold.SetFamilyFunds(Household.ActiveHousehold.FamilyFunds - af.Rent);
                                CommonMethods.PrintMessage("Rent paid: " + af.Rent);
                            }
                            else
                            {
                                Household.ActiveHousehold.UnpaidBills += af.Rent;
                                CommonMethods.PrintMessage("Not enough money for rent. " + af.Rent + "§ added to unpaid bills.");
                            }
                        }
                    }

                    //CommonMethods.PrintMessage("Set Rent: " + );
                }

            }
            catch (System.Exception ex)
            {
                CommonMethods.PrintMessage("BillsCreted " + ex.Message);
            }
        }
        #endregion RentAlarm
        
        #endregion initialization stuff

        #region Settings

        #endregion Settings
    }
}
