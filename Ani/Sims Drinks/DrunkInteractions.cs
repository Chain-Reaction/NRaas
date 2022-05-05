using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.CAS;

namespace Alcohol {
    class DrunkInteractions
    {
        #region Do Drunk Interactions
        /// <summary>
        /// Make the sim perform a drunk interaction
        /// </summary>
        /// <param name="sim"></param>
        public static void DoDrunkInteraction(Sim sim)
        {
            if (ActionToSelf(sim))
            {   
                DoInteractionToSelf(sim);
            }
            else
            {
                DoInteractionToOthers(sim);
            }
        }
        #endregion

        #region PayAndConcequences
        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="Actor"></param>
        /// <returns></returns>
        public static void PayAndConsequencs(Sim sim, int pay, bool drink, bool nectar)
        {
            //Pay for the drink
            //If sim is drinking alcohol
            bool getDrunk = true;
            if (sim.IsHoldingAnything())
            {
                if (sim.GetObjectInLeftHand() != null && sim.GetObjectInLeftHand().ObjectInstanceName.ToLower().Contains("cup"))
                    getDrunk = false;
                if (sim.GetObjectInRightHand() != null && sim.GetObjectInRightHand().ObjectInstanceName.ToLower().Contains("cup"))
                    getDrunk = false;
            }

            if (getDrunk)
            {
                //Pay
                if (pay > 0)
                {
                    PayForDrink(sim, pay, nectar);
                }

                //Moodlets and such
                if (drink)
                {
                    Drunk(sim);
                } 
            }
        }
        /// <summary>
        /// Pay the lot owner
        /// </summary>
        /// <param name="sim"></param>
        /// <param name="pay"></param>
        private static void PayForDrink(Sim sim, int pay, bool nectar)
        {
            bool isSimLotOwner = false;
            Household lotOwner = CommonMethods.ReturnLotOwner(sim.LotCurrent);
            if (lotOwner != null && lotOwner != sim.Household)
            {
                lotOwner.ModifyFamilyFunds(pay);
            }

            if (lotOwner != null && lotOwner == sim.Household)
                isSimLotOwner = true;

            if (nectar && sim.LotCurrent.IsCommunityLot && !isSimLotOwner)
                sim.ModifyFunds(-pay);

        }

        private static void Drunk(Sim Actor)
        {
            try
            {
              //  DrunkInteractions.DoDrunkInteraction(Actor);
                DrinkingBuffs.Add(Actor);
            }
            catch (Exception ex)
            {
                Actor.ShowTNSIfSelectable(ex.Message, StyledNotification.NotificationStyle.kGameMessagePositive);
            }

        }
        #endregion

        #region Help methods

        //Performe the random interaction


        private static void PerformeInteractionOnSelf(Sim sim, InteractionDefinition definition)
        {
            InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);
            if (sim.CurrentInteraction != null) {
                priority = sim.CurrentInteraction.GetPriority();
            }

            InteractionInstance instance3 = definition.CreateInstance(sim, sim, priority, false, true);

            sim.InteractionQueue.Add(instance3);

            if (AddMenuItem.ReturnShowNotification())
                StyledNotification.Show(new StyledNotification.Format(sim.Name + "\r" + instance3.GetInteractionName(), StyledNotification.NotificationStyle.kGameMessagePositive));

        }

        private static List<Sim> ReturnSimsOnLot(Sim sim)
        {
            List<Sim> sims = new List<Sim>();

            foreach (Sim s in sim.LotCurrent.GetSims())
            {
                //Don't do interactions with sims at work (bartenders, pianist...)
                if (s.SimDescription.ChildOrAbove && CommonMethods.IsSociable(s))
                {
                    sims.Add(s);
                }
            }

            return sims;
        }

        //If true, drunken interaction will be done to self
        public static Boolean ActionToSelf(Sim sim)
        {
            //Get the number of sims on the lot
            List<Sim> simsOnLot = ReturnSimsOnLot(sim);

            //Do action to self if we are alone on the lot
            if (simsOnLot.Count == 0)
            {
                return true;
            }
            return false;

        }
        #endregion

