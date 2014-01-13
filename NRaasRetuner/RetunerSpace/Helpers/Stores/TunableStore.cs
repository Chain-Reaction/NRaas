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
    [Persistable]
    public abstract class TunableStore : IPersistence
    {
        protected Type mParentType;

        protected string mFieldName;

        public TunableStore()
        { }
        public TunableStore(FieldInfo field)
        {
            mParentType = field.DeclaringType;
            mFieldName = field.Name;
        }
        public TunableStore(TunableStore store)
        {
            mParentType = store.mParentType;
            mFieldName = store.mFieldName;
        }

        public string PersistencePrefix
        {
            get { return null; }
        }

        public virtual bool Valid
        {
            get
            {
                if (mParentType == null) return false;

                if (mParentType.Assembly == null) return false;

                if (mFieldName == null) return false;

                return true;
            }
        }

        public string FieldName
        {
            get { return mFieldName; }
        }

        public Type ParentType
        {
            get { return mParentType; }
        }

        public FieldInfo GetFieldInfo()
        {
            return mParentType.GetField(mFieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public virtual void Import(Persistence.Lookup settings)
        {
            mParentType = settings.GetType("ParentType");
            mFieldName = settings.GetString("FieldName");
        }

        public virtual void Export(Persistence.Lookup settings)
        {
            settings.Add("ParentType", mParentType);
            settings.Add("FieldName", mFieldName);
        }

        public object GetValue(bool stored)
        {
            return GetValue(null, stored);
        }

        public abstract object GetValue(object parent, bool stored);

        public void Apply(SettingsKey key)
        {
            try
            {
                if (Retuner.StoreDefault(key, Clone().SetToDefault(null)))
                {
                    PrivateApply(null);
                }
            }
            catch (Exception e)
            {
                Common.Exception(ToString(), e);
            }
        }

        public abstract void PrivateApply(object parent);

        public abstract TunableStore SetToDefault(object parent);

        public abstract string ToXMLString(SettingsKey key);

        public virtual bool Parse(XmlDbRow row)
        {
            mParentType = row.GetClassType("FullClassName");
            if (mParentType == null)
            {
                BooterLogger.AddError("Unknown FullClassName: " + row.GetString("FullClassName"));
                return false;
            }

            mFieldName = row.GetString("FieldName");
            if (string.IsNullOrEmpty(mFieldName))
            {
                BooterLogger.AddError("FieldName missing for " + mParentType.ToString());
                return false;
            }

            return true;
        }

        public virtual bool IsEqual(TunableStore store)
        {
            if (mParentType != store.mParentType) return false;

            if (mFieldName != store.mFieldName) return false;

            return true;
        }

        protected string GetXMLFieldName(int depth)
        {
            if (depth == 1)
            {
                return Common.NewLine + "    <SubFieldName>" + mFieldName + "</SubFieldName>";
            }
            else
            {
                return Common.NewLine + "    <SubFieldName" + depth + ">" + mFieldName + "</SubFieldName" + depth + ">";
            }
        }

        public string StartXML(SettingsKey key)
        {
            string result = null;

            string assemblyName = mParentType.Assembly.FullName;
            if (!string.IsNullOrEmpty(assemblyName))
            {
                assemblyName = assemblyName.Split(',')[0];
            }

            result += "  <XML>";
            result += Common.NewLine + "    <FullClassName>" + mParentType.FullName + "," + assemblyName + "</FullClassName>";
            result += Common.NewLine + "    <FieldName>" + mFieldName + "</FieldName>";

            if (key != null)
            {
                result += key.ToXMLString();
            }

            return result;
        }

        public virtual string NestedXML(int depth)
        {
            return null;
        }

        public string EndXML()
        {
            return Common.NewLine + "  </XML>";
        }

        public abstract TunableStore Clone();

        public override string ToString()
        {
            return ToXMLString(null);
        }
    }
}
