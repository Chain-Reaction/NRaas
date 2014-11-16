using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.ObjectComponents;
using ani_OFBStand;
using Sims3.Gameplay.Objects.TombObjects;
using Sims3.Gameplay.ActorSystems;
using System.Collections.Generic;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Register;
using Sims3.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using System.Text;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.EventSystem;
using System;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Core;
using System.Collections;
using Sims3.SimIFace.CAS;
using Sims3.Gameplay.Seasons;

namespace Sims3.Gameplay.Objects.TombObjects.ani_OFBStand
{
    public class OFBStand : TreasureChest
    {
        #region The Object

        public OFBStandInformation info;

        public ulong mBuyingSimId;
        public ulong mBrowsingSimId;
        public ulong mTendingSimId;
        public static float kShoppingTime = 5f;
        public Role mCurrentRole;

        public static bool ShowDebugMessages = false;

        //The object
        public override void OnCreation()
        {
            base.OnCreation();

            info = new OFBStandInformation();

        }
        public override Tooltip CreateTooltip(Vector2 mousePosition, WindowBase mousedOverWindow, ref Vector2 tooltipPosition)
        {
            string status = CommonMethodsOFBStand.LocalizeString("Open", new object[0]);
            string owner = CommonMethodsOFBStand.LocalizeString("NoOwnerSpecified", new object[0]);
            string employee = CommonMethodsOFBStand.LocalizeString("NoEmployeeSpecified", new object[0]);

            if (this.info.Owner != null)
                owner = this.info.Owner.FullName;

            if (this.info.Employee != null)
                employee = this.info.Employee.FullName;

            if (!this.info.IsStandOpen)
                status = CommonMethodsOFBStand.LocalizeString("Closed", new object[0]);


            return new SimpleTextTooltip(CommonMethodsOFBStand.LocalizeString("ObjectTooltip", new object[] { status, owner, employee, 
                this.info.StartTime.ToString(), this.info.EndTime.ToString(), this.info.PayPerHour }));
        }

        public override void OnStartup()
        {
            base.AddComponent<ItemComponent>(new object[] { ItemComponent.SimInventoryItem });

            base.OnStartup();

            if (this.Inventory != null)
                this.Inventory.IgnoreInventoryValidation = true;

            //Remove origianl interactions 
            base.RemoveInteractionByType(TreasureChest.OpenWithAnimation.Singleton);
            base.RemoveInteractionByType(TreasureChest.OpenWithoutAnimation.Singleton);
            base.RemoveInteractionByType(TreasureChest.InsertKeystone.Singleton);

            //Settings
            base.AddInteraction(SetOwner.Singleton);
            base.AddInteraction(SetEmployee.Singleton);
            base.AddInteraction(SetWorkingHours.Singleton);
            base.AddInteraction(SetPriceIncrease.Singleton);
            base.AddInteraction(ToggleStandOpen.Singleton);
            base.AddInteraction(SetAlwaysCanBuy.Singleton);
            base.AddInteraction(SetWage.Singleton);
            base.AddInteraction(SetWageFromOwnerFunds.Singleton);
            base.AddInteraction(SetCooldownPeriod.Singleton);
            base.AddInteraction(SetChangeToWorkOutfit.Singleton);

            //Settings -> Food 
            base.AddInteraction(UnSpoil.Singleton);
            base.AddInteraction(SetServingPrice.Singleton);

            //Inventory
            base.AddInteraction(StandOpenWithoutAnimation.Singleton);

            //Selling and buying
            base.AddInteraction(TendStand.Singleton);
            base.AddInteraction(BrowseStand.Singleton);
            base.AddInteraction(BuyFromStand.Singleton);

            if (this.info.IsStandOpen && this.info.Employee != null)
                this.RestartAlarms();
        }


        #region ShoppingRegister Stuff

