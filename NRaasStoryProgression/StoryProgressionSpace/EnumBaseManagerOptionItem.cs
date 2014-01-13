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
    public abstract class EnumBaseManagerOptionItem<TManager,TType> : ListedManagerOptionItem<TManager, TType, TType>, IGenericAddOption<TType>
        where TManager : StoryProgressionObject
    {
        public EnumBaseManagerOptionItem(TType value)
            : this(value, value)
        { }
        public EnumBaseManagerOptionItem(TType value, TType defValue)
            : base(value, defValue)
        { }

        protected abstract TType Convert(int value);

        protected abstract TType Combine(TType original, TType add, out bool same);

        public TType AddValue(TType value)
        {
            bool same = false;
            TType result = Combine(Value, value, out same);
            if (same) return result;

            SetValue(result);
            return result;
        }

        public void RemoveValue(TType value)
        {
            List<TType> values = new List<TType>(Lookup.Keys);
            values.Remove(value);

            bool same = false;

            TType result = default(TType);
            foreach (TType val in values)
            {
                result = Combine(result, val, out same);
            }

            SetValue(result);
        }

        protected override int NumSelectable
        {
            get { return 1; }
        }

        protected override List<TType> ConvertToList(TType value)
        {
            List<TType> results = new List<TType>();

            if (NumSelectable == 1)
            {
                results.Add(value);
            }
            else
            {
                foreach (TType val in Enum.GetValues(typeof(TType)))
                {
                    if (!Allow(val)) continue;

                    bool same = false;
                    Combine(value, val, out same);
                    if (same)
                    {
                        results.Add(val);
                    }
                }
            }

            return results;
        }

        protected virtual bool Allow(TType value)
        {
            return true;
        }

        protected override List<IGenericValueOption<TType>> GetAllOptions()
        {
            List<IGenericValueOption<TType>> allOptions = new List<IGenericValueOption<TType>>();

            foreach (TType value in Enum.GetValues(typeof(TType)))
            {
                if (!Allow(value)) continue;

                allOptions.Add(new ListItem(this, value));
            }

            return allOptions;
        }

        public sealed override object PersistValue
        {
            get
            {
                return (int)base.PersistValue;
            }
            set
            {
                if (value is string)
                {
                    int newValue;
                    if (!int.TryParse(value as string, out newValue)) return;

                    SetValue(Convert(newValue));
                }
                else
                {
                    SetValue(unchecked((TType)value));
                }
            }
        }

        protected override bool PrivatePerform(List<TType> values)
        {
            if ((NumSelectable == 1) && (values.Count == 1))
            {
                SetValue(values[0]);
            }
            else
            {
                foreach (TType value in values)
                {
                    if (Contains(value))
                    {
                        RemoveValue(value);
                    }
                    else
                    {
                        AddValue(value);
                    }
                }
            }

            return true;
        }

        public class ListItem : BaseListItem<EnumBaseManagerOptionItem<TManager, TType>>
        {
            public ListItem(EnumBaseManagerOptionItem<TManager, TType> option, TType value)
                : base(option, value)
            { }
        }
    }
}
