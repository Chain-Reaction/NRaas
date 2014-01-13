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
        bool Test(IMiniSimDescription me, bool fullFamily, IMiniSimDescription actor);
    }

    public abstract class TestableOption<TDataType, TStoreType> : ValueSettingOption<TStoreType>, IPersistence, ITestableOption
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

        public abstract bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<TStoreType, TDataType> results);

        public virtual bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<TStoreType, TDataType> results)
        {
            return false;
        }

        public abstract void SetValue(TDataType dataType, TStoreType storeType);

        public virtual bool Test(IMiniSimDescription me, bool fullFamily, IMiniSimDescription actor)
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
