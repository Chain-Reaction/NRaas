using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSimDescriptionEvent : Dereference<SimDescriptionEvent>
    {
        protected override DereferenceResult Perform(SimDescriptionEvent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "SimDescription", field, objects))
            {
                Remove(ref reference.SimDescription);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
