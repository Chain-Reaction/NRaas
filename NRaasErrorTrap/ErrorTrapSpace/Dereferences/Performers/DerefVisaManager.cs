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
    public class DerefVisaManager : Dereference<VisaManager>
    {
        protected override DereferenceResult Perform(VisaManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimDescription", field, objects))
            {
                //Remove(ref reference.mSimDescription);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mValues", field, objects))
            {
                RemoveValues(reference.mValues, objects);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
