using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Deaths;
using NRaas.CommonSpace.Scoring;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios
{
    public abstract class SocialEventScenario : SimEventScenario<SocialEvent>
    {
        protected IMiniSimDescription mTarget;

        public SocialEventScenario()
        { }
        protected SocialEventScenario(SocialEventScenario scenario)
            : base (scenario)
        { }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected SimDescription Target
        {
            get { return mTarget as SimDescription; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSocialInteraction);
        }

        protected override bool Allow(SocialEvent e)
        {
            if (!Allow(e.SocialName)) return false;

            mTarget = e.TargetSimDescription;

            return base.Allow(e);
        }

        protected abstract bool Allow(string socialName);

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                parameters = new object[] { Sim, mTarget };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }
    }
}
