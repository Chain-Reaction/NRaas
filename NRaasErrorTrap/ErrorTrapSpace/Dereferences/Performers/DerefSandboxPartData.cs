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
    public class DerefSandboxPartData : Dereference<Sandbox.SandboxPartData>
    {
        protected override DereferenceResult Perform(Sandbox.SandboxPartData reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Jig", field, objects))
            {
                Remove(ref reference.Jig);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
