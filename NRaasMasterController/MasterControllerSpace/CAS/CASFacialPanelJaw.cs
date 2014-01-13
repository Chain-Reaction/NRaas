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
    public class CASFacialJaw : CASFacialBlendPanelAdjustment<CASJaw>
    {
        public CASFacialJaw()
        { }

        protected override CASJaw GetPanel()
        {
            return CASJaw.gSingleton;
        }

        protected override CASPhysicalState GetState()
        {
            return CASPhysicalState.Jaw;
        }

        protected override void SetAdvanced(CASJaw ths)
        {}

        protected override List<CategoryGrids> GetCategories(CASJaw panel)
        {
            List<CategoryGrids> categories = new List<CategoryGrids>();

            categories.Add(new CategoryGrids(FacialBlendCategories.Basic, panel.mOptionsGrid));
            categories.Add(new CategoryGrids(FacialBlendCategories.Chin, panel.mChinGrid));
            categories.Add(new CategoryGrids(FacialBlendCategories.Cheek, panel.mCheekGrid));
            categories.Add(new CategoryGrids(FacialBlendCategories.Jaw, panel.mJawGrid));

            return categories;
        }

        protected override FacialBlendRegions GetRegion()
        {
            return FacialBlendRegions.Jaw;
        }
    }
}
