using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefMixPotion : Dereference<AlchemyStation.MixPotion>
    {
        protected override DereferenceResult Perform(AlchemyStation.MixPotion reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mCreatedPotion", field, objects))
            {
                if (Performing)
                {
                    Remove(ref reference.mCreatedPotion);
                    return DereferenceResult.End;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
