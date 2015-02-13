using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.TombObjects;
using Sims3.Gameplay.Interfaces;
using Sims3.UI;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay;
using Sims3.UI.Hud;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Careers;

namespace SellFromInventory
{
    public class CommonMethods
    {
        #region Localization
        public static string LocalizeString(string name, params object[] parameters)
        {
            return Localization.LocalizeString("SellFromInventory:" + name, parameters);
        }
        #endregion

        #region Object Type
        public static SimDescription ReturnCreaotrSimDescription(GameObject craftedObject)
        {
            Type t = craftedObject.GetType();

            // StyledNotification.Show(new StyledNotification.Format("Type: " + t.BaseType.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive));


            if (t == typeof(PreparedFood))
            {
                return SimDescription.Find(((PreparedFood)craftedObject).CookingProcess.Preparer.SimDescriptionId);
            }

            if (t == typeof(Fish))
            {
                return ((Fish)craftedObject).FishingSim;
            }
            if (t == typeof(NectarBottle))
            {
                return ((NectarBottle)craftedObject).Creator;
            }

            if (t == typeof(Photograph))
            {
                return ((Photograph)craftedObject).Artist;
            }

            if (t == typeof(Sculpture))
            {
                return ((Sculpture)craftedObject).SculptureComponent.Artist;
            }

            //If an invention            
            Widget wid = craftedObject as Widget;
            if (wid != null)
            {
                return wid.Inventor;
            }

            Invention inv = craftedObject as Invention;
            if (inv != null)
            {
                return inv.Inventor;
            }

            return null;
        }
        #endregion

        #region Item Listings
        /// <summary>
        /// Returns a list of items from the store's inventory
        /// </summary>
        /// <param name="sim"></param>
        /// <returns></returns>
        public static List<GameObject> ItemDictionary(Sim actor, RegisterType type, Lot lot)
        {
            //Nuutti
            //Get all treasure boxes on this lot
            List<GameObject> list = new List<GameObject>();
            List<TreasureChest> tList = ReturnTreasureChests(actor, lot);

            foreach (TreasureChest tChest in tList)
            {
                List<GameObject> cInventoryItems = tChest.Inventory.FindAll<GameObject>(false);

                //Go through list and filter 
                foreach (var item in cInventoryItems)
                {
                    ItemType itemType = ItemType.General;
                    DriedFood driedFood = item as DriedFood;
                    Morsel _morsel = item as Morsel;

                    if (driedFood != null || _morsel != null || item.GetType() == typeof(Morsel) || item.GetType() == typeof(PlateServing)
                                || item.GetType() == typeof(Ingredient)
                                || item.GetType() == typeof(Cake) ||
                            item.GetType() == typeof(Fish))
                    {
                        itemType = ItemType.Food;
                    }
                    else 
                    {
                        NectarBottle nectar = item as NectarBottle;
                        Book book = item as Book;
                        if (book != null)
                        {
                            itemType = ItemType.Book;
                        }
                        else if (nectar != null)
                        {
                            itemType = ItemType.Nectar;
                        }

                    }

                    //Add only to the correct type of register
                    if (type == RegisterType.Food && itemType == ItemType.Food)
                    {
                        list.Add(item);
                    }
                    else if (type == RegisterType.Books && itemType == ItemType.Book)
                    {
                        list.Add(item);
                    }
                    else if (type == RegisterType.General && (itemType == ItemType.General || AddMenuItem.ReturnSellIngInGeneralRegister()))
                    {
                        list.Add(item);
                    }
                    else if (type == RegisterType.Nectar && itemType == ItemType.Nectar)
                    {
                        list.Add(item);
                    }
                }
            }

            return list;
        }

        public static void ShowMessage(string message)
        {
            StyledNotification.Show(new StyledNotification.Format(message, StyledNotification.NotificationStyle.kGameMessagePositive));
        }


        /// <summary>
        ///Returns a list of items from the store's inventory, this method is used when paying 
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public static List<StorageInventory> ReturnStorageInventory(Sim actor, Lot lotCurrent)
        {
            //Catalogue the inventory
            List<StorageInventory> storeInventory = new List<StorageInventory>();


            List<TreasureChest> tList = ReturnTreasureChests(actor, lotCurrent);

            foreach (TreasureChest tChest in tList)
            {
                List<GameObject> cInventoryItems = tChest.Inventory.FindAll<GameObject>(false);

                foreach (IGameObject inventoryItem in cInventoryItems)
                {
                    StorageInventory si = new StorageInventory();
                    si.StorageGuid = tChest.ObjectId;
                    si.ItemGuid = inventoryItem.ObjectId;
                    storeInventory.Add(si);
                }
            }

            return storeInventory;
        }

