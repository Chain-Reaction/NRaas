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
    public class DerefWritingRoyaltyAlarm : Dereference<Writing.RoyaltyAlarm>
    {
        protected override DereferenceResult Perform(Writing.RoyaltyAlarm reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSkill", field, objects))
            {
                //Remove(ref reference.mSkill);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
