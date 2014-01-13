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
    public class DerefPicnicSituation : Dereference<PicnicSituation>
    {
        protected override DereferenceResult Perform(PicnicSituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mBasket", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(ref reference.mBasket);
                }
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
