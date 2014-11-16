using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Register;
using System.Collections.Generic;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Abstracts;
using System;
using Sims3.Gameplay.CAS;
using Sims3.UI;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.TombObjects;
using System.Text;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Decorations;
namespace SellFromInventory
{
    public class RegisterInteraction
    {
        public class ShopFromInventory : Interaction<Sim, ShoppingRegister>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();
            public static List<object> purchasedItems;

            // Methods          
            public override bool Run()
            {
                //Navigate to the register
                if (!base.Actor.RouteToPointRadialRange(base.Target.Position, 0.5f, 2f))
                {
                    return false;
                }


                purchasedItems = new List<object>();
                if (base.Actor.Household.IsActive && this.GetPriority().Level == InteractionPriorityLevel.UserDirected)
                {

                    //ShowShoppingDialog();
                    purchasedItems = base.SelectedObjects;
                    FinishedCallBack(base.Actor);
                }
                else
                {
                    if (!base.Actor.Household.IsActive)
                    {

                        //Buy and go home
                        if (Buy(AddMenuItem.ReturnCustomerPrecentage()))
                        {
                            purchasedItems = ReturnRandomObject();
                            FinishedCallBack(base.Actor);
                        }
                        Sim.MakeSimGoHome(base.Actor, false);
                    }
                }

                return true;
            }

            /// <summary>
            /// Show shop from inventory shopping dialogue
            /// </summary>
            public void ShowShoppingDialog()
            {
                // Dictionary<string, List<StoreItem>> itemDictionary = new Dictionary<string, List<StoreItem>>();

                // //Register type
                // RegisterType type = RegisterType.General;
                // if (base.Target.GetType() == typeof(FoodStoreRegister))
                //     type = RegisterType.Food;
                // if (base.Target.GetType() == typeof(BookStoreRegister))
                //     type = RegisterType.Books;


                // //Get list of buyable items
                //itemDictionary = CommonMethods.ItemDictionary(base.Actor, type, base.Target.LotCurrent);              

                // CommonMethods.ShowCustomShoppingDialog(base.Actor, base.Target, itemDictionary);
            }


