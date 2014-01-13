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
    public class DerefFridge_EatLeftoversDefinition : Dereference<Fridge_EatLeftovers.Definition>
    {
        protected override DereferenceResult Perform(Fridge_EatLeftovers.Definition reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "ChosenContainer", field, objects))
            {
                Remove(ref reference.ChosenContainer);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
