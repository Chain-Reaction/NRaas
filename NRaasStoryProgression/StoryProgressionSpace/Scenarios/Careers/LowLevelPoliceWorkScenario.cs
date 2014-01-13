using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Deaths;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class LowLevelPoliceWorkScenario : SimEventScenario<Event>
    {
        public LowLevelPoliceWorkScenario()
        { }
        protected LowLevelPoliceWorkScenario(LowLevelPoliceWorkScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;
            
            return "LowLevelPoliceWork";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kLowLevelPoliceWork);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.CreatedSim.InteractionQueue == null)
            {
                IncStat("No Queue");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool leaveWork = false;

            PrivateEye job = Sim.Occupation as PrivateEye;
            if ((job == null) || (!job.IsAllowedToWork()))
            {
                leaveWork = true;
            }

            if (leaveWork)
            {
                // Time to stop working
                Sim.CreatedSim.InteractionQueue.CancelAllInteractions();
                return true;
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new LowLevelPoliceWorkScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSituation, LowLevelPoliceWorkScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "LowLevelPoliceWork";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP2);
            }
        }
    }
}
