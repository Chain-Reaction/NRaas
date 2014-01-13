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
    public class SchoolUpdateScenario : CareerUpdateScenario
    {
        public SchoolUpdateScenario(SimDescription sim)
            : base (sim)
        { }
        protected SchoolUpdateScenario(SchoolUpdateScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "SchoolUpdate";
        }

        public override Career Job
        {
            get
            {
                if (Sim.CareerManager == null) return null;

                return Sim.CareerManager.School;
            }
        }

        public override CareerUpdateScenario.SchedulingSimData Scheduling
        {
            get { return GetData<SchedulingSimData>(Sim); }
        }

        public static event UpdateDelegate OnSchoolStartTwoHourCommuteScenario;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Job.SetHoursUntilWork();

            Add(frame, new SchoolPushScenario(Sim), ScenarioResult.Start);

            if (OnSchoolStartTwoHourCommuteScenario != null)
            {
                base.PrivateUpdate(frame);

                OnSchoolStartTwoHourCommuteScenario(this, frame);
            }
            return false;
        }

        public override Scenario Clone()
        {
            return new SchoolUpdateScenario(this);
        }

        protected new class SchedulingSimData : CareerUpdateScenario.SchedulingSimData
        {
            public SchedulingSimData()
            { }
        }
    }
}
