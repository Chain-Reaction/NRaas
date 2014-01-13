using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBroomStand : Dereference<BroomStand>
    {
        protected override DereferenceResult Perform(BroomStand reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mReservedBroom", field, objects))
            {
                Remove(ref reference.mReservedBroom);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
