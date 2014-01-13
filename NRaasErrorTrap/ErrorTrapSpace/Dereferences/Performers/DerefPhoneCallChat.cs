using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPhoneCallChat : Dereference<Phone.CallChat>
    {
        protected override DereferenceResult Perform(Phone.CallChat reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            DereferenceResult result = DereferenceResult.Failure;

            if (Matches(reference, "mHandset", field, objects))
            {
                Remove(ref reference.mHandset);
                result = DereferenceResult.Continue;
            }

            return result;
        }
    }
}
