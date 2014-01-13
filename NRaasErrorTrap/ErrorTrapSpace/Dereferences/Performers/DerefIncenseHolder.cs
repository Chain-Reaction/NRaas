using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefIncenseHolder : Dereference<IncenseHolder>
    {
        protected override DereferenceResult Perform(IncenseHolder reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBroadcaster", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mBroadcaster.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mBroadcaster);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
