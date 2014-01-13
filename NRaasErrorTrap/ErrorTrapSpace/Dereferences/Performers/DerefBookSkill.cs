using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBookSkill : Dereference<BookSkill>
    {
        protected override DereferenceResult Perform(BookSkill reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "mSimReading", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    Remove(ref reference.mSimReading);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
