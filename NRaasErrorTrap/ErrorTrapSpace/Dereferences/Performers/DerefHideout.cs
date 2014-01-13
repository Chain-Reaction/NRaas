using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefHideout : Dereference<Hideout>
    {
        protected override DereferenceResult Perform(Hideout reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimToLastRaidTimestamp", field, objects))
            {
                RemoveKeys(reference.mSimToLastRaidTimestamp, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
