using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
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
    public class BurgleScenario : StealScenario, IHasPersonality
    {
        WeightOption.NameOption mName = null;

        IntegerOption.OptionValue mMinimum;
        IntegerOption.OptionValue mMaximum;

        bool mKeepObject = false;

        WeightScenarioHelper mSuccess = null;
        WeightScenarioHelper mFailure = null;

        string mSneakinessScoring = "Sneakiness";

        bool mAllowGoToJail = false;

        IntegerOption.OptionValue mBail = null;

        InvestigationHelper mInvestigate = null;
      
        public BurgleScenario()
            : base(-10)
        { }
        protected BurgleScenario(BurgleScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mMinimum = scenario.mMinimum;
            mMaximum = scenario.mMaximum;
            mKeepObject = scenario.mKeepObject;
            mSuccess = scenario.mSuccess;
            mFailure = scenario.mFailure;
            mSneakinessScoring = scenario.mSneakinessScoring;
            mAllowGoToJail = scenario.mAllowGoToJail;
            mBail = scenario.mBail;
            mInvestigate = scenario.mInvestigate;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Minimum=" + mMinimum;
            text += Common.NewLine + "Maximum=" + mMaximum;
            text += Common.NewLine + "AllowGoToJail=" + mAllowGoToJail;
            text += Common.NewLine + "Bail=" + mBail;
            text += Common.NewLine + "KeepObject=" + mKeepObject;
            text += Common.NewLine + "SneakinessScoring=" + mSneakinessScoring;
            text += Common.NewLine + "Investigate" + Common.NewLine + mInvestigate;
            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;
            text += Common.NewLine + "Failure" + Common.NewLine + mFailure;

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
                if (Fail)
                {
                    return mName + "Fail";
                }
                else
                {
                    return mName.ToString();
                }
            }
        }

        public override string InvestigateStoryName
        {
            get
            {
                return mInvestigate.GetStoryName(base.InvestigateStoryName);
            }
        }

        public override int InvestigateMinimum
        {
            get { return mInvestigate.GetMinimum(250); }
        }

        public override int InvestigateMaximum
        {
            get { return mInvestigate.GetMaximum(1000); }
        }

        public override bool AllowGoToJail
        {
            get { return mAllowGoToJail; }
        }

        protected override int Bail
        {
            get 
            {
                if (mBail.Value > 0)
                {
                    return mBail.Value;
                }
                else
                {
                    return base.Bail;
                }
            }
        }

        protected override int Minimum
        {
            get { return mMinimum.Value; }
        }

        protected override int Maximum
        {
            get { return mMaximum.Value; }
        }

        public override bool KeepObject
        {
            get { return mKeepObject; }
        }

        public override bool IsFriendly
        {
            get { return false; }
        }

        public SimPersonality Personality
        {
            get { return Manager as SimPersonality; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            if (!row.Exists("AllowGoToJail"))
            {
                error = "AllowGoToJail missing";
                return false;
            }

            mAllowGoToJail = row.GetBool("AllowGoToJail");

            mBail = new IntegerOption.OptionValue(-1);
            if (row.Exists("Bail"))
            {
                if (!mBail.Parse(row, "Bail", Manager, this, ref error))
                {
                    return false;
                }
            }

            mInvestigate = new InvestigationHelper();
            if (!mInvestigate.Parse(row, Manager, this, ref error))
            {
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

            mKeepObject = row.GetBool("KeepObject");

            mSneakinessScoring = row.GetString("SneakinessScoring");

            mSuccess = new WeightScenarioHelper(Origin.FromBurglar);
            if (!mSuccess.Parse(row, Manager, this, "Success", ref error))
            {
                return false;
            }

            mFailure = new WeightScenarioHelper(Origin.FromBurglar);
            if (!mFailure.Parse(row, Manager, this, "Failure", ref error))
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

        protected override bool Allow()
        {
            if (Manager.GetValue<WeightOption, int>(mName.WeightName) <= 0) return false;

            return base.Allow();
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Money.Allow(this, sim))
            {
                IncStat("Money Denied");
                return false;
            }
            else if (!Personalities.Allow(this, sim))
            {
                IncStat("Personality Denied");
                return false;
            }

            return (base.CommonAllow(sim));
        }

        protected override bool IsFail(SimDescription sim, SimDescription target)
        {
            if (string.IsNullOrEmpty(mSneakinessScoring))
            {
                IncStat("No Sneakiness Scoring");
                return false;
            }

            return (AddScoring("Target Sneak", ScoringLookup.GetScore(mSneakinessScoring, target)) > AddScoring("Sim Sneak", ScoringLookup.GetScore(mSneakinessScoring, sim)));
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

            if (!base.PrivateUpdate(frame)) return false;

            Add(frame, new PropagateClanDelightScenario(Sim, Manager, Origin.FromBurglar), ScenarioResult.Start);

            if (Fail)
            {
                mFailure.Perform(this, frame, "Failure", Sim, Target);
            }
            else
            {
                mSuccess.Perform(this, frame, "Success", Sim, Target);
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new BurgleScenario(this);
        }
    }
}