        /// <summary>
        /// Returns all treasure chests either on this lot, or the owners lot
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public static List<TreasureChest> ReturnTreasureChests(Sim actor, Lot lotCurrent)
        {
            //Get the lot owner 
            Household ownerHousehold = CommonMethods.ReturnLotOwner(lotCurrent);
            Lot ownerLot = null;
            if (ownerHousehold != null)
            {
                ownerLot = ownerHousehold.LotHome;
            }

            List<TreasureChest> tList = new List<TreasureChest>(Sims3.Gameplay.Queries.GetObjects<TreasureChest>()).FindAll(
                                                                                                            delegate(TreasureChest t)
                                                                                                            {
                                                                                                                if (ownerLot != null)
                                                                                                                {
                                                                                                                    return t.LotCurrent == lotCurrent || t.LotCurrent == ownerLot;
                                                                                                                }
                                                                                                                return t.LotCurrent == lotCurrent;
                                                                                                            }
                                                                                                        );

            return tList;
        }

        #endregion

        #region Shopper treasurechest

        public static TreasureChest ShopperTreasureChest(Sim actor)
        {
            //Get all treasure boxes on this lot
            TreasureChest chest = null;
            List<TreasureChest> chests = ReturnTreasureChests(actor, actor.LotHome);

            //Find one
            chest = chests.Find(delegate(TreasureChest t)
                    {
                        return t.LotCurrent == actor.LotHome;
                    });


            return chest;
        }

        #endregion

        #region ReturnOwner
        public static Household ReturnLotOwner(Lot lot)
        {
            Household lotOwner = null;
            if (lot != null)
            {
                //Check first is the lot a rabbit hole lot.
                List<RabbitHole> rList = new List<RabbitHole>(Sims3.Gameplay.Queries.GetObjects<RabbitHole>());
                RabbitHole rhOnThisLot = null;
                foreach (RabbitHole r in rList)
                {
                    if (r.LotCurrent == lot)
                    {
                        rhOnThisLot = r;
                        break;
                    }
                }
                //Is there a rabbit hole on this lot
                if (rhOnThisLot != null)
                {
                    List<Household> hList = new List<Household>(Sims3.Gameplay.Queries.GetObjects<Household>());
                    Sims3.Gameplay.RealEstate.PropertyData pd = null;
                    if (hList != null && hList.Count > 0)
                    {
                        foreach (Household h in hList)
                        {
                            pd = h.RealEstateManager.FindProperty(rhOnThisLot);

                            if (pd != null && pd.Owner != null)
                            {
                                lotOwner = pd.Owner.OwningHousehold;
                                break;
                            }
                        }
                    }
                }

                //If the lot is not a RH lot, check for venues
                if (lotOwner == null)
                {
                    List<Household> hList = new List<Household>(Sims3.Gameplay.Queries.GetObjects<Household>());
                    Sims3.Gameplay.RealEstate.PropertyData pd = null;
                    if (hList != null && hList.Count > 0)
                    {
                        foreach (Household h in hList)
                        {
                            pd = h.RealEstateManager.FindProperty(lot);

                            if (pd != null && pd.Owner != null)
                            {
                                lotOwner = pd.Owner.OwningHousehold;
                                break;
                            }
                        }
                    }
                }
            }
            return lotOwner;
        }
        #endregion

        #region PayLotOwner
        /// <summary>
        /// If we brought an item through a normal game register, pay the lot owner
        /// </summary>
        /// <param name="sim"></param>
        /// <param name="o"></param>
        public static void PayLotOwner(Sim sim, Lot lotCurrent, int price)
        {
            if (price > 0)
            {
                Household lotOwner = ReturnLotOwner(lotCurrent);
                if (lotOwner != null && lotOwner != sim.Household)
                {
                    lotOwner.ModifyFamilyFunds(price);

                    if (AddMenuItem.ReturnShowNotifications())
                        StyledNotification.Show(new StyledNotification.Format(CommonMethods.LocalizeString("LotOwnerEarned", new object[] { lotOwner.Name, price }), StyledNotification.NotificationStyle.kGameMessagePositive));
                }
            }

        }
        #endregion

        #region Calculate brought items

