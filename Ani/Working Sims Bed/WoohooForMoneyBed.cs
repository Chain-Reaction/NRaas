using Sims3.Gameplay.Objects.Beds.Mimics;
using Sims3.Gameplay.Actors;
using System.Collections.Generic;
using Sims3.Gameplay.Interactions;
using Sims3.UI;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ObjectComponents;
using Sims3.SimIFace.Enums;
using Sims3.SimIFace.SACS;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Utilities;
using System;
using Sims3.Gameplay.EventSystem;
using System.Text;
using Sims3.Gameplay.Objects.Decorations.Mimics;


namespace Sims3.Gameplay.Objects.Decorations.WorkingSimsBed
{
    #region Bed Controller
    public class WooHooForMoneyBedController : SculptureVaseOrchids
    {
        public override void OnStartup()
        {
            base.OnStartup();
            base.AddInteraction(SetBedInteractions.Singleton);
            base.AddInteraction(RemoveBedInteractions.Singleton);
        }

        #region Set Bed Interactions
        internal sealed class SetBedInteractions : ImmediateInteraction<Sim, WooHooForMoneyBedController>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods

            public override bool Run()
            {
                List<BedDouble> beds = new List<BedDouble>(Sims3.Gameplay.Queries.GetObjects<BedDouble>(base.Target.LotCurrent));
                foreach (BedDouble bed in beds)
                {
                    //Only select beds in the same room as the object
                    if (base.Target.RoomId == bed.RoomId && !WooHooBedList.listWP.ContainsKey(bed.ObjectId))
                    {                       
                        bed.AddInteraction(WoohooForMoneyBed.SelectClient.Singleton);
                        bed.AddInteraction(WoohooForMoneyBed.SelectSoliciter.Singleton);
                        bed.AddInteraction(WoohooForMoneyBed.WoohooForMoney.Singleton);
                        bed.AddInteraction(WoohooForMoneyBed.ListParticipents.Singleton);

                        //Add the bed info into the list                           
                        if (!WooHooBedList.listWP.ContainsKey(bed.ObjectId))
                        {
                            WooHooParticipants wp = new WooHooParticipants();
                            wp.bed = bed;
                            WooHooBedList.listWP.Add(bed.ObjectId, wp);
                        }
                        else
                        {
                            //Update the bed
                            WooHooBedList.listWP[base.Target.ObjectId].bed = bed;
                        }

                    }

                }

                return true;
            }

            // Nested Types
            public sealed class Definition : ImmediateInteractionDefinition<Sim, WooHooForMoneyBedController, WooHooForMoneyBedController.SetBedInteractions>
            {
                // Methods 
                public override string GetInteractionName(Sim a, WooHooForMoneyBedController target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("SetBedInteractions", new object[0]);
                }

                public override bool Test(Sim a, WooHooForMoneyBedController target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }
        #endregion

        #region Remove Bed Interactions
        internal sealed class RemoveBedInteractions : ImmediateInteraction<Sim, WooHooForMoneyBedController>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods

            public override bool Run()
            {
                List<BedDouble> beds = new List<BedDouble>(Sims3.Gameplay.Queries.GetObjects<BedDouble>(base.Target.LotCurrent));
                foreach (BedDouble bed in beds)
                {
                    //Only select beds in the same room as the object
                    if (WooHooBedList.listWP.ContainsKey(bed.ObjectId))
                    {
                        if (base.Target.RoomId == bed.RoomId)
                        {
                            bed.RemoveInteractionByType(WoohooForMoneyBed.SelectClient.Singleton);
                            bed.RemoveInteractionByType(WoohooForMoneyBed.SelectSoliciter.Singleton);
                            bed.RemoveInteractionByType(WoohooForMoneyBed.WoohooForMoney.Singleton);
                            bed.RemoveInteractionByType(WoohooForMoneyBed.ListParticipents.Singleton);

                            //bed.RemoveAllInteractions();
                            //bed.OnStartup();

                            WooHooBedList.listWP.Remove(bed.ObjectId);
                        }
                    }
                }
                return true;

            }

