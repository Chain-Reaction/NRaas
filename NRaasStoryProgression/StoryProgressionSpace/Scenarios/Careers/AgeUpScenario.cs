using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class AgeUpScenario : AgeUpBaseScenario
    {
        public AgeUpScenario()
        { }
        protected AgeUpScenario(AgeUpScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "CareerAgeUp";
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.Household == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (SimTypes.IsSpecial(sim))
            {
                IncStat("Special");
                return false;
            }
            else if (!Careers.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            Add(frame, new RetirementScenario(Sim), ScenarioResult.Start);

            if (Sim.YoungAdultOrBelow)
            {
                Add(frame, new FindJobScenario(Sim, true, false), ScenarioResult.Start);
            }
            else
            {
                IncStat("Too Old For Job");
            }
            return false;
        }

        public override Scenario Clone()
        {
            return new AgeUpScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerCareer, AgeUpScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CareerAgeUp";
            }
        }
    }
}
