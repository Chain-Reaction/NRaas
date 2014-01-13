using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSocialInteractionA : Dereference<SocialInteractionA>
    {
        protected override DereferenceResult Perform(SocialInteractionA reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mEffects", field, objects))
            {
                RemoveKeys(reference.mEffects, objects);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
