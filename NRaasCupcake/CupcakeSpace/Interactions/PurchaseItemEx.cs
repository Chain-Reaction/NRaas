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
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<CraftersConsignment, CraftersConsignment.PurchaseItem.Definition, Definition>(false);

            if (Common.AssemblyCheck.IsInstalled("NRaasBOGO")) return;

            sOldSingleton = Singleton;
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
            if (((mObject == ObjectGuid.InvalidObjectGuid) && base.Autonomous) && !base.Actor.IsSelectable)
            {
                List<ObjectGuid> objectsICanBuyInDisplay = DisplayHelper.GetObjectsICanBuyInDisplay(base.Actor, base.Target);
                RandomUtil.RandomizeListOfObjects<ObjectGuid>(objectsICanBuyInDisplay);
                int familyFunds = base.Actor.FamilyFunds;
                for (int i = 0; i < objectsICanBuyInDisplay.Count; i++)
                {
                    int cost = DisplayHelper.ComputeFinalPriceOnObject(objectsICanBuyInDisplay[i]);
                    if (cost <= familyFunds)
                    {
                        Definition continuationDefinition = new Definition(objectsICanBuyInDisplay[i], cost, objectsICanBuyInDisplay.Count != 1);
                        base.TryPushAsContinuation(continuationDefinition);                       
                        return true;
                    }
                }                
                return false;
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
            if (base.Actor.FamilyFunds < interactionDefinition.mCost)
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
            bool flag = false;
            bool flag2 = false;
            if (((target.IsLiveDraggingEnabled() && !target.InUse) && ((target.ItemComp != null) && target.ItemComp.CanAddToInventory(base.Actor.Inventory))) && (base.Actor.Inventory.CanAdd(target) && base.Actor.Inventory.TryToAdd(target)))
            {
                flag = true;
            }
            else if (!target.InUse && base.Actor.Household.SharedFamilyInventory.Inventory.TryToAdd(target))
            {
                flag2 = true;
            }
            bool succeeded = flag || flag2;
            if (succeeded)
            {
                if (flag2)
                {
                    base.Actor.ShowTNSIfSelectable(CraftersConsignment.LocalizeString(base.Actor.IsFemale, "PlacedInFamilyInventory", new object[] { base.Actor, target }), StyledNotification.NotificationStyle.kGameMessagePositive);
                }
                else
                {
                    base.Actor.ShowTNSIfSelectable(CraftersConsignment.LocalizeString(base.Actor.IsFemale, "PlacedInPersonalInventory", new object[] { base.Actor, target }), StyledNotification.NotificationStyle.kGameMessagePositive);
                }
                base.Target.GiveMarkupBuffs(base.Actor, interactionDefinition.mObject);
                base.Actor.ModifyFunds(-interactionDefinition.mCost);
                base.Target.GiveLotOwnerMoney(interactionDefinition.mCost, base.Actor);
                base.Target.AccumulateRevenue(interactionDefinition.mCost);
            }
            base.EndCommodityUpdates(succeeded);
            base.StandardExit();
            return succeeded;
        }

        public new class Definition : CraftersConsignment.PurchaseItem.Definition
        {
            public new ObjectGuid mObject;
            public new int mCost;
            private new string[] mPath;
            private new bool mUsePath;

            public Definition()
            {
                this.mPath = new string[] { CraftersConsignment.LocalizeString(false, "PurchasePath", new object[0]) + Localization.Ellipsis };
            }

            public Definition(ObjectGuid target, int cost, bool usePath)
            {
                this.mPath = new string[] { CraftersConsignment.LocalizeString(false, "PurchasePath", new object[0]) + Localization.Ellipsis };
                this.mObject = target;
                this.mCost = cost;
                this.mUsePath = usePath;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, CraftersConsignment target, List<InteractionObjectPair> results)
            {
                List<ObjectGuid> objectsICanBuyInDisplay = DisplayHelper.GetObjectsICanBuyInDisplay(actor, target);
                for (int i = 0; i < objectsICanBuyInDisplay.Count; i++)
                {
                    results.Add(new InteractionObjectPair(new Definition(objectsICanBuyInDisplay[i], DisplayHelper.ComputeFinalPriceOnObject(objectsICanBuyInDisplay[i]), objectsICanBuyInDisplay.Count != 1), iop.Target));
                }
            }

            public override string[] GetPath(bool isFemale)
            {
                if (this.mUsePath)
                {
                    return this.mPath;
                }
                return new string[0];
            }

            public override string GetInteractionName(Sim actor, CraftersConsignment target, InteractionObjectPair iop)
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
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new PurchaseItemEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}