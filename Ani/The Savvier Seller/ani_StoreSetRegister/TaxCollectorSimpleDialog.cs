using Sims3.UI;
using System.Collections.Generic;
using Sims3.SimIFace;
using ani_StoreSetRegister;

namespace ani_StoreSetBase
{ 
    public class TaxCollectorSimpleDialog : ModalDialog
    {
        private const int ITEM_TABLE = 99576784;
        private const int OKAY_BUTTON = 99576785;
        private const int CANCEL_BUTTON = 99576786;
        private const int TITLE_TEXT = 99576787;
        private const int AVAILABLEFUNDS_TEXT = 99576788;
        private const string kLayoutName = "SimplePurchaseDialog";
        private const int kWinExportID = 1;
        private List<ObjectPicker.RowInfo> mResult;
        private Vector2 mTableOffset;
        private ObjectPicker mTable;
        private Button mOkayButton;
        private Button mCloseButton;
        private int mFunds;
        public List<ObjectPicker.RowInfo> Result
        {
            get
            {
                return this.mResult;
            }
        }
        public static List<ObjectPicker.RowInfo> Show(string title, int funds, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers)
        {
            return TaxCollectorSimpleDialog.Show(title, funds, listObjs, headers, false);
        }
        public static List<ObjectPicker.RowInfo> Show(string title, int funds, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, bool viewTypeToggle)
        {
            return TaxCollectorSimpleDialog.Show(title, funds, listObjs, headers, viewTypeToggle, new Vector2(-1f, -1f));
        }
        public static List<ObjectPicker.RowInfo> Show(string title, int funds, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, bool viewTypeToggle, string okayCaption)
        {
            return TaxCollectorSimpleDialog.Show(title, funds, listObjs, headers, viewTypeToggle, new Vector2(-1f, -1f), okayCaption);
        }
        public static List<ObjectPicker.RowInfo> Show(string title, int funds, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, bool viewTypeToggle, Vector2 position)
        {
            return TaxCollectorSimpleDialog.Show(title, funds, listObjs, headers, viewTypeToggle, position, "OK");
        }
        public static List<ObjectPicker.RowInfo> Show(string title, int funds, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, bool viewTypeToggle, Vector2 position, string okayCaption)
        {
            List<ObjectPicker.RowInfo> result;
            using (TaxCollectorSimpleDialog simplePurchaseDialog = new TaxCollectorSimpleDialog(title, funds, listObjs, headers, viewTypeToggle, position))
            {
                if (okayCaption != string.Empty)
                {
                    simplePurchaseDialog.mOkayButton.Caption = okayCaption;
                }
                simplePurchaseDialog.StartModal();
                if (simplePurchaseDialog.Result == null || simplePurchaseDialog.Result.Count == 0)
                {
                    result = null;
                }
                else
                {
                    List<ObjectPicker.RowInfo> result2 = simplePurchaseDialog.Result;
                    result = result2;
                }
            }
            return result;
        }
        public TaxCollectorSimpleDialog(string title, int funds, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, bool viewTypeToggle, Vector2 position)
            : base("SimplePurchaseDialog", 1, true, ModalDialog.PauseMode.PauseSimulator, null)
        {
            if (this.mModalDialogWindow != null)
            {
                Text text = this.mModalDialogWindow.GetChildByID(99576787u, false) as Text;
                text.Caption = title;
                this.mFunds = funds;
                text = (this.mModalDialogWindow.GetChildByID(99576788u, false) as Text);
                text.Caption = Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/Shopping/Cart:AvailableFunds", new object[]
				{
					UIUtils.FormatMoney(funds)
				});
                this.mTable = (this.mModalDialogWindow.GetChildByID(99576784u, false) as ObjectPicker);
                this.mTable.ObjectTable.TableChanged += new TableContainer.TableChangedEventHandler(this.OnTableChanged);
                this.mTable.ObjectTable.SelectionChanged += new UIEventHandler<UISelectionChangeEventArgs>(this.OnSelectionChanged);
                this.mOkayButton = (this.mModalDialogWindow.GetChildByID(99576785u, false) as Button);
                this.mOkayButton.Enabled = false;
                this.mOkayButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnOkayButtonClick);
                this.mOkayButton.Caption = CMStoreSet.LocalizeString("Select", new object[0]);

                base.OkayID = this.mOkayButton.ID;
                base.SelectedID = this.mOkayButton.ID;
                this.mCloseButton = (this.mModalDialogWindow.GetChildByID(99576786u, false) as Button);
                this.mCloseButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnCloseButtonClick);
                base.CancelID = this.mCloseButton.ID;
                this.mTableOffset = this.mModalDialogWindow.Area.BottomRight - this.mModalDialogWindow.Area.TopLeft - (this.mTable.Area.BottomRight - this.mTable.Area.TopLeft);
                this.mTable.Populate(listObjs, headers, 1);
                this.mTable.ViewTypeToggle = viewTypeToggle;
                this.mModalDialogWindow.Area = new Rect(this.mModalDialogWindow.Area.TopLeft, this.mModalDialogWindow.Area.TopLeft + this.mTable.TableArea.BottomRight + this.mTableOffset);
                float x = position.x;
                float y = position.y;
                if (x < 0f && y < 0f)
                {
                    this.mModalDialogWindow.CenterInParent();
                }
                else
                {
                    Rect area = this.mModalDialogWindow.Area;
                    float num = area.BottomRight.x - area.TopLeft.x;
                    float num2 = area.BottomRight.y - area.TopLeft.y;
                    area.Set(x, y, x + num, y + num2);
                    this.mModalDialogWindow.Area = area;
                }
                this.mModalDialogWindow.Visible = true;
            }
        }
        public override bool OnEnd(uint endID)
        {
            if (endID == base.OkayID)
            {
                if (!this.mOkayButton.Enabled)
                {
                    return false;
                }
                this.mResult = this.mTable.Selected;
            }
            else
            {
                this.mResult = null;
            }
            this.mTable.Populate(null, null, 0);
            return true;
        }
        private void OnCloseButtonClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            eventArgs.Handled = true;
            this.EndDialog(base.CancelID);
        }
        private void OnOkayButtonClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            eventArgs.Handled = true;
            this.EndDialog(base.OkayID);
        }
        private void OnSelectionChanged(WindowBase sender, UISelectionChangeEventArgs eventArgs)
        {
            Audio.StartSound("ui_tertiary_button");
            this.OnTableChanged();
        }
        private void OnTableChanged()
        {
            int selectedItem = this.mTable.ObjectTable.SelectedItem;
            if (selectedItem >= 0)
            {
                TableRow row = this.mTable.ObjectTable.GetRow(selectedItem);
                if (row != null)
                {
                    TableMoneyController tableMoneyController = row.CellControllers[1] as TableMoneyController;
                    //this.mOkayButton.Enabled = (tableMoneyController.Number <= this.mFunds);
                    //return;
                }
            }
            this.mOkayButton.Enabled = true;
        }
    }
}
