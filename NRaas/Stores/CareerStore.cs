using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Stores
{
    public class CareerStore : IDisposable
    {
        Dictionary<ulong, SafeStore> mSafeStore;

        static Dictionary<ulong, SafeStore> sSafeStore = null;

        public static void StoreAll()
        {
            if (Household.ActiveHousehold == null) return;

            sSafeStore = new Dictionary<ulong, SafeStore>();

            foreach (SimDescription sim in Households.All(Household.ActiveHousehold))
            {
                sSafeStore[sim.SimDescriptionId] = new SafeStore(sim, SafeStore.Flag.OnlyAcademic | SafeStore.Flag.LoadFixup | SafeStore.Flag.Selectable | SafeStore.Flag.Unselectable);
            }
        }

        public static void RestoreAll()
        {
            if (sSafeStore == null) return;

            foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
            {
                SafeStore store;
                if (!sSafeStore.TryGetValue(sim.SimDescription.SimDescriptionId, out store)) continue;

                store.Dispose();
            }

            sSafeStore = null;
        }

        public static void OnWorldLoadFinished()
        {
            StoreAll();

            new Common.AlarmTask(1f, TimeUnit.Seconds, RestoreAll);
        }

        public CareerStore()
        {
            StoreAll();
        }
        public CareerStore(Household house, SafeStore.Flag flags)
        {
            mSafeStore = new Dictionary<ulong, SafeStore>();

            foreach (SimDescription sim in Households.All(house))
            {
                mSafeStore[sim.SimDescriptionId] = new SafeStore(sim, flags);
            }
        }

        public void Dispose()
        {
            if (mSafeStore != null)
            {
                foreach (SafeStore store in mSafeStore.Values)
                {
                    store.Dispose();
                }

                mSafeStore = null;
            }
            else
            {
                RestoreAll();
            }
        }
    }
}