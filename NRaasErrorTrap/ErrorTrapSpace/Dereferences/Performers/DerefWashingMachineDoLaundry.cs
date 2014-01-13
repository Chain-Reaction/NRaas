using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefWashingMachineDoLaundry : Dereference<WashingMachine.DoLaundry>
    {
        protected override DereferenceResult Perform(WashingMachine.DoLaundry reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mClothingPileBeingCarried", field, objects))
            {
                //Remove(ref reference.mClothingPileBeingCarried);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
