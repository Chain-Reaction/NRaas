using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefCribInCribPosture : Dereference<Crib.InCribPosture>
    {
        protected override DereferenceResult Perform(Crib.InCribPosture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Child", field, objects))
            {
                Remove(ref reference.Child);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "Crib", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Child.Posture = null;
                    }
                    catch
                    { }

                    Remove(ref reference.Crib);
                }

                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
