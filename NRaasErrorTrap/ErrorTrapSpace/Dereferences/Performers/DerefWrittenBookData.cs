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
    public class DerefWrittenBookData : Dereference<WrittenBookData>
    {
        protected override DereferenceResult Perform(WrittenBookData reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "BiographySubject", field, objects))
            {
                Remove(ref reference.BiographySubject);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
