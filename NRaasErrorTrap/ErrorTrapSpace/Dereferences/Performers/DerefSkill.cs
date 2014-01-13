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
    public class DerefSkill : Dereference<Skill>
    {
        protected override DereferenceResult Perform(Skill reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSkillOwner", field, objects))
            {
                //Remove(ref reference.mSkillOwner);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "callbackObjects", field, objects))
            {
                Remove(reference.callbackObjects,objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
