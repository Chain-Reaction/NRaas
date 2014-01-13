using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class PerformanceCareerShowStageLayouts : Dereference<PerformanceCareer.ShowStageLayouts>
    {
        protected override DereferenceResult Perform(PerformanceCareer.ShowStageLayouts reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mLargeStageLayout", field, objects))
            {
                RemoveValues(reference.mLargeStageLayout, objects);

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
