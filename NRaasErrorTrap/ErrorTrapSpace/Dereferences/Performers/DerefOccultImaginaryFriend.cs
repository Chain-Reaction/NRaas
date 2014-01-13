using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefOccultImaginaryFriend : Dereference<OccultImaginaryFriend>
    {
        protected override DereferenceResult Perform(OccultImaginaryFriend reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimDescription", field, objects))
            {
                //Remove(ref reference.mSimDescription );
                return DereferenceResult.Ignore;
            }

            DereferenceResult reason = MatchAndRemove(reference, "mOnSelectedSimChangedListener", field, ref reference.mOnSelectedSimChangedListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                return reason;
            }

            reason = MatchAndRemove(reference, "mOnOwnerFallingAsleepListener", field, ref reference.mOnOwnerFallingAsleepListener, objects, DereferenceResult.End);
            if (reason != DereferenceResult.Failure)
            {
                return reason;
            }

            return DereferenceResult.Failure;
        }
    }
}
