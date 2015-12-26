using NRaas.CommonSpace;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using Sims3.UI.CAS;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public class FilterHelper : Common.IWorldLoadFinished
    {
        static Common.MethodStore sGetFilters = new Common.MethodStore("NRaasMasterController", "NRaas.MasterController", "GetAllFilters", new Type[] { typeof(bool) });
        static Common.MethodStore sGetFiltersAsCriteria = new Common.MethodStore("NRaasMasterController", "NRaas.MasterController", "GetAllFiltersAsCriteria", new Type[] { typeof(bool) });
        static Common.MethodStore sGetSimsMatchingFilter = new Common.MethodStore("NRaasMasterController", "NRaas.MasterController", "GetSimsMatchingFilter", new Type[] { typeof(List<object>) });
        static Common.MethodStore sGetSingleFilter = new Common.MethodStore("NRaasMasterController", "NRaas.MasterController", "GetSingleFilter", new Type[] { typeof(string) });
        static Common.MethodStore sSpawnCreateFilterDialog = new Common.MethodStore("NRaasMasterController", "NRaas.MasterController", "CreateFilter", new Type[] { typeof(string) });
        static Common.MethodStore sSpawnDeleteFilterDialog = new Common.MethodStore("NRaasMasterController", "NRaas.MasterController", "DeleteFilter", new Type[] { typeof(string) });

        [Tunable, TunableComment("How long a filter should have it's results cached (in Sim minutes)")]
        public static int kFilterCacheTime = 120;

        [PersistableStatic(false)]
        public static Dictionary<string, Dictionary<ulong, SimFilter>> filters = new Dictionary<string, Dictionary<ulong, SimFilter>>();

        public static SimFilter GetFilter(string filter)
        {
            return GetFilter(filter, 0, false);
        }

        public static SimFilter GetFilter(string filter, ulong cacheSim)
        {
            return GetFilter(filter, cacheSim, false);
        }

        public static SimFilter GetFilter(string filter, ulong cacheSim, bool forceLive)
        {
            if (!filters.ContainsKey(filter))
            {
                Common.DebugNotify("Doesn't contain " + filter);
            }
            else
            {
                Common.DebugNotify("Cache contains " + filter);
                if (filters[filter].ContainsKey(cacheSim))
                {
                    Common.DebugNotify("Cache contains " + filter + " for sim " + cacheSim);

                    if (filters[filter][cacheSim].CacheValid || forceLive)
                    {
                        Common.DebugNotify("CacheValid" + (forceLive ? " because of force" : ""));
                    }
                    else
                    {
                        Common.DebugNotify("CacheInvalid");
                    }
                }
                else
                {
                    Common.DebugNotify("Doesn't contain " + filter + " for sim " + cacheSim);

                    if (filters[filter].ContainsKey(0))
                    {
                        Common.DebugNotify("returnDefaultIfNull: has 0");
                        if (filters[filter][0].CacheValid || forceLive)
                        {
                            Common.DebugNotify("CacheValid" + (forceLive ? " because of force" : ""));
                        }
                        else
                        {
                            Common.DebugNotify("CacheInvalid");
                        }
                    }
                }
            }

            if (filters.ContainsKey(filter))
            {
                if (filters[filter].ContainsKey(cacheSim))                
                {
                    if (filters[filter][cacheSim].CacheValid && !forceLive)
                    {
                        return filters[filter][cacheSim];
                    }
                }
                    // cache won't contain the default 0 key if they are Sim specific
                else if (filters[filter].ContainsKey(0) && (filters[filter][0].CacheValid && !forceLive))
                {
                    return filters[filter][0];
                }
            }

            return UpdateFilter(filter, cacheSim);
        }

        public static bool IsValidFilter(string filter)
        {
            return filters.ContainsKey(filter);
        }

        public static Dictionary<string, bool> GetFilters()
        {
            // this is always live because it should for the most part only be called by the UI
            Dictionary<string, bool> results = new Dictionary<string, bool>();
            if (sGetFilters.Valid)
            {
                results = sGetFilters.Invoke<Dictionary<string, bool>>(new object[] { true });
            }

            return results;
        }

        // an idea for another time
        public static Dictionary<string, ProtoSimSelection<IMiniSimDescription>.ICriteria> GetFiltersAsCriteria()
        {
            Dictionary<string, ProtoSimSelection<IMiniSimDescription>.ICriteria> results = new Dictionary<string, ProtoSimSelection<IMiniSimDescription>.ICriteria>();
            if (sGetFilters.Valid)
            {
                results = sGetFilters.Invoke<Dictionary<string, ProtoSimSelection<IMiniSimDescription>.ICriteria>>(new object[] { true });
            }

            return results;
        }

        public static OptionResult CreateFilter()
        {
            OptionResult result = OptionResult.Unset;
            if (sSpawnCreateFilterDialog.Valid)
            {
                // will never return a successful result due to dialog weirdness described in MasterController.cs
                string mNamespace = VersionStamp.sNamespace.ToLower().Replace(".", "");                
                result = sSpawnCreateFilterDialog.Invoke<OptionResult>(new object[] { mNamespace });
            }

            return result;
        }

        public static OptionResult DeleteFilter()
        {
            OptionResult result = OptionResult.Unset;
            if (sSpawnDeleteFilterDialog.Valid)
            {
                string mNamespace = VersionStamp.sNamespace.ToLower().Replace(".", "");
                result = sSpawnDeleteFilterDialog.Invoke<OptionResult>(new object[] { mNamespace });
            }

            if (result != OptionResult.Failure)
            {
                // delayed to let the user complete the exchange via the alarm on the other end
                new Common.AlarmTask(5, TimeUnit.Minutes, UpdateFilters); 
            }

            return result;
        }

        public static List<ulong> GetSimMatchingFilter(string filter)
        {
            return GetSimsMatchingFilter(filter, 0);
        }

        public static List<ulong> GetSimsMatchingFilter(string filter, ulong cacheSim)
        {
            Common.DebugNotify("GetSimsMatchingFilter: " + filter);
            List<ulong> results = new List<ulong>();

            if (sGetSimsMatchingFilter.Valid)
            {
                results = sGetSimsMatchingFilter.Invoke<List<ulong>>(new object[] { new List<object>() { filter, cacheSim } });
                Common.DebugNotify("GSMF Hot: " + results.Count);
            }            

            return results;
        }

        public static bool DoesSimMatchFilter(ulong simToMatch, string filter)
        {
            return DoesSimMatchFilter(simToMatch, 0, filter, false);
        }

        public static bool DoesSimMatchFilter(ulong simToMatch, ulong cacheSim, string filter)
        {
            return DoesSimMatchFilter(simToMatch, cacheSim, filter, false);
        }

        public static bool DoesSimMatchFilter(ulong simToMatch, ulong cacheSim, string filter, bool forceLive)
        {
            return GetFilter(filter, cacheSim, forceLive).SimMatches(simToMatch);
        }

        public static bool DoesSimMatchFilters(ulong simToMatch, List<string> filters)
        {
            return DoesSimMatchFilters(simToMatch, 0, filters, false); 
        }

        public static bool DoesSimMatchFilters(ulong simToMatch, ulong cacheSim, List<string> filters)
        {
            return DoesSimMatchFilters(simToMatch, cacheSim, filters, false);
        }

        public static bool DoesSimMatchFilters(ulong simToMatch, List<string> filters, bool matchAll)
        {
            return DoesSimMatchFilters(simToMatch, 0, filters, matchAll);
        }

        public static bool DoesSimMatchFilters(ulong simToMatch, ulong cacheSim, List<string> filters, bool matchAll)
        {
            return DoesSimMatchFilters(simToMatch, cacheSim, filters, matchAll, false);
        }

        public static bool DoesSimMatchFilters(ulong simToMatch, ulong cacheSim, List<string> filters, bool matchAll, bool forceLive)
        {
            bool match = false;
            foreach (string filter in filters)
            {
                match = DoesSimMatchFilter(simToMatch, cacheSim, filter);
                if (!match && matchAll)
                {
                    return match;
                }

                if (match && !matchAll)
                {
                    return match;
                }
            }

            return match;
        }

        public static bool FilterHasMatches(string filter)
        {
            return FilterHasMatches(filter, 0);
        }

        public static bool FilterHasMatches(string filter, ulong simId)
        {
            return GetFilter(filter, simId).FilterHasMatches();
        }

        public static bool DoesAnySimMatchFilter(List<SimDescription> descs, string filter)
        {
            return DoesAnySimMatchFilter(0, descs, filter);
        }

        public static bool DoesAnySimMatchFilter(ulong cacheSim, List<SimDescription> descs, string filter)
        {
            SimFilter cFilter = GetFilter(filter, cacheSim);
            foreach (SimDescription desc in descs)
            {
                if (cFilter.SimMatches(desc.SimDescriptionId))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool DoesAnySimMatchFilters(List<SimDescription> descs, List<string> filters, bool matchAll)
        {
            return DoesAnySimMatchFilters(0, descs, filters, matchAll);
        }

        public static bool DoesAnySimMatchFilters(ulong cacheSim, List<SimDescription> descs, List<string> filters, bool matchAll)
        {
            bool match = false;
            foreach (string filter in filters)
            {
                match = DoesAnySimMatchFilter(cacheSim, descs, filter);
                if (!match && matchAll)
                {
                    return match;
                }

                if (match && !matchAll)
                {
                    return match;
                }
            }

            return match;
        }

        // this one assumes the filter has already been initalized
        public static SimFilter UpdateFilter(string filter, ulong cacheSim)
        {
            Common.DebugNotify("UpdateFilterInternal: " + filter + " cacheSim: " + cacheSim);

            Dictionary<ulong, SimFilter> mCache;            
            filters.TryGetValue(filter, out mCache);

            if (mCache != null)
            {
                if (mCache.ContainsKey(0))
                {
                    Common.DebugNotify("mCache.ContainsKey 0");
                    return UpdateFilter(filter, cacheSim, false);
                }
                else
                {
                    Common.DebugNotify("mCache doesnt ContainsKey 0");
                    return UpdateFilter(filter, cacheSim, true);
                }
            }
            else
            {
                if (filter != string.Empty && sGetSingleFilter.Valid)
                {
                    Dictionary<string, bool> result = new Dictionary<string, bool>();
                    result = sGetSingleFilter.Invoke<Dictionary<string, bool>>(new object[] { filter });

                    if (result.Count > 0 && result.ContainsKey(filter))
                    {
                        Common.DebugNotify("GrabFilterHot: " + result[filter]);
                        return UpdateFilter(filter, cacheSim, result[filter]);
                    }
                }

                // return blank as fail safe
                SimFilter mFilter = new SimFilter(filter, 0);                
                return mFilter;
            }
        }

        // this one should only be called from UpdateFilters
        public static SimFilter UpdateFilter(string filter, ulong cacheSim, bool simSpecific)
        {
            Common.DebugNotify("Updating filter: " + filter + " SimSpecific: " + simSpecific + " CacheSim: " + cacheSim);
            if (simSpecific)
            {
                if (cacheSim != 0)
                {
                    return Cache(filter, cacheSim, GetSimsMatchingFilter(filter, cacheSim));
                }
            }
            else
            {
                return Cache(filter, GetSimsMatchingFilter(filter, cacheSim));
            }

            // invalid, will return a blank result
            return UpdateFilter("", 0);
        }

        public static void UpdateFilters()
        {
            FlushCache();

            Dictionary<string, bool> liveFilters = GetFilters();
            foreach (KeyValuePair<string, bool> filter in liveFilters)
            {
                UpdateFilter(filter.Key, 0, filter.Value);
            }
        }

        public static SimFilter Cache(string filter, List<ulong> sims)
        {
            return Cache(filter, 0, sims);
        }

        public static SimFilter Cache(string filter, ulong cacheSim, List<ulong> sims)
        {
            Common.DebugNotify("Cache: " + filter + " Count: " + sims.Count + " cacheSim: " + cacheSim);
            SimFilter cFilter = new SimFilter(filter, cacheSim);
            cFilter.Sims = sims;

            Dictionary<ulong, SimFilter> mCache;
            filters.TryGetValue(filter, out mCache);
            if (mCache != null)
            {
                mCache.Remove(cacheSim);
                mCache.Add(cacheSim, cFilter);
                filters[filter] = mCache;
            }
            else
            {
                mCache = new Dictionary<ulong, SimFilter>();
                mCache.Add(cacheSim, cFilter);
                filters.Add(filter, mCache);
            }
            
            return cFilter;
        }

        public static void FlushCache()
        {
            filters.Clear();
        }

        public static void FlushCache(string filter)
        {
            if (filters.ContainsKey(filter))
            {
                filters.Remove(filter);
            }

            if (PlumbBob.SelectedActor != null)
            {
                UpdateFilter(filter, PlumbBob.SelectedActor.SimDescription.SimDescriptionId);
            }
        }

        public static bool FiltersEnabled()
        {
            return sGetFilters.Valid;
        }

        public static void ValidateFilters()
        {
            if (filters.Count > 0)
            {
                Dictionary<string, bool> liveFilters = GetFilters();
                foreach (string filter in new List<string>(filters.Keys))
                {
                    if (!liveFilters.ContainsKey(filter))
                    {
                        filters.Remove(filter);
                    }
                    else
                    {
                        List<ulong> mRemove = new List<ulong>();
                        foreach (KeyValuePair<ulong, SimFilter> kvp in filters[filter])
                        {
                            if (MiniSimDescription.Find(kvp.Key) == null)
                            {
                                mRemove.Add(kvp.Key);
                            }
                            else
                            {
                                kvp.Value.Validate();
                            }
                        }

                        foreach (ulong k in mRemove)
                        {
                            filters[filter].Remove(k);
                        }
                    }
                }
            }
            else
            {
                UpdateFilters();
            }
        }

        public void OnWorldLoadFinished()
        {
            if (FiltersEnabled())
            {
                ValidateFilters();
            }
        }

        public static string StripNamespace(string filter)
        {            
            return filter.StartsWith("nraas") && filter.Contains(".") ? filter.Substring(filter.IndexOf('.') + 1) : filter;
        }

        [Persistable]
        public class SimFilter
        {
            public string name;
            private List<ulong> sims = new List<ulong>();
            private long lastUpdated = 0;
            public ulong cacheSim = 0;

            public SimFilter()
            {
            }

            public SimFilter(string filterName, ulong id)
            {
                name = filterName;
                cacheSim = id;
            }           

            public bool SimMatches(ulong id)
            {                
                return sims.Contains(id);
            }

            public bool FilterHasMatches()
            {
                return sims.Count > 0;
            }

            public List<ulong> Sims
            {
                get
                {
                    return sims;
                }
                set {
                    sims = value;
                    lastUpdated = SimClock.CurrentTicks;
                }
            }

            public bool CacheValid
            {
                get
                {
                    return ((lastUpdated + (SimClock.kSimulatorTicksPerSimMinute * kFilterCacheTime)) > SimClock.CurrentTicks);
                }
            }

            public void Validate()
            {
                foreach (ulong sim in sims)
                {
                    IMiniSimDescription desc = MiniSimDescription.Find(sim);
                    if (desc == null)
                    {
                        sims.Remove(sim);
                    }
                }
            }

            public string DisplayName
            {
                get
                {
                    return StripNamespace(name);
                }
            }
        }
    }
}