        public bool CheckForMotiveFailure(Sim actor)
        {
            if (actor.Motives.MotiveInDistress != null)
            {
                BuffNames guid = actor.Motives.MotiveInDistress.Guid;
                if (actor.Motives.InMotiveDistress && guid != BuffNames.Desolate && guid != BuffNames.Stressed && guid != BuffNames.Starving)
                {
                    actor.AddExitReason(ExitReason.BuffFailureState);
                    return true;
                }
            }
            foreach (BuffInstance current in actor.BuffManager.Buffs)
            {
                if (current.mBuff.PermaMoodlet && (current.mBuff is BuffHasToPee || current.mBuff is BuffSleepy || current.mBuff is BuffTired))
                {
                    actor.AddExitReason(ExitReason.BuffFailureState);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Set Alarms for hire

        private float kOneMinuteInHours = 0.017f;
        private static float kHoursBeforeWorkToHeadToWork = 0.5f;
        private static float kfAlarmContinuousSummonFrequncy = 15f;

        private AlarmHandle mfJobAlarmEarly = AlarmHandle.kInvalidHandle;
        private AlarmHandle mJobAlarmLate = AlarmHandle.kInvalidHandle;
        private AlarmHandle mJobFetchMidJobAlarm = AlarmHandle.kInvalidHandle;
        private AlarmHandle mJobOver = AlarmHandle.kInvalidHandle;

        public override void Dispose()
        {
            this.DeleteAlarms();
            base.Destroy();
        }

        public bool IsValidStore()
        {
            if (!this.info.IsStandOpen)
                return false;

            //Is the sim really tending it
            Sim sim = SimDescription.GetCreatedSim(this.mTendingSimId);
            if (sim == null)
            {
                return false;
            }
            else
            {
                if (!sim.InteractionQueue.GetCurrentInteraction().GetInteractionName().Equals(CommonMethodsOFBStand.LocalizeString("TendStand", new object[0])))
                {
                    this.mTendingSimId = 0uL;
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void DeleteAlarms()
        {
            if (this.mfJobAlarmEarly != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mfJobAlarmEarly);
                this.mfJobAlarmEarly = AlarmHandle.kInvalidHandle;
            }
            if (this.mJobAlarmLate != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mJobAlarmLate);
                this.mJobAlarmLate = AlarmHandle.kInvalidHandle;
            }
            if (this.mJobFetchMidJobAlarm != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mJobFetchMidJobAlarm);
                this.mJobFetchMidJobAlarm = AlarmHandle.kInvalidHandle;
            }
            if (this.mJobOver != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mJobOver);
                this.mJobOver = AlarmHandle.kInvalidHandle;
            }
        }

        public void RestartAlarms()
        {
            this.DeleteAlarms();
            float hour = SimClock.CurrentTime().Hour;
            float num = this.info.StartTime;
            float num2 = num - hour - kHoursBeforeWorkToHeadToWork;
            float num3 = num - hour + this.kOneMinuteInHours;
            while (num2 < 0f)
            {
                num2 += 24f;
            }
            while (num3 < 0f)
            {
                num3 += 24f;
            }
            this.mfJobAlarmEarly = base.AddAlarm(num2, TimeUnit.Hours, new AlarmTimerCallback(this.SummonEmployy), "Oven chef-summon alarm", AlarmType.AlwaysPersisted);
            this.mJobAlarmLate = base.AddAlarm(num3, TimeUnit.Hours, new AlarmTimerCallback(this.SummonEmployy), "Oven chef-summon alarm", AlarmType.AlwaysPersisted);
            if (SimClock.IsTimeBetweenTimes(this.info.StartTime, this.info.EndTime))
            {
                this.mJobFetchMidJobAlarm = base.AddAlarm(kfAlarmContinuousSummonFrequncy, TimeUnit.Minutes, new AlarmTimerCallback(this.SummonEmployy), "Oven chef-summon alarm", AlarmType.AlwaysPersisted);
            }
        }

        public void ExitWork()
        {
            if ((int)SimClock.CurrentTime().Hour >= this.info.EndTime &&
                this.info.Employee != null && this.info.Employee.SimDescriptionId == this.mTendingSimId)
            {
                this.info.Employee.CreatedSim.InteractionQueue.CancelAllInteractionsByType(TendStand.Singleton);
                this.mTendingSimId = 0uL;
                this.RestartAlarms();
            }
        }

        private void SummonEmployy()
        {
            //Exit work           
            if ((int)SimClock.CurrentTime().Hour >= this.info.EndTime)
            {
                ExitWork();
                return;
            }
            else
            {
                //this.RetestHoursOfOperation();
                this.RestartAlarms();

                if (!base.InWorld || base.InInventory)
                {
                    return;
                }
                if (this.info.Employee == null || this.info.Employee.SimDescriptionId == 0uL)
                {
                    return;
                }
                Sim sim = null;
                SimDescription simDesc = null;

                foreach (SimDescription sd in SimDescription.GetAll(SimDescription.Repository.Household))
                {
                    if (sd.SimDescriptionId == this.info.Employee.SimDescriptionId)
                    {
                        simDesc = sd;
                        sim = sd.CreatedSim;

                        break;
                    }
                }

                if (sim != null && SimClock.IsTimeBetweenTimes(this.info.StartTime, this.info.EndTime))
                {
                    InteractionInstance instance = TendStand.Singleton.CreateInstance(this, sim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true);

                    //Is sim working or not
                    if (sim.InteractionQueue.GetCurrentInteraction() != instance)
                    {
                        //If the sim has this interaction in the que, don't add
                        if (!sim.InteractionQueue.HasInteractionOfType(instance.GetType()))
                            sim.InteractionQueue.AddNext(instance);
                    }
                    else
                        sim.InteractionQueue.PushAsContinuation(instance, false);
                }
                else
                {
                    if (sim == null)
                    {
                        sim = simDesc.Instantiate(base.LotCurrent);
                    }
                }
            }

        }
        #endregion

        #region Treasure Chest

        public override string OpenSfx
        {
            get
            {
                return "treas_chest_opena";
            }
        }
        public override string CloseSfx
        {
            get
            {
                return "treas_chest_closea";
            }
        }

        #endregion

        #endregion The Object

        #region Interactions

        #region Settings

        //The settings that needs the stand to be closed before editing
        class SetOwner : ImmediateInteraction<Sim, OFBStand>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBStand, SetOwner>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
				CommonMethodsOFBStand.LocalizeString(CommonMethodsOFBStand.MenuSettingsPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, OFBStand target, InteractionObjectPair interaction)
                {
                    if (target.info.Owner != null)
                        return CommonMethodsOFBStand.LocalizeString("SetOwner", new object[] { target.info.Owner.FullName });
                    return CommonMethodsOFBStand.LocalizeString("SetOwner", new object[] { CommonMethodsOFBStand.LocalizeString("NoOwnerSpecified", new object[0]) });
                }

                public override bool Test(Sim a, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.info.IsStandOpen)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("StandOpen", new object[0]));
                        return false;
                    }
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new SetOwner.Definition();

