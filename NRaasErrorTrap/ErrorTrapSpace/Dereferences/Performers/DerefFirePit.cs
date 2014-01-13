using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DereFirePit : Dereference<FirePit>
    {
        protected override DereferenceResult Perform(FirePit reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mFreeChairs", field, objects))
            {
                Remove(reference.mFreeChairs, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mFreeChairsReserved", field, objects))
            {
                Remove(reference.mFreeChairsReserved, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mFreeChairsToDestroy", field, objects))
            {
                Remove(reference.mFreeChairsToDestroy, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSituation", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mSituation.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mSituation);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
