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
using Sims3.Gameplay.EventSystem;
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
    public class CleanupRelationships : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupRelationships");

            Dictionary<ulong, List<SimDescription>> lookup = SimListing.AllSims<SimDescription>(null, true);

            List<SimDescription> removeA = new List<SimDescription>();

            foreach(KeyValuePair<SimDescription,Dictionary<SimDescription,Relationship>> relationsA in Relationship.sAllRelationships)
            {
                if (!lookup.ContainsKey(relationsA.Key.SimDescriptionId))
                {
                    removeA.Add(relationsA.Key);
                }
                else
                {
                    List<SimDescription> removeB = new List<SimDescription>();

                    foreach (KeyValuePair<SimDescription, Relationship> relationB in relationsA.Value)
                    {
                        if (!lookup.ContainsKey(relationB.Key.SimDescriptionId))
                        {
                            removeB.Add(relationB.Key);
                        }
                    }

                    foreach (SimDescription remove in removeB)
                    {
                        Relationship.sAllRelationships.Remove(remove);

                        Overwatch.Log(" Removed B: " + remove.FullName);
                    }
                }
            }

            foreach (SimDescription remove in removeA)
            {
                Relationship.sAllRelationships.Remove(remove);

                Overwatch.Log(" Removed A: " + remove.FullName);
            }
        }
    }
}
