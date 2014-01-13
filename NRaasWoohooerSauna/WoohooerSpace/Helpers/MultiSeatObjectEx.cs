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
    public class MultiSeatObjectEx
    {
        public static bool StateMachineEnterAndSit(MultiSeatObject ths, StateMachineClient smc, SittingPosture sitPosture, Slot routingSlot, object sitContext)
        {
            if (!SittableComponentEx.StateMachineEnterAndSit(ths.Sittable, smc, sitPosture, routingSlot, sitContext))
            {
                return false;
            }

            SittableComponent.SitContext context = sitContext as SittableComponent.SitContext;
            if (((context != null) && (context.PreferredSeat != null)) && (context.PreferredSeat.ContainedSim == null))
            {
                Scoot entry = (Scoot)Scoot.Singleton.CreateInstance(sitPosture.Container, sitPosture.Sim, sitPosture.Sim.InteractionQueue.GetHeadInteraction().GetPriority(), false, true);
                entry.TargetSeat = context.PreferredSeat as Seat;
                if (entry.TargetSeat != null)
                {
                    sitPosture.Sim.InteractionQueue.AddNext(entry);
                }
            }

            if ((ths.SculptureComponent != null) && (ths.SculptureComponent.Material == SculptureComponent.SculptureMaterial.Ice))
            {
                sitPosture.Sim.BuffManager.AddElementPaused(BuffNames.Chilly, Origin.FromSittingOnIce);
            }
            return true;
        }
    }
}


