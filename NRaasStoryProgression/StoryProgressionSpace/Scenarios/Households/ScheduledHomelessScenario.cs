using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public class ScheduledHomelessScenario : HouseholdScenario 
    {
        public ScheduledHomelessScenario ()
        { }
        protected ScheduledHomelessScenario(ScheduledHomelessScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Homeless";
        }

        protected override int ContinueChance
        {
            get 
            {
                if (GetValue<NewcomerGoneScenario.AllowHomelessMoveInOptionV2, bool>())
                {
                    return 0;
                }
                else
                {
                    return 100;
                }
            }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<OptionV2,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            if (house.LotHome != null)
            {
                IncStat("Resident");
                return false;
            }
            else if (SimTypes.IsSpecial(house))
            {
                IncStat("Special");
                return false;
            }

            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            IncStat("Found", Common.DebugLevel.High);

            Add(frame, new AttachLostFathersScenario(HouseholdsEx.All(House)), ScenarioResult.Start);
            Add(frame, new MoveRegisteredToServiceScenario(House), ScenarioResult.Start);
            Add(frame, new CleanHomelessScenario(House), ScenarioResult.Failure);
            Add(frame, new HomelessSplitFamilyScenario(House), ScenarioResult.Failure);
            Add(frame, new NewcomerGoneScenario(House), ScenarioResult.Failure);
            Add(frame, new HomelessMoveInLotScenario(HouseholdsEx.All(House)), ScenarioResult.Failure);
            Add(frame, new NewcomerFailScenario(House), ScenarioResult.Failure);

            Add(frame, new HomelessMergeScenario(House), ScenarioResult.Start);

            return false;
        }

        public override Scenario Clone()
        {
            return new ScheduledHomelessScenario(this);
        }

        public class OptionV2 : BooleanScenarioOptionItem<ManagerHousehold, ScheduledHomelessScenario>
        {
            public OptionV2()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ManageHomeless";
            }
        }
    }
}
