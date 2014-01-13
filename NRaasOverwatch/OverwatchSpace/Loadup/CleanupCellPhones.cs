using NRaas.CommonSpace.Helpers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
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
    public class CleanupCellPhones : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupCellPhones");

            int removed = 0;

            foreach(Sim sim in LotManager.Actors)
            {
                PhoneCell truePhone = null;

                List<PhoneCell> remove = new List<PhoneCell>();
                foreach (PhoneCell phone in Inventories.InventoryDuoFindAll<PhoneCell,Phone>(sim.SimDescription))
                {
                    if (truePhone == null)
                    {
                        truePhone = phone;
                    }
                    else
                    {
                        remove.Add(phone);
                    }
                }

                foreach (PhoneCell phone in remove)
                {
                    try
                    {
                        phone.Dispose();
                        phone.Destroy();
                        removed++;
                    }
                    catch 
                    { }
                }
            }

            if (removed > 0)
            {
                Overwatch.Log("Duplicate Phones Removed: " + removed);
            }
        }
    }
}
