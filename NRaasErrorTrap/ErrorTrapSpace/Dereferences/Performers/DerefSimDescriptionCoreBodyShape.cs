using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSimDescriptionCoreBodyShape : Dereference<SimDescriptionCore.BodyShape>
    {
        protected override DereferenceResult Perform(SimDescriptionCore.BodyShape reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Owner", field, objects))
            {
                //Remove(ref reference.Owner);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
