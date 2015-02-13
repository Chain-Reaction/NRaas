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

namespace Sims3.Gameplay.Objects.TombObjects.ani_BistroSet
{
    public class OFBOven : IndustrialOven
    {
        public static bool AlwaysRandomFood = true;
        public OFBOvenInfo info;
        public List<ulong> Waiters;

        public AlarmHandle mWaiterAlarmWork;
        public AlarmHandle mWaiterJobAlarmEarly;
        public AlarmHandle mWaiterJobAlarmLate;

        //public List<AlarmHandle> mWaiterAlarms;
        //public List<AlarmHandle> m

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

                                    base.Target.RestartAlarms();
                                    base.Target.RestartWaiterAlarms();
                                }
                                else
                                {
                                    CommonMethodsOFBBistroSet.PrintMessage("No next shift found");
                                }
                            }
                            else
                            {
                                CommonMethodsOFBBistroSet.PrintMessage("No shifts available");
                            }
                        }
                    }
                    else
                    {
                        base.Target.DeleteAlarms();
                        base.Target.DeleteWaiterAlarms();
                        //Stop tending the stand 

                    }
                }
                catch (Exception ex)
                {
                    CommonMethodsOFBBistroSet.PrintMessage(ex.Message);
                }

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

                    return "Create Shift";//CommonMethodsOFBOven.LocalizeString("CloseStand", new object[0]);
                }

                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.info.Open)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("The Oven must be closed");//InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("StandOpen", new object[0]));
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
                    float.TryParse(CommonMethodsOFBBistroSet.ShowDialogueNumbersOnly("Shift Start", "Enter the starting hour for this shift.", "8"), out shift.StarWork);
                    float.TryParse(CommonMethodsOFBBistroSet.ShowDialogueNumbersOnly("Shift End", "Enter the ending hour for this shift.", "12"), out shift.EndWork);


                    //Create Cheff
                    Employee cheff = new Employee();
                    SimDescription sd = CommonMethodsOFBBistroSet.ReturnSim(base.Actor, true, true);

                    if (sd != null)
                    {
                        cheff.DescriptionId = sd.SimDescriptionId;
                        int.TryParse(CommonMethodsOFBBistroSet.ShowDialogueNumbersOnly("Cheff wage", "The cheff's wage/h.", "10"), out cheff.Wage);

                        shift.Cheff = cheff;
                    }

                    int waiterPay = 0;
                    int.TryParse(CommonMethodsOFBBistroSet.ShowDialogueNumbersOnly("Waiter Wage", "The waiters wage/h.", "10"), out waiterPay);

                    //Create Waiters
                    List<IMiniSimDescription> waiters = CommonMethodsOFBBistroSet.ShowSimSelector(base.Actor, shift.Cheff.DescriptionId, "Select Waiters");
                    if (waiters != null && waiters.Count > 0)
                    {
                        foreach (var w in waiters)
                        {
                            Employee e = new Employee();
                            e.DescriptionId = w.SimDescriptionId;
                            e.Wage = waiterPay;
                            shift.Waiters.Add(e);
                            base.Target.Waiters.Add(e.DescriptionId);
                        }
                    }

                    //Add to list
                    base.Target.info.Shifts.Add(shift);

                    //Done
                    CommonMethodsOFBBistroSet.PrintMessage(BusinessMethods.ShowShiftInfo(shift, sd, waiters));

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

                    return "Delete Shift";//CommonMethodsOFBOven.LocalizeString("CloseStand", new object[0]);
                }

                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.info.Open)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("The Oven must be closed");//InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("StandOpen", new object[0]));
                        return false;
                    }
                    if (target.info == null ||(target.info != null && target.info.Shifts.Count == 0))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("No Shifts to Delete");//InteractionInstance.CreateTooltipCallback(CommonMethodsOFBStand.LocalizeString("StandOpen", new object[0]));
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
                    base.Target.info.Shifts = new List<Shift>();
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
                    List<Recipe> availableRecipes = oven.GetAvailableRecipes(oven.GetChefSkill());
                    headers.Add(new ObjectPicker.HeaderInfo("Store/Objects/IndustrialOven:SetRecipeHeader", "Store/Objects/IndustrialOven:SetRecipeHeaderTooltip", 500));
                    List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();
                    for (int i = 0; i < availableRecipes.Count; i++)
                    {
                        List<ObjectPicker.ColumnInfo> list2 = new List<ObjectPicker.ColumnInfo>();
                        list2.Add(new ObjectPicker.ThumbAndTextColumn(availableRecipes[i].GetThumbnailKey(ThumbnailSize.Large), availableRecipes[i].GenericName));
                        ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(availableRecipes[i], list2);
                        list.Add(item);
                    }
                    ObjectPicker.TabInfo item2 = new ObjectPicker.TabInfo("recipeRowImageName", StringTable.GetLocalizedString("Store/Objects/IndustrialOven/SetMenu:TabText"), list);
                    listObjs.Add(item2);
                    NumSelectableRows = list.Count;
                }
            }
            public static InteractionDefinition Singleton = new OFBOven.SetMenuChoices.Definition();
            public override bool Run()
            {
                if (base.SelectedObjects != null)
                {
                    List<OFBOven.MenuRecipeInfo> list = new List<OFBOven.MenuRecipeInfo>(base.SelectedObjects.Count);
                    for (int i = 0; i < base.SelectedObjects.Count; i++)
                    {
                        Recipe recipe = base.SelectedObjects[i] as Recipe;
                        if (recipe != null)
                        {
                            int cost = OFBOven.ComputeFoodCost(recipe);
                            OFBOven.MenuRecipeInfo item = new OFBOven.MenuRecipeInfo(recipe.Key, cost, recipe.CookingSkillLevelRequired, recipe.Favorite, recipe.IsVegetarian);
                            list.Add(item);
                        }
                    }
                    this.Target.SetMenusForAllOvens(list);
                }
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

        new public class SetChefQuality : ImmediateInteraction<IActor, OFBOven>
        {
            [DoesntRequireTuning]
            public class Definition : ActorlessInteractionDefinition<IActor, OFBOven, OFBOven.SetChefQuality>
            {
                public OFBOven.ChefQuality mQuality;
                public int mCost;
                public Definition()
                {
                }
                public Definition(OFBOven.ChefQuality quality, int cost)
                {
                    this.mQuality = quality;
                    this.mCost = cost;
                }

                public override string GetInteractionName(IActor a, OFBOven target, InteractionObjectPair interaction)
                {
                    return OFBOven.LocalizeString(false, "Hire" + this.mQuality, new object[]
					{
						this.mCost
					});
                }
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
					{
                        CommonMethodsOFBBistroSet.LocalizeString(CommonMethodsOFBBistroSet.MenuSettingsPath, new object[0]),
						OFBOven.LocalizeString(isFemale, "SetChefQuality", new object[0]) + Localization.Ellipsis
					};
                }
                //public override void AddInteractions(InteractionObjectPair iop, IActor actor, IndustrialOven target, List<InteractionObjectPair> results)
                //{
                //    results.Add(new InteractionObjectPair(new IndustrialOven.SetChefQuality.Definition(IndustrialOven.ChefQuality.LineCook, ani_OFBOven.kChefCostLineCook), iop.Target));
                //    results.Add(new InteractionObjectPair(new ani_OFBOven.SetChefQuality.Definition(ani_OFBOven.ChefQuality.SousChef, target..kChefCostSousChef), iop.Target));
                //    results.Add(new InteractionObjectPair(new ani_OFBOven.SetChefQuality.Definition(ani_OFBOven.ChefQuality.ExecutiveChef, ani_OFBOven.kChefCostExecutiveChef), iop.Target));
                //}
                public override bool Test(IActor a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.mChefQuality == this.mQuality)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(OFBOven.LocalizeString(false, "AlreadyAtThisQuality", new object[0]));
                        return false;
                    }
                    return true;
                }
            }
            public static InteractionDefinition Singleton = new OFBOven.SetChefQuality.Definition();
            public override bool Run()
            {
                OFBOven.SetChefQuality.Definition definition = base.InteractionDefinition as OFBOven.SetChefQuality.Definition;
                if (definition == null)
                {
                    return false;
                }
                OFBOven[] objects = this.Target.LotCurrent.GetObjects<OFBOven>();
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] != null)
                    {
                        if (objects[i].mChefQuality > definition.mQuality)
                        {
                            objects[i].mDailyQuitChance += OFBOven.kQuitChanceAfterPayDecreaseBump;
                        }
                        else
                        {
                            objects[i].mDailyQuitChance = 0f;
                        }
                        objects[i].mChefQuality = definition.mQuality;
                    }
                }
                this.Target.VerifyMenuFoodLevelAndUpdateAllOvens();
                StyledNotification.Format format = new StyledNotification.Format(OFBOven.LocalizeString(false, "ChefQualityNow" + this.Target.mChefQuality, new object[0]), this.Target.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
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
                        InteractionInstance interactionInstance = OFBOven.WorkAsCheff.Singleton.CreateInstance(this.Target, this.Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false);
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
                    this.Target.RestartAlarms();
                    this.Actor.Motives.MaxEverything();

                    #region Exit work
                    if (this.Actor.SimDescription.SimDescriptionId != this.Target.mPreferredChef || !SimClock.IsTimeBetweenTimes(this.Target.mOvenHoursStart - OFBOven.kChefHoursBeforeWorkToHeadToWork, this.Target.mOvenHoursEnd) || !this.Target.InWorld || this.Target.InInventory)
                    {
                        this.Actor.PopPosture();

                        this.Target.ChangeSimToPreviousOutfit(this.Actor);

                        if (!this.Actor.IsActiveSim)
                            Sim.MakeSimGoHome(this.Actor, false);

                        //Send waiters home
                        if (base.Target.Waiters != null)
                        {
                            foreach (ulong id in base.Target.Waiters)
                            {
                                SimDescription simDescription = SimDescription.Find(id);
                                if (simDescription != null && simDescription.CreatedSim != null)
                                {
                                    if (!simDescription.CreatedSim.IsActiveSim)
                                    {
                                        simDescription.CreatedSim.SwitchToOutfitWithoutSpin(OutfitCategories.Everyday);
                                        Sim.MakeSimGoHome(simDescription.CreatedSim, false);
                                    }
                                }
                            }
                        }

                        //Make next shift active 
                        if (base.Target.info != null)
                        {
                            Shift shift = BusinessMethods.ReturnNextValidShift(base.Target, base.Target.info.Shifts);
                            base.Target.mPreferredChef = shift.Cheff.DescriptionId;
                            base.Target.mOvenHoursStart = shift.StarWork;
                            base.Target.mOvenHoursEnd = shift.EndWork;

                            base.Target.Waiters = new List<ulong>();

                            foreach (Employee e in shift.Waiters)
                            {
                                base.Target.Waiters.Add(e.DescriptionId);
                            }

                            CommonMethodsOFBBistroSet.PrintMessage("Shift ended\nnext shift: " + shift.StarWork + " - " + shift.EndWork);
                        }

                        return true;
                    }
                    #endregion

                    if (!(this.Actor.Posture is OFBOven.DoingChefStuffPosture))
                    {
                        OFBOven.DoingChefStuffPosture posture = new OFBOven.DoingChefStuffPosture(this.Actor, this.Target, null);
                        this.Actor.Posture = posture;
                    }
                    this.Target.ChangeSimToChefOutfit(this.Actor);
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
                    if (this.Actor.LotCurrent != this.Target.LotCurrent && SimClock.IsTimeBetweenTimes(this.Target.mOvenHoursStart + OFBOven.kChefHoursBeforeWorkToHeadToWork, this.Target.mOvenHoursEnd))
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
                    base.StandardEntry(false);
                    base.BeginCommodityUpdates();
                    this.Target.PayChefForTodayIfHaventAlready(this.Actor);
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
                    CommonMethodsOFBBistroSet.PrintMessage("Preparing food: " + ex.Message);
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

        new public class SetWorkingHours : ImmediateInteraction<IActor, OFBOven>
        {
            [DoesntRequireTuning]
            public class Definition : ActorlessInteractionDefinition<IActor, OFBOven, OFBOven.SetWorkingHours>
            {
                public float mStart;
                public float mEnd;
                public Definition()
                {
                }
                public Definition(float start, float end)
                {
                    this.mStart = start;
                    this.mEnd = end;
                }
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
					{
						OFBOven.LocalizeString(false, "SetHoursPath", new object[0]) + Localization.Ellipsis
					};
                }
                public override string GetInteractionName(IActor a, OFBOven target, InteractionObjectPair interaction)
                {
                    return OFBOven.LocalizeString(false, "WorkHours", new object[]
					{
						this.mStart, 
						this.mEnd
					});
                }
                public override bool Test(IActor a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override void AddInteractions(InteractionObjectPair iop, IActor actor, OFBOven target, List<InteractionObjectPair> results)
                {
                    for (int i = 0; i < OFBOven.kWorkHourStart.Length; i++)
                    {
                        float num = OFBOven.kWorkHourStart[i];
                        float num2;
                        for (num2 = num + OFBOven.kWorkHourDuration; num2 > 24f; num2 -= 24f)
                        {
                        }
                        results.Add(new InteractionObjectPair(new OFBOven.SetWorkingHours.Definition(num, num2), iop.Target));
                    }
                }
            }
            public static InteractionDefinition Singleton = new OFBOven.SetWorkingHours.Definition();
            public override bool Run()
            {
                try
                {
                    float.TryParse(CommonMethodsOFBBistroSet.ShowDialogueNumbersOnly(CommonMethodsOFBBistroSet.LocalizeString("SetStartTime", new object[0]),
                         CommonMethodsOFBBistroSet.LocalizeString("SetStartTimeDescription", new object[0]),
                         base.Target.mOvenHoursStart.ToString()),
                         out base.Target.mOvenHoursStart);
                    float.TryParse(CommonMethodsOFBBistroSet.ShowDialogueNumbersOnly(CommonMethodsOFBBistroSet.LocalizeString("SetEndTime", new object[0]),
                       CommonMethodsOFBBistroSet.LocalizeString("SetEndTimeDescription", new object[0]),
                        base.Target.mOvenHoursEnd.ToString()),
                        out base.Target.mOvenHoursEnd);

                }
                catch (Exception ex)
                {
                    CommonMethodsOFBBistroSet.PrintMessage(ex.Message);
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
                    return "Work as Waiter";
                }
            }
            public static InteractionDefinition Singleton = new OFBOven.WorkAsWaiter.Definition();
            public override bool Run()
            {
                try
                {
                    Simulator.Sleep(0u);
                    this.Target.RestartWaiterAlarms();
                    if (this.Actor.Household != null && !this.Actor.Household.IsActive)
                        this.Actor.Motives.MaxEverything();

                    if (!(this.Actor.Posture is OFBOven.DoingWaiterStuffPosture))
                    {
                        OFBOven.DoingWaiterStuffPosture posture = new OFBOven.DoingWaiterStuffPosture(this.Actor, this.Target, null);
                        this.Actor.Posture = posture;
                    }

                    this.Actor.SwitchToOutfitWithoutSpin(OutfitCategories.Career);
                    this.Actor.GreetSimOnLot(this.Target.LotCurrent);

                    if (this.Target.PeopleWaitingForFood(this.Actor) != null && base.TryPushAsContinuation(OFBOven.ServeFood.Singleton))
                    {
                        CommonMethodsOFBBistroSet.PrintMessage("Success serving food");
                        return true;
                    }

                    if (this.Target.FindBestMenuToGetOrders() != null && base.TryPushAsContinuation(OFBOven.CollectOrders.Singleton))
                    {
                        return true;
                    }
                    //  base.TryPushAsContinuation(OFBOven.PrepFood.Singleton);

                }
                catch (Exception ex)
                {
                    CommonMethodsOFBBistroSet.PrintMessage("WorkAsWaiter: " + ex.Message);
                }
                return true;
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
                    return "waiter posture";//OFBOven.LocalizeString(this.Actor.IsFemale, "CookingPosture", new object[0]);
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

        public class WaiterIdles : Interaction<Sim, OFBOven>, IInteractionPreventsRoutingWithUmbrellaSometimes
        {
            public class Definition : InteractionDefinition<Sim, OFBOven, OFBOven.WaiterIdles>
            {
                public override bool Test(Sim a, OFBOven target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override string GetInteractionName(Sim actor, OFBOven target, InteractionObjectPair iop)
                {
                    return "Waiter Idles";
                }
            }
            public const int kBowlToPotEvent = 305;
            public static InteractionDefinition Singleton = new OFBOven.WaiterIdles.Definition();
            public ObjectGuid mGuidPot = ObjectGuid.InvalidObjectGuid;
            public ObjectGuid mGuidBowl = ObjectGuid.InvalidObjectGuid;
            public ObjectGuid mGuidBoard = ObjectGuid.InvalidObjectGuid;
            public override bool Run()
            {
                try
                {
                    if (this.Actor.LotCurrent != this.Target.LotCurrent && SimClock.IsTimeBetweenTimes(this.Target.mOvenHoursStart + OFBOven.kChefHoursBeforeWorkToHeadToWork, this.Target.mOvenHoursEnd))
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
                    base.StandardEntry(false);
                    base.BeginCommodityUpdates();
                    this.Target.PayChefForTodayIfHaventAlready(this.Actor);
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
                    CommonMethodsOFBBistroSet.PrintMessage("Preparing food: " + ex.Message);
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
                if (menu == null || !this.Actor.RouteToObjectRadius(menu, OFBOven.kChefDistanceFromSimToTakeOrder))
                {
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
                this.Actor.SkillManager.AddElement(SkillNames.Cooking);
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
            //public enum OrderingState
            //{
            //    Idle,
            //    Deciding,
            //    WaitingForFood,
            //    Delivered
            //}
            new public class OrderFood : Interaction<Sim, OFBOven.Menu>
            {
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
                    //public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                    //{
                    //    NumSelectableRows = 1;
                    //    headers = new List<ObjectPicker.HeaderInfo>();
                    //    listObjs = new List<ObjectPicker.TabInfo>();
                    //    OFBOven.Menu menu = parameters.Target as OFBOven.Menu;
                    //    if (menu == null)
                    //    {
                    //        return;
                    //    }
                    //    int cost = parameters.Actor.FamilyFunds;
                    //    if (parameters.Actor.TraitManager.HasElement(TraitNames.DiscountDiner))
                    //    {
                    //        cost = 2147483647;
                    //    }
                    //    List<OFBOven.MenuRecipeInfo> foodsAtOrBelowCost = menu.GetFoodsAtOrBelowCost(cost);
                    //    if (foodsAtOrBelowCost == null)
                    //    {
                    //        return;
                    //    }
                    //    headers.Add(new ObjectPicker.HeaderInfo("Store/Objects/IndustrialOven:SelectRecipeHeader", "Store/Objects/IndustrialOven:SelectRecipeHeaderTooltip", 500));
                    //    List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();
                    //    for (int i = 0; i < foodsAtOrBelowCost.Count; i++)
                    //    {
                    //        Recipe recipe = foodsAtOrBelowCost[i].FindRecipe();
                    //        if (recipe != null)
                    //        {
                    //            List<ObjectPicker.ColumnInfo> list2 = new List<ObjectPicker.ColumnInfo>();
                    //            list2.Add(new ObjectPicker.ThumbAndTextColumn(recipe.GetThumbnailKey(ThumbnailSize.Large), recipe.GenericName));
                    //            ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(foodsAtOrBelowCost[i], list2);
                    //            list.Add(item);
                    //        }
                    //    }
                    //    ObjectPicker.TabInfo item2 = new ObjectPicker.TabInfo("recipeRowImageName", StringTable.GetLocalizedString("Store/Objects/IndustrialOven/SetMenu:TabText"), list);
                    //    listObjs.Add(item2);
                    //}
                }
                public static InteractionDefinition Singleton = new OFBOven.Menu.OrderFood.Definition();
                public IFoodContainer mCreatedFood;
                public IGameObject mTable;
                public SlotPair mSlotpair;
                public GameObject mFakeMenu;
                public override bool Run()
                {
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
                    this.Target.mOrderingState = IndustrialOven.Menu.OrderingState.Deciding;
                    this.Target.mTimeStartedWaiting = SimClock.CurrentTime();
                    bool flag = this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.LookAtMenuLoop), this.mCurrentStateMachine);
                    base.AnimateSim("SimExit");

                    CommonMethodsOFBBistroSet.PrintMessage("Flag: " + flag);

                    if (flag)
                    {
                        this.Actor.ClearExitReasons();
                        this.Actor.LoopIdle();
                        flag = this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.WaitForFoodLoop), this.mCurrentStateMachine);
                    }
                    if (flag)
                    {
                        IndustrialOven.MenuRecipeInfo menuRecipeInfo = this.Target.PickGoodFoodForMe(this.Actor);
                        if (base.SelectedObjects != null && base.SelectedObjects.Count == 1)
                        {
                            menuRecipeInfo = ((base.SelectedObjects[0] as IndustrialOven.MenuRecipeInfo) ?? menuRecipeInfo);
                        }
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
                        SimDescription simDescription = SimDescription.Find(this.Target.mServingChefSimId);
                        if (simDescription != null)
                        {
                            Sim createdSim = simDescription.CreatedSim;
                            if (createdSim != null && (createdSim.Posture is IndustrialOven.DoingChefStuffPosture || createdSim.Posture is OFBOven.DoingWaiterStuffPosture))
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
                        this.Actor.AddExitReason(ExitReason.StageComplete);
                    }
                    bool flag = false;
                    SimDescription simDescription = SimDescription.Find(this.Target.mServingChefSimId);
                    if (simDescription != null)
                    {
                        Sim createdSim = simDescription.CreatedSim;
                        if (createdSim != null && (createdSim.Posture is IndustrialOven.DoingChefStuffPosture ||createdSim.Posture is OFBOven.DoingWaiterStuffPosture))
                        {
                            flag = true;
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

                    CommonMethodsOFBBistroSet.PrintMessage("Clean up menu");

                    base.Cleanup();
                }
            }
            //public OFBOven.Menu.OrderingState mOrderingState;
            //public DateAndTime mTimeStartedWaiting = DateAndTime.Invalid;
            //public ulong mServingChefSimId;
            public override void OnCreation()
            {
                base.OnCreation();
            }
            public override void OnStartup()
            {
                base.AddInteraction(OFBOven.Menu.OrderFood.Singleton);
            }
            //public void ReplaceMenuWithFood()
            //{
            //    if (base.ActorsUsingMe.Count == 1)
            //    {
            //        base.ActorsUsingMe[0].AddExitReason(ExitReason.StageComplete);
            //    }
            //}
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
            //public SlotPair GetFoodSlotPair()
            //{
            //    IEatingSurface eatingSurface = base.Parent as IEatingSurface;
            //    if (eatingSurface != null)
            //    {
            //        SlotPair[] foodSlotPairs = eatingSurface.FoodSlotPairs;
            //        if (foodSlotPairs != null)
            //        {
            //            for (int i = 0; i < foodSlotPairs.Length; i++)
            //            {
            //                if (eatingSurface.GetContainedObject(foodSlotPairs[i].FoodSlot) == this)
            //                {
            //                    return foodSlotPairs[i];
            //                }
            //            }
            //        }
            //    }
            //    return null;
            //}
            //public SitData GetSitData()
            //{
            //    SlotPair foodSlotPair = this.GetFoodSlotPair();
            //    if (foodSlotPair == null)
            //    {
            //        return null;
            //    }
            //    IEatingSurface eatingSurface = base.Parent as IEatingSurface;
            //    if (eatingSurface != null)
            //    {
            //        ISittable sittable = eatingSurface.GetContainedObject(foodSlotPair.ChairSlot) as ISittable;
            //        if (sittable != null && (sittable as IGameObject).InWorld)
            //        {
            //            using (Dictionary<PartArea, PartData>.ValueCollection.Enumerator enumerator = sittable.PartComponent.PartDataList.Values.GetEnumerator())
            //            {
            //                if (enumerator.MoveNext())
            //                {
            //                    return (SitData)enumerator.Current;
            //                }
            //            }
            //        }
            //    }
            //    return null;
            //}
            //public bool IsSimSittingBeforeMenu(Sim actor)
            //{
            //    SitData sitData = this.GetSitData();
            //    return sitData != null && sitData.ContainedSim == actor;
            //}
            //public List<OFBOven.MenuRecipeInfo> GetFoodList()
            //{
            //    OFBOven[] objects = base.LotCurrent.GetObjects<OFBOven>();
            //    if (objects == null || objects.Length < 1 || objects[0].mSelectedRecipes == null || objects[0].mSelectedRecipes.Count == 0)
            //    {
            //        return null;
            //    }
            //    return objects[0].mSelectedRecipes;
            //}
            //public List<OFBOven.MenuRecipeInfo> GetFoodsAtOrBelowCost(int cost)
            //{
            //    List<OFBOven.MenuRecipeInfo> foodList = this.GetFoodList();
            //    if (foodList == null || foodList.Count == 0)
            //    {
            //        return null;
            //    }
            //    List<OFBOven.MenuRecipeInfo> list = new List<OFBOven.MenuRecipeInfo>(foodList.Count);
            //    for (int i = 0; i < foodList.Count; i++)
            //    {
            //        if (foodList[i].Cost <= cost)
            //        {
            //            list.Add(foodList[i]);
            //        }
            //    }
            //    if (list.Count == 0)
            //    {
            //        return null;
            //    }
            //    return list;
            //}
            //public OFBOven.MenuRecipeInfo PickGoodFoodForMe(Sim actor)
            //{
            //    int cost = actor.FamilyFunds;
            //    if (actor.TraitManager.HasElement(TraitNames.DiscountDiner))
            //    {
            //        cost = 2147483647;
            //    }
            //    List<OFBOven.MenuRecipeInfo> foodsAtOrBelowCost = this.GetFoodsAtOrBelowCost(cost);
            //    FavoriteFoodType favoriteFood = actor.SimDescription.FavoriteFood;
            //    if (favoriteFood != FavoriteFoodType.None)
            //    {
            //        for (int i = 0; i < foodsAtOrBelowCost.Count; i++)
            //        {
            //            if (foodsAtOrBelowCost[i].Favorite == favoriteFood)
            //            {
            //                return foodsAtOrBelowCost[i];
            //            }
            //        }
            //    }
            //    if (actor.SimDescription.IsVegetarian)
            //    {
            //        List<OFBOven.MenuRecipeInfo> list = new List<OFBOven.MenuRecipeInfo>(foodsAtOrBelowCost.Count);
            //        for (int j = 0; j < foodsAtOrBelowCost.Count; j++)
            //        {
            //            if (foodsAtOrBelowCost[j].IsVegetarian)
            //            {
            //                list.Add(foodsAtOrBelowCost[j]);
            //            }
            //        }
            //        if (list.Count > 0)
            //        {
            //            return RandomUtil.GetRandomObjectFromList<OFBOven.MenuRecipeInfo>(list);
            //        }
            //    }
            //    return RandomUtil.GetRandomObjectFromList<OFBOven.MenuRecipeInfo>(foodsAtOrBelowCost);
            //}
            //public void GiveFoodCostBuffAndDeductMoney(Sim actor, Recipe recipe, Quality quality)
            //{
            //    OFBOven[] objects = base.LotCurrent.GetObjects<OFBOven>();
            //    if (objects == null || objects.Length < 1)
            //    {
            //        return;
            //    }
            //    bool flag = actor.TraitManager.HasElement(TraitNames.Frugal);
            //    float num = (float)OFBOven.ComputeFoodCost(recipe);
            //    Household household = objects[0].FindMoneyGetter();
            //    int num2 = (int)num;
            //    switch (objects[0].mFoodMarkup)
            //    {
            //        case OFBOven.FoodMarkup.VeryLow:
            //            {
            //                if (quality >= Quality.Nice)
            //                {
            //                    actor.BuffManager.AddElement((BuffNames)5462585240442848368uL, flag ? 25 : 15, Origin.None);
            //                }
            //                num2 = (int)(num * OFBOven.kMarkupVeryLow);
            //                break;
            //            }
            //        case OFBOven.FoodMarkup.Low:
            //            {
            //                if (quality >= Quality.Nice)
            //                {
            //                    actor.BuffManager.AddElement((BuffNames)5462585240442848369uL, flag ? 15 : 5, Origin.None);
            //                }
            //                num2 = (int)(num * OFBOven.kMarkupLow);
            //                break;
            //            }
            //        case OFBOven.FoodMarkup.Regular:
            //            {
            //                num2 = (int)(num * OFBOven.kMarkupRegular);
            //                break;
            //            }
            //        case OFBOven.FoodMarkup.High:
            //            {
            //                if (quality <= Quality.Excellent)
            //                {
            //                    actor.BuffManager.AddElement((BuffNames)5462585240442848370uL, flag ? -15 : -5, Origin.None);
            //                }
            //                num2 = (int)(num * OFBOven.kMarkupHigh);
            //                break;
            //            }
            //        case OFBOven.FoodMarkup.VeryHigh:
            //            {
            //                if (quality <= Quality.Excellent)
            //                {
            //                    actor.BuffManager.AddElement((BuffNames)5462585240442848371uL, flag ? -25 : -15, Origin.None);
            //                }
            //                num2 = (int)(num * OFBOven.kMarkupVeryHigh);
            //                break;
            //            }
            //    }
            //    if (num2 > actor.FamilyFunds)
            //    {
            //        num2 = actor.FamilyFunds;
            //    }
            //    if (!actor.TraitManager.HasElement(TraitNames.DiscountDiner))
            //    {
            //        actor.ModifyFunds(-num2);
            //    }
            //    if (household != null && actor.Household != household)
            //    {
            //        household.ModifyFamilyFunds(num2);
            //    }
            //}
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
            base.AddInteraction(OFBOven.DeleteShifts.Singleton);
            base.AddInteraction(OFBOven.SetFoodMarkup.Singleton);
            base.AddInteraction(OFBOven.SetChefQuality.Singleton);
            base.AddInteraction(OFBOven.SetMenuChoices.Singleton);

            //Other
            base.AddInteraction(OFBOven.ToggleOpenClose.Singleton);
            base.AddInteraction(OFBOven.FixQuickMeal.Singleton);

            //Work interactions

            //base.AddInteraction(OFBOven.SetWorkingHours.Singleton);

            this.RestartAlarms();
            this.RestartWaiterAlarms();
            this.ValidateRecipeList();
        }
        new public void Cleanup()
        {
            this.DeleteAlarms();
            this.DeleteWaiterAlarms();
        }
        public override void Destroy()
        {
            this.Cleanup();
            base.Destroy();
        }

        #region Chef Alarms

        new public void DeleteAlarms()
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
        new public void RestartAlarms()
        {
            this.DeleteAlarms();
            float hour = SimClock.CurrentTime().Hour;
            float num = this.mOvenHoursStart;
            float num2 = num - hour - OFBOven.kChefHoursBeforeWorkToHeadToWork;
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
            this.RestartAlarms();
            if (!base.InWorld || base.InInventory)
            {
                return;
            }
            if (this.mPreferredChef == 0uL)
            {
                return;
            }
            SimDescription simDescription = SimDescription.Find(this.mPreferredChef);

            Sim sim = null;
            if (simDescription == null)
            {
                foreach (SimDescription sd in SimDescription.GetAll(SimDescription.Repository.Household))
                {
                    if (sd.SimDescriptionId == this.mPreferredChef)
                    {
                        simDescription = sd;
                        sim = sd.CreatedSim;

                        break;
                    }
                }
            }


            if (sim == null)
            {
                sim = simDescription.Instantiate(base.LotCurrent);
                OFBOven.DoingChefStuffPosture posture = new OFBOven.DoingChefStuffPosture(sim, this, null);
                sim.Posture = posture;
            }
            if (sim == null)
            {
                CommonMethodsOFBBistroSet.PrintMessage("Chef: " + simDescription.FullName + " couldn't be found. Shift cancelled.");
                this.DeleteAlarms();
                this.DeleteWaiterAlarms();

                //TODO:Find next shift and make active

                return;
            }
            if (!(sim.Posture is OFBOven.DoingChefStuffPosture))
            {
                this.mOriginalOutfitCategory = OutfitCategories.None;
                this.mOriginalSimOutfit = null;
                sim.GreetSimOnLot(base.LotCurrent);
                InteractionInstance entry = OFBOven.WorkAsCheff.Singleton.CreateInstance(this, sim, new InteractionPriority(InteractionPriorityLevel.High), false, false);
                sim.InteractionQueue.AddNext(entry);
            }

            ////Waiters
            //if (this.Waiters != null && this.Waiters.Count > 0)
            //{
            //    foreach (ulong id in this.Waiters)
            //    {
            //        SimDescription wd = SimDescription.Find(id);
            //        Sim waiter = null;

            //        if (wd == null)
            //        {
            //            foreach (SimDescription sd in SimDescription.GetAll(SimDescription.Repository.Household))
            //            {
            //                if (sd.SimDescriptionId == id)
            //                {
            //                    wd = sd;
            //                    waiter = wd.CreatedSim;
            //                    break;
            //                }
            //            }
            //        }

            //        if (wd != null)
            //        {
            //            if (waiter == null)
            //            {
            //                waiter = wd.Instantiate(base.LotCurrent);
            //            }

            //            if (waiter != null)
            //            {
            //                InteractionInstance entry = OFBOven.WorkAsWaiter.Singleton.CreateInstance(this, waiter, new InteractionPriority(InteractionPriorityLevel.High), false, false);
            //                waiter.InteractionQueue.AddNext(entry);
            //            }
            //            else
            //            {
            //                CommonMethodsOFBBistroSet.PrintMessage("Couldn't find waiter: " + wd.FullName);
            //            }
            //        }
            //        else
            //        {
            //            CommonMethodsOFBBistroSet.PrintMessage("Couldn't find one of the waiters in shift: " + this.mOvenHoursStart + ":00 - " + this.mOvenHoursEnd + ":00");
            //        }
            //    }
            //}
        }

        #endregion Chef Alarms

        #region Waiter Alarms
        public void DeleteWaiterAlarms()
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

        public void RestartWaiterAlarms()
        {
            this.DeleteWaiterAlarms();
            float hour = SimClock.CurrentTime().Hour;
            float num = this.mOvenHoursStart;
            float num2 = num - hour - OFBOven.kChefHoursBeforeWorkToHeadToWork;
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
            this.RestartWaiterAlarms();
            if (!base.InWorld || base.InInventory)
            {
                return;
            }

            if (this.Waiters != null && this.Waiters.Count > 0)
            {
                foreach (ulong id in this.Waiters)
                {
                    SimDescription wd = SimDescription.Find(id);
                    Sim waiter = null;

                    if (wd == null)
                    {
                        foreach (SimDescription sd in SimDescription.GetAll(SimDescription.Repository.Household))
                        {
                            if (sd.SimDescriptionId == id)
                            {
                                wd = sd;
                                waiter = wd.CreatedSim;
                                break;
                            }
                        }
                    }

                    if (wd != null)
                    {
                        if (waiter == null)
                        {
                            waiter = wd.Instantiate(base.LotCurrent);
                            OFBOven.DoingWaiterStuffPosture posture = new OFBOven.DoingWaiterStuffPosture(waiter, this, null);
                            waiter.Posture = posture;
                        }

                        if (waiter != null)
                        {
                            InteractionInstance entry = OFBOven.WorkAsWaiter.Singleton.CreateInstance(this, waiter, new InteractionPriority(InteractionPriorityLevel.High), false, false);
                            waiter.InteractionQueue.AddNext(entry);
                        }
                        else
                        {
                            CommonMethodsOFBBistroSet.PrintMessage("Couldn't find waiter: " + wd.FullName);
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

        new public void PayChefForTodayIfHaventAlready(Sim actor)
        {

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
        new public void ChangeSimToChefOutfit(Sim s)
        {
            if (this.mOriginalSimOutfit != null || s.CurrentOutfitCategory == OutfitCategories.Singed || s.Service is GrimReaper || s.SimDescription.TeenOrBelow)
            {
                return;
            }
            SimDescription simDescription = s.SimDescription;
            string name = "career_execchef_" + (s.IsFemale ? "female" : "male") + (simDescription.Elder ? "elder" : "");
            ResourceKey key = ResourceKey.CreateOutfitKeyFromProductVersion(name, ProductVersion.BaseGame);
            SimOutfit simOutfit = OutfitUtils.CreateOutfitForSim(simDescription, key, OutfitCategories.Career, true);
            if (simOutfit != null)
            {
                if (s.CurrentOutfit.Key == simOutfit.Key)
                {
                    return;
                }
                this.mOriginalSimOutfit = s.CurrentOutfit;
                this.mOriginalOutfitCategory = s.CurrentOutfitCategory;
                Sim.SwitchOutfitHelper switchOutfitHelper = new Sim.SwitchOutfitHelper(s, simOutfit.Key);
                if (switchOutfitHelper != null)
                {
                    switchOutfitHelper.Start();
                    switchOutfitHelper.Wait(true);
                    s.SwitchToOutfitWithSpin(simOutfit.Key);
                    switchOutfitHelper.Dispose();
                }
            }
        }
        new public void ChangeSimToPreviousOutfit(Sim s)
        {
            if (this.mOriginalSimOutfit == null || s.CurrentOutfitCategory == OutfitCategories.Singed)
            {
                return;
            }
            if (s.CurrentOutfitCategory == OutfitCategories.Career)
            {
                s.SimDescription.RemoveOutfit(OutfitCategories.Career, s.CurrentOutfitIndex, true);
                if (s.SimDescription.HasOutfit(this.mOriginalOutfitCategory, this.mOriginalSimOutfit.Key) >= 0)
                {
                    Sim.SwitchOutfitHelper switchOutfitHelper = new Sim.SwitchOutfitHelper(s, this.mOriginalSimOutfit.Key);
                    if (switchOutfitHelper != null)
                    {
                        switchOutfitHelper.Start();
                        switchOutfitHelper.Wait(true);
                        s.SwitchToOutfitWithSpin(this.mOriginalSimOutfit.Key);
                        switchOutfitHelper.Dispose();
                        this.mOriginalSimOutfit = null;
                        return;
                    }
                }
            }
            Sim.SwitchOutfitHelper switchOutfitHelper2 = new Sim.SwitchOutfitHelper(s, Sim.ClothesChangeReason.Force, OutfitCategories.Everyday);
            if (switchOutfitHelper2 != null)
            {
                switchOutfitHelper2.Start();
                switchOutfitHelper2.Wait(true);
                s.SwitchToOutfitWithoutSpin(Sim.ClothesChangeReason.Force, OutfitCategories.Everyday);
                switchOutfitHelper2.Dispose();
            }
            this.mOriginalSimOutfit = null;
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
            switch (this.mChefQuality)
            {
                case OFBOven.ChefQuality.LineCook:
                    {
                        return OFBOven.kChefSkillLineCook;
                    }
                case OFBOven.ChefQuality.SousChef:
                    {
                        return OFBOven.kChefSkillSousChef;
                    }
                case OFBOven.ChefQuality.ExecutiveChef:
                    {
                        return OFBOven.kChefSkillExecutiveChef;
                    }
                default:
                    {
                        return OFBOven.kChefSkillLineCook;
                    }
            }
        }
        new public List<Recipe> GetAvailableRecipes(int chefSkill)
        {
            List<Recipe> recipes = Recipe.Recipes;
            List<Recipe> list = new List<Recipe>();
            for (int i = 0; i < recipes.Count; i++)
            {
                if (!recipes[i].IsPetFood && (recipes[i].AvailableForBreakfast || recipes[i].AvailableForBrunch || recipes[i].AvailableForLunch || recipes[i].AvailableForDinner || recipes[i].Key.Equals("EP9KeyLimePie") || recipes[i].Key.Equals("EP9CheeseDanish") || recipes[i].Key.Equals("GrasshopperPie") || recipes[i].Key.Equals("PulledPork")) && !(recipes[i].Key == "Ambrosia") && !(recipes[i].Key == "VampireFood") && !(recipes[i].Key == "Wedding Cake") && !(recipes[i].Key == "AlooMasalaCurry") && !(recipes[i].Key == "Ceviche") && !(recipes[i].Key == "ChiliConCarne") && !(recipes[i].Key == "VegetarianChili") && !(recipes[i].Key == "FirecrackerShrimp") && !(recipes[i].Key == "FirecrackerTofu") && !(recipes[i].Key == "HotAndSourSoup") && recipes[i].CookingSkillLevelRequired <= chefSkill)
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
                    CommonMethodsOFBBistroSet.PrintMessage("found menu");
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
