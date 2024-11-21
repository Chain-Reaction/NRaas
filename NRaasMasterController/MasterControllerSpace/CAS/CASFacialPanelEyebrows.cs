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
    public class CASFacialPanelEyebrows : CASFacialBlendPanelAdjustment<CASEyebrows>
    {
        public CASFacialPanelEyebrows()
        { }

        protected override CASEyebrows GetPanel()
        {
            return CASEyebrows.gSingleton;
        }

        protected override CASPhysicalState GetState()
        {
            return CASPhysicalState.Eyebrows;
        }

        protected override void SetAdvanced(CASEyebrows panel)
        {
            Slider slider = panel.GetChildByID(0x5db9715, true) as Slider;

            slider.MaxValue = 256 * NRaas.MasterController.Settings.mSliderMultiple;

            List<BlendUnit> list = new List<BlendUnit>(CASController.Singleton.BlendUnits);
            foreach (BlendUnit unit in list)
            {
                if ((unit.Region == FacialBlendRegions.Eyelashes) && (unit.Category == FacialBlendCategories.Global))
                {
                    FacialBlendData facialBlendData = panel.GetFacialBlendData(unit);

                    if (facialBlendData.mBidirectional)
                    {
                        slider.MinValue = -256;
                    }
                    else
                    {
                        slider.MinValue = 0x0;
                    }
                    slider.Value = (int)Math.Round((double)(facialBlendData.Value * 256f));
                    break;
                }
            }
        }

        protected override List<CategoryGrids> GetCategories(CASEyebrows panel)
        {
            return null;
        }

        protected override FacialBlendRegions GetRegion()
        {
            return FacialBlendRegions.Eyelashes;
        }
    }
}
