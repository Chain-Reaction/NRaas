using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
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

namespace NRaas.StoryProgressionSpace
{
    [Persistable]
    public abstract class GenericOptionBase : PersistentOptionBase, IInstallationBase
    {
        public enum DefaultingLevel
        {
            None = 0x0,
            Castes = 0x1,
            HeadOfFamily = 0x2,
            All = Castes | HeadOfFamily,
        }

        static Dictionary<Type, IGenericLevelOption> sDefaultOptions = new Dictionary<Type, IGenericLevelOption>();

        [Persistable(false)]
        Dictionary<Type, IGenericLevelOption> mGenericOptions = new Dictionary<Type, IGenericLevelOption>();

        [Persistable(false)]
        Dictionary<Type, IGenericLevelOption> mCachedOptions = new Dictionary<Type, IGenericLevelOption>();

        public GenericOptionBase()
        { }

        public abstract string Name
        {
            get;
        }

        public bool AddOption(IGenericLevelOption option)
        {
            Type type = option.GetType();

            mGenericOptions.Remove(type);
            mCachedOptions.Remove(type);

            if (!IsValidOption(option))
            {
                return false;
            }

            mGenericOptions.Add(type, option);
            return true;
        }

        public void RemoveOption(IGenericLevelOption option)
        {
            Type type = option.GetType();

            mGenericOptions.Remove(type);
            mCachedOptions.Remove(type);

            Remove(option as OptionItem);
        }
        
        public abstract SimDescription SimDescription
        {
            get;
        }

        public abstract Household House
        {
            get;
        }

        public abstract Lot Lot
        {
            get;
        }

        private void Clear()
        {
            List<IGenericLevelOption> options = new List<IGenericLevelOption>(mGenericOptions.Values);
            foreach (IGenericLevelOption option in options)
            {
                option.Manager = this;

                if (!option.ShouldDisplay()) continue;

                RemoveOption(option);

                UpdateInheritors(option);
            }
        }

        protected abstract void GetParentOptions(List<ParentItem> options, DefaultingLevel level);

        protected T GetDefaultOption<T>()
            where T : OptionItem, IGenericLevelOption, new()
        {
            IGenericLevelOption option;
            if (!sDefaultOptions.TryGetValue(typeof(T), out option))
            {
                option = new T();
                option.InitDefaultValue();

                sDefaultOptions.Add(typeof(T), option);
            }

            return option as T;
        }

        protected T GetInternalOption<T>()
            where T : OptionItem, IGenericLevelOption
        {
            return GetInheritedOption(typeof(T)) as T;
        }
        protected R GetInternalValue<T, R>()
            where T : GenericOptionItem<R>, IGenericLevelOption, new()
        {
            T item = GetInternalOption<T>();
            if (item == null)
            {
                item = GetDefaultOption<T>();
            }

            GenericOptionBase trueManager = item.Manager;
            try
            {
                item.Manager = this;
                return item.Value;
            }
            finally
            {
                item.Manager = trueManager;
            }
        }

        protected bool HasInternalValue<T, TType>(TType value)
            where T : OptionItem, IGenericHasOption<TType>, IGenericLevelOption, new()
        {
            T option = GetInternalOption<T>();
            if (option == null)
            {
                option = GetDefaultOption<T>();
            }

            GenericOptionBase trueManager = option.Manager;
            try
            {
                option.Manager = this;
                return option.Contains(value);
            }
            finally
            {
                option.Manager = trueManager;
            }
        }

        protected bool HasAnyInternalValue<T, TType>()
            where T : OptionItem, IGenericHasOption<TType>, IGenericLevelOption, new()
        {
            T option = GetInternalOption<T>();
            if (option == null)
            {
                option = GetDefaultOption<T>();
            }

            GenericOptionBase trueManager = option.Manager;
            try
            {
                option.Manager = this;
                return (option.Count > 0);
            }
            finally
            {
                option.Manager = trueManager;
            }
        }

        /*
        public bool CopyOption(IGenericLevelOption option)
        {
            IGenericLevelOption newOption = GetLocalOption(option.GetType());
            if (newOption == null) return false;

            newOption.Set(option, true);
            return true;
        }
        */

        public void Import(Manager manager, string key, Persistence.Lookup settings)
        {
            Clear();

            //Common.StringBuilder msg = new Common.StringBuilder();

            foreach (DefaultableOption option in GetOptions(manager, key, false))
            {
                List<string> names = PersistenceEx.GetExportKey(option);
                if (names == null) continue;

                for (int i = 0; i < names.Count; i++)
                {
                    string name = names[i];

                    string value = settings.GetString(name);
                    if (value == null) continue;

                    try
                    {
                        option.Clear(this);

                        //msg += Common.NewLine + "Name: " + option.Name;
                        //msg += Common.NewLine + option.GetUIValue(false);
                        //msg += Common.NewLine + value;

                        option.PersistValue = value;

                        //msg += Common.NewLine + option.GetUIValue(false);
                    }
                    catch (Exception e)
                    {
                        Common.Exception("Name: " + name + Common.NewLine + "Value: " + value, e);
                    }
                }
            }

            //Common.WriteLog(msg);
        }

