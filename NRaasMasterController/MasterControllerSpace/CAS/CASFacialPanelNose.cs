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
    public class CASFacialNose : CASFacialBlendPanelAdjustment<CASNose>
    {
        public CASFacialNose()
        { }

        protected override CASNose GetPanel()
        {
            return CASNose.gSingleton;
        }

        protected override CASPhysicalState GetState()
        {
            return CASPhysicalState.Nose;
        }

        protected override void SetAdvanced(CASNose ths)
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

        protected override List<CategoryGrids> GetCategories(CASNose panel)
        {
            List<CategoryGrids> categories = new List<CategoryGrids>();

            categories.Add(new CategoryGrids(new FacialBlendCategories[] { FacialBlendCategories.Basic, FacialBlendCategories.Global }, panel.mMiscGrid));
            categories.Add(new CategoryGrids(FacialBlendCategories.Bridge, panel.mBridgeGrid));
            categories.Add(new CategoryGrids(FacialBlendCategories.Tip, panel.mTipGrid));
            categories.Add(new CategoryGrids(FacialBlendCategories.Nostril, panel.mNostrilGrid));

            return categories;
        }

        protected override FacialBlendRegions GetRegion()
        {
            return FacialBlendRegions.Nose;
        }
    }
}
