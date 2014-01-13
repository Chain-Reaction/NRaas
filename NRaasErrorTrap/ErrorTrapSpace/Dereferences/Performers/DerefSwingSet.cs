using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefSwingSet : Dereference<SwingSet>
    {
        protected override DereferenceResult Perform(SwingSet reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCachedLeftPartner", field, objects))
            {
                Remove(ref reference.mCachedLeftPartner);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mCachedRightPartner", field, objects))
            {
                Remove(ref reference.mCachedRightPartner);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
