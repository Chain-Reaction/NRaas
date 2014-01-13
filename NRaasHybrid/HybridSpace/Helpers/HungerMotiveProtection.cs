using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
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
    public class HungerMotiveProtection : IDisposable
    {
        Sim mSim;

        bool mAltered;

        public HungerMotiveProtection(Sim sim)
        {
            mSim = sim;

            if (sim.Motives.GetMotive(CommodityKind.Hunger) == null)
            {
                mAltered = true;
                sim.Motives.CreateMotive(CommodityKind.Hunger);
                sim.Motives.ForceSetMax(CommodityKind.Hunger);
            }
            else
            {
                mAltered = false;
            }
        }

        public void Dispose()
        {
            if (mAltered)
            {
                if (mSim.Motives != null)
                {
                    mSim.Motives.RemoveMotive(CommodityKind.Hunger);
                }
            }
        }
    }
}
