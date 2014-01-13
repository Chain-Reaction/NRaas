using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class InteractionsEx
    {
        public static List<InteractionObjectPair> GetImmediateInteractions(GameObject obj)
        {
            List<InteractionObjectPair> immediateInteractions = new List<InteractionObjectPair>();

            foreach (InteractionObjectPair interaction in obj.Interactions)
            {
                if (interaction.InteractionDefinition is IImmediateInteractionDefinition)
                {
                    immediateInteractions.Add(interaction);
                }
            }

            return obj.BuildInteractions(Sim.ActiveActor, immediateInteractions);
        }

        public static bool HasInteraction<T>(SimDescription sim)
        {
            if (sim == null) return false;

            return HasInteraction<T>(sim.CreatedSim);
        }
        public static bool HasInteraction<T>(Sim sim)
        {
            if (sim == null) return false;

            InteractionQueue queue = sim.InteractionQueue;
            if (queue == null) return false;

            foreach (InteractionInstance instance in queue.mRunningInteractions)
            {
                if ((instance is T) || (instance.InteractionDefinition is T)) return true;
            }

            foreach (InteractionInstance instance2 in queue.mInteractionList)
            {
                if ((instance2 is T) || (instance2.InteractionDefinition is T)) return true;
            }

            return false;
        }
    }
}

