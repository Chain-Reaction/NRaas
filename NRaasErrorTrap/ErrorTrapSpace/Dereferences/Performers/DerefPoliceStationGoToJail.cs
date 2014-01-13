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
    public class DerefPoliceStationGoToJail : Dereference<PoliceStation.GoToJail>
    {
        protected override DereferenceResult Perform(PoliceStation.GoToJail reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mInmatesObjects", field, objects))
            {
                if (Performing)
                {
                    Remove(ref reference.mInmatesObjects);
                    return DereferenceResult.End;
                }

                return DereferenceResult.Found;
            }

            return DereferenceResult.Failure;
        }
    }
}
