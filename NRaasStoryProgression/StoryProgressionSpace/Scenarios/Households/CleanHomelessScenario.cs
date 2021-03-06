﻿using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
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
    public class CleanHomelessScenario : HouseholdScenario
    {
        public CleanHomelessScenario(Household house)
            : base(house)
        { }
        public CleanHomelessScenario(CleanHomelessScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "CleanHomeless";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            foreach (SimDescription sim in new List<SimDescription>(HouseholdsEx.All(House)))
            {
                string reason = null;
                if (!ManagerSim.ValidSim(sim, out reason))
                {
                    Deaths.CleansingKill(sim, false);
                }
            }

            if (HouseholdsEx.NumSims(House) == 0)
            {
                return true;
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new CleanHomelessScenario(this);
        }
    }
}
