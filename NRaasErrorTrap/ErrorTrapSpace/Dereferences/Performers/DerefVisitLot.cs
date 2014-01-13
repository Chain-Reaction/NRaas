using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefVisitLot : Dereference<VisitLot>
    {
        protected override DereferenceResult Perform(VisitLot reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mFollowers", field, objects))
            {
                Remove(reference.mFollowers, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
