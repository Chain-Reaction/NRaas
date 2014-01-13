using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefCauseEffectService : Dereference<CauseEffectService>
    {
        protected override DereferenceResult Perform(CauseEffectService reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mEventListeners", field, objects))
            {
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
