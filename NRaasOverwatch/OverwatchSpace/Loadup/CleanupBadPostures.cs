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
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupBadPostures : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupBadPostures");

            foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim> ())
            {
                try
                {
                    int count = 0;

                    if (sim.Posture == null)
                    {
                        sim.Posture = null; // This will reset the posture to standing
                    }

                    Posture posture = sim.Posture;
                    while ((posture != null) && (count < 11))
                    {
                        count++;

                        posture = posture.PreviousPosture;
                    }

                    if (count > 10)
                    {
                        sim.Posture.PreviousPosture = null;

                        Overwatch.Log("Bogus PreviousPosture dropped " + sim.FullName);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);
                }
            }
        }
    }
}
