using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Socializing;
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
    public class Investments
    {
        public static int Collect(Sim sim)
        {
            int totalFunds = 0;

            foreach (PropertyData data in sim.Household.RealEstateManager.AllProperties)
            {
                if (data.CurrentCollectibleFunds <= 0) continue;

                if (data.World != GameUtils.GetCurrentWorld()) continue;

                try
                {
                    totalFunds += data.CurrentCollectibleFunds;
                    data.CollectedMoney(sim);
                }
                catch (Exception e)
                {
                    Common.DebugException(sim, e);
                }
            }

            return totalFunds;
        }
    }
}

