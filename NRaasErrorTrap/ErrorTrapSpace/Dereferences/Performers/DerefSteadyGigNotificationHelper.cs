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
    public class DerefSteadyGigNotificationHelper : Dereference<SteadyGigNotificationHelper>
    {
        protected override DereferenceResult Perform(SteadyGigNotificationHelper reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSim", field, objects))
            {
                //Remove(ref reference.mSim);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
