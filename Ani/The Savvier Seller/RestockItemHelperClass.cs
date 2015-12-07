using System.Collections.Generic;
using ani_StoreSetBase;
using ani_StoreSetRegister;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreRestockItem;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreSetBase;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreSetRegister;
using Sims3.Gameplay.Objects.Toys;
using Sims3.Gameplay.Utilities;
using Sims3.Store.Objects;
using Sims3.Metadata;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Reflection;

namespace ani_StoreRestockItem
{
	public static class RestockItemHelperClass
	{
		public static GameObject RecreateSoldObject(RestockItem restockItem, SimDescription actor)
		{
			try
			{
				IGameObject gameObject = null;
				bool restockBuyMode = false;
				bool restockCraftable = false;
				StoreSetRegister register = null;

				bool isRug;
				StoreSetBase shopBase = ReturnStoreSetBase(restockItem, out isRug);

				if (shopBase != null)
				{
					if (shopBase.Info.RegisterId != ObjectGuid.InvalidObjectGuid)
					{
						register = CMStoreSet.ReturnRegister(shopBase.Info.RegisterId, shopBase.LotCurrent);
					}

					restockBuyMode = shopBase.Info.RestockBuyMode;
					restockCraftable = shopBase.Info.RestockCraftable;

					#region Find the slot

					bool slotFound = false;
					Slot slot = Slot.ContainmentSlot_0;
					if (!isRug)
					{
						Slot[] containmentSlots = shopBase.GetContainmentSlots();

						if (containmentSlots != null)
						{
							for (int i = 0; i < containmentSlots.Length; i++)
							{
								GameObject o = shopBase.GetContainedObject(containmentSlots[i]) as GameObject;

								if (o != null && o.ObjectId == restockItem.ObjectId)
								{
									slotFound = true;
									slot = containmentSlots[i];
									break;
								}
							}
						}
					}
					#endregion

					//Restock from inventory only, if not buy object and linked to register
					bool restockFromInventory = RestockFromInventory(restockItem, restockCraftable);

					//Restock from buy mode
					#region Buy Mode
					if (!restockFromInventory)
					{
						if (restockItem.info.Type == ItemType.Buy || restockItem.info.Type == ItemType.Craftable)
						{
							gameObject = GlobalFunctions.CreateObject(restockItem.info.Key, restockItem.Position, restockItem.mLevel, restockItem.ForwardVector);
							if (!(gameObject is FailureObject))
							{
								if (!string.IsNullOrEmpty(restockItem.info.DesignPreset))
								{
									SortedList<string, bool> enabledStencils = new SortedList<string, bool>();
									SortedList<string, Complate> patterns = StoreHelperClass.ExtractPatterns(restockItem.info.DesignPreset,enabledStencils);
									DesignModeSwap designModeSwap = Complate.SetupDesignSwap(gameObject.ObjectId, patterns, false, enabledStencils);
									if (designModeSwap != null)
									{
										designModeSwap.ApplyToObject();
									}
								}
							}
						}
						else
						{
							gameObject = ReturnShoppingObject(restockItem, actor, register);
							gameObject.AddToWorld();
							gameObject.SetPosition(restockItem.Position);                          
						}
						#region Pay for Restock

						//Reduce from base owner or register's owner
						if (shopBase.Info.Owner != 0uL)
						{
							SimDescription sd = CMStoreSet.ReturnSim(shopBase.Info.Owner);
							if (sd != null)
							{
								sd.ModifyFunds(-restockItem.info.Price);
							}
							else
							{
								CMStoreSet.PrintMessage("Couldn't find owner sim");
							}
						}
						else if (shopBase.Info.RegisterId != ObjectGuid.InvalidObjectGuid)
						{
							//StoreSetRegister register = CMStoreSet.ReturnRegister(shopBase.Info.RegisterId, shopBase.LotCurrent);
							if (register != null && register.Info.OwnerId != 0uL)
							{
								SimDescription sd = CMStoreSet.ReturnSim(register.Info.OwnerId);
								if (sd != null)
								{
									sd.ModifyFunds(-restockItem.info.Price);
								}
							}
						}

						#endregion
					}
					#endregion Buy Mode

					#region Inventory
					else
					{
						//Restock from Inventory                
						if (shopBase != null && shopBase.Info.RegisterId != ObjectGuid.InvalidObjectGuid)
						{
							gameObject = ReturnRestocableObject(restockItem, shopBase.Info.RegisterId);

							if (gameObject != null)
							{
								gameObject.AddToWorld();
								gameObject.SetPosition(restockItem.Position);
								gameObject.SetForward(restockItem.ForwardVector);
							}
							else
							{
								CMStoreSet.PrintMessage("Restockable object null");
							}
						}
					}
					#endregion Inventory

					//Delete restock object
					if (restockItem != null)
						restockItem.Destroy();

					//Add restocked item back to slot                
					if (slotFound)
					{
						IGameObject io = (IGameObject)shopBase;
						gameObject.ParentToSlot(io, slot);
					}


					return (GameObject)gameObject;
				}
				else
				{
					return null;
				}
			}
			catch (System.Exception ex)
			{
				CMStoreSet.PrintMessage("RecreateSoldObject: " + ex.Message);
				return null;
			}

		}

