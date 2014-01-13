using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefWidget : Dereference<Widget>
    {
        protected override DereferenceResult Perform(Widget reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mInventor", field, objects))
            {
                Remove(ref reference.mInventor);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
