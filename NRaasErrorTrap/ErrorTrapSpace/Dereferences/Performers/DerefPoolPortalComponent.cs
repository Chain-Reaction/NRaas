using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPoolPortalComponent : Dereference<PoolPortalComponent>
    {
        protected override DereferenceResult Perform(PoolPortalComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "OwnerPoolPortalObject", field, objects))
            {
                Remove(ref reference.OwnerPoolPortalObject);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
