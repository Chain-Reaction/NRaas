using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public class CleanseTheDead
    {
        public static void Retrieve(Dictionary<SimDescription, Pair<IMausoleum,Urnstone>> urnstones)
        {
            foreach (IMausoleum mausoleum in Sims3.Gameplay.Queries.GetObjects<IMausoleum>())
            {
                Retrieve(mausoleum, urnstones);
            }

            foreach (List<SimDescription> sims in SimListing.AllSims<SimDescription>(null, true).Values)
            {
                foreach (SimDescription sim in sims)
                {
                    if (urnstones.ContainsKey(sim)) continue;

                    if (!IsBuried(sim)) continue;

                    if (HasLiveFamily(sim)) continue;

                    urnstones.Add(sim, new Pair<IMausoleum, Urnstone>(null,null));
                }
            }
        }
        public static void Retrieve(IMausoleum mausoleum, Dictionary<SimDescription, Pair<IMausoleum, Urnstone>> urnstones)
        {
            List<Urnstone> allStones = Inventories.QuickFind<Urnstone>(mausoleum.Inventory);

            foreach (Urnstone stone in allStones)
            {
                SimDescription sim = stone.DeadSimsDescription;
                if ((stone.InWorld) && (!stone.InInventory)) continue;

                if (urnstones.ContainsKey(sim)) continue;

                if (HasLiveFamily(sim)) continue;

                urnstones.Add(sim, new Pair<IMausoleum, Urnstone>(mausoleum, stone));
            }
        }

        public static bool IsBuried(SimDescription sim)
        {
            if (sim.CreatedSim != null) return false;

            if (sim.Household != null) return false;

            return true;
        }

        protected static bool HasLiveFamily(SimDescription sim)
        {
            try
            {
                if (sim == null) return false;
                
                if (sim.Genealogy == null) return false;

                bool found = false;

                List<Genealogy> sims = new List<Genealogy>();

                sims.AddRange(sim.Genealogy.Parents);
                sims.AddRange(sim.Genealogy.Children);
                sims.AddRange(sim.Genealogy.Siblings);

                foreach (Genealogy relation in sims)
                {
                    IMiniSimDescription relationSim = relation.IMiniSimDescription;
                    if (relationSim == null) continue;

                    if ((relationSim.IsDead) && (!relationSim.IsPlayableGhost))
                    {
                        continue;
                    }

                    found = true;
                }

                return found;
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
                return true;
            }
        }

        public delegate void Logger(string value);

        public static bool Dispose(SimDescription sim, Logger log)
        {
            if (sim == null) return false;

            Genealogy oldGene = sim.Genealogy;

            if (sim.CelebrityManager == null)
            {
                // If missing, the Dispose() fires a script error
                sim.CelebrityManager = new Sims3.Gameplay.CelebritySystem.CelebrityManager();
            }

            try
            {
                if (sim.Genealogy == null)
                {
                    MiniSimDescription miniSim = MiniSimDescription.Find(sim.SimDescriptionId);
                    if (miniSim != null)
                    {
                        sim.mGenealogy = miniSim.Genealogy;
                    }

                    if (sim.Genealogy == null)
                    {
                        sim.mGenealogy = new Genealogy(sim);
                    }
                }

                Urnstone stone = Urnstones.FindGhostsGrave(sim);
                if (stone != null)
                {
                    Inventory inventory = Inventories.ParentInventory(stone);
                    if (inventory != null)
                    {
                        inventory.RemoveByForce(stone);
                    }

                    try
                    {
                        stone.Dispose();
                        stone.Destroy();
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, e);

                        if (inventory != null)
                        {
                            inventory.TryToAdd(stone, false);
                        }
                    }
                }
                else
                {
                    sim.Dispose(true, false, true);
                }

                if (log != null)
                {
                    log(" Disposed: " + sim.FullName);
                }

                return true;
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
                return false;
            }
            finally
            {
                // Reassign the genealogy, to compensate for an issue where the miniSim is reprocessed later
                sim.mGenealogy = oldGene;
            }
        }

        public static void Cleanse(IEnumerable<SimDescription> choices, Dictionary<SimDescription, Pair<IMausoleum, Urnstone>> urnstones, bool requireMausoleum, Logger log)
        {
            foreach (SimDescription sim in choices)
            {
                Pair<IMausoleum, Urnstone> stone;
                if (!urnstones.TryGetValue(sim, out stone)) continue;

                if (requireMausoleum)
                {
                    if (stone.First == null) continue;
                }

                if (sim != null)
                {
                    Relationship.RemoveSimDescriptionRelationships(sim);
                }

                Dispose(sim, log);
            }
        }
    }
}
