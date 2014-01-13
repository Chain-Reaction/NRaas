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
    public class EditTownLibraryPanelEx
    {
        public static void OnCASClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                Common.FunctionTask.Perform(GotoCASMode);
            }
            catch (Exception e)
            {
                Common.Exception("OnCASClick", e);
            }
        }

        protected static CASMode OnGetMode(SimDescription sim, ref OutfitCategories startCategory, ref int startIndex, ref Sims.CASBase.EditType editType)
        {
            return CASMode.Full;
        }

        private static void GotoCASMode()
        {
            try
            {
                if (EditTownPuck.Instance == null) return;

                if (!EditTownPuck.Instance.ExitingGameEntry)
                {
                    EditTownPuck.Instance.UpdateBackButton(true);

                    try
                    {
                        IEditTownModel model = EditTownLibraryPanel.Instance.mModel;

                        if (model.IsLoading || Responder.Instance.OptionsModel.SaveGameInProgress)
                        {
                            return;
                        }

                        Sims.Advanced.EditInCAS.Perform((SimDescription)null, OnGetMode);
                    }
                    finally
                    {
                        EditTownPuck.Instance.UpdateBackButton(false);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("GotoCASMode", e);
            }
        }
    }
}
