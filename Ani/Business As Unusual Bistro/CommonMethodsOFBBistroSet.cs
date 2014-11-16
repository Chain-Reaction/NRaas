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
using Sims3.Gameplay.Objects.TombObjects.ani_BistroSet;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Store.Objects;


namespace ani_BistroSet
{
    public static class CommonMethodsOFBBistroSet
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
            return Localization.LocalizeString("ani_OFBBistroSet:" + name, parameters);
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
        public static List<IMiniSimDescription> ShowSimSelector(Sim actor, ulong cheffId, string interactionName)
        {
            List<IMiniSimDescription> residents = new List<IMiniSimDescription>();
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

        #region Return Shifts
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interactionName"></param>
        /// <returns></returns>
        public static Shift ReturnShift(Sim sim, OFBOven oven, int NumSelectableRows)
        {
            string buttonFalse = Localization.LocalizeString("Ui/Caption/ObjectPicker:Cancel", new object[0]);

            List<ObjectPicker.HeaderInfo> headers = new List<ObjectPicker.HeaderInfo>();
            List<ObjectPicker.TabInfo> listObjs = new List<ObjectPicker.TabInfo>();

            headers.Add(new ObjectPicker.HeaderInfo("", "", 500));
            List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();

            foreach (Shift shift in oven.info.Shifts)
            {
                List<ObjectPicker.ColumnInfo> list2 = new List<ObjectPicker.ColumnInfo>();

                ThumbnailKey key = new ThumbnailKey();

                SimDescription simDescription = SimDescription.Find(shift.Cheff.DescriptionId);
                if (simDescription != null)
                {
                    key = simDescription.GetThumbnailKey(ThumbnailSize.Large, 0);
                }
                list2.Add(new ObjectPicker.ThumbAndTextColumn(key, shift.StarWork + ":00 - " + shift.EndWork + ":00"));
                ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(shift, list2);
                list.Add(item);
            }

            ObjectPicker.TabInfo item2 = new ObjectPicker.TabInfo("recipeRowImageName", StringTable.GetLocalizedString("Store/Objects/IndustrialOven/SetMenu:TabText"), list);
            listObjs.Add(item2);


            List<ObjectPicker.RowInfo> objects = ObjectPickerDialog.Show("Shifts", "Select", "Cancel", listObjs, headers, NumSelectableRows);

            if (objects != null && objects.Count > 0)
                return ((Shift)objects[0].Item);

            return null;


        }
        #endregion Show Sim Selector

        #region Return Recipe ObjectPicker.RowInfo

        public static List<ObjectPicker.RowInfo> ReturnRecipeholdsAsRowInfo(List<Recipe> recipes, List<Sims3.Store.Objects.IndustrialOven.MenuRecipeInfo> selectedRecipes)
        {
            List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();
            
            if (selectedRecipes != null)
            {
                recipes = new List<Recipe>();
                foreach (IndustrialOven.MenuRecipeInfo o in selectedRecipes)
                {
                    Recipe r = o.FindRecipe();
                    if (r != null)
                        recipes.Add(r);
                }
            }

            if (recipes != null)
                foreach (Recipe r in recipes)
                {
                    {
                        List<ObjectPicker.ColumnInfo> list4 = new List<ObjectPicker.ColumnInfo>();
                        list4.Add(new ObjectPicker.ThumbAndTextColumn(r.GetThumbnailKey(), r.GenericName));
                        list4.Add(new ObjectPicker.MoneyColumn(0));
                        ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(r, list4);
                        list.Add(item);
                    }
                }

            return list;
        }

        #endregion Return Household ObjectPicker.RowInfo

    }
}
