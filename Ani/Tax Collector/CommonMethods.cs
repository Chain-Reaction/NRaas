using Sims3.Gameplay.Utilities;
using Sims3.UI;
using System.Collections.Generic;
using Sims3.UI.CAS;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.RealEstate;
using System;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Electronics.ani_taxcollector;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Objects.Decorations.Mimics.ani_DonationBox;

namespace ani_taxcollector
{
    public class CommonMethodsTaxCollector
    {
        public static string MenuAni = "ani_";
        public static string MenuSettingsPath = "SettingsMenu";
        public static string MenuTaxPath = "TaxMenu";
        public static string MenuWithdrawalPath = "WithdrawalMenu";
        public static string MenuBuildingPath = "BuildingMenu";

        #region Localization
        /// <summary>
        /// Localization
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string LocalizeString(string name, params object[] parameters)
        {
            return Localization.LocalizeString("ani_TaxCollector:" + name, parameters);
        }
        #endregion Localization

        #region PrintMessage
        /// <summary>
        /// Print message on screen
        /// </summary>
        /// <param name="message"></param>
        public static void PrintMessage(string message)
        {
            StyledNotification.Show(new StyledNotification.Format(message, StyledNotification.NotificationStyle.kGameMessagePositive));
        }
        #endregion

        #region Show Dialogue
        /// <summary>
        /// Show a dialogue
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static string ShowDialogue(string title, string description, string defaultText)
        {
            return StringInputDialog.Show(title, description, defaultText, StringInputDialog.Validation.NoneAllowEmptyOK);
        }

        public static string ShowDialogueNumbersOnly(string title, string description, string defaultText)
        {
            return StringInputDialog.Show(title, description, defaultText, true);
        }
        #endregion Show Dialogue

        #region Show YesNo Dialog
        public static bool ShowConfirmationDialog(string message)
        {
            return AcceptCancelDialog.Show(message);
        }
        #endregion Show YesNo Dialog

        #region Show Sim Selector
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interactionName"></param>
        /// <returns></returns>
        public static List<IMiniSimDescription> ShowSimSelector(Sim actor, Household currentHousehold, string interactionName)
        {
            List<IMiniSimDescription> residents = new List<IMiniSimDescription>();
            string buttonFalse = Localization.LocalizeString("Ui/Caption/ObjectPicker:Cancel", new object[0]);

            List<PhoneSimPicker.SimPickerInfo> list = new List<PhoneSimPicker.SimPickerInfo>();

            List<object> list2;

            //Create list of sims
            foreach (Sim s in currentHousehold.Sims)
            {
                list.Add(Phone.Call.CreateBasicPickerInfo(actor.SimDescription, s.SimDescription));
            }

            list2 = PhoneSimPicker.Show(true, ModalDialog.PauseMode.PauseSimulator, list, interactionName, interactionName, buttonFalse, currentHousehold.Sims.Count, false);

            if (list2 == null || list2.Count == 0)
            {
                return null;
            }
            foreach (var item in list2)
            {
                residents.Add(item as SimDescription);
            }

            return residents;
        }
        #endregion Show Sim Selector

        #region Calculate TAX
        /// <summary>
        /// Calculate how much tax should the household pay
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public static int CalculateTax(Household h, float multiplyer)
        {
            int num = (int)((float)h.ComputeNetWorthOfObjectsInHousehold(true) * Mailbox.kPercentageOfWealthBilled);
            int valueOfAllVacationHomes = h.RealEstateManager.GetValueOfAllVacationHomes();
            num += (int)Math.Round((double)((float)valueOfAllVacationHomes * RealEstateManager.kPercentageOfVacationHomeValueBilled));

            num = (int)(num * multiplyer);

            return num;
        }
        #endregion Caclulate TAX


        #region Return Household ObjectPicker.RowInfo

