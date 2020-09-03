using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace
{
    public interface ITestableOption
    {
        string OptionName { get; }
        string OptionValue { get; }
        bool Test(IMiniSimDescription me, bool fullFamily, IMiniSimDescription actor);
        bool Test(IMiniSimDescription me, bool fullFamily, IMiniSimDescription actor, bool testRandom);        
    }

    public abstract class TestableOption<TDataType, TStoreType> : ValueSettingOption<TStoreType>, IPersistence, ITestableOption, IScorableOption
    {
        public TestableOption()
        { }
        public TestableOption(TStoreType value, string name, int count)
            : base(value, name, count)
        { }
        public TestableOption(TStoreType value, string name, int count, ResourceKey key)
            : base(value, name, count, key)
        { }
        public TestableOption(TStoreType value, string name, int count, ThumbnailKey key)
            : base(value, name, count, key)
        { }

        public OptionScoreData mOptionScoreData;

        public string OptionName
        {
            get { return mName; }
        }

        public string OptionValue
        {
            get { return Value.ToString(); }
        }

        public OptionScoreData OptionScoreData
        {
            get
            {
                if (mOptionScoreData == null)
                {
                    mOptionScoreData = new OptionScoreData();
                }

                return mOptionScoreData;
            }
            set
            {
                mOptionScoreData = value;
            }
        }

        public bool CanBeRandomValue
        {
            get { return OptionScoreData.CanBeRandomValue; }
            set { OptionScoreData.CanBeRandomValue = value; }
        }

        public int MinHitValue
        {
            get { return OptionScoreData.MinHitValue; }
            set { OptionScoreData.MinHitValue = value; }
        }

        public int MaxHitValue
        {
            get { return OptionScoreData.MaxHitValue; }
            set { OptionScoreData.MaxHitValue = value; }
        }

        public int MinMissValue
        {
            get { return OptionScoreData.MinMissValue; }
            set { OptionScoreData.MinMissValue = value; }
        }

        public int MaxMissValue
        {
            get { return OptionScoreData.MaxMissValue; }
            set { OptionScoreData.MaxMissValue = value; }
        }

        public int OptionHitValue
        {
            get { return OptionScoreData.OptionHitValue; }
            set { OptionScoreData.OptionHitValue = value; }
        }

        public int OptionMissValue
        {
            get { return OptionScoreData.OptionMissValue; }
            set { OptionScoreData.OptionMissValue = value; }
        }

        public Dictionary<int, float> ChanceAtOptionLevel
        {
            get { return OptionScoreData.ChanceAtOptionLevel; }
            set { OptionScoreData.ChanceAtOptionLevel = value; }
        }

        public abstract bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<TStoreType, TDataType> results);

        public virtual bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<TStoreType, TDataType> results)
        {
            return false;
        }

        public abstract void SetValue(TDataType dataType, TStoreType storeType);

        public int GetScoreValue(IMiniSimDescription me, IMiniSimDescription actor, bool satisfies, int missDivisor)
        {
            if (Test(me, false, actor, false) && satisfies)
            {
                return OptionScoreData.mOptionHitValue;
            }
            else
            {
                if (OptionScoreData.mOptionMissValue != 0 || !satisfies || missDivisor == 0)
                {
                    return OptionScoreData.mOptionMissValue;
                }

                /*
                int val = OptionScoreData.mOptionHitValue / missDivisor;

                if (val > 0)
                {
                    return -1 * val;
                }
                else
                {
                    return val;
                }
                 */

                return 0;
            }
        }

        public virtual bool Test(IMiniSimDescription me, bool fullFamily, IMiniSimDescription actor)
        {
            return Test(me, fullFamily, actor, false);
        }

        public virtual bool Test(IMiniSimDescription me, bool fullFamily, IMiniSimDescription actor, bool testRandom)
        {
            Dictionary<TStoreType, TDataType> results = new Dictionary<TStoreType, TDataType>();

            SimDescription trueSim = me as SimDescription;
            if (trueSim != null)
            {
                if ((fullFamily) && (trueSim.Household != null))
                {
                    foreach (SimDescription member in CommonSpace.Helpers.Households.All(trueSim.Household))
                    {
                        Get(member, actor, results);
                    }
                }
                else
                {
                    if (!Get(trueSim, actor, results)) return false;
                }
            }
            else
            {
                MiniSimDescription miniSim = me as MiniSimDescription;
                if (miniSim != null)
                {
                    if (!Get(miniSim, actor, results)) return false;
                }
            }

            if (testRandom && OptionScoreData.mCanBeRandomValue)
            {
                if (RandomUtil.CoinFlip())
                {
                    return results.ContainsKey(Value);
                }
                else
                {
                    return false;
                }
                // don't have to worry about new criteria being added to a filter... if it is we can flush Sims matching that filter
                // once filter with random criteria has been applied it won't be altered on the sim or removed unless the random criteria applied is no longer valid
                // if random criteria is applied, create new filter with it so it can be tested
            }

            return results.ContainsKey(Value);
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.ReflectionAdd(this);
        }

        public void Import(Persistence.Lookup settings)
        {
            settings.ReflectionGet(this);
        }

        public string PersistencePrefix
        {
            get { return null; }
        }

        public override string DisplayKey
        {
            get { return "Has"; }
        }
    }
}
