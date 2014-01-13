using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
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
    public class InitialHomelessScenario : ScheduledHomelessScenario 
    {
        public InitialHomelessScenario ()
        { }
        protected InitialHomelessScenario(InitialHomelessScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "InitialHomeless";
        }

        protected override bool Allow(bool fullUpdate, bool initialPass)
        {
            return true;
        }

        protected override GatherResult Gather(List<Scenario> list, ref int continueChance, ref int maximum, ref bool random)
        {
            GatherResult result = base.Gather(list, ref continueChance, ref maximum, ref random);

            if (result == GatherResult.Success)
            {
                Scenarios.DumpStats();

                if (!AcceptCancelDialog.Show(Localize("InitialHomelessPrompt")))
                {
                    return GatherResult.Failure;
                }
            }

            return result;
        }

        public override Scenario Clone()
        {
            return new InitialHomelessScenario(this);
        }
    }
}
