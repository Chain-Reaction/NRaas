using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Methods;
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
    public class StealScenario : NRaas.StoryProgressionSpace.Scenarios.Money.StealScenario
    {
        string mName = "Steal";

        IntegerOption.OptionValue mMinimum;
        IntegerOption.OptionValue mMaximum;

        bool mKeepObject = false;

        WeightScenarioHelper mSuccess = null;
        WeightScenarioHelper mFailure = null;

        string mSneakinessScoring = "Sneakiness";

        bool mAllowGoToJail = false;

        public StealScenario()
            : base(-10)
        { }
        protected StealScenario(StealScenario scenario)
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
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += "\nMinimum=" + mMinimum;
            text += "\nMaximum=" + mMaximum;
            text += "\nAllowGoToJail=" + mAllowGoToJail;
            text += "\nKeepObject=" + mKeepObject;
            text += "\nSneakinessScoring=" + mSneakinessScoring;
            text += "\nSuccess\n" + mSuccess;
            text += "\nFailure\n" + mFailure;

            return text;
        }

        public override string Name
        {
            get { return mName; }
        }

        protected override bool AllowGoToJail
        {
            get { return mAllowGoToJail; }
        }

        protected override int Minimum
        {
            get { return mMinimum.Value; }
        }

        protected override int Maximum
        {
            get { return mMaximum.Value; }
        }

        protected override bool KeepObject
        {
            get { return mKeepObject; }
        }

        public override bool IsFriendly
        {
            get { return false; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = row.GetString("Name");

            if (!mMinimum.Parse(row, "Minimum", Manager, ref error))
            {
                return false;
            }

            if (!mMaximum.Parse(row, "Maximum", Manager, ref error))
            {
                return false;
            }

            mAllowGoToJail = row.GetBool("AllowGoToJail");

            mKeepObject = row.GetBool("KeepObject");

            mSneakinessScoring = row.GetString("SneakinessScoring");

            mSuccess = new WeightScenarioHelper(Origin.FromBurglar);
            if (!mSuccess.Parse(row, Manager, "Success", ref error))
            {
                return false;
            }

            mFailure = new WeightScenarioHelper(Origin.FromBurglar);
            if (!mFailure.Parse(row, Manager, "Failure", ref error))
            {
                return false;
            }

            return base.Parse(row, ref error);
        }

        protected override List<SimDescription> GetSims()
        {
            return GetFilteredSims(Personalities.GetClanLeader(Manager));
        }

        protected override List<SimDescription> GetTargets(SimDescription sim)
        {
            return GetFilteredTargets(sim);
        }

        public override bool Allow()
        {
            if (GetValue<WeightOption, int>(Manager, Name) <= 0) return false;

            return base.Allow();
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Money.Allow(sim))
            {
                IncStat("Money Denied");
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

            return (AddScoring("Target Sneak", GetScore(mSneakinessScoring, target)) > AddScoring("Sim Sneak", GetScore(mSneakinessScoring, sim)));
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            Add(frame, new PropagateClanDelightScenario(Sim, Manager, Origin.FromBurglar), ScenarioResult.Start);

            if (Fail)
            {
                mFailure.Perform(this, frame, Sim, Target);
            }
            else
            {
                mSuccess.Perform(this, frame, Sim, Target);
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new StealScenario(this);
        }
    }
}
