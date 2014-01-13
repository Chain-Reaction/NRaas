using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public class HomelessMoveInScenario : ScoredMoveInScenario
    {
        public HomelessMoveInScenario(SimDescription sim, List<SimDescription> going)
            : base(sim, going)
        { }
        protected HomelessMoveInScenario(HomelessMoveInScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "HomelessMoveIn";
            }
            else
            {
                return "MoveIn";
            }
        }

        protected override ManagerLot.FindLotFlags Inspect
        {
            get
            {
                if (GetValue<ManagerHousehold.HomelessInspectionOption, bool>())
                {
                    return ManagerLot.FindLotFlags.Inspect;
                }
                else
                {
                    return ManagerLot.FindLotFlags.None;
                }
            }
        }

        protected override bool OnlyFamilyMoveIn
        {
            get { return false; }
        }

        public override Scenario Clone()
        {
            return new HomelessMoveInScenario(this);
        }
    }
}
