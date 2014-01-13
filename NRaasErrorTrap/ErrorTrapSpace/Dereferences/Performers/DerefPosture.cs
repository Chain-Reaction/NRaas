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
    public class DerefPosture : Dereference<Posture>
    {
        protected override DereferenceResult Perform(Posture reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "CurrentStateMachine", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.CurrentStateMachine.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.CurrentStateMachine);
                }
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
