using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ConsignerSpace.Helpers
{
    public class ConsignmentHelper : Common.IWorldLoadFinished
    {
        static Common.MethodStore sStoryProgressionMatchesAlertLevel = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "MatchesAlertLevel", new Type[] { typeof(string), typeof(SimDescription) });
        static Common.MethodStore sStoryProgressionAdjustFunds = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "AdjustFunds", new Type[] { typeof(SimDescription), typeof(string), typeof(int) });

        public void OnWorldLoadFinished()
        {
            if (GameUtils.IsInstalled(ProductVersion.EP2))
            {
                if (ConsignmentRegister.sConsignedObjects == null)
                {
                    ConsignmentRegister.sConsignedObjects = new Dictionary<ulong, List<ConsignmentRegister.ConsignedObject>>();
                }

                new Common.AlarmTask(ConsignmentRegister.kTimeOfSale, ~DaysOfTheWeek.None, OnAmbitionConsign);

                AlarmManager.Global.RemoveAlarm(ConsignmentRegister.sDailyAlarm);
                ConsignmentRegister.sDailyAlarm = AlarmHandle.kInvalidHandle;

                GameStates.PreReturnHome -= ConsignmentRegister.OnPreReturnHome;
            }

            if (GameUtils.IsInstalled(ProductVersion.EP7))
            {
                if (PotionShopConsignmentRegister.sConsignedObjects == null)
                {
                    PotionShopConsignmentRegister.sConsignedObjects = new Dictionary<ulong, List<PotionShopConsignmentRegister.ConsignedObject>>();
                }

                new Common.AlarmTask(ConsignmentRegister.kTimeOfSale, ~DaysOfTheWeek.None, OnSupernaturalConsign);

                AlarmManager.Global.RemoveAlarm(PotionShopConsignmentRegister.sDailyAlarm);
                PotionShopConsignmentRegister.sDailyAlarm = AlarmHandle.kInvalidHandle;

                GameStates.PreReturnHome -= PotionShopConsignmentRegister.OnPreReturnHome;
            }

            if (GameUtils.IsInstalled(ProductVersion.EP11))
            {
                if (BotShopRegister.sConsignedObjects == null)
                {
                    BotShopRegister.sConsignedObjects = new Dictionary<ulong, List<BotShopRegister.ConsignedObject>>();
                }

                new Common.AlarmTask(ConsignmentRegister.kTimeOfSale, ~DaysOfTheWeek.None, OnIntoTheFutureConsign);

                AlarmManager.Global.RemoveAlarm(BotShopRegister.sDailyAlarm);
                BotShopRegister.sDailyAlarm = AlarmHandle.kInvalidHandle;

                GameStates.PreReturnHome -= BotShopRegister.OnPreReturnHome;
            }

            GameStates.PreReturnHome -= OnPreReturnHome;
            GameStates.PreReturnHome += OnPreReturnHome;
        }

        protected static void OnAmbitionConsign()
        {
            if (ConsignmentRegister.sSkillVsSaleChanceCurve == null)
            {
                ConsignmentRegister.InitializeData();
            }

            Dictionary<ulong, List<ConsignedObjectProxy>> objects = ConsignedObjectProxy.ConvertToList(ConsignmentRegister.sConsignedObjects);
            Consign(objects, ConsignmentRegister.sConsignAttempts, ConsignmentRegister.kConsignmentPrices, ConsignmentRegister.kMadeGoodSaleThreshold, ConsignmentRegister.kReputationBonusForPositiveFeedback, ConsignmentRegister.sQualityVsReputationCurve, ConsignmentRegister.kSellXItemsOpportunityReputationGainMultiplier, ConsignmentRegister.kConsignmentLifespan, ConsignmentRegister.kDailyNumberOfAttemptsDecrement, ConsignmentRegister.sReputationVsStoreFeeCurve, ConsignmentRegister.kNumberOfTopSellingItems, ConsignmentRegister.kMakeXSimoleonsOpportunityConsignmentFeeMultiplier);

            ConsignmentRegister.sConsignedObjects = ConsignedObjectProxy.ConvertToList<ConsignmentRegister.ConsignedObject>(objects);
        }

        protected static void OnSupernaturalConsign()
        {
            if (PotionShopConsignmentRegister.sSkillVsSaleChanceCurve == null)
            {
                PotionShopConsignmentRegister.InitializeData();
            }

            Dictionary<ulong, List<ConsignedObjectProxy>> objects = ConsignedObjectProxy.ConvertToList(PotionShopConsignmentRegister.sConsignedObjects);
            Consign(objects, PotionShopConsignmentRegister.sConsignAttempts, PotionShopConsignmentRegister.kConsignmentPrices, PotionShopConsignmentRegister.kMadeGoodSaleThreshold, PotionShopConsignmentRegister.kReputationBonusForPositiveFeedback, PotionShopConsignmentRegister.sQualityVsReputationCurve, PotionShopConsignmentRegister.kSellXItemsOpportunityReputationGainMultiplier, PotionShopConsignmentRegister.kConsignmentLifespan, PotionShopConsignmentRegister.kDailyNumberOfAttemptsDecrement, PotionShopConsignmentRegister.sReputationVsStoreFeeCurve, PotionShopConsignmentRegister.kNumberOfTopSellingItems, PotionShopConsignmentRegister.kMakeXSimoleonsOpportunityConsignmentFeeMultiplier);

            PotionShopConsignmentRegister.sConsignedObjects = ConsignedObjectProxy.ConvertToList<PotionShopConsignmentRegister.ConsignedObject>(objects);
        }

        protected static void OnIntoTheFutureConsign()
        {
            if (BotShopRegister.sSkillVsSaleChanceCurve == null)
            {
                BotShopRegister.InitializeData();
            }

            Dictionary<ulong, List<ConsignedObjectProxy>> objects = ConsignedObjectProxy.ConvertToList(BotShopRegister.sConsignedObjects);
            Consign(objects, BotShopRegister.sConsignAttempts, BotShopRegister.kConsignmentPrices, BotShopRegister.kMadeGoodSaleThreshold, BotShopRegister.kReputationBonusForPositiveFeedback, BotShopRegister.sQualityVsReputationCurve, BotShopRegister.kSellXItemsOpportunityReputationGainMultiplier, BotShopRegister.kConsignmentLifespan, BotShopRegister.kDailyNumberOfAttemptsDecrement, BotShopRegister.sReputationVsStoreFeeCurve, BotShopRegister.kNumberOfTopSellingItems, BotShopRegister.kMakeXSimoleonsOpportunityConsignmentFeeMultiplier);

            BotShopRegister.sConsignedObjects = ConsignedObjectProxy.ConvertToList<BotShopRegister.ConsignedObject>(objects);
        }

        protected static void Consign(Dictionary<ulong, List<ConsignedObjectProxy>> lookup, Dictionary<ObjectGuid, float> consignAttempts, float[] consignmentPrices, float madeGoodSaleThreshold, float reputationBonusForPositiveFeedback, Curve qualityVsReputationCurve, float sellXItemsOpportunityReputationGainMultiplier, int consignmentLifespan, float dailyNumberOfAttemptsDecrement, Curve reputationVsStoreFeeCurve, int numberOfTopSellingItems, float makeXSimoleonsOpportunityConsignmentFeeMultiplier)
        {
            if (lookup != null)
            {
                Dictionary<ulong, SimDescription> sims = SimListing.GetSims<SimDescription>(null, false);

                foreach (ulong num in lookup.Keys)
                {
                    SimDescription desc = null;
                    sims.TryGetValue(num, out desc);
                    if ((desc != null) && (desc.CreatedSim != null))// && desc.CreatedSim.IsInActiveHousehold)
                    {
                        Consignment consignment = desc.SkillManager.AddElement(SkillNames.Consignment) as Consignment;

                        bool unsold = false;

                        int totalSale = 0x0;

                        List<Pair<int, IGameObject>> list = new List<Pair<int, IGameObject>>();
                        
                        bool bornSalesmanAdded = false;

                        string displayName = null;

                        List<ConsignedObjectProxy> list2 = lookup[num];

                        int index = 0x0;
                        while (index < list2.Count)
                        {
                            ConsignedObjectProxy local1 = list2[index];
                            local1.Age++;

                            float chance = 0;

                            try
                            {
                                if (list2[index].Object != null)
                                {
                                    chance = list2[index].GetChanceOfSale(desc.CreatedSim);
                                }
                                else
                                {
                                    list2.RemoveAt(index);
                                    continue;
                                }
                            }
                            catch (Exception e)
                            {
                                Common.DebugException("Consignment Section A", e);
                                chance = 1f;
                            }

                            if (RandomUtil.RandomChance01(chance))
                            {
                                if (list2[index].Age >= consignmentPrices.Length)
                                {
                                    list2[index].Age = consignmentPrices.Length - 1;
                                }

                                int valueOfSale = list2[index].GetValueOfSale(desc);
                                totalSale += valueOfSale;

                                float quality = list2[index].GetQuality();

                                try
                                {
                                    if ((((float)valueOfSale) / ((float)list2[index].Object.Value)) >= madeGoodSaleThreshold)
                                    {
                                        desc.CreatedSim.BuffManager.AddElement(BuffNames.MadeGoodSale, Origin.None);
                                    }

                                    if (!bornSalesmanAdded && (((desc.TraitManager.HasElement(TraitNames.BornSalesman) && (quality >= TraitTuning.BornSalesmanQualityMinimumThresholdForPositiveFeedback)) && RandomUtil.InterpolatedChance(TraitTuning.BornSalesmanQualityMinimumThresholdForPositiveFeedback, 1f, TraitTuning.BornSalesmanQualityMinimumChanceOfPositiveFeedback, TraitTuning.BornSalesmanQualityMaximumChanceOfPositiveFeedback, quality)) || ((!desc.TraitManager.HasElement(TraitNames.BornSalesman) && (quality >= ConsignmentRegister.kQualityMinimumThreshold)) && RandomUtil.InterpolatedChance(ConsignmentRegister.kQualityMinimumThreshold, 1f, ConsignmentRegister.kQualityMinimumChanceOfPositiveFeedback, ConsignmentRegister.kQualityMaximumChanceOfPositiveFeedback, quality))))
                                    {
                                        bornSalesmanAdded = true;
                                        displayName = list2[index].DisplayName;
                                        consignment.TrackReputationChange(reputationBonusForPositiveFeedback);
                                    }

                                    float reputationChange = qualityVsReputationCurve.Fx(quality);
                                    if (consignment.OppItemsSoldLifetimeOpportunityCompleted && (reputationChange > 0f))
                                    {
                                        reputationChange *= sellXItemsOpportunityReputationGainMultiplier;
                                    }
                                    
                                    if (desc.TraitManager.HasElement(TraitNames.BornSalesman) && (reputationChange > 0f))
                                    {
                                        reputationChange *= TraitTuning.BornSalesmanReputationOnSaleBonusMultiplier;
                                    }

                                    consignment.TrackConsignmentSale(list2[index].Object, valueOfSale, reputationChange);
                                }
                                catch (Exception e)
                                {
                                    Common.DebugException("Consignment Section B", e);
                                }

                                list.Add(new Pair<int, IGameObject>(valueOfSale, list2[index].Object));

                                EventTracker.SendEvent(EventTypeId.kSoldConsignedObject, desc.CreatedSim);

                                GameObject obj2 = list2[index].Object as GameObject;
                                if (obj2 != null)
                                {
                                    Consignments.NotifySell(desc, obj2, valueOfSale);
                                }
                                list2.RemoveAt(index);
                            }
                            else if (list2[index].Age >= consignmentLifespan)
                            {
                                unsold = true;
                                consignment.TrackConsignmentReturn();

                                if (desc.CreatedSim.Inventory.ValidForThisInventory(list2[index].Object))
                                {
                                    Inventories.TryToMove(list2[index].Object, desc.CreatedSim);
                                }
                                else
                                {
                                    desc.CreatedSim.Household.SharedFamilyInventory.Inventory.TryToAdd(list2[index].Object);
                                }

                                list2[index].Object.EnableInteractions();
                                list2.RemoveAt(index);
                            }
                            else
                            {
                                index++;
                            }
                        }

                        DisplayStory(desc, list, totalSale, unsold, displayName, reputationVsStoreFeeCurve, numberOfTopSellingItems, makeXSimoleonsOpportunityConsignmentFeeMultiplier);
                    }
                }
            }

            foreach (ObjectGuid guid in new List<ObjectGuid>(consignAttempts.Keys))
            {
                float attempts = consignAttempts[guid];

                consignAttempts[guid] = Math.Max((float)0f, (float)(attempts - dailyNumberOfAttemptsDecrement));
            }
        }

        protected static void DisplayStory(SimDescription sim, List<Pair<int, IGameObject>> list, int totalSale, bool unsold, string bornSalesmanName, Curve reputationVsStoreFeeCurve, int numberOfTopSellingItems, float makeXSimoleonsOpportunityConsignmentFeeMultiplier)
        {
            try
            {
                int reportGate = Consigner.Settings.mReportGate;

                Consignment consignment = sim.SkillManager.AddElement(SkillNames.Consignment) as Consignment;

                if (list.Count > 0x0)
                {
                    float num7 = Math.Max((float)1f, (float)(totalSale * reputationVsStoreFeeCurve.Fx(consignment.Reputation)));
                    if (consignment.OppMoneyMadeLifetimeOpportunityCompleted)
                    {
                        num7 *= makeXSimoleonsOpportunityConsignmentFeeMultiplier;
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.Append(ShoppingRegister.LocalizeString("ItemsSoldHeader", new object[] { totalSale, (int)num7 }));
                    builder.Append(Common.NewLine);
                    builder.Append(Common.NewLine);
                    list.Sort(delegate(Pair<int, IGameObject> x, Pair<int, IGameObject> y)
                    {
                        return y.First - x.First;
                    });
                    for (int i = 0; (i < numberOfTopSellingItems) && (i < list.Count); i++)
                    {
                        try
                        {
                            builder.Append(ShoppingRegister.LocalizeString("ItemSoldBullet", new object[] { list[i].Second.GetLocalizedName(), list[i].First }));
                            builder.Append(Common.NewLine);
                        }
                        catch
                        { }
                    }
                    if (sim.TraitManager.HasElement(TraitNames.SuaveSeller))
                    {
                        builder.Append(ShoppingRegister.LocalizeString("SuaveSellerMention", new object[] { sim.CreatedSim }));
                    }
                    if (!string.IsNullOrEmpty(bornSalesmanName))
                    {
                        builder.Append(ShoppingRegister.LocalizeString("PositiveFeedbackMention", new object[] { bornSalesmanName }));
                        builder.Append(Common.NewLine);
                        builder.Append(Common.NewLine);
                    }
                    if (unsold)
                    {
                        builder.Append(ShoppingRegister.LocalizeString("SomeItemsUnsold", new object[0x0]));
                    }

                    if (SimTypes.IsSelectable(sim))
                    {
                        sim.CreatedSim.ShowTNSAndPlayStingIfSelectable(builder.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, sim.CreatedSim.ObjectId, "sting_sales_succes");
                    }
                    else if ((totalSale > reportGate) && ((!sStoryProgressionMatchesAlertLevel.Valid) || (sStoryProgressionMatchesAlertLevel.Invoke<bool>(new object[] { "Money", sim }))))
                    {
                        StyledNotification.Format format = new StyledNotification.Format(builder.ToString(), ObjectGuid.InvalidObjectGuid, sim.CreatedSim.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
                        StyledNotification.Show(format);
                    }

                    sim.ModifyFunds(totalSale - ((int)num7));
                    sStoryProgressionAdjustFunds.Invoke<bool>(new object[] { sim, "Consignment", totalSale - ((int)num7) });

                    foreach (Pair<int, IGameObject> pair in list)
                    {
                        pair.Second.Destroy();
                    }

                    EventTracker.SendEvent(new IncrementalEvent(EventTypeId.kEarnedMoneyFromSelfEployment, sim.CreatedSim, null, (float)(totalSale - ((int)num7))));
                }
                else if (unsold)
                {
                    sim.CreatedSim.ShowTNSAndPlayStingIfSelectable(ShoppingRegister.LocalizeString("AllItemsUnsold", new object[0x0]), StyledNotification.NotificationStyle.kGameMessageNegative, ObjectGuid.InvalidObjectGuid, sim.CreatedSim.ObjectId, "sting_sales_fail");
                }
            }
            catch (Exception e)
            {
                Common.DebugException(sim, e);
            }
        }

        protected static void PreReturnHome(Dictionary<ulong,List<ConsignedObjectProxy>> list)
        {
            Dictionary<ulong, SimDescription> sims = SimListing.GetSims<SimDescription>(null, false);

            foreach (KeyValuePair<ulong, List<ConsignedObjectProxy>> pair in list)
            {
                SimDescription description = null;
                sims.TryGetValue(pair.Key, out description);
                if (description != null)
                {
                    Sim createdSim = description.CreatedSim;

                    Inventory familyInventory = null;
                    if ((description.Household != null) && (description.Household.SharedFamilyInventory != null))
                    {
                        familyInventory = description.Household.SharedFamilyInventory.Inventory;
                    }

                    foreach (ConsignedObjectProxy obj in pair.Value)
                    {
                        if (obj.Object == null) continue;

                        if ((createdSim == null) || !Inventories.TryToMove(obj.Object, createdSim))
                        {
                            if (familyInventory != null)
                            {
                                familyInventory.TryToAdd(obj.Object);
                            }
                            else
                            {
                                obj.Object.Destroy();
                            }
                        }
                    }
                }
                else
                {
                    foreach (ConsignedObjectProxy obj3 in pair.Value)
                    {
                        if (obj3.Object == null) continue;

                        obj3.Object.Destroy();
                    }
                }
            }
        }

        private static void OnPreReturnHome()
        {
            try
            {
                if (GameUtils.IsInstalled(ProductVersion.EP2))
                {
                    PreReturnHome(ConsignedObjectProxy.ConvertToList(ConsignmentRegister.sConsignedObjects));

                    ConsignmentRegister.sConsignedObjects = new Dictionary<ulong, List<ConsignmentRegister.ConsignedObject>>();
                }

                if (GameUtils.IsInstalled(ProductVersion.EP7))
                {
                    PreReturnHome(ConsignedObjectProxy.ConvertToList(PotionShopConsignmentRegister.sConsignedObjects));

                    PotionShopConsignmentRegister.sConsignedObjects = new Dictionary<ulong, List<PotionShopConsignmentRegister.ConsignedObject>>();
                }

                if (GameUtils.IsInstalled(ProductVersion.EP11))
                {
                    PreReturnHome(ConsignedObjectProxy.ConvertToList(BotShopRegister.sConsignedObjects));

                    BotShopRegister.sConsignedObjects = new Dictionary<ulong, List<BotShopRegister.ConsignedObject>>();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnPreReturnHome", e);
            }
        }

        public abstract class ConsignedObjectProxy
        {
            public abstract int Age
            {
                get;
                set;
            }

            public abstract ICancelSellableUIItem Data
            {
                get;
            }

            public abstract IGameObject Object
            {
                get;
            }

            protected abstract float PrivateGetQuality();

            public float GetQuality()
            {
                try
                {
                    return PrivateGetQuality();
                }
                catch 
                {
                    return QualityHelper.GetQualityValue(Quality.Neutral);
                }
            }

            public abstract float GetChanceOfSale(Sim createdSim);

            public abstract int GetValueOfSale(SimDescription sim);

            public abstract string DisplayName
            {
                get;
            }

            public static ConsignedObjectProxy Convert(object obj)
            {
                if (obj is ConsignmentRegister.ConsignedObject)
                {
                    return new AmbitonConsignedObjectProxy(obj);
                }

                else if (obj is PotionShopConsignmentRegister.ConsignedObject)
                {
                    return new SupernaturalConsignedObjectProxy(obj);
                }

                else
                {
                    return new IntoTheFutureConsignedObjectProxy(obj);
                }
            }

            public static Dictionary<ulong, List<T>> ConvertToList<T>(Dictionary<ulong, List<ConsignedObjectProxy>> lookup)
                where T : class
            {
                Dictionary<ulong, List<T>> results = new Dictionary<ulong, List<T>>();

                if (lookup != null)
                {
                    foreach (KeyValuePair<ulong, List<ConsignedObjectProxy>> pair in lookup)
                    {
                        List<T> list = new List<T>();

                        foreach (ConsignedObjectProxy obj in pair.Value)
                        {
                            list.Add(obj.Data as T);
                        }

                        results.Add(pair.Key, list);
                    }
                }

                return results;
            }
            public static Dictionary<ulong, List<ConsignedObjectProxy>> ConvertToList<T>(Dictionary<ulong, List<T>> lookup)
                where T : class
            {
                Dictionary<ulong, List<ConsignedObjectProxy>> results = new Dictionary<ulong, List<ConsignedObjectProxy>>();

                if (lookup != null)
                {
                    foreach (KeyValuePair<ulong, List<T>> pair in lookup)
                    {
                        List<ConsignedObjectProxy> list = new List<ConsignedObjectProxy>();

                        foreach (T obj in pair.Value)
                        {
                            list.Add(Convert(obj));
                        }

                        results.Add(pair.Key, list);
                    }
                }

                return results;
            }
        }

        public class AmbitonConsignedObjectProxy : ConsignedObjectProxy
        {
            ConsignmentRegister.ConsignedObject mData;

            public AmbitonConsignedObjectProxy(object data)
            {
                mData = data as ConsignmentRegister.ConsignedObject;
            }

            public override int Age
            {
                get
                {
                    return mData.Age;
                }
                set
                {
                    mData.Age = value;
                }
            }

            public override string DisplayName
            {
                get { return mData.DisplayName; }
            }

            public override IGameObject Object
            {
                get { return mData.Object; }
            }

            public override ICancelSellableUIItem Data
            {
                get { return mData; }
            }

            public override float GetChanceOfSale(Sim createdSim)
            {
                return ConsignmentRegister.GetChanceOfSale(createdSim, mData);
            }

            public override int GetValueOfSale(SimDescription sim)
            {
                return ConsignmentRegister.GetValueOfSale(mData, sim);
            }

            protected override float PrivateGetQuality()
            {
                return mData.GetQuality();
            }
        }

        public class SupernaturalConsignedObjectProxy : ConsignedObjectProxy
        {
            PotionShopConsignmentRegister.ConsignedObject mData;

            public SupernaturalConsignedObjectProxy(object data)
            {
                mData = data as PotionShopConsignmentRegister.ConsignedObject;
            }

            public override string DisplayName
            {
                get { return mData.DisplayName; }
            }

            public override int Age
            {
                get
                {
                    return mData.Age;
                }
                set
                {
                    mData.Age = value;
                }
            }

            public override IGameObject Object
            {
                get { return mData.Object; }
            }

            public override ICancelSellableUIItem Data
            {
                get { return mData; }
            }

            protected override float PrivateGetQuality()
            {
                return mData.GetQuality();
            }

            public override float GetChanceOfSale(Sim createdSim)
            {
                return PotionShopConsignmentRegister.GetChanceOfSale(createdSim, mData);
            }

            public override int GetValueOfSale(SimDescription sim)
            {
                return PotionShopConsignmentRegister.GetValueOfSale(mData, sim);
            }
        }

        public class IntoTheFutureConsignedObjectProxy : ConsignedObjectProxy
        {
            BotShopRegister.ConsignedObject mData;

            public IntoTheFutureConsignedObjectProxy(object data)
            {
                mData = data as BotShopRegister.ConsignedObject;
            }

            public override string DisplayName
            {
                get
                {
                    return mData.DisplayName;
                }
            }

            public override int Age
            {
                get
                {
                    return mData.Age;
                }
                set
                {
                    mData.Age = value;
                }
            }

            public override IGameObject Object
            {
                get
                {
                    return mData.Object;
                }
            }

            public override ICancelSellableUIItem Data
            {
                get
                {
                    return mData;
                }
            }

            protected override float PrivateGetQuality()
            {
                return mData.GetQuality();
            }

            public override float GetChanceOfSale(Sim createdSim)
            {
                return BotShopRegister.GetChanceOfSale(createdSim, mData);
            }

            public override int GetValueOfSale(SimDescription sim)
            {
                return BotShopRegister.GetValueOfSale(mData, sim);
            }
        }
    }
}
