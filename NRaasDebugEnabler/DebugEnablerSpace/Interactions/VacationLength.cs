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
    public class VacationLength : DebugEnablerInteraction<GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if (Common.IsRootMenuObject(obj))
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        public override bool Run()
        {
            try
            {
                string text = StringInputDialog.Show(Common.Localize("VacationLength:MenuName"), Common.Localize("VacationLength:Prompt"), GameStates.sTravelData.mTripLength.ToString());
                if (string.IsNullOrEmpty(text)) return false;

                int value;
                if (!int.TryParse(text, out value)) return false;

                GameStates.sTravelData.mTripLength = value;
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<VacationLength>
        {
            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("VacationLength:MenuName");
            }

            public override bool Test(IActor a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                if (GameStates.sTravelData == null) return false;

                return true;
            }
        }
    }
}