using NRaas.CommonSpace.Helpers;
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
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ConsignerSpace.Interactions
{
    public class BuyItemsWithRegisterEx : ShoppingRegister.BuyItemsWithRegister, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<ShoppingRegister, ShoppingRegister.BuyItemsWithRegister.BuyItemsDefinition, Definition>(false);

            sOldSingleton = BuyItemsSingleton;
            BuyItemsSingleton = new Definition();
        }

        public override void PostAnimation()
        {
            try
            {
                ShowShoppingDialog(mRegister, Actor);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
        }

        protected static bool ShowShoppingDialog(ShoppingRegister ths, Sim sim)
        {
            float num = (1f - ths.GetSalePercentage(sim)) * 100f;

            Dictionary<object, IList> objectsForSale = Consignments.Cleanup(null);          

            Dictionary<string, List<StoreItem>> itemDictionary = ths.ItemDictionary(sim);

            // ItemDictionary will remove objects from the Consignment list, but not delete them, correct for that now
            Consignments.ValidateObjectForSale(objectsForSale);

            if (itemDictionary.Count == 0x0)
            {
                return false;
            }

            ShoppingRabbitHole.CreateSellableCallback createSellableCallback = ths.CreateSellableObjectsList;

            if (ths is ConsignmentRegister)
            {
                createSellableCallback = CreateConsignmentObjectsList;
            }
            else if (ths is PotionShopConsignmentRegister)
            {
                createSellableCallback = CreatePotionObjectsList;
            }
            else if (ths is BotShopRegister)
            {
                createSellableCallback = CreateFutureObjectsList;
            }
            else if (ths is PetstoreRegister)
            {
                createSellableCallback = CreatePetObjectsList;
            }

            ShoppingModel.CurrentStore = ths;
            ShoppingRabbitHole.StartShopping(sim, itemDictionary, ths.PercentModifier, (float)((int)num), 0x0, null, sim.Inventory, null, ths.FinishedCallBack, createSellableCallback, ths.GetRegisterType != RegisterType.General);
            return true;
        }

        private static List<ISellableUIItem> CreatePetObjectsList(Sim customer)
        {
            List<ISellableUIItem> consignableObjectsList = new List<ISellableUIItem>();
            foreach (GameObject obj in Inventories.QuickFind<GameObject>(customer.Inventory, true, IsPetConsignable))
            {
                ConsignmentRegister.AddObjectToSellableObjectsList(obj, consignableObjectsList);
            }

            foreach (GameObject obj in Inventories.QuickFind<GameObject>(customer.Household.SharedFamilyInventory.Inventory, true, IsPetConsignable))
            {
                ConsignmentRegister.AddObjectToSellableObjectsList(obj, consignableObjectsList);
            }

            return consignableObjectsList;
        }

        private static List<ISellableUIItem> CreateConsignmentObjectsList(Sim customer)
        {
            List<ISellableUIItem> consignableObjectsList = new List<ISellableUIItem>();
            foreach (GameObject obj in Inventories.QuickFind<GameObject>(customer.Inventory, true, IsObjectConsignable))
            {
                ConsignmentRegister.AddObjectToSellableObjectsList(obj, consignableObjectsList);
            }

            foreach (GameObject obj in Inventories.QuickFind<GameObject>(customer.Household.SharedFamilyInventory.Inventory, true, IsObjectConsignable))
            {
                ConsignmentRegister.AddObjectToSellableObjectsList(obj, consignableObjectsList);
            }

            return consignableObjectsList;
        }

        private static List<ISellableUIItem> CreatePotionObjectsList(Sim customer)
        {
            List<ISellableUIItem> consignableObjectsList = new List<ISellableUIItem>();
            foreach (GameObject obj in Inventories.QuickFind<GameObject>(customer.Inventory, true, IsPotionConsignable))
            {
                PotionShopConsignmentRegister.AddObjectToSellableObjectsList(obj, consignableObjectsList);
            }

            foreach (GameObject obj in Inventories.QuickFind<GameObject>(customer.Household.SharedFamilyInventory.Inventory, true, IsPotionConsignable))
            {
                PotionShopConsignmentRegister.AddObjectToSellableObjectsList(obj, consignableObjectsList);
            }
            return consignableObjectsList;
        }

        private static List<ISellableUIItem> CreateFutureObjectsList(Sim customer)
        {
            List<ISellableUIItem> consignableObjectsList = new List<ISellableUIItem>();
            foreach (GameObject obj in Inventories.QuickFind<GameObject>(customer.Inventory, true, IsFutureObjectConsignable))
            {
                BotShopRegister.AddObjectToSellableObjectsList(obj, consignableObjectsList);
            }

            foreach (GameObject obj in Inventories.QuickFind<GameObject>(customer.Household.SharedFamilyInventory.Inventory, true, IsFutureObjectConsignable))
            {
                BotShopRegister.AddObjectToSellableObjectsList(obj, consignableObjectsList);
            }
            return consignableObjectsList;
        }

        private static bool IsConsignable(IGameObject obj, object customData)
        {
            if (!obj.CanBeSoldBase()) return false;

            if (obj.Value <= 0) return false;

            if (obj is IHiddenInInventory) return false;

            return true;
        }

        private static bool IsPetConsignable(IGameObject obj, object customData)
        {
            if (!IsConsignable(obj, customData)) return false;

            if (Consigner.Settings.mAllowSellAll) return true;

            if (obj is MinorPet) return true;

            if (obj is IHazInsect) return true;

            if (obj is Insect) return true;

            return false;
        }

        private static bool IsPotionConsignable(IGameObject obj, object customData)
        {
            if (!IsConsignable(obj, customData)) return false;

            if (Consigner.Settings.mAllowSellAll) return true;

            if (obj is ICraft) return true;

            if (obj.SculptureComponent != null) return true;

            if (obj is IGem) return true;

            if (obj is IMetal) return true;

            if (obj is AlchemyPotion) return true;

            // Non-standard

            if (obj is Ingredient) return true;

            if (obj is IHazInsect) return true;

            if (obj is Insect) return true;

            if (obj is Potion) return true;

            return false;
        }

        private static bool IsObjectConsignable(IGameObject obj, object customData)
        {
            if (!IsConsignable(obj, customData)) return false;

            if (Consigner.Settings.mAllowSellAll) return true;

            if (obj is ICraft) return true;

            if (obj.SculptureComponent != null) return true;

            // Non-standard

            if (obj is RockGemMetalBase) return true;
            
            if (obj is IRelic) return true;

            if (obj is ICraftedArt) return true;

            if (obj is ISeaShell) return true;

            // Alchemy

            if (obj is AlchemyPotion) return true;

            if (obj is Potion) return true;

            // Pets

            if (obj is MinorPet) return true;

            if (obj is Insect) return true;

            if (obj is IHazInsect) return true;

            if (obj is Wildflower) return true;

            // Future

            if (obj is ChipBlankLarge || obj is ChipBlankMedium || obj is ChipBlankSmall) return true;

            if (obj is TraitChip) return true;

            if (obj is INanite) return true;

            if (obj is ITerrarium) return (obj as ITerrarium).IsNaniteTerrarium;

            return false;
        }

        private static bool IsFutureObjectConsignable(IGameObject obj, object customData)
        {
            if (!IsConsignable(obj, customData)) return false;

            if (Consigner.Settings.mAllowSellAll) return true;

            if (obj is ICraft) return true;

            if (obj.SculptureComponent != null) return true;

            if (obj is IGem) return true;

            if (obj is IMetal) return true;

            if (obj is ChipBlankLarge || obj is ChipBlankMedium || obj is ChipBlankSmall) return true;

            if (obj is TraitChip) return true;

            if (obj is INanite) return true;

            if (obj is ITerrarium) return (obj as ITerrarium).IsNaniteTerrarium;

            return false;
        }

        public class Definition : ShoppingRegister.BuyItemsWithRegister.BuyItemsDefinition
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new BuyItemsWithRegisterEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}