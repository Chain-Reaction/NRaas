using NRaas.CommonSpace.Helpers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupCelebrityLevel : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupCelebrityLevel");

            CelebrityLevelStaticData topLevel = null;
            if (CelebrityUtil.sCelebrityLevelData != null)
            {
                topLevel = CelebrityUtil.sCelebrityLevelData[CelebrityManager.HighestLevel];
            }

            foreach (SimDescription sim in SimListing.GetResidents(true).Values)
            {
                if (sim.CelebrityManager == null)
                {
                    sim.Fixup();
                }

                if (topLevel != null)
                {
                    if (sim.CelebrityManager.Points > topLevel.GoalPoints)
                    {
                        sim.CelebrityManager.mPoints = topLevel.GoalPoints;

                        Overwatch.Log("Celebrity Points Reset: " + sim.FullName);
                    }
                }

                if (sim.CelebrityLevel > CelebrityManager.HighestLevel)
                {
                    sim.CelebrityManager.mLevel = CelebrityManager.HighestLevel;

                    Overwatch.Log("Celebrity Level Reset: " + sim.FullName);
                }
            }
        }
    }
}
