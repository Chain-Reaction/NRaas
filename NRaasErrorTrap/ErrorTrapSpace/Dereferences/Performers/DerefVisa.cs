using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefVisa : Dereference<Visa>
    {
        protected override DereferenceResult Perform(Visa reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mVisaOwner", field, objects))
            {
                //Remove(ref reference.mVisaOwner);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
