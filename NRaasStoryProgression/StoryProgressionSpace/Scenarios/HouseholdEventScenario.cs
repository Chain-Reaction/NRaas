using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios
{
    public abstract class HouseholdEventScenario : HouseholdScenario, IEventScenario
    {
        protected HouseholdEventScenario()
        { }
        protected HouseholdEventScenario(Household house)
            : base (house)
        { }
        protected HouseholdEventScenario(HouseholdScenario scenario)
            : base (scenario)
        { }

        public abstract bool SetupListener(IEventHandler events);

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            HouseholdUpdateEvent data = e as HouseholdUpdateEvent;
            if (data != null)
            {
                House = data.Household;
            }
            else
            {
                House = e.TargetObject as Household;
            }

            if (House == null) return null;

            return Clone();
        }
    }
}
