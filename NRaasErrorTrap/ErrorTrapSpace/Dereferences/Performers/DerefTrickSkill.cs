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
    public class DerefTrickSkill : Dereference<TrickSkill>
    {
        protected override DereferenceResult Perform(TrickSkill reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mTricks", field, objects))
            {
                RemoveValues(reference.mTricks, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
