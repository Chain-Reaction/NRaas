using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefFireManager : Dereference<FireManager>
    {
        protected override DereferenceResult Perform(FireManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mWindowFires", field, objects))
            {
                Remove(reference.mWindowFires, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mObjectsToRestore", field, objects))
            {
                if (Performing)
                {
                    EmergencyFireRestorer obj = FindLast<EmergencyFireRestorer>(objects);
                    if (obj != null)
                    {
                        ErrorTrap.AddToBeDeleted(obj.mObject, true);
                    }

                    Remove(reference.mObjectsToRestore, objects);
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "mSimsReactedToFire", field, objects))
            {
                Remove(reference.mSimsReactedToFire, objects);
                return DereferenceResult.End;
            }

            ReferenceWrapper result;
            if (Matches(reference, "mFireSource", field, objects, out result) != MatchResult.Failure)
            {
                if (Performing)
                {
                    if (result.Valid)
                    {
                        Remove(ref reference.mFireSource);
                    }
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