            public override bool Run()
            {
                try
                {
                    base.Target.info.Owner = CommonMethodsOFBStand.ReturnSim(base.Actor, true, true);
                    if (base.Target.info.Owner != null)
                        CommonMethodsOFBStand.PrintMessage(Target.info.Owner.FullName);
                }
                catch (Exception ex)
                {
                    CommonMethodsOFBStand.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class SetEmployee : ImmediateInteraction<Sim, OFBStand>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBStand, SetEmployee>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
				CommonMethodsOFBStand.LocalizeString(CommonMethodsOFBStand.MenuSettingsPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, OFBStand target, InteractionObjectPair interaction)
                {
                    if (target.info.Employee != null)
                        return CommonMethodsOFBStand.LocalizeString("SetEmployee", new object[] { target.info.Employee.FullName });
                    return CommonMethodsOFBStand.LocalizeString("SetEmployee", new object[] { CommonMethodsOFBStand.LocalizeString("NoEmployeeSpecified", new object[0]) });
                }

                public override bool Test(Sim a, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.info.IsStandOpen)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("StandOpen", new object[0]));
                        return false;
                    }
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new SetEmployee.Definition();

            public override bool Run()
            {
                try
                {
                    base.Target.info.Employee = CommonMethodsOFBStand.ReturnSim(base.Actor, true, false);
                }
                catch (Exception ex)
                {
                    CommonMethodsOFBStand.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class SetWorkingHours : ImmediateInteraction<Sim, OFBStand>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBStand, SetWorkingHours>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
				CommonMethodsOFBStand.LocalizeString(CommonMethodsOFBStand.MenuSettingsPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, OFBStand target, InteractionObjectPair interaction)
                {
                    return CommonMethodsOFBStand.LocalizeString("SetHours", new object[] { target.info.StartTime.ToString(), target.info.EndTime.ToString() });
                }

                public override bool Test(Sim a, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.info.IsStandOpen)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("StandOpen", new object[0]));
                        return false;
                    }
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new SetWorkingHours.Definition();

            public override bool Run()
            {
                try
                {
                    float.TryParse(CommonMethodsOFBStand.ShowDialogueNumbersOnly(CommonMethodsOFBStand.LocalizeString("SetStartTime", new object[0]),
                         CommonMethodsOFBStand.LocalizeString("SetStartTimeDescription", new object[0]),
                         base.Target.info.StartTime.ToString()),
                         out base.Target.info.StartTime);
                    float.TryParse(CommonMethodsOFBStand.ShowDialogueNumbersOnly(CommonMethodsOFBStand.LocalizeString("SetEndTime", new object[0]),
                       CommonMethodsOFBStand.LocalizeString("SetEndTimeDescription", new object[0]),
                        base.Target.info.EndTime.ToString()),
                        out base.Target.info.EndTime);

                    //string employee = "[Employee Not Selected]";
                    //if (base.Target.info.Employee != null)
                    //    employee = base.Target.info.Employee.FullName;
                }
                catch (Exception ex)
                {
                    CommonMethodsOFBStand.PrintMessage(ex.Message);
                }

                return true;
            }

        }


        //The other ones
        class SetPriceIncrease : ImmediateInteraction<Sim, OFBStand>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBStand, SetPriceIncrease>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
				CommonMethodsOFBStand.LocalizeString(CommonMethodsOFBStand.MenuSettingsPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, OFBStand target, InteractionObjectPair interaction)
                {
                    return CommonMethodsOFBStand.LocalizeString("SetPriceMultiplyer", new object[] { target.info.PriceIncrease.ToString() }); //CommonMethodsOFBStand.LocalizeString("SetName", new object[0]);
                }

