using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public class HouseholdNameScenario : HouseholdScenario
    {
        public HouseholdNameScenario()
        { }
        protected HouseholdNameScenario(HouseholdNameScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "HouseName";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            if (SimTypes.IsSpecial(house))
            {
                IncStat("Special");
                return false;
            }
            else if (!GetValue<AllowNameChangeOption, bool>(house))
            {
                IncStat("User Denied");
                return false;
            }

            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            SimDescription head = SimTypes.HeadOfFamily(House);
            if (head == null) return false;

            if ((Households.AllowGuardian(head)) && (House.Name != head.LastName))
            {
                House.Name = head.LastName;
                return true;
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new HouseholdNameScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerHousehold, HouseholdNameScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "HandleHomeName";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
