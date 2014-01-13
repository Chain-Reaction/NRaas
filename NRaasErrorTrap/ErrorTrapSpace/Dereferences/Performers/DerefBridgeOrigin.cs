using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefBridgeOrigin : Dereference<BridgeOrigin>
    {
        protected override DereferenceResult Perform(BridgeOrigin reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mStateMachineClient", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mStateMachineClient.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mStateMachineClient);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            if (Matches(reference, "BridgeOriginUsed", field, objects))
            {
                //Remove(ref reference.BridgeOriginUsed);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
