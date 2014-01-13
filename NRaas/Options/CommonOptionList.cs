using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Options
{
    public interface ICommonOptionListProxy<T>
        where T : class, ICommonOptionItem
    {
        void GetOptions(List<T> items);
    }

    public abstract class CommonOptionList<T> : CommonOptionItem
        where T : class, ICommonOptionItem
    {
        public CommonOptionList()
        { }
        public CommonOptionList(string name)
            : base(name)
        { }

        protected virtual int NumSelectable
        {
            get { return 1; }
        }

        public static int OnNameCompare(T left, T right)
        {
            try
            {
                return left.Name.CompareTo(right.Name);
            }
            catch (Exception e)
            {
                Common.Exception(Common.NewLine + "Left: " + left.GetType() + Common.NewLine + "Right: " + right.GetType(), e);
                return 0;
            }
        }

        public static List<T> AllOptions()
        {
            List<T> items = new List<T>();
            foreach (T item in Common.DerivativeSearch.Find<T>())
            {
                ICommonOptionListProxy<T> proxy = item as ICommonOptionListProxy<T>;
                if (proxy != null)
                {
                    proxy.GetOptions(items);
                }
                else
                {
                    items.Add(item);
                }
            }

            items.Sort(new Comparison<T>(OnNameCompare));

            return items;
        }

        public abstract List<T> GetOptions();
    }
}
