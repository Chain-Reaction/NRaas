using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefPlant : Dereference<Plant>
    {
        protected override DereferenceResult Perform(Plant reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimsWhoHelpedGrow", field, objects))
            {
                Remove(reference.mSimsWhoHelpedGrow, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mDeadPlantBroadcast", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mDeadPlantBroadcast.Dispose();
                    }
                    catch
                    { }
                }

                Remove(ref reference.mDeadPlantBroadcast);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
