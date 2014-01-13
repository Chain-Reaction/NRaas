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

namespace NRaas.MasterControllerSpace.Helpers
{
    public class PlayFlowMenuPanelEx
    {
        protected static CASMode OnGetMode(SimDescription sim, ref OutfitCategories startCategory, ref int startIndex, ref Sims.CASBase.EditType editType)
        {
            return CASMode.Full;
        }

        public static void OnMenuButtonClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                PlayFlowMenuPanel playFlow = PlayFlowMenuPanel.gSingleton;

                if (sender == playFlow.mCASButton)
                {
                    (playFlow.Model as PlayFlowModel).mSelectedBinContentId = ulong.MaxValue;

                    Sims.Advanced.EditInCAS.Perform((SimDescription)null, OnGetMode);
                }
                else
                {
                    playFlow.OnMenuButtonClick(sender, eventArgs);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnMenuButtonClick", e);
            }
        }
    }
}
