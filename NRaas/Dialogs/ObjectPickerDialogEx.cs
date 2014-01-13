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

namespace NRaas.CommonSpace.Dialogs
{
    public class ObjectPickerDialogEx : ModalDialog
    {
        Button mOkayButton;
        List<ObjectPicker.RowInfo> mResult;
        ObjectPicker mTable;

        Vector2 mTableOffset;

        bool mWasOkay;

        public ObjectPickerDialogEx(string title, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, int numSelectableRows, List<ObjectPicker.RowInfo> preSelectedRows)
            : base("UiObjectPicker", 1, true, ModalDialog.PauseMode.PauseSimulator, null)
        {
            if (mModalDialogWindow == null) return;

            Rect area = mModalDialogWindow.Area;
            area.mBottomRight.x += 200;
            mModalDialogWindow.Area = area;

            Text caption = mModalDialogWindow.GetChildByID(0x05ef6bd3, false) as Text;
            caption.Caption = title;

            mTable = mModalDialogWindow.GetChildByID(0x05ef6bd0, false) as ObjectPicker;
            mTable.ObjectTable.TableChanged += OnTableChanged;
            mTable.SelectionChanged += OnSelectionChanged;
            mTable.RowSelected += OnSelectionChanged;
            mTable.mViewButton.Visible = false;

            mTable.mTable.mPopulationCompletedCallback += OnComplete;

            area = mTable.Area;
            area.mBottomRight.x += 200;
            mTable.Area = area;

            mOkayButton = mModalDialogWindow.GetChildByID(0x05ef6bd1, false) as Button;
            mOkayButton.TooltipText = Common.LocalizeEAString("Ui/Caption/Global:Accept");
            mOkayButton.Enabled = true;
            mOkayButton.Click += OnOkayButtonClick;
            OkayID = mOkayButton.ID;
            SelectedID = mOkayButton.ID;

            Button closeButton = mModalDialogWindow.GetChildByID(0x05ef6bd2, false) as Button;
            closeButton.TooltipText = Common.LocalizeEAString("Ui/Caption/ObjectPicker:Cancel");
            closeButton.Click += OnCloseButtonClick;
            CancelID = closeButton.ID;

            mTableOffset = (mModalDialogWindow.Area.BottomRight - mModalDialogWindow.Area.TopLeft) - (mTable.Area.BottomRight - mTable.Area.TopLeft);

            mTable.Populate(listObjs, headers, numSelectableRows);
            mTable.mTabs.TabSelect -= mTable.OnTabSelect;
            mTable.mTabs.TabSelect += OnTabSelect;

            mTable.ViewTypeToggle = true;
            mTable.Selected = preSelectedRows;

            ResizeWindow(true);
        }

        private void ResizeWindow(bool center)
        {
            Rect screenSize = mModalDialogWindow.Parent.Area;
            float screenWidth = screenSize.Width;
            float screenHeight = screenSize.Height;

            int maximumRows = (int)screenHeight - (int)(mTableOffset.y * 2);

            maximumRows /= (int)mTable.mTable.RowHeight;

            if (maximumRows > mTable.mTable.NumberRows)
            {
                maximumRows = mTable.mTable.NumberRows;
            }

            mTable.mTable.VisibleRows = (uint)maximumRows;

            mTable.mTable.GridSizeDirty = true;
            mTable.OnPopulationComplete();

            mModalDialogWindow.Area = new Rect(mModalDialogWindow.Area.TopLeft, (mModalDialogWindow.Area.TopLeft + mTable.TableArea.BottomRight) + mTableOffset);

            if (center)
            {
                Rect area = mModalDialogWindow.Area;
                float windowWidth = area.Width;
                float windowHeight = area.Height;

                float x = (float)Math.Round((double)((screenWidth - windowWidth) / 2f));
                float y = (float)Math.Round((double)((screenHeight - windowHeight) / 2f));

                area.Set(x, y, x + windowWidth, y + windowHeight);
                mModalDialogWindow.Area = area;

                Text caption = mModalDialogWindow.GetChildByID(0x05ef6bd3, false) as Text;

                Rect captionArea = caption.Area;
                captionArea.Set(captionArea.TopLeft.x, 20, captionArea.BottomRight.x, (30 + 20) - area.Height);
                caption.Area = captionArea;

                mModalDialogWindow.Visible = true;
            }
        }

