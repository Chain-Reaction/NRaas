using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefRecycleBinOutdoor : Dereference<RecycleBinOutdoor>
    {
        protected override DereferenceResult Perform(RecycleBinOutdoor reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mRummagingSim", field, objects))
            {
                Remove(ref reference.mRummagingSim);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
