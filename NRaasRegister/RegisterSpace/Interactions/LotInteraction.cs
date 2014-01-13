using NRaas.CommonSpace.Interactions;
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
    public class LotInteraction : ListedInteraction<ILotOption, Lot>
    {
        public static InteractionDefinition Singleton = new CommonDefinition<LotInteraction>(true, true);

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Lot>(Singleton);
        }

        protected override bool Test(IActor actor, Lot target, GameObjectHit hit, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (Common.IsRootMenuObject(target)) return false;

            return base.Test(actor, target, hit, ref greyedOutTooltipCallback);
        }
    }
}
