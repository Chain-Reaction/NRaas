using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefOccultMummy : Dereference<OccultMummy>
    {
        protected override DereferenceResult Perform(OccultMummy reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mOwningSim", field, objects))
            {
                Remove(ref reference.mOwningSim);
                return DereferenceResult.End;
            }

            if (Matches(reference, "SimsReacted", field, objects))
            {
                Remove(reference.SimsReacted, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
