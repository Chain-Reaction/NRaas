using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Flirts;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Households;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Romances
{
    public abstract class ScheduledMarriageBaseScenario : PartnerScenario
    {
        public ScheduledMarriageBaseScenario()
        { }
        protected ScheduledMarriageBaseScenario(ScheduledMarriageBaseScenario scenario)
            : base (scenario)
        { }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override int ContinueChance
        {
            get { return 10; }
        }

        protected override int Rescheduling
        {
            get { return 120; }
        }

        protected virtual bool TestScoring
        {
            get { return true; }
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Romances.AllowMarriage(this, sim, Managers.Manager.AllowCheck.Active))
            {
                IncStat("Marriage Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new EngagementScenario(Sim, Target, TestScoring), ScenarioResult.Failure);
            Add(frame, new CooldownMarriageScenario(Sim, Target, TestScoring), ScenarioResult.Failure);
            Add(frame, new StrandedCoupleScenario(Sim, Target, true), ScenarioResult.Start);
            return false;
        }
    }
}
