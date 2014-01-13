using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefGnubbFieldSpot : Dereference<GnubbField.Spot>
    {
        protected override DereferenceResult Perform(GnubbField.Spot reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Knight", field, objects))
            {
                Remove(ref reference.Knight);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
