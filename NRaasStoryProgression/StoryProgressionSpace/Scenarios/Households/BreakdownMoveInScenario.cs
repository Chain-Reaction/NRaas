using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public class BreakdownMoveInScenario : DualSimScenario
    {
        bool mIgnoreHead;

        public BreakdownMoveInScenario(SimDescription sim, SimDescription target, bool ignoreHead)
            : base (sim, target)
        {
            mIgnoreHead = ignoreHead;
        }
        protected BreakdownMoveInScenario(bool ignoreHead)
        {
            mIgnoreHead = ignoreHead;
        }
        protected BreakdownMoveInScenario(BreakdownMoveInScenario scenario)
            : base (scenario)
        {
            mIgnoreHead = scenario.mIgnoreHead;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "BreakdownMoveIn";
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
            throw new NotImplementedException();
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            throw new NotImplementedException();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Households.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (Sim.Household == Target.Household)
            {
                IncStat("Together");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            HouseholdBreakdown breakdown = new HouseholdBreakdown(Manager, this, UnlocalizedName, Sim, HouseholdBreakdown.ChildrenMove.Scoring, mIgnoreHead);

            if (!breakdown.SimGoing)
            {
                IncStat("Not Going");
                return false;
            }
            else if ((breakdown.NoneStaying) && (GetValue<IsAncestralOption, bool>(Sim.Household)))
            {
                IncStat("Ancestral");
                return false;
            }

            Add(frame, new InspectedMoveInScenario(breakdown.Going, Target), ScenarioResult.Start);
            return false;
        }

        public override Scenario Clone()
        {
            return new BreakdownMoveInScenario(this);
        }
    }
}
