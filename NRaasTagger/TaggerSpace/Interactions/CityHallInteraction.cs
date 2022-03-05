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
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Interactions
{
    public class CityHallInteraction : ListedInteraction<ICityHallOption, GameObject>
    {
        public static InteractionDefinition Singleton = new CommonDefinition<CityHallInteraction>(true, true);

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<CityHall>(Singleton);
            interactions.Add<ComboCityhallPoliceMilitary>(Singleton);
            interactions.Add<AdminstrationCenter>(Singleton);
            interactions.Add<Lot>(Singleton);
            interactions.Add<Computer>(Singleton);
        }

        protected override bool Test(IActor actor, GameObject target, GameObjectHit hit, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (target is Lot)
            {
                return Common.IsRootMenuObject(target);
            }

            return true;
        }
    }
}
