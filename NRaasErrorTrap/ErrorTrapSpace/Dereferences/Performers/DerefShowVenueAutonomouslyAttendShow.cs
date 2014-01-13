using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefShowVenueAutonomouslyAttendShow : Dereference<ShowVenue.AutonomouslyAttendShow>
    {
        protected override DereferenceResult Perform(ShowVenue.AutonomouslyAttendShow reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimFollowers", field, objects))
            {
                Remove(reference.mSimFollowers, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
