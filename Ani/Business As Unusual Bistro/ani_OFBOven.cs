using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Counters;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.SimIFace.CAS;
using System.Collections.Generic;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.CAS;
using System;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Services;
using ani_BistroSet;
using Sims3.Store.Objects;
using System.Text;
using Sims3.UI.CAS;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Controllers;
using Sims3.UI.View;
using Sims3.Gameplay.Objects.Electronics;
using System.Collections;
using Sims3.Gameplay.Situations;

namespace Sims3.Gameplay.Objects.TombObjects.ani_BistroSet
{
    public class OFBOven : IndustrialOven
    {
        [Persistable]
        public static bool AlwaysRandomFood = true;

        public static bool ShowDebugMessages = false;

        public OFBOvenInfo info;
        public List<ulong> Waiters;

        public AlarmHandle mWaiterAlarmWork;
        public AlarmHandle mWaiterJobAlarmEarly;
        public AlarmHandle mWaiterJobAlarmLate;

        //public ulong mManualCook;

        #region Settings
        class ToggleOpenClose : ImmediateInteraction<Sim, OFBOven>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBOven, ToggleOpenClose>
            {
                public override string GetInteractionName(Sim a, OFBOven target, InteractionObjectPair interaction)
                {
                    if (target.info.Open)
                        return CommonMethodsOFBBistroSet.LocalizeString("Close", new object[0]);
                    else
                        return CommonMethodsOFBBistroSet.LocalizeString("Open", new object[0]);
                }

                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new ToggleOpenClose.Definition();

            public override bool Run()
            {
                try
                {
                    base.Target.info.Open = !base.Target.info.Open;
                    base.Target.Cleanup();

                    if (base.Target.info.Open)
                    {
                        //Make next shift active 
                        if (base.Target.info != null)
                        {
                            if (base.Target.info.Shifts != null && base.Target.info.Shifts.Count > 0)
                            {
                                Shift shift = BusinessMethods.ReturnNextValidShift(base.Target, base.Target.info.Shifts);
                                if (shift != null)
                                {
                                    base.Target.mPreferredChef = shift.Cheff.DescriptionId;
                                    base.Target.mOvenHoursStart = shift.StarWork;
                                    base.Target.mOvenHoursEnd = shift.EndWork;

                                    base.Target.Waiters = new List<ulong>();

                                    if (shift.Waiters != null)
                                    {
                                        foreach (Employee e in shift.Waiters)
                                        {
                                            base.Target.Waiters.Add(e.DescriptionId);
                                        }
                                    }

                                    base.Target.RestartAlarmsChef();
                                    base.Target.RestartAlarmsWaiter();
                                }
                                else
                                {
                                    CommonMethodsOFBBistroSet.PrintMessage(CommonMethodsOFBBistroSet.LocalizeString("NoShiftFound", new object[0]));
                                }
                            }
                            else
                            {
                                CommonMethodsOFBBistroSet.PrintMessage(CommonMethodsOFBBistroSet.LocalizeString("NoShiftsAvailable", new object[0]));
                            }
                        }
                    }
                    else
                    {
                        SimDescription cheff = CommonMethodsOFBBistroSet.ReturnSim(base.Target.mPreferredChef);
                        if (cheff != null && cheff.CreatedSim != null)
                        {
                            this.Target.PayChefForTodayIfHaventAlready(cheff.CreatedSim);
                            BusinessMethods.SendEverybodyHome(base.Target, cheff.CreatedSim);
                        }
                        else
                        {
                            //Send staff home manually                            
                            CommonMethodsOFBBistroSet.PrintMessage(CommonMethodsOFBBistroSet.LocalizeString("SendingChefHomeFailed", new object[0]));

                            BusinessMethods.SendEverybodyHome(base.Target, null);
                        }

                        base.Target.mPreferredChef = 0L;
                        base.Target.mOvenHoursStart = -1;
                        base.Target.mOvenHoursEnd = -1;
                        base.Target.Waiters = new List<ulong>();

                    }
                }
                catch (Exception ex)
                {
                    CommonMethodsOFBBistroSet.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class ToggleAlwaysRandom : ImmediateInteraction<Sim, OFBOven>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBOven, ToggleAlwaysRandom>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                CommonMethodsOFBBistroSet.LocalizeString(CommonMethodsOFBBistroSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, OFBOven target, InteractionObjectPair interaction)
                {

                    return CommonMethodsOFBBistroSet.LocalizeString("ToggleRandomFood", new object[] { OFBOven.AlwaysRandomFood.ToString() }); //CommonMethodsOFBOven.LocalizeString("CloseStand", new object[0]);
                }

                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new ToggleAlwaysRandom.Definition();

            public override bool Run()
            {

                OFBOven.AlwaysRandomFood = !OFBOven.AlwaysRandomFood;

                return true;
            }

        }

        class TogglePayOnActive : ImmediateInteraction<Sim, OFBOven>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBOven, TogglePayOnActive>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                CommonMethodsOFBBistroSet.LocalizeString(CommonMethodsOFBBistroSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, OFBOven target, InteractionObjectPair interaction)
                {

                    return CommonMethodsOFBBistroSet.LocalizeString("TogglePayOnActive", new object[] { target.info.PayWhenActive.ToString() });//CommonMethodsOFBOven.LocalizeString("CloseStand", new object[0]);
                }

                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new TogglePayOnActive.Definition();

            public override bool Run()
            {

                this.Target.info.PayWhenActive = !this.Target.info.PayWhenActive;

                return true;
            }

        }

        class CreateShift : ImmediateInteraction<Sim, OFBOven>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBOven, CreateShift>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                CommonMethodsOFBBistroSet.LocalizeString(CommonMethodsOFBBistroSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, OFBOven target, InteractionObjectPair interaction)
                {

