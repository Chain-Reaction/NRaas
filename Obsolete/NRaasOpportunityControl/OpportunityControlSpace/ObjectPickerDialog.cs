using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.OpportunityControlSpace
{
    public class ObjectPickerDialog : ModalDialog
    {
        // Fields
        private const int CANCEL_BUTTON = 0x5ef6bd2;
        private const int ITEM_TABLE = 0x5ef6bd0;
        private const int kWinExportID = 0x1;
        private Button mCloseButton;
        private Button mOkayButton;
        private List<ObjectPicker.RowInfo> mResult;
        private ObjectPicker mTable;
        private Vector2 mTableOffset;
        private const int OKAY_BUTTON = 0x5ef6bd1;
        private const int TITLE_TEXT = 0x5ef6bd3;

        // Methods
        public ObjectPickerDialog(string title, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, int numSelectableRows, List<ObjectPicker.RowInfo> preSelectedRows)
            : base("NRaas.OpportunityControl.UiObjectPicker", 1, true, ModalDialog.PauseMode.PauseSimulator, null)
        {
            if (mModalDialogWindow != null)
            {
                Text childByID = mModalDialogWindow.GetChildByID(0x5ef6bd3, false) as Text;
                childByID.Caption = title;
                mTable = mModalDialogWindow.GetChildByID(0x5ef6bd0, false) as ObjectPicker;
                mTable.ObjectTable.TableChanged += new TableContainer.TableChangedEventHandler(OnTableChanged);
                mTable.SelectionChanged += new ObjectPicker.ObjectPickerSelectionChanged(OnSelectionChanged);
                mTable.RowSelected += new ObjectPicker.ObjectPickerSelectionChanged(OnSelectionChanged);
                mOkayButton = mModalDialogWindow.GetChildByID(0x5ef6bd1, false) as Button;
                mOkayButton.TooltipText = NRaas.OpportunityControl.Localize("Choice:OK");
                mOkayButton.Enabled = false;
                mOkayButton.Click += new UIEventHandler<UIButtonClickEventArgs>(OnOkayButtonClick);
                OkayID = mOkayButton.ID;
                SelectedID = mOkayButton.ID;
                mCloseButton = mModalDialogWindow.GetChildByID(0x5ef6bd2, false) as Button;
                mCloseButton.TooltipText = Localization.LocalizeString("Ui/Caption/ObjectPicker:Cancel", new object[0]);
                mCloseButton.Click += new UIEventHandler<UIButtonClickEventArgs>(OnCloseButtonClick);
                CancelID = mCloseButton.ID;
                mTableOffset = (mModalDialogWindow.Area.BottomRight - mModalDialogWindow.Area.TopLeft) - (mTable.Area.BottomRight - mTable.Area.TopLeft);
                mTable.Populate(listObjs, headers, numSelectableRows);
                mTable.ViewTypeToggle = true;
                mTable.Selected = preSelectedRows;
                mModalDialogWindow.Area = new Rect(mModalDialogWindow.Area.TopLeft, (mModalDialogWindow.Area.TopLeft + mTable.TableArea.BottomRight) + mTableOffset);
                Rect area = mModalDialogWindow.Area;
                float num = area.BottomRight.x - area.TopLeft.x;
                float num2 = area.BottomRight.y - area.TopLeft.y;

                Rect rect2 = mModalDialogWindow.Parent.Area;
                float num5 = rect2.BottomRight.x - rect2.TopLeft.x;
                float num6 = rect2.BottomRight.y - rect2.TopLeft.y;
                float x = (float)Math.Round((double)((num5 - num) / 2f));
                float y = (float)Math.Round((double)((num6 - num2) / 2f));

                area.Set(x, y, x + num, y + num2);
                mModalDialogWindow.Area = area;
                mModalDialogWindow.Visible = true;
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
            }
            else
            {
                mResult = null;
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
                mOkayButton.Enabled = true;

                if (mTable.mTable.NumSelectableRows == 1)
                {
                    EndDialog(OkayID);
                }
            }
            else
            {
                mOkayButton.Enabled = false;
            }
        }

        public static List<ObjectPicker.RowInfo> Show(string title, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, int numSelectableRows)
        {
            using (ObjectPickerDialog dialog = new ObjectPickerDialog(title, listObjs, headers, numSelectableRows, null))
            {
                dialog.StartModal();
                if ((dialog.Result == null) || (dialog.Result.Count == 0))
                {
                    return null;
                }
                return dialog.Result;
            }
        }
        public static List<ObjectPicker.RowInfo> Show(string title, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, int numSelectableRows, List<ObjectPicker.RowInfo> preSelectedRows)
        {
            using (ObjectPickerDialog dialog = new ObjectPickerDialog(title, listObjs, headers, numSelectableRows, preSelectedRows))
            {
                dialog.StartModal();
                if ((dialog.Result == null) || (dialog.Result.Count == 0))
                {
                    return null;
                }
                return dialog.Result;
            }
        }

        // Properties
        public List<ObjectPicker.RowInfo> Result
        {
            get
            {
                return mResult;
            }
        }
    }
}

