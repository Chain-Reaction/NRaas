using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Fireplaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DereFireplace : Dereference<Fireplace>
    {
        protected override DereferenceResult Perform(Fireplace reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCozyFireEffectBroadcast", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.StopFire();
                    }
                    catch
                    { }

                    Remove(ref reference.mCozyFireEffectBroadcast);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
