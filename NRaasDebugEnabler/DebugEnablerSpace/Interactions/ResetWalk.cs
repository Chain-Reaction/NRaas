using NRaas.CommonSpace.Interactions;
using NRaas.DebugEnablerSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class ResetWalk : DebugEnablerInteraction<GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if ((obj is Sim) || (obj is CityHall))
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        private static void Perform(Sim sim)
        {
            if (sim.SimRoutingComponent == null) return;

            foreach(KeyValuePair<Sim.WalkStyle,int> style in sim.SimRoutingComponent.mWalkStyleRequests)
            {
                if (style.Value > 0)
                {
                    sim.SimRoutingComponent.mWalkStyleRequests[style.Key] = 0;
                }
            }

            sim.SimRoutingComponent.UpdateWalkStyle();
        }

        public override bool Run()
        {
            try
            {
                Sim sim = Target as Sim;
                if (sim != null)
                {
                    Perform(sim);
                }
                else
                {
                    Sims3.Gameplay.Objects.RabbitHoles.CityHall cityHall = Target as Sims3.Gameplay.Objects.RabbitHoles.CityHall;
                    if (cityHall != null)
                    {
                        foreach (Sim member in LotManager.Actors)
                        {
                            Perform(member);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<ResetWalk>
        {
            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("ResetWalk:MenuName");
            }

            public override bool Test(IActor a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                Sim sim = target as Sim;
                if (sim != null)
                {
                    return true;
                }
                else
                {
                    Sims3.Gameplay.Objects.RabbitHoles.CityHall cityHall = target as Sims3.Gameplay.Objects.RabbitHoles.CityHall;
                    return (cityHall != null);
                }
            }
        }
    }
}