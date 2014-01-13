using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSimPlaceholder : Dereference<Sim.Placeholder>
    {
        protected override DereferenceResult Perform(Sim.Placeholder reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimDescription", field, objects))
            {
                if (Performing)
                {
                    ErrorTrap.AddToBeDeleted(reference, true);

                    Remove(ref reference.mSimDescription);
                    return DereferenceResult.End;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
