using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefUniversityMascot : Dereference<UniversityMascot>
    {
        protected override DereferenceResult Perform(UniversityMascot reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            DereferenceResult reason = MatchAndRemove(reference, "mMascotDestroyedListener", field, ref reference.mMascotDestroyedListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
