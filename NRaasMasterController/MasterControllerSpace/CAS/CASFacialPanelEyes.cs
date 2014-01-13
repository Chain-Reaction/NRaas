using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.CAS
{
    public class CASFacialPanelEyes : CASFacialBlendPanelAdjustment<CASEyes>
    {
        public CASFacialPanelEyes()
        { }

        protected override CASEyes GetPanel()
        {
            return CASEyes.gSingleton;
        }

        protected override CASPhysicalState GetState()
        {
            return CASPhysicalState.Eyes;
        }

        protected override void SetAdvanced(CASEyes ths)
        {
            (ths.GetChildByID(0x5dbb901, true) as Button).Selected = false;
            (ths.GetChildByID(0x5dbb902, true) as Button).Selected = true;

            if (MasterController.Settings.mDefaultAdvancedPanel)
            {
                ths.mBasicsPanel.Visible = false;
                ths.mAdvancedPanel.Visible = true;
            }

            if (CASFacialDetails.gSingleton.mLongPanel != null)
            {
                CASFacialDetails.gSingleton.SetLongPanel(ths.mBasicsPanel.Visible);
                CASFacialDetails.gSingleton.SetShortPanelHeight(ths.mAdvancedPanel.Visible);
            }
        }

        protected override List<CategoryGrids> GetCategories(CASEyes panel)
        {
            List<CategoryGrids> categories = new List<CategoryGrids>();

            categories.Add(new CategoryGrids(new FacialBlendCategories[] { FacialBlendCategories.Basic, FacialBlendCategories.Global }, panel.mMiscGrid));
            categories.Add(new CategoryGrids(FacialBlendCategories.Eyelid, panel.mEyeLidGrid));
            categories.Add(new CategoryGrids(FacialBlendCategories.Brow, panel.mBrowGrid));
            categories.Add(new CategoryGrids(FacialBlendCategories.Eyeshape, panel.mEyeShapeGrid));

            return categories;
        }

        protected override FacialBlendRegions GetRegion()
        {
            return FacialBlendRegions.Eyes;
        }
    }
}
