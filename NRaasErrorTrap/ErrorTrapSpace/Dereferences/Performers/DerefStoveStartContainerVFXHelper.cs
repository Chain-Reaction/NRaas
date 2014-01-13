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
    public class DerefStoveStartContainerVFXHelper : Dereference<Stove.StartContainerVFXHelper>
    {
        protected override DereferenceResult Perform(Stove.StartContainerVFXHelper reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mContainer", field, objects))
            {
                Remove(ref reference.mContainer);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
