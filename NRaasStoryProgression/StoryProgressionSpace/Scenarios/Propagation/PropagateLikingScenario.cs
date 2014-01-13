using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Propagation
{
    public class PropagateLikingScenario : PropagateBuffScenario
    {
        SimDescription mFriend;

        WhichSim mFriendSim = WhichSim.Unset;

        int mDelta = 10;

        int mRelationshipGate = -101;

        public PropagateLikingScenario()
            : base(BuffNames.Delighted, Origin.FromSocialization)
        { }
        public PropagateLikingScenario(SimDescription primary, SimDescription friend, int delta)
            : this(primary, friend, BuffNames.Delighted, Origin.FromSocialization, delta)
        { }
        protected PropagateLikingScenario(SimDescription primary, SimDescription friend, BuffNames buff, Origin origin, int delta)
            : base(primary, buff, origin)
        {
            mFriend = friend;
            mDelta = delta;
        }
        protected PropagateLikingScenario(PropagateLikingScenario scenario)
            : base(scenario)
        {
            mFriend = scenario.mFriend;
            mFriendSim = scenario.mFriendSim;
            mDelta = scenario.mDelta;
            mRelationshipGate = scenario.mRelationshipGate;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Delta=" + mDelta;
            text += Common.NewLine + "FriendSim=" + mFriendSim;
            text += Common.NewLine + "RelationshipGate=" + mRelationshipGate;

            return text;
        }

        protected SimDescription Friend
        {
            get { return mFriend; }
        }

        protected override bool UsesSim(ulong sim)
        {
            if (mFriend != null)
            {
                if (mFriend.SimDescriptionId == sim) return true;
            }

            return base.UsesSim(sim);
        }

        public override bool Parse(XmlDbRow row, string prefix, bool firstPass, ref string error)
        {
            mDelta = row.GetInt(prefix + "PropagateDelta", mDelta);

            if (row.Exists(prefix + "PropagateFriend"))
            {
                if (!ParserFunctions.TryParseEnum<WhichSim>(row.GetString(prefix + "PropagateFriend"), out mFriendSim, WhichSim.Actor))
                {
                    error = prefix + "PropagateFriend unknown";
                    return false;
                }
            }

            SimScenarioFilter.RelationshipLevel relationLevel;
            if (ParserFunctions.TryParseEnum<SimScenarioFilter.RelationshipLevel>(row.GetString(prefix + "PropagateRelationshipGate"), out relationLevel, SimScenarioFilter.RelationshipLevel.Neutral))
            {
                mRelationshipGate = (int)relationLevel;
            }
            else
            {
                mRelationshipGate = row.GetInt(prefix + "PropagateRelationshipGate", mRelationshipGate);
            }

            return base.Parse(row, prefix, firstPass, ref error);
        }

        public override bool PostParse(ref string error)
        {
            if (mFriendSim == WhichSim.Unset)
            {
                error = "PropagateFriend Unset";
                return false;
            }

            return base.PostParse(ref error);
        }

        public override void SetActors(SimDescription actor, SimDescription target)
        {
            base.SetActors(actor, target);

            if (mFriendSim == WhichSim.Target)
            {
                mFriend = target;
            }
            else
            {
                mFriend = actor;
            }
        }

        protected override bool Allow()
        {
            if (!GetValue<ScheduledFriendScenario.AllowFriendOption, bool>()) return false;

            return base.Allow();
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (sim == mFriend)
            {
                IncStat("Friend");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if ((ManagerSim.HasTrait(Target, TraitNames.Evil)) ||
                (ManagerSim.HasTrait(Target, TraitNames.MeanSpirited)))
            {
                return false;
            }

            // Not performed for the main Sim on purpose
            if (Target != Sim)
            {
                base.PrivateUpdate(frame);

                bool fail = false;

                Relationship relation = Relationship.Get(mFriend, Target, false);
                if (relation != null)
                {
                    if (relation.LTR.Liking <= mRelationshipGate)
                    {
                        fail = true;
                    }
                }

                if (!fail)
                {
                    int report = 0;
                    if (mFriend.Partner == Target)
                    {
                        report = ReportChance;
                    }

                    Add(frame, new ExistingFriendManualScenario(mFriend, Target, mDelta, report, GetTitlePrefix(PrefixType.Story)), ScenarioResult.Start);
                }
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new PropagateLikingScenario(this);
        }
    }
}
