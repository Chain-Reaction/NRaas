using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPicnicBasketSituationCleanUpPicnic : Dereference<PicnicBasketSituation.CleanUpPicnic>
    {
        protected override DereferenceResult Perform(PicnicBasketSituation.CleanUpPicnic reference, FieldInfo field, List<ReferenceWrapper> objects)
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
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
