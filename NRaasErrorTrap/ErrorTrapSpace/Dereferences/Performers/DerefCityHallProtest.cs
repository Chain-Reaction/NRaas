using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefCityHallProtest : Dereference<CityHall.Protest>
    {
        protected override DereferenceResult Perform(CityHall.Protest reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSign", field, objects))
            {
                //Remove(ref reference.mSign);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
