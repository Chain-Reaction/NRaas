using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Checks
{
    public class CheckMoondial : Check<MoonDial>
    {
        protected override bool PrePerform(MoonDial dial, bool postLoad)
        {
            if ((World.GetLunarPhase() < 0) || (World.GetLunarPhase() >= dial.mLunarFXLookUp.Length))
            {
                World.SetLunarPhase(0);

                ErrorTrap.LogCorrection("LunarPhase Repaired");
            }

            return true;
        }
    }
}
