using Sims3.UI;
using Sims3.Gameplay.Utilities;
using Sims3.UI.CAS;
using System.Collections.Generic;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Objects.Electronics;
using System;
using TS3Apartments;
using Sims3.Gameplay.Abstracts;
using Sims3.SimIFace;

namespace Sims3.Gameplay.Objects.Miscellaneous.TS3Apartments
{
    public class CommonMethods
    {
        public static string MenuSettingsPath = "Settings";

        #region Localization
        /// <summary>
        /// Localization
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string LocalizeString(string name, params object[] parameters)
        {
            return Localization.LocalizeString("ani_TS3Apartments:" + name, parameters);
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

        #region Show Sim List

        public static List<PhoneSimPicker.SimPickerInfo> ShowHomelesSims(Sim sim, Lot lot, List<ApartmentFamily> families)
        {
            List<PhoneSimPicker.SimPickerInfo> list = new List<PhoneSimPicker.SimPickerInfo>();

            //Create list of sims
            if (lot.Household.SimDescriptions != null)
                foreach (SimDescription s in lot.Household.SimDescriptions)
                {
                    list.Add(Phone.Call.CreateBasicPickerInfo(sim.SimDescription, s));
                }

            if (lot.Household.PetSimDescriptions != null)
                foreach (SimDescription s in lot.Household.PetSimDescriptions)
                {
                    list.Add(Phone.Call.CreateBasicPickerInfo(sim.SimDescription, s));
                }            

            //Find sims who are not in families
            if (list.Count > 0)
                foreach (ApartmentFamily f in families)
                {
                    if (f.Residents != null)
                        foreach (SimDescription s in f.Residents)
                        {
                            //If sim in family remove from available sims
                            PhoneSimPicker.SimPickerInfo sp = Phone.Call.CreateBasicPickerInfo(sim.SimDescription, s);
                            if (((SimDescription)sp.SimDescription).SimDescriptionId == s.SimDescriptionId)
                                list.Remove(sp);
                        }
                }

            return list;
        }

        #endregion

        #region Show resident sims

        public static List<PhoneSimPicker.SimPickerInfo> ShowResidentSims(Sim sim, List<SimDescription> residents)
        {
            List<PhoneSimPicker.SimPickerInfo> list = new List<PhoneSimPicker.SimPickerInfo>();

            //Create list of sims
            foreach (SimDescription s in residents)
            {
                list.Add(Phone.Call.CreateBasicPickerInfo(sim.SimDescription, s));
            }

            return list;
        }

        #endregion

        #region Return List of Minor Pets
        /// <summary>
        /// Returns list of minor pets in household
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static List<MinorPet> ReturnMinorPets(Controller c)
        {
            List<MinorPet> pets = new List<MinorPet>();

            //Find pets on lot
            MinorPet[] petArray = c.LotCurrent.GetObjects<MinorPet>();
            if(petArray != null)
                foreach (var item in petArray)
                {
                    pets.Add(item);
                }

            //Find pets in sim inventory
            if (c.LotCurrent.Household != null)
            {
                foreach (Sim s in c.LotCurrent.Household.Sims)
                {
                    if (s.Inventory != null)
                    {
                       pets.AddRange(s.Inventory.FindAll<MinorPet>(false));
                    }
                }
            }

            return pets;
        }

        #endregion

        #region Return Homeless pets

        public static List<MinorPet> ReturnHomelessPets(Controller c)
        {
            List<MinorPet> homeless = ReturnMinorPets(c);

            foreach (ApartmentFamily af in c.Families)
            {
                if (af.MinorPets != null)
                {
                    foreach (MinorPet p in af.MinorPets)
                    {
                        MinorPet p2 = homeless.Find(delegate(MinorPet p3) { return p3.ObjectId == p.ObjectId; });
                        if (p2 != null)
                            homeless.Remove(p2);
                    }
                }
            }

            return homeless;
        }


        #endregion

        #region Return Minor pet ObjectPicker.RowInfo

        public static List<ObjectPicker.RowInfo> ReturnMinorPetsAsRowInfo(List<MinorPet> pets)
        {
            List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();

            foreach (MinorPet p in pets)
            {
                List<ObjectPicker.ColumnInfo> list4 = new List<ObjectPicker.ColumnInfo>();
                string text = p.Data.Name; //Localization.LocalizeString(p.Data.Name, new object[0]);
                list4.Add(new ObjectPicker.ThumbAndTextColumn(p.GetThumbnailKey(), text));
                list4.Add(new ObjectPicker.MoneyColumn(0));
                ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(p, list4);
                list.Add(item);
               
            }

            return list;
        }

        #endregion Return Minor Pet ObjectPicker.RowInfo

        #region Minor Pet Dialog
        /// <summary>
        /// Show minor pets dialog
        /// </summary>
        /// <returns></returns>
        public static List<MinorPet> ShowMinorPetDialog(List<MinorPet> pets)
        {
         //   MinorPetTerrarium.Stock.Definition definition = base.InteractionDefinition as MinorPetTerrarium.Stock.Definition;
            List<ObjectPicker.HeaderInfo> list = new List<ObjectPicker.HeaderInfo>();
            List<ObjectPicker.TabInfo> list2 = new List<ObjectPicker.TabInfo>();
            string headerTextId = "Gameplay/Objects/MinorPetTerrarium/Stock:StockDialogPetColumnHeader";
            string headerTooltipId = "Gameplay/Objects/MinorPetTerrarium/Stock:StockDialogPetColumnHeaderTooltip";
            string headerTextId2 = "Gameplay/Objects/MinorPetTerrarium/Stock:StockDialogPriceColumnHeader";
            string headerTooltipId2 = "Gameplay/Objects/MinorPetTerrarium/Stock:StockDialogPriceColumnHeaderTooltip";
            list.Add(new ObjectPicker.HeaderInfo(headerTextId, headerTooltipId));
            list.Add(new ObjectPicker.HeaderInfo(headerTextId2, headerTooltipId2));
            List<ObjectPicker.RowInfo> list3 = new List<ObjectPicker.RowInfo>();
            //foreach (KeyValuePair<MinorPetSpecies, MinorPetData> current in MinorPet.sData)
            //{
            //    if (current.Value.Stockable && definition.mPetTypeToShowInDialog == current.Value.MinorPetType && this.Target.AllowsPetType(current.Value.MinorPetType))
            //    {
            //        ThumbnailKey thumbnail = new ThumbnailKey(GlobalFunctions.CreateProductKey(current.Value.MedatorName, current.Value.CodeVersion), ThumbnailSize.Medium);
            //        List<ObjectPicker.ColumnInfo> list4 = new List<ObjectPicker.ColumnInfo>();
            //        string text = Localization.LocalizeString(current.Value.Name, new object[0]);
            //        list4.Add(new ObjectPicker.ThumbAndTextColumn(thumbnail, text));
            //        list4.Add(new ObjectPicker.MoneyColumn(current.Value.StockCost));
            //        ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(current.Key, list4);
            //        list3.Add(item);
            //    }
            //}

            foreach (MinorPet current in pets)
            {
               // ThumbnailKey thumbnail = new ThumbnailKey(GlobalFunctions.CreateProductKey(current.Value.MedatorName, current.Value.CodeVersion), ThumbnailSize.Medium);
                List<ObjectPicker.ColumnInfo> list4 = new List<ObjectPicker.ColumnInfo>();
                string text = Localization.LocalizeString(current.Data.Name, new object[0]);
                list4.Add(new ObjectPicker.ThumbAndTextColumn(current.GetThumbnailKey(), text));
                list4.Add(new ObjectPicker.MoneyColumn(0));
                ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(current.Data, list4);
                list3.Add(item);
            }

            list2.Add(new ObjectPicker.TabInfo(string.Empty, string.Empty, list3));
            string title = Localization.LocalizeString("Gameplay/Objects/MinorPetTerrarium/Stock:StockDialogTitle", new object[0]);
          //  List<ObjectPicker.RowInfo> list5 = Simpled ModalDialog.
            List<ObjectPicker.RowInfo> list5 = SimpleListDialog.Show(title, 0, list2, list, true, Localization.LocalizeString("Ui/Caption/Global:Accept", new object[0]));
            if (list5 == null)
            {
                return null;
            }

            List<MinorPet> minorPets = new List<MinorPet>();
            foreach (var item in list5)
            {
                minorPets.Add(item.Item as MinorPet);
            }
            //MinorPetSpecies species = (MinorPetSpecies)list5[0].Item;
            //MinorPet minorPet = MinorPet.Make(species, true, false) as MinorPet;
            //if (minorPet != null)
            //{
            //    minorPet.SetBehaviorSMCStateStopped();
            //}
            return minorPets;
        }

        #endregion Minor Pet Dialog

    }
}
