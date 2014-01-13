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
using Sims3.Gameplay.EventSystem;
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

namespace NRaas.StoryProgressionSpace.Scenarios
{
    public abstract class HouseChangedBaseScenario : ScheduledSoloScenario, IEventScenario
    {
        Household mOldHousehold;

        HouseholdUpdateEvent mEvent;

        public HouseChangedBaseScenario()
        { }
        protected HouseChangedBaseScenario(HouseChangedBaseScenario scenario)
            : base (scenario)
        {
            mOldHousehold = scenario.mOldHousehold;
            mEvent = scenario.mEvent;
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override ListenHandleType HandleImmediately
        {
            get { return ListenHandleType.Task; }
        }

        protected Household OldHouse
        {
            get { return mOldHousehold; }
        }

        protected Household NewHouse
        {
            get { return mEvent.Household; }
        }

        protected HouseholdUpdateEvent Event
        {
            get { return mEvent; }
        }

        public bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kHouseholdSelected);
        }

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            mEvent = e as HouseholdUpdateEvent;
            if (mEvent == null) return null;

            mOldHousehold = StoryProgression.Main.Households.ActiveHousehold;

            return base.Handle(e, ref result);
        }
    }
}
