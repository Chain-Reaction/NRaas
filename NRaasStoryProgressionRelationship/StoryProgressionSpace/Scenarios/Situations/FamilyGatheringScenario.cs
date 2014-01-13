using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class FamilyGatheringScenario : GatheringScenario
    {
        public FamilyGatheringScenario()
        { }
        protected FamilyGatheringScenario(FamilyGatheringScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "FamilyGathering";
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int Rescheduling
        {
            get { return 120; }
        }

        protected override OutfitCategories PartyAttire
        {
            get { return OutfitCategories.Everyday; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (GetValue<Option,int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (!Households.AllowGuardian(sim))
            {
                IncStat("Too Young");
                return false;
            }
            else if (AddScoring("Popularity", sim) <= 0)
            {
                IncStat("Unpopular");
                return false;
            }
            else if (AddScoring("Friendly", sim) <= 0)
            {
                IncStat("Score Fail");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (!Relationships.IsCloselyRelated(Sim, Target, false))
            {
                IncStat("Unrelated");
                return false;
            }
            else if (ManagerSim.GetLTR(Sim, Target) < 0)
            {
                IncStat("Low LTR");
                return false;
            }

            return base.TargetAllow(sim);
        }

        public override Scenario Clone()
        {
            return new FamilyGatheringScenario(this);
        }

        public class Option : ChanceScenarioOptionItem<ManagerSituation, FamilyGatheringScenario>, ManagerSituation.IGatheringOption
        {
            public Option()
                : base(10)
            { }

            public override string GetTitlePrefix()
            {
                return "FamilyGathering";
            }
        }
    }
}
