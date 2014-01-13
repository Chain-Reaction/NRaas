using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBarProfessional : Dereference<BarProfessional>
    {
        protected override DereferenceResult Perform(BarProfessional reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBartender", field, objects))
            {
                Remove(ref reference.mBartender);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mExtenderEast", field, objects))
            {
                Remove(ref reference.mExtenderEast);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mExtenderWest", field, objects))
            {
                Remove(ref reference.mExtenderWest);
                return DereferenceResult.End;
            }

            ReferenceWrapper result;
            if (Matches(reference, "mBarCounters", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(reference.mBarCounters, objects);
                }

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
