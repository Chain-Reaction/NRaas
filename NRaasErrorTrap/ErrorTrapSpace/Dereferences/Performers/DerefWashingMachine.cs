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
    public class DerefWashingMachine : Dereference<WashingMachine>
    {
        protected override DereferenceResult Perform(WashingMachine reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mWashCycleStateMachine", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.mWashCycleStateMachine.Dispose();
                    }
                    catch
                    { }

                    Remove(ref reference.mWashCycleStateMachine);
                }

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
