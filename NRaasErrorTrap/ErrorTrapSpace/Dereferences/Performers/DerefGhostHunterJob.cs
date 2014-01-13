using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefGhostHunterJob : Dereference<GhostHunter.GhostHunterJob>
    {
        protected override DereferenceResult Perform(GhostHunter.GhostHunterJob reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSims", field, objects))
            {
                Remove(reference.mSims, objects);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mCreatedObjects", field, objects))
            {
                Remove(reference.mCreatedObjects, objects);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
