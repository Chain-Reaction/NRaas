using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
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
    public class DownsizeScenario : DemoteScenario, IHasPersonality
    {
        WeightOption.NameOption mName = null;

        WeightScenarioHelper mSuccess = null;

        bool mbShouldFire = false;

        public DownsizeScenario()
        { }
        protected DownsizeScenario(DownsizeScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mSuccess = scenario.mSuccess;
            mbShouldFire = scenario.mbShouldFire;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "ShouldFire=" + mbShouldFire;
            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;

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
                return mName.ToString();
            }
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

        protected override bool ShouldFire
        {
            get { return mbShouldFire; }
        }

        public SimPersonality Personality
        {
            get { return Manager as SimPersonality; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            mbShouldFire = row.GetBool("ShouldFire");

            mSuccess = new WeightScenarioHelper(Origin.FromWatchingSimSuffer);
            if (!mSuccess.Parse(row, Manager, this, "Success", ref error))
            {
                return false;
            } 

            return base.Parse(row, ref error);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return GetFilteredSims(Personalities.GetClanLeader(Manager));
        }

        protected override bool Allow()
        {
            if (Manager.GetValue<WeightOption, int>(mName.WeightName) <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Personalities.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!mSuccess.TestBeforehand(Manager, Sim, Sim))
            {
                IncStat("Success TestBeforehand Fail");
                return false;
            }

            if (!base.PrivateUpdate(frame)) return false;

            mSuccess.Perform(this, frame, "Success", Sim, null);
            return true;
        }

        public override Scenario Clone()
        {
            return new DownsizeScenario(this);
        }
    }
}
