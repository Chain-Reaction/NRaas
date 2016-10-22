using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Objects.TombObjects.ani_BistroSet;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;
using System;
using System.Text;
using Sims3.UI.CAS;
using Sims3.UI;
using Sims3.Gameplay.Actors;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Store.Objects;
using Sims3.Gameplay.ActorSystems;
using System.Collections;

namespace ani_BistroSet
{
    public static class BusinessMethods
    {
        public static Shift ReturnNextValidShift(OFBOven oven, List<Shift> shifts)
        {
            Shift shift = null;
            StringBuilder sb = new StringBuilder();
            //Sort the shifts and get the next one
            shifts.Sort(delegate(Shift s1, Shift s2)
            {
                return s1.StarWork.CompareTo(s2.StarWork);
            });

            Shift next = BusinessMethods.ReturnNextOrCurrentShift(shifts, SimClock.CurrentTime().Hour);

            if (next != null && next.Cheff != null)
            {
                if (BusinessMethods.CheckIfValidEmployee(next.Cheff.DescriptionId))
                {
                    shift = next;
                }
                else
                {
                    sb.Append("Selected chef is not valid.\n");
                    sb.Append("Please select a new chef");
                    CommonMethodsOFBBistroSet.PrintMessage(sb.ToString());
                }
            }
            else
            {
                if (next == null)
                    CommonMethodsOFBBistroSet.PrintMessage("Couldn't calculate next shift");
                else
                {
                    CommonMethodsOFBBistroSet.PrintMessage(BusinessMethods.ShowShiftInfo(next, "Shift information invalid", null, null));
                }

            }
            if (OFBOven.ShowDebugMessages)
                CommonMethodsOFBBistroSet.PrintMessage(BusinessMethods.ShowShiftInfo(next, "Next shift:", null, null));

            return shift;
        }

        private static Shift ReturnNextOrCurrentShift(List<Shift> emps, float time)
        {
            Shift shift = null;

            //Return current shift
            foreach (Shift e in emps)
            {
                //Is the end of shift the next day?
                float end = e.EndWork;
                if (end < e.StarWork)
                    end = Math.Abs(time - e.EndWork) + time;

                if (time >= e.StarWork && time <= end)
                {
                    shift = e;
                }
            }
            //Return next
            if (shift == null)
                shift = FindClosest(emps, time);


            return shift;

        }

        private static Shift FindClosest(List<Shift> shifts, float time)
        {
            Shift nearest = shifts[0];
            Double currentDifference = Math.Abs(nearest.StarWork - time);

            foreach (Shift s in shifts)
            {
                Double diff = Math.Abs(s.StarWork - time);
                if (diff < currentDifference && s.StarWork >= time)
                {
                    currentDifference = diff;
                    nearest = s;
                }
            }

            if (nearest != null)
                if (nearest.StarWork < time)
                    nearest = shifts[0];

            return nearest;
        }

        public static string ShowShiftInfo(Shift shift, string title, SimDescription cheff, List<PhoneSimPicker.SimPickerInfo> waiters)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(title);
            sb.Append("\n");
            sb.Append(shift.StarWork + ":00 - " + shift.EndWork + ":00");
            sb.Append("\n");
            sb.Append("Chef: ");

            //Check if the cheff is valid            
            if (cheff != null)
            {
                if (CheckIfValidEmployee(cheff.SimDescriptionId))
                {
                    sb.Append(cheff.FullName);
                    sb.Append(": " + shift.Cheff.Wage + "§/h");
                }
                else
                {
                    sb.Append("Selected chef is not valid.\n");
                    sb.Append("Please select a new chef");
                }
            }
            else
            {
                //If cheff null, re-check from shift info
                if (shift.Cheff == null)
                    sb.Append("No chef selected");
                else
                {
                    if (CheckIfValidEmployee(shift.Cheff.DescriptionId))
                        sb.Append("Selected chef is valid");
                    else
                        sb.Append("Selected chef is not valid");
                }
            }

            sb.Append("\n");

            int waiterWage = -1;
            if (shift.Waiters != null && shift.Waiters.Count > 0)
                waiterWage = shift.Waiters[0].Wage;

            if (waiters != null && waiters.Count > 0)
            {
                sb.Append("Waiters:\n");
                foreach (var w in waiters)
                {
                    sb.Append(((SimDescription)w.SimDescription).FullName);

                    if (waiterWage > -1)
                    {
                        sb.Append(": ");
                        sb.Append(waiterWage);
                        sb.Append("§/h");
                    }

                    sb.Append("\n");

                }
            }
            else
            {
                if (shift.Waiters != null && shift.Waiters.Count > 0)
                {
                    sb.Append("Waiters count:");
                    sb.Append(shift.Waiters.Count);
                }
                else
                    sb.Append("No waiters selected.");
            }

            return sb.ToString();
        }

        private static bool CheckIfValidEmployee(ulong id)
        {
            bool valid = false;
            //Find the cheff                    
            foreach (SimDescription sd in SimDescription.GetAll(SimDescription.Repository.Household))
            {
                if (!sd.IsPet && sd.TeenOrAbove && sd.IsContactable && sd.SimDescriptionId == id)
                {
                    valid = true;
                }
            }
            return valid;
        }

