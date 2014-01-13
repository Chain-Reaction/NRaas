using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.RetunerSpace.Helpers.Stores;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RetunerSpace.Helpers.FieldInfos
{
    public class TunableFieldInfo
    {
        TunableFieldInfo mParentInfo;

        FieldInfo mField;

        int mDepth;

        public TunableFieldInfo()
        { }
        public TunableFieldInfo(FieldInfo field, TunableFieldInfo parentInfo)
        {
            mField = field;
            mParentInfo = parentInfo;
            if (mParentInfo != null)
            {
                mDepth = mParentInfo.mDepth + 1;
            }
        }

        public virtual string Name
        {
            get { return mField.Name; }
        }

        public FieldInfo Field
        {
            get { return mField; }
        }

        public virtual Type ElementalType
        {
            get { return Field.FieldType; }
        }

        public int Depth
        {
            get { return mDepth; }
        }

        public object GetValue(bool pure)
        {
            if (!pure)
            {
                TunableStore value = Retuner.SeasonSettings.GetTunable(GetParentStore(new ValueStore(mField, null)));
                if (value != null)
                {
                    return value.GetValue(true);
                }
            }

            return PrivateGetValue();
        }

        protected virtual object PrivateGetValue()
        {
            if (mParentInfo != null)
            {
                return Field.GetValue(mParentInfo.GetValue(true));
            }
            else
            {
                return Field.GetValue(null);
            }
        }

        public virtual TunableStore GetParentStore(TunableStore child)
        {
            if (mParentInfo != null)
            {
                return mParentInfo.GetParentStore(new NestedStore(mParentInfo.Field, child));
            }
            else
            {
                return child;
            }
        }

        public void SetValue(SettingsKey key, object value)
        {
            TunableStore store = GetParentStore(new ValueStore(mField, value));
            store.Apply(key);

            Retuner.SeasonSettings.AddTunable(store, true);
        }

        public TunableFieldInfo ParentInfo
        {
            get
            {
                return mParentInfo;
            }
        }

        public string ToXMLString()
        {
            return GetParentStore(new ValueStore(mField, GetValue(true))).ToString();
        }

        public override string ToString()
        {
            string result = null;

            if (mParentInfo != null)
            {
                result += mParentInfo.ToString();
            }

            result += ":" + mField.ToString();

            return result;
        }
    }
}
