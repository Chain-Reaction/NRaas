using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.BuildBuy;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public abstract class PurchaseObjectScenario<T> : SimScenario
        where T : class, IGameObject
    {
        T mObject = default(T);

        public PurchaseObjectScenario()
        { }
        protected PurchaseObjectScenario(PurchaseObjectScenario<T> scenario)
            : base (scenario)
        {
            mObject = scenario.mObject;
        }

        protected T Object
        {
            get
            {
                return mObject;
            }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected abstract int Minimum
        {
            get;
        }

        protected abstract int Maximum
        {
            get;
        }

        protected virtual int Funds
        {
            get
            {
                if (Sim == null) return 0;

                if (Sim.Household == null) return 0;

                return Sim.FamilyFunds;
            }
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        protected abstract BuildBuyProduct.eBuyCategory Category
        {
            get;
        }

        protected abstract BuildBuyProduct.eBuySubCategory SubCategory
        {
            get;
        }

        protected abstract bool Test(T obj);

        protected override bool Allow()
        {
            if (Maximum <= 0)
            {
                IncStat("No Maximum");
                return false;
            }
            else if (Common.IsOnTrueVacation())
            {
                IncStat("Vacation");
                return false;
            }

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Not Resident");
                return false;
            }
            else if (sim.ToddlerOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (Deaths.IsDying(sim))
            {
                IncStat("Dying");
                return false;
            }
            else if (!Sims.AllowInventory(this, sim, AllowActive ? Managers.Manager.AllowCheck.None : Managers.Manager.AllowCheck.Active))
            {
                IncStat("Inventory Denied");
                return false;
            }
            else if (!Money.Allow(this, sim))
            {
                IncStat("Money Denied");
                return false;
            }
            else if (SimTypes.IsSpecial(sim))
            {
                IncStat("Special");
                return false;
            }
            else if ((Funds - Minimum) < 0)
            {
                AddStat("No Money Funds", Funds);
                AddStat("No Money Minimum", Minimum);
                return false;
            }
            else if (GetValue<DebtOption, int>(sim.Household) > 0)
            {
                IncStat("Debt");
                return false;
            }

            return (base.Allow(sim));
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int availableFunds = Funds;

            int minimum = Minimum;
            if (minimum > availableFunds)
            {
                AddStat("High Min", availableFunds);
                return false;
            }

            AddStat("Minimum", minimum);

            int maximum = Maximum;
            if (maximum > availableFunds)
            {
                maximum = availableFunds;
            }

            AddStat("Maximum", maximum);

            if (minimum > maximum)
            {
                IncStat("Min Bigger");
                return false;
            }

            BuyProductList list = null;
            try
            {
                list = new BuyProductList(this, Category, SubCategory, minimum, maximum);
            }
            catch(Exception e)
            {
                Common.DebugException(ToString(), e);
            }

            if ((list == null) || (list.Count == 0))
            {
                IncStat("No Choices");
                return false;
            }

            int price = 0;

            mObject = list.Get<T>(this, UnlocalizedName, Test, out price);
            if (mObject == null)
            {
                IncStat("No Object");
                return false;
            }

            if (!Inventories.TryToMove(mObject, Sim.CreatedSim))
            {
                IncStat("Inventory Fail");

                mObject.Destroy();

                mObject = null;

                return false;
            }

            Money.AdjustFunds(Sim, "BuyItem", -price);

            AddStat("Purchased", price);
            return true;
        }

        protected override bool Push()
        {
            return Situations.PushToRabbitHole(this, Sim, RabbitHoleType.Grocery, true, false);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            GameObject obj = mObject as GameObject;
            if (obj == null) return null;

            string objectName = obj.CatalogName;

            if (!string.IsNullOrEmpty(objectName))
            {
                objectName = objectName.Trim();
            }

            if (parameters == null)
            {
                parameters = new object[] { Sim, objectName };
            }

            if (extended == null)
            {
                extended = new string[] { objectName };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }
    }
}