        public static void CalculateBroughtItems(Dictionary<uint, List<ObjectGuid>> inventoryItemStack, Sim sim, GameObject target)
        {

            //  if (sim.Household.IsActive)
            {
                List<ObjectGuid> newItemList = new List<ObjectGuid>();

                foreach (IInventoryItemStack item in sim.InventoryComp.InventoryUIModel.InventoryItems)
                {
                    //new item
                    if (!inventoryItemStack.ContainsKey(item.StackId))
                    {
                        newItemList.AddRange(item.StackObjects);
                    }
                    else
                    {
                        List<ObjectGuid> foundStack = inventoryItemStack[item.StackId];

                        //Check stack
                        if (foundStack.Count < item.StackObjects.Count)
                        {
                            List<ObjectGuid> ids = new List<ObjectGuid>();
                            //Find the objec it 
                            foreach (ObjectGuid g in item.StackObjects)
                            {
                                if (!foundStack.Contains(g))
                                {
                                    ids.Add(g);
                                }
                            }

                            if (ids != null && ids.Count > 0)
                            {
                                newItemList.AddRange(ids);
                            }
                        }
                    }
                }

                //Loop through the new items 
                int totalPrice = 0;
                foreach (ObjectGuid g in newItemList)
                {
                    GameObject itemPurchased = GameObject.GetObject(g);
                    if (itemPurchased != null)
                    {
                        int price = (int)(itemPurchased.Value * (new decimal(AddMenuItem.ReturnProfit()) / 100));
                        if (price == 0)
                        {
                            price = 1;
                        }
                        totalPrice += price;

                    }
                }
                if (newItemList.Count > 0)
                {
                    CommonMethods.PayLotOwner(sim, target.LotCurrent, totalPrice);
                }
            }
        }

        #endregion

        #region Shopping Dialogue
        /// <summary>
        /// Overrided shopping dialogue
        /// </summary>
        /// <param name="sim"></param>
        /// <param name="register"></param>
        /// <returns></returns>
        public static bool ShowCustomShoppingDialog(Sim sim, ShoppingRegister register, Dictionary<string, List<StoreItem>> itemDictionary)
        {
            float num = 1f;
            if ((sim != null) && sim.HasTrait(TraitNames.Haggler))
            {
                num *= 1f - (TraitTuning.HagglerSalePercentAdd / 100f);
            }
            num = (1f - num) * 100f;

            if (itemDictionary.Count == 0)
            {
                return false;
            }


            ShoppingModel.CurrentStore = register;
            ShoppingRabbitHole.StartShopping(sim, itemDictionary, register.PercentModifier, (float)((int)num), 0, null, sim.Inventory, null, new ShoppingRabbitHole.ShoppingFinished(FinishedCallBack), new ShoppingRabbitHole.CreateSellableCallback(register.CreateSellableObjectsList), register.GetRegisterType != RegisterType.General);
            return true;
        }

        /// <summary>
        /// Finished shoppign callback
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="boughtItemForFamilyInventory"></param>
        private static void FinishedCallBack(Sim customer, bool boughtItemForFamilyInventory)
        {
            //StyledNotification.Show(new StyledNotification.Format("finished callback", StyledNotification.NotificationStyle.kGameMessagePositive));

        }
        #endregion

        #region Check Availability

        public static bool IsSociable(Sim sim)
        {
            bool sociable = true;

            //Check if register clerk
            if (sim.SimDescription != null && sim.SimDescription.AssignedRole != null && sim.SimDescription.AssignedRole.Type == Sims3.Gameplay.Roles.Role.RoleType.LocationMerchant)
            {
                sociable = false;
            }

            return sociable;
        }
        #endregion

        #region Register Type
        /// <summary>
        /// Returns register type
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public static RegisterType ReturnRegisterType(ShoppingRegister register)
        {
            RegisterType type = RegisterType.General;

            if (register.GetType() == typeof(FoodStoreRegister))
                type = RegisterType.Food;

            if (register.GetType() == typeof(BookStoreRegister))
                type = RegisterType.Books;

            if (register.GetType() == typeof(NectarRegister))
                type = RegisterType.Nectar;

            return type;
        }
        #endregion

        #region IsIce-cream
        /// <summary>
        /// Is recipe ice-cream
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        internal static bool IsRecipeIceCream(Recipe recipe)
        {
            if (recipe.Key.Equals("IceCreamCone") || recipe.Key.Equals("RainbowPop")
                   || recipe.Key.Equals("FreezerBunnyPop") || recipe.Key.Equals("FudgePop"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