        private void OnTabSelect(TabControl oldTab, TabControl newTab)
        {
            try
            {
                int tag = (int)newTab.Tag;
                if (mTable.mSortedTab != tag)
                {
                    mTable.mSortedTab = tag;
                    mTable.mSortText.Caption = mTable.mItems[mTable.mSortedTab].TabText;
                    mTable.mTable.mPopulationCompletedCallback += mTable.OnPopulationComplete;
                    mTable.mTable.mPopulationCompletedCallback += OnComplete;
                    if (mTable.RepopulateTable())
                    {
                        OnComplete();
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnTabSelect", e);
            }
        }

        public void OnComplete()
        {
            try
            {
                ResizeWindow(true);
            }
            catch (Exception e)
            {
                Common.Exception("OnComplete", e);
            }
        }

        private void OnCloseButtonClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            eventArgs.Handled = true;
            EndDialog(CancelID);
        }

        public override bool OnEnd(uint endID)
        {
            if (endID == OkayID)
            {
                if (!mOkayButton.Enabled)
                {
                    return false;
                }
                mResult = mTable.Selected;
                mWasOkay = true;
            }
            else
            {
                mResult = null;
                mWasOkay = false;
            }
            mTable.Populate(null, null, 0x0);
            return true;
        }

        private void OnOkayButtonClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            eventArgs.Handled = true;
            EndDialog(OkayID);
        }

        private void OnSelectionChanged(List<ObjectPicker.RowInfo> selectedRows)
        {
            Audio.StartSound("ui_tertiary_button");
            OnTableChanged();
        }

        private void OnTableChanged()
        {
            int selectedItem = mTable.ObjectTable.SelectedItem;
            if ((selectedItem >= 0x0) && (mTable.ObjectTable.GetRow(selectedItem) != null))
            {
                if (mTable.mTable.NumSelectableRows == 1)
                {
                    EndDialog(OkayID);
                }
            }
        }

        public List<ObjectPicker.RowInfo> Result
        {
            get
            {
                return mResult;
            }
        }

        public static List<T> Show<T>(string title, List<ObjectPicker.TabInfo> listObjs, List<CommonHeaderInfo<T>> headers, int numSelectableRows, out bool okayed)
            where T : class
        {
            List<ObjectPicker.RowInfo> preSelectedRows = null;

            return Show(title, listObjs, headers, numSelectableRows, preSelectedRows, out okayed);
        }
        public static List<T> Show<T>(string title, List<ObjectPicker.TabInfo> tabInfo, List<CommonHeaderInfo<T>> paramHeaders, int numSelectableRows, List<ObjectPicker.RowInfo> preSelectedRows, out bool okayed)
            where T : class
        {
            SpeedTrap.Sleep();

            Dictionary<List<ObjectPicker.ColumnInfo>, bool> columns = new Dictionary<List<ObjectPicker.ColumnInfo>, bool>();

            foreach (ObjectPicker.TabInfo info in tabInfo)
            {
                foreach (ObjectPicker.RowInfo row in info.RowInfo)
                {
                    if (columns.ContainsKey(row.ColumnInfo)) continue;
                    columns.Add(row.ColumnInfo, true);

                    foreach (CommonHeaderInfo<T> column in paramHeaders)
                    {
                        ObjectPicker.ColumnInfo cell = column.GetValue(row.Item as T);
                        if (cell == null)
                        {
                            if (column.IsStub) continue;

                            cell = new ObjectPicker.TextColumn(""); 
                        }

                        row.ColumnInfo.Add(cell);
                    }
                }
            }

            List<ObjectPicker.HeaderInfo> headers = new List<ObjectPicker.HeaderInfo>();
            foreach (CommonHeaderInfo<T> header in paramHeaders)
            {
                headers.Add(header);
            }

            return Show<T>(title, tabInfo, headers, numSelectableRows, preSelectedRows, out okayed);
        }
        public static List<T> Show<T>(string title, List<ObjectPicker.TabInfo> tabInfo, List<ObjectPicker.HeaderInfo> headers, int numSelectableRows, List<ObjectPicker.RowInfo> preSelectedRows, out bool okayed)
            where T : class
        {
            using (ObjectPickerDialogEx dialog = new ObjectPickerDialogEx(title, tabInfo, headers, numSelectableRows, preSelectedRows))
            {
                dialog.StartModal();

                okayed = dialog.mWasOkay;

                if ((dialog.Result == null) || (dialog.Result.Count == 0))
                {
                    return null;
                }

                List<T> results = new List<T>();
                foreach (ObjectPicker.RowInfo row in dialog.Result)
                {
                    results.Add(row.Item as T);
                }

                return results;
            }
        }

        public abstract class CommonHeaderInfo<T> : ObjectPicker.HeaderInfo
            where T : class
        {
            public CommonHeaderInfo(string headerKey, string toolTipKey, int width)
                : base(headerKey, toolTipKey, width)
            { }
            public CommonHeaderInfo(string headerKey, string toolTipKey, int width, bool textIsImage)
                : base(headerKey, toolTipKey, width, textIsImage)
            { }

            public virtual bool IsStub
            {
                get { return false; }
            }

            public abstract ObjectPicker.ColumnInfo GetValue(T item);
        }
    }
}

