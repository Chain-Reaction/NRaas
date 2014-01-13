using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Selection
{
    public abstract class ProtoSelection<T>
        where T : class
    {
        List<ObjectPickerDialogEx.CommonHeaderInfo<T>> mColumns = new List<ObjectPickerDialogEx.CommonHeaderInfo<T>>();

        protected ICollection<T> mItems;

        protected string mTitle;

        protected string mSubTitle;

        protected bool mAllOnNull;

        protected bool mHadResults = true;

        public ProtoSelection(string title, string subTitle, ICollection<T> items)
        {
            mTitle = title;
            mSubTitle = subTitle;
            mItems = items;
        }

        public string Title
        {
            get { return mTitle; }
        }

        public ICollection<T> All
        {
            get { return mItems; }
        }

        public bool AllOnNull
        {
            get { return mAllOnNull; }
            set { mAllOnNull = value; }
        }

        public U GetColumn<U>()
            where U : ObjectPickerDialogEx.CommonHeaderInfo<T>
        {
            foreach (ObjectPickerDialogEx.CommonHeaderInfo<T> column in mColumns)
            {
                if (column is U) return (column as U);
            }

            return null;
        }

        public void ReplaceColumn(ObjectPickerDialogEx.CommonHeaderInfo<T> oldColumn, ObjectPickerDialogEx.CommonHeaderInfo<T> newColumn)
        {
            for (int i = 0; i < mColumns.Count; i++)
            {
                if (mColumns[i] == oldColumn)
                {
                    mColumns[i] = newColumn;
                    break;
                }
            }
        }

        protected virtual List<ObjectPicker.TabInfo> GetTabInfo(out List<ObjectPicker.RowInfo> preSelectedRows)
        {
            preSelectedRows = null;

            string subTitle = mSubTitle;
            if (string.IsNullOrEmpty(subTitle))
            {
                subTitle = Common.LocalizeEAString("Ui/Caption/ObjectPicker:All");
            }

            List<ObjectPicker.TabInfo> tabInfo = new List<ObjectPicker.TabInfo>();
            tabInfo.Add(new ObjectPicker.TabInfo("shop_all_r2", subTitle, GetRowInfo()));
            return tabInfo;
        }

        protected void ClearAllColumns()
        {
            mColumns.Clear();
        }

        protected void AddColumn(ObjectPickerDialogEx.CommonHeaderInfo<T> column)
        {
            mColumns.Add(column);
        }

        protected virtual bool AllowRow(T item)
        {
            return true;
        }

        public List<ObjectPicker.RowInfo> GetRowInfo()
        {
            List<ObjectPicker.RowInfo> results = new List<ObjectPicker.RowInfo>();
            if (mItems != null)
            {
                foreach (T item in mItems)
                {
                    if (!AllowRow(item)) continue;

                    results.Add(new ObjectPicker.RowInfo(item, new List<ObjectPicker.ColumnInfo>()));
                }
            }

            return results;
        }

        public T SelectSingle()
        {
            bool okayed = false;
            return SelectSingle(out okayed);
        }
        public T SelectSingle(out bool okayed)
        {
            Results results = SelectMultiple(1);

            okayed = results.mOkayed;

            if (results.Count == 0) return null;

            foreach (T item in results)
            {
                return item;
            }

            return null;
        }
        public Results SelectMultiple()
        {
            return SelectMultiple(0);
        }
        public Results SelectMultiple(int maxSelection)
        {
            List<ObjectPicker.RowInfo> preSelectedRows = null;
            List<ObjectPicker.TabInfo> tabInfo = GetTabInfo(out preSelectedRows);
            if (tabInfo == null)
            {
                mHadResults = false;

                return new Results(false, false, null, AllOnNull, All);
            }

            int count = tabInfo[0].RowInfo.Count;
            if (count == 0)
            {
                mHadResults = false;

                return new Results(false, false, null, AllOnNull, All);
            }

            mHadResults = true;

            if (maxSelection <= 0)
            {
                maxSelection = count;
            }

            bool okayed = false;

            List<T> list = ObjectPickerDialogEx.Show(mTitle, tabInfo, mColumns, maxSelection, preSelectedRows, out okayed);
            if ((list == null) || (list.Count == 0))
            {
                return new Results(false, okayed, null, AllOnNull, All);
            }

            bool foundNull = false;

            List<T> results = new List<T>();

            foreach (T info in list)
            {
                if (info == null)
                {
                    foundNull = true;
                }
                else
                {
                    results.Add(info);
                }
            }

            return new Results(foundNull, okayed, results, AllOnNull, All);
        }

        public class Results : IEnumerable<T>
        {
            List<T> mList = new List<T>();

            public readonly bool mOkayed;

            public readonly bool mWasAll;

            public Results(bool foundNull, bool okayed, List<T> results, bool allOnNull, ICollection<T> fullList)
            {
                mOkayed = okayed;

                if (foundNull)
                {
                    if (allOnNull)
                    {
                        mList.AddRange(fullList);

                        mList.Remove(null);

                        mWasAll = true;
                    }
                    else
                    {
                        mList.AddRange(results);
                    }
                }
                else if (results != null)
                {
                    mList.AddRange(results);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return mList.GetEnumerator();
            }
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return mList.GetEnumerator();
            }

            public int Count 
            {
                get { return mList.Count; }
            }
        }
    }
}

