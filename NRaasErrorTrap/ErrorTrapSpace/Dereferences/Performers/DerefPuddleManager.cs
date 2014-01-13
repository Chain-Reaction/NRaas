using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPuddleManager : Dereference<PuddleManager>
    {
        protected override DereferenceResult Perform(PuddleManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mFlowingPuddleAlarms", field, objects))
            {
                RemoveKeys(reference.mFlowingPuddleAlarms, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
