using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
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
    public class DropMemberScenario : DualSimScenario, IHasPersonality
    {
        WeightOption.NameOption mName = null;

        string mAcceptanceScoring = null;

        WeightScenarioHelper mSuccess = null;
        WeightScenarioHelper mFailure = null;

        bool mFail = false;

        int mChance = 100;

        public DropMemberScenario()
        { }
        protected DropMemberScenario(DropMemberScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mAcceptanceScoring = scenario.mAcceptanceScoring;
            mSuccess = scenario.mSuccess;
            mFailure = scenario.mFailure;
            mFail = scenario.mFail;
            mChance = scenario.mChance;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "AcceptanceScoring=" + mAcceptanceScoring;
            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;
            text += Common.NewLine + "Failure" + Common.NewLine + mFailure;
            text += Common.NewLine + "Chance=" + mChance;

            return text;
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

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool CheckBusy
        {
            get { return base.ShouldPush; }
        }

        public override bool ShouldPush
        {
            get
            {
                return mSuccess.ShouldPush(base.ShouldPush);
            }
        }

        public SimPersonality Personality
        {
            get { return Manager as SimPersonality; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            mChance = row.GetInt("Chance", mChance);

            mAcceptanceScoring = row.GetString("AcceptanceScoring");

            mSuccess = new WeightScenarioHelper(Origin.None);
            if (!mSuccess.Parse(row, Manager, this, "Success", ref error))
            {
                return false;
            }

            mFailure = new WeightScenarioHelper(Origin.FromRomanticBetrayal);
            if (!mFailure.Parse(row, Manager, this, "Failure", ref error))
            {
                return false;
            }

            return base.Parse(row, ref error);
        }

        protected override bool Allow()
        {
            if (Manager.GetValue<WeightOption, int>(mName.WeightName) <= 0) return false;

            return base.Allow();
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

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!mSuccess.TestBeforehand(Manager, Sim, Target))
            {
                IncStat("Success TestBeforehand Fail");
                return false;
            }

            if (!mFailure.TestBeforehand(Manager, Sim, Target))
            {
                IncStat("Failure TestBeforehand Fail");
                return false;
            }

            if (!RandomUtil.RandomChance(mChance))
            {
                IncStat("Chance Fail");
                return false;
            }

            SimPersonality clan = Personalities.GetPersonality(TargetFilter.Clan);

            if (clan == null)
            {
                clan = Manager as SimPersonality;
            }

            if (clan == null)
            {
                IncStat("Clan Missing");
                return false;
            }

            mFail = ((!string.IsNullOrEmpty(mAcceptanceScoring)) && (AddScoring("Acceptance", ScoringLookup.GetScore(mAcceptanceScoring, Target, Sim)) <= 0));

            if (!mFail)
            {
                if (!clan.RemoveFromClan(Target)) return false;

                mSuccess.Perform(this, frame, "Success", Sim, Target);
            }
            else
            {
                mFailure.Perform(this, frame, "Failure", Sim, Target);
            }
            return true;
        }

        public override Scenario Clone()
        {
            return new DropMemberScenario(this);
        }
    }
}