        public static void SendEverybodyHome(OFBOven oven, Sim cheff)
        {
            try
            {
                if (cheff != null)
                {
                    cheff.PopPosture();
                    cheff.InteractionQueue.CancelAllInteractions();
                    cheff.SwitchToOutfitWithoutSpin(OutfitCategories.Everyday);

                    if (!cheff.IsActiveSim)
                        Sim.MakeSimGoHome(cheff, false);
                }

                //Send waiters home
                if (oven.Waiters != null)
                {
                    foreach (ulong id in oven.Waiters)
                    {
                        SimDescription simDescription = CommonMethodsOFBBistroSet.ReturnSim(id); ;
                        if (simDescription != null && simDescription.CreatedSim != null)
                        {
                            simDescription.CreatedSim.PopPosture();
                            simDescription.CreatedSim.InteractionQueue.CancelAllInteractions();
                            simDescription.CreatedSim.SwitchToOutfitWithoutSpin(OutfitCategories.Everyday);
                            if (!simDescription.CreatedSim.IsActiveSim)
                            {
                                Sim.MakeSimGoHome(simDescription.CreatedSim, false);
                            }
                        }
                        else
                        {
                            CommonMethodsOFBBistroSet.PrintMessage("Couldn't send waiter home");
                        }
                    }
                }
            }
            finally
            {
                oven.Waiters = new List<ulong>();
            }
        }

        public static void ChangeToCareerOutfit(Sim sim)
        {
            //Does the sim have an career outfit
            //ArrayList list = sim.SimDescription.GetOutfits(OutfitCategories.Career);
            //CommonMethodsOFBBistroSet.PrintMessage(list.Count.ToString());
            //if (list == null || (list != null && list.Count == 0))
            //{
            //    CommonMethodsOFBBistroSet.PrintMessage(sim.FullName + " Doesn't have career outfit");
            //    sim.SwitchToOutfitWithoutSpin(OutfitCategories.Career);
            //    sim.SimDescription.AddOutfit(sim.CurrentOutfit, OutfitCategories.Career);
            //}
            //else
            if((sim.SimDescription.OccultManager != null && !sim.SimDescription.OccultManager.DisallowClothesChange()) && (sim.BuffManager != null && !sim.BuffManager.DisallowClothesChange()))
            {
                sim.SwitchToOutfitWithoutSpin(OutfitCategories.Career);
            }
        }

        public static IndustrialOven.MenuRecipeInfo ReturnSelectedFoodItem(OFBOven.Menu menu, Sim sim)
        {
            List<ObjectPicker.HeaderInfo> headers = new List<ObjectPicker.HeaderInfo>();
            List<ObjectPicker.TabInfo> listObjs = new List<ObjectPicker.TabInfo>();
            //IndustrialOven.Menu menu = parameters.Target as IndustrialOven.Menu;
            if (menu == null)
            {
                return null;
            }
            int cost = sim.FamilyFunds;
            if (sim.TraitManager.HasElement(TraitNames.DiscountDiner))
            {
                cost = 2147483647;
            }
            List<IndustrialOven.MenuRecipeInfo> foodsAtOrBelowCost = menu.GetFoodsAtOrBelowCost(cost);
            if (foodsAtOrBelowCost == null)
            {
                return null;
            }
            headers.Add(new ObjectPicker.HeaderInfo("Store/Objects/IndustrialOven:SelectRecipeHeader", "Store/Objects/IndustrialOven:SelectRecipeHeaderTooltip", 500));
            List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();
            for (int i = 0; i < foodsAtOrBelowCost.Count; i++)
            {
                Recipe recipe = foodsAtOrBelowCost[i].FindRecipe();
                if (recipe != null)
                {
                    List<ObjectPicker.ColumnInfo> list2 = new List<ObjectPicker.ColumnInfo>();
                    list2.Add(new ObjectPicker.ThumbAndTextColumn(recipe.GetThumbnailKey(ThumbnailSize.Large), recipe.GenericName));
                    ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(foodsAtOrBelowCost[i], list2);
                    list.Add(item);
                }
            }
            ObjectPicker.TabInfo item2 = new ObjectPicker.TabInfo("recipeRowImageName", StringTable.GetLocalizedString("Store/Objects/IndustrialOven/SetMenu:TabText"), list);
            listObjs.Add(item2);

            List<ObjectPicker.RowInfo> selection = ObjectPickerDialog.Show(true, ModalDialog.PauseMode.PauseSimulator, 
                CommonMethodsOFBBistroSet.LocalizeString("SelectMeal", new object[]{sim.FullName}),
                CommonMethodsOFBBistroSet.LocalizeString("Select", new object[0]),
                CommonMethodsOFBBistroSet.LocalizeString("Cancel", new object[0]), 
                listObjs, headers, 1);

            if (selection != null && selection.Count > 0)
                return ((IndustrialOven.MenuRecipeInfo)selection[0].Item);

            return null;

        }
    }
}
