using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefFridge_EatLeftovers : Dereference<Fridge_EatLeftovers>
    {
        protected override DereferenceResult Perform(Fridge_EatLeftovers reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "ChosenContainer", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.Cleanup();
                    }
                    catch
                    { }

                    Remove(ref reference.ChosenContainer);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
