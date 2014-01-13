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
    public class UnpopularFriendScenario : RelationshipScenario
    {
        public UnpopularFriendScenario(SimDescription sim, SimDescription target, int delta)
            : base (sim, target, delta)
        { }
        protected UnpopularFriendScenario()
            : base(-10)
        { }
        protected UnpopularFriendScenario(UnpopularFriendScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Unpopular";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public override bool IsFriendly
        {
            get { return false; }
        }

        protected override bool TestRelationship
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<ScheduledUnpopularScenario.Option,bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Friends.FindExistingFriendFor(this, sim);
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if ((sim.TraitManager != null) && (sim.TraitManager.HasElement(TraitNames.LongDistanceFriend)))
            {
                IncStat("LongDistance");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (ManagerSim.GetLTR(Sim, Target) <= 0)
            {
                IncStat("Min Liking");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool Push()
        {
            // Complete override of base class
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Friends;
            }

            if (string.IsNullOrEmpty(name)) return null;

            if (Sim.Partner == Target)
            {
                name += "Partner";
            }
            else if ((Sim.ToddlerOrBelow) || (Target.ToddlerOrBelow))
            {
                return null;
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new UnpopularFriendScenario(this);
        }
    }
}
