using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
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
    public class StalkerScenario : RelationshipScenario
    {
        public StalkerScenario()
            : base(-25)
        { }
        protected StalkerScenario(StalkerScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Stalker";
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
            get { return false; }
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Friends.FindExistingFriendFor(this, sim, -50, false);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            if (!GetValue<ScheduledEnemyScenario.AllowEnemyOption, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (AddScoring("Stalking", sim) <= 0)
            {
                IncStat("Score Fail");
                return false;
            }
            else
            {
                List<SimDescription> friends = Flirts.FindExistingFor(this, sim, true);
                if ((friends != null) && (friends.Count > 0))
                {
                    IncStat("Other Loves");
                    return false;
                }
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (Sim.Household == sim.Household)
            {
                IncStat("Same House");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Situations Denied");
                return false;
            }

            Relationship relation = ManagerSim.GetRelationship(Sim, sim);
            if (relation == null)
            {
                IncStat("Bad Relation");
                return false;
            }

            if ((!relation.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.BreakUp)) &&
                (!relation.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.Divorce)))
            {
                IncStat("Not Breakup");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return true;
        }

        protected override bool Push()
        {
            return Situations.PushVisit(this, Sim, Target.LotHome);
        }

        public override Scenario Clone()
        {
            return new StalkerScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerFriendship,StalkerScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "Stalker";
            }
        }
    }
}
