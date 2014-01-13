using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPunishmentTimeOutObject : Dereference<Punishment.TimeOutObject>
    {
        protected override DereferenceResult Perform(Punishment.TimeOutObject reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "WallObject", field, objects))
            {
                Remove(ref reference.WallObject);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