        public void Export(Manager manager, string key, Persistence.Lookup settings)
        {
            foreach (OptionItem option in mGenericOptions.Values)
            {
                if (option is INotExportableOption) continue;

                if (option == null) continue;

                try
                {
                    List<string> names = PersistenceEx.GetExportKey(option);
                    if ((names == null) || (names.Count == 0)) continue;

                    settings.Add(key + names[0], option.GetExportValue());
                }
                catch (Exception e)
                {
                    Common.Exception(option.Name, e);
                }
            }
        }

        protected T GetLocalOption<T>()
            where T : OptionItem, IGenericLevelOption, new()
        {
            return GetLocalOption(typeof(T)) as T;
        }
        protected IGenericLevelOption GetLocalOption(Type type)
        {
            IGenericLevelOption item = GetInternalOption(type) as IGenericLevelOption;
            if (item == null)
            {
                item = type.GetConstructor(new Type[0]).Invoke(new object[0]) as IGenericLevelOption;

                if (!Installer<GenericOptionBase>.Install(this, item, true)) return null;
            }

            item.Manager = this;
            return item;
        }

        protected bool SetInternalValue<T, R>(R value)
            where T : GenericOptionItem<R>, IGenericLevelOption, new()
        {
            T item = GetLocalOption<T>();
            if (item == null) return false;

            item.SetValue (value);

            UpdateInheritors(item);
            return true;
        }

        protected TType AddInternalValue<T, TType>(TType value)
            where T : OptionItem, IGenericAddOption<TType>, IGenericLevelOption, new()
        {
            T item = GetLocalOption<T>();
            if (item == null) return default(TType);

            TType result = item.AddValue(value);

            UpdateInheritors(item);
            return result;
        }

        protected void RemoveInternalValue<T, TType>(TType value)
            where T : OptionItem, IGenericRemoveOption<TType>, IGenericLevelOption, new()
        {
            T item = GetLocalOption<T>();
            if (item == null) return;

            item.RemoveValue(value);

            UpdateInheritors(item);
        }

        protected IGenericLevelOption GetCachedOption(Type optionType)
        {
            IGenericLevelOption item;
            if (mCachedOptions.TryGetValue(optionType, out item))
            {
                return item;
            }
            else
            {
                return null;
            }
        }

        protected IGenericLevelOption GetInternalOption(Type optionType)
        {
            IGenericLevelOption item;
            if (mGenericOptions.TryGetValue(optionType, out item))
            {
                item.Manager = this;
                return item;
            }
            else
            {
                return null;
            }
        }

        public virtual void InvalidateCache()
        {
            mCachedOptions.Clear();
        }

        public void Uncache(IGenericLevelOption option)
        {
            mCachedOptions.Remove(option.GetType());
        }

        public void CopyOptions(GenericOptionBase options)
        {
            foreach (IGenericLevelOption option in options.mGenericOptions.Values)
            {
                AddOption(option.Clone() as IGenericLevelOption);
            }
        }

        public List<ParentItem> GetCombinedParents(DefaultingLevel level)
        {
            List<ParentItem> parents = new List<ParentItem>();
            parents.Add(new ParentItem(this, level));

            int i = 0;
            while (i < parents.Count)
            {
                ParentItem parent = parents[i];

                List<ParentItem> newParents = new List<ParentItem>();
                parent.mParent.GetParentOptions(newParents, parent.mLevel);

                i++;

                bool first = true;
                foreach (ParentItem newParent in newParents)
                {
                    if (first)
                    {
                        parents.Add(newParent);
                        first = false;
                    }
                    else
                    {
                        parents.Insert(i, newParent);
                    }
                }
            }

            return parents;
        }

        public struct ParentItem
        {
            public GenericOptionBase mParent;
            public DefaultingLevel mLevel;

            public ParentItem(GenericOptionBase parent, DefaultingLevel level)
            {
                mParent = parent;
                mLevel = level;
            }
        }

