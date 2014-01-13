using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Metadata;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupKnownCompositions : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupKnownCompositions");

            foreach(SimDescription sim in Household.EverySimDescription())
            {
                if (sim.Singing == null) continue;

                if (sim.Singing.mKnownCompositions == null)
                {
                    sim.Singing.mKnownCompositions = new List<SingingInfo.SingingComposition>();
                }
                else
                {
                    for(int i=sim.Singing.mKnownCompositions.Count-1; i>=0; i--)
                    {
                        if (sim.Singing.mKnownCompositions[i] == null)
                        {
                            sim.Singing.mKnownCompositions.RemoveAt (i);

                            Overwatch.Log(" Removed Null mKnownComposition");
                        }
                    }

                    for (int i = sim.Singing.mKnownRomanticCompositions.Count - 1; i >= 0; i--)
                    {
                        if (sim.Singing.mKnownRomanticCompositions[i] == null)
                        {
                            sim.Singing.mKnownRomanticCompositions.RemoveAt(i);

                            Overwatch.Log(" Removed Null mKnownRomanticCompositions");
                        }
                    }
                }
            }
        }
    }
}