            // Nested Types
            public sealed class Definition : ImmediateInteractionDefinition<Sim, WooHooForMoneyBedController, WooHooForMoneyBedController.RemoveBedInteractions>
            {
                // Methods 
                public override string GetInteractionName(Sim a, WooHooForMoneyBedController target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("RemoveBedInteractions", new object[0]);
                }
                public override bool Test(Sim a, WooHooForMoneyBedController target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }
        #endregion
    }
    #endregion

    #region WooHoo Participants
    /// <summary>
    /// All the info we need to set the participants
    /// </summary>
    public class WooHooParticipants
    {
        public Sim client { get; set; }
        public Sim solicitor { get; set; }
        public Sim activeSim { get; set; }

        public Bed bed { get; set; }
        //public PartData data { get; set; }


        public WooHooParticipants()
        {

        }
    }
    #endregion

    #region Lists
    /// <summary>
    /// A list of all the beds, we have added interactions to
    /// </summary>
    public class WooHooBedList
    {
        public static Dictionary<ObjectGuid, WooHooParticipants> listWP = new Dictionary<ObjectGuid, WooHooParticipants>();
    }
 
    #endregion

    public class WoohooForMoneyBed : BedDouble
    {

        #region Info stuff

        public static void NotOnLot(Sim sim)
        {
            StyledNotification.Show(new StyledNotification.Format(CommonMethods.LocalizeString("SimNotOnLot", new object[0]), StyledNotification.NotificationStyle.kGameMessagePositive));
        }

        public static void DisplayInfo(ObjectGuid id)
        {
            StringBuilder sb = new StringBuilder();

            if (WooHooBedList.listWP[id].client != null)
            {
                sb.Append(CommonMethods.LocalizeString("Client", new object[] { WooHooBedList.listWP[id].client.Name }));
            }
            else
            {
                sb.Append(CommonMethods.LocalizeString("ClientNotSelected", new object[0]));
            }

            if (WooHooBedList.listWP[id].solicitor != null)
            {
                sb.Append(CommonMethods.LocalizeString("Solicitor", new object[] { WooHooBedList.listWP[id].solicitor.Name }));
            }
            else
            {
                sb.Append(CommonMethods.LocalizeString("SolicitorNotSelected", new object[0]));
            }
            StyledNotification.Show(new StyledNotification.Format(sb.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive));
        }

        #endregion

        #region Select Client
        public class SelectClient : ImmediateInteraction<Sim, BedDouble>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                try
                {
                    WooHooBedList.listWP[base.Target.ObjectId].activeSim = base.Actor;
                    WooHooBedList.listWP[base.Target.ObjectId].client = base.GetSelectedObject() as Sim;
                }
                catch (System.Exception ex)
                {
                    base.Actor.ShowTNSIfSelectable(ex.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, base.Actor.ObjectId);
                }
                return true;
            }


            // Nested Types
            private sealed class Definition : ImmediateInteractionDefinition<Sim, BedDouble, SelectClient>
            {
                // Methods
                public override string GetInteractionName(Sim a, BedDouble target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("SelectTheClient", new object[0]);
                }

                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 1;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();

                    foreach (Sim sim in actor.LotCurrent.GetSims())
                    {
                        if (sim.SimDescription.TeenOrAbove)
                        {
                            sims.Add(sim);
                        }
                    }

                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }

                public override bool Test(Sim a, BedDouble target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }
        #endregion

        #region Select Soliciter
        public sealed class SelectSoliciter : ImmediateInteraction<Sim, BedDouble>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                try
                {
                    WooHooBedList.listWP[base.Target.ObjectId].activeSim = base.Actor;
                    WooHooBedList.listWP[base.Target.ObjectId].solicitor = base.GetSelectedObject() as Sim;
                }
                catch (System.Exception ex)
                {
                    base.Actor.ShowTNSIfSelectable(ex.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, base.Actor.ObjectId);
                }
                return true;
            }

            // Nested Types
            public sealed class Definition : ImmediateInteractionDefinition<Sim, BedDouble, SelectSoliciter>
            {
                // Methods
                public override string GetInteractionName(Sim a, BedDouble target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("SelectTheSolicitor", new object[0]);
                }
                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 1;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();

                    foreach (Sim sim in actor.LotCurrent.GetSims())
                    {
                        if (sim.SimDescription.TeenOrAbove)
                        {
                            sims.Add(sim);
                        }
                    }

                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }

