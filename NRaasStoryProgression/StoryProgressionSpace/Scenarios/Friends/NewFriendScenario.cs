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
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Friends
{
    public class NewFriendScenario : RelationshipScenario
    {
        public NewFriendScenario(DualSimScenario scenario)
            : this(scenario.Sim, scenario.Target)
        { }
        protected NewFriendScenario(SimDescription sim, SimDescription target)
            : base(sim, target, new SimScoringParameters(sim).Score("Friendly") * 2)
        { }
        protected NewFriendScenario(NewFriendScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "NewFriend";
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public override bool IsFriendly
        {
            get { return true; }
        }

        protected override bool TestRelationship
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<ScheduledFriendScenario.AllowFriendOption, bool>()) return false;

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim, SimDescription target)
        {
            if (!sim.IsHuman) return true;

            // The FirstAction pushes require that the initiator be the non-human
            return target.IsHuman;
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return null;
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (!Relationship.AreStrangers(Sim, Target)) 
            {
                IncStat("Known");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Friends;
            }

            if (!Sim.IsHuman)
            {
                name += "Pet";
            }
            else if (Sim.ToddlerOrBelow)
            {
                name += "Toddler";
            }
            else if (Target.ToddlerOrBelow)
            {
                ExchangeSims();

                name += "Toddler";
            }
            else if ((Sim.TeenOrAbove) && (Target.TeenOrAbove))
            {
                name += "TeenAbove";
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new NewFriendScenario(this);
        }
    }
}
