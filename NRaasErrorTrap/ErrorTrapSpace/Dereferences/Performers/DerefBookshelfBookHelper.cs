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
    public class DerefBookshelfBookHelper : Dereference<Bookshelf.BookHelper>
    {
        protected override DereferenceResult Perform(Bookshelf.BookHelper reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBook", field, objects))
            {
                Remove(ref reference.mBook);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
