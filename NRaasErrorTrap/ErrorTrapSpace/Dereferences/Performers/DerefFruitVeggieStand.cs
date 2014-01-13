using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefFruitVeggieStandFruitVeggieStandObjectData : DereferenceGeneric<object>
    {
        protected override bool Matches(string type)
        {
            return (type == "Sims3.Store.Objects.FruitVeggieStand+FruitVeggieStandObjectData");
        }

        protected override DereferenceResult Perform(object reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            FieldInfo actualField;
            ReferenceWrapper result;
            if (Matches(reference, "mObject", field, objects, out result, out actualField) != MatchResult.Failure)
            {
                /* Dereferencing the list containing the FruitVeggieStandObjectData is prohibitively difficult so don't bother
                if (result.Valid)
                {
                    Remove(actualField, ref reference);
                }
                return DereferenceResult.ContinueIfReferenced;
                */
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