        public static List<ObjectPicker.RowInfo> ReturnHouseholdsAsRowInfo(List<Household> households)
        {
            List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();

            foreach (Household h in households)
            {
                if (h.InWorld && h.Sims != null && h.Sims.Count > 0)
                {
                    List<ObjectPicker.ColumnInfo> list4 = new List<ObjectPicker.ColumnInfo>();
                    list4.Add(new ObjectPicker.ThumbAndTextColumn(h.Sims.ToArray()[0].GetThumbnailKey(), h.Name));
                    list4.Add(new ObjectPicker.MoneyColumn(0));
                    ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(h, list4);
                    list.Add(item);
                }
            }

            return list;
        }

        #endregion Return Household ObjectPicker.RowInfo

        #region Return TaxCollector ObjectPicker.RowInfo

        public static List<ObjectPicker.RowInfo> ReturnTaxCollectorsAsRowInfo(List<TaxCollector> collectors)
        {
            List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();

            foreach (TaxCollector t in collectors)
            {
                List<ObjectPicker.ColumnInfo> list4 = new List<ObjectPicker.ColumnInfo>();
                list4.Add(new ObjectPicker.TextColumn(t.info.Name));
                // list4.Add(new ObjectPicker.MoneyColumn(0));
                ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(t, list4);
                list.Add(item);
            }

            return list;
        }

        public static GameObject ReturnTaxCollector(TaxCollector active, List<GameObject> objects)
        {
            ThumbnailKey thumbnail = ThumbnailKey.kInvalidThumbnailKey;
            string text = string.Empty;
            List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();

            foreach (GameObject t in objects)
            {
                List<ObjectPicker.ColumnInfo> list2 = new List<ObjectPicker.ColumnInfo>();

                int num = 0;
                thumbnail = t.GetThumbnailKey();               

                if (t.GetType() == typeof(TaxCollector))
                {                   
                    text = (t as TaxCollector).info.Name;
                    num = (t as TaxCollector).info.Funds;                    
                }

                if (t.GetType() == typeof(DonationBox))
                {
                    text = (t as DonationBox).info.Name;
                    num = (t as DonationBox).info.Funds;
                }

                //common
                list2.Add(new ObjectPicker.ThumbAndTextColumn(thumbnail, text));
                list2.Add(new ObjectPicker.MoneyColumn(num));
                ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(t, list2);
                list.Add(item);
            }


            List<ObjectPicker.HeaderInfo> list3 = new List<ObjectPicker.HeaderInfo>();
            List<ObjectPicker.TabInfo> list4 = new List<ObjectPicker.TabInfo>();
            list3.Add(new ObjectPicker.HeaderInfo(string.Empty, string.Empty, 200));
            list3.Add(new ObjectPicker.HeaderInfo(CommonMethodsTaxCollector.LocalizeString("Funds", new object[0]), CommonMethodsTaxCollector.LocalizeString("Funds", new object[0])));
            list4.Add(new ObjectPicker.TabInfo("coupon", ShoppingRegister.LocalizeString("AvailableConcessionsFoods", new object[0]), list));
            List<ObjectPicker.RowInfo> list5 = TaxCollectorSimpleDialog.Show(CommonMethodsTaxCollector.LocalizeString("TransferToObject", new object[0]), 0, list4, list3, true);
            if (list5 == null || list5.Count != 1)
            {
                return null;
            }
            return (list5[0].Item as GameObject);

            

        }

        #endregion Return TaxCollector ObjectPicker.RowInfo

        #region Return None Taxable Households

        public static List<Household> ReturnNoneTaxableHouseholds(TaxInformation taxed)
        {
            List<Household> noneTaxable = Household.sHouseholdList;

            foreach (Household t in taxed.TaxableHouseholds)
            {
                if (t.LotHome != null && t.LotHome.InWorld)
                {
                    Household h = noneTaxable.Find(delegate(Household h2) { return h2.HouseholdId == t.HouseholdId; });
                    if (h != null)
                        noneTaxable.Remove(t);
                }

            }

            return noneTaxable;
        }

        #endregion

    }
}
