using NRaas.CareerSpace.Situations;
using NRaas.Gameplay.Opportunities;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.Gameplay.Rewards
{
    public class ArrestReward : RewardInfoEx
    {
        public ArrestReward()
        { }

        public override void Grant(Sim actor, object target)
        {
            actor.InteractionQueue.CancelAllInteractions();

            SimArrestSituationEx.Create(actor);
        }
    }
}