                public override bool Test(Sim a, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new SetPriceIncrease.Definition();

            public override bool Run()
            {
                try
                {
                    float.TryParse(CommonMethodsOFBStand.ShowDialogue(CommonMethodsOFBStand.LocalizeString("PriceMultiplyer", new object[0]),
                        CommonMethodsOFBStand.LocalizeString("PriceMultiplyerDescription", new object[0]),
                        base.Target.info.PriceIncrease.ToString()), out base.Target.info.PriceIncrease);
                }
                catch (Exception ex)
                {
                    CommonMethodsOFBStand.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class SetAlwaysCanBuy : ImmediateInteraction<Sim, OFBStand>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBStand, SetAlwaysCanBuy>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
				CommonMethodsOFBStand.LocalizeString(CommonMethodsOFBStand.MenuSettingsPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, OFBStand target, InteractionObjectPair interaction)
                {
                    return CommonMethodsOFBStand.LocalizeString("SetAlwaysCanBuy", new object[] { target.info.AlwayCanBuy.ToString() });
                }

                public override bool Test(Sim a, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new SetAlwaysCanBuy.Definition();

            public override bool Run()
            {
                try
                {
                    base.Target.info.AlwayCanBuy = !base.Target.info.AlwayCanBuy;

                }
                catch (Exception ex)
                {
                    CommonMethodsOFBStand.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class SetWage : ImmediateInteraction<Sim, OFBStand>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBStand, SetWage>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
				CommonMethodsOFBStand.LocalizeString(CommonMethodsOFBStand.MenuSettingsPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, OFBStand target, InteractionObjectPair interaction)
                {
                    return CommonMethodsOFBStand.LocalizeString("SetPayPerHour", new object[] { target.info.PayPerHour });
                }

                public override bool Test(Sim a, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new SetWage.Definition();

            public override bool Run()
            {
                try
                {
                    int.TryParse(CommonMethodsOFBStand.ShowDialogueNumbersOnly("Set Pay/hour", string.Empty, base.Target.info.PayPerHour.ToString()), out base.Target.info.PayPerHour);
                }
                catch (Exception ex)
                {
                    CommonMethodsOFBStand.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class SetWageFromOwnerFunds : ImmediateInteraction<Sim, OFBStand>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBStand, SetWageFromOwnerFunds>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
				CommonMethodsOFBStand.LocalizeString(CommonMethodsOFBStand.MenuSettingsPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, OFBStand target, InteractionObjectPair interaction)
                {
                    return CommonMethodsOFBStand.LocalizeString("SetPayFromOwner", new object[] { target.info.PayWageFromOwnersFunds.ToString() });
                }

                public override bool Test(Sim a, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new SetWageFromOwnerFunds.Definition();

            public override bool Run()
            {
                try
                {
                    base.Target.info.PayWageFromOwnersFunds = !base.Target.info.PayWageFromOwnersFunds;
                }
                catch (Exception ex)
                {
                    CommonMethodsOFBStand.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        public class SetCooldownPeriod : ImmediateInteraction<Sim, OFBStand>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBStand, SetCooldownPeriod>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
				CommonMethodsOFBStand.LocalizeString(CommonMethodsOFBStand.MenuSettingsPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, OFBStand target, InteractionObjectPair interaction)
                {
                    return CommonMethodsOFBStand.LocalizeString("SetCooldown", new object[] { target.info.CooldownInMinutes.ToString() });
                }
                public override bool Test(Sim actor, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static InteractionDefinition Singleton = new SetCooldownPeriod.Definition();
            public override bool Run()
            {
                int.TryParse(CommonMethodsOFBStand.ShowDialogueNumbersOnly(
                    CommonMethodsOFBStand.LocalizeString("Cooldown", new object[0]),
                    CommonMethodsOFBStand.LocalizeString("CooldownDescription", new object[0]),
                    base.Target.info.CooldownInMinutes.ToString()), out base.Target.info.CooldownInMinutes);
                return true;
            }
        }

        class SetChangeToWorkOutfit : ImmediateInteraction<Sim, OFBStand>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBStand, SetChangeToWorkOutfit>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
				CommonMethodsOFBStand.LocalizeString(CommonMethodsOFBStand.MenuSettingsPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, OFBStand target, InteractionObjectPair interaction)
                {
                    return "Change to work outfit: " + target.info.ChangeToWorkOutfit;// CommonMethodsOFBStand.LocalizeString("SetPriceMultiplyer", new object[] { target.info.PriceIncrease.ToString() }); //CommonMethodsOFBStand.LocalizeString("SetName", new object[0]);
                }

                public override bool Test(Sim a, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new SetChangeToWorkOutfit.Definition();

            public override bool Run()
            {
                try
                {
                    base.Target.info.ChangeToWorkOutfit = !base.Target.info.ChangeToWorkOutfit;
                }
                catch (Exception ex)
                {
                    CommonMethodsOFBStand.PrintMessage(ex.Message);
                }

                return true;
            }

        }


        //Settings -> Food 
        class UnSpoil : ImmediateInteraction<Sim, OFBStand>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBStand, UnSpoil>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
				CommonMethodsOFBStand.LocalizeString(CommonMethodsOFBStand.MenuSettingsPath, new object[0]), 
                CommonMethodsOFBStand.LocalizeString(CommonMethodsOFBStand.MenuFoodPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, OFBStand target, InteractionObjectPair interaction)
                {
                    return CommonMethodsOFBStand.LocalizeString("Unspoil", new object[0]);// CommonMethodsOFBStand.LocalizeString("SetPriceMultiplyer", new object[] { target.info.PriceIncrease.ToString() }); //CommonMethodsOFBStand.LocalizeString("SetName", new object[0]);
                }

                public override bool Test(Sim a, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new UnSpoil.Definition();

            public override bool Run()
            {
                try
                {
                    ShoppingMethods.UnSpoil(base.Target);
                }
                catch (Exception ex)
                {
                    CommonMethodsOFBStand.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class SetServingPrice : ImmediateInteraction<Sim, OFBStand>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBStand, SetServingPrice>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{                        
				CommonMethodsOFBStand.LocalizeString(CommonMethodsOFBStand.MenuSettingsPath, new object[0]), 
                CommonMethodsOFBStand.LocalizeString(CommonMethodsOFBStand.MenuFoodPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, OFBStand target, InteractionObjectPair interaction)
                {
                    return CommonMethodsOFBStand.LocalizeString("SetServingPrice", new object[] { target.info.ServingPrice });
                }

                public override bool Test(Sim a, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new SetServingPrice.Definition();

            public override bool Run()
            {
                try
                {
                    int.TryParse(
                        CommonMethodsOFBStand.ShowDialogueNumbersOnly(CommonMethodsOFBStand.LocalizeString("SetServingPriceTitle", new object[0]),
                        CommonMethodsOFBStand.LocalizeString("SetServingPriceDescription", new object[0]),
                        base.Target.info.ServingPrice.ToString()),
                        out base.Target.info.ServingPrice);
                }
                catch (Exception ex)
                {
                    CommonMethodsOFBStand.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        #endregion

        #region Business

        class ToggleStandOpen : ImmediateInteraction<Sim, OFBStand>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBStand, ToggleStandOpen>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
				CommonMethodsOFBStand.LocalizeString(CommonMethodsOFBStand.MenuBusinessPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, OFBStand target, InteractionObjectPair interaction)
                {
                    if (target.info.IsStandOpen)
                        return CommonMethodsOFBStand.LocalizeString("CloseStand", new object[0]);
                    else
                        return CommonMethodsOFBStand.LocalizeString("OpenStand", new object[0]);
                }

                public override bool Test(Sim a, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new ToggleStandOpen.Definition();

            public override bool Run()
            {
                try
                {
                    base.Target.info.IsStandOpen = !base.Target.info.IsStandOpen;
                    if (base.Target.info.IsStandOpen)
                    {
                        if (base.Target.info.Employee != null)
                        {
                            base.Target.RestartAlarms();
                            this.Target.mTendingSimId = 0uL;
                        }
                    }
                    else
                    {
                        base.Target.DeleteAlarms();
                        //Stop tending the stand 
                        if (base.Target.mTendingSimId != 0uL)
                        {
                            if (base.Target.info.Employee != null && base.Target.info.Employee.SimDescriptionId == base.Target.mTendingSimId)
                                base.Target.info.Employee.CreatedSim.InteractionQueue.CancelAllInteractions();
                            base.Target.mTendingSimId = 0uL;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommonMethodsOFBStand.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        public class StandOpenWithoutAnimation : ImmediateInteraction<Sim, OFBStand>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBStand, StandOpenWithoutAnimation>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
				CommonMethodsOFBStand.LocalizeString(CommonMethodsOFBStand.MenuBusinessPath, new object[0])
    			};
                }
                public override string GetInteractionName(Sim a, OFBStand target, InteractionObjectPair interaction)
                {
                    return CommonMethodsOFBStand.LocalizeString("OpenInventory", new object[0]);
                }
                public override bool Test(Sim actor, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return !target.HasTreasureChestInfoData();
                }
            }
            public static InteractionDefinition Singleton = new StandOpenWithoutAnimation.Definition();
            public override bool Run()
            {
                HudModel.OpenObjectInventoryForOwner(this.Target);
                EventTracker.SendEvent(EventTypeId.kSimOpenedTreasureChest, this.Actor, this.Target);
                return true;
            }
        }

        #endregion

        #region Animated
        [Persistable]
        public class TendStand : Interaction<Sim, OFBStand>
        {
            private sealed class Definition : InteractionDefinition<Sim, OFBStand, TendStand>
            {
                public override bool Test(Sim a, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    //Initialize data
                    target.IsValidStore();

                    if (!target.info.IsStandOpen)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("StandClosed", new object[0]));
                        return false;
                    }

                    if (target.mTendingSimId != 0uL)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("TendingOccupied", new object[0]));
                        return false;
                    }
                    return true;
                }
                public override string GetInteractionName(Sim actor, OFBStand target, InteractionObjectPair iop)
                {
                    return CommonMethodsOFBStand.LocalizeString("TendStand", new object[0]);
                }
            }
            private float mTimeToExitBehindTend;
            public static readonly InteractionDefinition Singleton = new TendStand.Definition();
            [TunableComment("Range:  1-x  Description:  Time in Sim minutes sim will stand behind counter playing randomd idle loop"), Tunable]
            private static int kSimMinBehindRegister = 30;
            public override bool Run()
            {
                float startTime = SimClock.CurrentTime().Hour;
                try
                {
                    if (!this.Actor.RouteToSlot(this.Target, Slot.RoutingSlot_1))
                    {
                        return false;
                    }

                    if (base.Target.info.ChangeToWorkOutfit)
                        this.Actor.SwitchToOutfitWithoutSpin(OutfitCategories.Career);

                    if (OFBStand.ShowDebugMessages)
                        CommonMethodsOFBStand.PrintMessage(base.Actor.FullName + " started work: " + startTime);

                    this.Target.mTendingSimId = base.Actor.SimDescription.SimDescriptionId;

                    base.StandardEntry();
                    base.BeginCommodityUpdates();

                    this.mTimeToExitBehindTend = (float)kSimMinBehindRegister;
                    base.EnterStateMachine("fruitveggiestand_store", "TendStart", "x", "FruitVeggieStand");
                    FoodProp foodProp = FoodProp.Create("apple");
                    if (foodProp != null)
                    {
                        foodProp.ActorsUsingMe.Add(this.Actor);
                        foodProp.SetGeometryState("plantHarvest");
                        this.mCurrentStateMachine.SetActor("FruitProp", foodProp);
                    }
                    bool flag = true;
                    while (flag)
                    {
                        base.AnimateSim("TendLoop");
                        this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.LoopCallback), null);
                        if ((this.Actor.ExitReason & ExitReason.StageComplete) != ExitReason.None)
                        {
                            this.Actor.ClearExitReasons();
                            if (this.Target.mBuyingSimId != 0uL)
                            {
                                base.AnimateSim("Sell");
                            }
                            else
                            {
                                if (this.Target.mBrowsingSimId != 0uL)
                                {
                                    base.AnimateSim("Stock");
                                }
                            }
                        }
                        else
                        {
                            flag = false;
                        }
                    }

                    float hours = SimClock.CurrentTime().Hour - startTime;

                    if (base.Target.info.PayPerHour > 0)
                    {
                        ShoppingMethods.PayEmployee(base.Target, base.Actor, hours);
                    }

                    if (base.Target.info.ChangeToWorkOutfit)
                    {
                        if (SeasonsManager.CurrentSeason == Sims3.SimIFace.Enums.Season.Winter)
                            this.Actor.SwitchToOutfitWithoutSpin(OutfitCategories.Outerwear);
                        else
                            this.Actor.SwitchToOutfitWithoutSpin(OutfitCategories.Everyday);
                    }

                    if (OFBStand.ShowDebugMessages)
                        CommonMethodsOFBStand.PrintMessage(base.Actor.FullName + " stopped work. ");

                    base.AnimateSim("TendExit");
                    base.EndCommodityUpdates(true);
                    base.StandardExit();
                    this.Target.mTendingSimId = 0uL;
                }
                catch (Exception ex)
                {
                    CommonMethodsOFBStand.PrintMessage("Exception in TendStand: " + ex.Message);

                }
                return true;
            }
            private void LoopCallback(StateMachineClient smc, InteractionInstance.LoopData ld)
            {
                //if motive failure, and not in active household, max motives
                if (!this.Actor.IsInActiveHousehold && this.Target.CheckForMotiveFailure(this.Actor))
                {
                    this.Actor.Motives.MaxEverything();
                    //this.Actor.AddExitReason(ExitReason.MoodFailure);
                    //return;
                }
                if (base.Autonomous && ld.mLifeTime >= this.mTimeToExitBehindTend)
                {
                    this.Actor.AddExitReason(ExitReason.Finished);
                }
                if (this.Target.mBuyingSimId != 0uL || this.Target.mBrowsingSimId != 0uL)
                {
                    this.Actor.AddExitReason(ExitReason.StageComplete);
                }
                if (this.Actor.InteractionQueue.HasInteractionOfType(typeof(CarpoolManager.GetInCarpool)) || this.Actor.InteractionQueue.HasInteractionOfType(typeof(GoToSchoolInRabbitHole)))
                {
                    this.Actor.AddExitReason(ExitReason.Canceled);
                }
                this.Actor.Motives.SetValue(CommodityKind.Temperature, 0f);
            }
        }

        public class BrowseStand : Interaction<Sim, OFBStand>
        {
            private sealed class Definition : InteractionDefinition<Sim, OFBStand, BrowseStand>
            {
                public override bool Test(Sim a, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    //Initialize data
                    target.IsValidStore();

                    if (!target.info.IsStandOpen)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("StandClosed", new object[0]));
                        return false;
                    }

                    if (!target.info.AlwayCanBuy && target.mTendingSimId == 0uL)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("NoTender", new object[0]));
                        return false;
                    }

                    if (target.Inventory == null || target.Inventory.IsEmpty)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("NoInventory", new object[0]));
                        return false;
                    }

                    if (isAutonomous && a.BuffManager.HasElement(Sims3.Gameplay.ActorSystems.BuffNames.NewStuff))
                        return false;

                    if (!isAutonomous && target.info.AlwayCanBuy)
                        return true;

                    return true;
                }
                public override string GetInteractionName(Sim actor, OFBStand target, InteractionObjectPair iop)
                {
                    return CommonMethodsOFBStand.LocalizeString("Browse", new object[0]);
                }
            }
            [TunableComment("Range: int 1-x. Description: how long after sim plays look at before exiting browse interaction"), Tunable]
            private static int kMaxBrowseMaxTimeInMinutes = 10;
            public static readonly InteractionDefinition Singleton = new BrowseStand.Definition();
            public override bool Run()
            {
                if (!this.Actor.RouteToSlot(this.Target, Slot.RoutingSlot_0))
                {
                    return false;
                }

                if (OFBStand.ShowDebugMessages)
                    CommonMethodsOFBStand.PrintMessage(base.Actor.FullName + " is browsing");

                base.StandardEntry();
                base.BeginCommodityUpdates();
                base.EnterStateMachine("fruitveggiestand_store", "ShopStart", "y", "FruitVeggieStand");
                this.mCurrentStateMachine.RequestState(true, "y", "shopLoop");
                this.Target.mBrowsingSimId = this.Actor.SimDescription.SimDescriptionId;
                this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.BrowseLoopDelegate), this.mCurrentStateMachine);
                this.mCurrentStateMachine.RequestState(true, "y", "ShopExit");
                this.Target.mBrowsingSimId = 0uL;
                base.EndCommodityUpdates(true);
                base.StandardExit();


                //Check for exit work                
                // base.Target.ExitWork(base.Target.IsUserDirectedTend);
                this.Actor.Wander(ShoppingRegister.TendRegister.kMinWanderDistance, ShoppingRegister.TendRegister.kMaxWanderDistance, false, RouteDistancePreference.NoPreference, false);


                return true;
            }
            public void BrowseLoopDelegate(StateMachineClient smc, InteractionInstance.LoopData loopData)
            {
                if (loopData.mLifeTime > (float)BrowseStand.kMaxBrowseMaxTimeInMinutes)
                {
                    this.Actor.AddExitReason(ExitReason.StageComplete);
                }
            }
        }

        public class BuyFromStand : Interaction<Sim, OFBStand>
        {
            private sealed class Definition : InteractionDefinition<Sim, OFBStand, BuyFromStand>
            {
                public override bool Test(Sim a, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    //Initialize data
                    target.IsValidStore();

                    if (!target.info.IsStandOpen)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("StandClosed", new object[0]));
                        return false;
                    }

                    if (!target.info.AlwayCanBuy && target.mTendingSimId == 0uL)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("NoTender", new object[0]));
                        return false;
                    }

                    if (target.Inventory == null || target.Inventory.IsEmpty)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("NoInventory", new object[0]));
                        return false;
                    }

                    if (isAutonomous && a.BuffManager.HasElement(Sims3.Gameplay.ActorSystems.BuffNames.NewStuff))
                        return false;

                    if (!isAutonomous && target.info.AlwayCanBuy)
                        return true;

                    return true;
                }
                public override string GetInteractionName(Sim actor, OFBStand target, InteractionObjectPair iop)
                {
                    return CommonMethodsOFBStand.LocalizeString("Buy", new object[0]);
                }
            }
            public static readonly InteractionDefinition Singleton = new BuyFromStand.Definition();

            public void PostAnimation()
            {

            }

            public override bool Run()
            {
                int price = 0;

                if (!this.Actor.RouteToSlot(this.Target, Slot.RoutingSlot_2))
                {
                    return false;
                }

                if (OFBStand.ShowDebugMessages)
                    CommonMethodsOFBStand.PrintMessage(base.Actor.FullName + " is buying");

                #region Animations Start
                base.StandardEntry();
                base.BeginCommodityUpdates();

                base.EnterStateMachine("fruitveggiestand_store", "ShopStart", "y", "FruitVeggieStand");
                this.mCurrentStateMachine.RequestState(true, "y", "shopLoop");
                this.Target.mBuyingSimId = this.Actor.SimDescription.SimDescriptionId;
                this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.ShopLoopDelegate), this.mCurrentStateMachine);
                this.mCurrentStateMachine.RequestState(true, "y", "Purchase");
                #endregion

