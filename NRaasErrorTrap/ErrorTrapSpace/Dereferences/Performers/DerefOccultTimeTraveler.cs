using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefOccultTimeTraveler : Dereference<OccultTimeTraveler>
    {
        protected override DereferenceResult Perform(OccultTimeTraveler reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mOwningSim", field, objects, out result) != MatchResult.Failure)
            {
                Remove(ref reference.mOwningSim);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
