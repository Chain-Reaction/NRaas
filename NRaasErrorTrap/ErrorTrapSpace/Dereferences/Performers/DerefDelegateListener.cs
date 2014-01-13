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
    public class DerefDelegateListener : Dereference<DelegateListener>
    {
        protected override DereferenceResult Perform(DelegateListener reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mProcessEvent", field, objects))
            {
                RemoveDelegate<ProcessEventDelegate>(ref reference.mProcessEvent, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
