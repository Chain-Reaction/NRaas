using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class SwitchOutfits
    {
        protected static bool ValidToSwitch(Sim sim)
        {
            if (sim == null) return false;

            if (sim.mPendingOutfitInfo.Category != OutfitCategories.None) return false;

            if (sim.HasBeenDestroyed) return false;

            if (sim.SocialComponent == null) return false;

            return true;
        }

        public static void SwitchNoSpin(Sim ths, Sim.ClothesChangeReason reason)
        {
            SwitchNoSpinTaskA.Perform(ths, reason);
        }
        public static void SwitchNoSpin(Sim ths, CASParts.Key key)
        {
            SwitchNoSpinTaskB.Perform(ths, key.mCategory, key.GetIndex(ths.SimDescription, false));
        }

        private static void SwitchToOutfitWithoutSpin(Sim ths, Sim.ClothesChangeReason reason)
        {
            SwitchToOutfitWithoutSpin(ths, reason, false);
        }
        private static void SwitchToOutfitWithoutSpin(Sim ths, Sim.ClothesChangeReason reason, bool ignoreCurrentCategory)
        {
            if (reason == Sim.ClothesChangeReason.RemovingOuterwear)
            {
                if (ths.OutfitCategoryToUseWhenSpinOutOfOuterwear == OutfitCategories.None)
                {
                    ths.OutfitCategoryToUseWhenSpinOutOfOuterwear = OutfitCategories.Everyday;
                }
            }

            OutfitCategories category;
            ths.GetOutfitForClothingChange(reason, out category);

            if (!ths.SwitchToOutfitTraitTest(reason, ref category)) 
            {
                return;
            }

            // MC adds swimwear for toddlers
            if (!ths.AcceptableClothingCategoryForAge(category))
            {
                if (ths.SimDescription.Toddler && category != OutfitCategories.Swimwear)
                {
                    return;
                }                
            }

            if (ths.SimDescription.OccultManager.DisallowClothesChange())
            {
                return;
            }
                
            if (ths.BuffManager.DisallowClothesChange(reason, category))
            {
                return;                    
            }
                
            if ((!ignoreCurrentCategory && (category == ths.CurrentOutfitCategory)))
            {
                return;                    
            }

            int num;
            ths.GetCategoryAndIndexToUse(reason, ref category, out num);
            ths.SwitchToOutfitWithoutSpin(category, num);
        }
        private static bool SwitchToOutfitWithoutSpin(Sim ths, OutfitCategories category, int index)
        {
            if (!ths.AcceptableClothingCategoryForAge(category))
            {
                if (ths.SimDescription.Toddler && category != OutfitCategories.Swimwear)
                {
                    return false;
                }
            }

            if (!ths.SwitchToOutfitTraitTest(Sim.ClothesChangeReason.Force, ref category))
            {
                return false;
            }

            SimOutfit outfit = CASParts.GetOutfit(ths.mSimDescription, new CASParts.Key(category, index), false);
            if ((outfit == null) || (outfit.Key == ResourceKey.kInvalidResourceKey))
            {
                return false;
            }

            if (ths.SimDescription.IsSupernaturalForm)
            {
                if (ths.BuffManager.TransformBuffInst != null)
                {
                    if (ths.BuffManager.TransformBuffInst.GenerateTransformOutfit(outfit))
                    {
                        int superIndex = ths.SimDescription.GetOutfitCount(OutfitCategories.Supernatural) - 0x1;
                        outfit = CASParts.GetOutfit(ths.SimDescription, new CASParts.Key(OutfitCategories.Supernatural, superIndex), false);
                    }
                }
            }

            ths.SwitchToOutfitWithoutSpin(category, outfit, index);
            return true;
        }

        public class SwitchNoSpinTaskA : Common.FunctionTask
        {
            Sim mSim;
            Sim.ClothesChangeReason mReason;

            protected SwitchNoSpinTaskA(Sim sim, Sim.ClothesChangeReason reason)
            {
                mSim = sim;
                mReason = reason;
            }

            public static void Perform(Sim sim, Sim.ClothesChangeReason reason)
            {
                new SwitchNoSpinTaskA(sim, reason).AddToSimulator();
            }

            protected override void OnPerform()
            {
                try
                {
                    if (!ValidToSwitch(mSim)) return;

                    SwitchToOutfitWithoutSpin(mSim, mReason);
                }
                catch (Exception e)
                {
                    Common.Exception(mSim, e);
                }
            }
        }

        public class SwitchNoSpinTaskB : Common.FunctionTask
        {
            Sim mSim;
            OutfitCategories mCategory;
            int mIndex;

            protected SwitchNoSpinTaskB(Sim sim, OutfitCategories category, int index)
            {
                mSim = sim;
                mCategory = category;
                mIndex = index;
            }

            public static void Perform(Sim sim, OutfitCategories category, int index)
            {
                new SwitchNoSpinTaskB(sim, category, index).AddToSimulator();
            }

            protected override void OnPerform()
            {
                try
                {
                    if (!ValidToSwitch(mSim)) return;

                    SwitchToOutfitWithoutSpin(mSim, mCategory, mIndex);
                }
                catch (Exception e)
                {
                    Common.Exception(mSim, e);
                }
            }
        }
    }
}

