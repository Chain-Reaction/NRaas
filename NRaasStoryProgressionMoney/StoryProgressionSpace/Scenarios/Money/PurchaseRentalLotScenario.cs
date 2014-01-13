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
    public class PurchaseRentalLotScenario : PurchaseDeedsBaseScenario
    {
        Lot mLot;

        List<Lot> mLots = null;

        public PurchaseRentalLotScenario()
            : base ()
        { }
        protected PurchaseRentalLotScenario(PurchaseRentalLotScenario scenario)
            : base (scenario)
        {
            mLot = scenario.mLot;
            mLots = scenario.mLots;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "PurchaseRentalLot";
        }

        protected override int MinimumWealth
        {
            get { return GetValue<Option, int>(); }
        }

        protected override bool Allow()
        {
            if (GetValue<Option,int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernated");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int totalFunds = Sim.FamilyFunds - MinimumWealth;

            AddStat("Total Funds", totalFunds);

            List<Lot> lots = new List<Lot>();

            foreach (Lot lot in RentalHelper.GetPurchaseableLots(Manager, Sim.Household))
            {
                if (lot.Household == null)
                {
                    IncStat("Empty");
                    continue;
                }

                if (totalFunds < RentalHelper.GetPurchaseLotCost(lot))
                {
                    IncStat("Too Expensive");
                    continue;
                }

                lots.Add(lot);
            }

            AddStat("Choices", lots.Count);

            if (lots.Count == 0)
            {
                IncStat("No Choices");
                return false;
            }

            mLot = RandomUtil.GetRandomObjectFromList(lots);

            RentalHelper.PurchaseRentalLot(Money, Sim.CreatedSim, mLot);
            return true;
        }

        protected override bool Push()
        {
            if (mLot != null)
            {
                Situations.PushVisit(this, Sim, mLot);
                return true;
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
                parameters = new object[] { Sim, mLot.Name, mLot.Address };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new PurchaseRentalLotScenario(this);
        }

        public class Option : IntegerScenarioOptionItem<ManagerMoney, PurchaseRentalLotScenario>, ManagerMoney.IPurchaseOption
        {
            public Option()
                : base(100000)
            { }

            public override string GetTitlePrefix()
            {
                return "PurchaseRentalLot";
            }

            public override bool Install(ManagerMoney manager, bool initial)
            {
                if (!base.Install(manager, initial)) return false;

                if (initial)
                {
                    ManagerCaste.OnInitializeCastes += OnInitialize;
                }

                return true;
            }

            protected static void OnInitialize()
            {
                bool created;
                CasteOptions options = StoryProgression.Main.Options.GetNewCasteOptions("ActiveRentable", Common.Localize("Caste:ActiveRentable"), out created);
                if (created)
                {
                    options.SetValue<CasteAutoOption, bool>(true);
                    options.AddValue<CasteTypeOption, SimType>(SimType.ActiveFamily);

                    options.SetValue<RentableOption, bool>(false);
                }
            }
        }
    }
}