                //If autonomous, buy random object, and then cooloff
                if (this.Autonomous)
                {
                    #region Autonomous

                    GameObject produce = null;
                    if (base.Target.Inventory != null)
                        produce = base.Target.Inventory.GetRandomInventoryObject() as GameObject;

                    if (base.Target.Inventory.TryToRemove(produce))
                    {
                        //Only do this when active household
                        if (base.Actor.Inventory.TryToAdd(produce, false) &&
                            (base.Target.info.Owner != null && base.Target.info.Owner.Household != null && base.Target.info.Owner.Household.IsActive))
                            CommonMethodsOFBStand.PrintMessage(base.Actor.FullName + ": item bought " + produce.GetLocalizedName());

                        price = ShoppingMethods.CalculatePrice(produce.Value, base.Target.info.PriceIncrease);

                        if (base.Actor.Household != null)
                            base.Actor.Household.SetFamilyFunds(base.Actor.Household.FamilyFunds - price);

                        //Pay lot owner
                        ShoppingMethods.PayLotOwner(base.Target, price);
                        ShoppingMethods.UpdateSkillBasedCareerEarning(base.Target.info.Owner, produce);

                        //Add buffs
                        base.Actor.BuffManager.AddBuff(Sims3.Gameplay.ActorSystems.BuffNames.NewStuff, 0, base.Target.info.CooldownInMinutes, false, Sims3.Gameplay.ActorSystems.MoodAxis.None, Sims3.Gameplay.ActorSystems.Origin.FromNewObjects, false);

                    }
                    else
                    {
                        // CommonMethodsOFBStand.PrintMessage("Couldn't add item to " + base.Actor.FullName + " inventory");
                    }
                    #endregion
                }
                else
                {
                    //User directed 
                    #region User directed

                    List<InventoryItem> list = new List<InventoryItem>();
                    foreach (var item in base.Target.Inventory.InventoryItems.Values)
                    {
                        list.AddRange(item.List);
                    }

                    List<ObjectPicker.RowInfo> list2 = CommonMethodsOFBStand.ReturnInventoryItemAsRowInfo(list);

                    List<ObjectPicker.RowInfo> shoppingCart = DualPanelShopping.Show(list2, "Shopping Cart", "Items For Sale", base.Target.info.PriceIncrease);

                    //If we bought something
                    if (shoppingCart != null)
                    {
                        //Calculate Total Price
                        foreach (var item in shoppingCart)
                        {
                            price += ShoppingMethods.CalculatePrice(((InventoryItem)item.Item).Object.Value, base.Target.info.PriceIncrease);
                        }

                        if (price <= base.Actor.FamilyFunds)
                        {
                            bool success = true;
                            foreach (var item in shoppingCart)
                            {
                                GameObject o = ((InventoryItem)item.Item).Object as GameObject;

                                if (o != null)
                                {
                                    if (base.Target.Inventory.TryToRemove(o))
                                    {
                                        base.Actor.Inventory.TryToAdd(o);
                                        ShoppingMethods.UpdateSkillBasedCareerEarning(base.Target.info.Owner, o);
                                    }
                                    else
                                    {
                                        success = false;
                                        price -= ShoppingMethods.CalculatePrice(o.Value, base.Target.info.PriceIncrease);
                                    }
                                }
                            }

                            ShoppingMethods.PayLotOwner(this.Target, price);
                            base.Actor.Household.SetFamilyFunds(base.Actor.Household.FamilyFunds - price);

                            if (!success)
                            {
                                //  CommonMethodsOFBStand.PrintMessage("Some items couldn't be added to your inventory");
                            }
                        }
                        else
                        {
                            //Can't afford purchase
                            CommonMethodsOFBStand.PrintMessage(CommonMethodsOFBStand.LocalizeString("CantAfford", new object[0]));
                        }
                    }

                    #endregion

                }

