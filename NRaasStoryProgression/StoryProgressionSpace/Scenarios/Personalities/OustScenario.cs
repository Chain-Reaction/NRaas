using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Personalities
{
    public class OustScenario : RelationshipScenario, IHasPersonality
    {
        WeightOption.NameOption mName = null;

        protected bool mFail = false;

        FightScenarioHelper mFight = null;

        bool mReaddOldLeader = false;

        public OustScenario()
            : base(-25)
        { }
        public OustScenario(int delta)
            : base(delta)
        { }
        protected OustScenario(OustScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mFail = scenario.mFail;
            mFight = scenario.mFight;
            mReaddOldLeader = scenario.mReaddOldLeader;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "ReaddOldLeader=" + mReaddOldLeader;
            text += Common.NewLine + "Fight" + Common.NewLine + mFight;

            return text;
        }

        protected override bool Allow()
        {
            if (Manager.GetValue<WeightOption, int>(mName.WeightName) <= 0) return false;

            return base.Allow();
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return mName.WeightName;
            }
            else
            {
                if (mFail)
                {
                    return mName + "Fail";
                }
                else
                {
                    return mName.ToString();
                }
            }
        }

        protected override bool CheckBusy
        {
            get { return base.ShouldPush; }
        }

        public override bool IsFriendly
        {
            get { return false; }
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        protected virtual SimDescription.DeathType DeathType
        {
            get { return SimDescription.DeathType.OldAge; }
        }

        public override bool ShouldPush
        {
            get
            {
                return mFight.ShouldPush(mFail, base.ShouldPush);
            }
        }

        public SimPersonality Personality
        {
            get { return Manager as SimPersonality; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            mFight = new FightScenarioHelper(Origin.FromWatchingSimSuffer, DeathType);
            if (!mFight.Parse(row, Manager, this, ref error))
            {
                return false;
            }

            if (!row.Exists("ReaddOldLeader"))
            {
                error = "ReaddOldLeader Missing";
                return false;
            }

            mReaddOldLeader = row.GetBool("ReaddOldLeader");

            return base.Parse(row, ref error);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return GetFilteredSims(Personalities.GetClanLeader(Manager));
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return GetFilteredTargets(sim);
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Personalities.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Personality.IsPotentialCandiate(sim))
            {
                IncStat("Not Candidate");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool result = mFight.Perform(this, frame, Sim, Target, null, out mFail);

            // Sims are still going to hate each other regardless of fight outcome
            base.PrivateUpdate(frame);

            if (!mFail)
            {
                Personality.SetLeader(Sim, false);

                if (mReaddOldLeader)
                {
                    Personality.AddToClan(this, Target, false);
                }
            }

            return result;
        }

        protected override bool Push()
        {
            return Situations.PushMeetUp(this, Sim, Target, ManagerSituation.MeetUpType.Commercial, ManagerFriendship.FightFirstAction);
        }

        public override Scenario Clone()
        {
            return new OustScenario(this);
        }
    }
}
