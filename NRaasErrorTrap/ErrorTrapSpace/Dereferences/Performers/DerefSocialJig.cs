using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSocialJig : Dereference<SocialJig>
    {
        protected override DereferenceResult Perform(SocialJig reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "SimA", field, objects))
            {
                Remove(ref reference.SimA);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "SimB", field, objects))
            {
                Remove(ref reference.SimB);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
