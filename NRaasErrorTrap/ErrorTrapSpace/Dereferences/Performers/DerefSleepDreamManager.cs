using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSleepDreamManager : Dereference<SleepDreamManager>
    {
        protected override DereferenceResult Perform(SleepDreamManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mOwner", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mOwner.SleepDreamManager = null;
                    }
                    catch
                    { }

                    Remove(ref reference.mOwner);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
