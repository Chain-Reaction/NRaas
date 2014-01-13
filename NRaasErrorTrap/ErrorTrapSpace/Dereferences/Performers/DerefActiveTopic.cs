using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefActiveTopic : Dereference<ActiveTopic>
    {
        protected override DereferenceResult Perform(ActiveTopic reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mParameter", field, objects))
            {
                //Remove(ref reference.mParameter);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
