using NRaas.CommonSpace.Helpers;
using NRaas.CupcakeSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.Store.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CupcakeSpace.Interactions
{
    public class PurchaseItemEx : CraftersConsignment.PurchaseItem, Common.IPreLoad
    {
        //static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<CraftersConsignment, CraftersConsignment.PurchaseItem.Definition, Definition>(false);

            if (Common.AssemblyCheck.IsInstalled("NRaasEconomizer")) return;

            //sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {            
            Definition interactionDefinition = base.InteractionDefinition as Definition;
            if (interactionDefinition == null)
            {               
                return false;
            }
            ObjectGuid mObject = interactionDefinition.mObject;
            int mCost = interactionDefinition.mCost;
            if (mObject == ObjectGuid.InvalidObjectGuid)
            {
                List<ObjectGuid> objectsICanBuyInDisplay = DisplayHelper.GetObjectsICanBuyInDisplay(base.Actor, base.Target);
                if (!Autonomous && Actor.IsSelectable)
                {
                    List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();
                    foreach (ObjectGuid current in objectsICanBuyInDisplay)
                    {
                        GameObject obj = GlobalFunctions.ConvertGuidToObject<GameObject>(current);
                        ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(current, new List<ObjectPicker.ColumnInfo>
                            {
                                new ObjectPicker.ThumbAndTextColumn(obj.GetThumbnailKey(), obj.GetLocalizedName()),
                                new ObjectPicker.MoneyColumn(DisplayHelper.ComputeFinalPriceOnObject(obj, true))
                            });
                        list.Add(item);
                    }
                    List<ObjectPicker.HeaderInfo> list2 = new List<ObjectPicker.HeaderInfo>();
                    List<ObjectPicker.TabInfo> list3 = new List<ObjectPicker.TabInfo>();
                    list2.Add(new ObjectPicker.HeaderInfo(ShoppingRegister.sLocalizationKey + ":BuyFoodColumnName", ShoppingRegister.sLocalizationKey + ":BuyFoodColumnTooltip", 200));
                    list2.Add(new ObjectPicker.HeaderInfo("Ui/Caption/Shopping/Cart:Price", "Ui/Tooltip/Shopping/Cart:Price"));
                    list3.Add(new ObjectPicker.TabInfo("", ShoppingRegister.LocalizeString("AvailableFoods"), list));
                    List<ObjectPicker.RowInfo> list4 = SimplePurchaseDialog.Show(ShoppingRegister.LocalizeString("BuyFoodTitle"), Actor.FamilyFunds, list3, list2, true);
                    if (list4 == null || list4.Count != 1)
                    {
                        return false;
                    }
                    mObject = (ObjectGuid)list4[0].Item;
                    mCost = ((ObjectPicker.MoneyColumn)list4[0].ColumnInfo[1]).Value;
                }
                else
                {
                RandomUtil.RandomizeListOfObjects<ObjectGuid>(objectsICanBuyInDisplay);
                int familyFunds = base.Actor.FamilyFunds;
                for (int i = 0; i < objectsICanBuyInDisplay.Count; i++)
                {
                    int cost = DisplayHelper.ComputeFinalPriceOnObject(objectsICanBuyInDisplay[i]);
                    if (cost <= familyFunds)
                    {
                            //Definition continuationDefinition = new Definition(objectsICanBuyInDisplay[i], cost, false);
                            //base.TryPushAsContinuation(continuationDefinition);                       
                            //return true;
                            mObject = objectsICanBuyInDisplay[i];
                            mCost = cost;
                            break;
                        }
                    }
                    //return false;
                }                
            }
            if (mObject == ObjectGuid.InvalidObjectGuid)
            {                
                return false;
            }
            if (!base.Actor.RouteToObjectRadialRange(base.Target, 0f, base.Target.MaxProximityBeforeSwiping()))
            {                
                return false;
            }
            base.Actor.RouteTurnToFace(base.Target.Position);
            if (!DisplayHelper.GetObjectsICanBuyInDisplay(base.Actor, base.Target).Contains(mObject))
            {                
                return false;
            }
            if (base.Actor.FamilyFunds < mCost)
            {                
                return false;
            }
            GameObject target = GlobalFunctions.ConvertGuidToObject<GameObject>(mObject);
            if (target == null)
            {                
                return false;
            }
            
            base.StandardEntry();
            base.BeginCommodityUpdates();
            string swipeAnimationName = base.Target.GetSwipeAnimationName(target);
            if (Actor.SimDescription.Child)
            {
                swipeAnimationName = "c" + swipeAnimationName.Substring(1);
            }
            base.Actor.PlaySoloAnimation(swipeAnimationName, true);
            VisualEffect effect = VisualEffect.Create(base.Target.GetSwipeVfxName());
            Vector3 zero = Vector3.Zero;
            Vector3 axis = Vector3.Zero;
            if (Slots.AttachToBone(effect.ObjectId, base.Target.ObjectId, ResourceUtils.HashString32("transformBone"), false, ref zero, ref axis, 0f) == TransformParentingReturnCode.Success)
            {
                effect.SetAutoDestroy(true);
                effect.Start();
            }
            else
            {
                effect.Dispose();
                effect = null;
            }
            //bool flag = false;
            //bool flag2 = false;
            bool succeeded = false;
            bool addInteractions = true;
            string tnsKey = null;
            if (target.IsLiveDraggingEnabled() && !target.InUse && (interactionDefinition.mPushEat || (target.ItemComp != null && target.ItemComp.CanAddToInventory(base.Actor.Inventory) && base.Actor.Inventory.CanAdd(target)))) //&& base.Actor.Inventory.TryToAdd(target)))
            {
                ServingContainerGroup groupServing = null;
                if (interactionDefinition.mSingleServing)
                {
                   groupServing = target as ServingContainerGroup;
                    if (groupServing != null)
                    {
                        target = groupServing.CookingProcess.CreateSingleServingOfFood(groupServing, true, true);
                        addInteractions = false;
                    }
                }
                if (interactionDefinition.mPushEat)
                {
                    target.SetOpacity(0f, 0f);
                    if (Actor.ParentToRightHand(target))
                    {
                        succeeded = true;
                        CarrySystem.EnterWhileHolding(Actor, target as ICarryable);
                    }
                    target.FadeIn();
                }
                else if (Actor.Inventory.TryToAdd(target))
                {
                    succeeded = true;
                    tnsKey = "PlacedInPersonalInventory";
                }
                if (succeeded)
                {
                    if (groupServing != null)
                    {
                        groupServing.DecrementServings ();
                        if (groupServing.NumServingsLeft == 0)
                        {
                            groupServing.FadeOut (false, true);
                        }
                    }
                }
                else if (groupServing != null && target != null)
            {
                    target.Destroy();
                }
            }
            else if (!target.InUse && base.Actor.Household.SharedFamilyInventory.Inventory.TryToAdd(target))
            {
                succeeded = true;
                tnsKey = "PlacedInFamilyInventory";
            }
            //bool succeeded = flag || flag2;
            if (succeeded)
            {
                if (addInteractions)
                {
                    Target.OnHandToolChildUnslotted(target, Slot.None);
                    if (target is Snack)
                    {
                        target.AddInteraction(Sims3.Gameplay.Objects.CookingObjects.Eat.Singleton, true);
                        target.AddInteraction(Snack_CleanUp.Singleton, true);
                    }
                }
                /*if (flag2)
                {
                    base.Actor.ShowTNSIfSelectable(CraftersConsignment.LocalizeString(base.Actor.IsFemale, "PlacedInFamilyInventory", new object[] { base.Actor, target }), StyledNotification.NotificationStyle.kGameMessagePositive);
                }
                else
                {
                    base.Actor.ShowTNSIfSelectable(CraftersConsignment.LocalizeString(base.Actor.IsFemale, "PlacedInPersonalInventory", new object[] { base.Actor, target }), StyledNotification.NotificationStyle.kGameMessagePositive);
                }*/
                if (tnsKey != null)
                {
                    Actor.ShowTNSIfSelectable(CraftersConsignment.LocalizeString(Actor.IsFemale, tnsKey, new object[] { Actor, target }), StyledNotification.NotificationStyle.kGameMessagePositive);
                }
                base.Target.GiveMarkupBuffs(base.Actor, mObject);
                base.Actor.ModifyFunds(-mCost);
                base.Target.GiveLotOwnerMoney(mCost, base.Actor);
                base.Target.AccumulateRevenue(mCost);
                if (interactionDefinition.mPushEat)
                {
                    (target as IFoodContainer).PushEatHeldFoodInteraction(Actor);
                }
            }
            base.EndCommodityUpdates(succeeded);
            base.StandardExit();
            return succeeded;
        }

        public new class Definition : CraftersConsignment.PurchaseItem.Definition
        {
            //public new ObjectGuid mObject;
            //public new int mCost;
            //private new string[] mPath;
            //private new bool mUsePath;

            public bool mPushEat;
            public bool mSingleServing;

            public Definition()
            {
                //this.mPath = new string[] { CraftersConsignment.LocalizeString(false, "PurchasePath", new object[0]) + Localization.Ellipsis };
            }

            public Definition(ObjectGuid target, bool usePath) : base(target, DisplayHelper.ComputeFinalPriceOnObject(target), usePath)
            {
                //this.mPath = new string[] { CraftersConsignment.LocalizeString(false, "PurchasePath", new object[0]) + Localization.Ellipsis };
                //this.mObject = target;
                //this.mCost = cost;
                //this.mUsePath = usePath;
            }

            public Definition(GameObject target, bool singleServing) : base(target.ObjectId, DisplayHelper.ComputeFinalPriceOnObject(target, singleServing), true)
            {
                mSingleServing = singleServing;
                mPath = new string[]{ CraftersConsignment.LocalizeString(false, "PurchasePath", new object[0]) + Localization.Ellipsis, Food.GetString(singleServing ? Food.StringIndices.MakeOneSingle : Food.StringIndices.MakeOneGroup) + Localization.Ellipsis };
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, CraftersConsignment target, List<InteractionObjectPair> results)
            {
                List<ObjectGuid> objectsICanBuyInDisplay = DisplayHelper.GetObjectsICanBuyInDisplay(actor, target);
                bool usePath = objectsICanBuyInDisplay.Count != 1;
                foreach (ObjectGuid objectGuid in objectsICanBuyInDisplay)
                {
                    ServingContainerGroup groupServing = GlobalFunctions.ConvertGuidToObject<ServingContainerGroup>(objectGuid);
                    if (groupServing != null)
                {
                        results.Add(new InteractionObjectPair(new Definition(groupServing, true), iop.Target, iop.Tuning));
                        results.Add(new InteractionObjectPair(new Definition(groupServing, false), iop.Target, iop.Tuning));
                        continue;
                    }
                    results.Add(new InteractionObjectPair(new Definition(objectGuid, usePath), iop.Target, iop.Tuning));
                }
            }

            /*public override string[] GetPath(bool isFemale)
            {
                if (this.mUsePath)
                {
                    return this.mPath;
                }
                return new string[0];
            }*/

            /*public override string GetInteractionName(Sim actor, CraftersConsignment target, InteractionObjectPair iop)
            {
                GameObject obj2 = GlobalFunctions.ConvertGuidToObject<GameObject>(this.mObject);
                if (obj2 == null)
                {
                    return CraftersConsignment.LocalizeString(actor.IsFemale, "Buy", new object[0]);
                }
                ServingContainer container = obj2 as ServingContainer;
                if (container != null)
                {
                    return CraftersConsignment.LocalizeString(actor.IsFemale, "BuyObjectForCost", new object[] { container.CookingProcess.RecipeNameFinal, this.mCost });
                }
                return CraftersConsignment.LocalizeString(actor.IsFemale, "BuyObjectForCost", new object[] { obj2.CatalogName, this.mCost });
            }*/

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new PurchaseItemEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, CraftersConsignment target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (mObject == ObjectGuid.InvalidObjectGuid)
                {
                    return (!isAutonomous || !a.IsSelectable) && DisplayHelper.GetObjectsICanBuyInDisplay(a, target).Count > 0;
                }
                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }

        public class BuyFoodDefinition : Definition, Common.IAddInteraction
        {
            static InteractionDefinition BuyFoodSingleton = new BuyFoodDefinition();

            public void AddInteraction(Common.InteractionInjectorList interactions)
            {            
                interactions.Add<CraftersConsignment>(BuyFoodSingleton);
            }

            public BuyFoodDefinition()
            {
                mSingleServing = true;
                mPushEat = true;
            }

            public BuyFoodDefinition(GameObject target) : base (target, true)
            {
                mPushEat = true;
                mPath = new string[]{ Common.LocalizeEAString("Gameplay/Objects/Register/ShoppingRegister/Buy_Food:InteractionName") + Localization.Ellipsis };
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, CraftersConsignment target, List<InteractionObjectPair> results)
            {
                List<ObjectGuid> objectsICanBuyInDisplay = DisplayHelper.GetObjectsICanBuyInDisplay(actor, target);
                if (objectsICanBuyInDisplay.Count == 1)
                {
                    PreparedFood food = GlobalFunctions.ConvertGuidToObject<PreparedFood>(objectsICanBuyInDisplay[0]);
                    if (food != null)
                    {
                        results.Add(new InteractionObjectPair(new BuyFoodDefinition(food), iop.Target, iop.Tuning));
                    }
                }
                else if (objectsICanBuyInDisplay.Count > 0)
                {
                    results.Add(iop);
                }
            }

            public override string GetInteractionName(ref InteractionInstanceParameters parameters)
            {
                if (mObject == ObjectGuid.InvalidObjectGuid)
                {
                    return Common.LocalizeEAString("Gameplay/Objects/Register/ShoppingRegister/Buy_Food:InteractionName");
                }
                return base.GetInteractionName(ref parameters);
            }
        }
    }
}