using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefProprietorSimFestData : Dereference<Proprietor.SimFestData>
    {
        protected override DereferenceResult Perform(Proprietor.SimFestData reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mUpcomingNPCContestants", field, objects))
            {
                Remove(reference.mUpcomingNPCContestants, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mFinishedContestants", field, objects))
            {
                Remove(reference.mFinishedContestants, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mCurrentContestant", field, objects))
            {
                Remove(ref reference.mCurrentContestant);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
