using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefImageObject : Dereference<ImageObject>
    {
        protected override DereferenceResult Perform(ImageObject reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPainterSimDescription", field, objects))
            {
                Remove(ref reference.mPainterSimDescription);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
