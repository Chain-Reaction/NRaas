using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.MasterControllerSpace.CAS;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace.CAS;
using Sims3.UI.CAS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NRaas.MasterControllerSpace.Proxies
{
    public class CASModelProxy : NRaas.CommonSpace.Proxies.CASModelProxy
    {
        static Type sCASClothingCategory = typeof(CASClothingCategory);
        static Type sDelayedCategoryUpdate = typeof(CASClothingCategoryEx.DelayedCategoryUpdate);

        public CASModelProxy(ICASModel model)
            : base(model)
        { }

        public override void RequestAddCASPart(CASPart part, string preset)
        {
            if (MasterController.Settings.mAllowMultipleAccessories)
            {
                CASClothingCategory clothingCategory = CASClothingCategory.gSingleton;
                if (clothingCategory != null)
                {
                    if (clothingCategory.IsAccessoryType(part.BodyType))
                    {
                        CASLogic.GetSingleton().mRequestModelDirty = true;
                        CASLogic.CASOperationStack.Instance.Push(new AddCASPartOperationEx(part, preset));
                        return;
                    }
                }
            }

            if (MasterController.Settings.mAllowMultipleMakeup)
            {
                CASMakeup makeup = CASMakeup.gSingleton;
                if (makeup != null)
                {
                    if (CASParts.IsMakeup(part.BodyType))
                    {
                        CASLogic.GetSingleton().mRequestModelDirty = true;
                        CASLogic.CASOperationStack.Instance.Push(new AddCASPartOperationEx(part, preset));
                        return;
                    }
                }
            }

            base.RequestAddCASPart(part, preset);
        }
        public override void RequestAddCASPart(CASPart part, bool randomizeDesign)
        {
            if (MasterController.Settings.mAllowMultipleAccessories)
            {
                CASClothingCategory clothingCategory = CASClothingCategory.gSingleton;
                if (clothingCategory != null)
                {
                    if (clothingCategory.IsAccessoryType(part.BodyType))
                    {
                        CASLogic.GetSingleton().mRequestModelDirty = true;
                        CASLogic.CASOperationStack.Instance.Push(new AddCASPartOperationEx(part, randomizeDesign));
                        return;
                    }
                }
            }

            if (MasterController.Settings.mAllowMultipleMakeup)
            {
                CASMakeup makeup = CASMakeup.gSingleton;
                if (makeup != null)
                {
                    if (CASParts.IsMakeup(part.BodyType))
                    {
                        CASLogic.GetSingleton().mRequestModelDirty = true;
                        CASLogic.CASOperationStack.Instance.Push(new AddCASPartOperationEx(part, randomizeDesign));
                        return;
                    }
                }
            }

            base.RequestAddCASPart(part, randomizeDesign);
        }

        protected List<CASParts.Wrapper> SubGetVisibleCASParts(BodyTypes bodyType, uint categories)
        {
            CASLogic logic = mCASModel as CASLogic;

            logic.AdjustAvailableCategoriesForCASMode(ref categories);

            List<CASParts.Wrapper> results = new List<CASParts.Wrapper>();
            foreach(CASParts.Wrapper part in CASBase.sUnboxedParts)
            {
                if ((part.mPart.BodyType == bodyType) && OutfitUtils.PartMatchesSim(logic.mBuilder, categories, part.mPart))
                {
                    results.Add(part);
                }
            }
            return results;
        }

        public override ArrayList GetVisibleCASParts(BodyTypes bodyType, uint categories)
        {
            try
            {
                List<CASParts.Wrapper> list = SubGetVisibleCASParts(bodyType, categories);
                //ArrayList list = base.GetVisibleCASParts(bodyType, categories);

                StackTrace trace = new StackTrace(false);

                int blackListCount = 0;

                CASClothingCategory clothingCategory = CASClothingCategory.gSingleton;
                if (clothingCategory != null)
                {
                    // Fix for an issue where this listing is never cleared by the Core
                    if (clothingCategory.PartPresetsList != null)
                    {
                        clothingCategory.PartPresetsList.Clear();
                    }
                }

                bool truncate = false;

                if (list.Count > 0)
                {
                    bool found = false;

                    foreach (StackFrame frame in trace.GetFrames())
                    {
                        if (frame.GetMethod().DeclaringType == sDelayedCategoryUpdate)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        foreach (StackFrame frame in trace.GetFrames())
                        {
                            if ((frame.GetMethod().DeclaringType == sCASClothingCategory) &&
                                (frame.GetMethod().ToString() == "Void HideUnusedIcons()"))
                            {
                                truncate = true;
                                break;
                            }
                            else if ((frame.GetMethod().DeclaringType == sCASClothingCategory) &&
                                 (frame.GetMethod().ToString() == "Void LoadParts()"))
                            {
                                truncate = true;

                                if ((clothingCategory != null) && (clothingCategory.mClothingTypesGrid != null))
                                {
                                    clothingCategory.mClothingTypesGrid.Clear();
                                }

                                CASClothingCategoryEx.DelayedCategoryUpdate.Perform();
                                break;
                            }
                        }

                        if (truncate)
                        {
                            List<CASParts.Wrapper> newList = new List<CASParts.Wrapper>();

                            newList.Add(list[0]);

                            list = newList;
                        }
                    }
                }

                if ((!truncate) && (InvalidPartBooter.HasInvalidParts))
                {
                    SimBuilder builder = CASLogic.Instance.mBuilder;

                    CASAgeGenderFlags age = builder.Age;
                    CASAgeGenderFlags gender = builder.Gender;
                    CASAgeGenderFlags species = builder.Species;

                    List<CASParts.Wrapper> newList = new List<CASParts.Wrapper>();

                    foreach(CASParts.Wrapper part in list)
                    {
                        InvalidPartBase.Reason reason = InvalidPartBooter.Allow(part, age, gender, species, false, (OutfitCategories)categories);
                        if (reason == InvalidPartBase.Reason.None)
                        {
                            newList.Add(part);
                        }
                        else
                        {
                            blackListCount++;
                        }
                    }

                    list.Clear();
                    list = newList;
                }

                ArrayList results = new ArrayList();

                foreach (CASParts.Wrapper part in list)
                {
                    results.Add(part.mPart);
                }

                return results;
            }
            catch (Exception e)
            {
                Common.Exception("GetVisibleCASParts", e);

                return new ArrayList();
            }
        }
    }
}