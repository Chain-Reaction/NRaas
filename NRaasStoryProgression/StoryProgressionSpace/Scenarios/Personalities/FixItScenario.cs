using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
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
    public class FixItScenario : RepairScenario
    {
        WeightOption.NameOption mName = null;

        SimScenarioFilter mTargetFilter = null;

        WeightScenarioHelper mSuccess = null;

        public FixItScenario()
        { }
        protected FixItScenario(FixItScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mTargetFilter = scenario.mTargetFilter;
            mSuccess = scenario.mSuccess;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;
            text += Common.NewLine + "TargetFilter=" + mTargetFilter;

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
                if (TownRepair)
                {
                    return mName + "Town";
                }
                else
                {
                    return mName.ToString();
                }
            }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            mTargetFilter = new SimScenarioFilter();
            if (!mTargetFilter.Parse(row, Manager, this, "Target", true, ref error))
            {
                return false;
            }

            mSuccess = new WeightScenarioHelper(Origin.FromCharity);
            if (!mSuccess.Parse(row, Manager, this, "Success", ref error))
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

        protected override bool AllowHouse(Household house)
        {
            foreach (SimDescription member in HouseholdsEx.All(house))
            {
                if (HasValue<DisallowPersonalityOption, SimPersonality>(member, Manager as SimPersonality)) return false;
            }

            SimDescription head = SimTypes.HeadOfFamily(house);
            if (head == null) return false;

            return mTargetFilter.Test(new SimScenarioFilter.Parameters(this, false), "Target", Sim, head);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!mSuccess.TestBeforehand(Manager, Sim, Sim))
            {
                IncStat("Success TestBeforehand Fail");
                return false;
            }

            if (!base.PrivateUpdate(frame)) return false;

            Add(frame, new PropagateClanDelightScenario(Sim, Manager, Origin.FromCharity), ScenarioResult.Start);

            if (House != Sim.Household)
            {
                mSuccess.Perform(this, frame, "Success", Sim, SimTypes.HeadOfFamily(House));
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new FixItScenario(this);
        }
    }
}
