using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPartComponent : Dereference<PartComponent>
    {
        protected override DereferenceResult Perform(PartComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            ReferenceWrapper result;
            if (Matches(reference, "OnPartAvailabilityChangedCallback", field, objects, out result) != MatchResult.Failure)
            {
                if (result.Valid)
                {
                    if (Performing)
                    {
                        PartComponent.PartAvailabilityChanged callback = FindLast<PartComponent.PartAvailabilityChanged>(objects);
                        if (callback != null)
                        {
                            reference.OnPartAvailabilityChangedCallback -= callback;
                        }
                    }
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
