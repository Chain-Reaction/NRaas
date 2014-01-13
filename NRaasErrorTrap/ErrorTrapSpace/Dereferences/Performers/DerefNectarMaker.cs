using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefNectarMaker : Dereference<NectarMaker>
    {
        protected override DereferenceResult Perform(NectarMaker reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBottles", field, objects))
            {
                Remove(reference.mBottles, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mLastSimToMake", field, objects))
            {
                Remove(ref reference.mLastSimToMake);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mCurrentStateMachine", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.UpdateStateOnInterrupt();
                    }
                    catch
                    { }

                    Remove(ref reference.mCurrentStateMachine);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
