using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Romances
{
    public abstract class CaughtCheatingBaseScenario : DualSimScenario
    {
        bool mRelatedStay;

        public CaughtCheatingBaseScenario(SimDescription sim, SimDescription target, bool relatedStay)
            : base(sim, target)
        {
            mRelatedStay = relatedStay;
        }
        protected CaughtCheatingBaseScenario(CaughtCheatingBaseScenario scenario)
            : base(scenario)
        {
            mRelatedStay = scenario.mRelatedStay;
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.Partner == null)
            {
                IncStat("No Partner");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if ((Sim.CreatedSim != null) && (Sim.Partner.CreatedSim != null))
            {
                EventTracker.SendEvent(EventTypeId.kCaughtCheatingOnPartner, Sim.CreatedSim, Sim.Partner.CreatedSim);
                if (Target.CreatedSim != null)
                {
                    EventTracker.SendEvent(EventTypeId.kCaughtCheatingWithSim, Sim.CreatedSim, Target.CreatedSim);
                }

                EventTracker.SendEvent(EventTypeId.kCaughtSimCheatingOnMe, Sim.Partner.CreatedSim, Sim.CreatedSim);
            }

            Add(frame, new ExistingEnemyScenario(Sim.Partner, Target), ScenarioResult.Start);
            Add(frame, new CaughtCheatingBreakupScenario(Sim, true, mRelatedStay), ScenarioResult.Start);
            return false;
        }
    }
}
