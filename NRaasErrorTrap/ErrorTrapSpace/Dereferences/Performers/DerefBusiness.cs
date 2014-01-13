using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBusiness : Dereference<Business>
    {
        protected override DereferenceResult Perform(Business reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "SimAffectedByPrank", field, objects))
            {
                Remove(ref reference.SimAffectedByPrank);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
