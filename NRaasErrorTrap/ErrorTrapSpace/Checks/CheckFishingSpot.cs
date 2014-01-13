using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Checks
{
    public class CheckFishingSpot : Check<FishingSpot>
    {
        protected override bool PrePerform(FishingSpot spot, bool postLoad)
        {
            bool success = true;
            try
            {
                spot.GetClonedData();
            }
            catch
            {
                success = false;
            }

            if (!success)
            {
                Fish.sFishData.Clear();
                FishHere.sBoxData.Clear();
                FishHere.sBoxChances.Clear();
                FishingSpot.sGlobalFishingData.Clear();

                try
                {
                    Fish.ParseFishData();
                }
                catch (Exception e)
                {
                    Common.DebugException(spot.GetType().ToString(), e);
                }
            }

            return true;
        }
    }
}
