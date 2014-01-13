using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using NRaas.CommonSpace.Scoring;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class EmigrationScenario : EmigrateScenario
    {
        public EmigrationScenario()
        { }
        protected EmigrationScenario(EmigrationScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Emigration";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (GetValue<Option,int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            if (!base.Allow(house)) return false;

            int ratio = GetValue<Option, int>();

            int houseRatio = GetValue<EmigrationRatioOption, int>(House);
            if (houseRatio > 0)
            {
                if (ratio > houseRatio)
                {
                    ratio = houseRatio;
                }
            }
            else if (houseRatio < 0)
            {
                IncStat("Forced");
                return true;
            }

            if (AddStat("Ratio", GetValue<NetRatioOption, int>(House)) > ratio)
            {
                IncStat("Ratio");
                return true;
            }

            IncStat("No Match");
            return false;
        }

        public override Scenario Clone()
        {
            return new EmigrationScenario(this);
        }

        public class Option : IntegerScenarioOptionItem<ManagerLot, EmigrationScenario>, ManagerLot.IImmigrationEmigrationOption
        {
            public Option()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "EmigrationRatio";
            }
        }
    }
}
