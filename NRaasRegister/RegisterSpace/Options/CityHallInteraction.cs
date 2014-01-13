using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.RegisterSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RegisterSpace.Interactions
{
    public class CityHallInteraction : ListedInteraction<IPrimaryOption<GameObject>, GameObject>
    {
        public static InteractionDefinition Singleton = new CommonDefinition<CityHallInteraction>();

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddRoot(Singleton);
        }
    }
}