		public static GameObject ReturnShoppingObject(RestockItem rItem, SimDescription actor, StoreSetRegister register)
		{
			GameObject o = null;
			bool keepLooping = true;

			switch (rItem.info.Type)
			{
			case ItemType.Herb:
			case ItemType.Ingredient:
				foreach (KeyValuePair<string, List<StoreItem>> kvp in Grocery.mItemDictionary)
				{
					foreach (StoreItem item in kvp.Value)
					{
						if (item.Name.Equals(rItem.info.Name))
						{
							keepLooping = false;
							IngredientData data = (IngredientData)item.CustomData;
							if (rItem.info.Type == ItemType.Ingredient)
							{                                   
								o = Ingredient.Create(data);

							}
							else
							{
								o = Herb.Create(data);
								//PlantableNonIngredientData data = (PlantableNonIngredientData)item.CustomData;
								//o = (GameObject)PlantableNonIngredient.Create(data);
							}
							break;
						}
					}
					if (!keepLooping)
						break;
				}

				break;
			case ItemType.Fish:
				o = Fish.CreateFishOfRandomWeight(rItem.info.FType);
				break;

			case ItemType.Craftable:
				break;
			case ItemType.Gem:
			case ItemType.Metal:

				o = (GameObject)RockGemMetalBase.Make(rItem.info.RockData, false);
				break;
			case ItemType.Nectar:

				NectarBottle bottle = (NectarBottle)GlobalFunctions.CreateObjectOutOfWorld("NectarBottle");
				NectarBottleObjectInitParams nectarBottleObjectInitParams = bottle.CreateAncientBottle(rItem.info.NectarAge, rItem.info.Price);

				if (nectarBottleObjectInitParams != null)
				{
					bottle.mBottleInfo.FruitHash = nectarBottleObjectInitParams.FruitHash;
					bottle.mBottleInfo.Ingredients = nectarBottleObjectInitParams.Ingredients;
					bottle.mBottleInfo.Name = rItem.info.Name;//nectarBottleObjectInitParams.Name;
					bottle.mDateString = nectarBottleObjectInitParams.DateString;
					bottle.mBottleInfo.DateNum = nectarBottleObjectInitParams.DateNum;
					bottle.mBaseValue = rItem.info.Price;// nectarBottleObjectInitParams.BaseValue;
					bottle.ValueModifier = (int)(nectarBottleObjectInitParams.CurrentValue - rItem.info.Price);
					bottle.mBottleInfo.mCreator = nectarBottleObjectInitParams.Creator;
					bottle.mBottleInfo.NectarQuality = Sims3.Gameplay.Objects.Quality.Neutral;//NectarBottle.GetQuality((float)rItem.info.Price);
					bottle.mBottleInfo.MadeByLevel10Sim = nectarBottleObjectInitParams.MadeByLevel10Sim;
					bottle.UpdateVisualState();
				}

				o = bottle;

				break;
			case ItemType.AlchemyPotion:

				foreach (AlchemyRecipe recipe in AlchemyRecipe.GetAllAwardPotionRecipes())
				{
					if (rItem.info.Name.Equals(recipe.Name))
					{
						string[] array = new string[] { recipe.Key };

						AlchemyRecipe randomAwardPotionRecipe = AlchemyRecipe.GetRandomAwardPotionRecipe();
						PotionShopConsignmentRegister.PotionShopConsignmentRegisterData potionShopConsignmentRegisterData = new PotionShopConsignmentRegister.PotionShopConsignmentRegisterData();

						potionShopConsignmentRegisterData.mParameters = array;
						potionShopConsignmentRegisterData.mObjectName = randomAwardPotionRecipe.MedatorName;
						potionShopConsignmentRegisterData.mGuid = potionShopConsignmentRegisterData.mObjectName.GetHashCode();
						potionShopConsignmentRegisterData.mSellerAge = CASAgeGenderFlags.None;
						potionShopConsignmentRegisterData.mWeight = 100f;
						potionShopConsignmentRegisterData.mSellPriceMinimum = 0.75f;
						potionShopConsignmentRegisterData.mSellPriceMaximum = 1.25f;
						potionShopConsignmentRegisterData.mDepreciationAgeMinimum = 0;
						potionShopConsignmentRegisterData.mDepreciationAgeMaximum = 5;
						potionShopConsignmentRegisterData.mLifespan = 3f;
						string text = string.Empty;
						if (!string.IsNullOrEmpty(randomAwardPotionRecipe.CustomClassName))
						{
							text = randomAwardPotionRecipe.CustomClassName;
						}
						else
						{
							text = "Sims3.Gameplay.Objects.Alchemy.AlchemyPotion";
						}
						potionShopConsignmentRegisterData.mScriptClass = text;
						potionShopConsignmentRegisterData.mIsRotatable = true;

						PotionShopConsignmentRegister.PotionShopConsignmentRegisterObjectData potionShopConsignmentRegisterObjectData2 = PotionShopConsignmentRegister.PotionShopConsignmentRegisterObjectData.Create(potionShopConsignmentRegisterData);
						potionShopConsignmentRegisterObjectData2.ShowTooltip = true;

						o = (GameObject)potionShopConsignmentRegisterObjectData2.mObject;


						break;
					}
				}

				break;
			case ItemType.Bug:
				Terrarium t = Terrarium.Create(rItem.info.BugType);
				if (t != null)
					t.StartVfx();
				o = t;
				break;

			case ItemType.Food:
				int servingPrice = 25;
				if (register != null)
					servingPrice = register.Info.ServingPrice;

				IFoodContainer container = rItem.info.cookingProcess.Recipe.CreateFinishedFood(rItem.info.cookingProcess.Quantity, rItem.info.cookingProcess.Quality);

				if (rItem.info.cookingProcess.Quantity == Recipe.MealQuantity.Group)
				{
					((ServingContainerGroup)container).mPurchasedPrice = StoreHelperClass.ReturnPriceByQuality(rItem.info.cookingProcess.Quality, servingPrice * ((ServingContainerGroup)container).mNumServingsLeft);
					((ServingContainerGroup)container).RemoveSpoilageAlarm();
				}
				else
				{
					((ServingContainerSingle)container).mPurchasedPrice = StoreHelperClass.ReturnPriceByQuality(rItem.info.cookingProcess.Quality, servingPrice);
					((ServingContainerSingle)container).RemoveSpoilageAlarm();
				}

				o = (GameObject)container;

				break;

			case ItemType.Flowers:                    
				o = Wildflower.CreateWildflowerOfType(rItem.info.TypeOfWildFlower, Wildflower.WildflowerState.InVase);
				break;

			case ItemType.BookAlchemyRecipe_:
			case ItemType.BookComic_:
			case ItemType.BookFish_:
			case ItemType.BookGeneral_:
			case ItemType.BookRecipe_:
			case ItemType.BookSkill_:
			case ItemType.BookToddler_:
			case ItemType.SheetMusic_:
			case ItemType.AcademicTextBook_:

				foreach (KeyValuePair<string, List<StoreItem>> kvp in Bookstore.sItemDictionary)
				{
					foreach (StoreItem item in kvp.Value)
					{
						if (item.Name.Equals(rItem.info.Name))
						{
							keepLooping = false;

							if (rItem.info.Type == ItemType.BookGeneral_)
								o = (GameObject)BookGeneral.CreateOutOfWorld(item.CustomData as BookGeneralData);
							if (rItem.info.Type == ItemType.BookSkill_)
								o = (GameObject)BookSkill.CreateOutOfWorld(item.CustomData as BookSkillData);
							if (rItem.info.Type == ItemType.BookRecipe_)
								o = (GameObject)BookRecipe.CreateOutOfWorld(item.CustomData as BookRecipeData);
							if (rItem.info.Type == ItemType.SheetMusic_)
								o = (GameObject)SheetMusic.CreateOutOfWorld(item.CustomData as SheetMusicData);
							if (rItem.info.Type == ItemType.BookToddler_)
								o = (GameObject)BookToddler.CreateOutOfWorld(item.CustomData as BookToddlerData);
							if (rItem.info.Type == ItemType.BookFish_)
								o = (GameObject)BookFish.CreateOutOfWorld(item.CustomData as BookFishData);
							if (rItem.info.Type == ItemType.BookAlchemyRecipe_)
								o = (GameObject)BookAlchemyRecipe.CreateOutOfWorld(item.CustomData as BookAlchemyRecipeData);
							if (rItem.info.Type == ItemType.AcademicTextBook_)
								o = (GameObject)AcademicTextBook.CreateOutOfWorld(item.CustomData as AcademicTextBookData, actor);
							if (rItem.info.Type == ItemType.BookComic_)
								o = (GameObject)BookComic.CreateOutOfWorld(item.CustomData as BookComicData);

							break;
						}
					}
					if (!keepLooping)
						break;
				}


				break;
			case ItemType.JamJar:
				JamJar jamJar = GlobalFunctions.CreateObjectOutOfWorld ("canningJarJam", ProductVersion.Store) as JamJar;

				if (jamJar != null) {
					Type tInfo = jamJar.GetType ();
					BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
					FieldInfo ingredientDataField = tInfo.GetField ("mData", flags);
					FieldInfo ingredientKeyField = tInfo.GetField ("mIngredientKey", flags);
					FieldInfo qualityField = tInfo.GetField ("mQuality", flags);
					FieldInfo preservesField = tInfo.GetField ("mIsPreserves", flags);

					ingredientDataField.SetValue (jamJar, rItem.info.IngData);
					ingredientKeyField.SetValue (jamJar, rItem.info.IngredientKey);
					qualityField.SetValue (jamJar, rItem.info.JamQuality);
					preservesField.SetValue (jamJar, rItem.info.JamIsPreserve);
				}
				o = (GameObject)jamJar;
				break;
			default:
				break;
			}

			return o;
		}

