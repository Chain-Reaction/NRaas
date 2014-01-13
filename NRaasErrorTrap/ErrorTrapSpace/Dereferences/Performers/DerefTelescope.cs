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
    public class DerefTelescope : Dereference<Telescope>
    {
        protected override DereferenceResult Perform(Telescope reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCurrentSim", field, objects))
            {
                Remove(ref reference.mCurrentSim);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
