using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupInvalidEarlyDepatures : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            Overwatch.Log("CleanupInvalidEarlyDepatures");

            // fix up corrupt early depatures
            if (GameStates.sTravelData != null && GameStates.sTravelData.mEarlyDepartures != null)
            {
                List<ulong> mSims = new List<ulong>();
                foreach (Sim sim in new List<Sim>(GameStates.sTravelData.mEarlyDepartures))
                {
                    if (sim == null || sim.SimDescription == null)
                    {
                        GameStates.sTravelData.mEarlyDepartures.Remove(sim);
                        Overwatch.Log("Corrupt early depature dropped");
                    }
                    else
                    {
                        mSims.Add(sim.SimDescription.SimDescriptionId);
                    }
                }

                GameStates.sTravelData.mEarlyDepartureIds = mSims;
            }
        }
    }
}