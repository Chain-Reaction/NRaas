using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefFieldTripSituation : Dereference<FieldTripSituation>
    {
        protected override DereferenceResult Perform(FieldTripSituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;

            if (Matches(reference, "mRabbitHole", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(ref reference.mRabbitHole);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mAdult", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        try
                        {
                            reference.Exit();
                        }
                        catch
                        { }

                        Remove(ref reference.mAdult);
                    }
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mAdultsThatFailedToShow", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(reference.mAdultsThatFailedToShow, objects);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mStudents", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(reference.mStudents, objects);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mStudentsLeft", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(reference.mStudentsLeft, objects);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
