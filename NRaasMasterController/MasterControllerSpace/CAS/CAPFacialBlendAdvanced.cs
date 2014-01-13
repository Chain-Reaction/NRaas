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
using Sims3.UI.CAS.CAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.CAS
{
    /*
    public class CAPFacialBlendAdvanced : CASFacialBlendPanelAdjustment<CAPFBDAdvanced>
    {
        public CAPFacialBlendAdvanced()
        { }

        protected override CAPFBDAdvanced GetPanel()
        {
            return CAPFBDAdvanced.gSingleton;
        }

        protected override CASPhysicalState GetState()
        {
            return GetPanel().sStartState;
        }

        protected override void SetAdvanced(CAPFBDAdvanced ths)
        {
        }

        protected override List<CategoryGrids> GetCategories(CAPFBDAdvanced panel)
        {
            List<CategoryGrids> categories = new List<CategoryGrids>();

            categories.Add(new CategoryGrids(new FacialBlendCategories[] { FacialBlendCategories.Basic, FacialBlendCategories.Global }, panel.mMiscGrid));
            categories.Add(new CategoryGrids(FacialBlendCategories.Upper, panel.mUpperGrid));
            categories.Add(new CategoryGrids(FacialBlendCategories.Lower, panel.mLowerGrid));

            return categories;
        }

        protected override FacialBlendRegions GetRegion()
        {
            return FacialBlendRegions.None;
        }
    }
    */
}
