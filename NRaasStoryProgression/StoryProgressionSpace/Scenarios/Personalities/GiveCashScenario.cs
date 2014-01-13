using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Money;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Personalities
{
    public class GiveCashScenario : MoneyTransferScenario, IHasPersonality
    {
        WeightOption.NameOption mName = null;

        IntegerOption.OptionValue mMinimum;
        IntegerOption.OptionValue mMaximum;

        WeightScenarioHelper mSuccess = null;
        WeightScenarioHelper mFailure = null;

        string mAcceptanceScoring = null;

        bool mFail = false;

        string mAccountingKey = "GiveAway";

        public GiveCashScenario()
            : base(10)
        { }
        protected GiveCashScenario(GiveCashScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mMinimum = scenario.mMinimum;
            mMaximum = scenario.mMaximum;
            mSuccess = scenario.mSuccess;
            mFailure = scenario.mFailure;
            mAcceptanceScoring = scenario.mAcceptanceScoring;
            mAccountingKey = scenario.mAccountingKey;
            //mFail = scenario.mFail;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Minimum=" + mMinimum;
            text += Common.NewLine + "Maximum=" + mMaximum;
            text += Common.NewLine + "Acceptance=" + mAcceptanceScoring;
            text += Common.NewLine + "AccountingKey=" + mAccountingKey;
            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;
            text += Common.NewLine + "Failure" + Common.NewLine + mFailure;

            return text;
        }

        protected override string AccountingKey
        {
            get { return mAccountingKey; }
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

        public override bool IsFriendly
        {
            get { return true; }
        }

        protected override int Minimum
        {
            get { return mMinimum.Value; }
        }

        protected override int Maximum
        {
            get { return mMaximum.Value; }
        }

        protected override bool CheckBusy
        {
            get { return base.ShouldPush; }
        }

        public override bool ShouldPush
        {
            get
            {
                return mSuccess.ShouldPush (base.ShouldPush);
            }
        }

        public SimPersonality Personality
        {
            get { return Manager as SimPersonality; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            if (row.Exists("AccountingKey"))
            {
                mAccountingKey = row.GetString("AccountingKey");
            }
            else
            {
                error = "AccountingKey missing";
                return false;
            }

            mMinimum = new IntegerOption.OptionValue();
            if (!mMinimum.Parse(row, "Minimum", Manager, this, ref error))
            {
                return false;
            }

            mMaximum = new IntegerOption.OptionValue();
            if (!mMaximum.Parse(row, "Maximum", Manager, this, ref error))
            {
                return false;
            }

            mSuccess = new WeightScenarioHelper(Origin.FromCharity);
            if (!mSuccess.Parse(row, Manager, this, "Success", ref error))
            {
                return false;
            }

            mFailure = new WeightScenarioHelper(Origin.FromCharity);
            if (!mFailure.Parse(row, Manager, this, "Failure", ref error))
            {
                return false;
            }

            mAcceptanceScoring = row.GetString("AcceptanceScoring");

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

            mFail = false;
            if (!string.IsNullOrEmpty(mAcceptanceScoring))
            {
                mFail = (AddScoring(mAcceptanceScoring, Target) <= 0);
            }

            if (!mFail)
            {
                ExchangeSims();

                bool result = base.PrivateUpdate(frame);

                ExchangeSims();

                if (result)
                {
                    mSuccess.Perform(this, frame, "Success", Sim, Target);
                }

                return result;
            }
            else
            {
                mFailure.Perform(this, frame, "Failure", Sim, Target);

                return true;
            }
        }

        public override Scenario Clone()
        {
            return new GiveCashScenario(this);
        }
    }
}
