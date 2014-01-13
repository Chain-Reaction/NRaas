using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.CommonSpace.Scoring;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class DuplicateLibraryBooksScenario : LotScenario
    {
        public DuplicateLibraryBooksScenario(Lot lot)
            : base(lot)
        { }
        protected DuplicateLibraryBooksScenario(DuplicateLibraryBooksScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "DuplicateLibraryBooks";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool> ()) return false;

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
            else if (lot.IsResidentialLot)
            {
                IncStat("Residential");
                return false;
            }

            return base.Allow(lot);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<BookWritten> allBooks = new List<BookWritten>(Lot.GetObjects<BookWritten>());

            Dictionary<BookWrittenData, int> books = new Dictionary<BookWrittenData, int>();
            foreach (BookWritten book in allBooks)
            {
                BookWrittenData data = book.Data as BookWrittenData;
                if (data == null)
                {
                    IncStat("Bad Removed");

                    book.FadeOut(false, true);
                }
                else
                {
                    if (!books.ContainsKey(data))
                    {
                        books.Add(data, 1);
                    }
                    else
                    {
                        books[data]++;
                    }
                }
            }

            foreach (KeyValuePair<BookWrittenData, int> data in books)
            {
                if (data.Value <= 1) continue;

                List<BookWritten> bookList = new List<BookWritten>();

                foreach (BookWritten book in allBooks)
                {
                    if (book.Data == data.Key)
                    {
                        bookList.Add(book);
                    }
                }

                if (bookList.Count > 1)
                {
                    BookWritten save = RandomUtil.GetRandomObjectFromList(bookList);
                    bookList.Remove(save);

                    foreach (BookWritten book in bookList)
                    {
                        IncStat("Removed");

                        book.FadeOut(false, true);
                    }
                }
            }
            return false;
        }

        public override Scenario Clone()
        {
            return new DuplicateLibraryBooksScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerLot>
        {
            public Option()
                : base(false)
            { }

            public override bool Install(ManagerLot main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    NightlyLotScenario.OnAdditionalScenario += new UpdateDelegate(OnInstall);
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "CleanupDuplicateLibraryBooks";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public static void OnInstall(Scenario scenario, ScenarioFrame frame)
            {
                LotScenario s = scenario as LotScenario;
                if (s == null) return;

                scenario.Add(frame, new DuplicateLibraryBooksScenario(s.Lot), ScenarioResult.Start);
            }
        }
    }
}
