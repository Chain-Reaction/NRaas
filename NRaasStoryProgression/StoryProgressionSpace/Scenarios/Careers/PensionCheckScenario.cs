using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
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
    public class PensionCheckScenario : SimScenario
    {
        public PensionCheckScenario()
            : base ()
        { }
        protected PensionCheckScenario(PensionCheckScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "PensionCheck";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (GetValue<Option, int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CareerManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (sim.CareerManager.RetiredCareer == null)
            {
                IncStat("Not Retired");
                return false;
            }
            else if (!(sim.CareerManager.RetiredCareer is ActiveCareer))
            {
                IncStat("Not ActiveCareer");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Sim.CareerManager.FixRetirementPension();

            ActiveCareer retired = Sim.CareerManager.RetiredCareer as ActiveCareer;

            if (retired == null)
            {
                IncStat("Unneeded");
                return false;
            }

            float optimal = retired.HighestLevel * GetValue<Option,int>();
            if (retired.PensionAmount() < optimal)
            {
                IncStat("Underscore");
                return false;
            }

            float ratio = optimal / retired.PensionAmount();

            retired.mLifetimeEarningsForPensionCalculation = (int)(retired.mLifetimeEarningsForPensionCalculation * ratio);

            AddStat("Ratio", ratio);

            return true;
        }

        public override Scenario Clone()
        {
            return new PensionCheckScenario(this);
        }

        public class Option : IntegerScenarioOptionItem<ManagerCareer, PensionCheckScenario>, ManagerCareer.IRetirementOption
        {
            public Option()
                : base(50)
            { }

            public override string GetTitlePrefix()
            {
                return "PensionCheck";
            }
        }
    }
}
