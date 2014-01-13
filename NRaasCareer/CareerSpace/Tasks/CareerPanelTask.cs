using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.CareerSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Tasks
{
    public class CareerPanelTask : RepeatingTask
    {
        protected override bool OnPerform()
        {
            CareerPanel panel = CareerPanel.sInstance;
            if (panel == null) return true;

            Sim activeSim = Sim.ActiveActor;
            if (activeSim == null) return true;

            if (panel.mPanelCategory == CareerPanelCategory.kAfterschoolActivity)
            {
                if ((panel.mCareerInfoWindow != null) && (panel.mCareerInfoWindow.Visible))
                {
                    if (activeSim.SimDescription.Child)
                    {
                        School school = activeSim.School;
                        if (school != null)
                        {
                            List<AfterschoolActivity> activities = school.AfterschoolActivities;
                            if ((activities != null) && (activities.Count == 1))
                            {
                                switch (activities[0].CurrentActivityType)
                                {
                                    case AfterschoolActivityType.Ballet:
                                    case AfterschoolActivityType.Scouts:
                                        break;
                                    default:
                                        panel.mCurrentJobText.Caption = AfterschoolActivity.LocalizeString(activeSim.IsFemale, activities[0].CurrentActivityType.ToString(), new object[0]);
                                        panel.mCurrentJobText.TooltipText = AfterschoolActivity.LocalizeString(activeSim.IsFemale, activities[0].CurrentActivityType.ToString() + "Description", new object[0]);
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public class Loader : Common.IPreLoad
        {
            public void OnPreLoad()
            {
                CareerPanelTask.Create<CareerPanelTask>();
            }
        }
    }
}
