using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.PerformanceObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefShowStage : Dereference<ShowStage>
    {
        protected override DereferenceResult Perform(ShowStage reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPerformingSim", field, objects))
            {
                Remove(ref reference.mPerformingSim);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mShellDoorForProprietorToReEnter", field, objects))
            {
                Remove(ref reference.mShellDoorForProprietorToReEnter);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mVenueStageLayout", field, objects))
            {
                RemoveValues(reference.mVenueStageLayout, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSlaveList", field, objects))
            {
                Remove(reference.mSlaveList, objects);
                return DereferenceResult.End;
            }

            ReferenceWrapper result;
            if (Matches(reference, "mProprietor", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        try
                        {
                            reference.RemoveRoleGivingInteraction(reference.mProprietor);
                        }
                        catch
                        { }

                        Remove(ref reference.mProprietor);
                    }
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
