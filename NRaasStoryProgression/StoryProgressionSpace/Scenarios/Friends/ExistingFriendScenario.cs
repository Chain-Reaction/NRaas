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
    public class ExistingFriendScenario : RelationshipScenario
    {
        public ExistingFriendScenario(SimDescription sim, SimDescription target)
            : this(sim, target, true)
        { }
        public ExistingFriendScenario(SimDescription sim, SimDescription target, bool shouldPush)
            : this(sim, target, new SimScoringParameters(sim).Score("Friendly"), shouldPush)
        { }
        public ExistingFriendScenario(SimDescription sim, SimDescription target, int delta, bool shouldPush)
            : base(sim, target, delta)
        {
            mShouldPush = shouldPush;
        }
        protected ExistingFriendScenario(ExistingFriendScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "ExistingFriend";
        }

        protected override bool Allow()
        {
            if (!GetValue<ScheduledFriendScenario.AllowFriendOption, bool>()) return false;

            return base.Allow();
        }

        protected override bool CheckBusy
        {
            get { return ShouldPush; }
        }

        public override bool IsFriendly
        {
            get { return true; }
        }

        protected override bool TestRelationship
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Friends.FindExistingFriendFor(this, sim);
        }

        protected override bool AllowSpecies(SimDescription sim, SimDescription target)
        {
            if (!sim.IsHuman) return true;

            // The FirstAction pushes require that the initiator be the non-human
            return target.IsHuman;
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (Delta < 0)
            {
                IncStat("Enemy");
                return false;
            }
            else if (Relationship.AreStrangers(Sim, Target))
            {
                IncStat("Unknown");
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

            if (string.IsNullOrEmpty(name)) return null;

            if (!Sim.IsHuman)
            {
                name += "Pet";
            }
            else if (Sim.Partner == Target)
            {
                name += "Partner";
            }
            else if ((Sim.ToddlerOrBelow) || (Target.ToddlerOrBelow))
            {
                name  += "Toddler";
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new ExistingFriendScenario(this);
        }
    }
}
