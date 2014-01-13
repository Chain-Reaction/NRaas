using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
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
    public class RepopulateLibraryScenario : LotScenario
    {
        public RepopulateLibraryScenario(Lot lot)
            : base (lot)
        { }
        protected RepopulateLibraryScenario(RepopulateLibraryScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "RepopulateLibrary";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (!Lots.GetValue<Option,bool> ()) return false;

            return base.Allow();
        }

        protected override bool Allow(Lot lot)
        {
            if (lot.IsWorldLot)
            {
                IncStat("World Lot");
                return false;
            }
            else if (lot.IsResidentialLot)
            {
                IncStat("Residential");
                return false;
            }
            else if ((lot.CommercialLotSubType == CommercialLotSubType.kEP2_Junkyard) ||
                (lot.CommercialLotSubType == CommercialLotSubType.kEP2_JunkyardNoVisitors))
            {
                IncStat("Junkyard");
                return false;
            }

            return base.Allow(lot);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool success = false;

            List<Bookshelf> shelves = new List<Bookshelf>(Lot.GetObjects<Bookshelf>());
            foreach (Bookshelf shelf in shelves)
            {
                RepairableComponent repairable = shelf.Repairable;
                if ((repairable != null) && (repairable.Broken)) continue;

                if (Inventories.QuickFind<Book>(shelf.Inventory).Count == 0)
                {
                    shelf.mHasAddedBooks = false;
                    shelf.AddBooksIfNeeded();

                    int newCount = Inventories.QuickFind<Book>(shelf.Inventory).Count;

                    AddStat("Books Added", newCount);

                    if (newCount > 0)
                    {
                        success = true;
                    }
                }
            }

            return success;
        }

        public override Scenario Clone()
        {
            return new RepopulateLibraryScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerLot>
        {
            public Option()
                : base(true)
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
                return "RepopulateEmptyLibraryShelves";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public static void OnInstall(Scenario scenario, ScenarioFrame frame)
            {
                LotScenario s = scenario as LotScenario;
                if (s == null) return;

                scenario.Add(frame, new RepopulateLibraryScenario(s.Lot), ScenarioResult.Start);
            }
        }
    }
}
