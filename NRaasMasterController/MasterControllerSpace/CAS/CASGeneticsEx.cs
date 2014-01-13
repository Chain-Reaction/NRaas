using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Tutorial;
using Sims3.Metadata;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.CAS
{
    public class CASGeneticsEx : Common.IPreLoad, Common.IWorldQuit
    {
        static bool sAlternate = false;

        public static SimDescription sChoice = null;

        static Household sHousehold;

        static Dictionary<SimDescription, Household> sOrigHouses = new Dictionary<SimDescription, Household>();

        public void OnPreLoad()
        {
            Task.Perform();
        }

        public static void AddTempSim(SimDescription sim)
        {
            sOrigHouses[sim] = sim.Household;

            sHousehold.AddTemporary(sim);
        }

        public static void InitHousehold(List<SimDescription> sims)
        {
            if (sHousehold == null)
            {
                sHousehold = Household.Create(false);
            }

            foreach (SimDescription parent in sims)
            {
                AddTempSim(parent);
            }

            CASLogic.sSingleton.mHouseholdScriptHandle = sHousehold.Proxy.ObjectId;
        }

        public static void DestroyHousehold()
        {
            if (sHousehold == null) return;

            foreach (SimDescription sim in new List<SimDescription>(CommonSpace.Helpers.Households.All(sHousehold)))
            {
                sHousehold.RemoveTemporary(sim);

                Household house;
                if (sOrigHouses.TryGetValue(sim, out house))
                {
                    if (house != null)
                    {
                        house.AddTemporary(sim);
                    }
                }
            }

            sOrigHouses.Clear();

            sHousehold.Dispose();
            sHousehold = null;
        }

        public void OnWorldQuit()
        {
            DestroyHousehold();
        }

        public static void OnFatherThumbnailMouseUp(WindowBase sender, UIMouseEventArgs args)
        {
            OnThumbnailMouseUp(sender, args, CASAgeGenderFlags.Male);
        }

        public static void OnMotherThumbnailMouseUp(WindowBase sender, UIMouseEventArgs args)
        {
            OnThumbnailMouseUp(sender, args, CASAgeGenderFlags.Female);
        }

        private static bool OnTest(SimDescription sim)
        {
            return (sim.GetOutfitCount(OutfitCategories.Everyday) > 0);
        }

        private static void OnThumbnailMouseUp(WindowBase sender, UIMouseEventArgs args, CASAgeGenderFlags gender)
        {
            try
            {
                CASGenetics ths = CASGenetics.gSingleton;
                if (ths == null) return;

                if (!ths.mHourglassVisible)
                {
                    CASAgeGenderFlags otherGender = CASAgeGenderFlags.Female;

                    Sims3.Gameplay.Function func = ths.UpdateFatherPreviewTask;
                    Sims3.Gameplay.Function altFunc = ths.UpdateMotherPreviewTask;

                    int index = 0, altIndex = 1;
                    if (gender == CASAgeGenderFlags.Female)
                    {
                        index = 1;
                        altIndex = 0;
                        otherGender = CASAgeGenderFlags.Male;

                        func = ths.UpdateMotherPreviewTask;
                        altFunc = ths.UpdateFatherPreviewTask;
                    }

                    ISimDescription tag = sender.Tag as ISimDescription;
                    
                    if (args.MouseKey == MouseKeys.kMouseLeft)
                    {
                        sAlternate = !sAlternate;
                        if ((!HasGender(otherGender)) && (sAlternate))
                        {
                            ths.mSelectedParents[altIndex] = tag;
                            if (tag != null)
                            {
                                ths.CheckSpecies();

                                Common.FunctionTask.Perform(altFunc);
                            }
                        }
                        else
                        {
                            // Elements are intentionally reversed
                            ths.mSelectedParents[index] = tag;
                            if (tag != null)
                            {
                                ths.CheckSpecies();

                                Common.FunctionTask.Perform(func);
                            }
                        }
                    }
                    else if (args.MouseKey == MouseKeys.kMouseRight)
                    {
                        // Elements are intentionally reversed
                        ths.mSelectedParents[altIndex] = tag;
                        if (tag != null)
                        {
                            ths.CheckSpecies();

                            Common.FunctionTask.Perform(altFunc);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnThumbnailMouseUp", e);
            }
        }

        public static bool HasGender(CASAgeGenderFlags gender)
        {
            ICASModel cASModel = Responder.Instance.CASModel;
            foreach (ISimDescription description in Responder.Instance.CASModel.GetSimsInHousehold())
            {
                if (description.Gender == gender) return true;
            }
            return false;
        }

        private static void OnAcceptButtonClick(WindowBase sender, UIButtonClickEventArgs args)
        {
            try
            {
                CASGenetics ths = CASGenetics.gSingleton;
                if (ths == null) return;

                if (!ths.mHourglassVisible)
                {
                    args.Handled = true;
                    if (ths.mSelectedOffspring != null)
                    {
                        if (CASPuck.Instance != null)
                        {
                            ths.mReturnState = ths.mSelectedOffspring.IsPet ? CASState.PetSummary : CASState.Summary;

                            CASPuck.Instance.AttemptingToAdd = true;
                            CASPuck.ShowInputBlocker();
                            Responder.Instance.CASModel.RequestLoadSim(ths.mSelectedOffspring, false);
                            Responder.Instance.CASModel.RequestAddSimToHousehold(false);
                            Responder.Instance.CASModel.RequestClearStack();
                            CASController.Singleton.SetCurrentState(ths.mReturnState);
                        }
                        else
                        {
                            FacialBlends.CopyGenetics(ths.mSelectedOffspring as SimDescription, sChoice, false, false);

                            new SavedOutfit.Cache(sChoice).PropagateGenetics(sChoice, CASParts.sPrimary);

                            SimOutfit currentOutfit = sChoice.CreatedSim.CurrentOutfit;
                            if (currentOutfit != null)
                            {
                                ThumbnailManager.GenerateHouseholdSimThumbnail(currentOutfit.Key, currentOutfit.Key.InstanceId, 0x0, ThumbnailSizeMask.Large | ThumbnailSizeMask.ExtraLarge | ThumbnailSizeMask.Medium | ThumbnailSizeMask.Small, ThumbnailTechnique.Default, true, false, sChoice.AgeGenderSpecies);
                            }

                            DestroyHousehold();

                            CASGenetics.Unload();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnAcceptButtonClick", e);
            }
        }

        private static void OnCancelButtonClick(WindowBase sender, UIButtonClickEventArgs args)
        {
            try
            {
                CASGenetics ths = CASGenetics.gSingleton;
                if (ths == null) return;

                if (!ths.mHourglassVisible)
                {
                    args.Handled = true;

                    if (CASPuck.Instance != null)
                    {
                        ths.mReturnState = Responder.Instance.CASModel.CurrentSimDescription.IsPet ? CASState.PetSummary : CASState.Summary;

                        if (ths.mSelectedOffspring == null)
                        {
                            CASController.Singleton.SetCurrentState(ths.mReturnState);
                        }
                        else
                        {
                            int currentPreviewSim = CASPuck.gSingleton.CurrentPreviewSim;
                            if (currentPreviewSim != -1)
                            {
                                ths.mCancelingSim = true;
                                Responder.Instance.CASModel.RequestSetPreviewSim(currentPreviewSim);
                            }
                            else
                            {
                                CASController.Singleton.SetCurrentState(ths.mReturnState);
                            }
                        }
                    }
                    else
                    {
                        DestroyHousehold();

                        CASGenetics.Unload();
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnCancelButtonClick", e);
            }
        }

        public class Task : RepeatingTask
        {
            protected Task()
            { }

            public static void Perform()
            {
                new Task().AddToSimulator();
            }

            protected override bool OnPerform()
            {
                CASGenetics genetics = CASGenetics.gSingleton;
                if (genetics != null)
                {
                    Button childByID = genetics.GetChildByID(0x5da4901, true) as Button;
                    childByID.Click -= genetics.OnCancelButtonClick;
                    childByID.Click -= CASGeneticsEx.OnCancelButtonClick;
                    childByID.Click += CASGeneticsEx.OnCancelButtonClick;

                    childByID = genetics.GetChildByID(0x5da97ff, true) as Button;
                    childByID.Click -= genetics.OnCancelButtonClick;
                    childByID.Click -= CASGeneticsEx.OnCancelButtonClick;
                    childByID.Click += CASGeneticsEx.OnCancelButtonClick;

                    if (genetics.mAcceptButton != null)
                    {
                        genetics.mAcceptButton.Click -= genetics.OnAcceptButtonClick;
                        genetics.mAcceptButton.Click -= CASGeneticsEx.OnAcceptButtonClick;
                        genetics.mAcceptButton.Click += CASGeneticsEx.OnAcceptButtonClick;
                    }

                    if ((genetics.mHourglassVisible) && (genetics.mSelectedOffspring == null) && (genetics.mSelectedParents.Length == 2))
                    {
                        if ((genetics.mSelectedParents[0] == null) && (genetics.mSelectedParents[1] == null))
                        {
                            if (genetics.mFathersButtons != null)
                            {
                                foreach (Button father in genetics.mFathersButtons)
                                {
                                    if (father == null) continue;

                                    ISimDescription fatherSim = father.Tag as ISimDescription;
                                    if (fatherSim == null) continue;

                                    genetics.mSelectedParents[0] = fatherSim;
                                    genetics.UpdateFatherPreviewTask();
                                    break;
                                }
                            }

                            if ((genetics.mSelectedParents[0] == null) && (genetics.mMothersButtons != null))
                            {
                                foreach (Button mother in genetics.mMothersButtons)
                                {
                                    if (mother == null) continue;

                                    ISimDescription motherSim = mother.Tag as ISimDescription;
                                    if (motherSim == null) continue;

                                    genetics.mSelectedParents[1] = motherSim;
                                    genetics.UpdateMotherPreviewTask();
                                    break;
                                }
                            }
                        }
                        else if ((genetics.mSelectedParents[0] == null) && (genetics.mSelectedParents[1] != null))
                        {
                            genetics.mSelectedParents[0] = genetics.mSelectedParents[1];
                            genetics.UpdateFatherPreviewTask();
                        }
                        else if ((genetics.mSelectedParents[1] == null) && (genetics.mSelectedParents[0] != null))
                        {
                            genetics.mSelectedParents[1] = genetics.mSelectedParents[0];
                            genetics.UpdateMotherPreviewTask();
                        }
                    }

                    bool enableButtons = (CASPuck.Instance == null);

                    if (genetics.mFathersButtons != null)
                    {
                        foreach (Button button in genetics.mFathersButtons)
                        {
                            if (button == null) continue;

                            button.Click -= genetics.OnFatherThumbnailClick;

                            button.MouseUp -= CASGeneticsEx.OnFatherThumbnailMouseUp;
                            button.MouseUp += CASGeneticsEx.OnFatherThumbnailMouseUp;
                        }
                    }

                    if (genetics.mMothersButtons != null)
                    {
                        foreach (Button button in genetics.mMothersButtons)
                        {
                            if (button == null) continue;

                            button.Click -= genetics.OnMotherThumbnailClick;

                            button.MouseUp -= CASGeneticsEx.OnMotherThumbnailMouseUp;
                            button.MouseUp += CASGeneticsEx.OnMotherThumbnailMouseUp;
                        }
                    }
                }

                return true;
            }
        }
    }
}
