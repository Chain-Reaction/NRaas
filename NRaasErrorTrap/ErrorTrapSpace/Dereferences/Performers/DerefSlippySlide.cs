using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSlippySlide : Dereference<SlippySlide>
    {
        protected override DereferenceResult Perform(SlippySlide reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSliderQueue", field, objects))
            {
                if (Performing)
                {
                    reference.mSliderQueue.Clear();
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
