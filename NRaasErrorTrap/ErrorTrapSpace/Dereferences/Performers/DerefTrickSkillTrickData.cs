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
    public class DerefTrickSkillTrickData : Dereference<TrickSkill.TrickData>
    {
        protected override DereferenceResult Perform(TrickSkill.TrickData reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mTrickSkill", field, objects))
            {
                Remove(ref reference.mTrickSkill);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