            #region Finished Shopping
            /// <summary>
            /// Finished shopping
            /// </summary>
            /// <param name="customer"></param>
            public void FinishedCallBack(Sim customer)
            {
                int price = 0;

                if (purchasedItems != null && purchasedItems.Count > 0)
                {
                    foreach (GameObject item in purchasedItems)
                    {
                        price += (int)ReturnItemPrice(item);
                    }
                }

                //Can the sim afford the purchase
                if (base.Actor.FamilyFunds >= price)
                {
                    HandlePurchases(purchasedItems);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public static float ReturnItemPrice(GameObject obj)
            {
                bool isFood = false;
                float objectValue = (float)obj.Value;
                SimDescription productMaker = CommonMethods.ReturnCreaotrSimDescription(obj);

                //Food has no value by default
                if (objectValue == 0)
                {
                    ServingContainer pf = obj as ServingContainer;
                    if (pf != null)
                    {
                        //Set price depending on quality
                        pf.RemoveSpoilageAlarm();
                        //   StyledNotification.Show(new StyledNotification.Format(pf.Cost + " " + pf.FoodQuality, StyledNotification.NotificationStyle.kGameMessagePositive));
                        isFood = true;
                        switch (pf.FoodQuality)
                        {
                            case Quality.Foul:
                            case Quality.Horrifying:
                            case Quality.Bad:
                            case Quality.Putrid:
                                objectValue = AddMenuItem.ReturnFoodPriceBad();
                                break;
                            case Quality.Nice:
                            case Quality.VeryNice:
                                objectValue = AddMenuItem.ReturnFoodPriceNice();
                                break;
                            case Quality.Great:
                            case Quality.Outstanding:
                            case Quality.Excellent:
                                objectValue = AddMenuItem.ReturnFoodPriceGreat();
                                break;
                            case Quality.Perfect:
                                objectValue = AddMenuItem.ReturnFoodPricePerfect();
                                break;
                            default:
                                objectValue = AddMenuItem.ReturnFoodPriceDefault();
                                break;
                        }
                    }
                }

                //If the product has no maker, multiply by 1.3
                float priceIncrease = objectValue * (1 + (float) AddMenuItem.ReturnProfitForNoneGrownGoods() / 100);
                               
                if (productMaker == null && !isFood)
                {
                    if (priceIncrease < (objectValue + 1))
                    {
                        objectValue += 1;
                    }
                    else
                    {
                        objectValue = priceIncrease;
                    }
                }
                else if (obj.GetType() == typeof(Ingredient))
                {
                    objectValue *= (1 + (float)AddMenuItem.ReturnProfitForIngredients() / 100);
                   
                }

                return objectValue;
            }

            #endregion

            #region Buy methods
            /// <summary>
            /// Returns random object for in-active household shopping
            /// </summary>
            /// <returns></returns>
            public List<object> ReturnRandomObject()
            {
                List<object> randomObjects = new List<object>();

                List<GameObject> allObjects = CommonMethods.ItemDictionary(base.Actor, CommonMethods.ReturnRegisterType(base.Target), base.Target.LotCurrent);

                int numberOfItems = 1;

                //Get the random object 
                if (numberOfItems <= allObjects.Count)
                {
                    randomObjects.Add(allObjects[new Random().Next((allObjects.Count))]);
                }
                return randomObjects;
            }

            public static Boolean Buy(int precentage)
            {
                Boolean buy = false;

                if ((new Random().Next(1, 101)) <= precentage)
                {
                    buy = true;
                }

                return buy;

            }
            /// <summary>
            /// Handle payments and object inventory swapping.
            /// </summary>
            /// <param name="purchasedItems"></param>
            public void HandlePurchases(List<object> purchasedItems)
            {
                try
                {
                    Boolean okToShop = true;

                    if (purchasedItems.Count > 0 && okToShop)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(CommonMethods.LocalizeString("ItemsPurchasedBySim", new object[] { base.Actor.Name }));

                        //Get the lot owner 
                        Household ownerHousehold = CommonMethods.ReturnLotOwner(base.Target.LotCurrent);

                        int purchaseTotal = 0;
                        int lotOwnerIncome = 0;

                        //Get Shop inventory
                        List<StorageInventory> storeInventory = CommonMethods.ReturnStorageInventory(base.Actor, base.Target.LotCurrent);

                        //Loop through the items that were purchased
                        foreach (GameObject item in purchasedItems)
                        {
                            StorageInventory si = storeInventory.Find(delegate(StorageInventory o) { return o.ItemGuid == item.ObjectId; });

                            List<TreasureChest> tList = CommonMethods.ReturnTreasureChests(base.Actor, base.Target.LotCurrent);

                            int price = 0;
                            int objectValue = (int)ReturnItemPrice(item);

                            if (si != null)
                            {
                                //Find the chest the object is in 
                                TreasureChest tc = tList.Find(delegate(TreasureChest t) { return t.ObjectId == si.StorageGuid; });

                                GameObject storageObject = null;

                                if (tc != null)
                                {
                                    storageObject = tc;
                                }

                                if (storageObject != null)
                                {
                                    //Did removing of the item succeed
                                    if (storageObject.Inventory.TryToRemove(item))
                                    {
                                        //If the product was made by a sim who is not a member of the lot owner household
                                        //Give the maker, 50% of the value

                                        SimDescription productMaker = CommonMethods.ReturnCreaotrSimDescription(item);

                                        if (productMaker != null && !productMaker.IsDead)
                                        {
                                            //Devide the earnings                                             
                                            if (ownerHousehold != null)
                                            {
                                                price = objectValue / 2;
                                                lotOwnerIncome += price;
                                            }
                                            else
                                            {
                                                price = objectValue;
                                            }

                                            //lotOwner.ModifyFamilyFunds(price);
                                            productMaker.Household.ModifyFamilyFunds(price);

                                            //If the owner of the pruduct is in a skill based career, and the item they sold is related to their job
                                            //Count it towards their job performance
                                            if (productMaker != null && productMaker.Occupation != null && productMaker.Occupation.IsSkillBased)
                                            {
                                                //First check that the buyer and the product maker are not in the same household
                                                if (base.Actor.Household != productMaker.Household)
                                                {
                                                    UpdateSkillBasedCareerEarning(productMaker, item, price);
                                                }
                                            }

                                            purchaseTotal += objectValue;
                                        }
                                        else
                                        {
                                            //The item doesn't have an owner
                                            //Add 30% to the price, if not the active Household
                                            price = (int)objectValue; //(int)(objectValue * 1.3);

                                            purchaseTotal += price;


                                            //If the lot is owned by a sim
                                            if (ownerHousehold != null)
                                            {
                                                lotOwnerIncome += price;

                                                //If the item was harvested, we assume it was planted by the owner or somebody in his household                                               
                                                // lotOwner.ModifyFamilyFunds(price);

                                                //Collect all sims from the lot, and if their occupation is gardener, 
                                                //devide the money earned from the purchase between them

                                                if (item.GetType() == typeof(Ingredient))
                                                {
                                                    if (((Ingredient)item).CraftType == CraftType.Fruit)
                                                    {
                                                        List<Sim> gardnerSims = new List<Sim>();
                                                        foreach (Sim sim in ownerHousehold.Sims)
                                                        {
                                                            if (sim.SimDescription.YoungAdultOrAbove && sim.SimDescription.OccupationAsSkillBasedCareer != null)
                                                            {
                                                                if (sim.SimDescription.OccupationAsSkillBasedCareer.CareerName.Equals(OccupationNames.Gardener.ToString()))
                                                                {
                                                                    gardnerSims.Add(sim);
                                                                }
                                                            }
                                                        }

                                                        if (gardnerSims != null && gardnerSims.Count > 0)
                                                        {
                                                            foreach (Sim sim in gardnerSims)
                                                            {
                                                                int simoleansEarned = (price / gardnerSims.Count) > 1 ? (price / gardnerSims.Count) : 1;
                                                                UpdateSkillBasedCareerEarning(sim.SimDescription, item, simoleansEarned);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        base.Actor.Inventory.TryToAdd(item, false);
                                        storeInventory.Remove(si);
                                        sb.Append(CommonMethods.LocalizeString("PurchasedItem", new object[] { item.CatalogName, price }));

                                    }
                                    else
                                    {
                                        StyledNotification.Show(new StyledNotification.Format("Failed to remove " + item.CatalogName, StyledNotification.NotificationStyle.kGameMessagePositive));
                                    }
                                }


                            }
                        }

                        if (purchaseTotal > 0)
                        {
                            base.Actor.ModifyFunds(-purchaseTotal);

                            sb.Append(CommonMethods.LocalizeString("PurchaseTotal", new object[] { purchaseTotal }));

                            //Give the money earned from the purchase to the owner of the lot 
                            if (lotOwnerIncome > 0 && ownerHousehold != null)
                            {
                                ownerHousehold.ModifyFamilyFunds(lotOwnerIncome);
                                sb.Append(CommonMethods.LocalizeString("LotOwnerEarned", new object[] { ownerHousehold.Name, lotOwnerIncome }));
                            }

                            //Show only if wanted
                            if (AddMenuItem.ReturnShowNotifications())
                                StyledNotification.Show(new StyledNotification.Format(sb.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive));
                        }
                    }
                }
                catch (Exception ex)
                {
                    StyledNotification.Show(new StyledNotification.Format(ex.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive));
                }
            }

            public void UpdateSkillBasedCareerEarning(SimDescription sd, GameObject soldItem, int itemValue)
            {
                if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.Inventor) && soldItem.GetType() == typeof(Invention))
                {
                    sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Inventing, itemValue);
                }

                if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.NectarMaker) && soldItem.GetType() == typeof(NectarBottle))
                {
                    sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Nectar, itemValue);
                }

                if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.Gardener) && soldItem.GetType() == typeof(Ingredient))
                {
                    sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Gardening, itemValue);
                }

