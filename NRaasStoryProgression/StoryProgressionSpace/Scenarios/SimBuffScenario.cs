using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scoring;
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
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios
{
    public abstract class SimBuffScenario : SimEventScenario<HasGuidEvent<BuffNames>>
    {
        public SimBuffScenario()
        { }
        protected SimBuffScenario(SimDescription sim)
            : base(sim)
        { }
        protected SimBuffScenario(SimBuffScenario scenario)
            : base (scenario)
        { }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kGotBuff);
        }

        protected abstract bool Allow(BuffNames buff);

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            HasGuidEvent<BuffNames> bEvent = e as HasGuidEvent<BuffNames>;
            if (bEvent == null) return null;

            if (!Allow(bEvent.Guid)) return null;

            return base.Handle(e, ref result);
        }
    }
}
