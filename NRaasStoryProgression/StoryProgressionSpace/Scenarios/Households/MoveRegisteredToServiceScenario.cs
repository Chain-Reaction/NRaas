using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
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
    public class MoveRegisteredToServiceScenario : HouseholdScenario
    {
        public MoveRegisteredToServiceScenario(Household house)
            : base(house)
        { }
        public MoveRegisteredToServiceScenario(MoveRegisteredToServiceScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "MoveRegisteredToService";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>())
            {
                IncStat("Disabled");
                return false;
            }

            if (GetValue<NewcomerGoneScenario.AllowHomelessMoveInOptionV2, bool>())
            {
                IncStat("Move In Allowed");
                return false;
            }

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<SimDescription> members = new List<SimDescription>(House.AllSimDescriptions);

            foreach (SimDescription sim in members)
            {
                if (sim.AssignedRole != null)
                {
                    if (!ManagerCareer.IsRegisterInstalled())
                    {
                        IncStat("Not Register");
                        return true;
                    }

                    House.Remove(sim);

                    Household.NpcHousehold.Add(sim);

                    IncStat("Role Moved");
                }
            }

            if (HouseholdsEx.NumSims(House) == 0)
            {
                IncStat("House Empty");
                return true;
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new MoveRegisteredToServiceScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerHousehold>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "MoveRoleToService";
            }
        }
    }
}
