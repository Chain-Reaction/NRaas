using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class PurchaseDeedsScenario : PurchaseDeedsBaseScenario
    {
        RabbitHole mRabbitHole;

        List<RabbitHole> mHoles = null;

        Lot mLot;

        List<Lot> mLots = null;

        public PurchaseDeedsScenario()
            : base ()
        { }
        protected PurchaseDeedsScenario(PurchaseDeedsScenario scenario)
            : base (scenario)
        {
            mRabbitHole = scenario.mRabbitHole;
            mHoles = scenario.mHoles;
            mLot = scenario.mLot;
            mLots = scenario.mLots;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "PurchaseDeeds";
        }

        protected override int MinimumWealth
        {
            get { return GetValue<Option, int>(); }
        }

        protected virtual bool AllowMultiple
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (GetValue<Option,int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if (!AllowMultiple)
            {
                int count = 0;
                foreach (PropertyData data in sim.Household.RealEstateManager.AllProperties)
                {
                    if (data.PropertyType != RealEstatePropertyType.VacationHome)
                    {
                        count++;
                    }
                }

                if (count > 0)
                {
                    int sims = 0;
                    foreach (SimDescription member in HouseholdsEx.All(sim.Household))
                    {
                        if (!Households.AllowGuardian(member)) continue;

                        sims++;
                    }

                    if (count >= sims)
                    {
                        IncStat("Enough");
                        return false;
                    }
                }
            }

            return true;
        }

        protected override Scenario.GatherResult Gather(List<Scenario> list, ref int continueChance, ref int maximum, ref bool random)
        {
            List<Household> houses = null;

            if (mHoles == null)
            {
                houses = Household.GetHouseholdsLivingInWorld();

                List<RabbitHole> holes = new List<RabbitHole>(Sims3.Gameplay.Queries.GetObjects<RabbitHole>());

                mHoles = new List<RabbitHole>();

                foreach (RabbitHole hole in holes)
                {
                    if (!hole.RabbitHoleTuning.kCanInvestHere) continue;

                    bool found = false;
                    foreach (Household house in houses)
                    {
                        if (house.RealEstateManager == null) continue;

                        PropertyData data = house.RealEstateManager.FindProperty(hole);
                        if (data == null) continue;

                        if (data.IsFullOwner)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        mHoles.Add(hole);
                    }
                }
            }

            if (mLots == null)
            {
                if (houses != null)
                {
                    houses = Household.GetHouseholdsLivingInWorld();
                }

                mLots = new List<Lot>();

                foreach (Lot lot in LotManager.AllLots)
                {
                    if (!RealEstateData.IsPurchaseableVenue(lot)) continue;

                    if (RealEstateData.GetVenuePurchaseCost(lot) < 0) continue;

                    bool found = false;
                    foreach (Household house in houses)
                    {
                        if (house.RealEstateManager == null) continue;

                        PropertyData data = house.RealEstateManager.FindProperty(lot);
                        if (data == null) continue;

                        if (data.IsFullOwner)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        mLots.Add(lot);
                    }
                }
            }

            if ((mHoles.Count == 0) && (mLots.Count == 0))
            {
                return GatherResult.Failure;
            }

            return base.Gather(list, ref continueChance, ref maximum, ref random);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int totalFunds = Sim.FamilyFunds - MinimumWealth;

            AddStat("Total Funds", totalFunds);

            List<OccupationNames> careers = new List<OccupationNames>();
            Careers.GetPotentialCareers(this, Sim, careers, false);

            List<RabbitHole> choices = new List<RabbitHole>();

            foreach (RabbitHole hole in mHoles)
            {
                if (totalFunds < hole.RabbitHoleTuning.kInvestCost)
                {
                    IncStat("Hole Too Expensive");
                    continue;
                }

                if (Sim.Household.RealEstateManager.FindProperty(hole) != null)
                {
                    IncStat("Hole Already Owned");
                    continue;
                }

                choices.Add(hole);
            }

            if (choices.Count > 0)
            {
                AddStat("Hole Choices", choices.Count);

                mRabbitHole = RandomUtil.GetRandomObjectFromList(choices);
                return true;
            }

            List<Lot> lots = new List<Lot>();

            foreach (Lot lot in mLots)
            {
                if (totalFunds < RealEstateData.GetVenuePurchaseCost(lot))
                {
                    IncStat("Venue Too Expensive");
                    continue;
                }

                if (Sim.Household.RealEstateManager.FindProperty(lot) != null)
                {
                    IncStat("Venue Already Owned");
                    continue;
                }

                lots.Add(lot);
            }

            if (lots.Count > 0)
            {
                AddStat("Venue Choices", lots.Count);

                mLot = RandomUtil.GetRandomObjectFromList(lots);
                return true;
            }

            IncStat("No Choices");
            return false;
        }

        protected override bool Push()
        {
            if (mRabbitHole != null)
            {
                return Situations.PushInteraction(this, Sim, mRabbitHole, InvestInRabbithole.InvestSingleton);
            }
            else if (mLot != null)
            {
                return Situations.PushInteraction(this, Sim, mLot, PurchaseVenueEx.Singleton);
            }
            else
            {
                return false;
            }
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                if (mRabbitHole != null)
                {
                    parameters = new object[] { Sim, mRabbitHole.CatalogName };
                }
                else if (mLot != null)
                {
                    parameters = new object[] { Sim, mLot.Name };
                }
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new PurchaseDeedsScenario(this);
        }

        public class Option : IntegerScenarioOptionItem<ManagerMoney, PurchaseDeedsScenario>, ManagerMoney.IPurchaseOption
        {
            public Option()
                : base(100000)
            { }

            public override string GetTitlePrefix()
            {
                return "PurchaseDeeds";
            }
        }
    }
}
