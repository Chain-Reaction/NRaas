using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
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
    public class NestedStore : TunableStore
    {
        TunableStore mChild;

        public NestedStore()
        { }
        public NestedStore(FieldInfo field, TunableStore child)
            : base(field)
        {
            mChild = child;
        }
        public NestedStore(NestedStore store)
            : base(store)
        {
            mChild = store.mChild.Clone();
        }

        public override bool Valid
        {
            get
            {
                if (mChild == null) return false;

                if (!mChild.Valid) return false;

                return base.Valid;
            }
        }

        public override void Import(Persistence.Lookup settings)
        {
            base.Import(settings);

            mChild = settings.GetChild<TunableStore>("Child");
        }

        public override void Export(Persistence.Lookup settings)
        {
            base.Export(settings);

            settings.AddChild("Child", mChild);
        }

        public override TunableStore SetToDefault(object parent)
        {
            FieldInfo field = GetFieldInfo();
            if (field == null) return null;

            object newParent = field.GetValue(parent);
            if (newParent == null) return null;

            mChild.SetToDefault(newParent);
            return this;
        }

        public override void PrivateApply(object parent)
        {
            FieldInfo field = GetFieldInfo();
            if (field == null) return;

            object newParent = field.GetValue(parent);
            if (newParent == null) return;

            mChild.PrivateApply(newParent);
        }

        public override object GetValue(object parent, bool stored)
        {
            FieldInfo field = GetFieldInfo();
            if (field == null) return null;

            object newParent = field.GetValue(parent);
            if (newParent == null) return null;

            return mChild.GetValue(newParent, stored);
        }

        public override bool Parse(XmlDbRow row)
        {
            if (!base.Parse(row)) return false;

            List<string> subFieldNames = new List<string>();

            int count = 1;
            while (true)
            {
                string suffix = null;
                if (count > 1)
                {
                    suffix = count.ToString();
                }

                string subFieldName = row.GetString("SubFieldName" + suffix);
                if (string.IsNullOrEmpty(subFieldName))
                {
                    break;
                }

                subFieldNames.Add(subFieldName);
                count++;
            }

            if (subFieldNames.Count == 0)
            {
                BooterLogger.AddError("SubFieldName missing for " + mParentType + "." + mFieldName);
                return false;
            }

            FieldInfo subField = GetFieldInfo();
            if (subField == null)
            {
                BooterLogger.AddError("Unknown Field for " + mParentType.ToString() + "." + mFieldName);
                return false;
            }

            NestedStore parentStore = this;

            for (int i = 0; i < subFieldNames.Count - 1; i++)
            {
                FieldInfo newField = subField.FieldType.GetField(subFieldNames[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (newField == null)
                {
                    BooterLogger.AddError("Unknown SubField for " + subField.FieldType.ToString() + "." + subFieldNames[i]);
                    return false;
                }

                NestedStore newStore = new NestedStore(newField, null);

                parentStore.mChild = newStore;

                subField = newField;
                parentStore = newStore;
            }

            string finalFieldName = subFieldNames[subFieldNames.Count - 1];

            FieldInfo finalField = subField.FieldType.GetField(finalFieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (finalField == null)
            {
                BooterLogger.AddError("Unknown SubField for " + subField.FieldType.ToString() + "." + finalFieldName);
                return false;
            }            

            ValueStore child = null;
            if (row.Exists("Index"))
            {
                int index = row.GetInt("Index", -1);
                if (index < 0)
                {
                    BooterLogger.AddError("Invalid Index for " + mParentType + "." + mFieldName);
                    return false;
                }

                child = new ArrayValueStore(finalField, index, null);
            }
            else
            {
                child = new ValueStore(finalField, null);
            }

            parentStore.mChild = child;

            return child.ParseValue(row);
        }

        public override bool IsEqual(TunableStore o)
        {
            NestedStore store = o as NestedStore;
            if (store == null) return false;

            if (!mChild.IsEqual(store.mChild)) return false;

            return base.IsEqual(o);
        }

        public override string NestedXML(int depth)
        {
            string result = GetXMLFieldName(depth);

            result += mChild.NestedXML(depth + 1);

            return result;
        }

        public override string ToXMLString(SettingsKey key)
        {
            string result = StartXML(key);
            result += mChild.NestedXML(1);
            result += EndXML();

            return result;
        }

        public override TunableStore Clone()
        {
            return new NestedStore(this);
        }
    }
}
