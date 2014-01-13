using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Interactions;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Interactions
{
    public class RouteToObject : InteractionInstance<Sim,GameObject>
    {
        public static InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            try
            {
                Definition definition = InteractionDefinition as Definition;

                if (definition.mSimToIgnore == null)
                {
                    return Actor.RouteToSlotAndCheckInUse(Target, Slot.RoutingSlot_0);
                }
                Route r = Actor.CreateRoute();
                r.AddObjectToIgnoreForRoute(definition.mSimToIgnore.ObjectId);
                r.PlanToSlot(Target, Slot.RoutingSlot_0);
                return Actor.DoRoute(r);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        [DoesntRequireTuning]
        public class Definition : InteractionDefinition<Sim, GameObject, RouteToObject>
        {
            public readonly Sim mSimToIgnore;

            public Definition()
            { }
            public Definition(Sim simToIgnore)
            {
                mSimToIgnore = simToIgnore;
            }

            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Core/Terrain/GoHereAlone:InteractionName", new object[0]);
            }

            public override bool Test(Sim a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }
    }
}
