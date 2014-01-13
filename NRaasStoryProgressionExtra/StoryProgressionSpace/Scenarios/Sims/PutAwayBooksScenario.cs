using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class PutAwayBooksScenario : InventorySimScenario
    {
        public PutAwayBooksScenario()
            : base ()
        { }
        protected PutAwayBooksScenario(PutAwayBooksScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "PutAwayBooks";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override int ContinueChance
        {
            get { return 100; }
        }

        protected override bool AllowActive
        {
            get
            {
                return GetValue<AllowActiveOption,bool>();
            }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            if (Common.IsOnTrueVacation()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (sim.ToddlerOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (AddScoring("Neat", sim) <= 0)
            {
                IncStat("Score Fail");
                return false;
            }

            return base.Allow(sim);
        }

        protected override List<GameObject> GetInventory(SimDescription sim)
        {
            List<GameObject> list = new List<GameObject>();
            foreach (Book book in Inventories.InventoryFindAll<Book>(sim))
            {
                if ((book is BookSkill) || (book is BookRecipe) || (book is BookFish) || (book is SheetMusic) || (book is BookGeneral) || (book is BookAlchemyRecipe))
                {
                    list.Add(book);
                }
            }
            return list;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool bFound = false;

            foreach (Book book in GetInventory(Sim))
            {
                Lots.PutAwayBook(this, book, Sim.LotHome);
                bFound = true;
            }

            return bFound;
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new PutAwayBooksScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSim, PutAwayBooksScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "PutAwayBooks";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }

        public class AllowActiveOption : BooleanManagerOptionItem<ManagerSim>
        {
            public AllowActiveOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowActivePutAwayBooks";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