        protected IGenericLevelOption GetInheritedOption(Type optionType)
        {
            IGenericLevelOption item = GetCachedOption(optionType);
            if (item == null)
            {
                DefaultingLevel level = DefaultingLevel.None;

                if (!typeof(INotHeadOfFamilyCasteLevelOption).IsAssignableFrom(optionType))
                {
                    level |= DefaultingLevel.HeadOfFamily;
                }

                if (!typeof(INotCasteLevelOption).IsAssignableFrom(optionType))
                {
                    level |= DefaultingLevel.Castes;
                }

                List<ParentItem> parents = GetCombinedParents(level);

                for (int i = parents.Count-1; i >= 0; i--)
                {
                    ParentItem parent = parents[i];

                    IGenericLevelOption parentItem = parent.mParent.GetInternalOption(optionType);
                    if (parentItem != null)
                    {
                        OverrideStyle style = OverrideStyle.None;
                        if (i != 0)
                        {
                            style = OverrideStyle.ClearSet;
                        }

                        if (item == null)
                        {
                            item = parentItem.Clone() as IGenericLevelOption;

                            item.ApplyOverride(parentItem, style);

                            mCachedOptions[optionType] = item;
                        }
                        else
                        {
                            style |= OverrideStyle.CopyData;

                            item.ApplyOverride(parentItem, style);

                            item.Manager = parentItem.Manager;
                        }
                    }
                }
            }

            return item;
        }

        public static bool IsValidOption(GenericOptionBase manager, IGenericLevelOption option)
        {
            if (manager == null) return false;

            return manager.IsValidOption(option);
        }

        public abstract bool IsValidOption(IGenericLevelOption option);

        public virtual void UpdateInheritors(IGenericLevelOption option)
        {
            Uncache(option);
        }

        public override string ToString()
        {
            Common.StringBuilder text = new Common.StringBuilder();

            foreach (IGenericLevelOption option in mGenericOptions.Values)
            {
                option.Manager = this;

                text.AddXML(option.GetStoreKey(), option.PersistValue);
            }

            List<ParentItem> parents = GetCombinedParents(DefaultingLevel.All);
            if (parents.Count > 0)
            {
                bool found = false;

                foreach (ParentItem parent in parents)
                {
                    if (parent.mParent == this) continue;

                    if (!found)
                    {
                        text += Common.NewLine + "<Parent>";
                    }

                    found = true;

                    text += Common.NewLine + parent.mParent.ToString();
                }

                if (found)
                {
                    text += Common.NewLine + "</Parent>";
                }
            }
            else
            {
                text += Common.NewLine + "<NoParents/>";
            }

            return text.ToString();
        }

        public List<DefaultableOption> GetOptions(Manager manager, string prefix, bool checkDisplay)
        {
            List<IGenericLevelOption> simOptions = Common.DerivativeSearch.Find<IGenericLevelOption>(Common.DerivativeSearch.Caching.NoCache);

            if (manager.GetValue<Main.ShowHiddenOption, bool>())
            {
                checkDisplay = false;
            }

            bool debugging = manager.DebuggingEnabled;

            List<DefaultableOption> allOptions = new List<DefaultableOption>();
            foreach (IGenericLevelOption simOption in simOptions)
            {
                if (simOption is IDebuggingOption)
                {
                    if (!debugging) continue;
                }

                DefaultableOption option = new DefaultableOption(this, prefix, simOption);
                if (option == null) continue;

                if (!option.ShouldDisplay(checkDisplay)) continue;

                allOptions.Add(option);
            }

            return allOptions;
        }
        
        public List<DefaultableOption> ListOptions(Manager manager, string localizedTitle, bool checkDisplay)
        {
            List<DefaultableOption> allOptions = GetOptions(manager, null, checkDisplay);

            bool okayed = false;
            List<DefaultableOption> selection = OptionItem.ListOptions(allOptions, localizedTitle, allOptions.Count, out okayed);
            if ((selection == null) || (selection.Count == 0)) return null;

            return selection;
        }

        public bool ShowOptions(Manager manager, string localizedTitle)
        {
            while (true)
            {
                List<DefaultableOption> selection = ListOptions(manager, localizedTitle, true);
                if (selection == null) return false;

                foreach (DefaultableOption option in selection)
                {
                    option.Perform();
                }
            }
        }

        public virtual void ValidateCasteOptions(Dictionary<ulong, CasteOptions> castes)
        {
            foreach (IGenericLevelOption option in mGenericOptions.Values)
            {
                CasteListOption casteOption = option as CasteListOption;
                if (casteOption != null)
                {
                    casteOption.Validate(castes);
                }
            }
        }

