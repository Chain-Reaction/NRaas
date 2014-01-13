using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefStylist : Dereference<Stylist>
    {
        protected override DereferenceResult Perform(Stylist reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Portfolio", field, objects))
            {
                Remove(ref reference.Portfolio);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
