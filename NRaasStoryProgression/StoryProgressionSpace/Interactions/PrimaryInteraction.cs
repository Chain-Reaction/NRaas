using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class PrimaryInteraction : ListedInteraction<IPrimaryOption<GameObject>, GameObject>
    {
        public static InteractionDefinition Singleton = new CommonDefinition<PrimaryInteraction>();

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddRoot(Singleton);
            interactions.Add<Lot>(Singleton);
            interactions.Add<Sim>(Singleton);
            interactions.Add<BuildableShell>(Singleton);
            interactions.Add<RabbitHole>(Singleton);
        }

        protected override bool Test(IActor actor, GameObject target, GameObjectHit hit, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (StoryProgression.Main == null) return false;

            if (target is Sim)
            {
                if (!StoryProgressionSpace.Managers.ManagerSim.AddInteractionsOption.sQuickCheck)
                {
                    return false;
                }
            }
            else if ((target is Lot) || (target is BuildableShell))
            {
                if (!StoryProgressionSpace.Managers.ManagerLot.AddInteractionsOption.sQuickCheck)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