                if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.Fisher) && soldItem.GetType() == typeof(Fish))
                {
                    sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Fishing, itemValue);
                }

                if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.Photographer) && soldItem.GetType() == typeof(Photograph))
                {
                    sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Photography, itemValue);
                }

                if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.Sculptor) && soldItem.GetType() == typeof(Sculpture))
                {
                    sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Sculpting, itemValue);
                }


            }
            #endregion

            // Nested Types
            public sealed class Definition : InteractionDefinition<Sim, ShoppingRegister, RegisterInteraction.ShopFromInventory>
            {
                // Methods  
                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    headers = new List<ObjectPicker.HeaderInfo>();
                    listObjs = new List<ObjectPicker.TabInfo>();
                    headers.Add(new ObjectPicker.HeaderInfo("Ui/Caption/ObjectPicker:ObjectName", "Ui/Tooltip/ObjectPicker:Name", 250));
                    headers.Add(new ObjectPicker.HeaderInfo("Ui/Caption/Shopping/Cart:Price", "Ui/Tooltip/Shopping/Cart:Price"));

                    List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();
                    Sim actor = parameters.Actor as Sim;
                    ShoppingRegister register = parameters.Target as ShoppingRegister;
                                    

                    if (actor != null)
                    {
                        List<GameObject> list2 = CommonMethods.ItemDictionary(actor, CommonMethods.ReturnRegisterType(register), parameters.Target.LotCurrent);
                        NumSelectableRows = list2.Count;                       

                        foreach (GameObject obj2 in list2)
                        {
                            List<ObjectPicker.ColumnInfo> columnInfo = new List<ObjectPicker.ColumnInfo>();

                            ThumbnailKey thumbnailKey = obj2.GetThumbnailKey();

                            columnInfo.Add(new ObjectPicker.ThumbAndTextColumn(thumbnailKey, obj2.GetLocalizedName()));

                            columnInfo.Add(new ObjectPicker.MoneyColumn((int)ReturnItemPrice(obj2)));
                            ObjectPicker.RowInfo info = new ObjectPicker.RowInfo(obj2, columnInfo);
                            rowInfo.Add(info);
                        }
                    }
                    else
                    {
                        NumSelectableRows = 1;
                    }

                    listObjs.Add(new ObjectPicker.TabInfo("all", "", rowInfo));
                }


                public override string GetInteractionName(Sim a, ShoppingRegister target, InteractionObjectPair interaction)
                {
                    return CommonMethods.LocalizeString("ShopFromInventory", new object[0]);
                }


                public override bool Test(Sim actor, ShoppingRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {

                    if (actor.Household.IsActive && isAutonomous)
                        return false;

                    return true;

                }
            }

        }
    }
}