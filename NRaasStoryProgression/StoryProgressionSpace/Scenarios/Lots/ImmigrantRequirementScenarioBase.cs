using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public abstract class ImmigrantRequirementScenarioBase : ScheduledSoloScenario
    {
        protected ManagerLot.ImmigrationRequirement mRequirement;

        public ImmigrantRequirementScenarioBase(ManagerLot.ImmigrationRequirement requirement)
        {
            mRequirement = requirement;
        }
        protected ImmigrantRequirementScenarioBase(ImmigrantRequirementScenarioBase scenario)
            : base (scenario)
        {
            mRequirement = scenario.mRequirement;
        }

        public ManagerLot.ImmigrationRequirement Requirement
        {
            get { return mRequirement; }
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ImmigrantRequirement";
        }

        protected override bool Progressed
        {
            get { return true; }
        }
    }
}
