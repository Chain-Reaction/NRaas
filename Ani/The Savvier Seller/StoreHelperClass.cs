using System;
using System.Collections.Generic;
using ani_StoreRestockItem;
using ani_StoreSetRegister;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreRestockItem;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreSetBase;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreSetRegister;
using Sims3.Gameplay.Visa;
using Sims3.Metadata;
using Sims3.SimIFace;
using Sims3.Store.Objects;
using System.Reflection;

namespace ani_StoreSetBase
{
	public static class StoreHelperClass
	{
		public static GameObject CreateRestockItem(GameObject src, int value, bool isRug)
		{
			try
			{
				RestockItem item = null;

				if (isRug)
				{
					item = GlobalFunctions.CreateObject(ResourceKey.FromString("319e4f1d:00000000:4d2d76202832ac21"),
						src.PositionOnFloor, src.mLevel, src.ForwardVector) as RestockItem;
				}
				else
				{
					item = GlobalFunctions.CreateObject(ResourceKey.FromString("319e4f1d:00000000:74eadf6231a9cf5e"),
						src.PositionOnFloor, src.mLevel, src.ForwardVector) as RestockItem;

				}
				item.info.Key = src.GetResourceKeyForClone(true);
				item.info.Type = RestockItemHelperClass.GetItemType(src);
				item.info.Name = src.GetLocalizedName();
				item.info.Price = value;

				switch (item.info.Type)
				{
				case ItemType.Buy:
				case ItemType.Craftable:
					item.info.DesignPreset = ObjectDesigner.GetObjectDesignPreset(src.ObjectId);
					break;
				case ItemType.Ingredient:
					//item.info.IngData = (IngredientData)((Ingredient)src).Data;
					// item.info.Key = ((Ingredient)src).GetResourceKey();
					//item.info.IngredientKey = ((Ingredient)src).IngredientKey;
					break;
				case ItemType.Fish:
					item.info.FType = ((Fish)src).mFishType;
					break;
				case ItemType.Herb:
					//item.info.PlantData = ((PlantableNonIngredient)src).mData;
					// item.info.Key = ((Herb)src).GetResourceKey();
					break;
				case ItemType.Metal:
					item.info.RockData = ((Metal)src).mGuid;
					item.info.Key = ((Metal)src).GetResourceKey();
					break;
				case ItemType.Gem:
					item.info.RockData = ((Gem)src).mGuid;
					item.info.Key = ((Gem)src).GetResourceKey();
					break;
				case ItemType.Nectar:
					item.info.Key = ((NectarBottle)src).GetResourceKey();
					item.info.NectarAge = ((NectarBottle)src).mBottleInfo.DateNum;

					if (item.info.NectarAge == 0)
						item.info.NectarAge = 1;

					item.info.NectarFruitHash = ((NectarBottle)src).mBottleInfo.FruitHash;
					item.info.NectarIngredients = ((NectarBottle)src).Ingredients;//.in.mBottleInfo;
					break;
				case ItemType.AlchemyPotion:

					break;
				case ItemType.Bug:
					item.info.BugType = ((NormalTerrarium)src).mInsectType;
					break;
				case ItemType.Food:
					item.info.cookingProcess = ((ServingContainer)src).CookingProcess;
					break;
				case ItemType.Flowers:
					item.info.TypeOfWildFlower = ((Wildflower)src).TypeOfWildFlower;
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
					item.info.Name = ((Book)src).CatalogName;
					break;
				case ItemType.JamJar:
					Type tInfo = src.GetType();
					BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
					FieldInfo ingredientDataField = tInfo.GetField("mData", flags);
					FieldInfo ingredientKeyField = tInfo.GetField("mIngredientKey", flags);
					FieldInfo qualityField = tInfo.GetField("mQuality", flags);
					FieldInfo preservesField = tInfo.GetField("mIsPreserves", flags);

					item.info.IngData = (IngredientData)ingredientDataField.GetValue(src);
					item.info.IngredientKey = (string)ingredientKeyField.GetValue(src);
					item.info.JamQuality = (Quality)qualityField.GetValue(src);
					item.info.JamIsPreserve = (bool)preservesField.GetValue(src);
				break;

				default:
					break;
				}
				return item;
			}
			catch (System.Exception ex)
			{
				CMStoreSet.PrintMessage("CreateRestockItem: " + ex.Message);
				return null;
			}
		}

