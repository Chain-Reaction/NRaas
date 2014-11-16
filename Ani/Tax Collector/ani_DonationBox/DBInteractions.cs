using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.SimIFace;
using Sims3.Gameplay.Utilities;
using ani_taxcollector;
using System;
using Sims3.Gameplay.Autonomy;
using System.Text;
using Sims3.Gameplay.Objects.Decorations.Mimics.ani_DonationBox;
using Sims3.Gameplay.Core;
using Sims3.UI;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.ActorSystems;

namespace ani_DonationBox
{
    //Settings
    public class ChangeName : ImmediateInteraction<Sim, DonationBox>
    {
        public class Definition : ImmediateInteractionDefinition<Sim, DonationBox, ChangeName>
        {
            public override bool Test(Sim a, DonationBox target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
            public override string[] GetPath(bool isFemale)
            {
                return new string[]{
				CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuSettingsPath, new object[0])
    			};
            }
            public override string GetInteractionName(Sim a, DonationBox target, InteractionObjectPair interaction)
            {
                return CommonMethodsTaxCollector.LocalizeString("SetName", new object[0]);
            }
        }

        public static InteractionDefinition Singleton = new ChangeName.Definition();

        public override bool Run()
        {
            try
            {
                string s = CommonMethodsTaxCollector.ShowDialogue(CommonMethodsTaxCollector.LocalizeString("SetName", new object[0]), string.Empty, base.Target.info.Name);
                if (!string.IsNullOrEmpty(s))
                    base.Target.info.Name = s;
            }
            catch (Exception ex)
            {
                CommonMethodsTaxCollector.PrintMessage(ex.Message);
            }

            return true;
        }

    }

    class EditFunds : ImmediateInteraction<Sim, DonationBox>
    {
        public class Definition : ImmediateInteractionDefinition<Sim, DonationBox, EditFunds>
        {
            public override bool Test(Sim a, DonationBox target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[]{
				CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuSettingsPath, new object[0])
    			};
            }
            public override string GetInteractionName(Sim a, DonationBox target, InteractionObjectPair interaction)
            {
                return CommonMethodsTaxCollector.LocalizeString("ModifyFunds", new object[0]);
            }
        }

        public static InteractionDefinition Singleton = new EditFunds.Definition();

