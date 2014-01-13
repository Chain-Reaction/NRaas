using NRaas.CommonSpace.Helpers;
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
    public class CleanupTraits : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupTraits");

            foreach (List<SimDescription> sims in SimListing.AllSims<SimDescription>(null, true).Values)
            {
                foreach (SimDescription sim in sims)
                {
                    if (sim.TraitManager == null) continue;

                    List<ulong> remove = new List<ulong>();

                    foreach (KeyValuePair<ulong, Trait> trait in sim.TraitManager.mValues)
                    {
                        if (TraitManager.sDictionary.ContainsKey(trait.Key)) continue;

                        remove.Add(trait.Key);
                    }

                    foreach (ulong trait in remove)
                    {
                        sim.TraitManager.mValues.Remove(trait);

                        Overwatch.Log(" Removed: " + sim.FullName);
                    }
                }
            }
        }
    }
}
