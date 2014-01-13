using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios
{
    public abstract class RabbitHolePushScenario : SimScenario
    {
        public RabbitHolePushScenario()
        { }
        protected RabbitHolePushScenario(RabbitHolePushScenario scenario)
            : base (scenario)
        { }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected abstract RabbitHoleType GetRabbitHole();

        protected override bool Push()
        {
            return Situations.PushToRabbitHole(this, Sim, GetRabbitHole(), true, false);
        }
    }
}
