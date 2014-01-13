using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSchool : Dereference<School>
    {
        protected override DereferenceResult Perform(School reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "OwnersHomework", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.OwnersHomework);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "SimToBringOver", field, objects))
            {
                Remove(ref reference.SimToBringOver);
                return DereferenceResult.End;
            }

            if (Matches(reference, "SimToVisit", field, objects))
            {
                Remove(ref reference.SimToVisit);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
