using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
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
    public class WorkUpdateScenario : CareerUpdateScenario
    {
        public WorkUpdateScenario(SimDescription sim)
            : base (sim)
        { }
        protected WorkUpdateScenario(WorkUpdateScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "WorkUpdate";
        }

        public override Career Job
        {
            get { return Sim.Occupation as Career; }
        }

        public override CareerUpdateScenario.SchedulingSimData Scheduling
        {
            get { return GetData<SchedulingSimData>(Sim); }
        }

        public static event UpdateDelegate OnWorkStartTwoHourCommuteScenario;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Job.SetHoursUntilWork();

            GetData<CareerSimData>(Sim).UpdateCareer ();

            Add(frame, new WorkPushScenario(Sim), ScenarioResult.Start);

            if (OnWorkStartTwoHourCommuteScenario != null)
            {
                base.PrivateUpdate(frame);

                OnWorkStartTwoHourCommuteScenario(this, frame);
            }
            return false;
        }

        public override Scenario Clone()
        {
            return new WorkUpdateScenario(this);
        }

        protected new class SchedulingSimData : CareerUpdateScenario.SchedulingSimData
        {
            public SchedulingSimData()
            { }
        }
    }
}
