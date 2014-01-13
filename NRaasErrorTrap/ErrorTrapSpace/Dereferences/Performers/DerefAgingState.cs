using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefAgingState : Dereference<AgingState>
    {
        protected override DereferenceResult Perform(AgingState reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "SimDescription", field, objects))
            {
                if (Performing)
                {
                    //Remove(ref reference.SimDescription);

                    if ((AgingManager.Singleton != null) && (AgingManager.Singleton.AgingStates != null))
                    {
                        AgingManager.Singleton.AgingStates.Remove(reference);
                    }
                }

                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
