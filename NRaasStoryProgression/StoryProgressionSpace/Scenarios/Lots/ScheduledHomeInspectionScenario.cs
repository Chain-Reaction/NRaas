using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class ScheduledHomeInspectionScenario : HouseholdScenario
    {
        public ScheduledHomeInspectionScenario()
        { }
        protected ScheduledHomeInspectionScenario(ScheduledHomeInspectionScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "HomeInspection";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override List<Household> GetHouses()
        {
            List<Household> list = new List<Household>();
            foreach (Household house in Household.sHouseholdList)
            {
                if (SimTypes.IsSpecial(house)) continue;

                list.Add(house);
            }

            return list;
        }

        protected override bool Allow(Household house)
        {
            if (house == Household.ActiveHousehold)
            {
                IncStat("Active");
                return false;
            }
            else if (house.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }

            if (!Households.Allow(this, house, GetValue<ManagerHousehold.MinTimeBetweenMovesOption, int>())) 
            {
                IncStat("User Denied");
                return false;
            }
            else if (GetValue<IsAncestralOption,bool>(house)) 
            {
                IncStat("Ancestral Denied");
                return false;
            }
            else if (Lots.PassesHomeInspection(this, house.LotHome, HouseholdsEx.All(house), ManagerLot.FindLotFlags.Inspect | ManagerLot.FindLotFlags.InspectPets))
            {
                IncStat("Pass");
                return false;
            }
            else if (GetValue<DebtOption, int>(house) > 0)
            {
                IncStat("Debt");
                return false;
            }

            return base.Allow(house);
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new BetterHomeMoveInScenario(House), ScenarioResult.Start);
            Add(frame, new InspectionFailScenario(House), ScenarioResult.Failure);
            return true;
        }

        public override Scenario Clone()
        {
            return new ScheduledHomeInspectionScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerLot, ScheduledHomeInspectionScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "HomeInspection";
            }
        }
    }
}
