using Sims3.Gameplay.Utilities;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.Gameplay.Actors;
using System.Collections.Generic;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay;

namespace ani_OFBStand
{
    public class CommonMethodsOFBStand
    {
        public static string MenuFoodPath = "Food";
        public static string MenuBusinessPath = "Business";
        public static string MenuSettingsPath = "SettingsMenu";

        #region Localization
        /// <summary>
        /// Localization
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string LocalizeString(string name, params object[] parameters)
        {
            return Localization.LocalizeString("ani_OFBStand:" + name, parameters);
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

        #region Return Sim
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interactionName"></param>
        /// <returns></returns>
        public static SimDescription ReturnSim(Sim actor, bool teenOrAbove, bool residentsOnly)
        {
            string buttonFalse = Localization.LocalizeString("Ui/Caption/ObjectPicker:Cancel", new object[0]);

            List<PhoneSimPicker.SimPickerInfo> list = new List<PhoneSimPicker.SimPickerInfo>();

            List<object> list2;            

            //Create list of sims
            if (residentsOnly)
                foreach (Household h in Household.GetHouseholdsLivingInWorld())
                {
                    foreach (SimDescription sd in h.SimDescriptions)
                    {
                        if (!sd.IsPet && sd.TeenOrAbove)
                        {
                            list.Add(Phone.Call.CreateBasicPickerInfo(actor.SimDescription, sd));
                        }
                    }
                }
            else
            {
                foreach (SimDescription sd in SimDescription.GetAll(SimDescription.Repository.Household))
                {
                    if (!sd.IsPet && sd.TeenOrAbove && sd.IsContactable)
                    {
                        list.Add(Phone.Call.CreateBasicPickerInfo(actor.SimDescription, sd));
                    }
                }
            }           

            list2 = PhoneSimPicker.Show(true, ModalDialog.PauseMode.PauseSimulator, list, "", "", buttonFalse, 1, false);

            if (list2 == null || list2.Count == 0)
            {
                return null;
            }

            return list2[0] as SimDescription;
        }
        #endregion Show Sim Selector

        #region Return Household ObjectPicker.RowInfo

        public static List<ObjectPicker.RowInfo> ReturnHouseholdsAsRowInfo(List<Household> households)
        {
            List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();

            foreach (Household h in households)
            {
                if (h.Sims != null && h.Sims.Count > 0)
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

        public static List<ObjectPicker.RowInfo> ReturnInventoryItemAsRowInfo(List<InventoryItem> items)
        {
            List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();

            foreach (InventoryItem i in items)
            {
                //Don't sell your cell, make a new mod for this ^_^
                if (i.GetType() != typeof(PhoneSmart))
                {
                    List<ObjectPicker.ColumnInfo> list4 = new List<ObjectPicker.ColumnInfo>();
                    string text = i.Object.GetLocalizedName(); //Localization.LocalizeString(p.Data.Name, new object[0]);
                    list4.Add(new ObjectPicker.ThumbAndTextColumn(i.Object.GetThumbnailKey(), text));
                    list4.Add(new ObjectPicker.MoneyColumn(0));
                    ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(i, list4);
                    list.Add(item);
                }
            }

            return list;
        }

    }
}
