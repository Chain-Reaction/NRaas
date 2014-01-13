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
    public class DerefBunkBedContainer : Dereference<BunkBedContainer>
    {
        protected override DereferenceResult Perform(BunkBedContainer reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "LowerBunk", field, objects))
            {
                Remove(ref reference.LowerBunk);
                return DereferenceResult.End;
            }

            if (Matches(reference, "UpperBunk", field, objects))
            {
                Remove(ref reference.UpperBunk);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mLoftBedFootprintSmall", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mLoftBedFootprintSmall.Destroy();
                    }
                    catch
                    { }

                    Remove(ref reference.mLoftBedFootprintSmall);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mLoftBedFootprintLarge", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mLoftBedFootprintLarge.Destroy();
                    }
                    catch
                    { }

                    Remove(ref reference.mLoftBedFootprintLarge);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
