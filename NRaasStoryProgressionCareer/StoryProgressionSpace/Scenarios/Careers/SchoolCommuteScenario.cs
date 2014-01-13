using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Situations;
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
    public class SchoolCommuteScenario : CommuteScenario
    {
        public SchoolCommuteScenario(SimDescription sim, bool push)
            : base (sim, push)
        { }
        protected SchoolCommuteScenario(SchoolCommuteScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "SchoolCommute";
        }

        public override Career Job
        {
            get
            {
                if (Sim.CareerManager == null) return null;

                return Sim.CareerManager.School;
            }
        }

        protected override CommuteScenario.AlarmSimData AlarmData
        {
            get { return GetData<AlarmSimData>(Sim); }
        }

        public override Scenario Clone()
        {
            return new SchoolCommuteScenario(this);
        }

        protected new class AlarmSimData : CommuteScenario.AlarmSimData
        {
            public AlarmSimData()
            { }
        }
    }
}
