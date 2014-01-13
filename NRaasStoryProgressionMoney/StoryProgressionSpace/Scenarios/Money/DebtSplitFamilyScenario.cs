using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class DebtSplitFamilyScenario : SplitFamilyScenario
    {
        public DebtSplitFamilyScenario(Household house)
            : base(house)
        { }
        protected DebtSplitFamilyScenario(DebtSplitFamilyScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;
            
            return "DebtSplitFamily";
        }

        protected override bool OnlyChildren
        {
            get { return false; }
        }

        protected override SplitMoveInScenario GetMoveInScenario(SplitFamilyScenario.SplitHome newHome)
        {
            return new DebtSplitMoveInScenario(Sim, newHome);
        }

        protected override SplitMoveOutScenario GetMoveOutScenario(SplitFamilyScenario.SplitHome newHome)
        {
            return new DebtSplitMoveOutScenario(Sim, newHome);
        }

        public override Scenario Clone()
        {
            return new DebtSplitFamilyScenario(this);
        }

        protected class DebtSplitMoveInScenario : SplitMoveInScenario
        {
            public DebtSplitMoveInScenario(SimDescription sim, SplitFamilyScenario.SplitHome newHome)
                : base(sim, newHome)
            { }
            public DebtSplitMoveInScenario(DebtSplitMoveInScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "DebtSplitMoveIn";
                }
                else
                {
                    return "MoveIn";
                }
            }

            protected override ManagerLot.FindLotFlags Inspect
            {
                get { return ManagerLot.FindLotFlags.None; }
            }

            public override Scenario Clone()
            {
                return new DebtSplitMoveInScenario(this);
            }
        }

        protected class DebtSplitMoveOutScenario : SplitMoveOutScenario
        {
            public DebtSplitMoveOutScenario(SimDescription sim, SplitFamilyScenario.SplitHome newHome)
                : base(sim, newHome)
            { }
            public DebtSplitMoveOutScenario(DebtSplitMoveOutScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;
                
                return "DebtSplitMoveOut";
            }

            protected override HouseholdBreakdown.ChildrenMove ChildMove
            {
                get { return HouseholdBreakdown.ChildrenMove.Go; }
            }

            protected override MoveInLotScenario GetMoveInLotScenario(List<SimDescription> going)
            {
                return null;
            }

            protected override ScoredMoveInScenario GetMoveInScenario(List<SimDescription> going)
            {
                return new UninspectedScoredMoveInScenario(Sim, going);
            }

            public override Scenario Clone()
            {
                return new DebtSplitMoveOutScenario(this);
            }
        }
    }
}
