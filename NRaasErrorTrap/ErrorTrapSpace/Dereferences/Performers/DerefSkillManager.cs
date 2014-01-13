using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSkillManager : Dereference<SkillManager>
    {
        protected override DereferenceResult Perform(SkillManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mValues", field, objects))
            {
                //RemoveValues(reference.mValues, objects);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mSimDescription", field, objects))
            {
                //Remove(ref reference.mSimDescription);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "mSkillMeter", field, objects))
            {
                Remove(ref reference.mSkillMeter);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
