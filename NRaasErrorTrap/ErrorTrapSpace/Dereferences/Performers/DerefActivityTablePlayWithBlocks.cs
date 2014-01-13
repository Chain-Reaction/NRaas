using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Toys;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefActivityTablePlayWithBlocks : Dereference<ActivityTable.PlayWithBlocks>
    {
        protected override DereferenceResult Perform(ActivityTable.PlayWithBlocks reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "SingleBlock", field, objects))
            {
                //Remove(ref reference.SingleBlock);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "Blocks", field, objects))
            {
                //Remove(ref reference.Blocks);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
