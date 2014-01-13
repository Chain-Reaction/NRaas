using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefConsignmentRegisterObjectData : Dereference<ConsignmentRegister.ConsignmentRegisterObjectData>
    {
        protected override DereferenceResult Perform(ConsignmentRegister.ConsignmentRegisterObjectData reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mObject", field, objects))
            {
                Remove(ref reference.mObject);
                if (Performing)
                {
                    return DereferenceResult.Continue;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
