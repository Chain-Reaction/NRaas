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
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Miscellaneous;
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
    public class ImmigrantRequirementScenario : ImmigrantRequirementScenarioBase
    {
        public ImmigrantRequirementScenario(ManagerLot.ImmigrationRequirement requirement)
            : base(requirement)
        { }
        public ImmigrantRequirementScenario(ImmigrantRequirementScenario scenario)
            : base(scenario)
        { }

        protected override bool Allow()
        {
            if (GetValue<PressureOption, int>() <= 0) return false;

            return base.Allow();
        }

        public static event UpdateDelegate OnImmigrantManageBossScenario;
        public static event UpdateDelegate OnImmigrantRequirementPostScenario;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (OnImmigrantManageBossScenario != null)
            {
                OnImmigrantManageBossScenario(this, frame);
            }
            if (OnImmigrantRequirementPostScenario != null)
            {
                OnImmigrantRequirementPostScenario(this, frame);
            }
            return true;
        }

        public override Scenario Clone()
        {
            return new ImmigrantRequirementScenario(this);
        }

        public class PressureOption : Manager.ImmigrationPressureBaseOption<ManagerCareer>
        {
            public PressureOption()
                : base(1)
            { }
        }
    }
}
