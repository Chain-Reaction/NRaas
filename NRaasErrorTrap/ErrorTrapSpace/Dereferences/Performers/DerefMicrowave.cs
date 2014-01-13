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
    public class DerefMicrowave : Dereference<Microwave>
    {
        protected override DereferenceResult Perform(Microwave reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mSimStateMachineClient", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mSimStateMachineClient.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mSimStateMachineClient);
                }
                return DereferenceResult.End;
            }

            if (Matches(reference, "mMicrowaveSelfStateMachineClient", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mMicrowaveSelfStateMachineClient.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mMicrowaveSelfStateMachineClient);
                }
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
