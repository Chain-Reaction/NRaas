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
    public class CheckFish : Check<Fish>
    {
        protected override bool PrePerform(Fish fish, bool postLoad)
        {
            if (fish.mData == null)
            {
                if (fish.mFishType == FishType.None)
                {
                    // The proper type will be set later, we simply need to stop it from bouncing until then
                    fish.mFishType = FishType.Anchovy;

                    fish.SetupData();

                    //DebugLogCorrection("Fish Type Corrected");
                }
            }

            return true;
        }
    }
}
