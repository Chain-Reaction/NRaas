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
    public class DerefEvent : Dereference<Event>
    {
        protected override DereferenceResult Perform(Event reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            DereferenceResult result = DereferenceResult.Failure;

            if (Matches(reference, "mActor", field, objects))
            {
                Remove(ref reference.mActor);
                result = DereferenceResult.Continue;
            }

            if (Matches(reference, "mTargetObject", field, objects))
            {
                Remove(ref reference.mTargetObject);
                result = DereferenceResult.Continue;
            }

            return result;
        }
    }
}