        public override bool Run()
        {
            try
            {
                int funds = 0;
                string changeValue = CommonMethodsTaxCollector.ShowDialogue(CommonMethodsTaxCollector.LocalizeString("ModifyFunds", new object[0]), string.Empty, base.Target.info.Funds.ToString());

                if (int.TryParse(changeValue, out funds))
                    base.Target.info.Funds = funds;
            }
            catch (Exception ex)
            {
                CommonMethodsTaxCollector.PrintMessage(ex.Message);
            }

            return true;
        }

    }

    class DonationMoodValue : ImmediateInteraction<Sim, DonationBox>
    {
        public class Definition : ImmediateInteractionDefinition<Sim, DonationBox, DonationMoodValue>
        {
            public override bool Test(Sim a, DonationBox target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[]{
				CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuSettingsPath, new object[0])
    			};
            }
            public override string GetInteractionName(Sim a, DonationBox target, InteractionObjectPair interaction)
            {
                return CommonMethodsTaxCollector.LocalizeString("DonationMoodValue", new object[0]);
            }
        }

        public static InteractionDefinition Singleton = new DonationMoodValue.Definition();

        public override bool Run()
        {
            try
            {
                int value = 0;
                string changeValue = CommonMethodsTaxCollector.ShowDialogue(CommonMethodsTaxCollector.LocalizeString("DonationMoodValue", new object[0]), string.Empty, base.Target.info.DonationMoodValue.ToString());

                if (int.TryParse(changeValue, out value))
                    base.Target.info.DonationMoodValue = value;
            }
            catch (Exception ex)
            {
                CommonMethodsTaxCollector.PrintMessage(ex.Message);
            }

            return true;
        }

    }

    //Buildings
    public class SaveLotValue : ImmediateInteraction<Sim, DonationBox>
    {
        public class Definition : ImmediateInteractionDefinition<Sim, DonationBox, SaveLotValue>
        {
            public override bool Test(Sim a, DonationBox target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[]{
				CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuBuildingPath, new object[0])
    			};
            }
            public override string GetInteractionName(Sim a, DonationBox target, InteractionObjectPair interaction)
            {
                return CommonMethodsTaxCollector.LocalizeString("SaveLotValue", new object[0]);
            }
        }

        public static InteractionDefinition Singleton = new SaveLotValue.Definition();

        public override bool Run()
        {
            try
            {
                base.Target.info.LotValue = base.Target.LotCurrent.Cost;
                CommonMethodsTaxCollector.PrintMessage(CommonMethodsTaxCollector.LocalizeString("SaveLotValueDescription", new object[]{base.Target.LotCurrent.Cost}));
            }
            catch (Exception ex)
            {
                CommonMethodsTaxCollector.PrintMessage(ex.Message);
            }

            return true;
        }

    }

    public class SubstractFromFunds : ImmediateInteraction<Sim, DonationBox>
    {
        public class Definition : ImmediateInteractionDefinition<Sim, DonationBox, SubstractFromFunds>
        {
            public override bool Test(Sim a, DonationBox target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
            public override string[] GetPath(bool isFemale)
            {
                return new string[]{
				CommonMethodsTaxCollector.LocalizeString(CommonMethodsTaxCollector.MenuBuildingPath, new object[0])
    			};
            }
            public override string GetInteractionName(Sim a, DonationBox target, InteractionObjectPair interaction)
            {
                return CommonMethodsTaxCollector.LocalizeString("SubstractChanges", new object[0]);
            }
        }

        public static InteractionDefinition Singleton = new SubstractFromFunds.Definition();

        public override bool Run()
        {
            try
            {

                base.Target.info.Funds -= (base.Target.LotCurrent.Cost - base.Target.info.LotValue);

                //StringBuilder sb = new StringBuilder();
                //sb.Append("Previous Lot Value: ");
                //sb.Append(base.Target.info.LotValue);
                //sb.Append("\n");
                //sb.Append("Current Lot Value: ");
                //sb.Append(base.Target.LotCurrent.Cost);
                //sb.Append("\n");
                //sb.Append("Funds Left: ");
                //sb.Append(base.Target.info.Funds);
                //sb.Append("\n");

                CommonMethodsTaxCollector.PrintMessage(CommonMethodsTaxCollector.LocalizeString("SubstractChangesDescription", new object[] { 
                    base.Target.info.LotValue, base.Target.LotCurrent.Cost, base.Target.info.Funds }));

            }
            catch (Exception ex)
            {
                CommonMethodsTaxCollector.PrintMessage(ex.Message);
            }

            return true;
        }

    }

    //Sim
    public class WithdrawFunds : Interaction<Sim, DonationBox>
    {
        public class Definition : InteractionDefinition<Sim, DonationBox, WithdrawFunds>
        {
            public override bool Test(Sim a, DonationBox target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
            public override string GetInteractionName(Sim a, DonationBox target, InteractionObjectPair interaction)
            {
                return CommonMethodsTaxCollector.LocalizeString("MakeWithdrawl", new object[0]);
            }
        }

        public static InteractionDefinition Singleton = new WithdrawFunds.Definition();

        public override bool Run()
        {
            try
            {
                int funds = 0;
                string s = CommonMethodsTaxCollector.ShowDialogueNumbersOnly(CommonMethodsTaxCollector.LocalizeString("MakeWithdrawl", new object[0]),
                   CommonMethodsTaxCollector.LocalizeString("FundsInTaxCollector", new object[] { base.Target.info.Name, base.Target.info.Funds }), string.Empty);

                if (int.TryParse(s, out funds))
                {
                    base.Target.info.Funds -= funds;
                    base.Actor.Household.SetFamilyFunds(base.Actor.Household.FamilyFunds + funds);
                }
            }
            catch (Exception ex)
            {
                CommonMethodsTaxCollector.PrintMessage(ex.Message);
            }

            return true;
        }

    }

    public class Donate : Interaction<Sim, DonationBox>
    {
        public class Definition : InteractionDefinition<Sim, DonationBox, Donate>
        {
            public override bool Test(Sim a, DonationBox target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
            public override string GetInteractionName(Sim a, DonationBox target, InteractionObjectPair interaction)
            {
                return CommonMethodsTaxCollector.LocalizeString("MakeDonation", new object[] { target.info.Name });
            }
        }

        public static InteractionDefinition Singleton = new Donate.Definition();

        public override bool Run()
        {
            try
            {
                int simoleons = 0;
                string s = CommonMethodsTaxCollector.ShowDialogueNumbersOnly(CommonMethodsTaxCollector.LocalizeString("MakeDonation", new object[] { base.Target.info.Name}), 
                    string.Empty, string.Empty);

                if (int.TryParse(s, out simoleons))
                {
                    MailboxDoor mailboxDoor = this.Target.GetDoorOfSim(this.Actor);// ?? this.Target.GetAnyDoor();
                   
                    //Route to mailbox
                    if (mailboxDoor == null || !this.Actor.RouteToObjectRadius(base.Target, 1))
                    {
                        return false;
                    }

                    if (simoleons > this.Actor.FamilyFunds)
                    {
                        this.Actor.AddExitReason(ExitReason.CanceledByScript);
                        return false;
                    }
                                       
                    bool flag = true;
                    base.StandardEntry();
                    base.EnterStateMachine("mailbox", "Enter", "x", "mailbox");
                    base.SetActor("wallMailboxes", this.Target);
                    base.SetParameter("IsWallMailbox", true);
                    mailboxDoor.SetAnimParams(this.mCurrentStateMachine);
                                    
                    bool flagUp = false; 
                    base.SetParameter("IsFlagAlreadyUp", flagUp);
                    base.BeginCommodityUpdates();
                    
                    mailboxDoor.SetProductVersionForDoorAnim(this.mCurrentStateMachine);
                    base.AnimateSim("Put Mail s1");
                    mailboxDoor.UnsetProductVersionForDoorAnim(this.mCurrentStateMachine);
                    base.AnimateSim("Put Mail");

                    //Substract money and add to funds
                    this.Actor.ModifyFunds(-simoleons);
                    base.Target.info.Funds += simoleons;

                    if (base.Target.info.DonationMoodValue > 0)
                    {
                        EventTracker.SendEvent(new IncrementalEvent(EventTypeId.kDonatedToCharity, this.Actor, null, (float)simoleons));
                        this.Actor.BuffManager.AddElement(BuffNames.Charitable, base.Target.info.DonationMoodValue, Origin.FromCharity);
                        this.Actor.DonatedToCharityTime = (int)SimClock.ElapsedTime(TimeUnit.Days);
                    }
                    
                   
                    Audio.StartSound("ui_object_buy");

                    base.EndCommodityUpdates(flag);
                    base.AnimateSim("Exit");
                    base.StandardExit();
                    return flag;

                }
            }
            catch (Exception ex)
            {
                CommonMethodsTaxCollector.PrintMessage(ex.Message);
            }

            return true;
        }

    }

}
