using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefScienceComponent : Dereference<ScienceComponent>
    {
        protected override DereferenceResult Perform(ScienceComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBuffedSims", field, objects))
            {
                Remove(reference.mBuffedSims, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