        #region Drunken Interactions done to others

        public static void DoInteractionToOthers(Sim sim)
        {
            try
            {
                Random r = new Random();

                //Get Sims on lot
                List<Sim> sims = ReturnSimsOnLot(sim);

                if (sims.Count > 0 && sim != null)
                {
                    int maxTries = 20;
                    bool success = false;

                    for (int tries = 0; tries < maxTries && !success; ++tries) {
                        //Get random sim
                        int randomSim = r.Next(sims.Count);
                        Sim targetSim = sims[randomSim];

                        if (sim.SocialComponent != null && targetSim != null)
                        {
                            List<InteractionObjectPair> pairs = new List<InteractionObjectPair>();
                            foreach (var item in sim.SocialComponent.GetAllInteractionsForAutonomy(targetSim))
                            {
                                if (item.CheckIfInteractionValid())
                                {
                                    pairs.Add(item);
                                }
                            }

                            if (pairs.Count < 1) continue;

                            InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);
                            if(sim.CurrentInteraction != null)
                                priority = sim.CurrentInteraction.GetPriority();

                            //IF the sim is not in the active household, raise the priority
                            if (sim.Household != null && !sim.Household.IsActive)
                            {
                                priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);
                            }

                            InteractionInstance instance = pairs[r.Next(pairs.Count)].InteractionDefinition.CreateInstance(targetSim, sim, priority, false, true);
                            if (instance != null)
                            {
                                try
                                {
                                    success = sim.InteractionQueue.Add(instance);

                                    if (success && AddMenuItem.ReturnShowNotification())
                                    {
                                        StyledNotification.Show(new StyledNotification.Format(sim.Name + "\r" + instance.GetInteractionName() + "\r" + targetSim.Name, StyledNotification.NotificationStyle.kGameMessagePositive));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    StyledNotification.Show(new StyledNotification.Format(sim.Name + "\r" + ex.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive));
                                }
                            }
                        }
                    }

                    //Break off if the interaction is not sticking
                    if (!success)
                        DoInteractionToSelf(sim);
                }
            }
            catch (Exception ex)
            {
                sim.ShowTNSIfSelectable(ex.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, sim.ObjectId);

            }
        }
        #endregion

        #region Drunken Interactions Done to Self

        public static void DoInteractionToSelf(Sim sim)
        {
            Random r = new Random();

            int numberOfEvents = Enum.GetNames(typeof(ListOfSelfInteractions)).Length;

            int randomEvent = r.Next(numberOfEvents);

            //  randomEvent = 3;
            StringBuilder sb = new StringBuilder();
            //InteractionPriority priority = sim.CurrentInteraction.GetPriority();
            switch ((ListOfSelfInteractions)randomEvent)
            {
                case ListOfSelfInteractions.KnockedOut:
                    PerformeInteractionOnSelf(sim, PassOut.Singleton);
                    break;
                case ListOfSelfInteractions.Bladderate:
                    PerformeInteractionOnSelf(sim, WetSelf.Singleton);
                    break;
                case ListOfSelfInteractions.GetNaked:
                    PerformeInteractionOnSelf(sim, GetNaked.Singleton);
                    break;
                case ListOfSelfInteractions.TalkToSelf:

                    PerformeInteractionOnSelf(sim, TalkToSelf2.Singleton);
                    break;
                case ListOfSelfInteractions.Sing:
                    sb.Append(SingShowerSong.Singleton.ToString());
                    PerformeInteractionOnSelf(sim, SingShowerSong.Singleton);
                    break;
                case ListOfSelfInteractions.BadBuzz:
                    PerformeInteractionOnSelf(sim, BadBuzz.Singleton);
                    break;
                case ListOfSelfInteractions.DrunkCry:
                    PerformeInteractionOnSelf(sim, DrunkCry.Singleton);
                    break;
                case ListOfSelfInteractions.DrunkHysteria:
                    PerformeInteractionOnSelf(sim, DrunkHysteria.Singleton);
                    break;
                default:
                    break;
            }
        }

        #region GetNaked
        public class GetNaked : Interaction<Sim, Sim>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                //Switch to naked
                if (base.Target.CurrentOutfitCategory != OutfitCategories.Naked)
                {
                    base.Target.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToBathe, OutfitCategories.Naked);
                }
                else
                {
                    //If already naked, then switch to random outfit
                    Random r = new Random();
                    int[] clothingArray = new int[] { 1, 2, 4, 8, 16, 32, 64, 128, 256 };
                    int randomOutfit = r.Next(clothingArray.Length);

                    base.Target.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToBathe, (OutfitCategories)clothingArray[randomOutfit]);
                }

                return true;
            }


            private sealed class Definition : InteractionDefinition<Sim, Sim, DrunkInteractions.GetNaked>
            {
                // Methods
                public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
                {
                    if (a.CurrentOutfitCategory != OutfitCategories.Naked)
                    {
                        return CommonMethods.LocalizeString("GetNaked", new object[0]);
                    }
                    else
                    {
                        return CommonMethods.LocalizeString("ChangeToRandomOutfit", new object[0]);
                    }
                }


                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

        }
        #endregion

        #region Wet Self
        public class WetSelf : Interaction<Sim, Sim>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                base.Actor.Motives.SetValue(CommodityKind.Bladder, -100);
                return true;
            }


            private sealed class Definition : InteractionDefinition<Sim, Sim, DrunkInteractions.WetSelf>
            {
                // Methods
                public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("WetSelf", new object[0]);
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

        }
        #endregion

        #region Pass Out
        public class PassOut : Interaction<Sim, Sim>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                base.Actor.BuffManager.AddElement(BuffNames.KnockedOut, -50, 120, Origin.FromPotion);
                return true;
            }


            private sealed class Definition : InteractionDefinition<Sim, Sim, DrunkInteractions.PassOut>
            {
                // Methods
                public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("PassOut", new object[0]);
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

        }
        #endregion

        #region TalkToSelf
        public class TalkToSelf2 : Sims3.Gameplay.ActorSystems.TraitFunctions.TalkToSelf
        {
            // Fields        
            //  public static readonly ISoloInteractionDefinition Singleton = new Definition();

            public static new readonly InteractionDefinition Singleton = new Definition();

            // Methods           
            public override bool Run()
            {
                return base.Run();
            }

            // Nested Types
            public new sealed class Definition : SoloSimInteractionDefinition<TalkToSelf2>, IWakesLightSleeper
            {
                // Methods
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
                {
                    return Localization.LocalizeString(actor.IsFemale, "Gameplay/Actors/Sim/TalkToSelf:InteractionName", new object[0]);
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }


        #endregion

        #region Sing Shower Song
        public class SingShowerSong : Interaction<Sim, Sim>
        {
            // Fields 
            //  private ReactionBroadcaster mBroadcaster;
            private ObjectSound mSingInShowerSound;
            private static ReactionBroadcasterParams kSingPerformBroadcast = new ReactionBroadcasterParams();

            //   private  mSituation;
            //   private Sim.SwitchOutfitHelper mSwitchOutfitHelper;
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods           
            public override bool Run()
            {
                //The song from the shower
                string name = base.Actor.SimDescription.ChildOrBelow ? "vo_shower_singC" : "vo_shower_singA";
                this.mSingInShowerSound = new ObjectSound(base.Actor.ObjectId, name);
                this.mSingInShowerSound.StartLoop();

                //Animations from singing a solo song
                base.BeginCommodityUpdates();
                base.EnterStateMachine("solo_generic", "Enter", "x");

                base.SetParameter("AnimationName", "a2a_soc_neutral_singFriendly_friendly_neutral_x");

                base.AnimateSim("Play Animation");
                base.AnimateSim("Exit");
                base.EndCommodityUpdates(true);

                this.mSingInShowerSound.Stop();
                return true;
            }



            public new void Cleanup()
            {
                if (this.mSingInShowerSound != null)
                {
                    this.mSingInShowerSound.Stop();
                    this.mSingInShowerSound.Dispose();
                    this.mSingInShowerSound = null;
                }
            }

            public float GetShowerTime()
            {
                float commodityUpdate = base.GetCommodityUpdate(CommodityKind.Hygiene);
                float num2 = (base.Actor.Motives.GetMax(CommodityKind.Hygiene) - base.Actor.Motives.GetValue(CommodityKind.Hygiene)) / commodityUpdate;
                num2 *= 60f;
                return Math.Max(Shower.kMinShowerTime, num2);
            }
            public void EventCallbackStopSinging(StateMachineClient sender, IEvent evt)
            {
                this.Cleanup();
            }

            private void DuringShower(StateMachineClient smc, Interaction<Sim, IShowerable>.LoopData loopData)
            {
                if (base.Actor.Motives.IsMax(CommodityKind.Hygiene))
                {
                    base.Actor.AddExitReason(ExitReason.Finished);
                }
                base.Actor.TrySinging();
                EventTracker.SendEvent(EventTypeId.kEventTakeBath, base.Actor, base.Target);
            }

            public void EventCallbackStartSinging()
            {
                string name = base.Actor.SimDescription.ChildOrBelow ? "vo_shower_singC" : "vo_shower_singA";
                this.mSingInShowerSound = new ObjectSound(base.Actor.ObjectId, name);
                this.mSingInShowerSound.StartLoop();
            }




            // Nested Types
            private sealed class Definition : InteractionDefinition<Sim, Sim, SingShowerSong>, IUsableWhileOnFire, IUsableDuringFire
            {
                // Methods
                public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("Sing", new object[0]);
                }
                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }


        #endregion

        #region Bad Buzz
        public class BadBuzz : Interaction<Sim, Sim>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                bool succeeded = false;
                base.StandardEntry();
                base.EnterStateMachine("AlcoholAnimations", "EnterStateBadBuzz", "x");

                base.BeginCommodityUpdates();
                base.AnimateSim("Electrocute");

                base.EndCommodityUpdates(succeeded);
                base.AnimateSim("Exit");
                base.StandardExit();
                return true;
            }


            private sealed class Definition : InteractionDefinition<Sim, Sim, DrunkInteractions.BadBuzz>
            {
                // Methods
                public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("BadBuzz", new object[0]);
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

        }
        #endregion

        #region Cry
        public class DrunkCry : Interaction<Sim, Sim>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                StateMachineClient smc = StateMachineClient.Acquire(base.Actor, "DeathReactions");
                smc.SetActor("x", base.Actor);
                smc.EnterState("x", "Enter");
                base.BeginCommodityUpdates();
                smc.RequestState("x", "Shocked");
                smc.RequestState("x", "LovedOneLoop");
                smc.RequestState("x", "Exit");
                return true;
            }


            private sealed class Definition : InteractionDefinition<Sim, Sim, DrunkInteractions.DrunkCry>
            {
                // Methods
                public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("DrunkCry", new object[0]);
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

        }
        #endregion

        #region DrunkHysteria
        public class DrunkHysteria : Interaction<Sim, Sim>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                bool succeeded = false;
                base.StandardEntry();
                base.EnterStateMachine("AlcoholAnimations", "EnterStateDrunkenHysteria", "x");

                base.BeginCommodityUpdates();
                base.AnimateSim("DrunkenHysteria");
                base.AnimateSim("Evil");
                base.AnimateSim("Cheer");
                base.AnimateSim("PumpFist");

                base.EndCommodityUpdates(succeeded);
                base.AnimateSim("Exit");
                base.StandardExit();
                return true;
            }


            private sealed class Definition : InteractionDefinition<Sim, Sim, DrunkInteractions.DrunkHysteria>
            {
                // Methods
                public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("DrunkHysteria", new object[0]);
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

        }
        #endregion

        #endregion
    }
}
