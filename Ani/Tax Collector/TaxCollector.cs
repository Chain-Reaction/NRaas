using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Electronics;
using ani_taxcollector;
using System.Collections.Generic;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace.SACS;
using Sims3.UI;
using System.Text;
using System;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Objects.Decorations.Mimics.ani_DonationBox;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Socializing;

namespace Sims3.Gameplay.Objects.Electronics.ani_taxcollector
{
    public class TaxCollector : ComputerCheap
    {
        public TaxInformation info;

        public override void OnCreation()
        {
            base.OnCreation();

            info = new TaxInformation();

        }

        public override void OnStartup()
        {
            base.OnStartup();

            //  base.AddInteraction(TestInteraction.Singleton);
            base.AddInteraction(CollectTax.Singleton);
            base.AddInteraction(PayMyTax.Singleton);
            // base.AddInteraction(RouteAndWorkOnComputer.Singleton);
            base.AddInteraction(TransferFundsCity.Singleton);
            base.AddInteraction(TransferFundsHouseHold.Singleton);
            base.AddInteraction(MakeAPayment.Singleton);
            base.AddInteraction(MakeWithdrawl.Singleton);

            //Setting options             
            base.AddInteraction(SetName.Singleton);
            base.AddInteraction(ShowInfo.Singleton);
            base.AddInteraction(ModifyFunds.Singleton);
            base.AddInteraction(SetMultiplier.Singleton);
            base.AddInteraction(CreateTaxableList.Singleton);

        }

        public override Tooltip CreateTooltip(Vector2 mousePosition, WindowBase mousedOverWindow, ref Vector2 tooltipPosition)
        {
            return new SimpleTextTooltip(this.info.Name + ": " + this.info.Funds + "§");
        }

        #region Computer methods

        //internal bool StartComputingX(CollectTax instance, SurfaceHeight height, bool turnOn)
        //{ 
        //    if (instance.ComputerInteractionOrigin == Computer.InteractionOrigin.Inventory)
        //    {
        //        instance.DoLaptopCleanup = true;
        //    }
        //    if (instance.ComputerInteractionOrigin == Computer.InteractionOrigin.None)
        //    {
        //        instance.ComputerInteractionOrigin = Computer.InteractionOrigin.World;
        //    }
        //    if (this.StartWorkingWithComputerX(instance, height))
        //    {
        //        if (this is ComputerCheap)
        //        {
        //            instance.LoopSound = new ObjectSound(base.ObjectId, "comp_run_cheap_lp");
        //        }
        //        else
        //        {
        //            if (this is ComputerExpensive)
        //            {
        //                instance.LoopSound = new ObjectSound(base.ObjectId, "comp_run_exp_lp");
        //            }
        //            else
        //            {
        //                instance.LoopSound = new ObjectSound(base.ObjectId, "comp_ltop_run_lp");
        //            }
        //        }
        //        if (turnOn)
        //        {
        //            instance.AddOneShotScriptEventHandler(100u, new SacsEventHandler(this.EventCallbackTurnOnLight));
        //            bool flag = base.BoobyTrapComponent != null && base.BoobyTrapComponent.CanTriggerTrap(instance.Actor.SimDescription);
        //            instance.SetParameter("isBoobyTrapped", flag);
        //            if (flag)
        //            {
        //                this.StartVideo(Computer.VideoType.Prank);
        //                if (instance.Actor.IsNPC)
        //                {
        //                    instance.SetPriority(InteractionPriorityLevel.NonCriticalNPCBehavior);
        //                }
        //                else
        //                {
        //                    if (instance.GetPriority().Level >= InteractionPriorityLevel.High)
        //                    {
        //                        instance.SetPriority(InteractionPriorityLevel.UserDirected);
        //                    }
        //                }
        //            }
        //            instance.AnimateSim("TurnOn");
        //            instance.LoopSound.StartLoop();
        //            if (flag)
        //            {
        //                this.TriggerTrap(instance.Actor);
        //            }
        //        }
        //        instance.Actor.LookAtManager.DisableLookAts();
        //        if (instance.Actor.HasTrait(TraitNames.AntiTV) && instance.HasCommodityUpdate(CommodityKind.Fun))
        //        {
        //            instance.BeginCommodityUpdate(CommodityKind.Fun, 0f);
        //        }
        //        return true;
        //    }
        //    CommonMethodsTaxCollector.PrintMessage("Returning false in start computing");
        //    return false;
        //}

