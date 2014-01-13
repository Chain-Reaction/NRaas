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
    public class GetOffStreetsScenario : MoveOutScenario
    {
        public GetOffStreetsScenario(SimDescription sim)
            : base(sim, null)
        { }
        protected GetOffStreetsScenario(GetOffStreetsScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;
            
            return "GetOffStreets";
        }

        protected override HouseholdBreakdown.ChildrenMove ChildMove
        {
            get { return HouseholdBreakdown.ChildrenMove.Go; }
        }

        protected override MoveInLotScenario GetMoveInLotScenario(List<SimDescription> going)
        {
            return new HomelessMoveInLotScenario(going);
        }

        protected override ScoredMoveInScenario GetMoveInScenario(List<SimDescription> going)
        {
            return new HomelessMoveInScenario(Sim, going);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.LotHome != null)
            {
                IncStat("Resident");
                return false;
            }
            else if (!Households.Allow(this, sim, 0))
            {
                IncStat("User Denied");
                return false;
            }
            else if (Sims.HasEnough(this, sim))
            {
                IncStat("Maximum Reached");
                return false;
            }

            return base.Allow(sim);
        }

        public override Scenario Clone()
        {
            return new GetOffStreetsScenario(this);
        }
    }
}