		public static SortedList<string, Complate> ExtractPatterns(string objectDesignPreset, SortedList<string, bool> enabledStencils)
		{
			//  string objectDesignPreset = ObjectDesigner.GetObjectDesignPreset(objectId);
			if (objectDesignPreset == null || objectDesignPreset.Length == 0)
			{
				return new SortedList<string, Complate>();
			}
			Dictionary<string, Complate> dictionary = new Dictionary<string, Complate>();
			string text = Complate.ProcessPreset(objectDesignPreset, dictionary);
			if (text == null || text.Length == 0)
			{
				return new SortedList<string, Complate>();
			}
			Complate complate;
			if (enabledStencils != null && dictionary.TryGetValue(text, out complate))
			{
				Complate.Variable[] variables = complate.Variables;
				for (int i = 0; i < variables.Length; i++)
				{
					Complate.Variable variable = variables[i];
					if (variable.Type == Complate.Variable.Types.Bool)
					{
						string text2 = variable.Name.ToLower();
						if (text2.StartsWith("stencil ") && text2.EndsWith(" enabled"))
						{
							enabledStencils.Add(variable.Name, bool.Parse(variable.Value));
						}
					}
				}
			}
			dictionary.Remove(text);
			return new SortedList<string, Complate>(dictionary);
		}

		public static bool SettingOwnerPossible(Lot lot)
		{
			List<StoreSetRegister> registers = new List<StoreSetRegister>(Sims3.Gameplay.Queries.GetObjects<StoreSetRegister>(lot));
			if (registers != null && registers.Count > 0)
				return false;

			return true;
		}

		public static bool CreateAndCarryshoppingBag(Sim sim)
		{
			bool flag = false;
			Suitcase suitcase = (GlobalFunctions.CreateObjectOutOfWorld("suitcaseVintage", ProductVersion.EP9) as Suitcase);

			//if (suitcase != null)
			//{
			//    if (sim.ParentToRightHand(suitcase))
			//    {
			//        CarrySystem.EnterWhileHolding(sim, suitcase);
			//    }
			//}

			if (suitcase != null)
			{
				flag = sim.TryAddObjectToInventory(suitcase);
				if (flag)
				{
					flag = CarrySystem.PickUpFromSimInventory(sim, suitcase, true);
				}
			}
			return flag;
		}

		public static void UnSpoil(StoreSetRegister register, StoreSetBase storebase, ani_StoreRug rug, int servingPrice)
		{
			foreach (var stack in register.Inventory.InventoryItems.Values)
			{
				if (register != null)
				{
					foreach (var item in stack.List)
					{
						// if (item.Object.Value == 0)
						{
							ServingContainer single = item.Object as ServingContainer;
							ServingContainerGroup group = item.Object as ServingContainerGroup;

							if (group != null)
							{
								group.mPurchasedPrice = ReturnPriceByQuality(single.FoodQuality, servingPrice * group.NumServingsLeft);
								group.RemoveSpoilageAlarm();
							}
							else if (single != null)
							{
								single.mPurchasedPrice = ReturnPriceByQuality(single.FoodQuality, servingPrice);
								single.RemoveSpoilageAlarm();
							}
						}
					}
				}

				//if (rug != null)
				//{
				//    List<ObjectGuid> objectsICanBuyInDisplay = rug.GetObjectsICanBuyInDisplay(null);
				//    for (int i = 0; i < objectsICanBuyInDisplay.Count; i++)
				//    {
				//        GameObject gameObject = GlobalFunctions.ConvertGuidToObject<GameObject>(objectsICanBuyInDisplay[i]);
				//        if (gameObject != null)
				//        {
				//            CMStoreSet.PrintMessage(gameObject.GetType().ToString());
				//            if (gameObject.GetType() == typeof(ServingContainerGroup) || gameObject.GetType() == typeof(ServingContainerSingle))
				//            {
				//                CMStoreSet.PrintMessage("start unspoiling");
				//                ServingContainer single = gameObject as ServingContainer;
				//                ServingContainerGroup group = gameObject as ServingContainerGroup;

				//                if (group != null)
				//                {
				//                    group.mPurchasedPrice = ReturnPriceByQuality(single.FoodQuality, servingPrice * group.NumServingsLeft);
				//                    group.RemoveSpoilageAlarm();
				//                }
				//                else if (single != null)
				//                {
				//                    single.mPurchasedPrice = ReturnPriceByQuality(single.FoodQuality, servingPrice);
				//                    single.RemoveSpoilageAlarm();
				//                }
				//            }
				//        }
				//    }
				//}


			}
		}

