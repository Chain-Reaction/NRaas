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
    public class DerefBuffInstanceHairball : Dereference<BuffHairball.BuffInstanceHairball>
    {
        protected override DereferenceResult Perform(BuffHairball.BuffInstanceHairball reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "AlarmOwner", field, objects))
            {
                Remove(ref reference.AlarmOwner);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
