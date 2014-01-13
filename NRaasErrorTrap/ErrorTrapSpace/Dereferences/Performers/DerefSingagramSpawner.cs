using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSingagramSpawner : Dereference<SingagramSpawner>
    {
        protected override DereferenceResult Perform(SingagramSpawner reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSpec", field, objects))
            {
                //Remove(ref reference.mSpec);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
