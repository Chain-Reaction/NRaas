using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
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
    public class SellDuplicateResidentialBookScenario : LotScenario
    {
        public SellDuplicateResidentialBookScenario(Lot lot)
            : base (lot)
        { }
        protected SellDuplicateResidentialBookScenario(SellDuplicateResidentialBookScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "SellDuplicateBook";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(Lot lot)
        {
            if (lot.IsActive)
            {
                IncStat("Active");
                return false;
            }
            else if (lot.IsWorldLot)
            {
                IncStat("World Lot");
                return false;
            }
            else if (!lot.IsResidentialLot)
            {
                IncStat("Commercial");
                return false;
            }
            else if (lot.Household == null)
            {
                IncStat("Unoccupied");
                return false;
            }

            return base.Allow(lot);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Dictionary<string, List<Book>> books = new Dictionary<string, List<Book>>();

            foreach (Book book in Lot.GetObjects<Book>())
            {
                List<Book> bookSet;
                if (!books.TryGetValue(book.BookId, out bookSet))
                {
                    bookSet = new List<Book>();

                    books.Add(book.BookId, bookSet);
                }

                bookSet.Add(book);
            }

            int funds = 0;

            SimDescription head = SimTypes.HeadOfFamily(Lot.Household);

            foreach (List<Book> bookSet in books.Values)
            {
                bool initial = true;
                foreach (Book book in bookSet)
                {
                    if (initial)
                    {
                        initial = false;
                    }
                    else
                    {
                        funds += Money.Sell(head, book);
                    }
                }
            }

            if (funds > 0)
            {
                AddStat("Revenue", funds);
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new SellDuplicateResidentialBookScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerMoney>, ManagerMoney.ISalesOption
        {
            public Option()
                : base(false)
            { }

            public override bool Install(ManagerMoney main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    NightlyLotScenario.OnAdditionalScenario += OnRun;
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "SellDuplicateResidentialBooks";
            }

            public static void OnRun(Scenario scenario, ScenarioFrame frame)
            {
                LotScenario s = scenario as LotScenario;
                if (s == null) return;

                scenario.Add(frame, new SellDuplicateResidentialBookScenario(s.Lot), ScenarioResult.Start);
            }
        }
    }
}
