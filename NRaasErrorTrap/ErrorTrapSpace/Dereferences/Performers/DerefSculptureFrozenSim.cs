using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations.Mimics;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSculptureFrozenSim : Dereference<SculptureFrozenSim>
    {
        protected override DereferenceResult Perform(SculptureFrozenSim reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mFrozenSim", field, objects))
            {
                Remove(ref reference.mFrozenSim);

                if (Performing)
                {
                    ErrorTrap.AddToBeDeleted(reference, true);
                }

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
