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
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
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
    public abstract class DebugEnablerInteraction<OBJ> : BaseInteraction<OBJ>, IAddInteractionPair
        where OBJ : class, IGameObject
    {
        public override void AddInteraction(Common.InteractionInjectorList interactions)
        { }

        public abstract void AddPair(GameObject obj, List<InteractionObjectPair> list);

        [DoesntRequireTuning]
        public abstract class DebugEnablerDefinition<INTERACTION> : BaseDefinition<INTERACTION>
            where INTERACTION : DebugEnablerInteraction<OBJ>, new()
        {
            public override bool Test(IActor a, OBJ target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous) return false;

                return DebugEnabler.Settings.mEnabled;
            }
        }
    }
}