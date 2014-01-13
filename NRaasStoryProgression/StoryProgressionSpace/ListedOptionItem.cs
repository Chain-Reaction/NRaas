using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace
{
    public abstract class ListedOptionItem<TRawType, TStorageType> : GenericOptionItem<TStorageType>, IGenericHasOption<TRawType>
    {
        Dictionary<TRawType, bool> mLookup = null;

        public ListedOptionItem(TStorageType value, TStorageType defValue)
            : base(value, defValue)
        { }

        protected abstract int NumSelectable
        {
            get;
        }

        protected virtual string GetLocalizationUIKey()
        {
            return GetLocalizationValueKey();
        }

        public virtual string ValuePrefix
        {
            get { return "YesNo"; }
        }

        // This stops the options window from displaying the "Count" column
        public override bool UsingCount
        {
            get { return false; }
        }

        public override int Count
        {
            get
            {
                return Lookup.Count;
            }
        }

        protected Dictionary<TRawType, bool> Lookup
        {
            get
            {
                if (mLookup == null)
                {
                    mLookup = new Dictionary<TRawType, bool>();

                    foreach (TRawType item in ConvertToList(Value))
                    {
                        mLookup[item] = true;
                    }
                }

                return mLookup;
            }
        }

        public bool Contains(TRawType value)
        {
            return Lookup.ContainsKey(value);
        }

        protected abstract List<TRawType> ConvertToList(TStorageType list);

        protected virtual string LocalizeValue(TRawType value)
        {
            return LocalizeValue(value.ToString());
        }

        protected virtual string ConvertToUIValue(string prefixKey, TStorageType list)
        {
            return new Converter(prefixKey, IsFemaleLocalization()).Convert(ConvertToList(list));
        }

        public override string GetUIValue(bool pure)
        {
            if (NumSelectable == 1)
            {
                return base.GetUIValue(pure);
            }
            else if (string.IsNullOrEmpty(GetLocalizationUIKey()))
            {
                return EAText.GetNumberString(Lookup.Count);
            }
            else 
            {
                string text = ConvertToUIValue(GetLocalizationUIKey(), Value);

                if ((!pure) && (!string.IsNullOrEmpty(text)))
                {
                    if (text == ConvertToUIValue(GetLocalizationUIKey(), Default))
                    {
                        text = "(" + text + ")";
                    }
                }

                return text;
            }
        }

        public override bool Persist()
        {
            mLookup = null;

            return base.Persist();
        }

        public override void SetValue(TStorageType value, bool persist)
        {
            mLookup = null;

            base.SetValue(value, persist);
        }

        protected abstract bool PrivatePerform(List<TRawType> value);

        protected abstract List<IGenericValueOption<TRawType>> GetAllOptions();

        protected override bool PrivatePerform()
        {
            List<IGenericValueOption<TRawType>> allOptions = GetAllOptions();
            if ((allOptions == null) || (allOptions.Count == 0)) return false;

            bool okayed = false;
            List<IGenericValueOption<TRawType>> selection = ListOptions(allOptions, Name, NumSelectable, out okayed);
            if ((selection == null) || (selection.Count == 0)) return false;

            List<TRawType> choices = new List<TRawType>();

            foreach (IGenericValueOption<TRawType> item in selection)
            {
                choices.Add(item.Value);
            }

            return PrivatePerform(choices);
        }

        public class Converter : ListToString<TRawType>
        {
            string mPrefixKey;
            bool mIsFemale;

            public Converter(string prefixKey, bool isFemale)
            {
                mPrefixKey = prefixKey;
                mIsFemale = isFemale;
            }

            protected override string PrivateConvert(TRawType value)
            {
                return Common.Localize(mPrefixKey + ":" + value, mIsFemale);
            }
        }

        public abstract class BaseListItem<TOption> : ValueSettingOption<TRawType>, IGenericValueOption<TRawType>
            where TOption : ListedOptionItem<TRawType, TStorageType>
        {
            protected readonly TOption mList;

            public BaseListItem(TOption list, TRawType value)
            {
                mList = list;
                mValue = value;
            }

            public override string DisplayKey
            {
                get
                {
                    return mList.ValuePrefix;
                }
            }

            public override string Name
            {
                get
                {
                    return mList.LocalizeValue(mValue);
                }
            }

            public override string DisplayValue
            {
                get
                {
                    return Common.Localize(mList.ValuePrefix + ":" + mList.Contains(mValue));
                }
            }
        }
    }
}
