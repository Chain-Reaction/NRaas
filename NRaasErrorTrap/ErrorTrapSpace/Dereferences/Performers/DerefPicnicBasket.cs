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
    public class DerefPicnicBasket : Dereference<PicnicBasket>
    {
        protected override DereferenceResult Perform(PicnicBasket reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "Blanket", field, objects))
            {
                Remove(ref reference.Blanket);
                return DereferenceResult.End;
            }

            if (Matches(reference, "OtherBasketForPlacement", field, objects))
            {
                Remove(ref reference.OtherBasketForPlacement );
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
