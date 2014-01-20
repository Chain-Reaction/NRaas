using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class Consignments
    {
        public delegate void Logger(string text);

        public static bool IsCollectable(GameObject obj)
        {
            return ((obj is RockGemMetalBase) || (obj is IRelic) || (obj is IHazInsect));
        }

        public static void NotifySell(SimDescription sim, GameObject obj)
        {
            NotifySell(sim, obj, obj.Value);
        }
        public static void NotifySell(SimDescription sim, GameObject obj, int value)
        {
            try
            {
                obj.SoldFor(value);

                if (obj.CanBeSold())
                {
                    if (IsCollectable(obj))
                    {
                        foreach (SimDescription member in sim.Household.AllSimDescriptions)
                        {
                            Collecting skill = member.SkillManager.GetSkill<Collecting>(SkillNames.Collecting);
                            if (skill != null)
                            {
                                skill.UpdateXpForEarningMoney(value);
                            }
                        }
                    }

                    if (obj.HasFlags(GameObject.FlagField.WasHarvested))
                    {
                        Gardening gardening = sim.SkillManager.GetSkill<Gardening>(SkillNames.Gardening);
                        if (gardening != null)
                        {
                            gardening.HarvestableSold(value);
                        }
                        foreach (SimDescription member in sim.Household.AllSimDescriptions)
                        {
                            gardening = member.SkillManager.GetSkill<Gardening>(SkillNames.Gardening);
                            if (gardening != null)
                            {
                                gardening.UpdateXpForEarningMoney(value);
                            }
                        }
                    }

                    if (obj is INectarBottle)
                    {
                        INectarBottle bottle = obj as INectarBottle;

                        NectarSkill skill = sim.SkillManager.GetSkill<NectarSkill>(SkillNames.Nectar);
                        if (skill != null)
                        {
                            skill.SellNectarBottle(value);
                        }

                        if ((bottle.Creator != null) && (bottle.Creator.IsValid))
                        {
                            skill = bottle.Creator.SkillManager.GetSkill<NectarSkill>(SkillNames.Nectar);
                            if (skill != null)
                            {
                                skill.UpdateXpForEarningMoney(value);
                            }
                        }
                    }

                    if (obj is IInvention)
                    {
                        IInvention invention = obj as IInvention;

                        SimDescription inventor = invention.Inventor;

                        if ((inventor != null) && (inventor.IsValid))
                        {
                            InventingSkill skill2 = inventor.SkillManager.GetSkill<InventingSkill>(SkillNames.Inventing);
                            if (skill2 != null)
                            {
                                skill2.UpdateXpForEarningMoney(value);
                            }
                        }
                    }

                    if (obj is IScubaCollectible)
                    {
                        ScubaDivingSkill skill3 = sim.SkillManager.AddElement(SkillNames.ScubaDiving) as ScubaDivingSkill;
                        if (skill3 != null)
                        {
                            skill3.SimoleonsFromSellingCollectibles += value;
                            skill3.UpdateXpForEarningMoney(value);
                        }
                    }

                    if (sim.CreatedSim != null)
                    {
                        obj.SendObjectSoldEvent(sim.CreatedSim);
                    }
                }
            }
            catch (Exception e)
            {
                Common.DebugException(sim.CreatedSim, obj, e);
            }
        }

        public static Dictionary<object, IList> Cleanup(Logger log)
        {
            Dictionary<object, IList> objectsForSale = new Dictionary<object, IList>();

            Cleanup1(log, "PotionShopConsignmentRegister", PotionShopConsignmentRegister.kConsignmentPrices.Length, ref PotionShopConsignmentRegister.sConsignedObjects);
            Cleanup2(log, "PotionShopConsignmentRegister", ref PotionShopConsignmentRegister.sData);

            Cleanup3(log, "PotionShopConsignmentRegister", ref PotionShopConsignmentRegister.mObjectsForSale, ref objectsForSale);

            Cleanup1(log, "BotShopRegister", BotShopRegister.kConsignmentPrices.Length, ref BotShopRegister.sConsignedObjects);
            Cleanup2(log, "BotShopRegister", ref BotShopRegister.sData);

            Cleanup3(log, "BotShopRegister", ref BotShopRegister.mObjectsForSale, ref objectsForSale);

            Cleanup1(log, "ConsignmentRegister", ConsignmentRegister.kConsignmentPrices.Length, ref ConsignmentRegister.sConsignedObjects);
            Cleanup2(log, "ConsignmentRegister", ref ConsignmentRegister.sData);

            foreach (ConsignmentRegister register in Sims3.Gameplay.Queries.GetObjects<ConsignmentRegister>())
            {
                Cleanup3(log, "ConsignmentRegister", ref register.mObjectsForSale, ref objectsForSale);
            }

            return objectsForSale;
        }

        protected static void Cleanup1<TConsigned>(Logger log, string prefix, int maximumAge, ref Dictionary<ulong, List<TConsigned>> list)
            where TConsigned : ICancelSellableUIItem, ISellableUIItem, IShopItem
        {
            if (list == null)
            {
                list = new Dictionary<ulong, List<TConsigned>>();

                if (log != null)
                {
                    log(" " + prefix + ".sConsignedObjects Restarted");
                }
            }

            List<ulong> remove = new List<ulong>();

            foreach (KeyValuePair<ulong, List<TConsigned>> objects in list)
            {
                if ((objects.Value == null) || (objects.Value.Count == 0))
                {
                    remove.Add(objects.Key);
                }
                else
                {
                    SimDescription owner = null;

                    for (int i = objects.Value.Count - 1; i >= 0; i--)
                    {
                        TConsigned obj = objects.Value[i];

                        IGameObject consignedObj = null;
                        int age = 0;

                        if (obj is ConsignmentRegister.ConsignedObject)
                        {
                            ConsignmentRegister.ConsignedObject castObj = obj as ConsignmentRegister.ConsignedObject;

                            consignedObj = castObj.Object;
                            age = castObj.Age;
                        }
                        else if (obj is PotionShopConsignmentRegister.ConsignedObject)
                        {
                            PotionShopConsignmentRegister.ConsignedObject castObj = obj as PotionShopConsignmentRegister.ConsignedObject;

                            consignedObj = castObj.Object;
                            age = castObj.Age;
                        }
                        else if (obj is BotShopRegister.ConsignedObject)
                        {
                            BotShopRegister.ConsignedObject castObj = obj as BotShopRegister.ConsignedObject;

                            consignedObj = castObj.Object;
                            age = castObj.Age;
                        }
                        try
                        {
                            if ((age < 0) || (age >= maximumAge))
                            {
                                if (owner == null)
                                {
                                    owner = SimDescription.Find(objects.Key);
                                }

                                bool success = false;
                                if (owner != null)
                                {
                                    if (owner.CreatedSim != null)
                                    {
                                        success = Inventories.TryToMove(consignedObj, owner.CreatedSim);
                                    }

                                    if ((!success) && (owner.Household != null) && (owner.Household.SharedFamilyInventory != null))
                                    {
                                        success = Inventories.TryToMove(consignedObj, owner.Household.SharedFamilyInventory.Inventory);
                                    }
                                }

                                if (!success)
                                {
                                    consignedObj.Destroy();

                                    if (log != null)
                                    {
                                        log(" Over Age Object deleted");
                                    }
                                }
                                else
                                {
                                    if (log != null)
                                    {
                                        log(" Over Age Object returned");
                                    }
                                }

                                objects.Value.RemoveAt(i);
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(consignedObj, e);
                        }
                    }
                }
            }

            foreach (ulong id in remove)
            {
                list.Remove(id);

                if (log != null)
                {
                    log(" Removed: " + id);
                }
            }
        }

        protected static void Cleanup2<TConsigned>(Logger log, string prefix, ref List<TConsigned> list)
            where TConsigned : IWeightable
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                TConsigned data = list[i];

                try
                {
                    Simulator.ObjectInitParameters parameters;

                    if (data is ConsignmentRegister.ConsignmentRegisterData)
                    {
                        ConsignmentRegister.ConsignmentRegisterData castObj = data as ConsignmentRegister.ConsignmentRegisterData;

                        castObj.CreateObjectInitParameters(out parameters);
                    }
                    else if (data is PotionShopConsignmentRegister.PotionShopConsignmentRegisterData)
                    {
                        PotionShopConsignmentRegister.PotionShopConsignmentRegisterData castObj = data as PotionShopConsignmentRegister.PotionShopConsignmentRegisterData;

                        castObj.CreateObjectInitParameters(out parameters);
                    }
                    else if (data is BotShopRegister.BotShopConsignmentRegisterData)
                    {
                        BotShopRegister.BotShopConsignmentRegisterData castObj = data as BotShopRegister.BotShopConsignmentRegisterData;

                        castObj.CreateObjectInitParameters(out parameters);
                    }
                }
                catch
                {
                    if (log != null)
                    {
                        log(" Bad Consignment choice removed (1)");
                    }

                    list.RemoveAt(i);
                }
            }
        }

        protected static void Cleanup3<TConsigned>(Logger log, string prefix, ref List<TConsigned> list, ref Dictionary<object,IList> validObjects)
        {
            if (list == null) return;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                TConsigned data = list[i];

                try
                {
                    if (data is ConsignmentRegister.ConsignmentRegisterObjectData)
                    {
                        ConsignmentRegister.ConsignmentRegisterObjectData castObj = data as ConsignmentRegister.ConsignmentRegisterObjectData;

                        castObj.GetThumbnailKey();
                    }
                    else if (data is PotionShopConsignmentRegister.PotionShopConsignmentRegisterObjectData)
                    {
                        PotionShopConsignmentRegister.PotionShopConsignmentRegisterObjectData castObj = data as PotionShopConsignmentRegister.PotionShopConsignmentRegisterObjectData;

                        castObj.GetThumbnailKey();
                    }
                    else if (data is BotShopRegister.BotShopConsignmentRegisterObjectData)
                    {
                        BotShopRegister.BotShopConsignmentRegisterObjectData castObj = data as BotShopRegister.BotShopConsignmentRegisterObjectData;

                        castObj.GetThumbnailKey();
                    }

                    validObjects.Add(data, list);
                }
                catch
                {
                    if (log != null)
                    {
                        log(" Bad Consignment choice removed (2)");
                    }

                    try
                    {
                        if (data is ConsignmentRegister.ConsignmentRegisterObjectData)
                        {
                            ConsignmentRegister.ConsignmentRegisterObjectData castObj = data as ConsignmentRegister.ConsignmentRegisterObjectData;

                            castObj.Destroy();
                        }
                        else if (data is PotionShopConsignmentRegister.PotionShopConsignmentRegisterObjectData)
                        {
                            PotionShopConsignmentRegister.PotionShopConsignmentRegisterObjectData castObj = data as PotionShopConsignmentRegister.PotionShopConsignmentRegisterObjectData;

                            castObj.Destroy();
                        }
                        else if (data is BotShopRegister.BotShopConsignmentRegisterObjectData)
                        {
                            BotShopRegister.BotShopConsignmentRegisterObjectData castObj = data as BotShopRegister.BotShopConsignmentRegisterObjectData;

                            castObj.Destroy();
                        }
                    }
                    catch
                    { }

                    list.RemoveAt(i);
                }
            }
        }

        public static void ValidateObjectForSale(Dictionary<object, IList> objectsForSale)
        {
            foreach (KeyValuePair<object, IList> pair in objectsForSale)
            {
                if (!pair.Value.Contains(pair.Key))
                {
                    if (pair.Key is ConsignmentRegister.ConsignmentRegisterObjectData)
                    {
                        ConsignmentRegister.ConsignmentRegisterObjectData castObj = pair.Key as ConsignmentRegister.ConsignmentRegisterObjectData;

                        castObj.Destroy();
                    }
                    else if (pair.Key is PotionShopConsignmentRegister.PotionShopConsignmentRegisterObjectData)
                    {
                        PotionShopConsignmentRegister.PotionShopConsignmentRegisterObjectData castObj = pair.Key as PotionShopConsignmentRegister.PotionShopConsignmentRegisterObjectData;

                        castObj.Destroy();
                    }
                    else if (pair.Key is BotShopRegister.BotShopConsignmentRegisterObjectData)
                    {
                        BotShopRegister.BotShopConsignmentRegisterObjectData castObj = pair.Key as BotShopRegister.BotShopConsignmentRegisterObjectData;

                        castObj.Destroy();
                    }
                }
            }
        }
    }
}

