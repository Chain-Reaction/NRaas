using Sims3.Store.Objects;
using Sims3.SimIFace;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay;
using System.Collections.Generic;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.UI;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ThoughtBalloons;
using System.Text;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Objects.Pets;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.ActorSystems;
using Sims3.Metadata;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreSetRegister;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreRestockItem;
using Sims3.UI.CAS;
using System;
using ani_StoreSetRegister;
using ani_StoreSetBase;
using ani_StoreRestockItem;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.SimIFace.BuildBuy;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreSetBase.Shopping;
using ani_StoreSetBase.Shopping;


namespace Sims3.Gameplay.Objects.TombObjects.ani_StoreSetBase
{
    //[RuntimeExport]
    public class StoreSetBase : GameObject
    {
        [Tunable]
        protected static bool BuyWhenActive;

        [Tunable]
        protected static bool SendHomeAfterPurchase;

        public ani_StoreBaseInfo Info;

        public static bool ReturnBuyWhenActive()
        {
            return BuyWhenActive;
        }

        public static bool ReturnSendHomeAfterPurchase()
        {
            return SendHomeAfterPurchase;
        }

        public enum Markup
        {
            ReallyLow,
            Low,
            Normal,
            High,
            ReallyHigh
        }

        #region Setting

        public sealed class SetMarkup : ImmediateInteraction<IActor, StoreSetBase>
        {
            public sealed class Definition : ActorlessInteractionDefinition<IActor, StoreSetBase, StoreSetBase.SetMarkup>
            {
                public enum ItemRoomLot
                {
                    Item,
                    Room,
                    Lot
                }
                // public StoreSetBase.Markup mMarkup;
                public float mMarkup;
                public StoreSetBase.SetMarkup.Definition.ItemRoomLot mItemRoomLot;
                public Definition()
                {
                }
                public Definition(StoreSetBase.SetMarkup.Definition.ItemRoomLot irl)
                {
                    //this.mMarkup = markup;
                    this.mItemRoomLot = irl;
                }
                public override string[] GetPath(bool isFemale)
                {
                    List<string> list = new List<string>(2);
                    list.Add(CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0]));
                    list.Add("Set Markup Path");//GalleryShopBase.LocalizeString(false, "SetMarkupPath", new object[0]) + Localization.Ellipsis);
                    switch (this.mItemRoomLot)
                    {
                        case StoreSetBase.SetMarkup.Definition.ItemRoomLot.Item:
                            {
                                list.Add("Item");//GalleryShopBase.LocalizeString(false, "ItemPath", new object[0]) + Localization.Ellipsis);
                                break;
                            }
                        case StoreSetBase.SetMarkup.Definition.ItemRoomLot.Room:
                            {
                                list.Add("Room");//list.Add(GalleryShopBase.LocalizeString(false, "RoomPath", new object[0]) + Localization.Ellipsis);
                                break;
                            }
                        case StoreSetBase.SetMarkup.Definition.ItemRoomLot.Lot:
                            {
                                list.Add("Lot");//list.Add(GalleryShopBase.LocalizeString(false, "LotPath", new object[0]) + Localization.Ellipsis);
                                break;
                            }
                    }
                    return list.ToArray();
                }
                public override string GetInteractionName(IActor a, StoreSetBase target, InteractionObjectPair interaction)
                {
                    return CMStoreSet.LocalizeString("SetMarkupWithFloat", new object[] { target.mMarkup.ToString() });
                }
                public override bool Test(IActor a, StoreSetBase target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override void AddInteractions(InteractionObjectPair iop, IActor actor, StoreSetBase target, List<InteractionObjectPair> results)
                {
                    results.Add(new InteractionObjectPair(new StoreSetBase.SetMarkup.Definition(StoreSetBase.SetMarkup.Definition.ItemRoomLot.Item), iop.Target));
                    results.Add(new InteractionObjectPair(new StoreSetBase.SetMarkup.Definition(StoreSetBase.SetMarkup.Definition.ItemRoomLot.Room), iop.Target));
                    results.Add(new InteractionObjectPair(new StoreSetBase.SetMarkup.Definition(StoreSetBase.SetMarkup.Definition.ItemRoomLot.Lot), iop.Target));
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetBase.SetMarkup.Definition();
            public override bool Run()
            {
                string s = CMStoreSet.ShowDialogue(
                    CMStoreSet.LocalizeString("SetMarkupTitle", new object[0] { }),
                    CMStoreSet.LocalizeString("SetMarkupDescription", new object[0] { }),
                    this.Target.mMarkup.ToString());
                StoreSetBase.SetMarkup.Definition definition = base.InteractionDefinition as StoreSetBase.SetMarkup.Definition;
                if (definition != null)
                {
                    if (definition.mItemRoomLot == StoreSetBase.SetMarkup.Definition.ItemRoomLot.Item)
                    {
                        float.TryParse(s, out  this.Target.mMarkup);
                    }
                    else
                    {
                        StoreSetBase[] objects = Sims3.Gameplay.Queries.GetObjects<StoreSetBase>(this.Target.LotCurrent);
                        if (objects != null)
                        {
                            if (definition.mItemRoomLot == StoreSetBase.SetMarkup.Definition.ItemRoomLot.Room)
                            {
                                for (int i = 0; i < objects.Length; i++)
                                {
                                    if (objects[i] != null && objects[i].RoomId == this.Target.RoomId)
                                    {
                                        float.TryParse(s, out  objects[i].mMarkup);
                                    }
                                }
                            }
                            else
                            {
                                for (int j = 0; j < objects.Length; j++)
                                {
                                    if (objects[j] != null)
                                    {
                                        float.TryParse(s, out objects[j].mMarkup);
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
        }

        public sealed class SetOneDaySale : ImmediateInteraction<IActor, StoreSetBase>
        {
            public sealed class Definition : ActorlessInteractionDefinition<IActor, StoreSetBase, StoreSetBase.SetOneDaySale>
            {
                public enum ItemRoomLot
                {
                    Item,
                    Room,
                    Lot
                }
                public float mSaleDiscount;
                public StoreSetBase.SetOneDaySale.Definition.ItemRoomLot mItemRoomLot;
                public Definition()
                {
                }
                public Definition(float saleDiscount, StoreSetBase.SetOneDaySale.Definition.ItemRoomLot irl)
                {
                    this.mSaleDiscount = saleDiscount;
                    this.mItemRoomLot = irl;
                }
                public override string[] GetPath(bool isFemale)
                {
                    List<string> list = new List<string>(2);

                    list.Add("Sale Path");//list.Add(GalleryShopBase.LocalizeString(false, "SetSalePath", new object[0]) + Localization.Ellipsis);
                    switch (this.mItemRoomLot)
                    {
                        case StoreSetBase.SetOneDaySale.Definition.ItemRoomLot.Item:
                            {
                                list.Add("Item");//list.Add(GalleryShopBase.LocalizeString(false, "ItemPath", new object[0]) + Localization.Ellipsis);
                                break;
                            }
                        case StoreSetBase.SetOneDaySale.Definition.ItemRoomLot.Room:
                            {
                                list.Add("Room");//list.Add(GalleryShopBase.LocalizeString(false, "RoomPath", new object[0]) + Localization.Ellipsis);
                                break;
                            }
                        case StoreSetBase.SetOneDaySale.Definition.ItemRoomLot.Lot:
                            {
                                list.Add("Lot");//list.Add(GalleryShopBase.LocalizeString(false, "LotPath", new object[0]) + Localization.Ellipsis);
                                break;
                            }
                    }
                    return list.ToArray();
                }
                public override string GetInteractionName(IActor a, StoreSetBase target, InteractionObjectPair interaction)
                {
                    if (this.mSaleDiscount == 0f)
                    {
                        return "Cancel Sale";// GalleryShopBase.LocalizeString(false, "CancelSale", new object[0]);
                    }
                    return "Set one day sale precent: " + this.mSaleDiscount * 100f;
                    //return GalleryShopBase.LocalizeString(false, "SetOneDaySalePercent", new object[]
                    //{
                    //    this.mSaleDiscount * 100f
                    //});
                }
                public override bool Test(IActor a, StoreSetBase target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (this.mSaleDiscount == target.mSaleDiscount && this.mItemRoomLot == StoreSetBase.SetOneDaySale.Definition.ItemRoomLot.Item)
                    {
                        if (this.mSaleDiscount != 0f)
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CMStoreSet.LocalizeString("IsCurrentSaleRate", new object[0])); //GalleryShopBase.LocalizeString(false, "IsCurrentSaleRate", new object[0]));
                        }
                        return false;
                    }
                    return true;
                }
                public override void AddInteractions(InteractionObjectPair iop, IActor actor, StoreSetBase target, List<InteractionObjectPair> results)
                {
                    for (int i = 0; i < StoreSetBase.kOneDaySaleRates.Length; i++)
                    {
                        results.Add(new InteractionObjectPair(new StoreSetBase.SetOneDaySale.Definition(StoreSetBase.kOneDaySaleRates[i], StoreSetBase.SetOneDaySale.Definition.ItemRoomLot.Item), iop.Target));
                        results.Add(new InteractionObjectPair(new StoreSetBase.SetOneDaySale.Definition(StoreSetBase.kOneDaySaleRates[i], StoreSetBase.SetOneDaySale.Definition.ItemRoomLot.Room), iop.Target));
                        results.Add(new InteractionObjectPair(new StoreSetBase.SetOneDaySale.Definition(StoreSetBase.kOneDaySaleRates[i], StoreSetBase.SetOneDaySale.Definition.ItemRoomLot.Lot), iop.Target));
                    }
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetBase.SetOneDaySale.Definition();
            public override bool Run()
            {
                StoreSetBase.SetOneDaySale.Definition definition = base.InteractionDefinition as StoreSetBase.SetOneDaySale.Definition;
                if (definition != null)
                {
                    if (definition.mItemRoomLot == StoreSetBase.SetOneDaySale.Definition.ItemRoomLot.Item)
                    {
                        this.Target.StartSale(definition.mSaleDiscount);
                    }
                    else
                    {
                        StoreSetBase[] objects = Sims3.Gameplay.Queries.GetObjects<StoreSetBase>(this.Target.LotCurrent);
                        if (objects != null)
                        {
                            if (definition.mItemRoomLot == StoreSetBase.SetOneDaySale.Definition.ItemRoomLot.Room)
                            {
                                for (int i = 0; i < objects.Length; i++)
                                {
                                    if (objects[i] != null && objects[i].RoomId == this.Target.RoomId)
                                    {
                                        objects[i].StartSale(definition.mSaleDiscount);
                                    }
                                }
                            }
                            else
                            {
                                for (int j = 0; j < objects.Length; j++)
                                {
                                    if (objects[j] != null)
                                    {
                                        objects[j].StartSale(definition.mSaleDiscount);
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
        }

        public class ToggleRestockBuyMode : ImmediateInteraction<Sim, StoreSetBase>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, StoreSetBase, ToggleRestockBuyMode>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }

                public override string GetInteractionName(Sim a, StoreSetBase target, InteractionObjectPair interaction)
                {
                    if (target.Info.RestockBuyMode)
                        return CMStoreSet.LocalizeString("DisableRestockBuyMode", new object[0] { });
                    else
                        return CMStoreSet.LocalizeString("EnableRestockBuyMode", new object[0] { });
                }

                public override bool Test(Sim a, StoreSetBase target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new ToggleRestockBuyMode.Definition();

            public override bool Run()
            {

                this.Target.Info.RestockBuyMode = !this.Target.Info.RestockBuyMode;

                return true;
            }

        }

        public class ToggleRestockCraftable : ImmediateInteraction<Sim, StoreSetBase>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, StoreSetBase, ToggleRestockCraftable>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                        CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }

                public override string GetInteractionName(Sim a, StoreSetBase target, InteractionObjectPair interaction)
                {
                    if (target.Info.RestockCraftable)
                        return CMStoreSet.LocalizeString("DisableRestockCraftable", new object[0] { });
                    else
                        return CMStoreSet.LocalizeString("EnableRestockCraftable", new object[0] { });
                }

                public override bool Test(Sim a, StoreSetBase target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new ToggleRestockCraftable.Definition();

            public override bool Run()
            {

                this.Target.Info.RestockCraftable = !this.Target.Info.RestockCraftable;

                return true;
            }

        }

        class SetOwner : ImmediateInteraction<Sim, StoreSetBase>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, StoreSetBase, SetOwner>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, StoreSetBase target, InteractionObjectPair interaction)
                {
                    return CMStoreSet.LocalizeString("SetOwner", new object[0] { });
                }

                public override bool Test(Sim a, StoreSetBase target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {

                    return true;
                }
            }

            public static InteractionDefinition Singleton = new SetOwner.Definition();

            public override bool Run()
            {
                try
                {
                    SimDescription sd = CMStoreSet.ReturnSimsInHousehold(this.Actor.SimDescription, true, true);

                    if (sd != null)
                    {
                        base.Target.Info.Owner = sd.SimDescriptionId;
                        base.Target.Info.OwnerName = sd.FullName;
                        // CMStoreSet.PrintMessage(sd.FullName);
                    }
                    else
                    {
                        base.Target.Info.Owner = 0uL;
                        base.Target.Info.OwnerName = string.Empty;
                        CMStoreSet.PrintMessage(CMStoreSet.LocalizeString("ResetOwner", new object[0] { }));
                    }
                }
                catch (Exception ex)
                {
                    CMStoreSet.PrintMessage("StoreSetBase SetOwner: " + ex.Message);
                }

                return true;
            }

        }

        class LinkToRegister : ImmediateInteraction<Sim, StoreSetBase>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, StoreSetBase, LinkToRegister>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, StoreSetBase target, InteractionObjectPair interaction)
                {
                    return CMStoreSet.LocalizeString("LinkToRegister", new object[0] { });
                }

                public override bool Test(Sim a, StoreSetBase target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {

                    return true;
                }
            }

            public static InteractionDefinition Singleton = new LinkToRegister.Definition();

            public override bool Run()
            {
                try
                {
                    StoreSetRegister[] registers = Sims3.Gameplay.Queries.GetObjects<StoreSetRegister>(this.Target.LotCurrent);

                    if (registers != null && registers.Length > 0)
                    {
                        StoreSetRegister register = CMStoreSet.ReturnRegisterForLinking(registers);
                        if (register != null)
                        {
                            base.Target.Info.RegisterId = register.ObjectId;
                            base.Target.Info.RegisterName = register.Info.RegisterName;

                            //Reset owner
                            base.Target.Info.Owner = 0uL;
                            base.Target.Info.OwnerName = string.Empty;
                        }
                        else
                        {
                            base.Target.Info.RegisterId = ObjectGuid.InvalidObjectGuid;
                            base.Target.Info.RegisterName = string.Empty;
                            CMStoreSet.PrintMessage(CMStoreSet.LocalizeString("RegisterRemoved", new object[0] { }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    CMStoreSet.PrintMessage("LinkToRegister: " + ex.Message);
                }

                return true;
            }

        }

        public class SetCooldownPeriod : ImmediateInteraction<Sim, StoreSetBase>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, StoreSetBase, SetCooldownPeriod>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, StoreSetBase target, InteractionObjectPair interaction)
                {
                    return CMStoreSet.LocalizeString("SetCooldownPeriod", new object[0] { });
                }
                public override bool Test(Sim actor, StoreSetBase target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static InteractionDefinition Singleton = new SetCooldownPeriod.Definition();
            public override bool Run()
            {
                int.TryParse(CMStoreSet.ShowDialogueNumbersOnly(
                    CMStoreSet.LocalizeString("CooldownTitle", new object[0]),
                    CMStoreSet.LocalizeString("CooldownDescription", new object[0]),
                    base.Target.Info.CooldownInMinutes.ToString()), out base.Target.Info.CooldownInMinutes);
                return true;
            }
        }

        public class ToggleBuyWhenActive : ImmediateInteraction<Sim, StoreSetBase>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, StoreSetBase, ToggleBuyWhenActive>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, StoreSetBase target, InteractionObjectPair interaction)
                {
                    if (StoreSetBase.BuyWhenActive)
                        return CMStoreSet.LocalizeString("DisableBuyWhenActive", new object[0] { });
                    else
                        return CMStoreSet.LocalizeString("EnableBuyWhenActive", new object[0] { });
                }

                public override bool Test(Sim a, StoreSetBase target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new ToggleBuyWhenActive.Definition();

            public override bool Run()
            {

                StoreSetBase.BuyWhenActive = !StoreSetBase.BuyWhenActive;

                return true;
            }

        }

        public class ToggleSendHomeAfterPurchase : ImmediateInteraction<Sim, StoreSetBase>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, StoreSetBase, ToggleSendHomeAfterPurchase>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, StoreSetBase target, InteractionObjectPair interaction)
                {
                    if (StoreSetBase.SendHomeAfterPurchase)
                        return CMStoreSet.LocalizeString("DisableSendHomeAfterPurchase", new object[0] { });
                    else
                        return CMStoreSet.LocalizeString("EnableSendHomeAfterPurchase", new object[0] { });
                }

                public override bool Test(Sim a, StoreSetBase target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new ToggleSendHomeAfterPurchase.Definition();

            public override bool Run()
            {

                StoreSetBase.SendHomeAfterPurchase = !StoreSetBase.SendHomeAfterPurchase;

                return true;
            }

        }

        public class TakeAllItems : ImmediateInteraction<Sim, StoreSetBase>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, StoreSetBase, TakeAllItems>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, StoreSetBase target, InteractionObjectPair interaction)
                {
                    return "Take All Items";// CMStoreSet.LocalizeString("EnableSendHomeAfterPurchase", new object[0] { });
                }

                public override bool Test(Sim a, StoreSetBase target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new TakeAllItems.Definition();

            public override bool Run()
            {

                List<ObjectGuid> objectsICanBuyInDisplay = this.Target.GetObjectIDsInDisplay();

                foreach (var oid in objectsICanBuyInDisplay)
                {
                    GameObject gameObject = GlobalFunctions.ConvertGuidToObject<GameObject>(oid);

                    //Only items that can be put into inventory
                    if (RestockItemHelperClass.GetItemType(gameObject) != ItemType.Buy)
                    {
                        this.Actor.Inventory.TryToAdd(gameObject);
                    }
                }

                return true;
            }

        }


        #endregion Settings

        #region Buy

        public sealed class PurchaseItem : Interaction<Sim, StoreSetBase>
        {
            private float mShoppingDuration;

            public sealed class Definition : InteractionDefinition<Sim, StoreSetBase, StoreSetBase.PurchaseItem>
            {
                public ObjectGuid mObject;
                public int mCost;
                public bool mUsePath;
                public string[] mPath = new string[]
				{
					CMStoreSet.LocalizeString("PurchasePath", new object[0]) + Localization.Ellipsis
				};
                public Definition()
                {
                }
                public Definition(ObjectGuid target, int cost, bool usePath)
                {
                    this.mObject = target;
                    this.mCost = cost;
                    this.mUsePath = usePath;
                }
                public override string[] GetPath(bool isFemale)
                {
                    if (this.mUsePath)
                    {
                        return this.mPath;
                    }
                    return new string[0];
                }
                public override string GetInteractionName(Sim actor, StoreSetBase target, InteractionObjectPair iop)
                {
                    GameObject gameObject = GlobalFunctions.ConvertGuidToObject<GameObject>(this.mObject);
                    if (gameObject != null)
                    {
                        string name = gameObject.CatalogName;

                        ServingContainer serving = gameObject as ServingContainer;
                        if (serving != null)
                        {
                            if (serving.CookingProcess != null)
                                name = serving.CookingProcess.GetFinalRecipeName();
                        }

                        return CMStoreSet.LocalizeString("BuyObjectForCost", new object[]
						{
							name, 
							this.mCost
						});
                    }
                    return CMStoreSet.LocalizeString("Buy", new object[0]);
                }

                public override bool Test(Sim a, StoreSetBase target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.GetType() == typeof(RestockItem))
                        return false;

                    if (isAutonomous && a.BuffManager.HasElement(Sims3.Gameplay.ActorSystems.BuffNames.NewStuff))
                        return false;

                    if (target.Charred)
                    {
                        return false;
                    }
                    if (a.FamilyFunds < this.mCost)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("InsufficientFunds: " + this.mCost

                        );
                        return false;
                    }
                    //Custom tests
                    //Find register and then the owner
                    StoreSetRegister register = null;

                    if (target.Info.RegisterId != ObjectGuid.InvalidObjectGuid)
                    {
                        register = CMStoreSet.ReturnRegister(target.Info.RegisterId, target.LotCurrent);

                        //If the store is linked to register and is closed sims should not buy
                        if (!CMStoreSet.IsStoreOpen(register))
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CMStoreSet.LocalizeString("ShopClosed", new object[0]));
                            return false;
                        }
                    }
                    //If linked to register and nobody tending, can't buy
                    //TODO

                    //If sim is not in the active household and BuyWhenActive = true     
                    //Can buy autonomously only if store owner is the active hosuehold
                    if (isAutonomous && StoreSetBase.BuyWhenActive)
                    {
                        SimDescription owner = null;

                        if (target.Info.Owner != 0uL)
                            owner = CMStoreSet.ReturnSim(target.Info.Owner);
                        else if (target.Info.RegisterId != ObjectGuid.InvalidObjectGuid)
                        {

                            if (register != null && register.Info.OwnerId != 0uL)
                            {
                                owner = CMStoreSet.ReturnSim(register.Info.OwnerId);
                            }
                        }

                        //in-active sims shouldn't buy unless from the store of the active sim
                        //  CMStoreSet.PrintMessage((!a.Household.IsActive && owner == null)+ " " + (!a.Household.IsActive && owner != null && !owner.Household.IsActive));
                        if ((!a.Household.IsActive && owner == null) || (!a.Household.IsActive && owner != null && !owner.Household.IsActive))
                        {
                            return false;
                        }

                        //Sims in the active household can buy if they don't own the store
                        if (a.Household.IsActive && owner != null && owner.Household.IsActive)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                public override void AddInteractions(InteractionObjectPair iop, Sim actor, StoreSetBase target, List<InteractionObjectPair> results)
                {
                    List<ObjectGuid> objectsICanBuyInDisplay = target.GetObjectIDsICanBuyInDisplay(actor, true);

                    int servingPrice = 0;
                    StoreSetRegister register = CMStoreSet.ReturnRegister(target.Info.RegisterId, target.LotCurrent);
                    if (register != null)
                        servingPrice = register.Info.ServingPrice;

                    for (int i = 0; i < objectsICanBuyInDisplay.Count; i++)
                    {
                        results.Add(new InteractionObjectPair(new StoreSetBase.PurchaseItem.Definition(objectsICanBuyInDisplay[i], target.ComputeFinalPriceOnObject(objectsICanBuyInDisplay[i], servingPrice), objectsICanBuyInDisplay.Count != 1), iop.Target));
                    }
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetBase.PurchaseItem.Definition();
            public override bool Run()
            {
                try
                {
                    this.mShoppingDuration = RandomUtil.GetFloat(10f, 30f);
                    float minutes = StoreHelperClass.CalcuateUpdateFrequency(this.mShoppingDuration);

                    // CMStoreSet.PrintMessage(this.mShoppingDuration + " - " + minutes);

                    StoreSetBase.PurchaseItem.Definition definition = base.InteractionDefinition as StoreSetBase.PurchaseItem.Definition;
                    if (definition == null)
                    {
                        return false;
                    }
                    ObjectGuid mObject = definition.mObject;

                    if (mObject == ObjectGuid.InvalidObjectGuid && base.Autonomous)
                    {
                        List<ObjectGuid> objectsICanBuyInDisplay = this.Target.GetObjectIDsICanBuyInDisplay(this.Actor, base.Autonomous);
                        RandomUtil.RandomizeListOfObjects<ObjectGuid>(objectsICanBuyInDisplay);
                        int familyFunds = this.Actor.FamilyFunds;

                        int servingPrice = 0;
                        StoreSetRegister register = CMStoreSet.ReturnRegister(this.Target.Info.RegisterId, this.Target.LotCurrent);
                        if (register != null)
                            servingPrice = register.Info.ServingPrice;

                        for (int i = 0; i < objectsICanBuyInDisplay.Count; i++)
                        {

                            int num = this.Target.ComputeFinalPriceOnObject(objectsICanBuyInDisplay[i], servingPrice);
                            if (num <= familyFunds)
                            {
                                StoreSetBase.PurchaseItem.Definition continuationDefinition = new StoreSetBase.PurchaseItem.Definition(objectsICanBuyInDisplay[i], num, objectsICanBuyInDisplay.Count != 1);
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
                    if (!this.Actor.RouteToObjectRadialRange(this.Target, 0f, this.Target.MaxProximityBeforeSwiping()))
                    {
                        return false;
                    }
                    this.Actor.RouteTurnToFace(this.Target.Position);
                    if (!this.Target.GetObjectIDsICanBuyInDisplay(this.Actor, base.Autonomous).Contains(mObject))
                    {
                        return false;
                    }
                    if (this.Actor.FamilyFunds < definition.mCost)
                    {
                        return false;
                    }
                    GameObject gameObject = GlobalFunctions.ConvertGuidToObject<GameObject>(mObject);
                    if (gameObject == null)
                    {
                        return false;
                    }

                    #region Shopping Animations
                    //Shopping process, only for autonomous
                    if (this.Autonomous)
                    {
                        ShoppingProgress progress = new ShoppingProgress(minutes);
                        ShoppingMeter.StartShoppingGain(this.Actor, progress, 1);

                        //Start Thinking

                        base.StandardEntry();
                        base.BeginCommodityUpdates();

                        base.EnterStateMachine("ani_Buy", "Enter", "x");
                        base.AnimateSim("Loop_WatchTV_Standing");

                        bool b = this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.RespondLoop), this.mCurrentStateMachine);

                        this.mCurrentStateMachine.RequestState("x", "Exit");

                    }
                    #endregion Shopping Animations

                    //Post decission to buy
                    //Buy the time we get here, the interactions is on the object, not the pedestal/shelf/rug
                    bool isRug;
                    StoreSetBase shopBase = RestockItemHelperClass.ReturnStoreSetBase(gameObject, out isRug);

                    if (shopBase == null)
                    {
                        CMStoreSet.PrintMessage("Shop base null");
                        return false;

                    }
                    else
                    {
                        bool slotFound = false;
                        Slot slot = Slot.ContainmentSlot_0;
                        if (!isRug && shopBase != null)
                        {
                            Slot[] containmentSlots = shopBase.GetContainmentSlots();

                            if (containmentSlots != null)
                            {
                                for (int i = 0; i < containmentSlots.Length; i++)
                                {
                                    GameObject o = shopBase.GetContainedObject(containmentSlots[i]) as GameObject;

                                    if (o != null && o.ObjectId == gameObject.ObjectId)
                                    {
                                        slotFound = true;
                                        slot = containmentSlots[i];
                                        break;
                                    }
                                }
                            }
                        }

                        //Find register and owner 
                        StoreSetRegister register = null;

                        SimDescription owner = null;
                        if (this.Target.Info.Owner != 0uL)
                            owner = CMStoreSet.ReturnSim(this.Target.Info.Owner);
                        else if (this.Target.Info.RegisterId != ObjectGuid.InvalidObjectGuid)
                        {
                            register = CMStoreSet.ReturnRegister(this.Target.Info.RegisterId, this.Target.LotCurrent);
                            if (register != null && register.Info.OwnerId != 0uL)
                                owner = CMStoreSet.ReturnSim(register.Info.OwnerId);
                        }

                        //If item is buy object clone, other than that create restock item. 
                        GameObject restockItem = null;

                        Vector3 position = gameObject.Position;
                        ItemType objectType = RestockItemHelperClass.GetItemType(gameObject);

                        int price = gameObject.Value;
                        //If register is owned, find serving price                            
                        if (register != null)
                        {
                            ServingContainerGroup group = gameObject as ServingContainerGroup;
                            ServingContainerSingle single = gameObject as ServingContainerSingle;

                            if (group != null)
                                price = group.NumServingsLeft * register.Info.ServingPrice;
                            else if (single != null)
                                price = register.Info.ServingPrice;
                        }

                        // clonedObject = gameObject;
                        restockItem = StoreHelperClass.CreateRestockItem(gameObject, price, isRug);

                        if (restockItem == null || restockItem is FailureObject)
                        {
                            if (restockItem != null)
                            {
                                restockItem.Destroy();
                            }
                            return false;
                        }

                        if (owner != null)
                            StoreHelperClass.UpdateSkillBasedCareerEarning(owner, gameObject);


                        //base.StandardEntry();
                        //base.BeginCommodityUpdates();                  

                        string swipeAnimationName = this.Target.GetSwipeAnimationName(gameObject);
                        this.Actor.PlaySoloAnimation(swipeAnimationName, true);
                        VisualEffect visualEffect = VisualEffect.Create(this.Target.GetSwipeVfxName());
                        Vector3 zero = Vector3.Zero;
                        Vector3 zero2 = Vector3.Zero;
                        if (Slots.AttachToBone(visualEffect.ObjectId, this.Target.ObjectId, ResourceUtils.HashString32("transformBone"), false, ref zero, ref zero2, 0f) == TransformParentingReturnCode.Success)
                        {
                            visualEffect.SetAutoDestroy(true);
                            visualEffect.Start();
                        }
                        else
                        {
                            visualEffect.Dispose();
                        }

                        if (gameObject == null)
                            CMStoreSet.PrintMessage("object null");

                        bool flag = false;
                        bool flag2 = false;
                        if (gameObject.IsLiveDraggingEnabled() && !gameObject.InUse && gameObject.ItemComp != null && gameObject.ItemComp.CanAddToInventory(this.Actor.Inventory) && this.Actor.Inventory.CanAdd(gameObject) && this.Actor.Inventory.TryToAdd(gameObject))
                        {
                            flag = true;
                        }
                        else
                        {
                            if (!gameObject.InUse && this.Actor.Household.SharedFamilyInventory.Inventory.TryToAdd(gameObject))
                            {
                                flag2 = true;
                            }
                        }
                        bool flag3 = flag || flag2;

                        if (flag3)
                        {
                            //Return Interactions 
                            gameObject.OnStartup();

                            restockItem.SetPosition(position);

                           
                                if (slotFound)
                                {
                                    IGameObject io = (IGameObject)shopBase;
                                    restockItem.ParentToSlot(io, slot);
                                }
                            

                            //TODO:Keksi tähän jotain
                            //  this.Target.GiveMarkupBuffs(this.Actor, definition.mObject);

                            //TODO: 
                            //Add customers to the register's shopping data   
                            if (shopBase != null)
                            {
                                if (register == null && shopBase.Info.RegisterId != ObjectGuid.InvalidObjectGuid)
                                    register = CMStoreSet.ReturnRegister(shopBase.Info.RegisterId, this.Actor.LotCurrent);

                                if (register != null && register.Info != null)
                                {

                                    if (register.Info.ShoppingData == null)
                                        register.Info.ShoppingData = new Dictionary<SimDescription, int>();

                                    if (register.Info.ShoppingData.ContainsKey(this.Actor.SimDescription))
                                        register.Info.ShoppingData[this.Actor.SimDescription] += definition.mCost;// shopBase.ComputeFinalPriceOnObject(gameObject.ObjectId, register.Info.ServingPrice); //definition.mCost;
                                    else
                                        register.Info.ShoppingData.Add(this.Actor.SimDescription, definition.mCost); //shopBase.ComputeFinalPriceOnObject(gameObject.ObjectId, register.Info.ServingPrice));//definition.mCost);
                                }
                            }

                            //Add buffs and send home 
                            if (base.Target.Info.CooldownInMinutes > 0)
                            {
                                base.Actor.BuffManager.AddBuff(Sims3.Gameplay.ActorSystems.BuffNames.NewStuff, 0, base.Target.Info.CooldownInMinutes, false, Sims3.Gameplay.ActorSystems.MoodAxis.None, Sims3.Gameplay.ActorSystems.Origin.FromNewObjects, false);
                            }

                            //Do this only if shop base is not linked to register. 
                            if (shopBase == null || shopBase.Info.RegisterId == ObjectGuid.InvalidObjectGuid)
                            {
                                this.Actor.SimDescription.ModifyFunds(-definition.mCost);

                                //If shop has owner, give them the money
                                if (owner != null)
                                    owner.ModifyFunds(definition.mCost);

                            }
                            else if (register != null)
                            {
                                //Start waiting for paying
                                if (!this.Actor.Household.IsActive || (this.Actor.Household.IsActive && !this.Actor.IsActiveSim))
                                    if (!(this.Actor.Posture is StoreSetRegister.DoingCustomerStuffPosture))
                                    {
                                        StoreSetRegister.DoingCustomerStuffPosture posture = new StoreSetRegister.DoingCustomerStuffPosture(this.Actor, register, null);
                                        this.Actor.Posture = posture;
                                    }
                            }

                        }
                        base.EndCommodityUpdates(flag3);
                        base.StandardExit();
                        return flag3;
                    }
                }
                catch (Exception ex)
                {
                    CMStoreSet.PrintMessage("Purchase item: " + ex.Message);
                    return false;
                }

            }

            private void RespondLoop(StateMachineClient smc, InteractionInstance.LoopData loopData)
            {
                if (loopData.mLifeTime > this.mShoppingDuration)
                {
                    this.Actor.AddExitReason(ExitReason.Finished);
                }
            }

            public override void Cleanup()
            {
                base.Cleanup();
            }
        }

        public sealed class Browse : Interaction<Sim, StoreSetBase>
        {
            public sealed class Definition : InteractionDefinition<Sim, StoreSetBase, StoreSetBase.Browse>
            {
                public override bool Test(Sim a, StoreSetBase target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.Charred)
                    {
                        return false;
                    }
                    List<ObjectGuid> objectsICanBuyInDisplay = target.GetObjectIDsICanBuyInDisplay(a, isAutonomous);
                    if (objectsICanBuyInDisplay.Count == 0)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(
                           "Nothing on display"// GalleryShopBase.LocalizeString(a.IsFemale, "NothingOnDisplay", new object[0])
                            );
                        return false;
                    }

                    //If the store is linked to register and is closed sims should not buy
                    //Find register and then the owner
                    StoreSetRegister register = null;

                    if (target.Info.RegisterId != ObjectGuid.InvalidObjectGuid)
                        register = CMStoreSet.ReturnRegister(target.Info.RegisterId, target.LotCurrent);

                    if (CMStoreSet.IsStoreOpen(register))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("The Shop is Closed");
                        return false;
                    }

                    return true;
                }

                public override string GetInteractionName(Sim actor, StoreSetBase target, InteractionObjectPair iop)
                {
                    return "Browse"; // GalleryShopBase.LocalizeString(actor.IsFemale, "BrowseGoods", new object[0]);
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetBase.Browse.Definition();
            public override bool Run()
            {
                if (!this.Actor.RouteToObjectRadialRange(this.Target, 1f, this.Target.MaxProximityBeforeSwiping()))
                {
                    //this.Actor.PlayRouteFailure();
                    return false;
                }
                this.Actor.RouteTurnToFace(this.Target.Position);
                base.StandardEntry();
                base.BeginCommodityUpdates();
                List<ObjectGuid> objectsICanBuyInDisplay = this.Target.GetObjectIDsICanBuyInDisplay(this.Actor, base.Autonomous);
                RandomUtil.RandomizeListOfObjects<ObjectGuid>(objectsICanBuyInDisplay);
                //int num = 0;
                //while (this.Actor.HasNoExitReason() && num < objectsICanBuyInDisplay.Count)
                {
                    ObjectGuid guid = objectsICanBuyInDisplay[0];
                    GameObject gameObject = GlobalFunctions.ConvertGuidToObject<GameObject>(guid);
                    if (gameObject != null)
                    {
                        this.Actor.RouteTurnToFace(gameObject.Position);
                        int priority = 100;
                        this.Actor.LookAtManager.SetInteractionLookAt(gameObject, priority, LookAtJointFilter.TorsoBones | LookAtJointFilter.HeadBones);
                        bool flag = RandomUtil.RandomChance01(StoreSetBase.kBrowseChanceOfDislikingObject);
                        ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.BalloonData(gameObject.GetThumbnailKey());
                        if (flag)
                        {
                            balloonData.LowAxis = ThoughtBalloonAxis.kDislike;
                        }
                        this.Actor.ThoughtBalloonManager.ShowBalloon(balloonData);
                        string state = "1";
                        if (flag)
                        {
                            state = RandomUtil.GetRandomStringFromList(new string[]
							{
								"3", 
								"5", 
								"CantStandArtTraitReaction"
							});
                        }
                        else
                        {
                            state = RandomUtil.GetRandomStringFromList(new string[]
							{
								"0", 
								"1", 
								"2"
							});
                        }
                        base.EnterStateMachine("viewobjectinteraction", "Enter", "x");
                        base.AnimateSim(state);
                        base.AnimateSim("Exit");
                        this.Actor.LookAtManager.ClearInteractionLookAt();
                    }
                }
                if (base.Autonomous && !this.Actor.IsSelectable)
                {
                    float chance = StoreSetBase.kBrowseBaseChanceOfBuyingObjectWithoutSale + this.Target.mSaleDiscount;
                    if (RandomUtil.RandomChance01(chance))
                    {
                        List<ObjectGuid> objectsICanBuyInDisplay2 = this.Target.GetObjectIDsICanBuyInDisplay(this.Actor, base.Autonomous);
                        if (objectsICanBuyInDisplay2.Count > 0)
                        {
                            ObjectGuid randomObjectFromList = RandomUtil.GetRandomObjectFromList<ObjectGuid>(objectsICanBuyInDisplay2);
                            if (randomObjectFromList != ObjectGuid.InvalidObjectGuid)
                            {
                                int servingPrice = 0;
                                StoreSetRegister register = CMStoreSet.ReturnRegister(this.Target.Info.RegisterId, this.Target.LotCurrent);
                                if (register != null)
                                    servingPrice = register.Info.ServingPrice;

                                StoreSetBase.PurchaseItem.Definition continuationDefinition = new StoreSetBase.PurchaseItem.Definition(randomObjectFromList, this.Target.ComputeFinalPriceOnObject(randomObjectFromList, servingPrice), false);
                                base.TryPushAsContinuation(continuationDefinition);
                            }
                        }
                    }
                }
                if (!base.Autonomous)
                {
                    List<ObjectGuid> objectsICanBuyInDisplay3 = this.Target.GetObjectIDsICanBuyInDisplay(this.Actor, base.Autonomous);
                    if (objectsICanBuyInDisplay.Count > 0)
                    {
                        int servingPrice = 0;
                        StoreSetRegister register = CMStoreSet.ReturnRegister(this.Target.Info.RegisterId, this.Target.LotCurrent);
                        if (register != null)
                            servingPrice = register.Info.ServingPrice;

                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.AppendLine(CMStoreSet.LocalizeString("BrowseItemsForPurchaseHeading", new object[0]));
                        for (int i = 0; i < objectsICanBuyInDisplay3.Count; i++)
                        {
                            GameObject gameObject2 = GlobalFunctions.ConvertGuidToObject<GameObject>(objectsICanBuyInDisplay3[i]);
                            if (gameObject2 != null)
                            {
                                stringBuilder.AppendLine(CMStoreSet.LocalizeString("BrowseLineItem", new object[]
								{
									gameObject2.CatalogName, 
									this.Target.ComputeFinalPriceOnObject(objectsICanBuyInDisplay3[i], servingPrice)
								}));
                            }
                        }
                        this.Actor.ShowTNSIfSelectable(stringBuilder.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive);
                    }
                }
                base.EndCommodityUpdates(true);
                base.StandardExit();
                return true;
            }
        }

        public sealed class ChildObjectPurchaseStub : Interaction<Sim, IGameObject>
        {
            [DoesntRequireTuning]
            public sealed class Definition : InteractionDefinition<Sim, IGameObject, StoreSetBase.ChildObjectPurchaseStub>
            {
                public ObjectGuid mParentRug = ObjectGuid.InvalidObjectGuid;
                public Definition()
                {
                }
                public Definition(ObjectGuid mParentRug)
                {
                    this.mParentRug = mParentRug;
                }
                public override bool Test(Sim a, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override void AddInteractions(InteractionObjectPair iop, Sim actor, IGameObject target, List<InteractionObjectPair> results)
                {
                    StoreSetBase ani_GalleryShopBase = null;
                    if (this.mParentRug != ObjectGuid.InvalidObjectGuid)
                    {
                        ani_GalleryShopBase = GlobalFunctions.ConvertGuidToObject<StoreSetBase>(this.mParentRug);
                    }
                    IGameObject parent = target.Parent;
                    while (ani_GalleryShopBase == null && parent != null)
                    {
                        ani_GalleryShopBase = (parent as StoreSetBase);
                        parent = parent.Parent;
                    }

                    if (ani_GalleryShopBase != null)
                    {
                        //CMStoreSet.PrintMessage("Add interactions: " + target.GetType());
                        //List<ObjectGuid> objectsICanBuyInDisplay = ani_GalleryShopBase.GetObjectsICanBuyInDisplay(actor);
                        //if (objectsICanBuyInDisplay.Contains(target.ObjectId))
                        if (target.GetType() != typeof(FoodProp) && target.GetType() != typeof(RestockItem))
                        {
                            int servingPrice = 0;

                            if (ani_GalleryShopBase != null)
                            {
                                StoreSetRegister register = CMStoreSet.ReturnRegister(ani_GalleryShopBase.Info.RegisterId, target.LotCurrent);
                                if (register != null)
                                    servingPrice = register.Info.ServingPrice;
                            }

                            results.Add(new InteractionObjectPair(new StoreSetBase.PurchaseItem.Definition(target.ObjectId, ani_GalleryShopBase.ComputeFinalPriceOnObject(target.ObjectId, servingPrice), false), ani_GalleryShopBase));
                        }
                    }
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetBase.ChildObjectPurchaseStub.Definition();
        }

        public sealed class ChildObjectBrowseStub : Interaction<Sim, IGameObject>
        {
            [DoesntRequireTuning]
            public sealed class Definition : InteractionDefinition<Sim, IGameObject, StoreSetBase.ChildObjectBrowseStub>
            {
                public ObjectGuid mParentRug = ObjectGuid.InvalidObjectGuid;
                public Definition()
                {
                }
                public Definition(ObjectGuid mParentRug)
                {
                    this.mParentRug = mParentRug;
                }
                public override bool Test(Sim a, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override void AddInteractions(InteractionObjectPair iop, Sim actor, IGameObject target, List<InteractionObjectPair> results)
                {
                    StoreSetBase ani_GalleryShopBase = null;
                    if (this.mParentRug != ObjectGuid.InvalidObjectGuid)
                    {
                        ani_GalleryShopBase = GlobalFunctions.ConvertGuidToObject<StoreSetBase>(this.mParentRug);
                    }
                    IGameObject parent = target.Parent;
                    while (ani_GalleryShopBase == null && parent != null)
                    {
                        ani_GalleryShopBase = (parent as StoreSetBase);
                        parent = parent.Parent;
                    }
                    if (ani_GalleryShopBase != null)
                    {
                        results.Add(new InteractionObjectPair(new StoreSetBase.Browse.Definition(), ani_GalleryShopBase));
                    }
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetBase.ChildObjectBrowseStub.Definition();
        }

        #endregion Buy

        #region Other Stuff
        public new static readonly string sLocalizationKey = "Sims3.Store.Objects".Substring(6).Replace('.', '/') + "/ani_GalleryShopBase";
        // protected StoreSetBase.Markup mMarkup = StoreSetBase.Markup.Normal;
        protected float mMarkup = 1f;
        protected float mSaleDiscount;
        public AlarmHandle mOneDaySaleAlarm = AlarmHandle.kInvalidHandle;
        public static ulong kGuidShoppingGallery_WhatADeal = 5462585244050725281uL;
        public static ulong kGuidShoppingGallery_NiceValue = 5462585244050725282uL;
        public static ulong kGuidShoppingGallery_ExpensiveWorthIt = 5462585244050725283uL;
        public static ulong kGuidShoppingGallery_ReallyExpensive = 5462585244050725284uL;
        public ulong mLifetimeSaleCount;
        public ulong mLifetimeRevenue;
        public ulong mDailyRevenue;
        public DateAndTime mLastRevenue = DateAndTime.Invalid;
        public AlarmHandle mDailySaleCheckAlarm = AlarmHandle.kInvalidHandle;
        public DateAndTime mLastDailySaleCheck = DateAndTime.Invalid;
        [Tunable, TunableComment("Markup percent for really low price (default 0.5 = 50%)")]
        public static float kMarkupReallyLow = 0.5f;
        [Tunable, TunableComment("Markup percent for low price (default 0.75 = 75%)")]
        public static float kMarkupLow = 0.75f;
        [TunableComment("Markup percent for normal price (default 1.0 = 100%)"), Tunable]
        public static float kMarkupNormal = 1f;
        [TunableComment("Markup percent for high price (default 1.25 = 125%)"), Tunable]
        public static float kMarkupHigh = 1.25f;
        [TunableComment("Markup percent for really high price (default 1.5 = 150%)"), Tunable]
        public static float kMarkupReallyHigh = 1.5f;
        [TunableComment("On-sale discount rates"), Tunable]
        public static float[] kOneDaySaleRates = new float[]
		{
			0.1f, 
			0.25f, 
			0.4f, 
			0.5f, 
			0.6f, 
			0.75f
		};
        [Tunable, TunableComment("When a sale happens, weighted values of probability to use the different rates.")]
        public static float[] kOneDaySaleOdds = new float[]
		{
			60f, 
			30f, 
			10f, 
			3f, 
			2f, 
			1f
		};
        [TunableComment("Percent of the income to give to the shop owner, design says this should be tunable.  Default 1.0 (100 percent) "), Tunable]
        public static float kPercentOfMoneyToGiveLotOwner = 1f;
        [TunableComment("Percent chance of disliking an object when viewing it (default = 0.25)"), Tunable]
        public static float kBrowseChanceOfDislikingObject = 0.25f;
        [TunableComment("Percent chance of buying an object after viewing it, before applying on sale bonus (default = 0.10)"), Tunable]
        public static float kBrowseBaseChanceOfBuyingObjectWithoutSale = 0.1f;
        [TunableComment("Percent chance of a random daily sale happening on any random day (default = 0.10)"), Tunable]
        public static float kChanceOfRandomDailySale = 0.1f;
        [TunableComment("Necessary proximity before swiping"), Tunable]
        public static float kMaxProximityBeforeSwiping = 3f;
        public static string LocalizeString(bool isFemale, string name, params object[] parameters)
        {
            return Localization.LocalizeString(isFemale, StoreSetBase.sLocalizationKey + ":" + name, parameters);
        }
        public virtual float MaxProximityBeforeSwiping()
        {
            return StoreSetBase.kMaxProximityBeforeSwiping;
        }

        #endregion Other Stuff

        public override Tooltip CreateTooltip(Vector2 mousePosition, WindowBase mousedOverWindow, ref Vector2 tooltipPosition)
        {
            StringBuilder sb = new StringBuilder();

            if (this.Info.Owner != 0uL)
            {
                sb.Append("Owner: ");
                sb.Append(this.Info.OwnerName);
                sb.Append("\n");
            }
            else if (this.Info.RegisterId != ObjectGuid.InvalidObjectGuid)
            {
                sb.Append("Linked to Register: ");
                sb.Append(this.Info.RegisterName);
            }

            return new SimpleTextTooltip(sb.ToString());
        }

        public override void OnCreation()
        {
            base.OnCreation();
            Info = new ani_StoreBaseInfo();
        }
        public override void OnStartup()
        {
            // base.AddInteraction(TestInteraction.Singleton);

            base.AddInteraction(SetOwner.Singleton);
            base.AddInteraction(LinkToRegister.Singleton);
            base.AddInteraction(ToggleRestockBuyMode.Singleton);
            base.AddInteraction(ToggleRestockCraftable.Singleton);
            base.AddInteraction(SetCooldownPeriod.Singleton);
            base.AddInteraction(ToggleBuyWhenActive.Singleton);
            base.AddInteraction(ToggleSendHomeAfterPurchase.Singleton);
            base.AddInteraction(TakeAllItems.Singleton);

            base.AddInteraction(StoreSetBase.SetMarkup.Singleton);
            base.AddInteraction(StoreSetBase.PurchaseItem.Singleton);
            base.AddInteraction(StoreSetBase.SetOneDaySale.Singleton);
            base.AddInteraction(StoreSetBase.Browse.Singleton);

            //this.AddInteractionsToChildObjects();

            base.AddAlarm(0f, TimeUnit.Seconds, new AlarmTimerCallback(this.AddInteractionsToChildObjects), "Timer to add interactions as soon as the world starts up", AlarmType.NeverPersisted);
            if (this.mDailySaleCheckAlarm == AlarmHandle.kInvalidHandle)
            {
                this.mDailySaleCheckAlarm = base.AddAlarmDay(0f, DaysOfTheWeek.All, new AlarmTimerCallback(this.DailySaleCheckCallback), this.ToString() + " Daily sale check", AlarmType.AlwaysPersisted);
            }
        }
        public void DailySaleCheckCallback()
        {
            if (!base.InWorld)
            {
                return;
            }
            if (this.mLastDailySaleCheck != DateAndTime.Invalid && SimClock.IsSameDay(this.mLastDailySaleCheck, SimClock.CurrentTime()))
            {
                return;
            }
            if (this.FindMoneyGetter() != null)
            {
                return;
            }
            float num = 0f;
            if (RandomUtil.RandomChance01(StoreSetBase.kChanceOfRandomDailySale) && StoreSetBase.kOneDaySaleOdds.Length == StoreSetBase.kOneDaySaleRates.Length)
            {
                int weightedIndex = RandomUtil.GetWeightedIndex(StoreSetBase.kOneDaySaleOdds);
                num = StoreSetBase.kOneDaySaleRates[weightedIndex];
            }
            StoreSetBase[] objects = Sims3.Gameplay.Queries.GetObjects<StoreSetBase>(base.LotCurrent);
            if (objects != null)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] != null)
                    {
                        objects[i].mLastDailySaleCheck = SimClock.CurrentTime();
                        objects[i].StartSale(num);
                    }
                }
                if (num != 0f && base.LotCurrent.Name != null)
                {
                    string titleText = CMStoreSet.LocalizeString("SaleStartsDialogText", new object[]
					{
						num * 100f, 
						base.LotCurrent.Name
					});
                    StyledNotification.Format format = new StyledNotification.Format(titleText, base.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
                    StyledNotification.Show(format);
                }
            }
        }
        public override void OnHandToolChildSlotted(IGameObject objectPlacedInSlot, Slot slot)
        {
            GameObject gameObject = objectPlacedInSlot as GameObject;

            if (gameObject != null)
            {
                //Remove all interactions
                gameObject.RemoveAllInteractions();

                //bool isRug = true;
                //StoreSetBase sBase = RestockItemHelperClass.ReturnStoreSetBase(this, out isRug);
                if (StoreHelperClass.AddPurchaseInteraction(null, gameObject, this, false))
                {
                    //gameObject.RemoveInteractionByType(StoreSetBase.ChildObjectPurchaseStub.Singleton);
                    //gameObject.RemoveInteractionByType(StoreSetBase.ChildObjectBrowseStub.Singleton);
                    gameObject.AddInteraction(StoreSetBase.ChildObjectPurchaseStub.Singleton);
                    gameObject.AddInteraction(StoreSetBase.ChildObjectBrowseStub.Singleton);
                }

                //For food
                StoreSetRegister register = CMStoreSet.ReturnRegister(this.Info.RegisterId, this.LotCurrent);

                ServingContainerGroup servingContainerGroup = gameObject as ServingContainerGroup;
                if (servingContainerGroup != null)
                {
                    //servingContainerGroup.RemoveInteractionByType(ServingContainerGroup.ServingContainerGroup_GrabAServing.Singleton);
                    //servingContainerGroup.RemoveInteractionByType(ServingContainerGroup.ServingContainerGroup_BringAServingTo.Singleton);
                    servingContainerGroup.RemoveSpoilageAlarm();
                }
                ServingContainerSingle servingContainerSingle = gameObject as ServingContainerSingle;
                if (servingContainerSingle != null)
                {
                    //servingContainerSingle.RemoveInteractionByType(Sims3.Gameplay.Objects.CookingObjects.Eat.Singleton);
                    servingContainerSingle.RemoveSpoilageAlarm();
                }
                ServingContainer servingContainer = gameObject as ServingContainer;
                if (servingContainer != null)
                {
                    //servingContainer.RemoveInteractionByType(ServingContainer.PutAwayLeftovers.Singleton);
                    if (servingContainer.Washable)
                    {
                        //servingContainer.RemoveInteractionByType(ServingContainer.ServingContainer_CleanUp.Singleton);
                    }
                }

            }
            base.OnHandToolChildSlotted(objectPlacedInSlot, slot);
        }
        public override void OnHandToolChildUnslotted(IGameObject objectRemovedFromSlot, Slot slot)
        {
            GameObject gameObject = objectRemovedFromSlot as GameObject;
            if (gameObject != null && StoreHelperClass.AddPurchaseInteraction(null, gameObject, null, true))
            {
                //Clear all interactins
                gameObject.RemoveAllInteractions();
                gameObject.OnStartup();

                //gameObject.RemoveInteractionByType(StoreSetBase.ChildObjectPurchaseStub.Singleton);
                //gameObject.RemoveInteractionByType(StoreSetBase.ChildObjectBrowseStub.Singleton);

                ServingContainerGroup servingContainerGroup = gameObject as ServingContainerGroup;
                if (servingContainerGroup != null)
                {
                    servingContainerGroup.AddInteraction(ServingContainerGroup.ServingContainerGroup_GrabAServing.Singleton);
                    servingContainerGroup.AddInteraction(ServingContainerGroup.ServingContainerGroup_BringAServingTo.Singleton);
                }
                ServingContainerSingle servingContainerSingle = gameObject as ServingContainerSingle;
                if (servingContainerSingle != null)
                {
                    servingContainerSingle.AddInteraction(Sims3.Gameplay.Objects.CookingObjects.Eat.Singleton);
                }
                ServingContainer servingContainer = gameObject as ServingContainer;
                if (servingContainer != null)
                {
                    servingContainer.AddInteraction(ServingContainer.PutAwayLeftovers.Singleton);
                    if (servingContainer.Washable)
                    {
                        servingContainer.AddInteraction(ServingContainer.ServingContainer_CleanUp.Singleton);
                    }
                }

            }
            base.OnHandToolChildUnslotted(objectRemovedFromSlot, slot);
        }
        public virtual void AddInteractionsToChildObjects()
        {
            List<ObjectGuid> list = new List<ObjectGuid>();

            Slot[] containmentSlots = base.GetContainmentSlots();

            for (int i = 0; i < containmentSlots.Length; i++)
            {
                // bool isRug = true;
                GameObject gameObject = base.GetContainedObject(containmentSlots[i]) as GameObject;
               
                if (gameObject != null)//this.TestIfObjectCanBeBoughtByActor(gameObject, actor))
                {
                    gameObject.RemoveAllInteractions();
                    gameObject.AddInteraction(StoreSetBase.ChildObjectPurchaseStub.Singleton);
                    gameObject.AddInteraction(StoreSetBase.ChildObjectBrowseStub.Singleton);
                }
            }

        }
        public Household FindMoneyGetter()
        {
            Household household = null;// base.LotCurrent.Household;
            // if (household == null)
            {
                if (this.Info.Owner != 0uL)
                {
                    SimDescription sd = CMStoreSet.ReturnSim(this.Info.Owner);
                    if (sd.Household != null)
                        household = sd.Household;
                }
                else if (this.Info.RegisterId != ObjectGuid.InvalidObjectGuid)
                {
                    StoreSetRegister register = CMStoreSet.ReturnRegister(this.Info.RegisterId, base.LotCurrent);
                    if (register != null && register.Info.OwnerId != 0uL)
                    {
                        SimDescription sd = CMStoreSet.ReturnSim(register.Info.OwnerId);
                        if (sd.Household != null)
                            household = sd.Household;
                    }
                }
            }
            return household;
        }

        public virtual List<ObjectGuid> GetObjectIDsICanBuyInDisplay(Sim actor, bool isAutonomous)
        {
            List<ObjectGuid> list = new List<ObjectGuid>();
            if (base.Charred)
            {
                return list;
            }
            Slot[] containmentSlots = base.GetContainmentSlots();

            for (int i = 0; i < containmentSlots.Length; i++)
            {
                // bool isRug = true;
                GameObject gameObject = base.GetContainedObject(containmentSlots[i]) as GameObject;
                // StoreSetBase sBase = RestockItemHelperClass.ReturnStoreSetBase(gameObject, out isRug);

                if (gameObject != null && StoreHelperClass.AddPurchaseInteraction(actor, gameObject, this, isAutonomous))//this.TestIfObjectCanBeBoughtByActor(gameObject, actor))
                {
                    list.Add(gameObject.ObjectId);
                }
            }

            return list;
        }

        public virtual List<ObjectGuid> GetObjectIDsInDisplay()
        {
            List<ObjectGuid> list = new List<ObjectGuid>();

            Slot[] containmentSlots = base.GetContainmentSlots();

            for (int i = 0; i < containmentSlots.Length; i++)
            {
                GameObject gameObject = base.GetContainedObject(containmentSlots[i]) as GameObject;

                if (gameObject != null)
                {
                    list.Add(gameObject.ObjectId);
                }
            }

            return list;
        }

        public virtual string GetSwipeAnimationName(GameObject target)
        {
            return "a2o_object_genericSwipe_x";
        }
        public virtual string GetSwipeVfxName()
        {
            return "store_rugPurchase";
        }
        public int ComputeFinalPriceOnObject(ObjectGuid targetGuid, int servingPrice)
        {
            int result = 0;
            int num = 0;
            GameObject obj = GlobalFunctions.ConvertGuidToObject<GameObject>(targetGuid);
            this.BasePriceFinalPriceDiff(obj, servingPrice, out result, out num);
            return result;
        }
        public int BasePriceFinalPriceDiff(GameObject obj, int servingPrice, out int FinalPrice, out int BasePrice)
        {
            BasePrice = 0;
            if (obj != null)
            {
                if (obj.Product != null)
                {
                    BasePrice = (int)obj.Value;//.Product.Price;
                }
                else
                {
                    BasePrice = obj.Value;
                }

                //For food
                ServingContainerSingle single = obj as ServingContainerSingle;
                ServingContainerGroup group = obj as ServingContainerGroup;

                if (group != null)
                {
                    BasePrice = StoreHelperClass.ReturnPriceByQuality(group.FoodQuality, servingPrice * group.NumServingsLeft);
                    group.RemoveSpoilageAlarm();
                }
                else if (single != null)
                {
                    BasePrice = StoreHelperClass.ReturnPriceByQuality(single.FoodQuality, servingPrice);
                    single.RemoveSpoilageAlarm();
                }
            }
            // CMStoreSet.PrintMessage(this.mMarkup + " " + this.mSaleDiscount);
            float num = this.mMarkup;
            float num2 = BasePrice * num;
            num2 -= num2 * this.mSaleDiscount;
            FinalPrice = (int)num2;
            return BasePrice - FinalPrice;
        }

        public void EndOneDaySale()
        {
            if (this.mOneDaySaleAlarm != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mOneDaySaleAlarm);
                this.mOneDaySaleAlarm = AlarmHandle.kInvalidHandle;
            }
            base.SetGeometryState("default");
            this.mSaleDiscount = 0f;
        }

        public void GiveLotOwnerMoney(int money, Sim actor)
        {
            Household household = this.FindMoneyGetter();
            if (household != null && household != actor.Household)
            {
                household.ModifyFamilyFunds((int)((float)money));
            }
        }
        public GameObject MakeClone(GameObject src)
        {
            IGameObject gameObject = GlobalFunctions.CreateObject(src.GetResourceKeyForClone(true), Vector3.OutOfWorld, 0, Vector3.UnitZ);
            if (!(gameObject is FailureObject))
            {
                SortedList<string, bool> enabledStencils = new SortedList<string, bool>();
                SortedList<string, Complate> patterns = Complate.ExtractPatterns(src.ObjectId, enabledStencils);
                DesignModeSwap designModeSwap = Complate.SetupDesignSwap(gameObject.ObjectId, patterns, false, enabledStencils);
                if (designModeSwap != null)
                {
                    designModeSwap.ApplyToObject();
                }
            }
            return (GameObject)gameObject;
        }
        public void AccumulateRevenue(int money)
        {
            if (money < 0)
            {
                money = 0;
            }
            this.mLifetimeSaleCount += 1uL;
            this.mLifetimeRevenue += (ulong)((long)money);
            if (SimClock.IsSameDay(this.mLastRevenue, SimClock.CurrentTime()))
            {
                this.mDailyRevenue += (ulong)((long)money);
                return;
            }
            this.mLastRevenue = SimClock.CurrentTime();
            this.mDailyRevenue = (ulong)((long)money);
        }
        public ulong GetLifetimeSaleCount()
        {
            return this.mLifetimeSaleCount;
        }
        public ulong GetLifetimeRevenue()
        {
            return this.mLifetimeRevenue;
        }
        public ulong GetDailyRevenue()
        {
            if (SimClock.IsSameDay(this.mLastRevenue, SimClock.CurrentTime()))
            {
                return this.mDailyRevenue;
            }
            return 0uL;
        }
        public void StartSale(float amount)
        {
            this.mSaleDiscount = amount;
            if (this.mOneDaySaleAlarm != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mOneDaySaleAlarm);
                this.mOneDaySaleAlarm = AlarmHandle.kInvalidHandle;
            }
            if (amount == 0f)
            {
                base.SetGeometryState("default");
                return;
            }
            this.mOneDaySaleAlarm = base.AddAlarm(1f, TimeUnit.Days, new AlarmTimerCallback(this.EndOneDaySale), "OneDaySale", AlarmType.AlwaysPersisted);
            base.SetGeometryState("Sale");
        }
        public override bool PackForMovingOverride()
        {
            return true;
        }
    }

}
