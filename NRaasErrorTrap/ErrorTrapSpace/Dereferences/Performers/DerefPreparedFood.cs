using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPreparedFood : Dereference<PreparedFood>
    {
        protected override DereferenceResult Perform(PreparedFood reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mDisgustedBroadcaster", field, objects))
            {
                try
                {
                    reference.mDisgustedBroadcaster.Dispose();
                }
                catch
                { }

                Remove(ref reference.mDisgustedBroadcaster);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
