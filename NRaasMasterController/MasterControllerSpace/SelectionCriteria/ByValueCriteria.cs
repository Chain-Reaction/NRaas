using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public interface IByValueCriteria
    {
        int GetValue(Household house);

        Household House
        {
            set;
        }

        int Sims
        {
            get;
        }

        int Value
        {
            get;
        }
    }

    public abstract class ByValueCriteria<T> : SelectionOptionBaseList<ByValueCriteria<T>.Item>
        where T : IByValueCriteria, new()
    {
        public override void GetOptions(List<IMiniSimDescription> actors, List<IMiniSimDescription> allSims, List<Item> items)
        {
            List<T> houses = new List<T>();
            foreach (Household house in Household.sHouseholdList)
            {
                if (house.IsSpecialHousehold) continue;

                houses.Add(New(house));
            }

            List<Pair<int, List<T>>> clumps = Clump(houses, MasterController.Settings.mByMoneyIntervals);

            int minimum = 0;
            for (int i=0; i<clumps.Count; i++)
            {
                Pair<int, List<T>> value = clumps[i];

                int count = 0;
                foreach (T item in value.Second)
                {
                    count += item.Sims;
                }

                items.Add(new Item(minimum, value.First, ((i+1) == clumps.Count), count));

                minimum = value.First;
            }
        }

        public static T New(Household house)
        {
            T result = new T();
            result.House = house;
            return result;
        }

        public static string ToString(string key, List<T> values, int intervals)
        {
            string result = null;

            List<Pair<int, List<T>>> byValue = Clump(values, intervals);
            if (byValue != null)
            {
                foreach (Pair<int, List<T>> pair in byValue)
                {
                    int houseCount = pair.Second.Count, residents = 0;
                    foreach (T house in pair.Second)
                    {
                        residents += house.Sims;
                    }

                    result += Common.Localize(key, false, new object[] { pair.First, houseCount, residents });
                }
            }

            return result;
        }

        public static int SortByValue(T a, T b)
        {
            return a.Value.CompareTo(b.Value);
        }

        public static int SortByFirst(Pair<int, List<T>> a, Pair<int, List<T>> b)
        {
            if (a.First > b.First)
            {
                return 1;
            }
            else if (a.First < b.First)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public static int SortBySecond(Pair<int, List<T>> a, Pair<int, List<T>> b)
        {
            if (a.Second.Count > b.Second.Count)
            {
                return -1;
            }
            else if (a.Second.Count < b.Second.Count)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static List<Pair<int, List<T>>> Clump(List<T> houses, int slots)
        {
            if ((houses == null) || (houses.Count == 0)) return null;

            List<Pair<int, List<T>>> clumps = new List<Pair<int, List<T>>>();

            houses.Sort(new Comparison<T>(SortByValue));

            int min = houses[0].Value;

            int max = houses[houses.Count - 1].Value;

            int interval = (max - min) / slots;
            if (interval == 0)
            {
                interval = 1;
            }

            for (int i = 1; i <= slots; i++)
            {
                clumps.Add(new Pair<int, List<T>>(min + i * interval, new List<T>()));
            }

            foreach (T house in houses)
            {
                int index = (house.Value - min) / interval;

                if (index >= slots)
                {
                    index = slots - 1;
                }

                clumps[index].Second.Add(house);
            }

            for (int i = slots - 1; i >= 0; i--)
            {
                if (clumps[i].Second.Count == 0)
                {
                    clumps.RemoveAt(i);
                }
            }

            if (clumps.Count > 0)
            {
                int available = slots - clumps.Count;
                while (available > 0)
                {
                    clumps.Sort(new Comparison<Pair<int, List<T>>>(SortBySecond));

                    Pair<int, List<T>> clump = clumps[0];

                    if ((clump.Second.Count == houses.Count) && (available + 1 == slots))
                    {
                        break;
                    }

                    clumps.RemoveAt(0);

                    List<Pair<int, List<T>>> newClumps = Clump(clump.Second, available + 1);
                    clumps.AddRange(newClumps);

                    if (newClumps.Count <= 1) break;

                    available = slots - clumps.Count;
                }

                clumps.Sort(new Comparison<Pair<int, List<T>>>(SortByFirst));
            }

            return clumps;
        }

        public abstract class BaseClumper : IByValueCriteria
        {
            Household mHouse;

            protected int mValue;

            public Household House
            {
                get
                {
                    return mHouse;
                }
                set
                {
                    mHouse = value;

                    mValue = GetValue(mHouse);
                }
            }

            public abstract int GetValue(Household house);

            public int Sims
            {
                get { return CommonSpace.Helpers.Households.NumSimsIncludingPregnancy(mHouse); }
            }

            public int Value
            {
                get { return mValue; }
            }
        }

        public class Item : ValueSettingOption<Vector2>, ITestableOption, IPersistence
        {
            bool mFinal;

            public Item()
            { }
            public Item(int minimum, int maximum, bool final, int count)
                : base(new Vector2(minimum, maximum), Common.Localize("Funds:MenuName", false, new object[] { maximum }), count)
            {
                mFinal = final;
            }

            public string OptionName
            {
                get { return string.Empty; }
            }

            public string OptionValue
            {
                get { return string.Empty; }
            }

            public bool CanBeRandomValue
            {
                get { return false; }
                set { }
            }

            public int OptionHitValue
            {
                get { return 0; }
                set { }
            }

            public int OptionMissValue
            {
                get { return 0; }
                set { }
            }

            public int GetScoreValue(IMiniSimDescription me, IMiniSimDescription actor, bool satisfies, int divisior)
            {
                return 0;
            }

            public bool Test(IMiniSimDescription me, bool fullFamily, IMiniSimDescription actor, bool testRandom)
            {
                return Test(me, fullFamily, actor);
            }

            public bool Test(IMiniSimDescription me, bool fullFamily, IMiniSimDescription actor)
            {
                SimDescription trueSim = me as SimDescription;
                if (trueSim == null) return false;

                if (SimTypes.IsSpecial(trueSim)) return false;

                int funds = 0;
                if (trueSim.Household != null)
                {
                    funds = new T().GetValue(trueSim.Household);
                }

                if (funds < Value.x) return false;

                if (mFinal) return true;

                return (funds <= Value.y);
            }

            public void Export(Persistence.Lookup settings)
            {
                settings.ReflectionAdd(this);
            }

            public void Import(Persistence.Lookup settings)
            {
                settings.ReflectionGet(this);
            }

            public string PersistencePrefix
            {
                get { return null; }
            }

            public override string DisplayKey
            {
                get { return "Has"; }
            }
        }
    }
}
