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
    public class ValueStore : TunableStore
    {
        protected object mValue;

        public ValueStore()
        { }
        public ValueStore(FieldInfo field, object value)
            : base(field)
        {
            mValue = value;
        }
        public ValueStore(ValueStore store)
            : base(store)
        {
            mValue = store.mValue;
        }

        public object Value
        {
            get
            {
                return mValue;
            }
        }

        public override bool Valid
        {
            get
            {
                if (mValue == null) return false;

                return base.Valid;
            }
        }

        public override void Import(Persistence.Lookup settings)
        {
            base.Import(settings);

            FieldInfo field = GetFieldInfo();
            if (field != null)
            {
                ITunableConvertOption converter = TunableTypeOption.GetFieldOption(GetParsingType(field));
                if (converter != null)
                {
                    mValue = converter.Clone(GetTunableFieldInfo ()).Convert(settings.GetString("Value"), false);
                }
            }
        }

        public override void Export(Persistence.Lookup settings)
        {
            base.Export(settings);

            if (mValue != null)
            {
                settings.Add("Value", mValue.ToString());
            }
        }

        public override object GetValue(object parent, bool stored)
        {
            if (stored)
            {
                return mValue;
            }
            else
            {
                FieldInfo field = GetFieldInfo();
                if (field == null) return null;

                return field.GetValue(parent);
            }
        }

        public override void PrivateApply(object parent)
        {
            FieldInfo field = GetFieldInfo();
            if (field == null) return;

            field.SetValue(parent, mValue);
        }

        public override TunableStore SetToDefault(object parent)
        {
            FieldInfo field = GetFieldInfo();
            if (field == null) return null;

            mValue = field.GetValue(parent);
            return this;
        }

        protected virtual Type GetParsingType(FieldInfo field)
        {
            return field.FieldType;
        }

        protected virtual TunableFieldInfo GetTunableFieldInfo()
        {
            return new TunableFieldInfo(GetFieldInfo(), null);
        }

        public bool ParseValue(XmlDbRow row)
        {
            if (!row.Exists("Value"))
            {
                BooterLogger.AddError("Value missing for " + mParentType + "." + mFieldName);
                return false;
            }

            FieldInfo field = GetFieldInfo();
            if (field == null)
            {
                BooterLogger.AddError("Unknown Field for " + mParentType + "." + mFieldName);
                return false;
            }

            ITunableConvertOption fieldOption = TunableTypeOption.GetFieldOption(GetParsingType(field));
            if (fieldOption != null)
            {
                try
                {
                    mValue = fieldOption.Clone(GetTunableFieldInfo()).Convert(row.GetString("Value"), true);
                    return true;
                }
                catch (Exception e)
                {
                    Common.Exception(mParentType + "." + mFieldName + Common.NewLine + row.GetString("Value"), e);
                    return false;
                }
            }
            else
            {
                BooterLogger.AddError("Unhandled Field type : " + field.FieldType + " (" + mParentType + "." + mFieldName + ")");
                return false;
            }
        }

        public override bool Parse(XmlDbRow row)
        {
            if (!base.Parse(row)) return false;

            return ParseValue(row);
        }

        public override bool IsEqual(TunableStore o)
        {
            ValueStore store = o as ValueStore;
            if (store == null) return false;

            // We do not compare value, only hierarchy
            //if (mValue != store.mValue) return false;

            return base.IsEqual(o);
        }

        public override string NestedXML(int depth)
        {
            string result = GetXMLFieldName(depth);
            result += Common.NewLine + "    <Value>" + mValue + "</Value>";
            return result;
        }

        public override string ToXMLString(SettingsKey key)
        {
            string result = StartXML(key);
            result += Common.NewLine + "    <Value>" + mValue + "</Value>";
            result += EndXML();

            return result;
        }

        public override TunableStore Clone()
        {
            return new ValueStore(this);
        }
    }
}
