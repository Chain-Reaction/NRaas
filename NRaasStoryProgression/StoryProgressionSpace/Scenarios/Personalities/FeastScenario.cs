using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Situations;
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
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Personalities
{
    public class FeastScenario : AntagonizeScenario
    {
        public FeastScenario()
        { }
        protected FeastScenario(FeastScenario scenario)
            : base (scenario)
        {}

        protected override bool Allow(SimDescription sim)
        {
            if (sim.OccultManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (!sim.OccultManager.HasOccultType(OccultTypes.Vampire))
            {
                IncStat("Not Vampire");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (sim.OccultManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (sim.OccultManager.HasAnyOccultType())
            {
                IncStat("Already Occult");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool result = base.PrivateUpdate(frame);

            if (!mFail)
            {
                DrinkScenario.Drink(Sim.CreatedSim, Target.CreatedSim);
            }

            return result;
        }

        protected override GoToLotSituation.FirstActionDelegate FirstAction
        {
            get { return DrinkScenario.DrinkFirstAction; }
        }

        public override Scenario Clone()
        {
            return new FeastScenario(this);
        }
    }
}
