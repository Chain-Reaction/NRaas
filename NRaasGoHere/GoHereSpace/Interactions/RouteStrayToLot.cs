using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.UI;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class StrayRouteToLot : Interaction<IActor, Lot>
    {
        public static InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            bool flag = false;            
            if (base.Target.IsResidentialLot)
            {
                Door door = null;
                flag = base.Target.RouteToFrontDoorOrMailbox((Sim) base.Actor, Sim.MinDistanceFromDoorWhenVisiting, Sim.MaxDistanceFromDoorWhenVisiting, ref door, true, true);
            }
            else
            {
                Route route = base.Actor.CreateRoute();
                base.Target.PlanToLotEx(route);
                flag = base.Actor.DoRoute(route);
            }

            return flag;
        }

        [DoesntRequireTuning]
        private class Definition : MetaInteractionDefinition<Sim, Lot, StrayRouteToLot>
        {
            public Definition()
            { }     

            public override bool Test(Sim actor, Lot target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return actor.SimDescription.IsStray;
            }
        }
    }
}
