using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Misc;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBoxOfDanger : Dereference<BoxOfDanger>
    {
        protected override DereferenceResult Perform(BoxOfDanger reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mStatue", field, objects))
            {
                if (Performing)
                {
                    Remove(ref reference.mStatue);
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
