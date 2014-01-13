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
    public class DerefOccupation : Dereference<Occupation>
    {
        protected override DereferenceResult Perform(Occupation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "OwnerDescription", field, objects))
            {
                //Remove(ref reference.OwnerDescription);
                return DereferenceResult.End;
            }

            if (Matches(reference, "Coworkers", field, objects))
            {
                Remove(reference.Coworkers, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "Boss", field, objects))
            {
                Remove(ref reference.Boss);
                return DereferenceResult.End;
            }

            ReferenceWrapper result;
            if (Matches(reference, "FormerBoss", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.FormerBoss);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mJobs", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        Job job = Find<Job>(objects);
                        if (job != null)
                        {
                            reference.RemoveJob(job, true);
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(reference.OwnerDescription, e);
                    }

                    RemoveValues(reference.mJobs, objects);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
