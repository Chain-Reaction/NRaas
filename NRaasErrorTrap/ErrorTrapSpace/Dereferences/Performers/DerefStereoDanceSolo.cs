using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefStereoDanceSolo : Dereference<Stereo.DanceSolo>
    {
        protected override DereferenceResult Perform(Stereo.DanceSolo reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mDancingJig", field, objects))
            {
                //Remove(ref reference.mDancingJig);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mReservedJig", field, objects))
            {
                //Remove(ref reference.mReservedJig);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