		public static StoreSetBase ReturnStoreSetBase(GameObject gameObject, out bool isRug)
		{
			isRug = false;

			if (gameObject == null)
				CMStoreSet.PrintMessage("ReturnStoreSetBase: null");

			GameObject sBase = FindParentShopBase(gameObject);
			if (sBase != null)
			{
				if (sBase.GetType() == typeof(ani_StoreRug))
					isRug = true;

				return (StoreSetBase)sBase;
			}
			return null;
		}

		public static bool RestockFromInventory(RestockItem restockItem, bool restockCraftable)
		{
			if (restockItem.info.Type != ItemType.Buy && !restockCraftable)
			{
				return true;
			}
			return false;
		}

		public static StoreSetBase FindParentShopBase(GameObject gameObject)
		{
			GameObject stand = null;
			List<StoreSetBase> bases = new List<StoreSetBase>(Sims3.Gameplay.Queries.GetObjects<StoreSetBase>(gameObject.LotCurrent));

			foreach (StoreSetBase b in bases)
			{
				GameObject[] objectsIntersectingObject = Sims3.Gameplay.Queries.GetObjectsIntersectingObject(b.ObjectId);
				if (objectsIntersectingObject != null)
				{
					for (int i = 0; i < objectsIntersectingObject.Length; i++)
					{
						if (objectsIntersectingObject[i].ObjectId == gameObject.ObjectId)
						{
							stand = b as GameObject;
							break;
						}
					}
					if (stand != null)
						break;
				}
			}
			if (stand != null)
				return (StoreSetBase)stand;
			else return null;
		}

