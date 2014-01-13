using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupSituationInteractions : DelayedLoadupOption
    {
        public static bool TestCallback(Delegate callback, string text)
        {
            if (callback == null) return false;

            if (callback.Target == null) return false;

            Situation situation = callback.Target as Situation;
            while (situation != null)
            {
                if (situation.IsRoot()) break;

                situation = situation.GenericParent;
            }

            if (situation != null)
            {
                if (!Situation.sAllSituations.Contains(situation)) return true;
            }

            return false;
        }

        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupSituationInteractions");

            foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
            {
                if (sim.InteractionQueue == null) continue;

                bool interactionChanged = false;

                foreach (InteractionInstance interaction in sim.InteractionQueue.InteractionList)
                {
                    IopWithCallbacks pair = interaction.InteractionObjectPair as IopWithCallbacks;
                    if (pair == null) continue;

                    bool found = false;
                    if (TestCallback(pair.mCallbackOnStart, "OnStart"))
                    {
                        found = true;
                    }
                    else if (TestCallback(pair.mCallbackOnFailure, "OnFailure"))
                    {
                        found = true;
                    }
                    else if (TestCallback(pair.mCallbackOnCompletion, "OnCompletion"))
                    {
                        found = true;
                    }

                    if (found)
                    {
                        Overwatch.Log(sim.FullName + " " + interaction.ToString() + " Bogus Situation Interaction Dropped");

                        pair.mCallbackOnStart = null;
                        pair.mCallbackOnFailure = null;
                        pair.mCallbackOnCompletion = null;

                        interactionChanged = true;
                    }
                }

                if (interactionChanged)
                {
                    sim.InteractionQueue.CancelAllInteractions();
                }
            }
        }
    }
}