        public virtual void Restore()
        {
            OptionLogger.AddTrace("Restore: " + GetType());

            if (Lot != null)
            {
                OptionLogger.AddTrace("Lot: " + Lot.Address + " (" + Lot.LotId + ")");
            }

            if (House != null)
            {
                OptionLogger.AddTrace("House: " + House.Name);
            }

            if (SimDescription != null)
            {
                OptionLogger.AddTrace("Sim: " + SimDescription.FullName);
            }

            mGenericOptions.Clear();
            mCachedOptions.Clear();

            new Installer<GenericOptionBase> (this).Perform(false);

            List<IGenericLevelOption> options = new List<IGenericLevelOption>(mGenericOptions.Values);

            foreach (IGenericLevelOption option in options)
            {
                if (!Restore(option as OptionItem))
                {
                    if (!RetainAllOptions)
                    {
                        Type type = option.GetType();

                        mGenericOptions.Remove(type);
                        mCachedOptions.Remove(type);
                    }
                    else if (Common.IsAwayFromHomeworld ())
                    {
                        IAdjustForVacationOption adjustForVacation = option as IAdjustForVacationOption;
                        if (adjustForVacation != null)
                        {
                            adjustForVacation.AdjustForVacationTown();
                        }
                    }
                }
            }
        }

        protected virtual bool RetainAllOptions
        {
            get { return false; }
        }

        public class DefaultableOption : OptionItem, INotPersistableOption
        {
            GenericOptionBase mManager;

            IGenericLevelOption mOption;

            string mPrefix;

            bool mInherited = false;

            public DefaultableOption(GenericOptionBase manager, string prefix, IGenericLevelOption option)
            {
                mManager = manager;
                mOption = option.Clone() as IGenericLevelOption;
                mPrefix = prefix;

                mOption.Manager = mManager;

                IGenericLevelOption existingOption = mManager.GetInheritedOption(mOption.GetType ());
                if (existingOption != null)
                {
                    mInherited = (existingOption.Manager != mManager);

                    mOption.Set(existingOption, false);
                }
                else
                {
                    mOption.InitDefaultValue();

                    mInherited = true;
                }
            }

            public override string Name
            {
                get
                {
                    return mOption.Name;
                }
            }

            public bool Resetable
            {
                get 
                {
                    if (mInherited) return false;

                    if (mOption is INotPersistableOption) return false;

                    return true;
                }
            }

            protected override string GetLocalizationValueKey()
            {
                return null;
            }

            public override string GetTitlePrefix()
            {
                return null;
            }

            public override string GetStoreKey()
            {
                if (mOption == null) return null;

                return mPrefix + mOption.GetStoreKey();
            }

            public void Clear(GenericOptionBase manager)
            {
                IGenericLevelOption option = manager.GetLocalOption(mOption.GetType());
                if (option == null) return;

                ICustomClearOption customClear = option as ICustomClearOption;
                if (customClear != null)
                {
                    if (customClear.Clear(manager))
                    {
                        manager.RemoveOption(option);
                    }
                    else
                    {
                        option.Persist();
                    }
                }
                else
                {
                    manager.RemoveOption(option);
                }

                manager.UpdateInheritors(option);
            }

