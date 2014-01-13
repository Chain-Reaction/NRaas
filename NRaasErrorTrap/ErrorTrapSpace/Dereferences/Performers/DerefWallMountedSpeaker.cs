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
    public class DerefWallMountedSpeaker : Dereference<WallMountedSpeaker>
    {
        protected override DereferenceResult Perform(WallMountedSpeaker reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mDancers", field, objects))
            {
                Remove(reference.mDancers, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mStereoMaster", field, objects))
            {
                Remove(ref reference.mStereoMaster);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
