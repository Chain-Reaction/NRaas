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
    public class SetValue : DebugEnablerInteraction<GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if (obj is Lot) return;

            if (obj is Sim) return;

            if (obj is RabbitHole) return;

            if (!obj.CanBeSoldBase()) return;

            list.Add(new InteractionObjectPair(Singleton, obj));
        }

        public override bool Run()
        {
            try
            {
                int oldValue = Target.Value;

                string text = StringInputDialog.Show(Common.Localize("SetValue:MenuName"), Common.Localize("SetValue:Prompt", false, new object[] { Target.CatalogName }), oldValue.ToString());
                if (string.IsNullOrEmpty(text)) return false;

                int newValue = 0;
                if (!int.TryParse(text, out newValue))
                {
                    SimpleMessageDialog.Show(Common.Localize("SetValue:MenuName"), Common.Localize("Numeric:Error"));
                    return false;
                }

                int diff = newValue - oldValue;

                SculptureComponent component = Target.SculptureComponent;
                if (component != null)
                {
                    component.mAppreciationAmount += diff;
                }
                else
                {
                    Target.mValueModifier += diff;
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<SetValue>
        {
            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("SetValue:MenuName");
            }

            public override bool Test(IActor a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }
    }
}