        //internal bool StartWorkingWithComputerX(Interaction<Sim, TaxCollector> instance, SurfaceHeight height)
        //{
        //    CommonMethodsTaxCollector.PrintMessage("Working x");
        //    instance.CancellableByPlayer = true;
        //    if (!this.CheckForChairX(instance.Actor, false))
        //    {
        //        CommonMethodsTaxCollector.PrintMessage("1");
        //        return false;
        //    }
        //    if (!this.Line.WaitForTurn(instance, SimQueue.WaitBehavior.NeverWait, ExitReason.Default, 0f))
        //    {
        //        CommonMethodsTaxCollector.PrintMessage("2");
        //        return false;
        //    }
        //    ISurface surface = base.Parent as ISurface;
        //    if (surface != null)
        //    {
        //        SurfaceSlot surfaceSlotFromContainedObject = surface.Surface.GetSurfaceSlotFromContainedObject(this);
        //        if (surfaceSlotFromContainedObject != null)
        //        {
        //            height = surfaceSlotFromContainedObject.Height;
        //        }
        //    }
        //    instance.AcquireStateMachine("computer");
        //    instance.UseAutoParameter(TraitNames.Clumsy);
        //    instance.SetActorAndEnter("x", instance.Actor, "Enter");
        //    instance.SetActor("computer", this);
        //    instance.SetActor("surface", this);
        //    instance.SetActor("chair", instance.Actor.Posture.Container);
        //    instance.SetActor("barstool", instance.Actor.Posture.Container);
        //    instance.SetActor("desk", base.Parent);
        //    instance.SetActor("counter", base.Parent);
        //    instance.SetParameter("height", height);
        //    if (this is ComputerLaptop)
        //    {
        //        instance.SetParameter("isLaptop", true);
        //        base.SetGeometryState("Laptop&Mouse");
        //    }
        //    else
        //    {
        //        instance.SetParameter("isLaptop", false);
        //    }
        //    if (instance.Actor.TraitManager.HasElement(TraitNames.AntiTV) && !(instance is Computer.Sabotage))
        //    {
        //        instance.Actor.BuffManager.AddElementPaused(BuffNames.AntiTV, Origin.FromComputer);
        //    }
        //    return true;
        //}

        //public bool CheckForChairX(Sim Actor, bool allowEmpty)
        //{
        //    ISurface surface = base.Parent as ISurface;
        //    if (surface != null)
        //    {
        //        ISittable sittable = Actor.Posture.Container as ISittable;
        //        SurfacePair surface2 = surface.Surface;
        //        SurfaceSlot surfaceSlotFromContainedObject = surface2.GetSurfaceSlotFromContainedObject(this);
        //        ISittable[] chairs = surface2.GetChairs(surfaceSlotFromContainedObject);

        //        CommonMethodsTaxCollector.PrintMessage("Chairs found: " + chairs.Length);

        //        for (int i = 0; i < chairs.Length; i++)
        //        {
        //            ISittable sittable2 = chairs[i];
        //            if (sittable2 != null && (allowEmpty || sittable == sittable2) && this.IsObjectInFrontOfMe(sittable2 as IGameObject))
        //            {
        //                return true;
        //            }
        //            if (sittable2 != null)
        //                CommonMethodsTaxCollector.PrintMessage("Sittable 2 != null");
        //            if (sittable == sittable2)
        //                CommonMethodsTaxCollector.PrintMessage("Sittable 2 = sittable");
        //            if (this.IsObjectInFrontOfMe(sittable2 as IGameObject))
        //                CommonMethodsTaxCollector.PrintMessage("Front of me");
        //        }
        //    }
        //    else
        //    {
        //        CommonMethodsTaxCollector.PrintMessage("Surface null");
        //    }

        //    return false;
        //}

