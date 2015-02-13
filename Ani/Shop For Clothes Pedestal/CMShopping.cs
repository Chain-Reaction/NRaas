using Sims3.Gameplay.Utilities;
using Sims3.UI;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Objects.Electronics;
using System.Collections.Generic;
using Sims3.SimIFace.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.Miscellaneous.Shopping.ani_ClothingPedestal;
using Sims3.Gameplay.Abstracts;
using Sims3.SimIFace;
using System.Text;
using Sims3.UI.CAS;

namespace ani_ClothingPedestal
{
    public static class CMShopping
    {
        public static string MenuClothingData = "ClothingData";
        public static string MenuSettingsPath = "Settings";
        public static string MenuMisc = "Misc";
        public static string MenuMannequin = "Mannequin";

        #region Localization
        /// <summary>
        /// Localization
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string LocalizeString(string name, params object[] parameters)
        {
            return Localization.LocalizeString("ani_CustomPedestal:" + name, parameters);
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

        #region Perform Interaction

        public static void PerformeInteraction(Sim sim, GameObject target, InteractionDefinition definition)
        {
            InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);
            InteractionInstance entry = definition.CreateInstance(target, sim, priority, false, true);
            sim.InteractionQueue.Add(entry);
        }

        #endregion

        #region Clothing Info

        public static Dictionary<OutfitCategories, List<SimOutfit>> ReturnClothingInfo(Sim sim)
        {
            Dictionary<OutfitCategories, List<SimOutfit>> dict = new Dictionary<OutfitCategories, List<SimOutfit>>();

            List<OutfitCategories> categories = new List<OutfitCategories>();
            categories.Add(OutfitCategories.Everyday);
            categories.Add(OutfitCategories.Formalwear);
            categories.Add(OutfitCategories.Sleepwear);
            categories.Add(OutfitCategories.Swimwear);
            categories.Add(OutfitCategories.Athletic);
            categories.Add(OutfitCategories.Outerwear);

            if (sim.SimDescription != null)
            {
                foreach (OutfitCategories category in categories)
                {
                    List<SimOutfit> outfits = new List<SimOutfit>();

                    foreach (SimOutfit outfit in sim.SimDescription.GetOutfits(category))
                    {
                        outfits.Add(outfit);
                    }
                    dict.Add(category, outfits);
                }
            }

            return dict;
        }

        public static int CalculateChangedOutfits(Sim sim, Dictionary<OutfitCategories, List<SimOutfit>> beforeCas)
        {
            int outfitCount = 0;

            //How many parts need to match so we declare it a match
            int kFullPartCount = 2;
            int kCurrentFullPartCount = kFullPartCount;

            try
            {
                Dictionary<OutfitCategories, List<SimOutfit>> afterCas = ReturnClothingInfo(sim);

                foreach (KeyValuePair<OutfitCategories, List<SimOutfit>> itemBeforeCas in beforeCas)
                {
                    List<ResourceKey> newOutfitsThatMatch = new List<ResourceKey>();

                    if (itemBeforeCas.Key == OutfitCategories.Everyday || itemBeforeCas.Key == OutfitCategories.Formalwear ||
                        itemBeforeCas.Key == OutfitCategories.Sleepwear || itemBeforeCas.Key == OutfitCategories.Athletic ||
                        itemBeforeCas.Key == OutfitCategories.Swimwear || itemBeforeCas.Key == OutfitCategories.Outerwear)
                    {
                        int index = 0;
                        int matchCount = 0;

                        int numberOfNewOutfits = afterCas[itemBeforeCas.Key].Count;

                        foreach (SimOutfit oldOutfit in itemBeforeCas.Value)
                        {
                            foreach (SimOutfit newOutfit in afterCas[itemBeforeCas.Key])
                            {
                                //Reset values
                                matchCount = 0;
                                kCurrentFullPartCount = kFullPartCount;

                                for (int i = 0; i < oldOutfit.Parts.Length; i++)
                                {
                                    if (/*oldOutfit.Parts[i].DisplayIndex > 0 && */(
                                        (oldOutfit.Parts[i].BodyType == BodyTypes.UpperBody) ||
                                        oldOutfit.Parts[i].BodyType == BodyTypes.LowerBody ||
                                        oldOutfit.Parts[i].BodyType == BodyTypes.FullBody))
                                    {
                                       
                                        //If full body, there is 1 less part to match
                                        if (oldOutfit.Parts[i].BodyType == BodyTypes.FullBody)
                                            kCurrentFullPartCount--;

                                        for (int j = 0; j < newOutfit.Parts.Length; j++)
                                        {
                                            if (newOutfit.Parts[j].BodyType == BodyTypes.UpperBody || newOutfit.Parts[j].BodyType == BodyTypes.LowerBody || newOutfit.Parts[j].BodyType == BodyTypes.FullBody)
                                            {
                                                NRaas.CommonSpace.Helpers.CASParts.Wrapper wrapperOld = new NRaas.CommonSpace.Helpers.CASParts.Wrapper(oldOutfit.Parts[i]);
                                                NRaas.CommonSpace.Helpers.CASParts.Wrapper wrapperNew = new NRaas.CommonSpace.Helpers.CASParts.Wrapper(newOutfit.Parts[j]);

                                                if (wrapperNew.mPart.Key == wrapperOld.mPart.Key)
                                                {
                                                    matchCount++;
                                                    break;
                                                }
                                            }

                                        }
                                    }
                                }
                               
                                //If the outfit is a match
                                if (matchCount >= kCurrentFullPartCount)
                                {
                                    numberOfNewOutfits--;
                                    break;
                                }
                            }
                            //CMShopping.PrintMessage("bare feet: " + barefootOldOutfit + " " + barefootNewOutfit);
                            //CMShopping.PrintMessage(itemBeforeCas.Key.ToString() + ": " + index + ": " + matchCount + " " + kCurrentFullPartCount);

                            index++;
                        }

                        //Number of new outfits
                        outfitCount += numberOfNewOutfits;

                    }

                }

            }
            catch (System.Exception ex)
            {
                CMShopping.PrintMessage(ex.Message);
            }
            if (outfitCount > 0)
                return outfitCount;
            return 0;
        }

        #endregion
    }
}
