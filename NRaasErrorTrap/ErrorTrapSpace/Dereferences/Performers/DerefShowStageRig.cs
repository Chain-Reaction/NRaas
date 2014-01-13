using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.PerformanceObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefShowStageRig : Dereference<ShowStageRig>
    {
        protected override DereferenceResult Perform(ShowStageRig reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mShowStage", field, objects))
            {
                Remove(ref reference.mShowStage);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
