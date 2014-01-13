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
using Sims3.Gameplay.Objects;
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
    public class SetAlienDNAPercentage : DebugEnablerInteraction<Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if (obj is Sim)
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        public override bool Run()
        {
            try
            {
                string value = StringInputDialog.Show(Common.Localize("SetAlienDNAPercentage:MenuName"), Common.Localize("SetAlienDNAPercentage:Prompt"), Target.SimDescription.AlienDNAPercentage.ToString());

                float percent = 0;
                if (!float.TryParse(value, out percent))
                {
                    Common.Notify(Common.Localize("Numeric:Error", false, new object[] { value }));
                    return false;
                }

                Target.SimDescription.mAlienDNAPercentage = percent;
            
                Target.Motives.RecreateMotives(Target);
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<SetAlienDNAPercentage>
        {
            public override string GetInteractionName(IActor a, Sim target, InteractionObjectPair interaction)
            {
                return Common.Localize("SetAlienDNAPercentage:MenuName");
            }
        }
    }
}