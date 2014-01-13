using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Passport;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class Lots
    {
        public static int GetUnfurnishedCost(Lot lot)
        {
            int cost = lot.Cost;

            foreach (GameObject obj2 in lot.GetObjects<GameObject>())
            {
                if ((!obj2.StaysAfterEvict() && !obj2.IsInPublicResidentialRoom) && !obj2.IsInHiddenResidentialRoom)
                {
                    try
                    {
                        cost -= obj2.Value;
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(lot, e);
                    }
                }
            }

            return cost;
        }
    }
}

