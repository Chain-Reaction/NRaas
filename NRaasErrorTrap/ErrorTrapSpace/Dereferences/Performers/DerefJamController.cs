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
    public class DerefJamController : Dereference<JamController>
    {
        protected override DereferenceResult Perform(JamController reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mJammers", field, objects))
            {
                Remove(reference.mJammers, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
