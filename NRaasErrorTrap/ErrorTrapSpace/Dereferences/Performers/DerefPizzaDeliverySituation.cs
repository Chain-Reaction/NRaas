using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPizzaDeliverySituation : Dereference<PizzaDeliverySituation>
    {
        protected override DereferenceResult Perform(PizzaDeliverySituation reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "PizzaBox", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Exit();
                    }
                    catch
                    { }

                    Remove(ref reference.PizzaBox);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
