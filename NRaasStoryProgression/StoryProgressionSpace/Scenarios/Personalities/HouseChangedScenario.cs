using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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
    public class HouseChangedScenario : HouseChangedBaseScenario
    {
        public HouseChangedScenario()
        { }
        protected HouseChangedScenario(HouseChangedScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "HouseChanged";
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (NewHouse != null)
            {
                List<SimDescription> sims = new List<SimDescription>();

                foreach (SimDescription sim in CommonSpace.Helpers.Households.All(NewHouse))
                {
                    if (!SimTypes.IsSelectable(sim))
                    {
                        if (GetValue<ManagerSim.ProgressActiveNPCOption, bool>()) continue;
                    }

                    if (Personalities.GetClanLeadership(sim).Count > 0)
                    {
                        sims.Add(sim);
                    }
                }

                if (sims.Count > 0)
                {
                    Common.Notify(sims[0], Common.Localize("ActivePersonality:Prompt"));
                }
            }
            return false;
        }

        public override Scenario Clone()
        {
            return new HouseChangedScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerPersonality, HouseChangedScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "PersonalityHouseChanged";
            }
        }
    }
}
