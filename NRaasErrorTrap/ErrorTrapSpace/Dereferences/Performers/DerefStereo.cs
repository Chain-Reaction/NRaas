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
    public class DerefStereo : Dereference<Stereo>
    {
        protected override DereferenceResult Perform(Stereo reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimsWithinStereoBroadcast", field, objects))
            {
                Remove(reference.mSimsWithinStereoBroadcast, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSimsListeningToStereo", field, objects))
            {
                Remove(reference.mSimsListeningToStereo, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
