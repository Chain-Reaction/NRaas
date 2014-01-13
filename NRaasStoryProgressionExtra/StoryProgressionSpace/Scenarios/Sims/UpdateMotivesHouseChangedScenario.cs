using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class UpdateMotivesHouseChangedScenario : HouseChangedBaseScenario
    {
        public UpdateMotivesHouseChangedScenario()
        { }
        protected UpdateMotivesHouseChangedScenario(UpdateMotivesHouseChangedScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "UpdateMotivesHouseChanged";
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            UpdateMotivesScenario scenario = new UpdateMotivesScenario();
            scenario.Manager = Manager;

            scenario.Perform(HouseholdsEx.All(OldHouse), frame);
            scenario.Perform(HouseholdsEx.All(NewHouse), frame);

            return true;
        }

        public override Scenario Clone()
        {
            return new UpdateMotivesHouseChangedScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSim, UpdateMotivesHouseChangedScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "UpdateMotivesHouseChanged";
            }
        }
    }
}
