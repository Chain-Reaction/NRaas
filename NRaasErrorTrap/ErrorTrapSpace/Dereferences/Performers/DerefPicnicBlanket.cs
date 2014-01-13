using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPicnicBlanket : Dereference<PicnicBlanket>
    {
        protected override DereferenceResult Perform(PicnicBlanket reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Basket", field, objects))
            {
                Remove(ref reference.Basket);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