                #region Animation End
                this.PostAnimation();
                this.Target.mBuyingSimId = 0uL;
                base.EndCommodityUpdates(true);
                base.StandardExit();

                #endregion

                //Make sim go home or step away from the stand 
                base.TryPushAsContinuation(WonderOff.Singleton);


                return true;
            }          

            private void ShopLoopDelegate(StateMachineClient smc, InteractionInstance.LoopData loopData)
            {
                if (loopData.mLifeTime > OFBStand.kShoppingTime)
                {
                    this.Actor.AddExitReason(ExitReason.StageComplete);
                }
            }
        }

        public class WonderOff : Interaction<Sim, OFBStand>
        {
            private sealed class Definition : InteractionDefinition<Sim, OFBStand, WonderOff>
            {
                public override bool Test(Sim a, OFBStand target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override string GetInteractionName(Sim actor, OFBStand target, InteractionObjectPair iop)
                {
                    return "Wonder off";
                }
            }

            public static readonly InteractionDefinition Singleton = new WonderOff.Definition();
            public override bool Run()
            {
                //Walk away from the stand
                OFBStand[] objects = this.Target.LotCurrent.GetObjects<OFBStand>();

                //Remove this stand from the list
                List<OFBStand> listOfObjects = new List<OFBStand>();

                if (objects != null && objects.Length > 0)
                {
                    for (int i = 0; i < objects.Length; i++)
                    {
                        if (objects[i].ObjectId != this.Target.ObjectId)
                        {
                            listOfObjects.Add(objects[i]);
                        }
                    }
                }

                if (listOfObjects.Count == 0)
                {
                    Sim.MakeSimGoHome(this.Actor, false);
                    return false;
                }
                OFBStand randomObjectFromList = RandomUtil.GetRandomObjectFromList<OFBStand>(listOfObjects);
                if (randomObjectFromList == null)
                {
                    Sim.MakeSimGoHome(this.Actor, false);
                    return false;
                }

                if (!this.Actor.RouteToSlot(randomObjectFromList, Slot.RoutingSlot_0))
                {
                    Sim.MakeSimGoHome(this.Actor, false);
                    return false;
                }


                return true;
            }

        }

        #endregion

        #endregion
    }
}
