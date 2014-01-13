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
    public class DerefBakeSaleTable : Dereference<BakeSaleTable>
    {
        protected override DereferenceResult Perform(BakeSaleTable reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSeller", field, objects))
            {
                Remove(ref reference.mSeller);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
