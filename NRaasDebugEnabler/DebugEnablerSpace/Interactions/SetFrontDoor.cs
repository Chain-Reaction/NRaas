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
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class SetFrontDoor : Door.SetFrontDoor, IAddInteractionPair
    {
        public new static readonly InteractionDefinition Singleton = new Definition();

        public void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if (obj is Door)
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        [DoesntRequireTuning]
        private new sealed class Definition : ImmediateInteractionDefinition<Sim, Door, SetFrontDoor>
        {
            public override string[] GetPath(bool isFemale)
            {
                return new string[] { "Door . . . " };
            }

            public override string GetInteractionName(Sim a, Door target, InteractionObjectPair interaction)
            {
                return Common.LocalizeEAString("Gameplay/Abstracts/Door/SetFrontDoor:InteractionName");
            }

            public override bool Test(Sim a, Door target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (target.LotCurrent == null)
                {
                    return false;
                }
                switch (target.GetAdjoiningRoom(0x0, eRoomDefinition.LightBlocking))
                {
                    case 0x8fff:
                    case 0x0:
                        return false;
                }
                if (target.LotCurrent.GetFrontDoor(eFrontDoorType.kFrontDoorTypeOverride) == target)
                {
                    return false;
                }
                int roomConnectedToOutside = target.LotCurrent.GetRoomConnectedToOutside(target);
                if (target.LotCurrent.ValidateRoomForFrontDoor(roomConnectedToOutside, target.Level) == float.MaxValue)
                {
                    return false;
                }
                return true;
            }
        }
    }
}