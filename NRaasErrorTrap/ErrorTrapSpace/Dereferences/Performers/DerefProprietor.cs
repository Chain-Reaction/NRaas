using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefProprietor : Dereference<Proprietor>
    {
        protected override DereferenceResult Perform(Proprietor reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCustomers", field, objects))
            {
                Remove(reference.mCustomers, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
