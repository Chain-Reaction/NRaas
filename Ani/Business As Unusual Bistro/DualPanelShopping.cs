using Sims3.UI;
using System.Collections.Generic;
using Sims3.Gameplay.Abstracts;
using Sims3.SimIFace;
using Sims3.Gameplay;
using Sims3.Gameplay.Objects.FoodObjects;

namespace ani_BistroSet 
{
    class DualPanelShopping : ModalDialog
    {
        public float mMultiplyer;

        public class DualPaneSimPickerRowController
        {
            private TableRow mRow;
            private TableContainer mTable;
            private ObjectPicker.RowInfo mInfo;
            private bool mIsEnabled;
            public ObjectPicker.RowInfo Info
            {
                get
                {
                    return this.mInfo;
                }
            }
            public bool Enabled
            {
                get
                {
                    return this.mIsEnabled;
                }
                set
                {
                    this.mIsEnabled = value;
                    foreach (WindowBase current in this.mRow.CellWindows)
                    {
                        current.Enabled = this.mIsEnabled;
                    }
                    foreach (CellController current2 in this.mRow.CellControllers)
                    {
                        TableThumbAndTextController tableThumbAndTextController = current2 as TableThumbAndTextController;
                        if (tableThumbAndTextController != null)
                        {
                            tableThumbAndTextController.Enabled = this.mIsEnabled;
                        }
                    }
                }
            }
            public DualPaneSimPickerRowController(TableRow row, TableContainer table, ObjectPicker.RowInfo info, float multiplyer)
            {
                this.mIsEnabled = true;
                this.mRow = row;
                this.mTable = table;
                this.mInfo = info;
                List<CellController> cellControllers = this.mRow.CellControllers;
                List<WindowBase> cellWindows = this.mRow.CellWindows;
                TableThumbAndTextController tableThumbAndTextController = new TableThumbAndTextController(cellWindows[0]);
                cellControllers.Add(tableThumbAndTextController);
                tableThumbAndTextController.ImageSize = 40f;

                tableThumbAndTextController.Entry = ((Recipe)this.mInfo.Item).GenericName;
                tableThumbAndTextController.Thumbnail = ((Recipe)this.mInfo.Item).GetThumbnailKey();
            }
        }
        private enum ControlIds : uint
        {
            kinviteeTable = 99576784u,
            kOKAY_BUTTON,
            kCANCEL_BUTTON,
            kPossiblesTable,
            kLeftArrow,
            kRightArrow,
            kAllFilter,
            kFriendsFilter,
            kCoworkerFilter,
            kDisabledSourceWin,
            kUnselectedSimsCaption = 239714016u,
            kSelectedSimsCaption = 239714080u
        }
        private const uint kEnabledColor = 4278190080u;
        private const string kLayoutName = "DualPaneSimPicker";
        private const int kWinExportID = 1;
        private List<ObjectPicker.RowInfo> mResult;
        private Window mDisableSourceWin;
        private Button mOkayButton;
        private Button mCloseButton;
        private TableContainer mSourceTable;
        private TableContainer mSelectedTable;
        private Button mLeftArrow;
        private Button mRightArrow;
        private List<ObjectPicker.RowInfo> mSims;
        private List<ObjectPicker.RowInfo> mSelectedSims;
        public List<ObjectPicker.RowInfo> Result
        {
            get
            {
                return this.mResult;
            }
        }
        public static List<ObjectPicker.RowInfo> Show(List<ObjectPicker.RowInfo> items, List<ObjectPicker.RowInfo> selected, string preLocalizedSelectedCaption, string preLocalizedunselectedCaption)
        {         
            List<ObjectPicker.RowInfo> result;
            using (DualPanelShopping dualPaneSimPicker = new DualPanelShopping(items, selected, preLocalizedSelectedCaption, preLocalizedunselectedCaption))
            {
                dualPaneSimPicker.StartModal();
                result = dualPaneSimPicker.Result;
            }
            return result;
        }
        public DualPanelShopping(List<ObjectPicker.RowInfo> items, List<ObjectPicker.RowInfo> selected, string preLocalizedSelectedSims, string preLocalizedUnselectedSims)
            : base("DualPaneSimPicker", 1, true, ModalDialog.PauseMode.PauseSimulator, null)
        {
            mMultiplyer = 1;
            if (this.mModalDialogWindow != null)
            {
                ILocalizationModel arg_2A_0 = Responder.Instance.LocalizationModel;
                this.mSims = items;
                this.mSelectedSims = selected;
                this.mDisableSourceWin = (this.mModalDialogWindow.GetChildByID(99576793u, true) as Window);
                this.mOkayButton = (this.mModalDialogWindow.GetChildByID(99576785u, true) as Button);
                this.mOkayButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnOkayButtonClick);
                this.mCloseButton = (this.mModalDialogWindow.GetChildByID(99576786u, true) as Button);
                this.mCloseButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnCloseButtonClick);
                this.mLeftArrow = (this.mModalDialogWindow.GetChildByID(99576788u, true) as Button);
                this.mLeftArrow.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnArrowClick);
                this.mRightArrow = (this.mModalDialogWindow.GetChildByID(99576789u, true) as Button);
                this.mRightArrow.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnArrowClick);
                this.mSelectedTable = (this.mModalDialogWindow.GetChildByID(99576784u, true) as TableContainer);
                this.mSelectedTable.ItemDoubleClicked += new TableContainer.ItemGridItemClickedEventHandler(this.OnGridDoubleClicked);
                this.mSelectedTable.SelectionChanged += new UIEventHandler<UISelectionChangeEventArgs>(this.OnSelectionChanged);
                this.mSourceTable = (this.mModalDialogWindow.GetChildByID(99576787u, true) as TableContainer);
                this.mSourceTable.ItemDoubleClicked += new TableContainer.ItemGridItemClickedEventHandler(this.OnGridDoubleClicked);
                this.mSourceTable.SelectionChanged += new UIEventHandler<UISelectionChangeEventArgs>(this.OnSelectionChanged);
                Text text = this.mModalDialogWindow.GetChildByID(239714080u, true) as Text;
                text.Caption = preLocalizedSelectedSims;
                Text text2 = this.mModalDialogWindow.GetChildByID(239714016u, true) as Text;
                text2.Caption = preLocalizedUnselectedSims;
                this.mModalDialogWindow.CenterInParent();
                this.mSourceTable.Flush();
                this.mSelectedTable.Flush();
                this.RepopulateSourceTable();
                this.RepopulateSelectedSimTable();
                this.mModalDialogWindow.Visible = true;
                base.SelectedID = 99576785u;
            }
        }
        public override bool OnEnd(uint endID)
        {
            this.mSourceTable.Clear();
            this.mSelectedTable.Clear();
            return true;
        }
        private void RepopulateSourceTable()
        {
            try
            {
                int row = -1;
                int num = -1;
                this.mSourceTable.GetFirstVisibleCell(ref num, ref row);
                this.mSourceTable.Clear();
                foreach (ObjectPicker.RowInfo current in this.mSims)
                {
                    if (!this.mSelectedSims.Contains(current))
                    {
                        TableRow tableRow = this.mSourceTable.CreateRow();
                        DualPanelShopping.DualPaneSimPickerRowController dualPaneSimPickerRowController = new DualPanelShopping.DualPaneSimPickerRowController(tableRow, this.mSourceTable, current, mMultiplyer);
                        tableRow.RowController = dualPaneSimPickerRowController;
                        dualPaneSimPickerRowController.Enabled = true;
                        this.mSourceTable.AddRow(tableRow);
                    }
                }
                this.mSourceTable.ClearSelection();
                this.mRightArrow.Enabled = false;
                this.mSourceTable.Flush();
                this.mSourceTable.ScrollRowToTop(row);
            }
            catch (System.Exception ex)
            {
                CommonMethodsOFBBistroSet.PrintMessage("DualPanelSource: " + ex.Message);
            }
        }
        private void RepopulateSelectedSimTable()
        {
            this.mSelectedTable.Clear();
            new List<ObjectPicker.RowInfo>();
            foreach (ObjectPicker.RowInfo current in this.mSelectedSims)
            {
                TableRow tableRow = this.mSelectedTable.CreateRow();
                DualPanelShopping.DualPaneSimPickerRowController rowController = new DualPanelShopping.DualPaneSimPickerRowController(tableRow, this.mSelectedTable, current, mMultiplyer);
                tableRow.RowController = rowController;
                this.mSelectedTable.AddRow(tableRow);
            }
        }
        private void OnArrowClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            if (sender.ID == 99576788u)
            {
                TableRow row = this.mSelectedTable.GetRow(this.mSelectedTable.SelectedItem);
                DualPanelShopping.DualPaneSimPickerRowController dualPaneSimPickerRowController = row.RowController as DualPanelShopping.DualPaneSimPickerRowController;
                ObjectPicker.RowInfo info = dualPaneSimPickerRowController.Info;
                this.mSelectedSims.Remove(info);
            }
            else
            {
                TableRow row2 = this.mSourceTable.GetRow(this.mSourceTable.SelectedItem);
                DualPanelShopping.DualPaneSimPickerRowController dualPaneSimPickerRowController2 = row2.RowController as DualPanelShopping.DualPaneSimPickerRowController;
                ObjectPicker.RowInfo info2 = dualPaneSimPickerRowController2.Info;
                this.mSelectedSims.Add(info2);
            }
            this.mLeftArrow.Enabled = false;
            this.mRightArrow.Enabled = false;
            this.RepopulateSourceTable();
            this.RepopulateSelectedSimTable();
        }
        private void OnGridDoubleClicked(TableContainer sender, TableRow row)
        {
            bool flag = false;
            if (sender.ID == 99576784u)
            {
                DualPanelShopping.DualPaneSimPickerRowController dualPaneSimPickerRowController = row.RowController as DualPanelShopping.DualPaneSimPickerRowController;
                ObjectPicker.RowInfo info = dualPaneSimPickerRowController.Info;
                this.mSelectedSims.Remove(info);
                flag = true;
            }
            else
            {
                DualPanelShopping.DualPaneSimPickerRowController dualPaneSimPickerRowController2 = row.RowController as DualPanelShopping.DualPaneSimPickerRowController;
                if (dualPaneSimPickerRowController2.Enabled)
                {
                    ObjectPicker.RowInfo info2 = dualPaneSimPickerRowController2.Info;
                    this.mSelectedSims.Add(info2);
                    flag = true;
                }
            }
            if (flag)
            {
                Audio.StartSound("ui_panel_shift");
                this.mLeftArrow.Enabled = false;
                this.mRightArrow.Enabled = false;
                this.RepopulateSourceTable();
                this.RepopulateSelectedSimTable();
            }
        }
        private void OnCloseButtonClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            this.mResult = null;
            eventArgs.Handled = true;
            this.EndDialog(0u);
        }
        private void OnOkayButtonClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            this.mResult = this.mSelectedSims;
            eventArgs.Handled = true;
            this.EndDialog(0u);
        }
        private void OnSelectionChanged(WindowBase sender, UISelectionChangeEventArgs eventArgs)
        {
            if (!(sender == this.mSelectedTable))
            {
                if (this.mSourceTable.SelectedItem >= 0)
                {
                    TableRow row = this.mSourceTable.GetRow(this.mSourceTable.SelectedItem);
                    if (row != null)
                    {
                        DualPanelShopping.DualPaneSimPickerRowController dualPaneSimPickerRowController = row.RowController as DualPanelShopping.DualPaneSimPickerRowController;
                        if (dualPaneSimPickerRowController != null && dualPaneSimPickerRowController.Enabled)
                        {
                            this.mRightArrow.Enabled = true;
                            return;
                        }
                        this.mRightArrow.Enabled = false;
                        return;
                    }
                }
                else
                {
                    this.mRightArrow.Enabled = false;
                }
                return;
            }
            if (this.mSelectedTable.SelectedItem >= 0 && this.mSelectedTable.GetRow(this.mSelectedTable.SelectedItem) != null)
            {
                this.mLeftArrow.Enabled = true;
                return;
            }
            this.mLeftArrow.Enabled = false;
        }
    }
}
