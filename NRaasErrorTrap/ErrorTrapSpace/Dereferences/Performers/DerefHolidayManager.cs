using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefHolidayManager : Dereference<HolidayManager>
    {
        protected override DereferenceResult Perform(HolidayManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            DereferenceResult reason = MatchAndRemove(reference, "mSeasonStartedListener", field, ref reference.mSeasonStartedListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                return reason;
            }

            reason = MatchAndRemove(reference, "mCurrentSeasonLengthChanged", field, ref reference.mSeasonStartedListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                return reason;
            }

            return DereferenceResult.Failure;
        }
    }
}
