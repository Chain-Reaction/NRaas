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
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.RabbitHoles;
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
    public class FixHomeworld : DebugEnablerInteraction<GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if ((obj is Sim) || (obj is RabbitHole))
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        public override bool Run()
        {
            try
            {
                int count = 0;

                Sim target = Target as Sim;
                if (target != null)
                {
                    if (target.LotHome != null)
                    {
                        if (target.SimDescription.HomeWorld != GameUtils.GetCurrentWorld())
                        {
                            target.SimDescription.mHomeWorld = GameUtils.GetCurrentWorld();
                            count++;
                        }
                    }
                }
                else
                {
                    foreach (SimDescription sim in Household.AllSimsLivingInWorld())
                    {
                        if (sim.LotHome == null) continue;

                        if (sim.HomeWorld == GameUtils.GetCurrentWorld()) continue;

                        sim.mHomeWorld = GameUtils.GetCurrentWorld();

                        count++;
                    }
                }

                Common.Notify(Common.Localize("FixHomeworld:Result", false, new object[] { count }));
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<FixHomeworld>
        {
            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("FixHomeworld:MenuName");
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { "Sim..." };
            }

            public override bool Test(IActor a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                if (target is RabbitHole) return true;

                Sim sim = target as Sim;
                if (sim == null) return false;

                return (sim.SimDescription.LotHome != null);
            }
        }
    }
}