            public override string GetUIValue(bool pure)
            {
                string uiValue = mOption.GetUIValue(true);
                if ((!pure) && (!string.IsNullOrEmpty(uiValue)))
                {
                    if (mInherited)
                    {
                        if ((!(mManager is CasteOptions)) || (mOption is ICasteFilterOption))
                        {
                            uiValue = "(" + uiValue + ")";
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                return uiValue;
            }

            public override object PersistValue
            {
                get 
	            {
                    return mOption.PersistValue;
	            }
	            set 
	            {
                    mOption.PersistValue = value;

                    PostPerform(mManager, true);
	            }
            }

            public override void NotifyAfterChange(string originalUIValue)
            { }

            // Used by MasterController Progression
            public void Persist(GenericOptionBase manager)
            {
                PostPerform(manager, false);
            }

            protected override bool PrivatePerform()
            {
                if (!mOption.Perform()) return false;

                PostPerform(mManager, false);
                return true;
            }

            protected void PostPerform(GenericOptionBase manager, bool clear)
            {
                if (manager == null) return;

                IGenericLevelOption option = manager.GetLocalOption(mOption.GetType());
                if (option != null)
                {
                    if (clear)
                    {
                        option.Set(mOption, false);
                    }
                    else
                    {
                        option.ApplyOverride(mOption, OverrideStyle.CopyData | OverrideStyle.MergeSet);
                    }
                
                    option.Persist();

                    manager.UpdateInheritors(option);
                }
            }

            public bool ShouldDisplay(bool checkDisplay)
            {
                if (checkDisplay)
                {
                    return ShouldDisplay();
                }
                else
                {
                    return mManager.IsValidOption(mOption);
                }
            }
            public override bool ShouldDisplay()
            {
                return mOption.ShouldDisplay();
            }
        }

        public abstract class BooleanOption : BooleanOptionItem, IGenericLevelOption
        {
            GenericOptionBase mManager = null;

            public BooleanOption(bool defValue)
                : base (defValue)
            { }

            public GenericOptionBase Manager
            {
                get { return mManager; }
                set { mManager = value; }
            }

            public virtual bool HasRequiredVersion()
            {
                return true;
            }

            public virtual bool Install(GenericOptionBase manager, bool initial)
            {
                if (!HasRequiredVersion()) return false;

                mManager = manager;

                return mManager.AddOption(this);
            }

            public virtual void Set(IGenericLevelOption option, bool persist)
            {
                BooleanOptionItem newOption = option as BooleanOptionItem;
                if (newOption == null) return;

                SetValue (newOption.PureValue, persist);
            }

            protected R GetValue<T, R>()
                where T : GenericOptionItem<R>, IGenericLevelOption, new()
            {
                return Manager.GetInternalValue<T, R>();
            }

            protected override PersistentOptionBase Store
            {
                get { return mManager; }
            }

            public void ApplyOverride(IGenericLevelOption paramOption, OverrideStyle style)
            {
                if ((style & OverrideStyle.CopyData) == OverrideStyle.CopyData)
                {
                    BooleanOption option = paramOption as BooleanOption;
                    if (option != null)
                    {
                        SetValue(option.Value, false);
                    }
                }
            }

            public bool BaseShouldDisplay()
            {
                return Manager.IsValidOption(this);
            }

            public override bool ShouldDisplay()
            {
                return BaseShouldDisplay();
            }
        }

        public abstract class StringOption : StringOptionItem, IGenericLevelOption
        {
            GenericOptionBase mManager = null;

            public StringOption(string defValue)
                : base(defValue)
            { }

            public GenericOptionBase Manager
            {
                get { return mManager; }
                set { mManager = value; }
            }

            public virtual bool HasRequiredVersion()
            {
                return true;
            }

            public virtual bool Install(GenericOptionBase manager, bool initial)
            {
                if (!HasRequiredVersion()) return false;

                mManager = manager;

                return mManager.AddOption(this);
            }

            public virtual void Set(IGenericLevelOption option, bool persist)
            {
                StringOptionItem newOption = option as StringOptionItem;
                if (newOption == null) return;

                SetValue(newOption.PureValue, persist);
            }

            protected R GetValue<T, R>()
                where T : GenericOptionItem<R>, IGenericLevelOption, new()
            {
                return Manager.GetInternalValue<T, R>();
            }

            protected override PersistentOptionBase Store
            {
                get { return mManager; }
            }

            public void ApplyOverride(IGenericLevelOption paramOption, OverrideStyle style)
            {
                if ((style & OverrideStyle.CopyData) == OverrideStyle.CopyData)
                {
                    StringOption option = paramOption as StringOption;
                    if (option != null)
                    {
                        SetValue(option.Value, false);
                    }
                }
            }

            public override bool ShouldDisplay()
            {
                return Manager.IsValidOption(this);
            }
        }

        public abstract class IntegerOption : IntegerOptionItem, IGenericLevelOption
        {
            GenericOptionBase mManager = null;

            public IntegerOption(int defValue)
                : base(defValue)
            { }

            public GenericOptionBase Manager
            {
                get { return mManager; }
                set { mManager = value; }
            }

            public virtual bool HasRequiredVersion()
            {
                return true;
            }

            public virtual bool Install(GenericOptionBase manager, bool initial)
            {
                if (!HasRequiredVersion()) return false;

                mManager = manager;

                return mManager.AddOption(this);
            }

            public void Set(IGenericLevelOption option, bool persist)
            {
                IntegerOptionItem newOption = option as IntegerOptionItem;
                if (newOption == null) return;

                SetValue(newOption.PureValue, persist);
            }

            protected R GetValue<T, R>()
                where T : GenericOptionItem<R>, IGenericLevelOption, new()
            {
                return Manager.GetInternalValue<T, R>();
            }

            protected override PersistentOptionBase Store
            {
                get { return mManager; }
            }

            public void ApplyOverride(IGenericLevelOption paramOption, OverrideStyle style)
            {
                if ((style & OverrideStyle.CopyData) == OverrideStyle.CopyData)
                {
                    IntegerOption option = paramOption as IntegerOption;
                    if (option != null)
                    {
                        SetValue(option.Value, false);
                    }
                }
            }

            public override bool ShouldDisplay()
            {
                return Manager.IsValidOption(this);
            }
        }

        public abstract class GenericOption<TValue> : GenericOptionItem<TValue>, IGenericLevelOption
        {
            GenericOptionBase mManager = null;

            public GenericOption(TValue value, TValue defValue)
                : base(value, defValue)
            { }

            public GenericOptionBase Manager
            {
                get { return mManager; }
                set { mManager = value; }
            }

            public virtual bool HasRequiredVersion()
            {
                return true;
            }

            public virtual bool Install(GenericOptionBase manager, bool initial)
            {
                if (!HasRequiredVersion()) return false;

                mManager = manager;

                return mManager.AddOption(this);
            }

            public virtual void Set(IGenericLevelOption option, bool persist)
            {
                GenericOptionItem<TValue> newOption = option as GenericOptionItem<TValue>;
                if (newOption == null) return;

                SetValue(newOption.PureValue, persist);
            }

            protected R GetValue<T, R>()
                where T : GenericOptionItem<R>, IGenericLevelOption, new()
            {
                return Manager.GetInternalValue<T, R>();
            }

            protected override PersistentOptionBase Store
            {
                get { return mManager; }
            }

            public virtual void ApplyOverride(IGenericLevelOption paramOption, OverrideStyle style)
            {
                if ((style & OverrideStyle.CopyData) == OverrideStyle.CopyData)
                {
                    GenericOption<TValue> option = paramOption as GenericOption<TValue>;
                    if (option != null)
                    {
                        SetValue(option.Value, false);
                    }
                }
            }

            public override bool ShouldDisplay()
            {
                return Manager.IsValidOption(this);
            }

            public override ICommonOptionItem Clone()
            {
                IGenericLevelOption result = base.Clone() as IGenericLevelOption;

                result.Set(this, false);

                return result;
            }
        }

        public abstract class ChoiceOptionItem<T> : GenericOption<T>
        {
            public ChoiceOptionItem(T value, T defValue)
                : base(value, defValue)
            { }

            protected abstract string GetLocalizedValue(T value, ref bool matches, ref ThumbnailKey icon);

            public override void Set(IGenericLevelOption option, bool persist)
            {
                ChoiceOptionItem<T> newOption = option as ChoiceOptionItem<T>;
                if (newOption == null) return;

                SetValue(newOption.PureValue, persist);
            }

            protected virtual bool Allow(T value)
            {
                return true;
            }

            protected virtual string ValuePrefix
            {
                get { return "Boolean"; }
            }

            protected abstract IEnumerable<T> GetOptions();

            protected override bool PrivatePerform()
            {
                List<Item> choices = new List<Item>();

                foreach (T choice in GetOptions())
                {
                    if (!Allow(choice)) continue;

                    bool matches = false;
                    ThumbnailKey icon = ThumbnailKey.kInvalidThumbnailKey;
                    string name = GetLocalizedValue(choice, ref matches, ref icon);

                    choices.Add(new Item(choice, name, ValuePrefix, matches, icon));
                }

                Item selection = new CommonSelection<Item>(Name, choices).SelectSingle();
                if (selection == null) return false;

                SetValue(selection.Value);
                return true;
            }

            public class Item : ValueSettingOption<T>
            {
                string mValuePrefix;

                public Item(T value, string name, string valuePrefix, bool selected, ThumbnailKey thumbnail)
                    : base(value, name, selected ? 1 : 0, thumbnail)
                {
                    mValuePrefix = valuePrefix;
                }

                public override string DisplayKey
                {
                    get { return mValuePrefix; }
                }
            }
        }

        public abstract class EnumOptionItem<T> : ChoiceOptionItem<T>
            where T : struct
        {
            public EnumOptionItem(T value, T defValue)
                : base(value, defValue)
            { }

            protected override IEnumerable<T> GetOptions()
            {
                List<T> results = new List<T>();

                foreach(T choice in Enum.GetValues(typeof(T)))
                {
                    results.Add(choice);
                }

                return results;
            }

            public override object PersistValue
            {
                set
                {
                    if (value is string)
                    {
                        T result;
                        ParserFunctions.TryParseEnum<T>(value as string, out result, Default);
                        SetValue(result);
                    }
                    else
                    {
                        SetValue((T)value);
                    }
                }
            }
        }

        public abstract class ListedOptionItem<TRawType, TStorageType> : GenericOption<List<TStorageType>>, IGenericHasOption<TRawType>, IGenericAddOption<TRawType>, IGenericRemoveOption<TRawType>, ICustomClearOption
        {
            Dictionary<TStorageType, bool> mLookup = null;

            List<TStorageType> mSet = new List<TStorageType>();

            public ListedOptionItem(List<TStorageType> value, List<TStorageType> defValue)
                : base(value, defValue)
            { }

            public override int Count
            {
                get
                {
                    return Value.Count;
                }
            }

            protected abstract string GetLocalizationUIKey();

            protected IEnumerable<TStorageType> SetList
            {
                get { return mSet; }
            }

            protected Dictionary<TStorageType, bool> Lookup
            {
                get
                {
                    if (mLookup == null)
                    {
                        mLookup = new Dictionary<TStorageType, bool>();

                        foreach (TStorageType item in Value)
                        {
                            mLookup[item] = true;
                        }
                    }

                    return mLookup;
                }
            }

            public override string GetUIValue(bool pure)
            {
                string prefix = GetLocalizationUIKey();
                if (string.IsNullOrEmpty(prefix))
                {
                    string text = null;

                    if (Lookup.Count > 0)
                    {
                        text = EAText.GetNumberString(Lookup.Count);
                    }

                    int difference = Lookup.Count - mSet.Count;
                    if (difference < 0)
                    {
                        if (!string.IsNullOrEmpty(text))
                        {
                            text = ":" + text;
                        }

                        text = EAText.GetNumberString(difference) + text;
                    }

                    return text;
                }
                else
                {
                    string text = ConvertToUIValue(prefix, Value);

                    if ((!pure) && (!string.IsNullOrEmpty(text)))
                    {
                        if (text == ConvertToUIValue(prefix, Default))
                        {
                            text = "(" + text + ")";
                        }
                    }

                    return text;
                }
            }

            protected string ConvertToUIValue(string prefix, IEnumerable<TStorageType> list)
            {
                return new Converter(prefix, IsFemaleLocalization()).Convert(list);
            }

            public TRawType AddValue(TRawType value)
            {
                bool valid;
                TStorageType type = ConvertToValue(value, out valid);
                if (!valid) return value;

                if (!ContainsDirect(type))
                {
                    Value.Add(type);
                    AddSet(type);

                    Persist();
                }

                return value;
            }

            public void RemoveValue(TRawType value)
            {
                bool valid;
                TStorageType storeValue = ConvertToValue(value, out valid);
                if (!valid) return;

                Value.Remove(storeValue);

                Persist();
            }

            protected void RemoveDirectValue(TStorageType value, bool persist)
            {
                Value.Remove(value);
                mSet.Remove(value);

                mLookup = null;

                if (persist)
                {
                    Persist();
                }
            }

            public override void SetValue(List<TStorageType> value, bool persist)
            {
                if (value == null)
                {
                    value = new List<TStorageType>();
                }

                mLookup = null;

                base.SetValue(value, persist);
            }

            public bool ContainsDirect(TStorageType value)
            {
                return Lookup.ContainsKey(value);
            }
            public bool Contains(TRawType value)
            {
                bool valid;
                TStorageType storeValue = ConvertToValue(value, out valid);
                if (!valid) return false;

                return Lookup.ContainsKey(storeValue);
            }

            protected void AddUnique(TStorageType value)
            {
                if (ContainsDirect(value)) return;

                Value.Add(value);
            }

            protected void AddSet(TStorageType value)
            {
                mSet.Add(value);
            }

            protected abstract string ValuePrefix
            {
                get;
            }

            protected abstract TStorageType ConvertFromString(string value);

            protected abstract TStorageType ConvertToValue(TRawType value, out bool valid);

            protected override string GetLocalizationValueKey()
            {
                return null;
            }

            protected abstract string GetLocalizedValue(TRawType value, ref ThumbnailKey icon);

            protected virtual bool Allow(TRawType value)
            {
                return true;
            }

            protected abstract IEnumerable<TRawType> GetOptions();

            public override void Set(IGenericLevelOption option, bool persist)
            {
                ListedOptionItem<TRawType, TStorageType> newOption = option as ListedOptionItem<TRawType, TStorageType>;
                if (newOption == null) return;

                mSet = new List<TStorageType>(newOption.mSet);

                SetValue(new List<TStorageType>(newOption.PureValue), persist);
            }

            public override void ApplyOverride(IGenericLevelOption option, OverrideStyle style)
            {
                ListedOptionItem<TRawType, TStorageType> overrideOption = option as ListedOptionItem<TRawType, TStorageType>;
                if (overrideOption != null)
                {
                    bool mergeSet = ((style & OverrideStyle.MergeSet) == OverrideStyle.MergeSet);

                    if ((style & OverrideStyle.CopyData) == OverrideStyle.CopyData)
                    {
                        foreach (TStorageType type in overrideOption.mSet)
                        {
                            if (overrideOption.ContainsDirect(type))
                            {
                                AddUnique(type);
                            }
                            else
                            {
                                Value.Remove(type);
                            }

                            if (mergeSet)
                            {
                                if (!mSet.Contains(type))
                                {
                                    AddSet(type);
                                }
                            }
                        }
                    }

                    if ((style & OverrideStyle.ClearSet) == OverrideStyle.ClearSet)
                    {
                        mSet.Clear();
                    }
                    else if (!mergeSet)
                    {
                        mSet = new List<TStorageType>(overrideOption.mSet);
                    }

                    mLookup = null;
                }
            }

            public bool Clear(GenericOptionBase manager)
            {
                List<Item> choices = new List<Item>();

                foreach (TRawType choice in GetOptions())
                {
                    bool valid;
                    TStorageType storeValue = ConvertToValue(choice, out valid);
                    if (!valid) continue;

                    if (!mSet.Contains(storeValue)) continue;

                    ThumbnailKey icon = ThumbnailKey.kInvalidThumbnailKey;
                    string name = GetLocalizedValue(choice, ref icon);

                    choices.Add(new Item(this, choice, name, icon));
                }

                CommonSelection<Item>.Results selection = new CommonSelection<Item>(Name, choices).SelectMultiple();

                foreach (Item item in selection)
                {
                    Value.Remove(item.Value);

                    mSet.Remove(item.Value);
                }

                mLookup = null;

                return (mSet.Count == 0);
            }

            protected override bool PrivatePerform()
            {
                List<Item> choices = new List<Item>();

                foreach (TRawType choice in GetOptions())
                {
                    if (!Allow(choice)) continue;

                    ThumbnailKey icon = ThumbnailKey.kInvalidThumbnailKey;
                    string name = GetLocalizedValue(choice, ref icon);

                    choices.Add(new Item(this, choice, name, icon));
                }

                CommonSelection<Item>.Results selection = new CommonSelection<Item>(Name, choices).SelectMultiple();

                foreach (Item item in selection)
                {
                    if (ContainsDirect(item.Value))
                    {
                        Value.Remove(item.Value);
                    }
                    else
                    {
                        Value.Add(item.Value);
                    }

                    if (!mSet.Contains(item.Value))
                    {
                        AddSet(item.Value);
                    }
                }

                mLookup = null;
                return true;
            }

            public override bool Persist()
            {
                mLookup = null;

                return base.Persist();
            }

            public override object PersistValue
            {
                get
                {
                    string result = new ListToString<TStorageType>().Convert(Value);
                    result += "|" + new ListToString<TStorageType>().Convert(mSet);
                    return result;
                }
                set
                {
                    Value.Clear();
                    mSet.Clear();

                    if (value is string)
                    {
                        string[] values = (value as string).Split('|');
                        if (values.Length == 2)
                        {
                            foreach (string val in values[1].Split(','))
                            {
                                if (string.IsNullOrEmpty(val)) continue;

                                TStorageType entry = ConvertFromString(val);
                                if (entry != null)
                                {
                                    AddSet(entry);
                                }
                            }
                        }

                        foreach (string val in values[0].Split(','))
                        {
                            if (string.IsNullOrEmpty(val)) continue;

                            TStorageType entry = ConvertFromString(val);
                            if (entry != null)
                            {
                                AddUnique(entry);
                            }
                        }
                    }

                    mLookup = null;
                }
            }

            public class Converter : ListToString<TStorageType>
            {
                string mPrefixKey;
                bool mIsFemale;

                public Converter(string prefixKey, bool isFemale)
                {
                    mPrefixKey = prefixKey;
                    mIsFemale = isFemale;
                }

                protected override string PrivateConvert(TStorageType value)
                {
                    return Common.Localize(mPrefixKey + ":" + value, mIsFemale);
                }
            }

            public class Item : ValueSettingOption<TStorageType>
            {
                ListedOptionItem<TRawType, TStorageType> mOption;

                public Item(ListedOptionItem<TRawType, TStorageType> option, TRawType value, string name, ThumbnailKey thumbnail)
                    : base(ConvertToValue(option, value), name, -1, thumbnail)
                {
                    mOption = option;
                }

                protected static TStorageType ConvertToValue(ListedOptionItem<TRawType, TStorageType> option, TRawType value)
                {
                    bool valid;
                    TStorageType result = option.ConvertToValue(value, out valid);
                    if (!valid) return default(TStorageType);

                    return result;
                }

                public override string DisplayValue
                {
                    get
                    {
                        if (string.IsNullOrEmpty(mOption.ValuePrefix)) return null;

                        string result = Common.Localize(mOption.ValuePrefix + ":" + mOption.ContainsDirect(mValue)).Trim();
                        if (!mOption.mSet.Contains(mValue))
                        {
                            if (!string.IsNullOrEmpty(result))
                            {
                                result = "(" + result + ")";
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(result))
                            {
                                result = "---";
                            }
                        }

                        return result;
                    }
                }
            }
        }
    }
}

