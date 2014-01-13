using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public class InspectedMoveInScenario : MoveInScenario
    {
        public InspectedMoveInScenario(List<SimDescription> going, SimDescription moveInWith)
            : base(going, moveInWith)
        { }
        protected InspectedMoveInScenario(InspectedMoveInScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "InspectedMoveIn";
            }
            else
            {
                return "MoveIn";
            }
        }

        protected override ManagerLot.FindLotFlags Inspect
        {
            get { return ManagerLot.FindLotFlags.Inspect | ManagerLot.FindLotFlags.InspectPets; }
        }

        public override Scenario Clone()
        {
            return new InspectedMoveInScenario(this);
        }
    }
}
