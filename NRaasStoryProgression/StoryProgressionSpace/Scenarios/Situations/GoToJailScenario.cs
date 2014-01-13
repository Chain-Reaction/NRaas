using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class GoToJailScenario : GoToJailBaseScenario
    {
        string mStoryName;

        int mBail;

        public GoToJailScenario(SimDescription sim, int bail)
            : base(sim)
        {
            mBail = bail;
        }
        public GoToJailScenario(SimDescription sim, string storyName, int bail)
            : base(sim)
        {
            mBail = bail;
            mStoryName = storyName;
        }
        protected GoToJailScenario(GoToJailScenario scenario)
            : base (scenario)
        {
            mBail = scenario.mBail;
            mStoryName = scenario.mStoryName;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "GoToJailPush";
        }

        protected override int Chance
        {
            get 
            {
                return AddScoring("GoToJail", GetValue<ChanceGoToJailOption, int>(Sim), ScoringLookup.OptionType.Bounded, Sim); 
            }
        }

        protected override int Bail
        {
            get { return mBail; }
        }

        protected override string StoryName
        {
            get { return mStoryName; }
        }

        public override Scenario Clone()
        {
            return new GoToJailScenario(this);
        }
    }
}
