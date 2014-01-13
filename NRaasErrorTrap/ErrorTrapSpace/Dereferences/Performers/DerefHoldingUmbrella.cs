using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefHoldingUmbrella : Dereference<Umbrella.HoldingUmbrella>
    {
        protected override DereferenceResult Perform(Umbrella.HoldingUmbrella reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mUmbrella", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mActor.Posture = null;
                    }
                    catch
                    { }

                    Remove(ref reference.mUmbrella);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
