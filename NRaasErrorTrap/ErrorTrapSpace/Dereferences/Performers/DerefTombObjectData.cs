using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefTombObjectData : Dereference<TombObjectData>
    {
        protected override DereferenceResult Perform(TombObjectData reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mOwner", field, objects))
            {
                Remove(ref reference.mOwner);
                return DereferenceResult.End;
            }

            if (Matches(reference, "TombObjectComponentChanged", field, objects))
            {
                RemoveDelegate<VoidEventHandler>(ref reference.TombObjectComponentChanged, objects);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
