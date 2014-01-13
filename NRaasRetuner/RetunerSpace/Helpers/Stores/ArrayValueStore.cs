using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.RetunerSpace.Helpers.FieldInfos;
using NRaas.RetunerSpace.Options.Tunable;
using NRaas.RetunerSpace.Options.Tunable.Fields;
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

namespace NRaas.RetunerSpace.Helpers.Stores
{
    public class ArrayValueStore : ValueStore
    {
        int mIndex;

        public ArrayValueStore()
        { }
        public ArrayValueStore(FieldInfo field, int index, object value)
            : base(field, value)
        {
            mIndex = index;
        }
        public ArrayValueStore(ArrayValueStore store)
            : base(store)
        {
            mIndex = store.mIndex;
        }

        public override bool Valid
        {
            get
            {
                if (mIndex < 0) return false;

                return base.Valid;
            }
        }

        public override void Import(Persistence.Lookup settings)
        {
            base.Import(settings);

            mIndex = settings.GetInt("Index", -1);
        }

        public override void Export(Persistence.Lookup settings)
        {
            base.Export(settings);

            settings.Add("Index", mIndex);
        }

        public override TunableStore SetToDefault(object parent)
        {
            FieldInfo field = GetFieldInfo();
            if (field == null) return null;

            Array array = field.GetValue(parent) as Array;
            if (array == null) return null;

            if (mIndex >= array.Length) return null;

            mValue = array.GetValue(mIndex);
            return this;
        }

        public override object GetValue(object parent, bool stored)
        {
            if (stored)
            {
                return base.GetValue(parent, stored);
            }
            else
            {
                FieldInfo field = GetFieldInfo();
                if (field == null) return null;

                Array array = field.GetValue(parent) as Array;
                if (array == null) return null;

                if (mIndex >= array.Length) return null;

                return array.GetValue(mIndex);
            }
        }

        public override void PrivateApply(object parent)
        {
            FieldInfo field = GetFieldInfo();
            if (field == null) return;

            Array array = field.GetValue(parent) as Array;
            if (array == null) return;

            if (mIndex >= array.Length) return;

            try
            {
                array.SetValue(mValue, mIndex);
            }
            catch (InvalidCastException e)
            {
                Common.Exception("Array: "+ array.GetType() + Common.NewLine + "Value: " + mValue.GetType(), e);
            }
        }

        public override bool Parse(XmlDbRow row)
        {
            mIndex = row.GetInt("Index", -1);
            if (mIndex < 0)
            {
                BooterLogger.AddError("Invalid Index for " + mParentType + "." + mFieldName);
                return false;
            }

            return base.Parse(row);
        }

        protected override TunableFieldInfo GetTunableFieldInfo()
        {
            return new ArrayFieldInfo(GetFieldInfo(), mIndex);
        }

        protected override Type GetParsingType(FieldInfo field)
        {
            return field.FieldType.GetElementType();
        }

        public override bool IsEqual(TunableStore o)
        {
            ArrayValueStore store = o as ArrayValueStore;
            if (store == null) return false;

            if (mIndex != store.mIndex) return false;

            return base.IsEqual(o);
        }

        public override string NestedXML(int depth)
        {
            string result = GetXMLFieldName(depth);
            result += Common.NewLine + "    <Index>" + mIndex + "</Index>";
            result += Common.NewLine + "    <Value>" + mValue + "</Value>";
            return result;
        }

        public override string ToXMLString(SettingsKey key)
        {
            string result = StartXML(key);
            result += Common.NewLine + "    <Index>" + mIndex + "</Index>";
            result += Common.NewLine + "    <Value>" + mValue.ToString() + "</Value>";
            result += EndXML();

            return result;
        }

        public override TunableStore Clone()
        {
            return new ArrayValueStore(this);
        }
    }
}
