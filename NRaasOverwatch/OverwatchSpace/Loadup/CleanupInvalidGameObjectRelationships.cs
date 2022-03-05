using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupInvalidGameobjectRelationships : ImmediateLoadupOption
    {
        public override string GetTitlePrefix()
        {
            return "CleanupInvalidGameobjectRelationships";
        }

        public override void OnWorldLoadFinished()
        {
            Overwatch.Log(GetTitlePrefix());

            bool removed = false;
            List<SimDescription> removedList = new List<SimDescription>();
            foreach (KeyValuePair<ulong, SimDescription> pair in SimListing.GetSims<SimDescription>(null, true))
            {
                if (pair.Value.GameObjectRelationships != null)
                {
                    bool rerun = false;
                    for (int index = pair.Value.GameObjectRelationships.Count - 1; index >= 0; index--)
                    {
                        if (pair.Value.GameObjectRelationships[index].GameObjectDescription == null)
                        {
                            pair.Value.GameObjectRelationships.RemoveAt(index);
                            removed = true;
                            removedList.Add(pair.Value);
                        }
                        else
                        {
                            if (pair.Value.GameObjectRelationships[index].GameObjectDescription.GameObject == null)
                            {
                                pair.Value.GameObjectRelationships.RemoveAt(index);
                                removed = true;
                                removedList.Add(pair.Value);
                                rerun = true;
                            }
                        }
                    }

                    if(rerun)
                    {
                        // This data being invalid causes EA's SimDescription:Fixup to fail on game load so rerun it
                        pair.Value.Fixup();
                    }
                }
            }

            if(removed)
            {
                Overwatch.Log("Removed invalid game relationships:");

                foreach(SimDescription desc in removedList)
                {
                    Overwatch.Log(desc.FullName);
                }

                removedList.Clear();
            }
        }
    }
}
