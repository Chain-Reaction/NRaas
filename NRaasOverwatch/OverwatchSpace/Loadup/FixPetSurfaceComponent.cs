using NRaas.CommonSpace.Helpers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class FixPetSurfaceComponent : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("FixPetSurfaceComponent");

            int count = 0;

            if (!GameUtils.IsInstalled(ProductVersion.EP5))
            {
                foreach (Counter counter in Sims3.Gameplay.Queries.GetObjects<Counter>())
                {
                    if (counter.PetSurfaceComponent == null)
                    {
                        counter.AddComponent<PetSurfaceComponent>(new object[] { new PetSurfaceData(Slot.None, Cabinetry.kContainmentSlotCenterBase, PetSurfaceData.JumpStyle.RadialRouteContainmentForwardOnly), PetSurfaceComponent.kAdultAndElderCats });

                        count++;
                    }
                }

                Overwatch.Log("  Corrected: " + count);        }
            }
    }
}
