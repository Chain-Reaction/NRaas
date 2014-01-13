using NRaas.CommonSpace.Tasks;
using NRaas.MasterControllerSpace.CAS;
using NRaas.MasterControllerSpace.Helpers;
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
using Sims3.UI.CAS.CAP;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Tasks
{
    public class EditTownLibrary : Common.IPreLoad
    {
        public void OnPreLoad()
        {
            EditTownLibraryTask.Create<EditTownLibraryTask>();
        }

        protected class EditTownLibraryTask : RepeatingTask
        {
            protected override bool OnPerform()
            {
                Common.StringBuilder msg = new Common.StringBuilder("EditTownLibraryTask" + Common.NewLine);

                try
                {

                }
                catch (Exception e)
                {
                    Common.Exception(msg, e);
                }
                return true;
            }
        }
    }
}