                public override bool Test(Sim a, BedDouble target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }
        #endregion

        #region WooHoo For Money
        public sealed class WoohooForMoney : InteractionGameObjectHit<Sim, BedDouble>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                Sim client = WooHooBedList.listWP[base.Target.ObjectId].client;
                Sim solicitor = WooHooBedList.listWP[base.Target.ObjectId].solicitor;

                //Set and get the active and none active sim
                WooHooBedList.listWP[base.Target.ObjectId].activeSim = base.Actor;
                Sim nonActiveSim = WooHooBedList.listWP[base.Target.ObjectId].client;

                //Get the none active sim
                if (WooHooBedList.listWP[base.Target.ObjectId].activeSim == WooHooBedList.listWP[base.Target.ObjectId].solicitor)
                {
                    nonActiveSim = WooHooBedList.listWP[base.Target.ObjectId].client;
                }
                else
                {
                    nonActiveSim = WooHooBedList.listWP[base.Target.ObjectId].solicitor;
                }

                //Check both have been assigned
                if (solicitor != null && client != null)
                {
                    //Check both are still on the same lot
                    if (solicitor.LotCurrent == client.LotCurrent)
                    {
                        if (nonActiveSim == client)
                        {
                            WooHooBedActions.AddWooHooToQue(base.Target, base.Actor, nonActiveSim, WoohooForMoneyBed.WooHooClient.Singleton);
                        }
                        else
                        {
                            WooHooBedActions.AddWooHooToQue(base.Target, base.Actor, nonActiveSim, WoohooForMoneyBed.WooHooSoliciter.Singleton);
                        }
                    }
                    else
                    {
                        WoohooForMoneyBed.NotOnLot(nonActiveSim);
                    }
                }
                else
                {
                    WoohooForMoneyBed.DisplayInfo(base.Target.ObjectId);
                }
                return true;
            }

            // Nested Types
            public sealed class Definition : InteractionDefinition<Sim, BedDouble, WoohooForMoney>
            {
                // Methods
                public override string GetInteractionName(Sim a, BedDouble target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("WooHooForMoney", new object[0]);
                }
                public override bool Test(Sim a, BedDouble target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }
        #endregion

        #region WooHoo Client
        public sealed class WooHooClient : WooHooBedActions.WooHooingForMoney
        {
            // Fields
            public static new readonly InteractionDefinition Singleton = new Definition();

            // Methods

            public override bool Run()
            {
               return base.Run(base.Target, base.Actor);
            }

            // Nested Types
            public new sealed class Definition : InteractionDefinition<Sim, Sim, WooHooClient>
            {
                // Methods       
                public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("WooHooForMoney", new object[0]);
                }
                public void Run(IActor actor, IGameObject target)
                {
                    Sim sim = actor as Sim;
                    Sim sim2 = target as Sim;

                    if ((sim.Conversation != null) && (sim.Conversation != sim2.Conversation))
                    {
                        actor.SocialComponent.LeaveConversation();
                    }
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }
        #endregion

        #region WooHoo Solicitor
        public sealed class WooHooSoliciter : WooHooBedActions.WooHooingForMoney
        {
            // Fields
            public static new readonly InteractionDefinition Singleton = new Definition();

            // Methods            
            public override bool Run()
            {
                return base.Run(base.Actor, base.Target);
            }

            // Nested Types
            public new sealed class Definition : InteractionDefinition<Sim, Sim, WooHooSoliciter>
            {
                // Methods   
                public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("WooHooForMoney", new object[0]);
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }
        #endregion

        #region List Participants
        public sealed class ListParticipents : ImmediateInteraction<Sim, BedDouble>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();


            // Methods
            public override bool Run()
            {
                WooHooBedList.listWP[base.Target.ObjectId].activeSim = base.Actor;
                WoohooForMoneyBed.DisplayInfo(base.Target.ObjectId);

                return true;
            }

            // Nested Types
            public sealed class Definition : ImmediateInteractionDefinition<Sim, BedDouble, ListParticipents>
            {
                // Methods
                public override string GetInteractionName(Sim a, BedDouble target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("ListParticipants", new object[0]);
                }
                public override bool Test(Sim a, BedDouble target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }
        #endregion

        #region Bed Tuning
        public override Bed.Tuning TuningBed
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

    }

}



