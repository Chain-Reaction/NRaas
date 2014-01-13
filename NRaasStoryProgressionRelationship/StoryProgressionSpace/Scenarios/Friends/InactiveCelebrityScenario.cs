using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
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
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Friends
{
    public class InactiveCelebrityScenario : SimEventScenario<Event>
    {
        public InactiveCelebrityScenario()
        { }
        protected InactiveCelebrityScenario(InactiveCelebrityScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "InactiveCelebrity";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSocialInteraction);
        }

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            Sim simA = e.Actor as Sim;
            Sim simB = e.TargetObject as Sim;

            if ((simA == null) || (simB == null)) return null;

            return base.Handle(e, ref result);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Friends.AllowCelebrity(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Sim simA = Event.Actor as Sim;
            Sim simB = Event.TargetObject as Sim;

            Relationship relation = Relationship.Get(simA, simB, false);
            if (relation == null) return false;

            if ((simA != null) && (simB != null))
            {
                if ((!simA.IsSelectable) && (simB.SimDescription.CelebrityLevel > simA.SimDescription.CelebrityLevel))
                {
                    Friends.AccumulateCelebrity(simA.SimDescription, GetValue<RelationshipScenario.HobnobCelebrityPointsOption,int>() / 5);
                }

                if ((!simB.IsSelectable) && (simA.SimDescription.CelebrityLevel > simB.SimDescription.CelebrityLevel))
                {
                    Friends.AccumulateCelebrity(simB.SimDescription, GetValue<RelationshipScenario.HobnobCelebrityPointsOption, int>() / 5);
                }
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new InactiveCelebrityScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerFriendship, InactiveCelebrityScenario>, ManagerFriendship.ICelebrityOption, IDebuggingOption
        {
            public Option()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "InactiveCelebrity";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP3);
            }
        }
    }
}
