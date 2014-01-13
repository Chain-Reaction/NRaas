using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefArboretum : Dereference<Arboretum>
    {
        protected override DereferenceResult Perform(Arboretum reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mGardeningContestants", field, objects))
            {
                Remove(reference.mGardeningContestants, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
