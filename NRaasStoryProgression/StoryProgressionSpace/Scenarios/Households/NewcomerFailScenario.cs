using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public class NewcomerFailScenario : HouseholdScenario
    {
        public NewcomerFailScenario(Household house)
            : base (house)
        { }
        protected NewcomerFailScenario(NewcomerFailScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "NewcomerFail";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<NewcomerGoneScenario.AllowHomelessMoveInOptionV2, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            if (house.LotHome != null)
            {
                IncStat("Resident");
                return false;
            }
            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new NewcomerFailScenario(this);
        }
    }
}
