using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class PushReadJournalScenario : SimScenario
    {
        public PushReadJournalScenario()
            : base ()
        { }
        protected PushReadJournalScenario(PushReadJournalScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ReadJournal";
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Careers.GetCareerSims(OccupationNames.Medical);
        }

        protected static bool OnTest(IGameObject obj, object customData)
        {
            Book book = obj as Book;
            if (book == null) return false;

            return (book.Data.MyType == BookData.BookType.MedicalJournal);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.CareerManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else
            {
                bool found = false;

                foreach(Book journal in Inventories.InventoryFindAll<Book>(sim, true, OnTest))
                {
                    if (Book.HasBeenRead(sim, journal.Data.ID)) continue;

                    found = true;
                    break;
                }

                if (!found)
                {
                    IncStat("No Unread Journal");
                    return false;
                }
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return true;
        }

        protected override bool Push()
        {
            Book choice = null;

            foreach (Book journal in Inventories.InventoryFindAll<Book>(Sim, true, OnTest))
            {
                if (Book.HasBeenRead(Sim, journal.Data.ID)) continue;

                choice = journal;
                break;
            }

            return Situations.PushInteraction<Book>(this, Sim, choice, ReadBook.FromSimInventorySingleton);
        }

        public override Scenario Clone()
        {
            return new PushReadJournalScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerCareer, PushReadJournalScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "PushJournalRead";
            }
        }
    }
}
