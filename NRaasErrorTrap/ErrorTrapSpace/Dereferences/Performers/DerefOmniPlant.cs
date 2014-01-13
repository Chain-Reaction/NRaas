using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefOmniPlant : Dereference<OmniPlant>
    {
        protected override DereferenceResult Perform(OmniPlant reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "FedObject", field, objects))
            {
                // Causes a script error in OmniPlant:PostHarvest
                //Remove(ref reference.FedObject);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
