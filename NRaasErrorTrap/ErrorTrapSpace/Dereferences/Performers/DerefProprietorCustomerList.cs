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
    public class DerefProprietorCustomerList : Dereference<Proprietor.CustomerList>
    {
        protected override DereferenceResult Perform(Proprietor.CustomerList reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCustomer", field, objects))
            {
                Remove(ref reference.mCustomer);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mDrinkBoughtForBy", field, objects))
            {
                Remove(ref reference.mDrinkBoughtForBy);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
