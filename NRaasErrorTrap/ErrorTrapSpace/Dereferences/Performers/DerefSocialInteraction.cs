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
    public class DerefSocialInteraction : Dereference<SocialInteraction>
    {
        protected override DereferenceResult Perform(SocialInteraction reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            DereferenceResult reason = MatchAndRemove(reference, "mSleepingListener", field, ref reference.mSleepingListener, objects, DereferenceResult.ContinueIfReferenced);
            if (reason != DereferenceResult.Failure)
            {
                return reason;
            }

            if (Matches(reference, "SocialJig", field, objects))
            {
                Remove(ref reference.SocialJig);
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "mImpassableRegion", field, objects))
            {
                //Remove(ref reference.mImpassableRegion);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
