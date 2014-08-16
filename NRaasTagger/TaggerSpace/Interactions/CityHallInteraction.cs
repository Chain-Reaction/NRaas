using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.TaggerSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Interactions
{
    public class CityHallInteraction : ListedInteraction<IMapTagOption, GameObject>
    {
        public static InteractionDefinition Singleton = new CommonDefinition<CityHallInteraction>(true, true);

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<CityHall>(Singleton);
        }
    }
}
