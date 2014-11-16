using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Toys;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.SimIFace.SACS;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.InteractionsShared;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Careers;
using Sims3.UI;
using TheKnifeProject;


namespace Sims3.Gameplay.Objects.TheKnifeProject
{

    public class KnifeObject : StuffedAnimal
    {
        #region public properties

        public enum Weapons { Knife = 1, Hammer, Poker, Sword };

        public Sim Player1;
        public Sim Player2;

        //Moodlet times
        int misery = -75;
        int delight = 75;
        int adreanalinerush = 100;

        float miseryTimeout = 10080; //7 days
        float delightTimeout = 2880; //2 days
        float steamTimeout = 1440; //2 days;
        float adrenalineTimeOut = 1440; //1 day

        #endregion
        // Boolean createWeaponBeforeKill = true;


        // Methods

        #region On Startup

        public override void OnStartup()
        {
            base.OnStartup();

            base.AddInteraction(Sim_Stab.Singleton);            
            base.AddInteraction(Sim_Poker.Singleton);
            base.AddInteraction(Sim_Hammer.Singleton);
            base.AddInteraction(Sim_Stab_Sword.Singleton);

            base.AddInventoryInteraction(Sim_Stab.Singleton);
            base.AddInventoryInteraction(Sim_Poker.Singleton);
            base.AddInventoryInteraction(Sim_Hammer.Singleton);
            base.AddInventoryInteraction(Sim_Stab_Sword.Singleton);

        }

        #endregion

        #region Handle Killing

        public void HandleKilling(Weapons weapon, float distance)
        {
            try
            {
                Vector3 originalVictimPosition = Player2.Position;

                Boolean flag = Player1.RouteToDynamicObjectRadius(Player2, distance, null, new Route.RouteOption[0]);

                if (!flag)
                {
                    Player1.ShowTNSIfSelectable(CommonMethods.LocalizeString("FailedToRoute", new object[0]), StyledNotification.NotificationStyle.kSystemMessage, Player1.ObjectId);

                }
                else
                {
                    if (Player1.SimDescription.TeenOrAbove)
                    {
                        //kill only if the sim is still there
                        if (originalVictimPosition == Player2.Position)
                        {
                            switch (weapon)
                            {
                                case Weapons.Hammer:
                                    Player2.Kill(SimDescription.DeathType.Burn);
                                    PlayKillingAnimations("hammer", "BeatWith - Hammer");
                                    break;
                                case Weapons.Poker:
                                    Player2.Kill(SimDescription.DeathType.Burn);
                                    PlayKillingAnimations("hammer", "BeatWith - Poker");
                                    break;

                                case Weapons.Knife:
                                    Player2.Kill(SimDescription.DeathType.Starve);
                                    PlayKillingAnimations("stab", "Exit");
                                    PlayKillingAnimations("stab", "Stab - loop");
                                    break;
                                case Weapons.Sword:
                                    Player2.Kill(SimDescription.DeathType.Starve);
                                    PlayKillingAnimations("stabsword", "ExitSword");
                                    PlayKillingAnimations("stabsword", "Stab - loopSword");
                                    break;
                            }

                            //Set the moodlets
                            SetMoodlets();
                        }
                        else
                        {
                            Player1.ShowTNSIfSelectable(CommonMethods.LocalizeString("VictimHasMoved", new object[0]), StyledNotification.NotificationStyle.kSystemMessage, Player1.ObjectId);
                        }
                    }
                    else
                    {
                        Player1.ShowTNSIfSelectable(CommonMethods.LocalizeString("CantKillChildren", new object[0]), StyledNotification.NotificationStyle.kSystemMessage, Player1.ObjectId);

                    }
                }
            }
            catch (Exception exept)
            {
                Player1.ShowTNSIfSelectable(exept.ToString(), StyledNotification.NotificationStyle.kSystemMessage, Player1.ObjectId);
            }

        }

