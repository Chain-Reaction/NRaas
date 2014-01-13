using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Methods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
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
    public class ArsonScenario : NRaas.StoryProgressionSpace.Scenarios.Lots.ArsonScenario
    {
        string mName = "Arson";

        bool mAllowInjury = false;

        WeightScenarioHelper mSuccess = null;
        WeightScenarioHelper mFailure = null;

        string mSneakinessScoring = "Sneakiness";

        bool mAllowGoToJail = false;

        public ArsonScenario()
            : base(-25)
        { }
        protected ArsonScenario(ArsonScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mAllowInjury = scenario.mAllowInjury;
            mSuccess = scenario.mSuccess;
            mFailure = scenario.mFailure;
            mSneakinessScoring = scenario.mSneakinessScoring;
            mAllowGoToJail = scenario.mAllowGoToJail;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += "\nAllowInjury=" + mAllowInjury;
            text += "\nAllowGoToJail=" + mAllowGoToJail;
            text += "\nSuccess\n" + mSuccess;
            text += "\nFailure\n" + mFailure;
            text += "\nSneakinessScoring=" + mSneakinessScoring;

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

        protected override bool AllowInjury
        {
            get { return mAllowInjury; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = row.GetString("Name");

            mAllowGoToJail = row.GetBool("AllowGoToJail");

            mAllowInjury = row.GetBool("AllowInjury");

            mSneakinessScoring = row.GetString("SneakinessScoring");

            mSuccess = new WeightScenarioHelper(Origin.FromFire);
            if (!mSuccess.Parse(row, Manager, "Success", ref error))
            {
                return false;
            }

            mFailure = new WeightScenarioHelper(Origin.FromFire);
            if (!mFailure.Parse(row, Manager, "Failure", ref error))
            {
                return false;
            }

            return base.Parse(row, ref error);
        }

        protected override Scenario.UpdateDelegate AdditionalScenario
        {
            get { return OnAdditionalScenario; }
        }

        public override bool Allow()
        {
            if (GetValue<WeightOption, int>(Manager, Name) <= 0) return false;

            return base.Allow();
        }

        protected override List<SimDescription> GetSims()
        {
            return GetFilteredSims(Personalities.GetClanLeader(Manager));
        }

        protected override List<SimDescription> GetTargets(SimDescription sim)
        {
            return GetFilteredTargets(sim);
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Personalities.Allow(sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.CommonAllow(sim);
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

        protected void OnAdditionalScenario(Scenario scenario, ScenarioFrame frame)
        {
            BurnScenario s = scenario as BurnScenario;
            if (s == null) return;

            s.Add(frame, new PropagateClanDelightScenario(s.Sim, Manager, Origin.FromFire), ScenarioResult.Start);

            if (Fail)
            {
                mFailure.Perform(scenario, frame, s.Sim, s.Target);
            }
            else
            {
                mSuccess.Perform(scenario, frame, s.Sim, s.Target);
            }
        }

        public override Scenario Clone()
        {
            return new ArsonScenario(this);
        }
    }
}
