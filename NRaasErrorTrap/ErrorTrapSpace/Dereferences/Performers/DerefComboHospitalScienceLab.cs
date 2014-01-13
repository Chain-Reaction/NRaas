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
    public class DerefComboHospitalScienceLab : Dereference<ComboHospitalScienceLab>
    {
        protected override DereferenceResult Perform(ComboHospitalScienceLab reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mScienceLab", field, objects))
            {
                if (Performing)
                {
                    Remove(ref reference.mScienceLab);
                    return DereferenceResult.End;
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
