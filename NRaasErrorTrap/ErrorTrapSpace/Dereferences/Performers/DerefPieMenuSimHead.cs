using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPieMenuSimHead : Dereference<PieMenuSimHead>
    {
        protected override DereferenceResult Perform(PieMenuSimHead reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mDisplayedSim", field, objects))
            {
                Remove(ref reference.mDisplayedSim);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mClonedDescription", field, objects))
            {
                Remove(ref reference.mClonedDescription);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