		public static GameObject ReturnRestocableObject(RestockItem rItem, ObjectGuid registerId)
		{
			StoreSetRegister register = null;
			GameObject restockObject = null;

			//If item is linked to a register, restock from the correct inventory
			if (registerId != ObjectGuid.InvalidObjectGuid)
			{
				register = CMStoreSet.ReturnRegister(registerId, rItem.LotCurrent);
				restockObject = ReturnRestocableObjectFromRegister(rItem, registerId);
			}

			if (restockObject != null && register != null && !register.Inventory.TryToRemove(restockObject))
				restockObject = null;

			return restockObject;

		}

		public static GameObject ReturnRestocableObjectFromRegister(RestockItem rItem, ObjectGuid registerId)
		{
			GameObject restockObject = null;
			bool continueLooping = true;

			StoreSetRegister register = CMStoreSet.ReturnRegister(registerId, rItem.LotCurrent);

			if (register != null)
			{
				foreach (var stack in register.Inventory.InventoryItems.Values)
				{
					if (!continueLooping)
						break;

					foreach (var item in stack.List)
					{
						ItemType type = RestockItemHelperClass.GetItemType((GameObject)item.Object);

						if (rItem.info.Type == type)
						{
							//Does the name match
							if (rItem.info.Name.Equals(item.Object.GetLocalizedName()))
							{
								restockObject = (GameObject)item.Object;
								continueLooping = false;
								break;
							}
						}
					}
				}
			}
			return restockObject;
		}

