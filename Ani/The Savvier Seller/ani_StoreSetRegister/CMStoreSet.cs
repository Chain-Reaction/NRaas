using System;
using Sims3.Gameplay.Utilities;
using Sims3.UI;
using Sims3.UI.CAS;
using System.Collections.Generic;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Store.Objects;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreSetRegister;
using ani_StoreSetBase;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreSetBase;


namespace ani_StoreSetRegister
{
    public static class CMStoreSet
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
            return Localization.LocalizeString("ani_StoreSet:" + name, parameters);
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
        public static List<SimDescription> ShowSimSelector(Sim actor, ulong cheffId, string interactionName)
        {
            List<SimDescription> residents = new List<SimDescription>();
            string buttonFalse = Localization.LocalizeString("Ui/Caption/ObjectPicker:Cancel", new object[0]);

            List<PhoneSimPicker.SimPickerInfo> list = new List<PhoneSimPicker.SimPickerInfo>();

            List<object> list2;

            //Create list of sims
            foreach (SimDescription sd in SimDescription.GetAll(SimDescription.Repository.Household))
            {
                if (!sd.IsPet && sd.TeenOrAbove && sd.IsContactable && sd.SimDescriptionId != cheffId)
                {
                    list.Add(Phone.Call.CreateBasicPickerInfo(actor.SimDescription, sd));
                }
            }

            list2 = PhoneSimPicker.Show(true, ModalDialog.PauseMode.PauseSimulator, list, interactionName, interactionName, buttonFalse, 3, false);

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
        public static SimDescription ReturnSim(ulong descriptionId)
        {
            foreach (SimDescription sd in SimDescription.GetAll(SimDescription.Repository.Household))
            {
                if (sd.SimDescriptionId == descriptionId && sd.IsContactable)
                    return sd;
            }
            return null;
        }
        #endregion Show Sim Selector

        #region Return Sims in Household

        public static SimDescription ReturnSimsInHousehold(SimDescription actor, bool teenOrAbove, bool residentsOnly)
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
                            list.Add(Phone.Call.CreateBasicPickerInfo(actor, sd));
                        }
                    }
                }
            else
            {
                foreach (SimDescription sd in SimDescription.GetAll(SimDescription.Repository.Household))
                {
                    if (!sd.IsPet && sd.TeenOrAbove && sd.IsContactable)
                    {
                        list.Add(Phone.Call.CreateBasicPickerInfo(actor, sd));
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

        #endregion

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

        #region Return Next Idle

        public static void DoRandomIdle(Sim sim, InteractionPriority priority)
        {
            //20% chance of doing random idle. 
            int percentage = RandomUtil.GetInt(1, 100);

            if (percentage <= 50)
            {
                string idle;
                IdleInfo info = null;

                bool customJazzGraph;
                ProductVersion version;

                idle = IdleManager.ChooseTraitIdle(sim.TraitManager.GetRandomVisibleElement().mTraitGuid, Sims3.SimIFace.CAS.CASAgeGenderFlags.Adult, Sims3.SimIFace.CAS.CASAgeGenderFlags.Female, out customJazzGraph, out version);

                foreach (IdleAnimationInfo item in IdleManager.sTraitIdleAnimations.Values)
                {
                    if (item.Animations != null)
                        info = item.Animations.Find(delegate(IdleInfo i) { return (!string.IsNullOrEmpty(i.AnimationName) && i.AnimationName.Equals(idle)); });
                    if (info != null)
                        break;
                }
                if (info != null)
                {
                    // CommonMethodsOFBBistroSet.PrintMessage(info.AnimationName);
                    Sim.PlayAnim(sim, info.AnimationName);
                }
            }
            else
            {
                sim.IdleManager.PlayOneFacialIdle();
            }
        }
        #endregion

        #region Return Unemployed sims

        public static List<PhoneSimPicker.SimPickerInfo> ReturnUnemployedSims(Sim sim, Lot lot, bool residentsOnly, List<SimDescription> workers)
        {
            List<PhoneSimPicker.SimPickerInfo> list = new List<PhoneSimPicker.SimPickerInfo>();

            //Create list of sims
            if (residentsOnly)
                foreach (Household h in Household.GetHouseholdsLivingInWorld())
                {
                    foreach (SimDescription sd in h.SimDescriptions)
                    {
                        if (!sd.IsPet && sd.TeenOrAbove && sd.IsContactable)
                        {
                            list.Add(Phone.Call.CreateBasicPickerInfo(sim.SimDescription, sd));
                        }
                    }
                }
            else
            {
                foreach (SimDescription sd in SimDescription.GetAll(SimDescription.Repository.Household))
                {
                    if (!sd.IsPet && sd.TeenOrAbove && sd.IsContactable)
                    {
                        list.Add(Phone.Call.CreateBasicPickerInfo(sim.SimDescription, sd));
                    }
                }
            }

            //Remove Employed Sims
            if (workers != null && workers.Count > 0)
                foreach (SimDescription sd in workers)
                {
                    PhoneSimPicker.SimPickerInfo sp = Phone.Call.CreateBasicPickerInfo(sim.SimDescription, sd);
                    if (sp.SimDescription != null && ((SimDescription)sp.SimDescription).SimDescriptionId == sd.SimDescriptionId)
                        list.Remove(sp);

                }

            return list;
        }

        #endregion

        #region Return Register For Linking

        public static StoreSetRegister ReturnRegisterForLinking(StoreSetRegister[] objects)
        {
            ThumbnailKey thumbnail = ThumbnailKey.kInvalidThumbnailKey;
            string text = string.Empty;
            List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();

            foreach (StoreSetRegister t in objects)
            {
                List<ObjectPicker.ColumnInfo> list2 = new List<ObjectPicker.ColumnInfo>();

                int num = 0;
                thumbnail = t.GetThumbnailKey();

                text = t.Info.RegisterName;

                //common
                list2.Add(new ObjectPicker.ThumbAndTextColumn(thumbnail, text));
                list2.Add(new ObjectPicker.MoneyColumn(num));
                ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(t, list2);
                list.Add(item);
            }


            List<ObjectPicker.HeaderInfo> list3 = new List<ObjectPicker.HeaderInfo>();
            List<ObjectPicker.TabInfo> list4 = new List<ObjectPicker.TabInfo>();
            list3.Add(new ObjectPicker.HeaderInfo(string.Empty, string.Empty, 200));
            list3.Add(new ObjectPicker.HeaderInfo("1", "2"));
            list4.Add(new ObjectPicker.TabInfo("3", CMStoreSet.LocalizeString("LinkToRegister", new object[0]), list));
            List<ObjectPicker.RowInfo> list5 = TaxCollectorSimpleDialog.Show(CMStoreSet.LocalizeString("LikToRegister", new object[0] { }), 0, list4, list3, true);
            if (list5 == null || list5.Count != 1)
            {
                return null;
            }
            return (list5[0].Item as StoreSetRegister);
        }

        #endregion

        #region Return Register

        public static StoreSetRegister ReturnRegister(ObjectGuid id, Lot lot)
        {
            StoreSetRegister register = null;
            List<StoreSetRegister> registers = new List<StoreSetRegister>(Sims3.Gameplay.Queries.GetObjects<StoreSetRegister>(lot));

            if (registers != null && registers.Count > 0)
            {
                register = registers.Find(delegate(StoreSetRegister r) { return r.ObjectId == id; });
            }

            return register;
        }

        #endregion Return Register

        #region Return First Customer

        public static SimDescription ReturnFirstCustomer(Dictionary<SimDescription, int> dct, Lot lot)
        {
            SimDescription sd = null;
            foreach (SimDescription item in dct.Keys)
            {
                if (item.CreatedSim != null && !item.CreatedSim.IsActiveSim && item.CreatedSim.LotCurrent == lot)
                {
                    sd = item;
                    break;
                }

            }
            return sd;
        }

        #endregion

        #region Pay Wages

        public static void PayWages(Sim clerck, StoreSetRegister register, float startTime, float endTime)
        {
            int pay = register.Info.HourlyWage * ((int)(endTime - startTime));

            //  CMStoreSet.PrintMessage(this.Actor.FullName + " pay : " + pay);

            SimDescription owner = null;

            if (register.Info.OwnerId != 0uL)
                CMStoreSet.ReturnSim(register.Info.OwnerId);

            if (owner == null)
            {
                if (!register.Info.PayWhenActive || (register.Info.PayWhenActive && clerck.Household.IsActive))
                    clerck.ModifyFunds(pay);
            }
            else if (owner != null && clerck.Household.HouseholdId != owner.Household.HouseholdId)
            {
                if (!register.Info.PayWhenActive || (register.Info.PayWhenActive && clerck.Household.IsActive))
                    clerck.ModifyFunds(pay);

                if (!register.Info.PayWhenActive || (register.Info.PayWhenActive && owner.Household.IsActive))
                    owner.ModifyFunds(-pay);
            }
        }

        #endregion

        #region Store Open
        public static bool IsStoreOpen(StoreSetRegister register)
        {
            if (register == null)
                return false;

            if (!register.Info.Open || !SimClock.IsTimeBetweenTimes(register.mOvenHoursStart, register.mOvenHoursEnd))
                return false;
            else
                return true;
        }
        #endregion


    }
}
