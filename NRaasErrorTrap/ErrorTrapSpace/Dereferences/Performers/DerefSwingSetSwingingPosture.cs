using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSwingSetSwingingPosture : Dereference<SwingSet.SwingingPosture>
    {
        protected override DereferenceResult Perform(SwingSet.SwingingPosture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Sim", field, objects))
            {
                Remove(ref reference.Sim);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "Swing", field, objects))
            {
                Remove(ref reference.Swing);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
