using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.CommonSpace.Helpers
{
    public interface IPersistence
    {
        string PersistencePrefix
        {
            get;
        }

        void Import(Persistence.Lookup settings);

        void Export(Persistence.Lookup settings);
    }

    public abstract class Persistence : IPersistence
    {
        public abstract void Import(Lookup settings);

        public abstract void Export(Lookup settings);

        public abstract string PersistencePrefix
        {
            get;
        }

        // Exported to Overwatch
        public static bool ImportSettings(XmlElement element)
        {
            Lookup settings = new Lookup(Logger.sLogger);

            string nameSpace = VersionStamp.sNamespace.Replace("NRaas.", "");

            if (!ParseSection(element, new string[] { nameSpace, "Settings" }, settings)) return false;

            foreach (IPersistence helper in Common.DerivativeSearch.Find<IPersistence>())
            {
                string prefix = helper.PersistencePrefix;
                if (prefix == null) continue;

                using (Lookup.Pusher pusher = new Lookup.Pusher(settings, prefix))
                {
                    helper.Import(settings);
                }
            }

            return settings.WasFound;
        }

        protected static XmlElement Locate(XmlElement parent, string[] keys)
        {
            foreach (string key in keys)
            {
                XmlElement element = parent;
                if (element == null) return null;

                if (element.Name == key) return element;

                XmlNode child = element.FirstChild;
                while (child != null)
                {
                    element = child as XmlElement;
                    if (element != null)
                    {
                        if (element.Name == key) return element;
                    }

                    child = child.NextSibling;
                }
            }

            return null;
        }

        protected static bool ParseSection(XmlElement parent, string[] keys, Lookup settings)
        {
            if (parent == null) return false;

            XmlElement element = Locate(parent, keys);
            if (element == null) return false;

            XmlNode child = element.FirstChild;
            while (child != null)
            {
                XmlAttribute value = child.Attributes["value"];
                if (value != null)
                {
                    settings.Add(child.Name, value.Value);
                }

                child = child.NextSibling;
            }

            return true;
        }

        // Exported to Overwatch
        public static string CreateExportString()
        {
            return CreateExportString2(VersionStamp.sNamespace.Replace("NRaas.", ""));
        }

        // Must have different name than above function
        public static string CreateExportString2(string parentName)
        {
            Lookup settings = new Lookup(Logger.sLogger);

            foreach (IPersistence helper in Common.DerivativeSearch.Find<IPersistence>())
            {
                string prefix = helper.PersistencePrefix;
                if (prefix == null) continue;

                using (Lookup.Pusher pusher = new Lookup.Pusher(settings, prefix))
                {
                    helper.Export(settings);
                }
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(Common.NewLine + "<" + parentName + ">");

            builder.Append(settings.ToString());

            builder.Append(Common.NewLine + "</" + parentName + ">");

            return builder.ToString();
        }

        public class Logger : Common.Logger<Logger>, Common.IAddLogger
        {
            public readonly static Logger sLogger = new Logger();

            public void Add(string value)
            {
                PrivateAppend(value);
            }

            protected override string Name
            {
                get { return "Persistence"; }
            }

            protected override Logger Value
            {
                get { return sLogger; }
            }
        }

        public class LookupBase
        {
            Dictionary<string, string> mLookup = new Dictionary<string, string>();

            List<string> mPrefix = new List<string>();

            Common.IAddLogger mErrors;

            bool mWasFound = false;

            protected LookupBase(Common.IAddLogger errors)
            {
                mErrors = errors;
            }

            public bool WasFound
            {
                get { return mWasFound; }
            }

            protected void Push(string key)
            {
                mPrefix.Add(GetPrefix () + key);
            }

            protected void Pop()
            {
                mPrefix.RemoveAt(mPrefix.Count - 1);
            }

            protected string GetPrefix()
            {
                if (mPrefix.Count == 0) return null;

                return mPrefix[mPrefix.Count - 1];
            }

            public void AddError(string error)
            {
                if (mErrors != null)
                {
                    mErrors.Add(error);
                }
            }

            public bool Add(string key, bool value)
            {
                return Add(key, value.ToString());
            }
            public bool Add(string key, int value)
            {
                return Add(key, value.ToString());
            }
            public bool Add(string key, ulong value)
            {
                return Add(key, value.ToString());
            }
            public bool Add(string key, float value)
            {
                return Add(key, value.ToString());
            }
            public bool Add(string key, string value)
            {
                if (string.IsNullOrEmpty(key)) return false;

                string lookup = GetPrefix() + key;
                if (lookup.Contains(" "))
                {
                    AddError("Space In Key: " + lookup);
                    return false;
                }

                if (mLookup.ContainsKey(lookup))
                {
                    AddError("Duplicate Key: " + lookup);

                    Common.DebugStackLog("Duplicate Key: " + lookup);
                    return false;
                }

                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Contains("&"))
                    {
                        AddError("Illegal Character: " + lookup);

                        value = value.Replace("&", " ");
                    }
                }

                mLookup.Add(lookup, value);
                return true;
            }

            public bool Exists(string key)
            {
                string result;
                return GetString(key, out result);
            }

            public bool GetString(string key, out string result)
            {
                if (key == null)
                {
                    result = null;
                    return false;
                }

                string lookup = GetPrefix() + key;
                if (string.IsNullOrEmpty(lookup))
                {
                    result = null;
                    return false;
                }

                if (mLookup.TryGetValue(lookup, out result))
                {
                    mWasFound = true;

                    //AddError("Found: " + lookup);
                    return true;
                }
                else
                {
                    //AddError("Missing: " + lookup);
                    return false;
                }
            }

            public override string ToString()
            {
                List<string> values = new List<string>();

                foreach (KeyValuePair<string, string> setting in mLookup)
                {
                    values.Add(" <" + setting.Key + " value=\"" + setting.Value + "\"/>");
                }

                values.Sort();

                StringBuilder builder = new StringBuilder();

                foreach(string value in values)
                {
                    builder.Append(Common.NewLine + value);
                }

                return builder.ToString();
            }

            public class WasFoundTest : IDisposable
            {
                bool mWasFound;

                LookupBase mLookup;

                public WasFoundTest(LookupBase lookup)
                {
                    mLookup = lookup;

                    mWasFound = mLookup.WasFound;
                    mLookup.mWasFound = false;
                }

                public void Dispose()
                {
                    if (!mLookup.WasFound)
                    {
                        mLookup.mWasFound = mWasFound;
                    }
                }
            }
        }

        public class Lookup : LookupBase
        {
            public Lookup(Common.IAddLogger errors)
                : base(errors)
            {}

            public bool Add(string key, Type type)
            {
                string assemblyName = type.Assembly.FullName;
                if (!string.IsNullOrEmpty(assemblyName))
                {
                    assemblyName = assemblyName.Split(',')[0];
                }

                return Add(key, type.FullName + "," + assemblyName);
            }

            public static bool IsPersistable(FieldInfo field)
            {
                foreach (object attribute in field.GetCustomAttributes(false))
                {
                    PersistableAttribute persistable = attribute as PersistableAttribute;
                    if (persistable != null)
                    {
                        return persistable.Persistable;
                    }
                }

                return true;
            }

            public void ReflectionGet(object obj)
            {
                foreach (FieldInfo field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (!IsPersistable(field)) continue;

                    string name = field.Name;

                    string value = GetString(name);

                    try
                    {
                        if (field.FieldType == typeof(string))
                        {
                            field.SetValue(obj, value);
                        }
                        else if (field.FieldType.IsEnum)
                        {
                            object result = null;
                            if (new EnumParser(field.FieldType, true).TryParse(value, out result))
                            {
                                field.SetValue(obj, result);
                            }
                            else
                            {
                                AddError("TryParse Enum Fail: " + GetPrefix() + name + " (" + field.FieldType.ToString() + ")");
                            }
                        }
                        else
                        {
                            Type[] argTypes = { typeof(string), field.FieldType.MakeByRefType() };
                            MethodInfo tryParse = field.FieldType.GetMethod("TryParse", argTypes);
                            if (tryParse != null)
                            {
                                object[] args = new object[] { value, null };
                                if ((bool)tryParse.Invoke(null, args))
                                {
                                    field.SetValue(obj, args[1]);
                                }
                                else
                                {
                                    AddError("TryParse Fail: " + GetPrefix() + name + " (" + field.FieldType.ToString() + ")");
                                }
                            }
                            else
                            {
                                AddError("TryParse Absent: " + GetPrefix() + name + " (" + field.FieldType.ToString() + ")");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(GetPrefix() + name, e);
                    }
                }
            }

            public void ReflectionAdd(object obj)
            {
                foreach (FieldInfo field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (!IsPersistable(field)) continue;

                    string name = field.Name;

                    object value = field.GetValue(obj);
                    if (value == null)
                    {
                        Add(name, "");
                    }
                    else
                    {
                        Add(name, value.ToString());
                    }
                }
            }

            public delegate T Conversion<T>(string value) where T : struct;

            public List<T> GetList<T>(string prefix, Conversion<T> convert)
                where T : struct
            {
                int count = GetInt(prefix + "Count", GetInt("Count", 0) /* Legacy */);

                List<T> elements = new List<T>();

                for (int i = 0; i < count; i++)
                {
                    elements.Add(convert(GetString(prefix + i)));
                }

                return elements;
            }
            public List<T> GetList<T>(string prefix)
                where T : class, IPersistence
            {
                int count = GetInt(prefix + "Count", GetInt("Count", 0) /* Legacy */);

                List<T> elements = new List<T>();

                for (int i = 0; i < count; i++)
                {
                    T element = GetChild<T>(prefix + i);
                    if (element == null) continue;

                    elements.Add(element);
                }

                return elements;
            }

            public void Add(string prefix, List<string> list)
            {
                string result = null;
                foreach (string value in list)
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        result += ",";
                    }

                    result += value;
                }

                Add(prefix, result);
            }
            public bool Add<T>(string prefix, IEnumerable<T> list, int length)
                where T : struct
            {
                Add(prefix + "Count", length.ToString());

                int count = 0;
                foreach (T element in list)
                {
                    Add(prefix + count, element.ToString());
                    count++;
                }

                return true;
            }
            public void Add<T>(string prefix, ICollection<T> list)
                where T : IPersistence
            {
                Add(prefix + "Count", list.Count.ToString());

                int count = 0;
                foreach (T element in list)
                {
                    AddChild(prefix + count, element);
                    count++;
                }
            }

            public void AddChild<T>(string prefix, T element)
                where T : IPersistence
            {
                using (Persistence.Lookup.Pusher pusher = new Persistence.Lookup.Pusher(this, prefix))
                {
                    Add("ClassName", element.GetType());

                    element.Export(this);
                }
            }

            public string[] GetStringList(string key)
            {
                string result;
                if (!GetString(key, out result)) return null;

                return result.Split(',');
            }

            public string GetString(string key)
            {
                string result;
                if (!GetString(key, out result)) return null;

                return result;
            }

            public T GetEnum<T>(string key, T def)
                where T : struct
            {
                string value = GetString(key);
                if (value == null) return def;

                T result;
                if (ParserFunctions.TryParseEnum<T>(value, out result, def))
                {
                    return result;
                }
                else
                {
                    return def;
                }
            }

            public int GetInt(string key, int def)
            {
                string value = GetString(key);
                if (value == null) return def;

                int result;
                if (!int.TryParse(value, out result))
                {
                    AddError("Parse Int Error: " + key);
                    return def;
                }

                return result;
            }

            public ulong GetULong(string key, ulong def)
            {
                string value = GetString(key);
                if (value == null) return def;

                ulong result;
                if (!ulong.TryParse(value, out result))
                {
                    AddError("Parse ULong Error: " + key);
                    return def;
                }

                return result;
            }

            public float GetFloat(string key, float def)
            {
                string value = GetString(key);
                if (value == null) return def;

                float result;
                if (!float.TryParse(value, out result))
                {
                    AddError("Parse Float Error: " + key);
                    return def;
                }

                return result;
            }

            public bool GetBool(string key, bool def)
            {
                string value = GetString(key);
                if (value == null) return def;

                bool result;
                if (!bool.TryParse(value, out result))
                {
                    AddError("Parse Bool Error: " + key);
                    return def;
                }

                return result;
            }

            public Type GetType(string key)
            {
                string value = GetString(key);
                if (value == null) return null;

                Type type = Type.GetType(value);
                if (type == null)
                {
                    AddError("Parse Type Error: " + key);
                }

                return type;
            }

            public T GetChild<T>(string prefix)
                where T : class, IPersistence
            {
                using (Persistence.Lookup.Pusher pusher = new Persistence.Lookup.Pusher(this, prefix))
                {
                    T element = GetObject<T>("ClassName");
                    if (element == null) return null;

                    using (Persistence.Lookup.WasFoundTest found = new Persistence.Lookup.WasFoundTest(this))
                    {
                        element.Import(this);

                        if (!WasFound) return null;
                    }

                    return element;
                }
            }

            protected T GetObject<T>(string key)
                where T : class
            {
                Type type = GetType(key);
                if (type == null) return null;

                ConstructorInfo constructor = type.GetConstructor(new Type[0]);
                if (constructor == null)
                {
                    AddError("Parse Constructor Missing: " + key + " (" + type.ToString() + ")");
                    return null;
                }

                T result = constructor.Invoke(new object[0]) as T;
                if (result == null)
                {
                    AddError("Parse Constructor Fail: " + key + " (" + type.ToString() + ")");
                    return null;
                }

                return result;
            }

            public class Pusher : IDisposable
            {
                Lookup mLookup;

                public Pusher(Lookup lookup, string key)
                {
                    mLookup = lookup;
                    mLookup.Push(key);
                }

                public void Dispose()
                {
                    mLookup.Pop();
                }
            }
        }
    }
}

