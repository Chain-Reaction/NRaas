using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scoring;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Caste
{
    public class OutCasteScenario : SimScenario
    {
        public OutCasteScenario()
        { }
        protected OutCasteScenario(OutCasteScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "OutCaste";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Households.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (sim.Household == null)
            {
                return false;
            }
            else if (SimTypes.IsSpecial(sim))
            {
                IncStat("Special");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (SimTypes.HeadOfFamily(sim.Household) == sim)
            {
                IncStat("Head");
                return false;
            }
            else if (!Households.AllowSoloMove(sim))
            {
                IncStat("Too Young");
                return false;
            }
            else if (Lots.AllowCastes(this, sim.LotHome, sim))
            {
                IncStat("Unnecessary");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new OutCasteMoveOutScenario(Sim), ScenarioResult.Start);
            return false;
        }

        public override Scenario Clone()
        {
            return new OutCasteScenario(this);
        }

        protected class OutCasteMoveOutScenario : MoveOutScenario
        {
            public OutCasteMoveOutScenario(SimDescription go)
                : base(go, null)
            { }
            protected OutCasteMoveOutScenario(OutCasteMoveOutScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;

                return "OutCasteMoveOut";
            }

            protected override HouseholdBreakdown.ChildrenMove ChildMove
            {
                get { return HouseholdBreakdown.ChildrenMove.Scoring; }
            }

            protected override MoveInLotScenario GetMoveInLotScenario(List<SimDescription> going)
            {
                return new StandardMoveInLotScenario(going, 0);
            }

            protected override ScoredMoveInScenario GetMoveInScenario(List<SimDescription> going)
            {
                return new InspectedScoredMoveInScenario(Sim, going);
            }

            protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
            {
                manager = Lots;

                return base.PrintStory(manager, name, parameters, extended, logging);
            }

            public override Scenario Clone()
            {
                return new OutCasteMoveOutScenario(this);
            }
        }

        public class Option : BooleanScenarioOptionItem<ManagerCaste, OutCasteScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "OutCaste";
            }
        }
    }
}
