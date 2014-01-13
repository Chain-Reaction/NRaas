using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ShoolessSpace.Interactions
{
    public class PrivacyInteraction : CommonInteraction<IPrimaryOption<GameObject>, GameObject>
    {
        public static InteractionDefinition Singleton = new CommonDefinition<PrivacyInteraction>(true, true);

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddRoot(Singleton);
            interactions.Add<IToiletOrUrinal>(Singleton);
            interactions.Add<IShowerable>(Singleton);
            interactions.Add<Bathtub>(Singleton);
            interactions.Add<Sink>(Singleton);
        }

        protected override OptionResult Perform(IActor actor, GameObject target, GameObjectHit hit)
        {
            throw new NotImplementedException();
        }
    }
}
