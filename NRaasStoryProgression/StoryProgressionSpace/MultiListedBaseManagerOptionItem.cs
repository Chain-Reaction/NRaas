using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace
{
    public abstract class MultiListedBaseManagerOptionItem<TManager, TType> : ListedManagerOptionItem<TManager, TType, List<TType>>, IInstallable<TManager>
        where TManager : StoryProgressionObject
    {
        public MultiListedBaseManagerOptionItem(TType[] value)
            : base(new List<TType>(value), new List<TType>(value))
        { }
        public MultiListedBaseManagerOptionItem(List<TType> value)
            : base(new List<TType>(value), new List<TType>(value))
        { }

        protected override int NumSelectable
        {
            get { return -1; }
        }

        protected override string GetLocalizationValueKey()
        {
            return null;
        }

        public void AddValue(TType value)
        {
            if (Value.Contains(value)) return;

            Value.Add(value);

            Persist();
        }

        public void RemoveValue(TType value)
        {
            Value.Remove(value);

            Persist();
        }

        protected virtual string PersistLookup(string value)
        {
            return value;
        }

        protected abstract bool PersistCreate(ref TType defValue, string value);

        public sealed override object PersistValue
        {
            get
            {
                return new ListToString<TType>().Convert(Value);
            }
            set
            {
                if (value is string)
                {
                    SetValue(new ToList(this).Convert(value as string));
                }
                else
                {
                    List<TType> list = value as List<TType>;
                    if (list != null)
                    {
                        SetValue(list);
                    }
                    else
                    {
                        SetValue(new List<TType>());
                    }
                }
            }
        }

        protected override List<TType> ConvertToList(List<TType> list)
        {
            if (list != null)
            {
                return list;
            }
            else
            {
                return new List<TType>();
            }
        }

        protected override bool PrivatePerform(List<TType> values)
        {
            foreach (TType value in values)
            {
                if (Value.Contains(value))
                {
                    Value.Remove(value);
                }
                else
                {
                    Value.Add(value);
                }
            }

            return true;
        }

        public class ToList : StringToList<TType>
        {
            MultiListedBaseManagerOptionItem<TManager, TType> mOption;

            Dictionary<string, TType> mLookup = new Dictionary<string, TType>();
                
            public ToList(MultiListedBaseManagerOptionItem<TManager, TType> option)
            {
                mOption = option;

                foreach (IGenericValueOption<TType> item in mOption.GetAllOptions())
                {
                    string lookupValue = mOption.PersistLookup(item.Value.ToString());
                    if (string.IsNullOrEmpty(lookupValue)) continue;

                    mLookup.Add(lookupValue, item.Value);
                }
            }

            protected override bool PrivateConvert(string value, out TType result)
            {
                string lookupValue = mOption.PersistLookup(value);

                if ((string.IsNullOrEmpty(lookupValue)) || (!mLookup.TryGetValue(lookupValue, out result)))
                {
                    result = default(TType);
                }

                try
                {
                    if (mOption.PersistCreate(ref result, value)) return true;
                }
                catch (Exception e)
                {
                    Common.Exception("Option: " + mOption.Name + Common.NewLine + "Value: " + value, e);
                }

                return false;
            }
        }
    }
}
