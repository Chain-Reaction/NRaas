using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
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
    public abstract class CareerUpdateScenario : CareerScenario
    {
        public CareerUpdateScenario(SimDescription sim)
            : base (sim)
        { }
        protected CareerUpdateScenario(CareerUpdateScenario scenario)
            : base (scenario)
        { }

        public abstract SchedulingSimData Scheduling
        { get; }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        public static event UpdateDelegate OnCarpoolUpdateScenario;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (OnCarpoolUpdateScenario != null)
            {
                OnCarpoolUpdateScenario(this, frame);
            }
            return true;
        }

        public abstract class SchedulingSimData : ElementalSimData, ICommuteSimData, ICarpoolSchedulingSimData
        {
            public bool mScheduled = false;

            public SchedulingSimData()
            { }

            public override string ToString()
            {
                return Common.StringBuilder.XML("Scheduled", mScheduled);
            }

            public void Reset()
            {
                mScheduled = false;
            }
        }
    }
}