		public static int ReturnPriceByQuality(Quality q, int defaultPrice)
		{
			int price = defaultPrice;
			try
			{
				switch (q)
				{
				case Quality.Foul:
				case Quality.Horrifying:
				case Quality.Bad:
				case Quality.Putrid:
					price = (int)(defaultPrice * 0.5f);
					break;
				case Quality.Nice:
				case Quality.VeryNice:
					price = (int)(defaultPrice * 1.2f);
					break;
				case Quality.Great:
				case Quality.Outstanding:
				case Quality.Excellent:
					price = (int)(defaultPrice * 1.3f);
					break;
				case Quality.Perfect:
					price = (int)(defaultPrice * 1.4f);
					break;
				default:
					price = defaultPrice;
					break;
				}
			}
			catch (System.Exception ex)
			{
				CMStoreSet.PrintMessage("Serving Price: " + ex.Message);
				throw;
			}
			return price;
		}

		public static bool AddPurchaseInteraction(Sim sim, GameObject o, StoreSetBase sBase, bool isAutonomous)
		{
			if (o.GetType() == typeof(FoodProp))
				return false;

			if (o.GetType() == typeof(RestockItem))
				return false;

			if (o.Charred)
				return false;

			//Custom tests
			//Find register and then the owner
			if (sim != null && sBase != null)
			{
				StoreSetRegister register = null;

				if (sBase.Info.RegisterId != ObjectGuid.InvalidObjectGuid)
					register = CMStoreSet.ReturnRegister(sBase.Info.RegisterId, sBase.LotCurrent);

				//If linked to register and nobody tending, can't buy
				//TODO

				//If sim is not in the active household and BuyWhenActive = true     
				//Can buy autonomously only if store owner is the active hosuehold
				if (isAutonomous && StoreSetBase.ReturnBuyWhenActive())
				{
					SimDescription owner = null;

					if (sBase.Info.Owner != 0uL)
						owner = CMStoreSet.ReturnSim(sBase.Info.Owner);
					else if (sBase.Info.RegisterId != ObjectGuid.InvalidObjectGuid)
					{

						if (register != null && register.Info.OwnerId != 0uL)
						{
							owner = CMStoreSet.ReturnSim(register.Info.OwnerId);
						}
					}

					//in-active sims shouldn't buy unless from the store of the active sim
					if ((!sim.Household.IsActive && owner == null) || (!sim.Household.IsActive && owner != null && !owner.Household.IsActive))
					{
						return false;
					}

					//Sims in the active household can buy if they don't own the store
					if (sim.Household.IsActive && owner != null && owner.Household.IsActive)
					{
						return false;
					}
				}
			}

			return true;
		}

		public static void UpdateSkillBasedCareerEarning(SimDescription sd, GameObject soldItem)
		{
			if (sd != null && sd.Occupation != null && sd.Occupation.IsSkillBased)
			{
				if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.Inventor) && soldItem.GetType() == typeof(Invention))
				{
					sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Inventing, soldItem.Value);
				}

				if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.NectarMaker) && soldItem.GetType() == typeof(NectarBottle))
				{
					sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Nectar, soldItem.Value);
				}

				if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.Gardener) && soldItem.GetType() == typeof(Ingredient))
				{
					sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Gardening, soldItem.Value);
				}

				if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.Fisher) && soldItem.GetType() == typeof(Fish))
				{
					sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Fishing, soldItem.Value);
				}

				if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.Photographer) && soldItem.GetType() == typeof(Photograph))
				{
					sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Photography, soldItem.Value);
				}

				if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.Sculptor) && soldItem.GetType() == typeof(Sculpture))
				{
					sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Sculpting, soldItem.Value);
				}
			}

		}

		public static float CalcuateUpdateFrequency(float minutes)
		{
			//progress bar is 10 points
			int maxProgress = 10;
			float f = minutes / maxProgress;


			return f;
		}

	}
}
