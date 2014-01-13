using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Misc;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefAttractionGift : Dereference<AttractionGift>
    {
        protected override DereferenceResult Perform(AttractionGift reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimReceivingGift", field, objects))
            {
                //Remove(ref reference.mSimReceivingGift);

                ErrorTrap.AddToBeDeleted(reference, false);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