		public static ItemType GetItemType(GameObject soldItem)
		{
			ItemType type = ItemType.Buy;

			if (soldItem.GetType () == typeof(Invention))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(NectarBottle))
				type = ItemType.Nectar;
			else if (soldItem.GetType () == typeof(Ingredient) || soldItem.GetType () == typeof(Pumpkin))
				type = ItemType.Ingredient;
			else if (soldItem.GetType () == typeof(Fish))
				type = ItemType.Fish;
			else if (soldItem.GetType () == typeof(Metal))
				type = ItemType.Metal;
			else if (soldItem.GetType () == typeof(Gem))
				type = ItemType.Gem;
			else if (soldItem.GetType () == typeof(Peppermint) || soldItem.GetType () == typeof(Chamomile)
				|| soldItem.GetType () == typeof(Cinnamon) || soldItem.GetType () == typeof(Greenleaf)
				|| soldItem.GetType () == typeof(Basil) || soldItem.GetType () == typeof(Bumbleleaf)
				|| soldItem.GetType () == typeof(Buzzberry) || soldItem.GetType () == typeof(Ginseng)
				|| soldItem.GetType () == typeof(Lavender) || soldItem.GetType () == typeof(Licorice)
				|| soldItem.GetType () == typeof(Wonderpetal))
				type = ItemType.Herb;
			else if (soldItem.GetType () == typeof(AlchemyPotion))
				type = ItemType.AlchemyPotion;
			else if (soldItem.GetType () == typeof(NormalTerrarium))
				type = ItemType.Bug;
			else if (soldItem.GetType () == typeof(Wildflower))
				type = ItemType.Flowers;
			else if (soldItem.GetType () == typeof(PlateServing))
				type = ItemType.Food;
			else if (soldItem.GetType () == typeof(BookGeneral))
				type = ItemType.BookGeneral_;
			else if (soldItem.GetType () == typeof(BookSkill))
				type = ItemType.BookSkill_;
			else if (soldItem.GetType () == typeof(BookRecipe))
				type = ItemType.BookRecipe_;
			else if (soldItem.GetType () == typeof(SheetMusic))
				type = ItemType.SheetMusic_;
			else if (soldItem.GetType () == typeof(BookToddler))
				type = ItemType.BookToddler_;
			else if (soldItem.GetType () == typeof(BookFish))
				type = ItemType.BookFish_;
			else if (soldItem.GetType () == typeof(BookAlchemyRecipe))
				type = ItemType.BookAlchemyRecipe_;
			else if (soldItem.GetType () == typeof(AcademicTextBook))
				type = ItemType.AcademicTextBook_;
			else if (soldItem.GetType () == typeof(BookComic))
				type = ItemType.BookComic_;
			else if (soldItem.GetType () == typeof(CraftedToyBoxToy))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(CraftedCow))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(CraftedDog))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(CraftedRobot))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(CraftedToy4))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(DrinkingLlama))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(WindUpToy))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(Carousel))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(StaticCoil))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(Levitation))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(DummyInvention))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(Sims3.Gameplay.Objects.HobbiesSkills.Inventing.FishTank))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(Harvester))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(Invention))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(Miner))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(MoonGlobe))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(NewtonsCradle))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(Sims3.Gameplay.Objects.HobbiesSkills.Inventing.Robot))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(TimeMachine))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(Widget))
				type = ItemType.Craftable;
			else if (soldItem.GetType () == typeof(JamJar))
				type = ItemType.JamJar;


			//|| soldItem.GetType() == typeof(Photograph) || soldItem.GetType() == typeof(Sculpture)
			//|| 
			//|| soldItem.GetType() == typeof(ServingContainer) || soldItem.GetType() == typeof(ServingContainerGroup))
			//{
			//    return false;
			//}


			return type;
		}

	}
}

