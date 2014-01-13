using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBeingRiddenPosture : Dereference<BeingRiddenPosture>
    {
        protected override DereferenceResult Perform(BeingRiddenPosture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mRider", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mMount.Posture = null;
                    }
                    catch
                    { }

                    Remove(ref reference.mRider);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mMount", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mRider.Posture = null;
                    }
                    catch
                    { }

                    Remove(ref reference.mMount);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
