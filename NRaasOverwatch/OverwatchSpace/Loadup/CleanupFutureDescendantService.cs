using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupFutureDescendantService : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupFutureDescendantService");

            FutureDescendantService instance = FutureDescendantService.GetInstance();
            if (instance != null)
            {
                List<ulong> remove = new List<ulong>();

                foreach (ulong num in FutureDescendantService.sPersistableData.DescendantHouseholdsMap.Keys)
                {
                    IMiniSimDescription ancestorSim = SimDescription.GetIMiniSimDescription(num);
                    if (ancestorSim == null)
                    {
                        remove.Add(num);
                    }
                }

                foreach (ulong num in remove)
                {
                    FutureDescendantService.sPersistableData.DescendantHouseholdsMap.Remove(num);

                    Overwatch.Log(" Missing Removed: " + num);
                }
            }
        }
    }
}
