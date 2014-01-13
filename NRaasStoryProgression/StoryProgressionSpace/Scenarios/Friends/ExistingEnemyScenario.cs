using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
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
    public class ExistingEnemyScenario : RelationshipScenario
    {
        bool mNemesis = false;

        public ExistingEnemyScenario(SimDescription sim, SimDescription target)
            : this(sim, target, new SimScoringParameters(sim).Score("Friendly"), true)
        { }
        public ExistingEnemyScenario(SimDescription sim, SimDescription target, int delta, bool shouldPush)
            : base(sim, target, delta)
        {
            mShouldPush = shouldPush;
        }
        protected ExistingEnemyScenario(ExistingEnemyScenario scenario)
            : base (scenario)
        {
            mNemesis = scenario.mNemesis;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "ExistingEnemy";
        }

        protected override bool CheckBusy
        {
            get { return ShouldPush; }
        }

        public override bool IsFriendly
        {
            get { return false; }
        }

        protected override bool TestRelationship
        {
            get { return true; }
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Nemesis=" + mNemesis;

            return text;
        }

        protected override bool Allow()
        {
            if (!GetValue<ScheduledEnemyScenario.AllowEnemyOption, bool>()) return false;

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
            if (Delta > 0)
            {
                IncStat("Friendly");
                return false;
            }
            else if (Relationship.AreStrangers(Sim, Target))
            {
                IncStat("Unknown");
                return false;
            }

            return base.TargetAllow(sim);
        }

        public static bool HandleNemesis(SimDescription sim, SimDescription target)
        {
            Relationship relation = ManagerSim.GetRelationship(sim, target);
            if (relation == null) return false;

            if ((relation.LTR.Liking <= -100) &&
                (!relation.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.MakeEnemy)))
            {
                relation.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.MakeEnemy);

                return true;
            }

            return false;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            mNemesis = HandleNemesis(Sim, Target);

            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Friends;
            }

            if (!Sim.IsHuman)
            {
                if (string.IsNullOrEmpty(name)) return null;

                name += "Pet";
            }
            else if (mNemesis)
            {
                SimPersonality clan = Manager as SimPersonality;
                if (clan != null)
                {
                    if (clan.Me != Sim) return null;
                }

                name = "Nemesis";
            }
            else if (Sim.Partner == Target)
            {
                if (string.IsNullOrEmpty(name)) return null;

                name += "Partner";
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new ExistingEnemyScenario(this);
        }
    }
}