        /// <summary>
        /// The animation of getting the weapon
        /// </summary>
        public void SetWeapon(Weapons weapon)
        {
            try
            {
                //Set weapon if we need to walk to victim
                float distanec = Player1.GetDistanceToObject(Player2);
                
                if (distanec > 1f)
                {
                    switch (weapon)
                    {
                        case Weapons.Knife:
                            PlayKillingAnimations("stab", "Chop - Start");
                            break;

                        case Weapons.Hammer:
                            PlayKillingAnimations("hammer", "Weapon - Hammer");
                            break;

                        case Weapons.Poker:
                            PlayKillingAnimations("hammer", "Weapon - Poker");
                            break;
                        case Weapons.Sword:
                            PlayKillingAnimations("stabsword", "Chop - StartSword");
                            break;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Player1.ShowTNSIfSelectable("ex: " + ex.Message, StyledNotification.NotificationStyle.kSystemMessage, ObjectGuid.InvalidObjectGuid, Player1.ObjectId);
            }
        }

        public void PlayKillingAnimations(string statemachine, string state)
        {
            try
            {
                Player1.CarryStateMachine = StateMachineClient.Acquire(Player1.Proxy.ObjectId, statemachine);
                Player1.CarryStateMachine.SetActor("x", Player1);
                Player1.CarryStateMachine.EnterState("x", state);
            }

            catch (Exception ex)
            {
                Player1.ShowTNSIfSelectable("ex: " + ex, StyledNotification.NotificationStyle.kSystemMessage, ObjectGuid.InvalidObjectGuid, Player1.ObjectId);
            }

        }

        public void SetMoodlets()
        {
            //Give the killer a good moodlet if evil or mean spirited
            if (Player1.TraitManager.HasElement(TraitNames.Evil) || Player1.TraitManager.HasElement(TraitNames.MeanSpirited))
            {
                Player1.BuffManager.AddElement(BuffNames.LetOffSteam, delight, steamTimeout, Origin.FromWitnessingDeath);
                Player1.BuffManager.AddElement(BuffNames.AdrenalineRush, adreanalinerush, adrenalineTimeOut, Origin.FromGrimReaper);
                Player1.BuffManager.AddElement(BuffNames.FiendishlyDelighted, delight, delightTimeout, Origin.FromWatchingSimSuffer);

            }
            else
            {
                //The killer is goodish, so he feels remorse + a bit of adrenaline rush
                Player1.BuffManager.AddElement(BuffNames.CreepedOut, misery, miseryTimeout, Origin.FromWitnessingDeath);
                Player1.BuffManager.AddElement(BuffNames.Horrified, misery, miseryTimeout, Origin.FromWatchingSimSuffer);
                Player1.BuffManager.AddElement(BuffNames.AdrenalineRush, adreanalinerush, adrenalineTimeOut, Origin.FromGrimReaper);

            }

            //Witness moodlets
            Boolean didTheGoodPeopleSeeIt = false;
            foreach (Sim sim in Player1.LotCurrent.GetSims())
            {
                if (sim != Player1 && sim.RoomId == Player1.RoomId && sim != Player2)
                {
                    //If the witness is evil
                    if (sim.TraitManager.HasElement(TraitNames.Evil) || sim.TraitManager.HasElement(TraitNames.MeanSpirited))
                    {
                        sim.BuffManager.AddElement(BuffNames.FiendishlyDelighted, delight, steamTimeout, Origin.FromWatchingSimSuffer);
                        sim.BuffManager.AddElement(BuffNames.Intrigued, (delight / 4), steamTimeout, Origin.FromWitnessingDeath);

                        //Give a relationship boost 
                        sim.SocialComponent.AddRelationshipUpdate(Player1, CommodityTypes.Friendly, 5f, 5f);

                    }
                    else
                    {   //The goodish witnesses    
                        //Babies and Toddlers can't call the cops, but they can hate you
                        if (sim.SimDescription.ChildOrAbove)
                        {
                            didTheGoodPeopleSeeIt = true;
                        }
                        sim.BuffManager.AddElement(BuffNames.Scared, misery, miseryTimeout, Origin.FromWitnessingDeath);
                        sim.BuffManager.AddElement(BuffNames.Horrified, misery, miseryTimeout, Origin.FromWatchingSimSuffer);
                        sim.BuffManager.AddElement(BuffNames.CreepedOut, misery, miseryTimeout, Origin.FromWitnessingDeath);

                        //Remove relationship
                        sim.SocialComponent.AddRelationshipUpdate(Player1, CommodityTypes.Creepy, -100f, -100f);
                    }
                }
            }

            //If good witnesses saw the killings, the killer is caught
            if (didTheGoodPeopleSeeIt)
            {
                GotCaught();
            }
        }

        #endregion

        #region Get Caught
        /// <summary>
        /// Handels getting caught
        /// </summary>
        public void GotCaught()
        {
            //Set all family members of the victim, to hate the killer
            foreach (var sim in Player2.SimDescription.Household.Sims)
            {
                if (sim != Player1)
                {
                    sim.BuffManager.AddElement(BuffNames.MyLove, misery, miseryTimeout, Origin.FromWitnessingDeath);
                    sim.BuffManager.AddElement(BuffNames.Upset, misery, miseryTimeout, Origin.FromGrimReaper);
                    sim.BuffManager.AddElement(BuffNames.WitnessedBetrayal, misery, miseryTimeout, Origin.FromEnemy);
                }

                //Remove relationship
                sim.SocialComponent.AddRelationshipUpdate(Player1, CommodityTypes.Creepy, -100f, -100f);
            }

            //Only adults can get arrested
            ArrestSuspectSituation.Create(Player1.LotCurrent, Player1);

            //Create aura of evil 
            SimpleAura aura = new SimpleAura(Player1, Criminal.kAuraOfEvilParams, "evilAura");

          

            //Loose job only if not a criminal
            if (Player1.SimDescription.Occupation != null)
            {
                if (((ulong)Player1.SimDescription.Occupation.Guid) != ((ulong)OccupationNames.Criminal))
                {
                    Player1.SimDescription.Occupation.FireSim(false);
                }
            }

        }

        #endregion

        #region Interactions
      

        private sealed class Sim_Stab : Interaction<Sim, KnifeObject>
        {
            // Fields
            public bool FromNeutral = true;
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                base.Target.Player1 = base.Actor;
                base.Target.Player2 = base.GetSelectedObject() as Sim;

                base.Target.SetWeapon(Weapons.Knife);
                base.Target.HandleKilling(Weapons.Knife, 0.8f);

                base.StandardExit();
                return true;
            }

            public override bool RunFromInventory()
            {
                return this.Run();
            }

            // Nested Types
            private class Definition : InteractionDefinition<Sim, KnifeObject, KnifeObject.Sim_Stab>
            {
                // Methods
                public override string GetInteractionName(Sim a, KnifeObject target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("SelectToStab", new object[0]);
                }

                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 1;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();

                    foreach (Sim sim in actor.LotCurrent.GetSims())
                    {
                        if (sim != actor && sim.SimDescription.TeenOrAbove)
                        {
                            sims.Add(sim);
                        }
                    }

                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }


                public override bool Test(Sim a, KnifeObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }

        private sealed class Sim_Hammer : Interaction<Sim, KnifeObject>
        {
            // Fields
            public bool FromNeutral = true;
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                base.Target.Player1 = base.Actor;
                base.Target.Player2 = base.GetSelectedObject() as Sim;

                base.Target.SetWeapon(Weapons.Hammer);
                base.Target.HandleKilling(Weapons.Hammer, 1f);

                base.StandardExit();
                return true;
            }

            public override bool RunFromInventory()
            {
                return this.Run();
            }


            // Nested Types
            private class Definition : InteractionDefinition<Sim, KnifeObject, KnifeObject.Sim_Hammer>
            {
                // Methods
                public override string GetInteractionName(Sim a, KnifeObject target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("SelectToHammer", new object[0]);
                }

                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 1;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();

                    foreach (Sim sim in actor.LotCurrent.GetSims())
                    {
                        if (sim != actor)
                        {
                            sims.Add(sim);
                        }
                    }

                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }


                public override bool Test(Sim a, KnifeObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

        }

        private sealed class Sim_Poker : Interaction<Sim, KnifeObject>
        {
            // Fields
            public bool FromNeutral = true;
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                base.Target.Player1 = base.Actor;
                base.Target.Player2 = base.GetSelectedObject() as Sim;

                base.Target.SetWeapon(Weapons.Poker);
                base.Target.HandleKilling(Weapons.Poker, 1f);

                base.StandardExit();
                return true;
            }

            public override bool RunFromInventory()
            {
                return this.Run();
            }


            // Nested Types
            private class Definition : InteractionDefinition<Sim, KnifeObject, KnifeObject.Sim_Poker>
            {
                // Methods
                public override string GetInteractionName(Sim a, KnifeObject target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("SelectToPoker", new object[0]);
                }

                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 1;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();

                    foreach (Sim sim in actor.LotCurrent.GetSims())
                    {
                        if (sim != actor)
                        {
                            sims.Add(sim);
                        }
                    }

                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }


                public override bool Test(Sim a, KnifeObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

        }

        private sealed class Sim_Stab_Sword : Interaction<Sim, KnifeObject>
        {
            // Fields
            public bool FromNeutral = true;
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                base.Target.Player1 = base.Actor;
                base.Target.Player2 = base.GetSelectedObject() as Sim;

                base.Target.SetWeapon(Weapons.Sword);
                base.Target.HandleKilling(Weapons.Sword, 0.8f);

                base.StandardExit();
                return true;
            }

            public override bool RunFromInventory()
            {
                return this.Run();
            }

            // Nested Types
            private class Definition : InteractionDefinition<Sim, KnifeObject, KnifeObject.Sim_Stab_Sword>
            {
                // Methods
                public override string GetInteractionName(Sim a, KnifeObject target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("SelectToStabSword", new object[0]);
                }

                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 1;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();

                    foreach (Sim sim in actor.LotCurrent.GetSims())
                    {
                        if (sim != actor && sim.SimDescription.TeenOrAbove)
                        {
                            sims.Add(sim);
                        }
                    }

                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }


                public override bool Test(Sim a, KnifeObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }

        #endregion

    }
}



