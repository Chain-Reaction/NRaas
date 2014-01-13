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
    public class DerefStateMachineClientHandlerRecord : Dereference<StateMachineClient.HandlerRecord>
    {
        protected override DereferenceResult Perform(StateMachineClient.HandlerRecord reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "handler", field, objects))
            {
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
