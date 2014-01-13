using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Friends
{
    public class ExistingEnemyManualScenario : ExistingEnemyScenario
    {
        string mStoryName = null;

        public ExistingEnemyManualScenario(SimDescription sim, SimDescription target, int delta, int reportChance)
            : this(sim, target, delta, reportChance, null)
        { }
        public ExistingEnemyManualScenario(SimDescription sim, SimDescription target, int delta, int reportChance, string storyName)
            : base(sim, target, delta, false)
        {
            mInitialReportChance = reportChance;
            mContinueReportChance = reportChance;
            mStoryName = storyName;
        }
        protected ExistingEnemyManualScenario(ExistingEnemyManualScenario scenario)
            : base (scenario)
        {
            mStoryName = scenario.mStoryName;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "StoryName=" + mStoryName;

            return text;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return base.GetTitlePrefix(type);
            }
            else
            {
                return mStoryName;
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            PushAndPrint();
            return false;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Manager;
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new ExistingEnemyManualScenario(this);
        }
    }
}
