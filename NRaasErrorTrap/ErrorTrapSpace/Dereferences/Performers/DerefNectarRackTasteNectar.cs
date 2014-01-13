using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefNectarRackTasteNectar : Dereference<NectarRack.TasteNectar>
    {
        protected override DereferenceResult Perform(NectarRack.TasteNectar reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Bottle", field, objects))
            {
                Remove(ref reference.Bottle);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
