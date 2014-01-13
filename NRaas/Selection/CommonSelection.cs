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
using System.Collections.Generic;

namespace NRaas.CommonSpace.Selection
{
    public class CommonSelection<T> : ProtoSelection<T>
        where T : class, ICommonOptionItem
    {
        public CommonSelection(string title, ICollection<T> items)
            : this(title, null, items, null)
        { }
        public CommonSelection(string title, string subTitle, ICollection<T> items)
            : this(title, subTitle, items, null)
        { }
        public CommonSelection(string title, ICollection<T> items, ObjectPickerDialogEx.CommonHeaderInfo<T> auxillary)
            : this(title, null, items, auxillary)
        { }
        public CommonSelection(string title, string subTitle, ICollection<T> items, ObjectPickerDialogEx.CommonHeaderInfo<T> auxillary)
            : this(title, subTitle, items, 0, auxillary)
        { }
        public CommonSelection(string title, string subTitle, ICollection<T> items, int widthDelta, ObjectPickerDialogEx.CommonHeaderInfo<T> auxillary)
            : base(title, subTitle, items)
        {
            int valueWidth = 40;

            bool hasSize = false;

            int digits = 1;

            bool useThumbnail = false, useCount = false, useValue = false, useBoolean = false, dontUseBoolean = false;
            foreach (T item in items)
            {
                if (item == null)
                {
                    AllOnNull = true;
                    continue;
                }

                if (string.IsNullOrEmpty(item.Name)) continue;

                if (item.Thumbnail != ThumbnailKey.kInvalidThumbnailKey)
                {
                    useThumbnail = true;
                }

                if (item.DisplayValue != null)
                {
                    useValue = true;
                }

                if (item.UsingCount)
                {
                    useCount = true;

                    digits = Math.Max(digits, item.Count.ToString().Length);

                    if (item.Count > 1)
                    {
                        dontUseBoolean = true;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.DisplayKey))
                        {
                            useBoolean = true;
                        }
                    }
                }

                if (item.ValueWidth > 0)
                {
                    valueWidth = item.ValueWidth;
                }

                hasSize = true;
            }

            if (dontUseBoolean)
            {
                useBoolean = false;
            }

            if (!hasSize)
            {
                mItems = null;
            }

            int nameWidth = DefaultNameWidth - widthDelta;

            if ((useBoolean) || (useCount) || (useValue))
            {
                nameWidth -= valueWidth;
            }

            if (auxillary != null)
            {
                nameWidth -= auxillary.Width;
            }

            AddColumn(new NameColumn(useThumbnail, nameWidth));

            if (auxillary != null)
            {
                AddColumn(auxillary);
            }

            if (useBoolean)
            {
                AddColumn(new BooleanColumn(valueWidth));
            }
            else if (useCount)
            {
                AddColumn(new CountColumn(digits, valueWidth));
            }
            else if (useValue)
            {
                AddColumn(new ValueColumn(valueWidth));
            }
        }

        public static bool HandleAllOrNothing(Results results)
        {
            if (!results.mWasAll) return false;

            bool allToTrue = false;
            foreach (T item in results)
            {
                if (item.Count == 0)
                {
                    allToTrue = true;
                    break;
                }
            }

            foreach (T item in results)
            {
                if (allToTrue)
                {
                    item.Count = 0;
                }
                else
                {
                    item.Count = 1;
                }
            }

            return true;
        }

        public static int DefaultNameWidth
        {
            get
            {
                string strWidth = Common.Localize("OptionItem:OptionWidth");

                int width;
                if (!int.TryParse(strWidth, out width))
                {
                    width = 440;
                }

                return width;
            }
        }

        public bool IsEmpty
        {
            get { return (mItems == null); }
        }

        public class NameColumn : ObjectPickerDialogEx.CommonHeaderInfo<T>
        {
            public readonly bool mUseThumbnail;

            public NameColumn(bool useThumbnail, int width)
                : this(useThumbnail, width, VersionStamp.sNamespace + ".OptionList:OptionTitle", VersionStamp.sNamespace + ".OptionList:OptionTooltip")
            { }
            protected NameColumn(bool useThumbnail, int width, string titleKey, string tooltipKey)
                : base(titleKey, tooltipKey, width)
            {
                mUseThumbnail = useThumbnail;
            }

            public override ObjectPicker.ColumnInfo GetValue(T item)
            {
                string name = null;
                if (item == null)
                {
                    name = "(" + Common.LocalizeEAString("Ui/Caption/ObjectPicker:All") + ")";
                }
                else
                {
                    name = item.Name;
                }

                if (!string.IsNullOrEmpty(name))
                {
                    name = name.Replace("...", "");
                }

                if ((mUseThumbnail) && (item != null))
                {
                    return new ObjectPicker.ThumbAndTextColumn(item.Thumbnail, name);
                }
                else
                {
                    return new ObjectPicker.TextColumn(name);
                }
            }
        }

        public class BooleanColumn : ObjectPickerDialogEx.CommonHeaderInfo<T>
        {
            public BooleanColumn(int width)
                : base(VersionStamp.sNamespace + ".OptionList:ValueTitle", VersionStamp.sNamespace + ".OptionList:ValueTooltip", width)
            { }

            public override ObjectPicker.ColumnInfo GetValue(T item)
            {
                if (item == null) return null;

                if ((item.Count == 0) || (item.Count == 1))
                {
                    return new ObjectPicker.TextColumn(Common.Localize(item.DisplayKey + ":" + (item.Count != 0)));
                }
                else
                {
                    return new ObjectPicker.TextColumn(item.Count.ToString("D4"));
                }
            }
        }

        public class CountColumn : ObjectPickerDialogEx.CommonHeaderInfo<T>
        {
            int mDigits;

            public CountColumn(int digits, int width)
                : base(VersionStamp.sNamespace + ".OptionList:CountTitle", VersionStamp.sNamespace + ".OptionList:CountTooltip", width)
            {
                mDigits = digits;
            }

            public override ObjectPicker.ColumnInfo GetValue(T item)
            {
                if (item == null) return null;

                return new ObjectPicker.TextColumn(item.Count.ToString("D" + mDigits));
            }
        }

        public class ValueColumn : ObjectPickerDialogEx.CommonHeaderInfo<T>
        {
            public ValueColumn(int width)
                : base(VersionStamp.sNamespace + ".OptionList:ValueTitle", VersionStamp.sNamespace + ".OptionList:ValueTooltip", width)
            { }

            public override ObjectPicker.ColumnInfo GetValue(T item)
            {
                if (item == null) return null;

                return new ObjectPicker.TextColumn(item.DisplayValue);
            }
        }
    }
}

