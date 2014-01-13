using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public class NetWorthOption : HouseIntegerOption, IReadHouseLevelOption, INotPersistableOption, IDebuggingOption, INotCasteLevelOption
    {
        static Dictionary<ulong, int> sCache = new Dictionary<ulong, int>();

        public NetWorthOption()
            : base(0)
        { }

        public override string GetTitlePrefix()
        {
            return "NetWorth";
        }

        public override int Value
        {
            get
            {
                if (Manager.House != null)
                {
                    int result;
                    if (sCache.TryGetValue(Manager.House.HouseholdId, out result))
                    {
                        return result;
                    }
                }

                return GetValue<AssetsOption, int>() - GetValue<DebtOption, int>();
            }
        }

        public class Initializer : Common.IWorldLoadFinished
        {
            public void OnWorldLoadFinished()
            {
                sCache.Clear();
            }
        }

        public class CacheValue : IDisposable
        {
            ulong mHouseID = 0;

            int mPreviousValue = -1;

            public CacheValue(Household house, int value)
            {
                if (house != null)
                {
                    mHouseID = house.HouseholdId;

                    if (!sCache.TryGetValue(mHouseID, out mPreviousValue))
                    {
                        mPreviousValue = -1;
                    }

                    sCache[mHouseID] = value;
                }
            }
            public CacheValue(GenericOptionBase manager, int value)
                : this(manager.House, value)
            { }
            public CacheValue(Manager manager, Household house)
                : this(house, manager.GetValue<NetWorthOption, int>(house))
            { }

            public void Dispose()
            {
                if (mHouseID != 0)
                {
                    if (mPreviousValue > 0)
                    {
                        sCache[mHouseID] = mPreviousValue;
                    }
                    else
                    {
                        sCache.Remove(mHouseID);
                    }
                }
            }
        }
    }
}

