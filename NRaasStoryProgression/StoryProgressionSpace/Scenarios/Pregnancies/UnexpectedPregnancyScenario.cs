using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.SimDataElement;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class UnexpectedPregnancyScenario : DualSimScenario
    {
        bool mPhaseTwoComplete = false;

        int mChance = -1;

        public UnexpectedPregnancyScenario(SimDescription sim, SimDescription target)
            : this(sim, target, -1)
        { }
        public UnexpectedPregnancyScenario(SimDescription sim, SimDescription target, int chance)
            : base(sim, target)
        {
            mChance = chance;
        }
        protected UnexpectedPregnancyScenario(UnexpectedPregnancyScenario scenario)
            : base (scenario)
        {
            mPhaseTwoComplete = scenario.mPhaseTwoComplete;
            mChance = scenario.mChance;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "UnexpectedPregnancy";
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
            return Flirts.FlirtySims;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Flirts.FindExistingFor(this, sim, false);
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (Sims.HasEnough(this, sim))
            {
                IncStat("Maximum Reached");
                return false;
            }
            else if (sim.IsPregnant)
            {
                IncStat("Couple Pregnant");
                return false;
            }
            else if (!Pregnancies.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            if (GetValue<MaximumNumberOfChildrenOption, int>(sim) > 0)
            {
                if (AddScoring("PreferredBabyCount", sim) <= Relationships.GetChildren(sim).Count)
                {
                    IncStat("Enough");
                    return false;
                }
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if ((Sim.Partner == Target) && (Sim.IsMarried))
            {
                IncStat("Married");
                return false;
            }

            Relationship relationship = ManagerSim.GetRelationship(Sim, Target);
            if (relationship == null) return false;

            LongTermRelationship LTR = relationship.LTR;
            if (LTR == null) return false;

            if (LTR.Liking <= Sims3.Gameplay.Actors.Sim.kRomanceUseLikingGate)
            {
                IncStat("Romance Gated");
                return false;
            }
            else if (LTR.Liking <= Sims3.Gameplay.Actors.Sim.kWooHooUseLikingGate)
            {
                IncStat("WooHoo Gated");
                return false;
            }
            else if (!relationship.AreRomantic())
            {
                IncStat("Not Romantic");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int scoreA = 0;
            int scoreB = 0;

            if (mChance == -1)
            {
                scoreA = AddScoring("UnexpectedPregnancy", GetValue<ChanceOfUnexpectedPregnancyOption, int>(Sim), ScoringLookup.OptionType.Chance, Sim);
                scoreB = AddScoring("UnexpectedPregnancy", GetValue<ChanceOfUnexpectedPregnancyOption, int>(Target), ScoringLookup.OptionType.Chance, Target);
            }
            else
            {
                scoreA = AddScoring("UnexpectedPregnancy", mChance, ScoringLookup.OptionType.Chance, Sim);
                scoreB = AddScoring("UnexpectedPregnancy", mChance, ScoringLookup.OptionType.Chance, Target);
            }

            if ((scoreA <= 0) && (scoreB <= 0))
            {
                Romances.AddWoohooerNotches(Sim, Target, false, false);

                IncStat("Scoring Fail");
                return false;
            }

            if ((Sim.LotHome == null) || (Target.LotHome == null))
            {
                // Required to get mother off streets before pregnancy
                Add(frame, new SettleDownScenario(Sim, Target), ScenarioResult.Start);
            }

            mPhaseTwoComplete = false;

            Romances.AddWoohooerNotches(Sim, Target, true, false);

            Add(frame, new HaveBabyScenario(Sim, Target), ScenarioResult.Start);
            Add(frame, new PhaseTwoScenario(Sim, Target, this), ScenarioResult.Success);

            Add(frame, new HaveBabyScenario(Target, Sim), ScenarioResult.Failure);
            Add(frame, new PhaseTwoScenario(Sim, Target, this), ScenarioResult.Success);

            return false;
        }

        public override Scenario Clone()
        {
            return new UnexpectedPregnancyScenario(this);
        }

        public class PhaseTwoScenario : DualSimScenario
        {
            UnexpectedPregnancyScenario mParent = null;

            public PhaseTwoScenario(SimDescription sim, SimDescription target, UnexpectedPregnancyScenario parent)
                : base(sim, target)
            {
                mParent = parent;
            }
            public PhaseTwoScenario(PhaseTwoScenario scenario)
                : base(scenario)
            {
                mParent = scenario.mParent;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;

                return "UnexpectedPhaseTwo";
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
                return null;
            }

            protected override ICollection<SimDescription> GetTargets(SimDescription sim)
            {
                return null;
            }

            protected override bool Allow()
            {
                if (mParent.mPhaseTwoComplete) return false;

                return base.Allow();
            }

            public static event UpdateDelegate OnRomanceAffairScenario;

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                if (OnRomanceAffairScenario != null)
                {
                    OnRomanceAffairScenario(this, frame);
                }

                Add(frame, new ShotgunMarriageScenario(Sim, Target), ScenarioResult.Failure);
                Add(frame, new SettleDownScenario(Sim, Target), ScenarioResult.Failure);

                // Required to stop the second HaveBabyScenario in calling frame
                Add(frame, new SuccessScenario(), ScenarioResult.Start);

                mParent.mPhaseTwoComplete = true;
                return false;
            }

            public override Scenario Clone()
            {
                return new PhaseTwoScenario(this);
            }
        }
    }
}
