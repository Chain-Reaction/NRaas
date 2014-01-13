using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
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
    public class CleanupAutonomy : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupAutonomy");

            if (AutonomyManager.sInstance == null)
            {
                foreach (Sim sim in LotManager.Actors)
                {
                    if (sim.Autonomy == null)
                    {
                        sim.mAutonomy = new Autonomy(sim.FullName, sim);

                        Overwatch.Log("Restarted Autonomy " + sim.FullName);
                    }

                    if (sim.Autonomy.PreferredVenues == null)
                    {
                        sim.Autonomy.PreferredVenues = new Dictionary<IMetaObject, bool>();

                        Overwatch.Log("Restarted PreferredVenues " + sim.FullName);
                    }
                }

                AutonomyManager.Startup();

                Overwatch.Log("Autonomy Manager Restarted");
            }
        }
    }
}
