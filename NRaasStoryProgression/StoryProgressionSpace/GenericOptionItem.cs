using NRaas.CommonSpace.Options;
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
    public interface IGenericValueOption<T> : ICommonOptionItem
    {
        T Value
        {
            get;
        }
    }

    public interface IGenericOption<T> : IGenericValueOption<T>, IOptionItem
    {
        void SetValue(T value);
        void SetValue(T value, bool persist);
    }

    public interface IGenericHasOption<T> : IOptionItem
    {
        bool Contains(T value);
    }

    public interface IGenericAddOption<T> : IOptionItem
    {
        T AddValue(T value);
    }

    public interface IGenericRemoveOption<T> : IOptionItem
    {
        void RemoveValue(T value);
    }

    public abstract class GenericOptionItem<T> : OptionItem, IGenericOption<T>, IHasDefaultOption
    {
        private T mValue = default(T);

        private T mDefault = default(T);

        public GenericOptionItem()
        { }
        public GenericOptionItem(T value, T defValue)
        {
            mValue = value;
            mDefault = defValue;
        }

        protected virtual T GetDefaultValue()
        {
            return mDefault;
        }

        public void InitDefaultValue()
        {
            mDefault = GetDefaultValue();
            SetValue(mDefault, false);
        }

        public override string GetUIValue(bool pure)
        {
            T value = Value;

            if (value == null) return "";

            string text = LocalizeValue(value.ToString());

            if ((!pure) && (!string.IsNullOrEmpty(text)))
            {
                if (value.Equals(Default))
                {
                    text = "(" + text + ")";
                }
            }

            return text;
        }

        public override object PersistValue
        {
            get
            {
                return mValue;
            }
        }

        public T Default
        {
            get { return mDefault; }
        }

        public T PureValue
        {
            get
            {
                return mValue;
            }
        }

        public virtual T Value
        {
            get
            {
                return mValue;
            }
        }

        public void SetValue(T value)
        {
            SetValue(value, true);
        }
        public virtual void SetValue(T value, bool persist)
        {
            mValue = value;
            if (persist)
            {
                Persist();
            }
        }
    }
}