        //internal void StopComputing(CollectTax instance, Computer.StopComputingAction stopAction, bool forceBreak)
        //{
        //    bool flag = false;
        //    if (this is ComputerLaptop && this.NextInteractionOnSameComputer(instance.Actor) && !forceBreak)
        //    {
        //        InteractionInstance nextInteraction = instance.Actor.InteractionQueue.GetNextInteraction();
        //        nextInteraction.MustRun = true;
        //        nextInteraction.CancellableByPlayer = false;
        //        Computer.ComputerInteraction computerInteraction = nextInteraction as Computer.ComputerInteraction;
        //        if (computerInteraction != null)
        //        {
        //            computerInteraction.ComputerInteractionOrigin = instance.ComputerInteractionOrigin;
        //        }
        //        instance.DoLaptopCleanup = false;
        //        flag = true;
        //    }
        //    instance.Actor.LookAtManager.EnableLookAts();
        //    if (stopAction == Computer.StopComputingAction.None)
        //    {
        //        instance.mCurrentStateMachine.RequestState(true, "x", "Exit", DriverRequestFlags.kInterrupt);
        //        return;
        //    }
        //    if (stopAction == Computer.StopComputingAction.TurnOff)
        //    {
        //        instance.SetParameter("inInventory", instance.ComputerInteractionOrigin == Computer.InteractionOrigin.Inventory && !flag);
        //        instance.mCurrentStateMachine.RequestState(true, "x", "TurnOff", DriverRequestFlags.kInterrupt);
        //        instance.Target.StopVideo();
        //        if (this is ComputerLaptop)
        //        {
        //            base.SetGeometryState("LaptopOnly");
        //        }
        //        this.TurnOffLightEffect();
        //        instance.LoopSound.Dispose();
        //        instance.LoopSound = null;
        //        instance.AnimateSim("Exit");
        //        RepairableComponent repairable = base.Repairable;
        //        if (forceBreak)
        //        {
        //            repairable.BreakObject(instance.Actor, false);
        //            this.TurnOnBrokenEffect();
        //        }
        //        else
        //        {
        //            if (repairable.UpdateBreakage(instance.Actor))
        //            {
        //                this.TurnOnBrokenEffect();
        //            }
        //        }
        //        if (this is ComputerLaptop && !flag && (instance.ComputerInteractionOrigin == Computer.InteractionOrigin.Inventory || (instance.Target.GetOwnerLot() == instance.Actor.LotHome && instance.Actor.LotCurrent != instance.Actor.LotHome)))
        //        {
        //            instance.Actor.Inventory.TryToAdd(this);
        //            base.UnParent();
        //            this.RemoveFromWorld();
        //            base.ResetBindPose();
        //        }
        //    }
        //    instance.Actor.BuffManager.UnpauseBuff(BuffNames.AntiTV);
        //    if (instance.Actor.HasTrait(TraitNames.AntiTV))
        //    {
        //        instance.Actor.PlayReaction(ReactionTypes.Angry, this, ReactionSpeed.AfterInteraction);
        //    }
        //    if (instance.InstanceActor.HasExitReason(ExitReason.PriorityOverride))
        //    {
        //        instance.InstanceActor.RouteAway(Computer.kMinDistanceToMoveAwayWhenFinished, Computer.kMaxDistanceToMoveAwayWhenFinished, true, new InteractionPriority(InteractionPriorityLevel.Zero), false, true, true, RouteDistancePreference.NoPreference);
        //    }
        //}

        #endregion

        #region Interactions

        class TestInteraction : ImmediateInteraction<Sim, TaxCollector>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, TaxCollector, TestInteraction>
            {
                public override bool Test(Sim a, TaxCollector target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override string GetInteractionName(Sim a, TaxCollector target, InteractionObjectPair interaction)
                {
                    //TODO: Luo localisaatio
                    return "Test Interaction";
                }
            }
            public const string sLocalizationKey = "Gameplay/Objects/Electronics/Phone/ChangeRingtone";
            public static InteractionDefinition Singleton = new TestInteraction.Definition();
            public static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Electronics/Phone/ChangeRingtone:" + name, parameters);
            }
            public override bool Run()
            {
                try
                {
                    CommonMethodsTaxCollector.PrintMessage("TEST: " + CommonMethodsTaxCollector.CalculateTax(Actor.Household, this.Target.info.Multiplier));
                }
                catch (Exception ex)
                {
                    CommonMethodsTaxCollector.PrintMessage(ex.Message);
                }

                return true;
            }
        }

        public class RouteAndWorkOnComputer : Computer.ComputerInteraction
        {
            public class Definition : InteractionDefinition<Sim, TaxCollector, RouteAndWorkOnComputer>
            {
                public override string GetInteractionName(Sim a, TaxCollector target, InteractionObjectPair interaction)
                {
                    return "Route and work on computer";
                }
                public override bool Test(Sim a, TaxCollector target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.IsComputerUsable(a, true, false, isAutonomous);
                }
            }
            public static InteractionDefinition Singleton = new RouteAndWorkOnComputer.Definition();
            public override bool Run()
            {
                base.StandardEntry();
                if (!this.Target.StartComputing(this, SurfaceHeight.Table, true))
                {
                    CommonMethodsTaxCollector.PrintMessage("failing");
                    base.StandardExit();
                    return false;
                }
                this.Target.StartVideo(Computer.VideoType.Browse);
                base.BeginCommodityUpdates();
                base.AnimateSim("GenericTyping");
                bool flag = base.DoLoop(ExitReason.Default);
                base.EndCommodityUpdates(flag);
                this.Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                base.StandardExit();
                return flag;
            }
        }

