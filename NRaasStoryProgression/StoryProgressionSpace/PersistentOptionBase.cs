using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace
{
    [Persistable]
    public class PersistentOptionBase
    {
        // This field must have a unique name amongst the fields in all derivative classes or persistence will fail on load
        List<PersistentOption> mOptions = new List<PersistentOption>();

        [Persistable(false)]
        Dictionary<Type, PersistentNameLookup> mOptionsLookup = null;

        public PersistentOptionBase()
        { }

        private Dictionary<Type, PersistentNameLookup> Options
        {
            get
            {
                if (mOptionsLookup == null)
                {
                    mOptionsLookup = new Dictionary<Type, PersistentNameLookup>();

                    int index = 0;
                    while (index < mOptions.Count)
                    {
                        PersistentOption option = mOptions[index];

                        if ((option == null) || (option.mType == null))
                        {
                            mOptions.RemoveAt(index);
                        }
                        else
                        {
                            PersistentNameLookup names;
                            if (!mOptionsLookup.TryGetValue(option.mType, out names))
                            {
                                names = new PersistentNameLookup();
                                mOptionsLookup.Add(option.mType, names);
                            }

                            names.Add(option.mName, option);
                            index++;
                        }
                    }
                }
                return mOptionsLookup;
            }
        }

        private PersistentNameLookup GetExisting(Type type)
        {
            PersistentNameLookup names;
            if (Options.TryGetValue(type, out names))
            {
                return names;
            }
            else
            {
                return null;
            }
        }

        private PersistentNameLookup Get(Type type)
        {
            PersistentNameLookup names = GetExisting(type);
            if (names == null)
            {
                names = new PersistentNameLookup();

                Options.Add(type, names);
            }

            return names;
        }

        public bool Restore(OptionItem item)
        {
            if (item == null) return false;

            try
            {
                string name = item.GetStoreKey();

                OptionLogger.AddTrace(" Option: " + name);

                PersistentNameLookup names = GetExisting(item.GetType());
                if (names == null)
                {
                    OptionLogger.AddError("  No Name");
                    return false;
                }

                PersistentOption option = names.Get(name);
                if (option == null)
                {
                    OptionLogger.AddError("  No Option");
                    return false;
                }

                try
                {
                    if (option.mValue != null)
                    {
                        item.PersistValue = option.mValue;
                    }
                    return true;
                }
                catch (InvalidCastException e)
                {
                    Common.DebugException(item.GetStoreKey() + ": " + option.mValue + " (" + option.mValue.GetType() + ")", e);

                    OptionLogger.AddError("  Casting Fail");
                    return false;
                }
            }
            catch (Exception e)
            {
                Common.Exception(item.GetStoreKey(), e);

                OptionLogger.AddError("  Exception");
                return false;
            }
        }

        protected void Remove(OptionItem item)
        {
            if (item == null) return;

            string name = item.GetStoreKey();
            if (string.IsNullOrEmpty(name)) return;

            Type type = item.GetType();

            PersistentNameLookup names = GetExisting(type);
            if (names == null) return;

            PersistentOption option = names.Get(name);
            if (option != null)
            {
                mOptions.Remove(option);

                names.Remove(name, option);
            }
        }

        public bool Store(OptionItem item)
        {
            if (item is INotPersistableOption) return false;

            string name = item.GetStoreKey();
            if (string.IsNullOrEmpty(name)) return false;

            Type type = item.GetType();

            PersistentNameLookup names = Get(type);

            object value = item.PersistValue;

            PersistentOption option = names.Get(name);
            if (option == null)
            {
                option = new PersistentOption(type, name, value);

                mOptions.Add(option);

                names.Add(name, option);
            }
            else
            {
                option.mName = name;
                option.mValue = value;
            }

            return true;
        }

        [Persistable]
        public class PersistentOption
        {
            public Type mType = null;
            public string mName = null;
            public object mValue = null;

            public PersistentOption() // required for persistence
            { }
            public PersistentOption(Type type, string name, object value)
            {
                mType = type;
                mName = name;
                mValue = value;
            }
        }

        private class PersistentNameLookup
        {
            private PersistentOption mOption = null;

            private Dictionary<string, PersistentOption> mLookup = new Dictionary<string, PersistentOption>();

            public PersistentNameLookup()
            { }

            public override string ToString()
            {
                string text = null;

                if (mOption != null)
                {
                    text += "Unnamed = " + mOption.mType;
                }
                else
                {
                    text += "Unnamed = (Missing)";
                }

                foreach (string name in mLookup.Keys)
                {
                    text += Common.NewLine + "Named = " + name;
                }

                return text;
            }

            public void Add(string name, PersistentOption option)
            {
                if (string.IsNullOrEmpty(name))
                {
                    mOption = option;
                }
                else
                {
                    mLookup.Add(name, option);
                }
            }

            public PersistentOption Get(string name)
            {
                PersistentOption value = mOption;
                if ((!string.IsNullOrEmpty(name)) && (!mLookup.TryGetValue(name, out value)) && (mOption != null))
                {
                    value = mOption;

                    mLookup.Add(name, mOption);

                    mOption = null;
                }

                return value;
            }

            public void Remove(string name, PersistentOption option)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    mLookup.Remove(name);
                }
                else if (mOption == option)
                {
                    mOption = null;
                }
            }
        }

        public class OptionLogger : Common.TraceLogger<OptionLogger>
        {
            readonly static OptionLogger sLogger = new OptionLogger();

            public static void AddTrace(string msg)
            {
                //sLogger.PrivateAddTrace(msg);
            }
            public static void AddError(string msg)
            {
                //sLogger.PrivateAddError(msg);
            }

            protected override string Name
            {
                get { return "Options"; }
            }

            protected override OptionLogger Value
            {
                get { return sLogger; }
            }
        }
    }
}

