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
    public class DerefCityHall : Dereference<CityHall>
    {
        protected override DereferenceResult Perform(CityHall reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mProtesters", field, objects))
            {
                Remove(reference.mProtesters, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mGatheringBroadcaster", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mGatheringBroadcaster.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mGatheringBroadcaster);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
