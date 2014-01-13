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
    public class DerefBoatOwnable : Dereference<BoatOwnable>
    {
        protected override DereferenceResult Perform(BoatOwnable reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mPreviousPassengers", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(reference.mPreviousPassengers, objects);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mPreviousCarriedPassengers", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(reference.mPreviousCarriedPassengers, objects);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
