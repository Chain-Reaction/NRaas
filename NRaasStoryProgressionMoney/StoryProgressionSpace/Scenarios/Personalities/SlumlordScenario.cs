using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
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
    public class SlumlordScenario : DerisionScenario
    {
        public SlumlordScenario()
        { }
        protected SlumlordScenario(SlumlordScenario scenario)
            : base (scenario)
        { }

        protected override bool TargetAllow(SimDescription target)
        {
            if (target.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else
            {
                Household deedOwner = Money.GetDeedOwner(target.LotHome);
                if ((deedOwner != Sim.Household) && ((Mutual == null) || (deedOwner != Mutual.Household)))
                {
                    IncStat("Not Owner");
                    return false;
                }
            }

            return base.TargetAllow(target);
        }

        public override Scenario Clone()
        {
            return new SlumlordScenario(this);
        }
    }
}
