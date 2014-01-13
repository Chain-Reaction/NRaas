using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefMedicalJournal : Dereference<MedicalJournal>
    {
        protected override DereferenceResult Perform(MedicalJournal reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mOwner", field, objects))
            {
                Remove(ref reference.mOwner);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
