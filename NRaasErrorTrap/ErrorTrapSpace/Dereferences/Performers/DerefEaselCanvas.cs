using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefEaselCanvas : Dereference<EaselCanvas>
    {
        protected override DereferenceResult Perform(EaselCanvas reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mEaselRef", field, objects))
            {
                Remove(ref reference.mEaselRef);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mPortraitSubject", field, objects))
            {
                Remove(ref reference.mPortraitSubject);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
