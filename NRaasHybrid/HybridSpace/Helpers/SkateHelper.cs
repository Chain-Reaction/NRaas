using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Helpers
{
    public class SkateHelper
    {
        public static bool CalculateIfActorIsOccultSkaterEx(Sim sim)
        {
            OccultImaginaryFriend friend;
            SimDescription simDescription = sim.SimDescription;

            if (!Hybrid.Settings.mAllowOccultSkating)
            {
                if ((simDescription.IsGenie) || (simDescription.IsFairy)) return true;
            }

            if ((simDescription.IsFrankenstein) || (simDescription.IsMummy) || (simDescription.IsZombie))
            {
                return true;
            }

            if (OccultImaginaryFriend.TryGetOccultFromSim(sim, out friend) && friend.IsWearingSpecialOutfit)
            {
                return true;
            }

            BuffManager buffManager = sim.BuffManager;
            if (buffManager.HasTransformBuff()) return true;

            if (!Hybrid.Settings.mAllowOccultSkating)
            {
                if (simDescription.IsVampire)
                {
                    if (!buffManager.HasAnyElement(new BuffNames[] { BuffNames.Exhausted, BuffNames.Sleepy, BuffNames.Tired, BuffNames.TooMuchSun }))
                    {
                        return true;
                    }
                }

                if (sim.IsGhostOrHasGhostBuff)
                {
                    return true;
                }
            }

            return false;
        }        
    }
}
