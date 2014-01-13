using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefLoungingPosture : Dereference<LoungingPosture>
    {
        protected override DereferenceResult Perform(LoungingPosture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mChair", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mSim.Posture = null;
                    }
                    catch
                    { }

                    Remove(ref reference.mChair);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
