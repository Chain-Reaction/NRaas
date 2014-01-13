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
    public class CleanupDrinkRecipes : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupDrinkRecipes");

            if (HotBeverageMachine.SimDrinkRecipes != null)
            {
                List<SimDescription> remove = new List<SimDescription>();

                foreach (SimDescription sim in HotBeverageMachine.SimDrinkRecipes.Keys)
                {
                    if (!sim.IsValidDescription)
                    {
                        remove.Add(sim);
                    }
                }

                foreach (SimDescription sim in remove)
                {
                    HotBeverageMachine.SimDrinkRecipes.Remove(sim);

                    Overwatch.Log(" Removed Bad Sim: " + sim.FullName);
                }
            }
        }
    }
}
