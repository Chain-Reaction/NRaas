using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSingingInfo : Dereference<SingingInfo>
    {
        protected override DereferenceResult Perform(SingingInfo reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimDesc", field, objects))
            {
                //Remove(ref reference.mSimDesc);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
