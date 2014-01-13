using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPickUpChild : Dereference<PickUpChild>
    {
        protected override DereferenceResult Perform(PickUpChild reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "SocialJig", field, objects))
            {
                //Remove(ref reference.SocialJig );
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
