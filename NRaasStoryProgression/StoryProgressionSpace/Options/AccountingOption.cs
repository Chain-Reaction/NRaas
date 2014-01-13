using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
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

namespace NRaas.StoryProgressionSpace.Options
{
    [Persistable]
    public class AccountingData
    {
        [Persistable]
        public class Value
        {
            string mKey;

            int mValue;

            public Value() // required for persistence
            { }
            public Value(string key, int value)
            {
                mKey = key;
                mValue = value;
            }

            public string Key
            {
                get { return mKey; }
            }

            public int Amount
            {
                get { return mValue; }
            }

            public void Add(int value)
            {
                mValue += value;
            }

            public override string ToString()
            {
                return StoryProgression.Localize("Accounting:Body", false, new object[] { StoryProgression.Localize("AccountingValue:" + mKey), mValue });
            }

            public static int SortByName(Value a, Value b)
            {
                return a.mKey.CompareTo(b.mKey);
            }
        }

        long mStartDay;

        List<Value> mValues = new List<Value>();

        [Persistable(false)]
        Dictionary<string, Value> mLookup = null;

        public AccountingData() // required for persistence
        {
            mStartDay = SimClock.ElapsedCalendarDays();
        }

        protected Dictionary<string, Value> Lookup
        {
            get
            {
                if (mLookup == null)
                {
                    mLookup = new Dictionary<string, Value>();

                    foreach (Value value in mValues)
                    {
                        mLookup.Add(value.Key, value);
                    }
                }
                return mLookup;
            }
        }

        public int Count
        {
            get { return mValues.Count; }
        }

        public int Get(string key)
        {
            Value value;
            if (!Lookup.TryGetValue(key, out value)) return 0;

            return value.Amount;
        }

        public void Add(string key, int delta)
        {
            Add(key, delta, false);
        }
        private void Add(string key, int delta, bool recursion)
        {
            if (delta == 0) return;

            if (string.IsNullOrEmpty(key))
            {
                if (delta > 0)
                {
                    key = "Income";
                }
                else
                {
                    key = "Expense";
                }
            }

            Value value;
            if (!Lookup.TryGetValue(key, out value))
            {
                value = new Value(key, delta);

                mValues.Add(value);

                mLookup.Add(key, value);
            }
            else
            {
                value.Add(delta);
            }

            if (!recursion)
            {
                if (key == "Income")
                {
                    if (value.Amount < 0)
                    {
                        int amount = value.Amount;

                        value.Add(-amount);

                        Add("Expense", amount, true);
                    }
                }
                else if (key == "Expense")
                {
                    if (value.Amount > 0)
                    {
                        int amount = value.Amount;

                        value.Add(-amount);

                        Add("Income", amount, true);
                    }
                }
            }
        }

        public override string ToString()
        {
            Common.StringBuilder text = new Common.StringBuilder();

            text += StoryProgression.Localize("Accounting:Header", false, new object[] { SimClock.ElapsedCalendarDays() - mStartDay });

            mValues.Sort(new Comparison<Value>(Value.SortByName));

            string result = null;

            int net = 0;

            foreach (Value value in mValues)
            {
                if (value.Amount > 0)
                {
                    result += value.ToString();

                    net += value.Amount;
                }
            }

            if (!string.IsNullOrEmpty(result))
            {
                text += Common.NewLine + Common.NewLine + StoryProgression.Localize("Accounting:Gain");
                text += result;
            }

            result = null;

            foreach (Value value in mValues)
            {
                if (value.Amount < 0)
                {
                    result += value.ToString();

                    net += value.Amount;
                }
            }

            if (!string.IsNullOrEmpty(result))
            {
                text += Common.NewLine + Common.NewLine + StoryProgression.Localize("Accounting:Loss");
                text += result;
            }

            text += Common.NewLine + Common.NewLine + StoryProgression.Localize("Accounting:Net", false, new object[] { net });

            return text.ToString();
        }
    }

    public class AcountingOption : GenericOptionBase.GenericOption<AccountingData>, IReadHouseLevelOption, IWriteHouseLevelOption, INotGlobalLevelOption, INotExportableOption, INotCasteLevelOption
    {
        public AcountingOption()
            : base(null,null)
        { }

        public override string GetTitlePrefix()
        {
            return "Accounting";
        }

        protected override string GetLocalizationValueKey()
        {
            return null;
        }

        public override string GetUIValue(bool pure)
        {
            int count = 0;
            if (Value != null)
            {
                count = Value.Count;
            }

            return EAText.GetNumberString(count);
        }

        public override object PersistValue
        {
            set
            {
                SetValue (value as AccountingData);
            }
        }

        public override void ApplyOverride(IGenericLevelOption paramOption, OverrideStyle style)
        {
            if ((style & OverrideStyle.MergeSet) == OverrideStyle.MergeSet)
            {
                AccountingData value = Value;
                if (value == null) return;

                string result = Manager.House.Name;

                result += Common.NewLine + value.ToString();

                int assets = GetValue<AssetsOption, int>();
                int debt = GetValue<DebtOption, int>();

                result += Common.NewLine + StoryProgression.Localize("Accounting:Assets", false, new object[] { assets });
                result += Common.NewLine + StoryProgression.Localize("Accounting:Debts", false, new object[] { debt });

                Common.Notify(result);
            }
        }

        protected override bool PrivatePerform()
        {
            // ApplyOverride does all the work
            return true;
        }
    }
}

