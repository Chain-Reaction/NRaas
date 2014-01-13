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
    public class DerefBakeSaleTableBrowseOrPurchaseFromSim : Dereference<BakeSaleTable.BrowseOrPurchaseFromSim>
    {
        protected override DereferenceResult Perform(BakeSaleTable.BrowseOrPurchaseFromSim reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "BakedGood", field, objects))
            {
                Remove(ref reference.BakedGood);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
