using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefThoughtBalloonManager : Dereference<ThoughtBalloonManager>
    {
        protected override DereferenceResult Perform(ThoughtBalloonManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mParent", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mParent);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