        //Tax Menu
        class PayMyTax : ImmediateInteraction<Sim, TaxCollector>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, TaxCollector, PayMyTax>
            {
                public override bool Test(Sim a, TaxCollector target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuAni, new object[0]),
				    CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuTaxPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, TaxCollector target, InteractionObjectPair interaction)
                {
                    return CommonMethodsTaxCollector.LocalizeString("PayActiveHouseholdTax", new object[0]);
                }
            }
            public const string sLocalizationKey = "Gameplay/Objects/Electronics/Phone/ChangeRingtone";
            public static InteractionDefinition Singleton = new PayMyTax.Definition();
            public static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Electronics/Phone/ChangeRingtone:" + name, parameters);
            }
            public override bool Run()
            {
                try
                {
                    int tax = 0;
                    string s = CommonMethodsTaxCollector.ShowDialogueNumbersOnly(CommonMethodsTaxCollector.LocalizeString("PayActiveHouseholdTax", new object[0]), string.Empty, CommonMethodsTaxCollector.CalculateTax(base.Actor.Household, this.Target.info.Multiplier).ToString());

                    if (int.TryParse(s, out tax) && tax > 0)
                    {
                        if (base.Actor.Household.FamilyFunds >= tax)
                        {
                            base.Actor.Household.SetFamilyFunds(base.Actor.Household.FamilyFunds - tax);
                            base.Target.info.Funds += tax;
                        }
                        else
                        {
                            CommonMethodsTaxCollector.PrintMessage(CommonMethodsTaxCollector.LocalizeString("NotEnoughFunds", new object[0]));
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommonMethodsTaxCollector.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class CollectTax : Computer.GenericComputerInteractionBase
        {
            public class Definition : InteractionDefinition<Sim, TaxCollector, CollectTax>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuAni, new object[0]),
				CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuTaxPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, TaxCollector target, InteractionObjectPair interaction)
                {
                    return CommonMethodsTaxCollector.LocalizeString("CollectTax", new object[0]);
                }

                public override bool Test(Sim a, TaxCollector target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.IsComputerUsable(a, false, false, isAutonomous);
                }
            }
            public static InteractionDefinition Singleton = new CollectTax.Definition();


            //Älä deletoi
            //public override bool Run()
            //{
            //    try
            //    {
            //        #region Pay The Tax
            ////        if (CommonMethodsTaxCollector.ShowConfirmationDialog("Collect Tax?"))
            ////        {
            ////            int totalTax = 0;

            ////            StringBuilder sb = new StringBuilder();
            ////            sb.Append(CommonMethodsTaxCollector.LocalizeString("TaxInformation", new object[0]));
            ////            sb.Append("\n");

            ////            foreach (var item in base.Target.info.TaxableHouseholds)
            ////            {
            ////                int tax = CommonMethodsTaxCollector.CalculateTax(item);
            ////                if (item.FamilyFunds >= tax)
            ////                {
            ////                    totalTax += tax;
            ////                    item.SetFamilyFunds(item.FamilyFunds - tax);
            ////                    sb.Append(item.Name + ": " + tax);
            ////                    sb.Append("\n");
            ////                }
            ////                else
            ////                {
            ////                    sb.Append(CommonMethodsTaxCollector.LocalizeString("CantAffordPayment", new object[] { item.Name, tax }));
            ////                    // sb.Append(item.Name + " couldn't afford to pay: " + tax);
            ////                    sb.Append("\n");
            ////                }

            ////            }

            ////            base.Target.info.Funds += totalTax;

            ////            sb.Append(CommonMethodsTaxCollector.LocalizeString("TotalTaxCollected", new object[] { totalTax }));
            ////            sb.Append("\n");
            ////            sb.Append(CommonMethodsTaxCollector.LocalizeString("FundsInTaxCollector", new object[] { base.Target.info.Name, base.Target.info.Funds }));

            ////            CommonMethodsTaxCollector.PrintMessage(sb.ToString());
            ////        }
            //        #endregion Pay The Tax

            //    }
            //    catch (Exception ex)
            //    {
            //        CommonMethodsTaxCollector.PrintMessage(ex.Message);
            //    }

            //    return true;
            //}

            public override void Loop()
            {
                try
                {
                    #region Pay The Tax
                    TaxCollector tx = this.Target as TaxCollector;
                    if (tx != null)
                    {
                        if (CommonMethodsTaxCollector.ShowConfirmationDialog("Collect Tax?"))
                        {
                            int totalTax = 0;

                            StringBuilder sb = new StringBuilder();
                            sb.Append(CommonMethodsTaxCollector.LocalizeString("TaxInformation", new object[0]));
                            sb.Append("\n");

                            foreach (var item in tx.info.TaxableHouseholds)
                            {
                                int tax = CommonMethodsTaxCollector.CalculateTax(item, tx.info.Multiplier);
                                if (item.FamilyFunds >= tax)
                                {
                                    totalTax += tax;
                                    item.SetFamilyFunds(item.FamilyFunds - tax);
                                    sb.Append(item.Name + ": " + tax);
                                    sb.Append("\n");
                                }
                                else
                                {
                                    sb.Append(CommonMethodsTaxCollector.LocalizeString("CantAffordPayment", new object[] { item.Name, tax }));
                                    // sb.Append(item.Name + " couldn't afford to pay: " + tax);
                                    sb.Append("\n");
                                }

                            }

                            tx.info.Funds += totalTax;

                            sb.Append(CommonMethodsTaxCollector.LocalizeString("TotalTaxCollected", new object[] { totalTax }));
                            sb.Append("\n");
                            sb.Append(CommonMethodsTaxCollector.LocalizeString("FundsInTaxCollector", new object[] { tx.info.Name, tx.info.Funds }));

                            CommonMethodsTaxCollector.PrintMessage(sb.ToString());
                        }
                    }
                    else
                        CommonMethodsTaxCollector.PrintMessage("Tax collector null");
                    #endregion Pay The Tax

                }
                catch (Exception ex)
                {
                    CommonMethodsTaxCollector.PrintMessage(ex.Message);
                }
                finally
                {
                    this.Actor.AddExitReason(ExitReason.Finished);
                }
            }

            public override Computer.VideoType VideoToPlay()
            {
                return Computer.VideoType.Hacking;
            }
        }


        //Money Transferes
        class MakeAPayment : ImmediateInteraction<Sim, TaxCollector>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, TaxCollector, MakeAPayment>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuAni, new object[0]),
				CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuWithdrawalPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, TaxCollector target, InteractionObjectPair interaction)
                {
                    return CommonMethodsTaxCollector.LocalizeString("MakeAPayment", new object[0]);
                }

                public override bool Test(Sim a, TaxCollector target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.IsComputerUsable(a, false, false, isAutonomous);
                }
            }

            public static InteractionDefinition Singleton = new MakeAPayment.Definition();

            public override bool Run()
            {
                try
                {
                    int sum;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(CommonMethodsTaxCollector.LocalizeString("MakeAPayment", new object[0]));
                    sb.Append("\n");
                    sb.Append(CommonMethodsTaxCollector.LocalizeString("FundsInTaxCollector", new object[] { base.Target.info.Name, base.Target.info.Funds }));


                    int.TryParse(CommonMethodsTaxCollector.ShowDialogueNumbersOnly(CommonMethodsTaxCollector.LocalizeString("MakeAPayment", new object[0]),
                        sb.ToString(), "0"), out sum);

                    if (sum > 0 && sum <= base.Actor.FamilyFunds)
                    {
                        base.Actor.Household.SetFamilyFunds(base.Actor.FamilyFunds - sum);
                        base.Target.info.Funds += sum;

                        CommonMethodsTaxCollector.PrintMessage(CommonMethodsTaxCollector.LocalizeString("FundsInTaxCollector", new object[] { base.Target.info.Name, base.Target.info.Funds }));
                    }

                }
                catch (Exception ex)
                {
                    CommonMethodsTaxCollector.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class MakeWithdrawl : ImmediateInteraction<Sim, TaxCollector>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, TaxCollector, MakeWithdrawl>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuAni, new object[0]),
				CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuWithdrawalPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, TaxCollector target, InteractionObjectPair interaction)
                {
                    return CommonMethodsTaxCollector.LocalizeString("MakeWithdrawl", new object[0]);
                }
                public override bool Test(Sim a, TaxCollector target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.IsComputerUsable(a, false, false, isAutonomous);
                }
            }

            public static InteractionDefinition Singleton = new MakeWithdrawl.Definition();

            public override bool Run()
            {
                try
                {
                    int withdrawal;
                    int.TryParse(CommonMethodsTaxCollector.ShowDialogueNumbersOnly(CommonMethodsTaxCollector.LocalizeString("MakeWithdrawl", new object[0]),
                        CommonMethodsTaxCollector.LocalizeString("FundsInTaxCollector", new object[] { base.Target.info.Name, base.Target.info.Funds }),
                        "0"), out withdrawal);

                    if (withdrawal > 0 && withdrawal <= base.Target.info.Funds)
                    {
                        //Add withdrawal to family funds 
                        base.Actor.ModifyFunds(withdrawal);
                        base.Target.info.Funds -= withdrawal;

                        CommonMethodsTaxCollector.PrintMessage(CommonMethodsTaxCollector.LocalizeString("FundsInTaxCollector", new object[] { base.Target.info.Name, base.Target.info.Funds }));
                    }

                }
                catch (Exception ex)
                {
                    CommonMethodsTaxCollector.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class TransferFundsCity : Computer.GenericComputerInteractionBase
        {
            public class Definition : InteractionDefinition<Sim, TaxCollector, TransferFundsCity>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuAni, new object[0]),
				CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuWithdrawalPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, TaxCollector target, InteractionObjectPair interaction)
                {
                    return CommonMethodsTaxCollector.LocalizeString("TransferToObject", new object[0]);
                }

                public override bool Test(Sim a, TaxCollector target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.IsComputerUsable(a, false, false, isAutonomous);
                }
            }

            public static InteractionDefinition Singleton = new TransferFundsCity.Definition();

            public override void Loop()
            {
                try
                {
                    TaxCollector tx = this.Target as TaxCollector;
                    if (tx != null)
                    {
                        //Collect all objects 
                        List<DonationBox> dList = new List<DonationBox>(Sims3.Gameplay.Queries.GetObjects<DonationBox>());
                        List<TaxCollector> tList = new List<TaxCollector>(Sims3.Gameplay.Queries.GetObjects<TaxCollector>());
                        List<GameObject> list = new List<GameObject>();
                        foreach (TaxCollector t in tList)
                        {
                            if (t.ObjectId != base.Target.ObjectId)
                                list.Add(t);
                        }

                        foreach (DonationBox d in dList)
                        {
                            list.Add(d);
                        }

                        GameObject selected = CommonMethodsTaxCollector.ReturnTaxCollector(tx, list);

                        if (selected != null)
                        {
                            int sum;

                            StringBuilder sb = new StringBuilder();

                            sb.Append(CommonMethodsTaxCollector.LocalizeString("TransferFrom", new object[] { tx.info.Name, tx.info.Funds }));

                            if (selected.GetType() == typeof(TaxCollector))
                            {
                                sb.Append(CommonMethodsTaxCollector.LocalizeString("TransferTo", new object[] { 
                            (selected as TaxCollector).info.Name, 
                            (selected as TaxCollector).info.Funds }));
                            }
                            else if (selected.GetType() == typeof(DonationBox))
                            {
                                sb.Append(CommonMethodsTaxCollector.LocalizeString("TransferTo", new object[] { 
                            (selected as DonationBox).info.Name, 
                            (selected as DonationBox).info.Funds }));
                            }

                            int.TryParse(CommonMethodsTaxCollector.ShowDialogueNumbersOnly(CommonMethodsTaxCollector.LocalizeString("TransferToObject", new object[0]),
                                CommonMethodsTaxCollector.LocalizeString("Funds", new object[] { tx.info.Funds }), "0"), out sum);

                            if (sum > 0 && sum <= tx.info.Funds)
                            {
                                if (selected.GetType() == typeof(TaxCollector))
                                    (selected as TaxCollector).info.Funds += sum;
                                else if (selected.GetType() == typeof(DonationBox))
                                    (selected as DonationBox).info.Funds += sum;

                                tx.info.Funds -= sum;

                                sb = new StringBuilder();

                                sb.Append(CommonMethodsTaxCollector.LocalizeString("TransferSuccesfull", new object[] { 
                             sum,
                            tx.info.Name, 
                            tx.info.Funds }));
                                sb.Append("\n");
                                sb.Append("\n");


                                if (selected.GetType() == typeof(TaxCollector))
                                {
                                    sb.Append(CommonMethodsTaxCollector.LocalizeString("TransferDestination", new object[] { 
                            (selected as TaxCollector).info.Name, 
                            (selected as TaxCollector).info.Funds }));

                                }
                                else if (selected.GetType() == typeof(DonationBox))
                                {
                                    sb.Append(CommonMethodsTaxCollector.LocalizeString("TransferDestination", new object[] { 
                            (selected as DonationBox).info.Name, 
                            (selected as DonationBox).info.Funds }));

                                }

                                CommonMethodsTaxCollector.PrintMessage(sb.ToString());
                            }
                        }
                    }
                    else
                    {
                        CommonMethodsTaxCollector.PrintMessage("TaxCollector is null");
                    }

                }
                catch (Exception ex)
                {
                    CommonMethodsTaxCollector.PrintMessage(ex.Message);
                }
                finally
                {
                    this.Actor.AddExitReason(ExitReason.Finished);
                }

            }


            public override VideoType VideoToPlay()
            {
                return VideoType.Hacking;
            }
        }

        class TransferFundsHouseHold : Computer.GenericComputerInteractionBase
        {
            public class Definition : InteractionDefinition<Sim, TaxCollector, TransferFundsHouseHold>
            {
                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 1;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();

                    foreach (Household h in Household.sHouseholdList)
                    {
                        if (h.LotHome != null && h.LotHome.InWorld)
                            if (h.Sims != null && h.Sims.Count > 0)
                            {
                                foreach (Sim s in h.Sims)
                                {
                                    if (s.SimDescription != null && s.SimDescription.YoungAdultOrAbove)
                                    {
                                        sims.Add(s);
                                        break;
                                    }
                                }
                              
                            }
                    }
                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuAni, new object[0]),
				CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuWithdrawalPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, TaxCollector target, InteractionObjectPair interaction)
                {
                    return CommonMethodsTaxCollector.LocalizeString("TransferToHousehold", new object[0]);
                }
                public override bool Test(Sim a, TaxCollector target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.IsComputerUsable(a, false, false, isAutonomous);
                }
            }

            public static InteractionDefinition Singleton = new TransferFundsHouseHold.Definition();

            public override void  Loop()
            {
                try
                {
                    TaxCollector tx = this.Target as TaxCollector;
                    if (tx != null)
                    {
                        List<Sim> selectedSims = new List<Sim>();
                        List<object> selectedObjects = base.SelectedObjects;
                        if (selectedObjects != null)
                        {
                            foreach (object obj2 in selectedObjects)
                            {
                                selectedSims.Add(obj2 as Sim);
                            }
                        }

                        //Transfere the money
                        if (selectedSims.Count > 0)
                        {
                            int withdrawal;
                            int.TryParse(CommonMethodsTaxCollector.ShowDialogueNumbersOnly(CommonMethodsTaxCollector.LocalizeString("TransferToHousehold", new object[0]),
                                CommonMethodsTaxCollector.LocalizeString("Funds", new object[] { tx.info.Funds }), "0"),
                                out withdrawal);

                            //Can't be more than total funds 
                            if (withdrawal <= tx.info.Funds)
                            {
                                selectedSims[0].Household.ModifyFamilyFunds(withdrawal);
                                tx.info.Funds -= withdrawal;

                                CommonMethodsTaxCollector.PrintMessage(CommonMethodsTaxCollector.LocalizeString("FundsInTaxCollector", new object[] { tx.info.Name, tx.info.Funds }));
                            }
                        }
                    }
                    else
                    {
                        CommonMethodsTaxCollector.PrintMessage("TaxCollector is null");
                    }
                }
                catch (Exception ex)
                {
                    CommonMethodsTaxCollector.PrintMessage(ex.Message);
                }
                finally
                {
                    this.Actor.AddExitReason(ExitReason.Finished);
                }
            }


            public override VideoType VideoToPlay()
            {
                return VideoType.Hacking;
            }
        }


        //Settings
        class SetName : ImmediateInteraction<Sim, TaxCollector>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, TaxCollector, SetName>
            {
                public override bool Test(Sim a, TaxCollector target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                // Methods  
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuAni, new object[0]),
				CommonMethodsTaxCollector.LocalizeString("SettingsMenu", new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, TaxCollector target, InteractionObjectPair interaction)
                {
                    //TODO: Luo localisaatio
                    return CommonMethodsTaxCollector.LocalizeString("SetName", new object[0]);
                }
            }
            public static InteractionDefinition Singleton = new SetName.Definition();

            public override bool Run()
            {
                try
                {
                    string name = CommonMethodsTaxCollector.ShowDialogue(CommonMethodsTaxCollector.LocalizeString("SetName", new object[0]), "#Name", base.Target.info.Name);
                    if (!string.IsNullOrEmpty(name))
                        base.Target.info.Name = name;
                }
                catch (Exception ex)
                {
                    CommonMethodsTaxCollector.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class ShowInfo : ImmediateInteraction<Sim, TaxCollector>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, TaxCollector, ShowInfo>
            {
                public override bool Test(Sim a, TaxCollector target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                // Methods  
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuAni, new object[0]),
				CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuSettingsPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, TaxCollector target, InteractionObjectPair interaction)
                {
                    //TODO: Luo localisaatio
                    return CommonMethodsTaxCollector.LocalizeString("ShowInfo", new object[0]);
                }
            }
            public static InteractionDefinition Singleton = new ShowInfo.Definition();

            public override bool Run()
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(CommonMethodsTaxCollector.LocalizeString("TaxInformation", new object[] { base.Target.info.Name }));
                    sb.Append("\n");

                    foreach (var item in base.Target.info.TaxableHouseholds)
                    {
                        sb.Append(item.Name + " " + CommonMethodsTaxCollector.CalculateTax(item, this.Target.info.Multiplier));
                        sb.Append("\n");

                    }
                    sb.Append(CommonMethodsTaxCollector.LocalizeString("Funds", new object[] { base.Target.info.Funds }));

                    CommonMethodsTaxCollector.PrintMessage(sb.ToString());
                }
                catch (Exception ex)
                {
                    CommonMethodsTaxCollector.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class CreateTaxableList : ImmediateInteraction<Sim, TaxCollector>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, TaxCollector, CreateTaxableList>
            {
                public override bool Test(Sim a, TaxCollector target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                // Methods  
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuAni, new object[0]),
				CommonMethodsTaxCollector.LocalizeString("SettingsMenu", new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, TaxCollector target, InteractionObjectPair interaction)
                {
                    //TODO: Luo localisaatio
                    return CommonMethodsTaxCollector.LocalizeString("EditTaxableList", new object[0]);
                }
            }
            public static InteractionDefinition Singleton = new CreateTaxableList.Definition();

            public override bool Run()
            {
                try
                {
                    List<Household> taxableHouseholds = new List<Household>();

                    foreach (var t in base.Target.info.TaxableHouseholds)
                    {
                        taxableHouseholds.Add(t);
                    }

                    List<ObjectPicker.RowInfo> taxable = CommonMethodsTaxCollector.ReturnHouseholdsAsRowInfo(taxableHouseholds);
                    List<ObjectPicker.RowInfo> noneTaxable = CommonMethodsTaxCollector.ReturnHouseholdsAsRowInfo(CommonMethodsTaxCollector.ReturnNoneTaxableHouseholds(base.Target.info));

                    //Combine none taxable and taxable
                    noneTaxable.AddRange(taxable);

                    List<ObjectPicker.RowInfo> result = DualPanelHouseholds.Show(noneTaxable, taxable,
                        CommonMethodsTaxCollector.LocalizeString("Taxable", new object[0]),
                        CommonMethodsTaxCollector.LocalizeString("NoneTaxable", new object[0]));

                    if (result != null && result.Count > 0)
                    {
                        base.Target.info.TaxableHouseholds = new List<Household>();
                        foreach (var r in result)
                        {
                            base.Target.info.TaxableHouseholds.Add((Household)r.Item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommonMethodsTaxCollector.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class ModifyFunds : ImmediateInteraction<Sim, TaxCollector>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, TaxCollector, ModifyFunds>
            {
                public override bool Test(Sim a, TaxCollector target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                // Methods  
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuAni, new object[0]),
				CommonMethodsTaxCollector.LocalizeString("SettingsMenu", new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, TaxCollector target, InteractionObjectPair interaction)
                {
                    //TODO: Luo localisaatio
                    return CommonMethodsTaxCollector.LocalizeString("ModifyFunds", new object[0]);
                }
            }

            public static InteractionDefinition Singleton = new ModifyFunds.Definition();

            public override bool Run()
            {
                try
                {
                    string funds = CommonMethodsTaxCollector.ShowDialogue(CommonMethodsTaxCollector.LocalizeString("ModifyFunds", new object[0]), string.Empty, base.Target.info.Funds.ToString());
                    if (!string.IsNullOrEmpty(funds))
                        int.TryParse(funds, out base.Target.info.Funds);
                }
                catch (Exception ex)
                {
                    CommonMethodsTaxCollector.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class SetMultiplier : ImmediateInteraction<Sim, TaxCollector>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, TaxCollector, SetMultiplier>
            {
                public override bool Test(Sim a, TaxCollector target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                // Methods  
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuAni, new object[0]),
				CommonMethodsTaxCollector.LocalizeString("SettingsMenu", new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, TaxCollector target, InteractionObjectPair interaction)
                {
                    //TODO: Luo localisaatio
                    return CommonMethodsTaxCollector.LocalizeString("SetMultiplier", new object[0]);
                }
            }
            public static InteractionDefinition Singleton = new SetMultiplier.Definition();

            public override bool Run()
            {
                try
                {
                    string name = CommonMethodsTaxCollector.ShowDialogue(
                        CommonMethodsTaxCollector.LocalizeString("SetMultiplier", new object[0]), 
                        CommonMethodsTaxCollector.LocalizeString("SetMultiplierDescription", new object[0]), 
                        base.Target.info.Multiplier.ToString());

                    if (!string.IsNullOrEmpty(name))
                        float.TryParse(name, out this.Target.info.Multiplier);
                }
                catch (Exception ex)
                {
                    CommonMethodsTaxCollector.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        #endregion

    }
}
