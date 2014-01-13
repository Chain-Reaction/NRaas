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
    public class DerefBook : Dereference<Book>
    {
        protected override DereferenceResult Perform(Book reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mMyShelf", field, objects))
            {
                Remove(ref reference.mMyShelf);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
