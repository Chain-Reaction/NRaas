using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Options
{
    public abstract class ListedSettingOption<TType, TTarget> : InteractionOptionItem<IActor, TTarget, GameHitParameters< TTarget>>, ICloseDialogOption, IPersistence
        where TTarget : class, IGameObject
    {
        protected virtual string GetValuePrefix()
        {
            return GetTitlePrefix();
        }

        public virtual string GetLocalizedValue(TType value)
        {
            return Common.Localize(GetValuePrefix() + ":" + value);
        }

        protected virtual int NumSelectable
        {
            get { return 0; }
        }

        protected abstract Proxy GetList();

        protected virtual bool Allow(TType value)
        {
            return true;
        }

        protected virtual List<Item> GetOptions()
        {
            List<Item> results = new List<Item>();
            foreach (TType value in Enum.GetValues(typeof(TType)))
            {
                if (!Allow(value)) continue;

                results.Add(new Item(this, value));
            }

            return results;
        }

        protected virtual void PrivatePerform(IEnumerable<Item> results)
        {
            Proxy list = GetList();

            foreach (Item item in results)
            {
                if (list.Contains(item.Value))
                {
                    list.Remove(item.Value);
                }
                else
                {
                    list.Add(item.Value);
                }
            }
        }

        public virtual string ExportName
        {
            get
            {
                string name = GetTitlePrefix();

                ITitlePrefixOption parent = ParentListingOption;
                if (parent != null)
                {
                    name = parent.ExportName + name;
                }

                return name;
            }
        }

        public abstract ITitlePrefixOption ParentListingOption
        {
            get;
        }

        public override string DisplayValue
        {
            get { return GetList().GetDisplayValue(this); }
        }

        public abstract string ConvertToString(TType value);

        public abstract bool ConvertFromString(string value, out TType newValue);

        public virtual string GetExportValue()
        {
            return GetList().GetExportValue(this);
        }

        public virtual void SetImportValue(string value)
        {
            Proxy list = GetList();

            list.Clear();

            foreach (TType newValue in StringToList<TType>.StaticConvert(value, ConvertFromString))
            {
                list.Add(newValue);
            }
        }

        public virtual string PersistencePrefix
        {
            get { return ""; }
        }

        public virtual void Import(Persistence.Lookup settings)
        {
            string name = GetType().ToString();
            try
            {
                string value = null;
                if (!settings.GetString(ExportName, out value)) return;

                SetImportValue(value);
            }
            catch (Exception e)
            {
                Common.Exception(name, e);
            }
        }

        public virtual void Export(Persistence.Lookup settings)
        {
            string name = GetType().ToString();

            try
            {
                settings.Add(ExportName, GetExportValue());
            }
            catch (Exception e)
            {
                Common.Exception(name, e);
            }
        }

        protected override OptionResult Run(GameHitParameters<TTarget> parameters)
        {
            CommonSelection<Item>.Results selection = new CommonSelection<Item>(Name, GetOptions()).SelectMultiple(NumSelectable);
            if (selection == null) return OptionResult.Failure;

            if (!selection.mOkayed) return OptionResult.Failure;

            PrivatePerform(selection);

            Common.Notify(ToString());
            return OptionResult.SuccessRetain;
        }

        public abstract class Proxy
        {
            public abstract string GetDisplayValue(ListedSettingOption<TType, TTarget> option);

            public abstract string GetExportValue(ListedSettingOption<TType, TTarget> option);

            public abstract void Clear();

            public abstract bool Contains(TType value);

            public abstract void Add(TType value);

            public abstract void Remove(TType value);
        }

        public class ListProxy : Proxy, IEnumerable<TType>
        {
            List<TType> mList;

            public ListProxy(List<TType> list)
            {
                mList = list;
            }

            public int Count
            {
                get { return mList.Count; }
            }

            public override string GetDisplayValue(ListedSettingOption<TType, TTarget> option)
            {
                int count = 0;
                if (mList != null)
                {
                    count = mList.Count;
                }

                return EAText.GetNumberString(count);
            }

            public override string GetExportValue(ListedSettingOption<TType, TTarget> option)
            {
                return ListToString<TType>.StaticConvert(mList, option.ConvertToString);
            }

            public override void Clear()
            {
                mList.Clear();
            }

            public override void Add(TType value)
            {
                mList.Add(value);
            }

            public override void Remove(TType value)
            {
                mList.Remove(value);
            }

            public override bool Contains(TType value)
            {
                return mList.Contains(value);
            }

            public IEnumerator<TType> GetEnumerator()
            {
                return mList.GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return mList.GetEnumerator();
            }
        }

        public class DictionaryProxy : Proxy, IEnumerable<TType>
        {
            Dictionary<TType, bool> mList;

            public DictionaryProxy(Dictionary<TType, bool> list)
            {
                mList = list;
            }

            public override string GetDisplayValue(ListedSettingOption<TType, TTarget> option)
            {
                int count = 0;
                if (mList != null)
                {
                    count = mList.Count;
                }

                return EAText.GetNumberString(count);
            }

            public override string GetExportValue(ListedSettingOption<TType, TTarget> option)
            {
                return ListToString<TType>.StaticConvert(mList.Keys, option.ConvertToString);
            }

            public override void Clear()
            {
                mList.Clear();
            }

            public override void Add(TType value)
            {
                mList[value] = true;
            }

            public override void Remove(TType value)
            {
                mList.Remove(value);
            }

            public override bool Contains(TType value)
            {
                return mList.ContainsKey(value);
            }

            public IEnumerator<TType> GetEnumerator()
            {
                return mList.Keys.GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return mList.Keys.GetEnumerator();
            }
        }

        public class Item : ValueSettingOption<TType>
        {
            public Item()
            { }
            public Item(ListedSettingOption<TType, TTarget> parent, TType value)
                : base(value, parent.GetLocalizedValue(value), parent.GetList().Contains(value) ? 1 : 0)
            { }

            public override string DisplayKey
            {
                get { return "Boolean"; }
            }
        }
    }
}
