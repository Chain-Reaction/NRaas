using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
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
    public class LynchMobScenario : EmigrateScenario
    {
        public LynchMobScenario()
        { }
        protected LynchMobScenario(LynchMobScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "LynchMob";
        }

        protected override bool Allow()
        {
            if (GetValue<ThresholdOption,int>() <= 0) return false;

            return base.Allow();
        }

        protected int GetTotalFriends(Household house)
        {
            int total = 0;
            foreach (SimDescription sim in HouseholdsEx.All(house))
            {
                total += Friends.FindExistingFriendFor(this, sim, 50, true).Count;
            }

            return total;
        }

        protected int GetTotalEnemies(Household house)
        {
            int total = 0;
            foreach (SimDescription sim in HouseholdsEx.All(house))
            {
                total += Friends.FindExistingEnemyFor(this, sim, -50, true).Count;
            }

            return total;
        }

        protected override bool Allow(Household house)
        {
            if (!base.Allow(house)) return false;

            int totalEnemies = GetTotalEnemies(House) - GetTotalFriends(House);
            if (totalEnemies < GetValue<ThresholdOption, int>())
            {
                IncStat("Too Few");
                return false;
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new LynchMobScenario(this);
        }

        public class ThresholdOption : IntegerScenarioOptionItem<ManagerFriendship, LynchMobScenario>
        {
            public ThresholdOption()
                : base(20)
            { }

            public override string GetTitlePrefix()
            {
                return "LynchMobThreshold";
            }

            public override int Value
            {
                get
                {
                    if (!ShouldDisplay()) return 0;

                    return base.Value;
                }
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<EmigrationScenario.Option, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
