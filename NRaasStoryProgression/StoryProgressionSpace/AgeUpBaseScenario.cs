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
    public abstract class AgeUpBaseScenario : SimEventScenario<Event>
    {
        public AgeUpBaseScenario()
        { }
        public AgeUpBaseScenario(SimDescription sim)
            : base (sim)
        { }
        protected AgeUpBaseScenario(AgeUpBaseScenario scenario)
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

        protected override Scenario.ListenHandleType HandleImmediately
        {
            get { return ListenHandleType.Task; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSimAgeTransition);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            GetData(Sim).InvalidateCache();
            return true;
        }
    }
}
