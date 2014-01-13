using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class AssignSchoolAgeUpScenario : AgeUpBaseScenario
    {
        public AssignSchoolAgeUpScenario()
        { }
        protected AssignSchoolAgeUpScenario(AssignSchoolAgeUpScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "AssignSchoolAgeUp";
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            SetValue<ManualSchoolOption, bool>(Sim, false);

            Add(frame, new ScheduledAssignSchoolScenario(Sim), ScenarioResult.Start);
            return false;
        }

        public override Scenario Clone()
        {
            return new AssignSchoolAgeUpScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerCareer, AssignSchoolAgeUpScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AssignSchoolAgeUp";
            }
        }
    }
}
