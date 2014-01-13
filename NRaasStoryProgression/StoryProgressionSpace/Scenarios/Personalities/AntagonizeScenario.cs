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
    public class AntagonizeScenario : RelationshipScenario, IHasPersonality, IViolentScenario
    {
        WeightOption.NameOption mName = null;

        protected bool mFail = false;

        bool mChildStory = true;

        FightScenarioHelper mFight = null;

        public AntagonizeScenario()
            : base(-50)
        { }
        public AntagonizeScenario(int delta)
            : base(delta)
        { }
        protected AntagonizeScenario(AntagonizeScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mFail = scenario.mFail;
            mFight = scenario.mFight;
            mChildStory = scenario.mChildStory;
        }

        public override string ToString()
        {
            string text = base.ToString();

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
                else if ((mChildStory) && (Target != null) && (Target.Child))
                {
                    return mName + "Child";
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

        public bool IsViolent
        {
            get { return true; }
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

            if (row.Exists("ChildStory"))
            {
                mChildStory = row.GetBool("ChildStory");
            }

            mFight = new FightScenarioHelper(Origin.FromWatchingSimSuffer, DeathType);
            if (!mFight.Parse(row, Manager, this, ref error))
            {
                return false;
            }

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

        protected override bool TargetAllow(SimDescription sim)
        {
            if ((Sim.YoungAdultOrAbove) && (sim.ChildOrBelow))
            {
                IncStat("Adult-Child Denied");
                return false;
            }
            else if ((sim.Gender == CASAgeGenderFlags.Female) && (Sim.Gender == CASAgeGenderFlags.Male) && (!GetValue<AllowMaleFemaleVictimOptionV2, bool>()))
            {
                IncStat("Male-Female Denied");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool result = mFight.Perform(this, frame, Sim, Target, null, out mFail);

            // Sims are still going to hate each other regardless of fight outcome
            base.PrivateUpdate(frame);

            return result;
        }

        protected override bool Push()
        {
            return Situations.PushMeetUp(this, Sim, Target, ManagerSituation.MeetUpType.Commercial, ManagerFriendship.FightFirstAction);
        }

        public override Scenario Clone()
        {
            return new AntagonizeScenario(this);
        }

        public class AllowMaleFemaleVictimOptionV2 : BooleanManagerOptionItem<ManagerPersonality>
        {
            public AllowMaleFemaleVictimOptionV2()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowMaleFemaleAntagonize";
            }
        }
    }
}