                    return CommonMethodsOFBBistroSet.LocalizeString("CreateShift", new object[0]);
                }

                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.info.Open)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CommonMethodsOFBBistroSet.LocalizeString("ShiftsMustBeDisabled", new object[0]));//InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("StandOpen", new object[0]));
                        return false;
                    }

                    return true;

                }
            }

            public static InteractionDefinition Singleton = new CreateShift.Definition();

            public override bool Run()
            {
                try
                {
                    if (base.Target.info == null)
                        base.Target.info = new OFBOvenInfo();

                    Shift shift = new Shift();

                    //Create working hours
                    float.TryParse(CommonMethodsOFBBistroSet.ShowDialogueNumbersOnly(
                        CommonMethodsOFBBistroSet.LocalizeString("ShiftStart", new object[0]),
                        CommonMethodsOFBBistroSet.LocalizeString("StartingHours", new object[0]), "8"), out shift.StarWork);

                    float.TryParse(CommonMethodsOFBBistroSet.ShowDialogueNumbersOnly(
                        CommonMethodsOFBBistroSet.LocalizeString("ShiftEnd", new object[0]),
                       CommonMethodsOFBBistroSet.LocalizeString("EndingHours", new object[0]), "12"), out shift.EndWork);

                    //Create Cheff
                    Employee cheff = new Employee();
                    SimDescription sd = null;
                    List<PhoneSimPicker.SimPickerInfo> chefs = CommonMethodsOFBBistroSet.ReturnUnemployedSims(this.Actor, this.Target.LotCurrent, false, null);//CommonMethodsOFBBistroSet.ReturnSim(base.Actor, true, true);

                    if (chefs == null && OFBOven.ShowDebugMessages)
                        CommonMethodsOFBBistroSet.PrintMessage("chefs null");

                    List<PhoneSimPicker.SimPickerInfo> list3 = DualPaneSimPicker.Show(chefs, new List<PhoneSimPicker.SimPickerInfo>(),
                        CommonMethodsOFBBistroSet.LocalizeString("Chef", new object[0]),
                        CommonMethodsOFBBistroSet.LocalizeString("Unemployed", new object[0]));

                    //Select cheff
                    if (list3 != null && list3.Count > 0)
                        sd = (SimDescription)list3[0].SimDescription;

                    //If cheff is not selected, don't continue
                    if (sd != null)
                    {
                        cheff.DescriptionId = sd.SimDescriptionId;
                        int.TryParse(CommonMethodsOFBBistroSet.ShowDialogueNumbersOnly(
                            CommonMethodsOFBBistroSet.LocalizeString("ChefWageTitle", new object[0]),
                            CommonMethodsOFBBistroSet.LocalizeString("ChefWageDescription", new object[0])
                            , "10"), out cheff.Wage);

                        shift.Cheff = cheff;


                        int waiterPay = 0;
                        int.TryParse(CommonMethodsOFBBistroSet.ShowDialogueNumbersOnly(
                           CommonMethodsOFBBistroSet.LocalizeString("WaiterWageTitle", new object[0]),
                            CommonMethodsOFBBistroSet.LocalizeString("WaiterWageDescription", new object[0]),
                            "10"), out waiterPay);

                        //Create Waiters
                        list3 = new List<PhoneSimPicker.SimPickerInfo>();
                        List<PhoneSimPicker.SimPickerInfo> unemployed = CommonMethodsOFBBistroSet.ReturnUnemployedSims(this.Actor, this.Target.LotCurrent, false, new List<SimDescription>() { sd });//CommonMethodsOFBBistroSet.ReturnSim(base.Actor, true, true);

                        unemployed.Remove(Phone.Call.CreateBasicPickerInfo(this.Actor.SimDescription, sd));

                        list3 = DualPaneSimPicker.Show(unemployed, new List<PhoneSimPicker.SimPickerInfo>(),
                            CommonMethodsOFBBistroSet.LocalizeString("Waiters", new object[0]),
                            CommonMethodsOFBBistroSet.LocalizeString("Unemployed", new object[0]));

                        base.Target.Waiters = new List<ulong>();
                        // List<IMiniSimDescription> waiters = CommonMethodsOFBBistroSet.ShowSimSelector(base.Actor, shift.Cheff.DescriptionId, "Select Waiters");
                        if (list3 != null && list3.Count > 0)
                        {
                            foreach (var w in list3)
                            {
                                Employee e = new Employee();
                                e.DescriptionId = ((SimDescription)w.SimDescription).SimDescriptionId;
                                e.Wage = waiterPay;
                                shift.Waiters.Add(e);
                                base.Target.Waiters.Add(e.DescriptionId);
                            }
                        }

                        //Add to list
                        base.Target.info.Shifts.Add(shift);

                        //Done
                        CommonMethodsOFBBistroSet.PrintMessage(BusinessMethods.ShowShiftInfo(shift, CommonMethodsOFBBistroSet.LocalizeString("ShiftCreated", new object[0]), sd, list3));
                    }
                    else
                    {
                        CommonMethodsOFBBistroSet.PrintMessage(CommonMethodsOFBBistroSet.LocalizeString("CreatingShiftCancelled", new object[0]));
                    }

                }
                catch (Exception ex)
                {
                    CommonMethodsOFBBistroSet.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class EditShift : ImmediateInteraction<Sim, OFBOven>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBOven, EditShift>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                CommonMethodsOFBBistroSet.LocalizeString(CommonMethodsOFBBistroSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, OFBOven target, InteractionObjectPair interaction)
                {

                    return CommonMethodsOFBBistroSet.LocalizeString("EditShift", new object[0]);
                }

                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.info.Open)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CommonMethodsOFBBistroSet.LocalizeString("ShiftsMustBeDisabled", new object[0]));
                        return false;
                    }


                    return true;

                }
            }

            public static InteractionDefinition Singleton = new EditShift.Definition();

            public override bool Run()
            {
                try
                {

                    Shift shift = CommonMethodsOFBBistroSet.ReturnShift(this.Actor, this.Target, 1);

                    if (shift != null)
                    {
                        //Edit
                        float.TryParse(CommonMethodsOFBBistroSet.ShowDialogueNumbersOnly(
                            CommonMethodsOFBBistroSet.LocalizeString("ShiftStart", new object[0]),
                            CommonMethodsOFBBistroSet.LocalizeString("StartingHours", new object[0]),
                            shift.StarWork.ToString()), out shift.StarWork);

                        float.TryParse(CommonMethodsOFBBistroSet.ShowDialogueNumbersOnly(
                            CommonMethodsOFBBistroSet.LocalizeString("ShiftEnd", new object[0]),
                            CommonMethodsOFBBistroSet.LocalizeString("EndingHours", new object[0]),
                            shift.EndWork.ToString()), out shift.EndWork);

                        //Edit Cheff   
                        SimDescription currentCheff = CommonMethodsOFBBistroSet.ReturnSim(shift.Cheff.DescriptionId);

                        if (currentCheff == null && OFBOven.ShowDebugMessages)
                            CommonMethodsOFBBistroSet.PrintMessage("currentChef null");

                        SimDescription sd = null;
                        List<PhoneSimPicker.SimPickerInfo> unemployed = CommonMethodsOFBBistroSet.ReturnUnemployedSims(this.Actor, this.Target.LotCurrent, false, new List<SimDescription>() { currentCheff });

                        List<PhoneSimPicker.SimPickerInfo> list3 = DualPaneSimPicker.Show(unemployed, new List<PhoneSimPicker.SimPickerInfo>() { Phone.Call.CreateBasicPickerInfo(this.Actor.SimDescription, currentCheff) }, CommonMethodsOFBBistroSet.LocalizeString("Chef", new object[0]), CommonMethodsOFBBistroSet.LocalizeString("Unemployed", new object[0]));


                        //Select cheff
                        if (list3 != null && list3.Count > 0)
                            sd = (SimDescription)list3[0].SimDescription;


                        if (sd == null)
                            sd = currentCheff;

                        shift.Cheff.DescriptionId = sd.SimDescriptionId;
                        int.TryParse(CommonMethodsOFBBistroSet.ShowDialogueNumbersOnly(
                            CommonMethodsOFBBistroSet.LocalizeString("ChefWageTitle", new object[0]),
                            CommonMethodsOFBBistroSet.LocalizeString("ChefWageDescription", new object[0]),
                            shift.Cheff.Wage.ToString()), out shift.Cheff.Wage);


                        //Edit Waiters
                        int waiterPay = 0;
                        if (shift.Waiters != null && shift.Waiters.Count > 0)
                            waiterPay = shift.Waiters[0].Wage;

                        int.TryParse(CommonMethodsOFBBistroSet.ShowDialogueNumbersOnly(
                            CommonMethodsOFBBistroSet.LocalizeString("WaiterWageTitle", new object[0]),
                            CommonMethodsOFBBistroSet.LocalizeString("WaiterWageDescription", new object[0]),
                            waiterPay.ToString()), out waiterPay);


                        list3 = new List<PhoneSimPicker.SimPickerInfo>();

                        List<SimDescription> currentStaffDescriptions = new List<SimDescription>();
                        List<PhoneSimPicker.SimPickerInfo> currentStaff = new List<PhoneSimPicker.SimPickerInfo>();

                        if (shift.Waiters != null)
                            foreach (var w in shift.Waiters)
                            {
                                SimDescription wsd = CommonMethodsOFBBistroSet.ReturnSim(w.DescriptionId);
                                if (wsd != null)
                                {
                                    currentStaffDescriptions.Add(wsd);
                                    currentStaff.Add(Phone.Call.CreateBasicPickerInfo(this.Actor.SimDescription, wsd));
                                }
                            }

                        unemployed = CommonMethodsOFBBistroSet.ReturnUnemployedSims(this.Actor, this.Target.LotCurrent, false, currentStaffDescriptions);

                        //Remove Cheff from unemployed 
                        unemployed.Remove(Phone.Call.CreateBasicPickerInfo(this.Actor.SimDescription, sd));

                        list3 = DualPaneSimPicker.Show(unemployed, currentStaff,
                            CommonMethodsOFBBistroSet.LocalizeString("Waiters", new object[0]),
                            CommonMethodsOFBBistroSet.LocalizeString("Unemployed", new object[0]));
                        shift.Waiters = new List<Employee>();
                        if (list3 != null && list3.Count > 0)
                        {
                            foreach (var w in list3)
                            {
                                Employee e = new Employee();
                                e.DescriptionId = ((SimDescription)w.SimDescription).SimDescriptionId;
                                e.Wage = waiterPay;
                                shift.Waiters.Add(e);
                                base.Target.Waiters.Add(e.DescriptionId);
                            }
                        }

                        //Done
                        CommonMethodsOFBBistroSet.PrintMessage(BusinessMethods.ShowShiftInfo(shift, "Shift Edited:", sd, list3));

                    }
                }
                catch (Exception ex)
                {
                    CommonMethodsOFBBistroSet.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        class DeleteShifts : ImmediateInteraction<Sim, OFBOven>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, OFBOven, DeleteShifts>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                CommonMethodsOFBBistroSet.LocalizeString(CommonMethodsOFBBistroSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, OFBOven target, InteractionObjectPair interaction)
                {

                    return CommonMethodsOFBBistroSet.LocalizeString("DeleteShift", new object[0]);
                }

                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.info.Open)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CommonMethodsOFBBistroSet.LocalizeString("ShiftsMustBeDisabled", new object[0]));//InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("StandOpen", new object[0]));
                        return false;
                    }
                    if (target.info == null || (target.info != null && target.info.Shifts.Count == 0))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CommonMethodsOFBBistroSet.LocalizeString("NoShiftsFound", new object[0]));//InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("StandOpen", new object[0]));
                        return false;
                    }

                    return true;

                }
            }

            public static InteractionDefinition Singleton = new DeleteShifts.Definition();

            public override bool Run()
            {
                try
                {

                    Shift shift = CommonMethodsOFBBistroSet.ReturnShift(this.Actor, this.Target, 1);
                    if (base.Target.info.Shifts.Contains(shift))
                        base.Target.info.Shifts.Remove(shift);
                }
                catch (Exception ex)
                {
                    CommonMethodsOFBBistroSet.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        new public class SetMenuChoices : ImmediateInteraction<IActor, OFBOven>
        {
            [DoesntRequireTuning]
            public sealed class Definition : ActorlessInteractionDefinition<IActor, OFBOven, OFBOven.SetMenuChoices>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                CommonMethodsOFBBistroSet.LocalizeString(CommonMethodsOFBBistroSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(IActor a, OFBOven target, InteractionObjectPair interaction)
                {
                    return OFBOven.LocalizeString(false, "SetMenuChoices", new object[0]);
                }
                public override bool Test(IActor a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static InteractionDefinition Singleton = new OFBOven.SetMenuChoices.Definition();
            public override bool Run()
            {
                if (this.Target.mSelectedRecipes == null)
                    this.Target.mSelectedRecipes = new List<MenuRecipeInfo>();

                List<ObjectPicker.RowInfo> allRecipes = CommonMethodsOFBBistroSet.ReturnRecipeholdsAsRowInfo(this.Target.GetAvailableRecipes(this.Target.GetChefSkill()), null);
                List<ObjectPicker.RowInfo> selectedRecipes = CommonMethodsOFBBistroSet.ReturnRecipeholdsAsRowInfo(this.Target.GetAvailableRecipes(this.Target.GetChefSkill()), this.Target.mSelectedRecipes);

                //Remove selected from all recipe
                foreach (ObjectPicker.RowInfo i in selectedRecipes)
                {
                    ObjectPicker.RowInfo dublicate = allRecipes.Find(delegate(ObjectPicker.RowInfo r) { return ((Recipe)r.Item).GenericName.Equals(((Recipe)i.Item).GenericName); });
                    if (dublicate != null)
                        allRecipes.Remove(dublicate);
                }

                selectedRecipes = DualPanelShopping.Show(allRecipes, selectedRecipes, "Selected Menu", "All Recipes");
                List<OFBOven.MenuRecipeInfo> list = new List<MenuRecipeInfo>(selectedRecipes.Count);
                for (int i = 0; i < selectedRecipes.Count; i++)
                {
                    Recipe recipe = selectedRecipes[i].Item as Recipe;
                    if (recipe != null)
                    {
                        int cost = OFBOven.ComputeFoodCost(recipe);
                        OFBOven.MenuRecipeInfo item = new OFBOven.MenuRecipeInfo(recipe.Key, cost, recipe.CookingSkillLevelRequired, recipe.Favorite, recipe.IsVegetarian);
                        list.Add(item);
                    }
                }
                this.Target.SetMenusForAllOvens(list);

                return true;
            }
        }

        new public class SetFoodMarkup : ImmediateInteraction<IActor, OFBOven>
        {
            [DoesntRequireTuning]
            public class Definition : ActorlessInteractionDefinition<IActor, OFBOven, OFBOven.SetFoodMarkup>
            {
                public OFBOven.FoodMarkup mMarkup;
                public float mRate;
                public Definition()
                {
                }
                public Definition(OFBOven.FoodMarkup markup, float rate)
                {
                    this.mMarkup = markup;
                    this.mRate = rate;
                }

                public override string GetInteractionName(IActor a, OFBOven target, InteractionObjectPair interaction)
                {
                    return OFBOven.LocalizeString(false, "Markup" + this.mMarkup, new object[]
					{
						this.mRate * 100f
					});
                }
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
					{
                        CommonMethodsOFBBistroSet.LocalizeString(CommonMethodsOFBBistroSet.MenuSettingsPath, new object[0]),
						OFBOven.LocalizeString(isFemale, "SetFoodMarkup", new object[0]) + Localization.Ellipsis
					};
                }
                public override void AddInteractions(InteractionObjectPair iop, IActor actor, OFBOven target, List<InteractionObjectPair> results)
                {
                    results.Add(new InteractionObjectPair(new OFBOven.SetFoodMarkup.Definition(OFBOven.FoodMarkup.VeryLow, OFBOven.kMarkupVeryLow), iop.Target));
                    results.Add(new InteractionObjectPair(new OFBOven.SetFoodMarkup.Definition(OFBOven.FoodMarkup.Low, OFBOven.kMarkupLow), iop.Target));
                    results.Add(new InteractionObjectPair(new OFBOven.SetFoodMarkup.Definition(OFBOven.FoodMarkup.Regular, OFBOven.kMarkupRegular), iop.Target));
                    results.Add(new InteractionObjectPair(new OFBOven.SetFoodMarkup.Definition(OFBOven.FoodMarkup.High, OFBOven.kMarkupHigh), iop.Target));
                    results.Add(new InteractionObjectPair(new OFBOven.SetFoodMarkup.Definition(OFBOven.FoodMarkup.VeryHigh, OFBOven.kMarkupVeryHigh), iop.Target));
                }
                public override bool Test(IActor a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.mFoodMarkup == this.mMarkup)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(OFBOven.LocalizeString(false, "AlreadyAtThisMarkup", new object[0]));
                        return false;
                    }
                    return true;
                }
            }
            public static InteractionDefinition Singleton = new OFBOven.SetFoodMarkup.Definition();
            public override bool Run()
            {
                OFBOven.SetFoodMarkup.Definition definition = base.InteractionDefinition as OFBOven.SetFoodMarkup.Definition;
                if (definition == null)
                {
                    return false;
                }
                OFBOven[] objects = this.Target.LotCurrent.GetObjects<OFBOven>();
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] != null)
                    {
                        objects[i].mFoodMarkup = definition.mMarkup;
                    }
                }
                StyledNotification.Format format = new StyledNotification.Format(OFBOven.LocalizeString(false, "FoodMarkupNow" + this.Target.mFoodMarkup, new object[]
				{
					definition.mRate * 100f
				}), this.Target.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
                StyledNotification.Show(format);
                return true;
            }
        }

        #endregion Settings

        #region Cheff

        [Persistable]
        new public class DoingChefStuffPosture : Posture
        {
            public Sim Actor;
            public OFBOven Target;
            public override bool PerformIdleLogic
            {
                get
                {
                    return false;
                }
            }
            public override IGameObject Container
            {
                get
                {
                    return this.Target;
                }
            }
            public override string Name
            {
                get
                {
                    return OFBOven.LocalizeString(this.Actor.IsFemale, "CookingPosture", new object[0]);
                }
            }
            public DoingChefStuffPosture()
            {
            }
            public DoingChefStuffPosture(Sim actor, OFBOven target, StateMachineClient swingStateMachine)
                : base(swingStateMachine)
            {
                this.Actor = actor;
                this.Target = target;
                if (this.Target != null)
                {
                    target.AddToUseList(this.Actor);
                }
            }
            public override bool AllowsReactionOverlay()
            {
                return true;
            }
            public override bool AllowsNormalSocials()
            {
                return false;
            }
            public override bool AllowsRouting()
            {
                return true;
            }
            public override void OnInteractionQueueEmpty()
            {
                if (this.Actor != null)
                {
                    try
                    {
                        this.Actor.GreetSimOnLot(this.Target.LotCurrent);
                        InteractionInstance interactionInstance = OFBOven.WorkAsCheff.Singleton.CreateInstance(this.Target, this.Actor, new InteractionPriority(InteractionPriorityLevel.High), false, false);
                        if (interactionInstance != null)
                        {
                            this.Actor.InteractionQueue.Add(interactionInstance);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonMethodsOFBBistroSet.PrintMessage(ex.Message);
                    }
                }
            }
            public override void PopulateInteractions()
            {
            }
            public override float Satisfaction(CommodityKind condition, IGameObject target)
            {
                if ((condition == CommodityKind.Standing || condition == CommodityKind.IsTarget) && (target == this.Actor || target == this.Container))
                {
                    return 1f;
                }
                return 0f;
            }
            public override InteractionInstance GetStandingTransition()
            {
                InteractionInstance headInteraction = this.Actor.InteractionQueue.GetHeadInteraction();
                if (headInteraction is OFBOven.WorkAsCheff || headInteraction is OFBOven.PrepFood || headInteraction is OFBOven.CollectOrders || headInteraction is OFBOven.ServeFood || headInteraction is OFBOven.CancelBeingAChef)
                {
                    return null;
                }
                return OFBOven.CancelBeingAChef.Singleton.CreateInstance(this.Target, this.Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false);
            }
            public override InteractionInstance GetCancelTransition()
            {
                InteractionInstance headInteraction = this.Actor.InteractionQueue.GetHeadInteraction();
                if (headInteraction is OFBOven.WorkAsCheff || headInteraction is OFBOven.PrepFood || headInteraction is OFBOven.CollectOrders || headInteraction is OFBOven.ServeFood || headInteraction is OFBOven.CancelBeingAChef)
                {
                    return null;
                }
                return OFBOven.CancelBeingAChef.Singleton.CreateInstance(this.Target, this.Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false);
            }
            public override void Shoo(bool yield, List<Sim> shooedSims)
            {
                throw new Exception("The method or operation is not implemented.");
            }
            public override ScriptPosture GetSacsPostureParameter()
            {
                return ScriptPosture.NoAnimation;
            }
            public override void OnExitPosture()
            {
                if (this.Target != null)
                {
                    // this.Target.ChangeSimToPreviousOutfit(this.Actor);
                    if (this.Target.IsActorUsingMe(this.Actor))
                    {
                        this.Target.RemoveFromUseList(this.Actor);
                    }
                }
                base.OnExitPosture();
            }
            public override Posture OnReset(IGameObject objectBeingReset)
            {
                if (this.Container == this.Target && this.Target.ActorsUsingMe.Contains(this.Actor))
                {
                    this.Target.RemoveFromUseList(this.Actor);
                }
                if (this.CurrentStateMachine != null)
                {
                    this.CurrentStateMachine.Dispose();
                    this.CurrentStateMachine = null;
                }
                this.Target.mOriginalOutfitCategory = OutfitCategories.None;
                this.Target.mOriginalSimOutfit = null;
                return null;
            }
            public override void AddInteractions(IActor actor, IActor target, List<InteractionObjectPair> results)
            {
                base.AddInteractions(actor, target, results);
            }
        }

        public class WorkAsCheff : Interaction<Sim, OFBOven>
        {
            public class Definition : InteractionDefinition<Sim, OFBOven, OFBOven.WorkAsCheff>
            {
                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override string GetInteractionName(Sim actor, OFBOven target, InteractionObjectPair iop)
                {
                    return OFBOven.LocalizeString(actor.IsFemale, "PrepFood", new object[]
					{
						actor
					});
                }
            }
            public static InteractionDefinition Singleton = new OFBOven.WorkAsCheff.Definition();
            public override bool Run()
            {
                try
                {
                    Simulator.Sleep(0u);
                    this.Target.RestartAlarmsChef();

                    if (this.Actor.Household != null && !this.Actor.Household.IsActive)
                        this.Actor.Motives.MaxEverything();

                    #region Exit work
                    if (this.Actor.SimDescription.SimDescriptionId != this.Target.mPreferredChef || !SimClock.IsTimeBetweenTimes(this.Target.mOvenHoursStart - OFBOven.kChefHoursBeforeWorkToHeadToWork, this.Target.mOvenHoursEnd) || !this.Target.InWorld || this.Target.InInventory)
                    {
                        this.Target.Cleanup();
                        this.Target.PayChefForTodayIfHaventAlready(this.Actor);

                        BusinessMethods.SendEverybodyHome(this.Target, this.Actor);

                        //Make next shift active 
                        Shift shift = BusinessMethods.ReturnNextValidShift(base.Target, base.Target.info.Shifts);
                        base.Target.mPreferredChef = shift.Cheff.DescriptionId;
                        base.Target.mOvenHoursStart = shift.StarWork;
                        base.Target.mOvenHoursEnd = shift.EndWork;

                        base.Target.Waiters = new List<ulong>();

                        foreach (Employee e in shift.Waiters)
                        {
                            base.Target.Waiters.Add(e.DescriptionId);
                        }
                        this.Target.RestartAlarmsChef();
                        this.Target.RestartAlarmsWaiter();

                        return true;
                    }
                    #endregion

                    if (!(this.Actor.Posture is OFBOven.DoingChefStuffPosture))
                    {
                        OFBOven.DoingChefStuffPosture posture = new OFBOven.DoingChefStuffPosture(this.Actor, this.Target, null);
                        this.Actor.Posture = posture;
                    }

                    if (this.Actor.CurrentOutfitCategory != OutfitCategories.Career)
                        BusinessMethods.ChangeToCareerOutfit(this.Actor);


                    this.Actor.GreetSimOnLot(this.Target.LotCurrent);

                    //If no waiters present, the chef will do both jobs.
                    if (base.Target.Waiters == null || (base.Target.Waiters != null && base.Target.Waiters.Count == 0))
                    {
                        if (this.Target.PeopleWaitingForFood(this.Actor) != null && base.TryPushAsContinuation(OFBOven.ServeFood.Singleton))
                        {
                            return true;
                        }
                        if (this.Target.FindBestMenuToGetOrders() != null && base.TryPushAsContinuation(OFBOven.CollectOrders.Singleton))
                        {

                            return true;
                        }
                    }
                    base.TryPushAsContinuation(OFBOven.PrepFood.Singleton);

                }
                catch (Exception ex)
                {
                    this.Actor.PopPosture();

                    if (OFBOven.ShowDebugMessages)
                        CommonMethodsOFBBistroSet.PrintMessage("PostureIdle: " + ex.Message);
                }
                return true;
            }
        }

        new public class PrepFood : Interaction<Sim, OFBOven>, IInteractionPreventsRoutingWithUmbrellaSometimes
        {
            public class Definition : InteractionDefinition<Sim, OFBOven, OFBOven.PrepFood>
            {
                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override string GetInteractionName(Sim actor, OFBOven target, InteractionObjectPair iop)
                {
                    return OFBOven.LocalizeString(actor.IsFemale, "PrepFood", new object[]
					{
						actor
					});
                }
            }
            public const int kBowlToPotEvent = 305;
            public static InteractionDefinition Singleton = new OFBOven.PrepFood.Definition();
            public ObjectGuid mGuidPot = ObjectGuid.InvalidObjectGuid;
            public ObjectGuid mGuidBowl = ObjectGuid.InvalidObjectGuid;
            public ObjectGuid mGuidBoard = ObjectGuid.InvalidObjectGuid;
            public override bool Run()
            {
                try
                {
                    if (this.Actor.LotCurrent != this.Target.LotCurrent && SimClock.IsTimeBetweenTimes(this.Target.mOvenHoursStart, this.Target.mOvenHoursEnd))
                    {
                        World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(this.Target.GetSlotPosition(Slot.RoutingSlot_1));
                        Vector3 destination;
                        Vector3 vector;
                        if (GlobalFunctions.FindGoodLocation(this.Actor, fglParams, out destination, out vector))
                        {
                            Terrain.Teleport(this.Actor, destination);
                        }
                    }
                    Cabinet[] objects = Sims3.Gameplay.Queries.GetObjects<Cabinet>(this.Target.GetSlotPosition(Slot.RoutingSlot_1) + this.Target.GetForwardOfSlot(Slot.RoutingSlot_1), 3f);
                    Route route = this.Actor.CreateRoute();
                    for (int i = 0; i < objects.Length; i++)
                    {
                        route.AddObjectToIgnoreForRoute(objects[i].ObjectId);
                    }
                    route.PlanToSlot(this.Target, Slot.RoutingSlot_1);
                    if (!this.Actor.DoRoute(route))
                    {
                        return false;
                    }

                    // this.Actor.SkillManager.RemoveElement(SkillNames.Cooking);                   
                    base.StandardEntry(false);
                    base.BeginCommodityUpdates();
                    base.EnterStateMachine("IndustrialOven_store", "SimEnter", "x", "oven");
                    base.AddPersistentScriptEventHandler(305u, new SacsEventHandler(this.OnAnimationEvent));
                    this.mGuidPot = GlobalFunctions.CreateProp("Pot", ProductVersion.BaseGame, Vector3.OutOfWorld, 0, Vector3.UnitZ);
                    this.mGuidBowl = GlobalFunctions.CreateProp("BowlLarge", ProductVersion.BaseGame, Vector3.OutOfWorld, 0, Vector3.UnitZ);
                    this.mGuidBoard = GlobalFunctions.CreateProp("CuttingBoard", ProductVersion.BaseGame, Vector3.OutOfWorld, 0, Vector3.UnitZ);
                    GameObject gameObject = GlobalFunctions.ConvertGuidToObject<GameObject>(this.mGuidPot);
                    if (gameObject != null)
                    {
                        gameObject.AddToUseList(this.Actor);
                        base.SetActor("pot", gameObject);
                    }
                    BowlLarge bowlLarge = GlobalFunctions.ConvertGuidToObject<BowlLarge>(this.mGuidBowl);
                    if (bowlLarge != null)
                    {
                        FoodProp foodProp = FoodProp.Create("foodPrepBowlSalad");
                        foodProp.ParentToSlot(bowlLarge, (Slot)2820733094u);
                        bowlLarge.AddToUseList(this.Actor);
                        base.SetActor("mixingBowl", bowlLarge);
                    }
                    GameObject gameObject2 = GlobalFunctions.ConvertGuidToObject<GameObject>(this.mGuidBoard);
                    if (gameObject2 != null)
                    {
                        gameObject2.SetGeometryState("withFood");
                        FoodProp foodProp2 = FoodProp.Create("lettuce#lettuce_half");
                        foodProp2.ParentToSlot(gameObject2, "Chop_Slot");
                        gameObject2.AddToUseList(this.Actor);
                        base.SetActor("cuttingBoard", gameObject2);
                    }
                    base.AnimateSim("PrepFood");
                    bool flag = this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.LoopDelegate), this.mCurrentStateMachine);
                    base.AnimateSim("SimExit");
                    base.EndCommodityUpdates(flag);
                    base.StandardExit(false, false);
                    return flag;
                }
                catch (Exception ex)
                {
                    if (OFBOven.ShowDebugMessages)
                    {
                        if (ex.InnerException != null)
                            CommonMethodsOFBBistroSet.PrintMessage("Preparing food inner: " + ex.InnerException);
                        else if (ex.StackTrace != null)
                            CommonMethodsOFBBistroSet.PrintMessage("Preparing food stack: " + ex.StackTrace);
                        else
                            CommonMethodsOFBBistroSet.PrintMessage("Preparing food message: " + ex.InnerException);
                    }
                    this.Target.RestartAlarmsChef();

                    return false;
                }
            }
            public void OnAnimationEvent(StateMachineClient sender, IEvent evt)
            {
                if (evt != null)
                {
                    uint eventId = evt.EventId;
                    if (eventId != 305u)
                    {
                    }
                }
            }
            public void LoopDelegate(StateMachineClient smc, InteractionInstance.LoopData ld)
            {
                if (this.Actor.Household != null && !this.Actor.Household.IsActive)
                    this.Actor.Motives.MaxEverything();

                if (this.Target.PeopleNeedToOrder())
                {
                    this.Actor.AddExitReason(ExitReason.Finished);
                }
                if (this.Actor.SimDescription.SimDescriptionId != this.Target.mPreferredChef || !SimClock.IsTimeBetweenTimes(this.Target.mOvenHoursStart, this.Target.mOvenHoursEnd))
                {
                    this.Actor.AddExitReason(ExitReason.Finished);
                }
            }
            public override void Cleanup()
            {
                if (this.mGuidBowl != ObjectGuid.InvalidObjectGuid)
                {
                    GameObject gameObject = GlobalFunctions.ConvertGuidToObject<GameObject>(this.mGuidBowl);
                    gameObject.UnParent();
                    gameObject.SetOpacity(0f, 0f);
                    gameObject.Destroy();
                    this.mGuidBowl = ObjectGuid.InvalidObjectGuid;
                }
                if (this.mGuidPot != ObjectGuid.InvalidObjectGuid)
                {
                    GameObject gameObject2 = GlobalFunctions.ConvertGuidToObject<GameObject>(this.mGuidPot);
                    gameObject2.UnParent();
                    gameObject2.SetOpacity(0f, 0f);
                    gameObject2.Destroy();
                    this.mGuidBowl = ObjectGuid.InvalidObjectGuid;
                }
                if (this.mGuidBoard != ObjectGuid.InvalidObjectGuid)
                {
                    GameObject gameObject3 = GlobalFunctions.ConvertGuidToObject<GameObject>(this.mGuidBoard);
                    gameObject3.UnParent();
                    gameObject3.SetOpacity(0f, 0f);
                    gameObject3.Destroy();
                    this.mGuidBoard = ObjectGuid.InvalidObjectGuid;
                }
                base.Cleanup();
            }
            public bool CanStartUsingUmbrella()
            {
                return false;
            }
        }

        new public class CancelBeingAChef : Interaction<Sim, OFBOven>
        {
            public class Definition : InteractionDefinition<Sim, OFBOven, OFBOven.CancelBeingAChef>
            {
                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override string GetInteractionName(Sim actor, OFBOven target, InteractionObjectPair iop)
                {
                    return OFBOven.LocalizeString(actor.IsFemale, "PrepFood", new object[]
					{
						actor
					});
                }
            }
            public static InteractionDefinition Singleton = new OFBOven.CancelBeingAChef.Definition();
            public override bool Run()
            {
                OFBOven.DoingChefStuffPosture doingChefStuffPosture = this.Actor.Posture as OFBOven.DoingChefStuffPosture;
                if (doingChefStuffPosture != null)
                {
                    this.Actor.PopPosture();
                }
                return true;
            }
        }

        #endregion Cheff

        #region Waiter

        public class WorkAsWaiter : Interaction<Sim, OFBOven>
        {
            public class Definition : InteractionDefinition<Sim, OFBOven, OFBOven.WorkAsWaiter>
            {
                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override string GetInteractionName(Sim actor, OFBOven target, InteractionObjectPair iop)
                {
                    if (actor.IsFemale)
                        return CommonMethodsOFBBistroSet.LocalizeString("WorkAsWaiterFemale", new object[0]);
                    else
                        return CommonMethodsOFBBistroSet.LocalizeString("WorkAsWaiterMale", new object[0]);
                }
            }
            public static InteractionDefinition Singleton = new OFBOven.WorkAsWaiter.Definition();
            public override bool Run()
            {
                try
                {
                    Simulator.Sleep(0u);
                    this.Target.RestartAlarmsWaiter();
                    if (this.Actor.Household != null && !this.Actor.Household.IsActive)
                        this.Actor.Motives.MaxEverything();

                    if (!(this.Actor.Posture is OFBOven.DoingWaiterStuffPosture))
                    {
                        OFBOven.DoingWaiterStuffPosture posture = new OFBOven.DoingWaiterStuffPosture(this.Actor, this.Target, null);
                        this.Actor.Posture = posture;
                    }

                    if (this.Actor.CurrentOutfitCategory != OutfitCategories.Career)
                        BusinessMethods.ChangeToCareerOutfit(this.Actor);

                    this.Actor.GreetSimOnLot(this.Target.LotCurrent);

                    if (this.Target.PeopleWaitingForFood(this.Actor) != null && base.TryPushAsContinuation(OFBOven.ServeFood.Singleton))
                    {
                        return true;
                    }

                    if (this.Target.FindBestMenuToGetOrders() != null && base.TryPushAsContinuation(OFBOven.CollectOrders.Singleton))
                    {
                        return true;
                    }

                    //Do idle animations 
                    base.TryPushAsContinuation(OFBOven.DoWaiterIdles.Singleton);

                }
                catch (Exception ex)
                {
                    this.Actor.PopPosture();

                    if (OFBOven.ShowDebugMessages)
                        CommonMethodsOFBBistroSet.PrintMessage("WorkAsWaiter: " + ex.Message);
                }
                return true;
            }
        }

        public class DoWaiterIdles : Interaction<Sim, OFBOven>, IInteractionPreventsRoutingWithUmbrellaSometimes
        {
            public class Definition : InteractionDefinition<Sim, OFBOven, OFBOven.DoWaiterIdles>
            {
                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override string GetInteractionName(Sim actor, OFBOven target, InteractionObjectPair iop)
                {
                    return CommonMethodsOFBBistroSet.LocalizeString("Idle", new object[0]);
                }
            }

            public static InteractionDefinition Singleton = new OFBOven.DoWaiterIdles.Definition();
            public ObjectGuid mGuidPot = ObjectGuid.InvalidObjectGuid;
            public ObjectGuid mGuidBowl = ObjectGuid.InvalidObjectGuid;
            public ObjectGuid mGuidBoard = ObjectGuid.InvalidObjectGuid;
            public override bool Run()
            {
                try
                {
                    //Teleport waiter back on lot
                    if (this.Actor.LotCurrent != this.Target.LotCurrent && SimClock.IsTimeBetweenTimes(this.Target.mOvenHoursStart, this.Target.mOvenHoursEnd))
                    {
                        World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(this.Target.GetSlotPosition(Slot.RoutingSlot_1));
                        Vector3 destination;
                        Vector3 vector;

                        if (GlobalFunctions.FindGoodLocation(this.Actor, fglParams, out destination, out vector))
                            Terrain.Teleport(this.Actor, destination);

                    }
                    //else if (this.Actor.LotCurrent != this.Target.LotCurrent && SimClock.IsTimeBetweenTimes(this.Target.mOvenHoursStart - OFBOven.kChefHoursBeforeWorkToHeadToWork, this.Target.mOvenHoursEnd))
                    //{
                    //    //Route to lot 
                    //    if (this.Actor.RouteToLot(this.Target.LotCurrent.LotId))
                    //        return false;
                    //}

                    //Wander off 
                    int percentage = RandomUtil.GetInt(1, 100);
                    if (percentage <= 50)
                    {
                        if (this.Actor.Wander(ShoppingRegister.TendRegister.kMinWanderDistance, ShoppingRegister.TendRegister.kMaxWanderDistance, false, RouteDistancePreference.NoPreference, false))
                        {
                            //Do idle
                            if (this.Target.LotCurrent.IsActive)
                                CommonMethodsOFBBistroSet.DoRandomIdle(this.Actor, base.GetPriority());
                        }
                    }
                    else
                    {
                        //Do idle
                        if (this.Target.LotCurrent.IsActive)
                            CommonMethodsOFBBistroSet.DoRandomIdle(this.Actor, base.GetPriority());
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    if (OFBOven.ShowDebugMessages)
                    {
                        if (ex.InnerException != null)
                            CommonMethodsOFBBistroSet.PrintMessage("Waiter Idle inner: " + ex.InnerException);
                        else if (ex.StackTrace != null)
                            CommonMethodsOFBBistroSet.PrintMessage("Waiter Idle stack: " + ex.StackTrace);
                        else
                            CommonMethodsOFBBistroSet.PrintMessage("Waiter Idle message: " + ex.InnerException);
                    }

                    this.Target.RestartAlarmsWaiter();

                    return false;
                }
            }
            public void OnAnimationEvent(StateMachineClient sender, IEvent evt)
            {
                if (evt != null)
                {
                    uint eventId = evt.EventId;
                    if (eventId != 305u)
                    {
                    }
                }
            }
            public void LoopDelegate(StateMachineClient smc, InteractionInstance.LoopData ld)
            {
                if (this.Actor.Household != null && !this.Actor.Household.IsActive)
                    this.Actor.Motives.MaxEverything();

                if (this.Target.PeopleNeedToOrder())
                {
                    this.Actor.AddExitReason(ExitReason.Finished);
                }
                if (this.Actor.SimDescription.SimDescriptionId != this.Target.mPreferredChef || !SimClock.IsTimeBetweenTimes(this.Target.mOvenHoursStart, this.Target.mOvenHoursEnd))
                {
                    this.Actor.AddExitReason(ExitReason.Finished);
                }
            }
            public override void Cleanup()
            {

            }
            public bool CanStartUsingUmbrella()
            {
                return false;
            }
        }

        [Persistable]
        public class DoingWaiterStuffPosture : Posture
        {
            public Sim Actor;
            public OFBOven Target;
            public override bool PerformIdleLogic
            {
                get
                {
                    return false;
                }
            }
            public override IGameObject Container
            {
                get
                {
                    return this.Target;
                }
            }
            public override string Name
            {
                get
                {
                    return CommonMethodsOFBBistroSet.LocalizeString("WaitingTablesPosture", new object[0]);
                }
            }
            public DoingWaiterStuffPosture()
            {
            }
            public DoingWaiterStuffPosture(Sim actor, OFBOven target, StateMachineClient swingStateMachine)
                : base(swingStateMachine)
            {
                this.Actor = actor;
                this.Target = target;
                if (this.Target != null)
                {
                    target.AddToUseList(this.Actor);
                }
            }
            public override bool AllowsReactionOverlay()
            {
                return true;
            }
            public override bool AllowsNormalSocials()
            {
                return false;
            }
            public override bool AllowsRouting()
            {
                return true;
            }
            public override void OnInteractionQueueEmpty()
            {
                if (this.Actor != null)
                {
                    try
                    {
                        this.Actor.GreetSimOnLot(this.Target.LotCurrent);
                        InteractionInstance interactionInstance = OFBOven.WorkAsWaiter.Singleton.CreateInstance(this.Target, this.Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false);
                        if (interactionInstance != null)
                        {
                            this.Actor.InteractionQueue.Add(interactionInstance);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonMethodsOFBBistroSet.PrintMessage(ex.Message);
                    }
                }
            }
            public override void PopulateInteractions()
            {
            }
            public override float Satisfaction(CommodityKind condition, IGameObject target)
            {
                if ((condition == CommodityKind.Standing || condition == CommodityKind.IsTarget) && (target == this.Actor || target == this.Container))
                {
                    return 1f;
                }
                return 0f;
            }
            public override InteractionInstance GetStandingTransition()
            {
                InteractionInstance headInteraction = this.Actor.InteractionQueue.GetHeadInteraction();
                if (headInteraction is OFBOven.WorkAsWaiter || headInteraction is OFBOven.CollectOrders || headInteraction is OFBOven.ServeFood || headInteraction is OFBOven.CancelBeingAChef)
                {
                    return null;
                }
                return OFBOven.CancelBeingAChef.Singleton.CreateInstance(this.Target, this.Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false);
            }
            public override InteractionInstance GetCancelTransition()
            {
                InteractionInstance headInteraction = this.Actor.InteractionQueue.GetHeadInteraction();
                if (headInteraction is OFBOven.WorkAsWaiter || headInteraction is OFBOven.PrepFood || headInteraction is OFBOven.CollectOrders || headInteraction is OFBOven.ServeFood || headInteraction is OFBOven.CancelBeingAChef)
                {
                    return null;
                }
                return OFBOven.CancelBeingAChef.Singleton.CreateInstance(this.Target, this.Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false);
            }
            public override void Shoo(bool yield, List<Sim> shooedSims)
            {
                throw new Exception("The method or operation is not implemented.");
            }
            public override ScriptPosture GetSacsPostureParameter()
            {
                return ScriptPosture.NoAnimation;
            }
            public override void OnExitPosture()
            {
                if (this.Target != null)
                {
                    // this.Target.ChangeSimToPreviousOutfit(this.Actor);
                    if (this.Target.IsActorUsingMe(this.Actor))
                    {
                        this.Target.RemoveFromUseList(this.Actor);
                    }
                }
                base.OnExitPosture();
            }
            public override Posture OnReset(IGameObject objectBeingReset)
            {
                if (this.Container == this.Target && this.Target.ActorsUsingMe.Contains(this.Actor))
                {
                    this.Target.RemoveFromUseList(this.Actor);
                }
                if (this.CurrentStateMachine != null)
                {
                    this.CurrentStateMachine.Dispose();
                    this.CurrentStateMachine = null;
                }
                this.Target.mOriginalOutfitCategory = OutfitCategories.None;
                this.Target.mOriginalSimOutfit = null;
                return null;
            }
            public override void AddInteractions(IActor actor, IActor target, List<InteractionObjectPair> results)
            {
                base.AddInteractions(actor, target, results);
            }
        }

        new public class CollectOrders : Interaction<Sim, OFBOven>, IInteractionPreventsRoutingWithUmbrellaSometimes
        {
            public class Definition : InteractionDefinition<Sim, OFBOven, OFBOven.CollectOrders>
            {
                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override string GetInteractionName(Sim actor, OFBOven target, InteractionObjectPair iop)
                {
                    return OFBOven.LocalizeString(actor.IsFemale, "CollectOrders", new object[]
					{
						actor
					});
                }
            }
            public static InteractionDefinition Singleton = new OFBOven.CollectOrders.Definition();
            public override bool Run()
            {
                OFBOven.Menu menu = this.Target.FindBestMenuToGetOrders();
                if (menu == null)
                {
                    return false;
                }

                this.Target.MarkAllTableMenusAsChefEnRoutes(menu, this.Actor);
                if (!this.Actor.RouteToObjectRadius(menu, OFBOven.kChefDistanceFromSimToTakeOrder))
                {
                    this.Target.ClearAllTableMenusAsChefEnRoute(menu, this.Actor);
                    this.Actor.PlayRouteFailure();
                    return false;
                }

                if (menu.mOrderingState != IndustrialOven.Menu.OrderingState.Deciding)
                {
                    return false;
                }

                base.StandardEntry(false);
                base.BeginCommodityUpdates();
                base.EnterStateMachine("IndustrialOven_store", "SimEnter", "x");
                base.AnimateSim("TakeOrders");
                int num = this.Target.MarkAllTableMenusAsOrdered(menu, this.Actor);
                base.DoTimedLoop(OFBOven.kTimeToWaitPerSimOrderingFood * (float)num);
                base.AnimateSim("SimExit");
                base.EndCommodityUpdates(true);
                base.StandardExit(false, false);
                return true;
            }
            public bool CanStartUsingUmbrella()
            {
                return false;
            }
        }

        new public sealed class ServeFood : Interaction<Sim, OFBOven>, IInteractionPreventsRoutingWithUmbrellaSometimes
        {
            public class Definition : InteractionDefinition<Sim, OFBOven, OFBOven.ServeFood>
            {
                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override string GetInteractionName(Sim actor, OFBOven target, InteractionObjectPair iop)
                {
                    return OFBOven.LocalizeString(actor.IsFemale, "ServeFood", new object[]
					{
						actor
					});
                }
            }
            public static InteractionDefinition Singleton = new OFBOven.ServeFood.Definition();
            public IndustrialOven.TrayProp mTray;
            public override bool Run()
            {
                OFBOven.Menu menu = this.Target.PeopleWaitingForFood(this.Actor);

                if (menu == null)
                {
                    return false;
                }
                if (!this.Actor.RouteToSlot(this.Target, Slot.RoutingSlot_0))
                {
                    this.Target.ClearAllTableMenusAsChefEnRoute(menu, this.Actor);
                    this.Actor.PlayRouteFailure();
                    return false;
                }
                if (this.Target.PeopleWaitingForFood(this.Actor) == null)
                {
                    return false;
                }
                base.StandardEntry(false);
                base.BeginCommodityUpdates();
                base.EnterStateMachine("IndustrialOven_store", "SimEnter", "x", "oven");
                this.mTray = (GlobalFunctions.CreateObjectOutOfWorld("accessoryServingTray", (ProductVersion)4294967295u, "Sims3.Store.Objects.IndustrialOven+TrayProp", null) as IndustrialOven.TrayProp);
                base.SetActor("tray", this.mTray);
                base.AnimateSim("GetFoodFromOven");
                base.AnimateSim("SimExit");
                CarrySystem.EnterWhileHolding(this.Actor, this.mTray);
                menu = this.Target.PeopleWaitingForFood(this.Actor);
                bool routingSuccess = this.Actor.RouteToObjectRadius(menu, OFBOven.kChefDistanceFromSimToTakeOrder);
                if (menu == null)
                {
                    CommonMethodsOFBBistroSet.PrintMessage("Route failing? " + menu);

                    CarrySystem.ExitAndKeepHolding(this.Actor);
                    base.EnterStateMachine("IndustrialOven_store", "SimEnter", "x", "oven");
                    base.SetActor("tray", this.mTray);
                    base.AnimateSim("PutAwayTray");
                    base.AnimateSim("SimExit");
                    this.Target.ClearAllTableMenusAsChefEnRoute(menu, this.Actor);
                    this.Actor.PlayRouteFailure();
                }
                else
                {
                    CarrySystem.ExitAndKeepHolding(this.Actor);
                    base.EnterStateMachine("IndustrialOven_store", "SimEnter", "x", "oven");
                    base.SetActor("tray", this.mTray);
                    base.AnimateSim("ServeFoodToTable");
                    this.Target.DeliverFoodToMenuTable(menu, this.Actor);
                    base.AnimateSim("SimExit");
                }
                base.EndCommodityUpdates(true);
                base.StandardExit(false, false);
                return true;
            }
            public override void Cleanup()
            {
                if (this.mTray != null)
                {
                    this.mTray.UnParent();
                    this.mTray.SetOpacity(0f, 0f);
                    this.mTray.Destroy();
                    this.mTray = null;
                }
                base.Cleanup();
            }
            public bool CanStartUsingUmbrella()
            {
                return false;
            }
        }

        #endregion Waiter

        #region Other

        new public class FixQuickMeal : Interaction<Sim, OFBOven>
        {
            public class Definition : InteractionDefinition<Sim, OFBOven, OFBOven.FixQuickMeal>
            {
                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.mPreferredChef != 0uL && SimClock.IsTimeBetweenTimes(target.mOvenHoursStart - OFBOven.kChefHoursBeforeWorkToHeadToWork, target.mOvenHoursEnd))
                    {
                        return false;
                    }
                    bool flag = false;
                    int familyFunds = a.FamilyFunds;
                    List<Recipe> availableRecipes = target.GetAvailableRecipes(OFBOven.kChefSkillExecutiveChef);
                    for (int i = 0; i < availableRecipes.Count; i++)
                    {
                        if (OFBOven.ComputeFoodCost(availableRecipes[i]) <= familyFunds)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(OFBOven.LocalizeString(a.IsFemale, "CannotAfford", new object[]
						{
							a
						}));
                        return false;
                    }
                    return true;
                }
                public override string GetInteractionName(Sim actor, OFBOven target, InteractionObjectPair iop)
                {
                    return OFBOven.LocalizeString(actor.IsFemale, "MakeQuickMeal", new object[]
					{
						actor
					});
                }
                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 1;
                    headers = new List<ObjectPicker.HeaderInfo>();
                    listObjs = new List<ObjectPicker.TabInfo>();
                    OFBOven oven = parameters.Target as OFBOven;
                    if (oven == null)
                    {
                        return;
                    }
                    List<Recipe> availableRecipes = oven.GetAvailableRecipes(OFBOven.kChefSkillExecutiveChef);
                    headers.Add(new ObjectPicker.HeaderInfo("Store/Objects/IndustrialOven:SetRecipeHeader", "Store/Objects/IndustrialOven:SetRecipeHeaderTooltip", 500));
                    List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();
                    int familyFunds = parameters.Actor.FamilyFunds;
                    for (int i = 0; i < availableRecipes.Count; i++)
                    {
                        if (OFBOven.ComputeFoodCost(availableRecipes[i]) <= familyFunds)
                        {
                            List<ObjectPicker.ColumnInfo> list2 = new List<ObjectPicker.ColumnInfo>();
                            list2.Add(new ObjectPicker.ThumbAndTextColumn(availableRecipes[i].GetThumbnailKey(ThumbnailSize.Large), availableRecipes[i].GenericName));
                            ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(availableRecipes[i].Key, list2);
                            list.Add(item);
                        }
                    }
                    ObjectPicker.TabInfo item2 = new ObjectPicker.TabInfo("recipeRowImageName", StringTable.GetLocalizedString("Store/Objects/IndustrialOven/SetMenu:TabText"), list);
                    listObjs.Add(item2);
                }
            }
            public static InteractionDefinition Singleton = new OFBOven.FixQuickMeal.Definition();
            public OFBOven.TrayProp mTray;
            public override bool Run()
            {
                string text = base.GetSelectedObject() as string;
                if (text == null && base.Autonomous)
                {
                    List<Recipe> availableRecipes = this.Target.GetAvailableRecipes(OFBOven.kChefSkillExecutiveChef);
                    int familyFunds = this.Actor.FamilyFunds;
                    List<Recipe> list = new List<Recipe>(availableRecipes.Count);
                    for (int i = 0; i < availableRecipes.Count; i++)
                    {
                        if (OFBOven.ComputeFoodCost(availableRecipes[i]) <= familyFunds)
                        {
                            list.Add(availableRecipes[i]);
                        }
                    }
                    if (list.Count > 0)
                    {
                        Recipe randomObjectFromList = RandomUtil.GetRandomObjectFromList<Recipe>(list);
                        if (randomObjectFromList != null)
                        {
                            text = randomObjectFromList.Key;
                        }
                    }
                }
                if (text == null)
                {
                    return false;
                }
                if (!this.Actor.RouteToSlotAndCheckInUse(this.Target, Slot.RoutingSlot_0))
                {
                    return false;
                }
                
               // this.Actor.SkillManager.AddElement(SkillNames.Cooking);
               
                base.StandardEntry();
                base.BeginCommodityUpdates();
                base.EnterStateMachine("IndustrialOven_store", "SimEnter", "x", "oven");
                this.mTray = (GlobalFunctions.CreateObjectOutOfWorld("accessoryServingTray", (ProductVersion)4294967295u, "Sims3.Store.Objects.IndustrialOven+TrayProp", null) as OFBOven.TrayProp);
                base.SetActor("tray", this.mTray);
                base.AnimateSim("GetFoodFromOven");
                base.AnimateSim("SimExit");
                base.EndCommodityUpdates(true);
                Recipe recipe = Recipe.NameToRecipeHash[text];
                if (recipe == null)
                {
                    return false;
                }
                Quality randomObjectFromList2 = RandomUtil.GetRandomObjectFromList<Quality>(OFBOven.kQualityQuickMeal);
                IFoodContainer foodContainer = recipe.CreateFinishedFood(Recipe.MealQuantity.Single, randomObjectFromList2);
                int num = OFBOven.ComputeFoodCost(recipe);
                this.Actor.ModifyFunds(-num);
                this.mTray.UnParent();
                this.mTray.SetOpacity(0f, 0f);
                foodContainer.ParentToSlot(this.Actor, Sim.ContainmentSlots.RightHand);
                VisualEffect visualEffect = VisualEffect.Create("store_IndustrialOven_foodplace");
                visualEffect.ParentTo(foodContainer, Slot.FXJoint_0);
                visualEffect.SetAutoDestroy(true);
                visualEffect.Start();
                CarrySystem.EnterWhileHolding(this.Actor, foodContainer as ICarryable);
                base.StandardExit();
                InteractionInstance interactionInstance = EatHeldFood.Singleton.CreateInstance(foodContainer, this.Actor, base.GetPriority(), base.Autonomous, base.CancellableByPlayer);
                if (interactionInstance != null)
                {
                    base.TryPushAsContinuation(interactionInstance);
                }
                return true;
            }
            public override void Cleanup()
            {
                if (this.mTray != null)
                {
                    this.mTray.UnParent();
                    this.mTray.SetOpacity(0f, 0f);
                    this.mTray.Destroy();
                    this.mTray = null;
                }
                base.Cleanup();
            }
        }

        #endregion Other

        [RuntimeExport]
        new public class Menu : IndustrialOven.Menu
        {
            [Persistable]
            new public class OrderFood : Interaction<Sim, OFBOven.Menu>
            {
                public DateAndTime mTimeWaitForOtherStarted;

                public class Definition : InteractionDefinition<Sim, OFBOven.Menu, OFBOven.Menu.OrderFood>
                {
                    public override bool Test(Sim a, OFBOven.Menu target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                    {
                        if (target.GetFoodList() == null)
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(OFBOven.LocalizeString(a.IsFemale, "NoMenuSet", new object[]
							{
								a
							}));
                            return false;
                        }
                        if (!target.IsAnySimWorking())
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(OFBOven.LocalizeString(a.IsFemale, "NoChefWorking", new object[]
							{
								a
							}));
                            return false;
                        }
                        int cost = a.FamilyFunds;
                        if (a.TraitManager.HasElement(TraitNames.DiscountDiner))
                        {
                            cost = 2147483647;
                        }
                        if (target.GetFoodsAtOrBelowCost(cost) == null)
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(OFBOven.LocalizeString(a.IsFemale, "CannotAfford", new object[]
							{
								a
							}));
                            return false;
                        }
                        if (target.GetSitData() == null)
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(OFBOven.LocalizeString(a.IsFemale, "NotUsable", new object[]
							{
								a
							}));
                            return false;
                        }
                        return true;
                    }
                    public override string GetInteractionName(Sim actor, OFBOven.Menu target, InteractionObjectPair iop)
                    {
                        return OFBOven.LocalizeString(actor.IsFemale, "OrderFood", new object[]
						{
							actor
						});
                    }
                }
                public static InteractionDefinition Singleton = new OFBOven.Menu.OrderFood.Definition();
                public IFoodContainer mCreatedFood;
                public IGameObject mTable;
                public SlotPair mSlotpair;
                public GameObject mFakeMenu;
                public override bool Run()
                {
                    //Raise the priority
                    this.mPriority.Level = InteractionPriorityLevel.UserDirected;

                    IndustrialOven.MenuRecipeInfo menuRecipeInfo = null;

                    if (OFBOven.AlwaysRandomFood || this.Autonomous || !this.Actor.Household.IsActive)
                    {
                        menuRecipeInfo = this.Target.PickGoodFoodForMe(this.Actor);
                    }
                    else
                    {
                        menuRecipeInfo = BusinessMethods.ReturnSelectedFoodItem(this.Target, this.Actor);
                    }

                    if (!this.Target.IsSimSittingBeforeMenu(this.Actor))
                    {
                        return false;
                    }
                    this.mTable = this.Target.Parent;
                    this.mSlotpair = this.Target.GetFoodSlotPair();
                    if (this.mTable == null)
                    {
                        return false;
                    }

                    //Reading menu animations
                    if (this.Actor.SimDescription.TeenOrAbove)
                    {
                        base.StandardEntry();
                        base.BeginCommodityUpdates();
                        this.Actor.RegisterGroupTalk();
                        base.EnterStateMachine("IndustrialOven_store", "SimEnter", "x", "menu01");
                        SitData sitData = this.Target.GetSitData();
                        if (sitData != null)
                        {
                            IHasScriptProxy hasScriptProxy = sitData.Sittable as IHasScriptProxy;
                            if (hasScriptProxy != null)
                            {
                                base.SetActor("diningchair", hasScriptProxy);
                            }
                            base.SetParameter("EatingPosture", (sitData.SitStyle == SitStyle.Stool) ? EatingPosture.barstoolIn : EatingPosture.diningIn);
                        }

                        this.mFakeMenu = (GameObject)GlobalFunctions.CreateObjectOutOfWorld("industrialOvenMenuClassic", (ProductVersion)4294967295u);
                        this.mFakeMenu.AddToUseList(this.Actor);
                        base.SetActor("menu", this.mFakeMenu);

                        base.AnimateSim("LookAtMenu");
                    }
                    else if (this.Actor.SimDescription.Child)
                    {
                        //No animations for you :(
                    }

                    this.Target.mOrderingState = IndustrialOven.Menu.OrderingState.Deciding;
                    this.Target.mTimeStartedWaiting = SimClock.CurrentTime();
                    bool flag = this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.LookAtMenuLoop), this.mCurrentStateMachine);

                    if (this.Actor.SimDescription.TeenOrAbove)
                        base.AnimateSim("SimExit");

                    if (flag)
                    {
                        this.Actor.ClearExitReasons();
                        this.Actor.LoopIdle();
                        flag = this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.WaitForFoodLoop), this.mCurrentStateMachine);
                    }
                    if (flag)
                    {

                        if (menuRecipeInfo == null)
                            return false;

                        Recipe recipe = null;
                        if (menuRecipeInfo != null)
                        {
                            recipe = menuRecipeInfo.FindRecipe();
                        }
                        if (recipe != null)
                        {
                            Quality foodQuality = this.Target.GetFoodQuality();
                            this.mCreatedFood = recipe.CreateFinishedFood(Recipe.MealQuantity.Single, foodQuality);
                            if (this.mCreatedFood != null)
                            {
                                List<InteractionObjectPair> list = new List<InteractionObjectPair>();
                                for (int i = 0; i < this.mCreatedFood.Interactions.Count; i++)
                                {
                                    if (this.mCreatedFood.Interactions[i].InteractionDefinition.GetType().ToString().Contains("ServingContainer_CleanUp") || this.mCreatedFood.Interactions[i].InteractionDefinition.GetType().ToString().Contains("PutAwayLeftovers") || this.mCreatedFood.Interactions[i].InteractionDefinition.GetType().ToString().Contains("EnhanceFood"))
                                    {
                                        list.Add(this.mCreatedFood.Interactions[i]);
                                    }
                                }
                                for (int j = 0; j < list.Count; j++)
                                {
                                    this.mCreatedFood.Interactions.Remove(list[j]);
                                }
                                this.Target.UnParent();
                                this.Target.SetOpacity(0f, 0f);
                                this.mCreatedFood.ParentToSlot(this.mTable, this.mSlotpair.FoodSlot);
                                VisualEffect visualEffect = VisualEffect.Create("store_industrialOven_foodplace");
                                visualEffect.ParentTo(this.mCreatedFood, Slot.FXJoint_0);
                                visualEffect.SetAutoDestroy(true);
                                visualEffect.Start();
                                this.Target.GiveFoodCostBuffAndDeductMoney(this.Actor, recipe, foodQuality);
                                this.Actor.UnregisterGroupTalk();
                                this.Actor.ClearExitReasons();
                                InteractionInstance interactionInstance = EatHeldFood.Singleton.CreateInstance(this.mCreatedFood, this.Actor, base.GetPriority(), base.Autonomous, base.CancellableByPlayer);
                                if (interactionInstance != null)
                                {
                                    interactionInstance.RunInteraction();
                                }
                                this.mCreatedFood.UnParent();
                                this.mCreatedFood.SetOpacity(0f, 0f);
                                this.mCreatedFood.Destroy();
                                this.mCreatedFood = null;
                                this.Target.ParentToSlot(this.mTable, this.mSlotpair.FoodSlot);
                                this.Target.SetOpacity(1f, 0f);
                            }
                        }
                    }
                    this.Actor.UnregisterGroupTalk();
                    base.EndCommodityUpdates(flag);
                    base.StandardExit();

                    //If not in active family, and has eaten, and is not in situation, send home 
                    if (!this.Actor.Household.IsActive && (this.Actor.BuffManager.HasElement(BuffNames.Stuffed) || this.Actor.BuffManager.HasElement(BuffNames.DivineMeal)
                        || this.Actor.BuffManager.HasElement(BuffNames.Meal)))
                    {
                        bool sendSimHome = true;
                        GroupingSituation situationOfType = this.Actor.GetSituationOfType<GroupingSituation>();
                        if (situationOfType != null)
                        {
                            //Is our sims in this situation                            
                            foreach (Sim s in situationOfType.Participants)
                            {
                                if (s.SimDescription.SimDescriptionId == s.SimDescription.SimDescriptionId)
                                {
                                    sendSimHome = false;
                                    break;
                                }
                            }
                        }

                        if (sendSimHome)
                            Sim.MakeSimGoHome(this.Actor, false);
                    }

                    return flag;
                }
                public void LookAtMenuLoop(StateMachineClient smc, InteractionInstance.LoopData ld)
                {
                    if (this.Target.mOrderingState == IndustrialOven.Menu.OrderingState.WaitingForFood)
                    {
                        this.Actor.AddExitReason(ExitReason.StageComplete);
                    }
                    bool flag = false;

                    if (this.Target.mServingChefSimId == 0uL)
                    {
                        if (this.Target.IsAnySimWorking())
                        {
                            flag = true;
                        }
                    }
                    else
                    {
                        SimDescription simDescription = CommonMethodsOFBBistroSet.ReturnSim(this.Target.mServingChefSimId); //SimDescription.Find(this.Target.mServingChefSimId);
                        if (simDescription != null)
                        {
                            Sim createdSim = simDescription.CreatedSim;
                            if (createdSim != null && (createdSim.Posture is OFBOven.DoingChefStuffPosture || createdSim.Posture is OFBOven.DoingWaiterStuffPosture))
                            {
                                flag = true;
                            }
                        }
                    }
                    if (!flag)
                    {
                        this.Actor.AddExitReason(ExitReason.CanceledByScript);
                    }
                    if (this.Actor.ShouldDoGroupTalk())
                    {
                        this.Actor.DoGroupTalk(null, null, false);
                    }
                }
                public void WaitForFoodLoop(StateMachineClient smc, InteractionInstance.LoopData ld)
                {
                    if (this.Target.mOrderingState == IndustrialOven.Menu.OrderingState.Delivered)
                    {

                        GameObject g = this.Actor.GetObjectInRightHand();

                        if (g != null)
                        {
                            g.UnParent();
                            g.SetOpacity(0f, 0f);
                            g.Destroy();
                        }

                        //Kids don't have menus
                        if (this.Actor.SimDescription.TeenOrAbove)
                        {
                            GameObject menu = this.Actor.GetObjectInLeftHand();
                            if (menu != null)
                            {
                                menu.UnParent();
                                menu.SetOpacity(0f, 0f);
                                this.Actor.ParentToRightHand(menu);
                            }
                        }
                        this.Actor.AddExitReason(ExitReason.StageComplete);
                    }
                    bool flag = false;
                    SimDescription simDescription = SimDescription.Find(this.Target.mServingChefSimId);
                    if (simDescription != null)
                    {
                        Sim createdSim = simDescription.CreatedSim;
                        if (createdSim != null && (createdSim.Posture is OFBOven.DoingChefStuffPosture || createdSim.Posture is OFBOven.DoingWaiterStuffPosture))
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        this.Actor.AddExitReason(ExitReason.CanceledByScript);
                    }

                    //Drink
                    if (this.Actor.SimDescription.TeenOrAbove && this.Actor.GetObjectInRightHand().GetType() != typeof(Glass))
                    {
                        //Switch menu to left hand                        
                        GameObject o = this.Actor.GetObjectInRightHand();
                        o.UnParent();
                        o.SetOpacity(0f, 0f);
                        bool b = this.Actor.ParentToLeftHand(o);


                        Bartending.DrinkGlass glassForMood = Bartending.GetGlassForMood(Bartending.DrinkMood.Regular);
                        Glass glass = (Glass)GlobalFunctions.CreateObjectOutOfWorld(Bartending.GetObjectInstanceNameForGlass(glassForMood), Bartending.GetProductVersionForGlass(glassForMood), "Sims3.Gameplay.Objects.CookingObjects.Glass", new Bartending.GlassInitParameters(this.Actor, Bartending.DrinkMood.Regular, new List<Ingredient>(), false));

                        if (glass != null)
                        {
                            if (this.Actor.ParentToRightHand(glass))
                            {
                                CarrySystem.EnterWhileHolding(this.Actor, glass);
                                DrinkWhileWaiting(this.Actor, glass);
                            }
                            else
                            {
                                glass.Destroy();
                            }
                        }
                    }
                    else
                    {
                        if (this.Actor.ShouldDoGroupTalk())
                        {
                            this.Actor.DoGroupTalk(null, null, false);
                        }
                    }
                }

                private bool DrinkWhileWaiting(Sim sim, Glass item)
                {
                    Posture posture = new Glass.CarryingGlassPosture(sim, item);
                    if (sim.Posture is Sim.StandingPosture)
                    {
                        sim.Posture = posture;
                    }
                    else
                    {
                        posture.PreviousPosture = sim.Posture.PreviousPosture;
                        sim.Posture.PreviousPosture = posture;
                    }

                    //Drinking starts here                   
                    bool flag = true;
                    if (item.Parent == this.Actor)
                    {
                        flag = false;
                    }

                    base.StandardEntry(flag);

                    this.mCurrentStateMachine = StateMachineClient.Acquire(this.Actor, "eat", AnimationPriority.kAPCarryRightPlus);
                    base.SetActor("x", this.Actor);
                    base.SetActor("ServingContainer", item);
                    base.SetActor("thingToEat", item);
                    Counter counter = item.Parent as Counter;
                    if (counter != null)
                    {
                        base.SetActor("Counter", counter);
                        base.SetParameter("IKSuffix", counter.IkSuffix);
                    }
                    this.mCurrentStateMachine.EnterState("x", "Enter");
                    if (this.Actor.Posture != null && this.Actor.Posture.Satisfies(CommodityKind.Sitting, item))
                    {
                        base.SetActor("sitTemplate", this.Actor.Posture.Container);
                        SittingPosture sittingPosture = this.Actor.Posture as SittingPosture;
                        base.SetParameter("sitTemplateSuffix", sittingPosture.Part.Target.IKSuffix);
                    }
                    EatingPosture postureParam = this.GetPostureParam();
                    base.SetParameter("eatPosture", postureParam);
                    bool paramValue = false;

                    base.SetParameter("isFavorite", paramValue);
                    base.SetParameter("isSloppy", this.Actor.HasTrait(TraitNames.Slob));
                    base.SetParameter("isSpoiled", false);
                    base.SetParameter("isIceCream", false);


                    base.SetParameter("UtensilType", UtensilType.hand);

                    EatHeldFood.ObjectHideHelper @object = new EatHeldFood.ObjectHideHelper(item);
                    this.mCurrentStateMachine.AddOneShotScriptEventHandler(101u, new SacsEventHandler(@object.Callback));
                    this.mCurrentStateMachine.AddOneShotScriptEventHandler(105u, new SacsEventHandler(this.ParentFoodToContainer));

                    if (postureParam == EatingPosture.living)
                    {
                        Seat.EnsureLivingChairPosture(this.Actor);
                    }
                    if (sim.SimDescription.TeenOrAbove)
                        base.AnimateSim("Loop Drink Coffee Cup");
                    else if (sim.SimDescription.Child)
                        base.AnimateSim("Loop Drink Glass");

                    base.SetParameter("wasInterrupted", true);
                    base.AddMotiveArrow(CommodityKind.Hunger, true);
                    base.BeginCommodityUpdates();

                    this.Actor.RegisterGroupTalk();
                    bool flag2 = this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.LoopCallback), this.mCurrentStateMachine);

                    this.Actor.UnregisterGroupTalk();
                    base.EndCommodityUpdates(flag2);
                    base.RemoveMotiveDelta(CommodityKind.VampireThirst);
                    base.RemoveMotiveDelta(CommodityKind.BeAZombie);

                    bool paramValue2 = !this.Actor.HasExitReason(ExitReason.Finished);
                    base.SetParameter("wasInterrupted", paramValue2);

                    base.AnimateSim("Exit");

                    if (this.Actor.HasExitReason())
                    {
                        //CommonMethodsOFBBistroSet.PrintMessage("Has exit reason: " + this.Actor.ExitReason.ToString());

                        this.Actor.RemoveExitReason(ExitReason.MoodFailure);
                        if (this.Actor.OnlyHasExitReason(ExitReason.Finished))
                        {
                            if (base.Autonomous && this.Actor.Posture.Container != null && this.Actor.Posture.Container.Parent != null)
                            {
                                IEatingSurface eatingSurface = this.Actor.Posture.Container.Parent as IEatingSurface;
                                if (eatingSurface != null && eatingSurface.AllowWaitForOthers && (eatingSurface.NumOtherSimsAtSurface(this.Actor) > 0 || ((GameObject)eatingSurface).ReferenceList.Count > 0))
                                {
                                    this.Actor.LoopIdle();
                                    this.Actor.RegisterGroupTalk();
                                    mTimeWaitForOtherStarted = SimClock.CurrentTime();
                                    this.Actor.RemoveExitReason(ExitReason.Finished);
                                    this.Actor.TryGroupTalk();
                                    this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.WaitForOthersLoopCallback), null, 5f);
                                    this.Actor.UnregisterGroupTalk();
                                }
                            }

                        }

                    }

                    CarrySystem.ExitCarry(this.Actor);
                    flag = false;
                    base.DestroyObject(item);

                    this.Actor.BuffManager.RemoveElement(BuffNames.MintyBreath);
                    base.StandardExit(flag, flag);

                    return flag2;
                }

                public void LoopCallback(StateMachineClient smc, InteractionInstance.LoopData ld)
                {
                    if (this.Actor.ShouldDoGroupTalk())
                    {
                        this.Actor.DoGroupTalk(new Sim.GroupTalkDelegate(this.PauseSim), new Sim.GroupTalkDelegate(this.UnpauseSim), true);
                    }
                }
                public void PauseSim(bool isSpeaking)
                {
                    if (isSpeaking)
                    {
                        base.AnimateSim("Pause");
                    }
                }
                public void UnpauseSim(bool isSpeaking)
                {
                    if (isSpeaking)
                    {
                        this.mCurrentStateMachine.Return("x");
                    }
                }
                public void WaitForOthersLoopCallback(StateMachineClient smc, InteractionInstance.LoopData ld)
                {
                    if (this.Actor.Posture.Container != null && this.Actor.Posture.Container.Parent != null)
                    {
                        IEatingSurface eatingSurface = this.Actor.Posture.Container.Parent as IEatingSurface;
                        GameObject gameObject = eatingSurface as GameObject;
                        if (eatingSurface.NumOtherSimsAtSurface(this.Actor) == 0 && gameObject.ReferenceList.Count == 0)
                        {
                            this.Actor.AddExitReason(ExitReason.Finished);
                            return;
                        }
                    }
                    if (SimClock.ElapsedTime(TimeUnit.Minutes, this.mTimeWaitForOtherStarted) > EatHeldFood.kNumMinutesToTalkToOthersAfterEating)
                    {
                        this.Actor.AddExitReason(ExitReason.Finished);
                        return;
                    }
                    this.Actor.TryGroupTalk();
                }


                public void ParentFoodToContainer(StateMachineClient smc, IEvent evt)
                {
                    IEdible item = (IEdible)this.Actor.GetObjectInRightHand();
                    if (item != null && !this.Target.HasBeenDestroyed && item.UtensilType == UtensilType.hand && item.ThingToEat != null && !item.ThingToEat.HasBeenDestroyed && item.ThingToEat != this.Target)
                    {
                        item.ThingToEat.ParentToSlot(this.Target, (Slot)2820733094u);
                        ServingContainer servingContainer = item as ServingContainer;
                        FoodProp foodProp = item.ThingToEat as FoodProp;
                        if (foodProp != null && servingContainer != null && servingContainer.CookingProcess != null && servingContainer.CookingProcess.Recipe != null && !servingContainer.HasFoodLeft())
                        {
                            string assetResourceKey = string.Empty;
                            if (servingContainer.CookingProcess.FoodState == FoodCookState.Burnt)
                            {
                                assetResourceKey = servingContainer.CookingProcess.Recipe.ModelsAndMaterials.SingleServingEmptyBurnt;
                            }
                            else
                            {
                                assetResourceKey = servingContainer.CookingProcess.Recipe.ModelsAndMaterials.SingleServingEmpty;
                            }
                            foodProp.InitToModelAndMaterial(assetResourceKey);
                        }
                    }
                }

                public EatingPosture GetPostureParam()
                {
                    if (this.Actor.Posture.Satisfies(CommodityKind.Standing, this.Target))
                    {
                        return EatingPosture.standing;
                    }
                    if (this.Actor.Posture.Satisfies(CommodityKind.Sitting, this.Target))
                    {
                        SitData sitData = null;
                        SittingPosture sittingPosture = this.Actor.Posture as SittingPosture;
                        if (sittingPosture != null)
                        {
                            sitData = sittingPosture.Part.Target;
                        }
                        else
                        {
                            ISittable sittable = SittingHelpers.CastToSittable(this.Actor.Posture.Container as GameObject);
                            if (sittable != null)
                            {
                                PartData partSimIsIn = sittable.PartComponent.GetPartSimIsIn(this.Actor);
                                sitData = (partSimIsIn as SitData);
                            }
                        }
                        if (sitData != null)
                        {
                            switch (sitData.SitStyle)
                            {
                                case SitStyle.Dining:
                                    {
                                        if (this.Target.Parent != this.Actor)
                                        {
                                            return EatingPosture.diningIn;
                                        }
                                        return EatingPosture.diningOut;
                                    }
                                //case SitStyle.Living:
                                //    {
                                //        if (this.Target is HotBeverageMachine.Cup)
                                //        {
                                //            return EatingPosture.living;
                                //        }
                                //        if (this.Target.Parent != this.Actor)
                                //        {
                                //            return EatingPosture.diningIn;
                                //        }
                                //        return EatingPosture.diningOut;
                                //    }
                                case SitStyle.Stool:
                                    {
                                        if (this.Target.Parent != this.Actor)
                                        {
                                            return EatingPosture.barstoolIn;
                                        }
                                        return EatingPosture.barstoolOut;
                                    }
                            }
                        }
                        return EatingPosture.diningOut;
                    }
                    return EatingPosture.standing;
                }


                public override void Cleanup()
                {
                    if (this.mCreatedFood != null)
                    {
                        this.mCreatedFood.UnParent();
                        this.mCreatedFood.SetOpacity(0f, 0f);
                        this.mCreatedFood.Destroy();
                        this.mCreatedFood = null;
                    }
                    if (this.mFakeMenu != null)
                    {
                        this.mFakeMenu.UnParent();
                        this.mFakeMenu.SetOpacity(0f, 0f);
                        this.mFakeMenu.Destroy();
                        this.mFakeMenu = null;
                    }
                    if (this.Target != null && this.mTable != null && this.mSlotpair != null)
                    {
                        if (this.Target.Parent != this.mTable)
                        {
                            this.Target.UnParent();
                            this.Target.ParentToSlot(this.mTable, this.mSlotpair.FoodSlot);
                        }
                        this.Target.SetHiddenFlags(HiddenFlags.Nothing);
                        this.Target.SetOpacity(1f, 0f);
                    }
                    if (base.StandardEntryCalled)
                    {
                        this.Target.mOrderingState = IndustrialOven.Menu.OrderingState.Idle;
                        this.Target.mTimeStartedWaiting = DateAndTime.Invalid;
                        this.Target.mServingChefSimId = 0uL;
                    }

                    base.Cleanup();
                }
            }

            public class AskToDine : ImmediateInteraction<Sim, OFBOven.Menu>
            {

                public class Definition : ImmediateInteractionDefinition<Sim, OFBOven.Menu, OFBOven.Menu.AskToDine>
                {
                    public override bool Test(Sim a, OFBOven.Menu target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                    {
                        if (target.GetFoodList() == null)
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(OFBOven.LocalizeString(a.IsFemale, "NoMenuSet", new object[]
							{
								a
							}));
                            return false;
                        }
                        if (!target.IsAnySimWorking())
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(OFBOven.LocalizeString(a.IsFemale, "NoChefWorking", new object[]
							{
								a
							}));
                            return false;
                        }

                        return true;
                    }
                    public override string GetInteractionName(Sim actor, OFBOven.Menu target, InteractionObjectPair iop)
                    {
                        return CommonMethodsOFBBistroSet.LocalizeString("AskToDine", new object[0]);
                    }

                    public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                    {
                        NumSelectableRows = 1;
                        Sim actor = parameters.Actor as Sim;
                        List<Sim> sims = new List<Sim>();

                        foreach (Sim sim in parameters.Target.LotCurrent.GetSims())
                        {
                            if (sim.SimDescription.ChildOrAbove && sim.Household.FamilyFunds > 10)
                            {
                                sims.Add(sim);
                            }
                        }

                        base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                    }
                }
                public static InteractionDefinition Singleton = new OFBOven.Menu.AskToDine.Definition();

                public override bool Run()
                {
                    List<object> selectedObjects = base.SelectedObjects;
                    if (selectedObjects != null && selectedObjects.Count > 0)
                    {
                        Sim sim = selectedObjects[0] as Sim;
                        InteractionInstance ii = OrderFood.Singleton.CreateInstance(base.Target, sim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true);
                        sim.InteractionQueue.Add(ii);
                    }
                    return true;
                }


            }

            public override void OnCreation()
            {
                base.OnCreation();
            }
            public override void OnStartup()
            {
                base.AddInteraction(OFBOven.Menu.AskToDine.Singleton);
                base.AddInteraction(OFBOven.Menu.OrderFood.Singleton);
            }

            public override bool HandToolChildAllowPlacementInSlot(IGameObject objectWithSlot, Slot slot)
            {
                IEatingSurface eatingSurface = objectWithSlot as IEatingSurface;
                if (eatingSurface != null)
                {
                    SlotPair[] foodSlotPairs = eatingSurface.FoodSlotPairs;
                    if (foodSlotPairs != null)
                    {
                        for (int i = 0; i < foodSlotPairs.Length; i++)
                        {
                            if (foodSlotPairs[i].FoodSlot == slot)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            public override bool HandToolAllowPlacement(ref string errorStr)
            {
                if (base.Parent != null && base.HandToolAllowPlacement(ref errorStr))
                {
                    return true;
                }
                errorStr = OFBOven.LocalizeString(false, "MustPlaceOnTableOrBar", new object[0]);
                return false;
            }
            public override bool IsObjectInFrontOfMe(IGameObject gameObject)
            {
                IEatingSurface eatingSurface = base.Parent as IEatingSurface;
                if (eatingSurface != null)
                {
                    SitData sitData = this.GetSitData();
                    return sitData != null && sitData.Container == gameObject && sitData.Container.PartComponent.GetOtherPart(sitData) == null;
                }
                return base.IsObjectInFrontOfMe(gameObject);
            }

            new public bool IsAnySimWorking()
            {
                OFBOven[] objects = base.LotCurrent.GetObjects<OFBOven>();
                if (objects == null || objects.Length < 1)
                {
                    return false;
                }
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i].mPreferredChef != 0uL)
                    {
                        SimDescription simDescription = SimDescription.Find(objects[i].mPreferredChef);
                        if (simDescription != null)
                        {
                            Sim createdSim = simDescription.CreatedSim;
                            if (createdSim != null)
                            {
                                OFBOven.DoingChefStuffPosture doingChefStuffPosture = createdSim.Posture as OFBOven.DoingChefStuffPosture;
                                if (doingChefStuffPosture != null && doingChefStuffPosture.Target == objects[i] && createdSim.LotCurrent == base.LotCurrent)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
        }

        public override void OnCreation()
        {
            base.OnCreation();
            info = new OFBOvenInfo();
            Waiters = new List<ulong>();
        }
        public override void OnStartup()
        {
            //Settings
            base.AddInteraction(OFBOven.CreateShift.Singleton);
            base.AddInteraction(OFBOven.EditShift.Singleton);
            base.AddInteraction(OFBOven.DeleteShifts.Singleton);
            base.AddInteraction(OFBOven.SetFoodMarkup.Singleton);
            base.AddInteraction(OFBOven.SetMenuChoices.Singleton);
            base.AddInteraction(OFBOven.ToggleAlwaysRandom.Singleton);
            base.AddInteraction(OFBOven.TogglePayOnActive.Singleton);

            //Other
            base.AddInteraction(OFBOven.ToggleOpenClose.Singleton);
            base.AddInteraction(OFBOven.FixQuickMeal.Singleton);

            //Work interactions
            //base.AddInteraction(OFBOven.WorkAsCheff.Singleton);

            //Only restart alarms in this world.
            //WorldType type = GameUtils.GetCurrentWorldType();
            //if(this.
            {
                if (this.info.Open)
                {
                    this.RestartAlarmsChef();
                    this.RestartAlarmsWaiter();
                }
            }
            this.ValidateRecipeList();
        }
        new public void Cleanup()
        {
            this.DeleteAlarmsChef();
            this.DeleteAlarmsWaiter();
        }
        public override void Destroy()
        {
            this.Cleanup();
            base.Destroy();
        }

        public override Tooltip CreateTooltip(Vector2 mousePosition, WindowBase mousedOverWindow, ref Vector2 tooltipPosition)
        {

            string message = CommonMethodsOFBBistroSet.LocalizeString("ShiftDisabled", new object[0]);
            if (this.info.Open)
                message = CommonMethodsOFBBistroSet.LocalizeString("CurrentShift", new object[] { this.mOvenHoursStart.ToString(), this.mOvenHoursEnd.ToString() });

            return new SimpleTextTooltip(message);
        }

        #region Chef Alarms

        public void DeleteAlarmsChef()
        {
            if (this.mChefJobAlarmEarly != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mChefJobAlarmEarly);
                this.mChefJobAlarmEarly = AlarmHandle.kInvalidHandle;
            }
            if (this.mChefJobAlarmLate != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mChefJobAlarmLate);
                this.mChefJobAlarmLate = AlarmHandle.kInvalidHandle;
            }
            if (this.mChefJobFetchMidJobAlarm != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mChefJobFetchMidJobAlarm);
                this.mChefJobFetchMidJobAlarm = AlarmHandle.kInvalidHandle;
            }
        }
        public void RestartAlarmsChef()
        {
            this.DeleteAlarmsChef();
            float hour = SimClock.CurrentTime().Hour;
            float num = this.mOvenHoursStart;
            float num2 = num;// -hour - OFBOven.kChefHoursBeforeWorkToHeadToWork;
            float num3 = num - hour + this.kOneMinuteInHours;
            while (num2 < 0f)
            {
                num2 += 24f;
            }
            while (num3 < 0f)
            {
                num3 += 24f;
            }
            this.mChefJobAlarmEarly = base.AddAlarm(num2, TimeUnit.Hours, new AlarmTimerCallback(this.SummonChef), "Oven chef-summon alarm", AlarmType.AlwaysPersisted);
            this.mChefJobAlarmLate = base.AddAlarm(num3, TimeUnit.Hours, new AlarmTimerCallback(this.SummonChef), "Oven chef-summon alarm", AlarmType.AlwaysPersisted);
            if (SimClock.IsTimeBetweenTimes(this.mOvenHoursStart, this.mOvenHoursEnd))
            {
                this.mChefJobFetchMidJobAlarm = base.AddAlarm(OFBOven.kChefAlarmContinuousSummonFrequncy, TimeUnit.Minutes, new AlarmTimerCallback(this.SummonChef), "Oven chef-summon alarm", AlarmType.AlwaysPersisted);
            }
        }

        new public void SummonChef()
        {
            this.RestartAlarmsChef();
            if (!base.InWorld || base.InInventory)
            {
                return;
            }
            if (this.mPreferredChef == 0uL)
            {
                return;
            }
            SimDescription simDescription = CommonMethodsOFBBistroSet.ReturnSim(this.mPreferredChef);//SimDescription.Find(this.mPreferredChef);

            Sim sim = null;
            if (simDescription != null)
            {
                sim = simDescription.CreatedSim;

                if (sim == null)
                {
                    sim = simDescription.Instantiate(base.LotCurrent);
                    OFBOven.DoingChefStuffPosture posture = new OFBOven.DoingChefStuffPosture(sim, this, null);
                    sim.Posture = posture;
                }
                else if (!(sim.Posture is OFBOven.DoingChefStuffPosture))
                {
                    sim.GreetSimOnLot(base.LotCurrent);
                    sim.InteractionQueue.CancelAllInteractions();
                    InteractionInstance entry = OFBOven.WorkAsCheff.Singleton.CreateInstance(this, sim, new InteractionPriority(InteractionPriorityLevel.High), false, false);
                    sim.InteractionQueue.AddNext(entry);
                }
            }
            else
            {
                CommonMethodsOFBBistroSet.PrintMessage("Couldn't find chef for shift: " + this.mOvenHoursStart + ":00 - " + this.mOvenHoursEnd + ":00");
            }
        }

        #endregion Chef Alarms

        #region Waiter Alarms
        public void DeleteAlarmsWaiter()
        {
            if (this.mWaiterJobAlarmEarly != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mWaiterJobAlarmEarly);
                this.mWaiterJobAlarmEarly = AlarmHandle.kInvalidHandle;
            }
            if (this.mWaiterJobAlarmLate != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mWaiterJobAlarmLate);
                this.mWaiterJobAlarmLate = AlarmHandle.kInvalidHandle;
            }
            if (this.mWaiterAlarmWork != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mWaiterAlarmWork);
                this.mWaiterAlarmWork = AlarmHandle.kInvalidHandle;
            }
        }

        public void RestartAlarmsWaiter()
        {
            this.DeleteAlarmsWaiter();
            float hour = SimClock.CurrentTime().Hour;
            float num = this.mOvenHoursStart;
            float num2 = num;// -hour - OFBOven.kChefHoursBeforeWorkToHeadToWork;
            float num3 = num - hour + this.kOneMinuteInHours;
            while (num2 < 0f)
            {
                num2 += 24f;
            }
            while (num3 < 0f)
            {
                num3 += 24f;
            }
            this.mWaiterJobAlarmEarly = base.AddAlarm(num2, TimeUnit.Hours, new AlarmTimerCallback(this.SummonWaiter), "Oven waiter-summon alarm", AlarmType.AlwaysPersisted);
            this.mWaiterJobAlarmLate = base.AddAlarm(num3, TimeUnit.Hours, new AlarmTimerCallback(this.SummonWaiter), "Oven waiter-summon alarm", AlarmType.AlwaysPersisted);

            if (SimClock.IsTimeBetweenTimes(this.mOvenHoursStart, this.mOvenHoursEnd))
            {
                //for (int i = 0; i < this.mWaiterAlarms.Count; i++)
                {
                    this.mWaiterAlarmWork = base.AddAlarm(OFBOven.kChefAlarmContinuousSummonFrequncy, TimeUnit.Minutes, new AlarmTimerCallback(this.SummonWaiter), "Oven waiter-summon alarm", AlarmType.AlwaysPersisted);
                }
            }
        }

        public void SummonWaiter()
        {
            this.RestartAlarmsWaiter();
            if (!base.InWorld || base.InInventory)
            {
                return;
            }

            if (this.Waiters != null && this.Waiters.Count > 0)
            {
                foreach (ulong id in this.Waiters)
                {
                    SimDescription wd = CommonMethodsOFBBistroSet.ReturnSim(id); ;//SimDescription.Find(id);
                    Sim waiter = null;

                    if (wd != null)
                    {
                        waiter = wd.CreatedSim;

                        //This should not happen
                        if (waiter == null)
                        {
                            waiter = wd.Instantiate(base.LotCurrent);
                            OFBOven.DoingWaiterStuffPosture posture = new OFBOven.DoingWaiterStuffPosture(waiter, this, null);
                            waiter.Posture = posture;

                        }
                        else if (!(waiter.Posture is OFBOven.DoingWaiterStuffPosture))
                        {
                            waiter.GreetSimOnLot(base.LotCurrent);
                            waiter.InteractionQueue.CancelAllInteractions();
                            InteractionInstance entry = OFBOven.WorkAsWaiter.Singleton.CreateInstance(this, waiter, new InteractionPriority(InteractionPriorityLevel.High), false, false);
                            waiter.InteractionQueue.AddNext(entry);
                        }
                    }
                    else
                    {
                        CommonMethodsOFBBistroSet.PrintMessage("Couldn't find one of the waiters in shift: " + this.mOvenHoursStart + ":00 - " + this.mOvenHoursEnd + ":00");
                    }
                }
            }
        }
        #endregion Waiter Alarms

        #region Other Stuff

        new public void PayChefForTodayIfHaventAlready(Sim cheff)
        {
            ulong householdId = 0L;
            Household household = this.FindMoneyGetter();

            if (household != null)
                householdId = household.HouseholdId;

            Shift shift = this.info.Shifts.Find(delegate(Shift s)
            {
                return s.Cheff.DescriptionId == cheff.SimDescription.SimDescriptionId &&
                    s.StarWork == this.mOvenHoursStart && s.EndWork == this.mOvenHoursEnd;
            });

            if (shift != null)
            {
                //Pay staff if active
                //reduce from household if they are active 
                int hoursWorked = 0;

                //Calculate hours
                if (shift.EndWork < shift.StarWork)
                    hoursWorked = (int)(shift.EndWork - shift.StarWork);
                else
                {
                    //hours until midnight
                    hoursWorked = (int)(24 - shift.EndWork);
                    //hours until work ends 
                    hoursWorked += (int)(shift.StarWork);
                }

                int multiplyer = (int)(SimClock.CurrentTime().Hour - shift.StarWork);
                int total = shift.Cheff.Wage * hoursWorked;
                if (this.info.PayWhenActive)
                {
                    if (cheff.Household != null && cheff.Household.IsActive && cheff.Household.HouseholdId != householdId)
                        cheff.Household.ModifyFamilyFunds(shift.Cheff.Wage * hoursWorked);

                    foreach (Employee e in shift.Waiters)
                    {
                        SimDescription sd = CommonMethodsOFBBistroSet.ReturnSim(e.DescriptionId);

                        if (sd != null && sd.Household != null && sd.Household.IsActive && sd.Household.HouseholdId != householdId)
                        {
                            total += e.Wage * hoursWorked;
                            sd.Household.ModifyFamilyFunds(e.Wage * hoursWorked);
                        }

                    }
                    if (household != null && household.IsActive)
                        household.ModifyFamilyFunds(-total);
                }
                else //Always pay
                {
                    if (cheff.Household != null)
                        cheff.Household.ModifyFamilyFunds(shift.Cheff.Wage * hoursWorked);

                    foreach (Employee e in shift.Waiters)
                    {
                        SimDescription sd = CommonMethodsOFBBistroSet.ReturnSim(e.DescriptionId);

                        if (sd != null && sd.Household != null)
                        {
                            total += e.Wage * hoursWorked;
                            sd.Household.ModifyFamilyFunds(e.Wage * hoursWorked);
                        }

                    }
                    if (household != null)
                        household.ModifyFamilyFunds(-total);
                }

            }

        }
        new public Household FindMoneyGetter()
        {
            Household household = base.LotCurrent.Household;
            if (household == null)
            {
                List<PropertyData> list = RealEstateManager.AllPropertiesFromAllHouseholds();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null && base.LotCurrent.LotId == list[i].LotId && list[i].Owner != null && list[i].Owner.OwningHousehold != null)
                    {
                        household = list[i].Owner.OwningHousehold;
                        break;
                    }
                }
            }
            return household;
        }

        new public bool CanBeActiveChef(SimDescription desc)
        {
            if (desc != null)
            {
                Sim activeActor = Sim.ActiveActor;
                return (activeActor == null || activeActor.Household != desc.Household) && ((desc.CreatedSim != null && desc.CreatedSim.Posture is OFBOven.DoingChefStuffPosture) || (desc.IsHuman && !desc.TeenOrBelow && !desc.Elder && !desc.IsServicePerson && !desc.HasActiveRole && desc.AssignedRole == null && desc.Occupation == null && !desc.OccultManager.HasAnyOccultType() && !desc.IsZombie && !desc.IsGhost));
            }
            return false;
        }
        new public bool AnyOneHome(SimDescription desc)
        {
            if (!base.LotCurrent.IsResidentialLot)
            {
                return true;
            }
            if (desc.LotHome == base.LotCurrent)
            {
                return true;
            }
            Household household = base.LotCurrent.Household;
            if (household != null)
            {
                foreach (Sim current in household.Sims)
                {
                    if (current.IsAtHome)
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        new public int GetChefSkill()
        {
            return OFBOven.kChefSkillExecutiveChef;
            //switch (this.mChefQuality)
            //{
            //    case OFBOven.ChefQuality.LineCook:
            //        {
            //            return OFBOven.kChefSkillLineCook;
            //        }
            //    case OFBOven.ChefQuality.SousChef:
            //        {
            //            return OFBOven.kChefSkillSousChef;
            //        }
            //    case OFBOven.ChefQuality.ExecutiveChef:
            //        {
            //            return OFBOven.kChefSkillExecutiveChef;
            //        }
            //    default:
            //        {
            //            return OFBOven.kChefSkillLineCook;
            //        }
            //}
        }
        new public List<Recipe> GetAvailableRecipes(int chefSkill)
        {
            List<Recipe> recipes = Recipe.Recipes;
            List<Recipe> list = new List<Recipe>();
            for (int i = 0; i < recipes.Count; i++)
            {
                if (!recipes[i].IsPetFood &&
                    (recipes[i].AvailableForBreakfast || recipes[i].AvailableForBrunch || recipes[i].AvailableForLunch || recipes[i].AvailableForDinner || recipes[i].Key.Equals("EP9KeyLimePie") || recipes[i].Key.Equals("EP9CheeseDanish") || recipes[i].Key.Equals("GrasshopperPie") || recipes[i].Key.Equals("PulledPork")) &&
                    !(recipes[i].Key == "Ambrosia") && !(recipes[i].Key == "VampireFood") && !(recipes[i].Key == "Wedding Cake") && !(recipes[i].Key == "AlooMasalaCurry") && !(recipes[i].Key == "Ceviche") && !(recipes[i].Key == "ChiliConCarne") && !(recipes[i].Key == "VegetarianChili") && !(recipes[i].Key == "FirecrackerShrimp") && !(recipes[i].Key == "FirecrackerTofu") && !(recipes[i].Key == "HotAndSourSoup") &&
                    recipes[i].CookingSkillLevelRequired <= chefSkill)
                {
                    list.Add(recipes[i]);
                }
            }
            return list;
        }
        new public static int ComputeFoodCost(Recipe r)
        {
            int registerValue = r.RegisterValue;
            int result = r.CalculateCost(new List<Ingredient>(), new List<Ingredient>());
            if (registerValue != 0)
            {
                result = registerValue;
            }
            return result;
        }

        new public bool PeopleNeedToOrder()
        {
            OFBOven.Menu[] objects = base.LotCurrent.GetObjects<OFBOven.Menu>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null && objects[i].mOrderingState == OFBOven.Menu.OrderingState.Deciding && objects[i].mServingChefSimId == 0uL)
                {
                    return true;
                }
            }
            return false;
        }
        new public OFBOven.Menu PeopleWaitingForFood(Sim actor)
        {
            OFBOven.Menu[] objects = base.LotCurrent.GetObjects<OFBOven.Menu>();

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null && objects[i].mServingChefSimId == actor.SimDescription.SimDescriptionId)
                {
                    return objects[i];
                }
            }
            return null;
        }

        public OFBOven.Menu FindBestMenuToGetOrders()
        {
            OFBOven.Menu[] objects = base.LotCurrent.GetObjects<OFBOven.Menu>();
            List<OFBOven.Menu> list = new List<OFBOven.Menu>(objects.Length);
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null && objects[i].mOrderingState == OFBOven.Menu.OrderingState.Deciding && objects[i].mServingChefSimId == 0uL)
                {
                    list.Add(objects[i]);
                }
            }

            if (list.Count != 0)
            {
                list.Sort(new Comparison<OFBOven.Menu>(this.MenuSortByWaitTime));
                return list[0];
            }
            return null;
        }
        public int MenuSortByWaitTime(OFBOven.Menu a, OFBOven.Menu b)
        {
            return a.mTimeStartedWaiting.CompareTo(b.mTimeStartedWaiting);
        }

        public int MarkAllTableMenusAsChefEnRoutes(OFBOven.Menu target, Sim actor)
        {
            if (target == null)
            {
                return 0;
            }
            List<OFBOven.Menu> list = new List<OFBOven.Menu>();
            GameObject gameObject = target.Parent as GameObject;
            if (gameObject != null)
            {
                List<OFBOven.Menu> containedObjectList = gameObject.GetContainedObjectList<OFBOven.Menu>(gameObject.GetContainmentSlots());
                for (int i = 0; i < containedObjectList.Count; i++)
                {
                    if (containedObjectList[i].mOrderingState == OFBOven.Menu.OrderingState.Deciding && containedObjectList[i].mServingChefSimId == 0uL)
                    {
                        list.Add(containedObjectList[i]);
                    }
                }
            }
            else
            {
                list.Add(target);
            }
            for (int j = 0; j < list.Count; j++)
            {
                list[j].mServingChefSimId = actor.SimDescription.SimDescriptionId;
            }
            return list.Count;
        }
        public int ClearAllTableMenusAsChefEnRoute(OFBOven.Menu target, Sim actor)
        {
            if (target == null)
            {
                return 0;
            }
            List<OFBOven.Menu> list = new List<OFBOven.Menu>();
            GameObject gameObject = target.Parent as GameObject;
            if (gameObject != null)
            {
                List<OFBOven.Menu> containedObjectList = gameObject.GetContainedObjectList<OFBOven.Menu>(gameObject.GetContainmentSlots());
                for (int i = 0; i < containedObjectList.Count; i++)
                {
                    if (containedObjectList[i].mOrderingState == OFBOven.Menu.OrderingState.Deciding && containedObjectList[i].mServingChefSimId == actor.SimDescription.SimDescriptionId)
                    {
                        list.Add(containedObjectList[i]);
                    }
                }
            }
            else
            {
                list.Add(target);
            }
            for (int j = 0; j < list.Count; j++)
            {
                list[j].mServingChefSimId = 0uL;
            }
            return list.Count;
        }
        public int MarkAllTableMenusAsOrdered(OFBOven.Menu target, Sim actor)
        {
            List<OFBOven.Menu> list = new List<OFBOven.Menu>();
            GameObject gameObject = target.Parent as GameObject;
            if (gameObject != null)
            {
                List<OFBOven.Menu> containedObjectList = gameObject.GetContainedObjectList<OFBOven.Menu>(gameObject.GetContainmentSlots());
                for (int i = 0; i < containedObjectList.Count; i++)
                {
                    if (containedObjectList[i].mOrderingState == OFBOven.Menu.OrderingState.Deciding)
                    {
                        list.Add(containedObjectList[i]);
                    }
                }
            }
            else
            {
                list.Add(target);
            }
            for (int j = 0; j < list.Count; j++)
            {
                list[j].mOrderingState = OFBOven.Menu.OrderingState.WaitingForFood;
                list[j].mServingChefSimId = actor.SimDescription.SimDescriptionId;
            }
            return list.Count;
        }
        public void DeliverFoodToMenuTable(OFBOven.Menu target, Sim actor)
        {
            List<OFBOven.Menu> list = new List<OFBOven.Menu>();
            GameObject gameObject = target.Parent as GameObject;
            if (gameObject != null)
            {
                List<OFBOven.Menu> containedObjectList = gameObject.GetContainedObjectList<OFBOven.Menu>(gameObject.GetContainmentSlots());
                for (int i = 0; i < containedObjectList.Count; i++)
                {
                    if (containedObjectList[i].mOrderingState == OFBOven.Menu.OrderingState.WaitingForFood && containedObjectList[i].mServingChefSimId == actor.SimDescription.SimDescriptionId)
                    {
                        list.Add(containedObjectList[i]);
                    }
                }
            }
            else
            {
                list.Add(target);
            }

            for (int j = 0; j < list.Count; j++)
            {
                list[j].mOrderingState = OFBOven.Menu.OrderingState.Delivered;
                list[j].mServingChefSimId = 0uL;
                list[j].mTimeStartedWaiting = DateAndTime.Invalid;
                list[j].ReplaceMenuWithFood();
            }
        }
        new public SimDescription HireNewChef()
        {
            CommonMethodsOFBBistroSet.PrintMessage("Trying to hire new cheff");
            return null;
        }

        #endregion Other Stuff

    }
}
