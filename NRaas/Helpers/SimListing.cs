using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class SimListing
    {
        public static Dictionary<ulong, SimDescription> GetResidents(bool includeDead)
        {
            Dictionary<ulong, SimDescription> sims = new Dictionary<ulong, SimDescription>();

            foreach (SimDescription sim in Household.EverySimDescription())
            {
                sims[sim.SimDescriptionId] = sim;
            }

            if (includeDead)
            {
                foreach (SimDescription sim in SimDescription.GetHomelessSimDescriptionsFromUrnstones())
                {
                    sims[sim.SimDescriptionId] = sim;
                }
            }

            return sims;
        }

        public static Dictionary<ulong, List<SimDescription>> GetFullResidents(bool includeDead)
        {
            Dictionary<ulong, List<SimDescription>> results = new Dictionary<ulong, List<SimDescription>>();

            foreach (SimDescription sim in Household.EverySimDescription())
            {
                List<SimDescription> sims;
                if (!results.TryGetValue(sim.SimDescriptionId, out sims))
                {
                    sims = new List<SimDescription>();
                    results.Add(sim.SimDescriptionId, sims);
                }

                sims.Add(sim);
            }

            if (includeDead)
            {
                foreach (SimDescription sim in SimDescription.GetHomelessSimDescriptionsFromUrnstones())
                {
                    List<SimDescription> sims;
                    if (!results.TryGetValue(sim.SimDescriptionId, out sims))
                    {
                        sims = new List<SimDescription>();
                        results.Add(sim.SimDescriptionId, sims);
                    }

                    if (!sims.Contains(sim))
                    {
                        sims.Add(sim);
                    }
                }
            }

            return results;
        }

        public static bool AddSim<T>(T sim, Dictionary<ulong, List<T>> lookup)
            where T : class, IMiniSimDescription
        {
            if (sim == null) return false;

            List<T> list = GetSims(sim.SimDescriptionId, lookup);

            if (sim is SimDescription)
            {
                // Remove all MiniSims if we have the full description available
                list.RemoveAll((e) => { return e is MiniSimDescription; });
            }
            else
            {
                // If the full description is available, don't add miniSims
                if (list.Find((e) => { return e is SimDescription; }) != null) return false;
            }

            if (list.Find((e) => { return object.ReferenceEquals(e, sim); }) != null) return false;

            list.Add(sim);
            return true;
        }

        public static List<T> GetSims<T>(ulong simID, Dictionary<ulong, List<T>> lookup)
            where T : class, IMiniSimDescription
        {
            List<T> list;
            if (!lookup.TryGetValue(simID, out list))
            {
                list = new List<T>();
                lookup.Add(simID, list);
            }

            return list;
        }
        public static Dictionary<ulong, T> GetSims<T>(T me, bool includeAncestors)
            where T : class, IMiniSimDescription
        {
            Dictionary<ulong, List<T>> sims = AllSims<T>(me, includeAncestors);

            Dictionary<ulong, T> results = new Dictionary<ulong, T>();

            foreach (List<T> list in sims.Values)
            {
                if (list.Count == 0) continue;

                results.Add(list[0].SimDescriptionId, list[0]);
            }

            return results;
        }

        public static Dictionary<ulong, List<T>> AllSims<T>(T me, bool includeAncestors)
            where T : class, IMiniSimDescription
        {
            return AllSims<T>(me, includeAncestors, includeAncestors);
        }
        public static Dictionary<ulong, List<T>> AllSims<T>(T me, bool includeAncestors, bool includeDrivers)
            where T : class, IMiniSimDescription
        {
            Dictionary<ulong, List<T>> simsList = new Dictionary<ulong, List<T>>();

            Dictionary<ulong, List<SimDescription>> residents = GetFullResidents(true);

            foreach (Sim actualSim in LotManager.Actors)
            {
                SimDescription sim = actualSim.SimDescription;
                if (sim == null) continue;

                List<SimDescription> sims;
                if (!residents.TryGetValue(sim.SimDescriptionId, out sims))
                {
                    sims = new List<SimDescription>();
                    residents.Add(sim.SimDescriptionId, sims);

                    sims.Add(sim);
                }
            }

            foreach (KeyValuePair<ulong, List<SimDescription>> sims in residents)
            {
                foreach (SimDescription resident in sims.Value)
                {
                    T sim = resident as T;
                    if (sim == null) continue;

                    AddSim(sim, simsList);
                }
            }

            if (me != null)
            {
                // Add the immediate relations of the active sim to the list (for family tree adjustment purposes)
                // Add my parents
                Genealogy genealogy = me.CASGenealogy as Genealogy;
                if (genealogy != null)
                {
                    if (genealogy.Parents != null)
                    {
                        foreach (Genealogy element in genealogy.Parents)
                        {
                            AddSim(element.mSim as T, simsList);
                            AddSim(element.mMiniSim as T, simsList);
                        }
                    }

                    // Add my children
                    if (genealogy.Children != null)
                    {
                        foreach (Genealogy element in genealogy.Children)
                        {
                            AddSim(element.mSim as T, simsList);
                            AddSim(element.mMiniSim as T, simsList);
                        }
                    }

                    // Add my siblings
                    if (genealogy.Siblings != null)
                    {
                        foreach (Genealogy element in genealogy.Siblings)
                        {
                            AddSim(element.mSim as T, simsList);
                            AddSim(element.mMiniSim as T, simsList);
                        }
                    }
                }
            }

            if (includeAncestors)
            {
                GetGenealogy(simsList);
            }

            if (MiniSimDescription.sMiniSims != null)
            {
                foreach (MiniSimDescription miniSim in MiniSimDescription.sMiniSims.Values)
                {
                    T sim = miniSim as T;
                    if (sim == null) continue;

                    AddSim(sim, simsList);
                }
            }

            if ((includeDrivers) && (CarNpcManager.Singleton != null) && (CarNpcManager.Singleton.NpcDriversManager != null))
            {
                if (CarNpcManager.Singleton.NpcDriversManager.mDescPools != null)
                {
                    foreach (Stack<SimDescription> stack in CarNpcManager.Singleton.NpcDriversManager.mDescPools)
                    {
                        if (stack == null) continue;

                        foreach (SimDescription sim in stack)
                        {
                            AddSim(sim as T, simsList);
                        }
                    }
                }

                if (CarNpcManager.Singleton.NpcDriversManager.mNpcDrivers != null)
                {
                    foreach (Sim sim in CarNpcManager.Singleton.NpcDriversManager.mNpcDrivers.Keys)
                    {
                        AddSim(sim.SimDescription as T, simsList);
                    }
                }
            }

            return simsList;
        }

        protected static void GetGenealogy<T>(Dictionary<ulong, List<T>> simsList)
            where T : class, IMiniSimDescription
        {
            List<T> sims = new List<T>();

            foreach (List<T> list in simsList.Values)
            {
                sims.AddRange(list);
            }

            int index = 0;
            while (index < sims.Count)
            {
                T sim = sims[index];
                index++;

                Genealogy genealogy = sim.CASGenealogy as Genealogy;
                if (genealogy == null) continue;

                // Add my parents
                if (genealogy.Parents != null)
                {
                    foreach (Genealogy element in genealogy.Parents)
                    {
                        if (AddSim(element.mSim as T, simsList))
                        {
                            sims.Add(element.mSim as T);
                        }

                        if (AddSim(element.mMiniSim as T, simsList))
                        {
                            sims.Add(element.mMiniSim as T);
                        }
                    }
                }

                // Add my children
                if (genealogy.Children != null)
                {
                    foreach (Genealogy element in genealogy.Children)
                    {
                        if (AddSim(element.mSim as T, simsList))
                        {
                            sims.Add(element.mSim as T);
                        }

                        if (AddSim(element.mMiniSim as T, simsList))
                        {
                            sims.Add(element.mMiniSim as T);
                        }
                    }
                }

                // Add my siblings
                if (genealogy.Siblings != null)
                {
                    foreach (Genealogy element in genealogy.Siblings)
                    {
                        if (AddSim(element.mSim as T, simsList))
                        {
                            sims.Add(element.mSim as T);
                        }

                        if (AddSim(element.mMiniSim as T, simsList))
                        {
                            sims.Add(element.mMiniSim as T);
                        }
                    }
                }
            }
        }
    }
}

