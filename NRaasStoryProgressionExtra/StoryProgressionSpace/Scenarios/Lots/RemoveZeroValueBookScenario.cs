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
    public class RemoveZeroValueBookScenario : LotScenario
    {
        public RemoveZeroValueBookScenario(Lot lot)
            : base(lot)
        { }
        protected RemoveZeroValueBookScenario(RemoveZeroValueBookScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ZeroValueBook";
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
            else if (!lot.IsResidentialLot)
            {
                IncStat("Commercial");
                return false;
            }

            return base.Allow(lot);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Dictionary<string, List<Book>> books = new Dictionary<string, List<Book>>();

            foreach (Book book in Lot.GetObjects<Book>())
            {
                if (book.Value == 0)
                {
                    IncStat("Removed");
                    IncStat(book.CatalogName);

                    book.FadeOut(false, true);
                }
            }
            return false;
        }

        public override Scenario Clone()
        {
            return new RemoveZeroValueBookScenario(this);
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
                return "RemoveZeroValueBooks";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public static void OnInstall(Scenario scenario, ScenarioFrame frame)
            {
                LotScenario s = scenario as LotScenario;
                if (s == null) return;

                scenario.Add(frame, new RemoveZeroValueBookScenario(s.Lot), ScenarioResult.Start);
            }
        }
    }
}
