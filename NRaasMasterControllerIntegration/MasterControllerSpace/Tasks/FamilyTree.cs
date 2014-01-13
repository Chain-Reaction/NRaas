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
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Tutorial;
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

namespace NRaas.MasterControllerSpace.Tasks
{
    public class FamilyTree : Common.IPreLoad
    {
        public void OnPreLoad()
        {
            FamilyTreeTask.Create<FamilyTreeTask>();
        }

        protected static void OnClickGenealogy(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                Common.FunctionTask.Perform(ShowFamilyTreeDialog);
            }
            catch (Exception e)
            {
                Common.Exception("OnClickGenealogy", e);
            }
        }

        public static void ShowFamilyTreeDialog()
        {
            try
            {
                NRaas.MasterControllerSpace.Sims.Basic.FamilyTree.Perform(Sim.ActiveActor.SimDescription);
            }
            catch (Exception e)
            {
                Common.Exception("ShowFamilyTreeDialog", e);
            }
        }

        protected class FamilyTreeTask : RepeatingTask
        {
            protected override bool OnPerform()
            {
                Sims3.UI.Hud.SimologyPanel panel = Sims3.UI.Hud.SimologyPanel.Instance;
                if ((panel != null) && (panel.Visible) && (panel.mGeneologyButton != null))
                {
                    panel.mGeneologyButton.Click -= panel.OnClickGenealogy;

                    panel.mGeneologyButton.Click -= OnClickGenealogy;
                    panel.mGeneologyButton.Click += OnClickGenealogy;
                }

                return true;
            }
        }
    }
}
