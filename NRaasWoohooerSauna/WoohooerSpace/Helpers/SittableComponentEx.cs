using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Interactions;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.InteractionsShared;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Situations;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Helpers
{
    public class SittableComponentEx
    {
        public static bool StateMachineEnterAndSit(SittableComponent ths, StateMachineClient smc, SittingPosture sitPosture, Slot routingSlot, object sitContext)
        {
            if ((smc == null) || (sitPosture == null))
            {
                return false;
            }

            SitData target = sitPosture.Part.Target;
            bool paramValue = (ths.Owner.BoobyTrapComponent != null) ? ths.Owner.BoobyTrapComponent.CanTriggerTrap(sitPosture.Sim.SimDescription) : false;
            smc.SetParameter("isBoobyTrapped", paramValue);
            smc.SetParameter("sitTemplateSuffix", target.IKSuffix);
            smc.EnterState("x", ths.GetEnterStateName(routingSlot));
            smc.RequestState("x", ths.GetSitStateName());

            if (paramValue)
            {
                (ths.Owner as IBoobyTrap).TriggerTrap(sitPosture.Sim);
                smc.SetParameter("isBoobyTrapped", false);
            }

            ths.TurnOnFootDiscouragmentArea(target);
            return true;
        }
    }
}


