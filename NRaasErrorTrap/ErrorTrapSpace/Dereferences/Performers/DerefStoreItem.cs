using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefStoreItem : Dereference<StoreItem>
    {
        protected override DereferenceResult Perform(StoreItem reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Callback", field, objects))
            {
                RemoveDelegate<CreateObjectCallback>(ref reference.Callback, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
