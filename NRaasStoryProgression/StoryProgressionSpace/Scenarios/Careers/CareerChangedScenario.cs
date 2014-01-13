using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scoring;
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
    public class CareerChangedScenario : SimScenario
    {
        public CareerChangedScenario(SimDescription sim)
            : base (sim)
        { }
        protected CareerChangedScenario(CareerChangedScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "CareerChanged";
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

        public static event UpdateDelegate OnManageCareerScenario;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new UpdateCareerScenario(Sim, true), ScenarioResult.Start);

            if (OnManageCareerScenario != null)
            {
                OnManageCareerScenario(this, frame);
            }
            return false;
        }

        public override Scenario Clone()
        {
            return new CareerChangedScenario(this);
        }
    }
}
