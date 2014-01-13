using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.BuildBuy;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class ManagedBuyProduct<T> : BuyProductList
        where T : class, IGameObject
    {
        SimDescription mSim;

        int mMinimumPrice;

        public ManagedBuyProduct(SimDescription sim, Common.IStatGenerator stats, int minimumPrice, BuildBuyProduct.eBuyCategory buyCategory, BuildBuyProduct.eBuySubCategory buySubCategory)
            : base(stats, buyCategory, buySubCategory, minimumPrice, sim.FamilyFunds)
        {
            mSim = sim;

            mMinimumPrice = minimumPrice;
        }

        public static T Purchase(SimDescription sim, int minimumPrice, Common.IStatGenerator stats, string unlocalizedName, TestDelegate<T> test, BuildBuyProduct.eBuyCategory buyCategory, BuildBuyProduct.eBuySubCategory buySubCategory)
        {
            T obj = Inventories.InventoryFind<T>(sim);
            if (obj != null)
            {
                return obj;
            }

            ManagedBuyProduct<T> list = new ManagedBuyProduct<T>(sim, stats, minimumPrice, buyCategory, buySubCategory);

            return list.PrivatePurchase(stats, unlocalizedName, test);
        }

        protected T PrivatePurchase(Common.IStatGenerator stats, string unlocalizedName, TestDelegate<T> test)
        {
            if (Count == 0)
            {
                stats.IncStat("No Purchase Choices " + unlocalizedName);
                return null;
            }

            int price;
            T obj = Get<T>(stats, unlocalizedName, test, out price);
            if (obj != null)
            {
                Inventories.TryToMove(obj, mSim.CreatedSim);

                NRaas.StoryProgression.Main.Money.AdjustFunds(mSim, "BuyItem", -price);
                return obj;
            }
            else
            {
                stats.IncStat("No Purchase " + unlocalizedName);
                return null;
            }
        }
    }
}

